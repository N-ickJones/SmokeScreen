using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;

using static HotSpot.Modules.Common;
using static HotSpot.Modules.Cryptography;
using static HotSpot.Modules.Sql;

namespace HotSpot
{
    public class AsyncServer
    {
        public AsyncServer(TextBox textBox)
        {
            _logBox = textBox;
        }

        private readonly TextBox _logBox;
        private readonly ManualResetEvent serverTalk = new ManualResetEvent(false);
        private readonly List<ClientObject> Ring = new List<ClientObject>();
        private readonly string TransactionEndTag = "</transaction>";

        public void StartListening(string note)
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IpAddress, PortNumber);
                using (Socket server = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    server.Bind(ipEndPoint);
                    server.Listen(MaxPendingConnections);

                    while (true)
                    {
                        serverTalk.Reset();
                        Log($"{note}\nWaiting for a Connection...");
                        server.BeginAccept(new AsyncCallback(AcceptCallback), server);
                        serverTalk.WaitOne();
                    }
                }
            }
            catch
            {
                Log($"Server is now Disconnected...");
            }
        }

        public void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                
                serverTalk.Set();
                Socket reciever = (Socket)asyncResult.AsyncState;
                Socket worker = reciever.EndAccept(asyncResult);
                StateObject stateObject = new StateObject { workSocket = worker };
                worker.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), stateObject);
            }
            catch
            {

            }
        }

        public void ReadCallback(IAsyncResult asyncResult)
        {
            string transaction = string.Empty;

            try
            {
                StateObject stateObject = (StateObject)asyncResult.AsyncState;
                Socket worker = stateObject.workSocket;

                int byteInput = worker.EndReceive(asyncResult);

                if (byteInput > 0)
                {
                    stateObject.stringBuilder.Append(encoding.GetString(stateObject.buffer, 0, byteInput));

                    transaction = stateObject.stringBuilder.ToString();

                    if (transaction.IndexOf(TransactionEndTag) > -1)
                    {
                        Log($"Read {transaction.Length} bytes from socket. \n Data : {transaction}");
                        ProcessTransaction(worker, transaction);
                    }
                    else
                    {
                        worker.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), stateObject);
                    }
                }
            }
            catch
            {

            }
        }

        private void ProcessTransaction(Socket worker, string data)
        {
            MatchCollection matchCollection = transactionRegex.Matches(data);

            if (matchCollection.Count != 0)
            {
                GroupCollection groupCollection = matchCollection[0].Groups;

                string type = groupCollection[1].Value;
                string key = groupCollection[2].Value;
                string token = groupCollection[3].Value;
                Algorithm algorithm = ConvertStringToAlgorithm(groupCollection[4].Value);
                string content = groupCollection[5].Value;

                data = Transaction(type, key, token, algorithm, content);
            }
            else
            {
                data = InvalidTransaction("Incorrect Transaction Formatting");
            }

            Send(worker, data);
        }

        private void Send(Socket worker, string data)
        {
            byte[] byteArray = encoding.GetBytes(data);
            worker.BeginSend(byteArray, 0, byteArray.Length, 0, new AsyncCallback(SendCallback), worker);
        }

        private void SendCallback(IAsyncResult ayncResult)
        {
            try
            {
                Socket worker = (Socket)ayncResult.AsyncState;
                int byteCount = worker.EndSend(ayncResult);
                Console.WriteLine($"Sent {byteCount} bytes to client.");
                worker.Shutdown(SocketShutdown.Both);
                worker.Close();
            }
            catch
            {

            }
        }

        private string Transaction(string type, string publicKey, string token, Algorithm algorithm, string content)
        {
            if (type.ToLower() == "exchange")
            {
                content = Exchange(algorithm, publicKey);
            }
            else if (type.ToLower() == "createaccount")
            {
                content = CreateAccount(algorithm, publicKey, token, content);
            }
            else if (type.ToLower() == "authenticate")
            {
                content = Authenicate(algorithm, publicKey, token, content);
            }
            else if (type.ToLower() == "message")
            {
                content = Message(algorithm, publicKey, token, content);
            }
            else if (type.ToLower().Contains("sendfile"))
            {
                content = SendFile(algorithm, type, publicKey, token, content);
            }
            else if (type.ToLower() == "requestfile")
            {
                content = RequestFile(algorithm, publicKey, token, content);
            }
            else
            {
                content = InvalidTransaction("Incorrect Transaction Type");
            }

            return content;
        }

        private string Exchange(Algorithm algorithm, string key)
        {
            string serverPublicKey;

            using (ECDiffieHellmanCng server = new ECDiffieHellmanCng())
            {
                server.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                server.HashAlgorithm = CngAlgorithm.Sha256;
                serverPublicKey = Convert.ToBase64String(server.PublicKey.ToByteArray());
                byte[] clientPublicKey = Convert.FromBase64String(key);
                byte[] symKeyBytes = server.DeriveKeyMaterial(CngKey.Import(clientPublicKey, CngKeyBlobFormat.EccPublicBlob));
                AddToRing(key, Convert.ToBase64String(symKeyBytes));
            }

            return TransactionFormat("exchange", key, serverPublicKey, algorithm.ToString(), "");
        }

        private string CreateAccount(Algorithm algorithm, string publicKey, string token, string content)
        {
            string IV;

            string symmetricKey = GetSymmetricKey(publicKey, Ring);

            content = Decrypt(algorithm, symmetricKey, token, content);

            MatchCollection matchCollection = createAccountRegex.Matches(content);

            if (matchCollection.Count != 0)
            {
                GroupCollection groupCollection = matchCollection[0].Groups;

                string username = groupCollection[1].Value;
                string password = groupCollection[2].Value;

                if (username.Length >= 8)
                {
                    if (IsUsername(username) == false)
                    {
                        if (AddAccount(username, password))
                        {
                            ProcessKey(publicKey);
                            content = MessageFormat(algorithm, symmetricKey, "CreatedAccount", out IV);
                        }
                        else
                        {
                            content = MessageFormat(algorithm, symmetricKey, "FailedAccount", out IV);
                        }
                    }
                    else
                    {
                        content = MessageFormat(algorithm, symmetricKey, "InvalidUsernameExists", out IV);
                    }
                }
                else
                {
                    content = MessageFormat(algorithm, symmetricKey, "InvalidUsernameLength", out IV);
                }
            }
            else
            {
                content = MessageFormat(algorithm, symmetricKey, "InvalidFormatCreateAccount", out IV);
            }

            return TransactionFormat("createAccount", publicKey, IV, algorithm.ToString(), content);
        }

        private string Authenicate(Algorithm algorithm, string publicKey, string token, string content)
        {
            string IV;

            string symmetricKey = GetSymmetricKey(publicKey, Ring);

            content = Decrypt(algorithm, symmetricKey, token, content);

            MatchCollection matchCollection = authenicationRegex.Matches(content);

            if (matchCollection.Count != 0)
            {
                GroupCollection groupCollection = matchCollection[0].Groups;
                string username = groupCollection[1].Value;
                string password = groupCollection[2].Value;

                if (IsUsername(username))
                {
                    if (IsPassword(username, password))
                    {
                        ProcessKey(publicKey);
                        content = MessageFormat(algorithm, symmetricKey, "Authorized", out IV);
                    }
                    else
                    {
                        content = MessageFormat(algorithm, symmetricKey, "InvalidPassword", out IV);
                    }
                }
                else
                {
                    content = MessageFormat(algorithm, symmetricKey, "InvalidUsername", out IV);
                }
            }
            else
            {
                content = MessageFormat(algorithm, symmetricKey, "InvalidFormatAuthorize", out IV);
            }

            return TransactionFormat("authenication", publicKey, IV, algorithm.ToString(), content);
        }

        private string Message(Algorithm algorithm, string publicKey, string token, string content)
        {
            string IV;

            if (IsAuthorized(publicKey))
            {
                string symmetricKey = GetSymmetricKey(publicKey, Ring);
                content = Decrypt(algorithm, symmetricKey, token, content);
                MatchCollection matchCollection = messageRegex.Matches(content);

                if (matchCollection.Count != 0)
                {
                    GroupCollection groupCollection = matchCollection[0].Groups;
                    string message = groupCollection[1].Value;
                    Log($"Recieved '{message}' from client");
                    content = MessageFormat(algorithm, symmetricKey, "RecievedMessage", out IV);
                }
                else
                {
                    Log($"Invalid Message Format Recieved {content}");
                    content = MessageFormat(algorithm, symmetricKey, "InvalidFormatMessage", out IV);
                }
            }
            else
            {
                Log($"Unauthorized Request from {publicKey}");
                content = MessageFormat(algorithm, "", "NotAuthorized", out IV);
            }

            return TransactionFormat("message", publicKey, IV, algorithm.ToString(), content);
        }

        private string SendFile(Algorithm algorithm, string type, string publicKey, string token, string content)
        {
            string IV;

            if (IsAuthorized(publicKey))
            {
                string symmetricKey = GetSymmetricKey(publicKey, Ring);
                content = Decrypt(algorithm, symmetricKey, token, content);
                MatchCollection matchCollection = messageRegex.Matches(content);

                if (matchCollection.Count != 0)
                {
                    GroupCollection groupCollection = matchCollection[0].Groups;
                    string message = groupCollection[1].Value;
                    string filetype = type.Substring(8);

                    Log($"Recieved file {filetype} from client");

                    SaveFile(filetype, message);
                    content = MessageFormat(algorithm, symmetricKey, $"RecievedFile{filetype}", out IV);
                }
                else
                {
                    Log($"Invalid Message Format Recieved {content}");
                    content = MessageFormat(algorithm, symmetricKey, "InvalidFormatMessage", out IV);
                }
            }
            else
            {
                Log($"Unauthorized Request from {publicKey}");
                content = MessageFormat(algorithm, "", "NotAuthorized", out IV);
            }

            return TransactionFormat("message", publicKey, IV, algorithm.ToString(), content);
        }

        private string RequestFile(Algorithm algorithm, string publicKey, string token, string content)
        {
            string IV;

            if (IsAuthorized(publicKey))
            {
                string symmetricKey = GetSymmetricKey(publicKey, Ring);
                content = Decrypt(algorithm, symmetricKey, token, content);
                MatchCollection matchCollection = messageRegex.Matches(content);

                if (matchCollection.Count != 0)
                {
                    GroupCollection groupCollection = matchCollection[0].Groups;
                    string fileRequest = groupCollection[1].Value;
                    Log($"Sent File {fileRequest} to client");
                    content = MessageFormat(algorithm, symmetricKey, ReadFile(fileRequest), out IV);
                }
                else
                {
                    Log($"Invalid Message Format Recieved {content}");
                    content = MessageFormat(algorithm, symmetricKey, "InvalidFormatMessage", out IV);
                }
            }
            else
            {
                Log($"Unauthorized Request from {publicKey}");
                content = MessageFormat(algorithm, "", "NotAuthorized", out IV);
            }

            return TransactionFormat("message", publicKey, IV, algorithm.ToString(), content);
        }

        private string InvalidTransaction(string exception = "")
        {
            return TransactionFormat("exception", "", "", exception);
        }

        private string MessageFormat(Algorithm algorithm, string symmetricKey, string message, out string IV)
        {
            if (string.IsNullOrEmpty(symmetricKey))
            {
                IV = string.Empty;
                return "{{Message = 'InvalidKeyDetected'}}";
            }
            else
            {
                return Encrypt(algorithm, symmetricKey, "{{Message = '" + message + "'}}", out IV);
            }
        }

        private void AddToRing(string publicKey, string symmetricKey)
        {
            Ring.Add(new ClientObject(publicKey, symmetricKey));
        }

        private string GetSymmetricKey(string publicKey, List<ClientObject> Ring)
        {
            foreach (ClientObject client in Ring)
            {
                if (client.PublicKey == publicKey)
                {
                    return client.SymmetricKey;
                }
            }
            return string.Empty;
        }

        private string ProcessKey(string key)
        {
            bool truth = false;

            foreach (ClientObject client in Ring)
            {
                if (client.PublicKey == key)
                {
                    truth = true;
                    client.Authenticated = true;
                    Log($"Successfully Authenticated Key: {client.PublicKey}");
                    break;
                }
            }

            if (!truth)
                Log($"Warning unable to authenticate key. The ring was not updated.");

            return key;
        }

        private bool IsAuthorized(string key)
        {
            bool truth = false;

            foreach (ClientObject client in Ring)
            {
                if (client.PublicKey == key)
                {
                    truth = true;
                    break;
                }
            }
            return truth;
        }

        private void SaveFile(string filename, string data)
        {

            Files file = ConvertStringToFiles(filename);

            if (file == Files.Sales)
            {
                File.WriteAllText(@"..\..\Static\FilesIn\Sales.txt", data);
            }
            else if (file == Files.Maps)
            {
                File.WriteAllText(@"..\..\Static\FilesIn\Maps.txt", data);
            }
            else if (file == Files.Budget)
            {
                File.WriteAllText(@"..\..\Static\FilesIn\Budget.txt", data);
            }
            else
            {
                File.WriteAllText(@"..\..\Static\FilesIn\Error.txt", data);
            }
        }

        private string ReadFile(string filename)
        {
            Files file = ConvertStringToFiles(filename);

            string fileContent;
            if (file == Files.Sales)
            {
                fileContent = File.ReadAllText(@"..\..\Static\FilesOut\Sales.txt");
            }
            else if (file == Files.Maps)
            {
                fileContent = File.ReadAllText(@"..\..\Static\FilesOut\Maps.txt");
            }
            else if (file == Files.Budget)
            {
                fileContent = File.ReadAllText(@"..\..\Static\FilesOut\Budget.txt");
            }
            else
            {
                fileContent = File.ReadAllText(@"..\..\Static\FilesOut\Error.txt");
            }
            return fileContent;
        }


        private void Log(string text)
        {
            _logBox.Dispatcher.Invoke(() =>
            {
                _logBox.Text = text;
            });
            Console.WriteLine(text);
        }
    }
}
