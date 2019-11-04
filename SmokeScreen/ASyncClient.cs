using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Text.RegularExpressions;

using static SmokeScreen.Modules.Cryptography;
using static SmokeScreen.Modules.Common;
using System.Windows.Forms;

namespace SmokeScreen
{
    public class AsynchronousClient
    {
        public AsynchronousClient()
        {

        }

        private delegate void InvokeDelegate();
        private TextBox _logBox;
        private readonly ManualResetEvent getConnected = new ManualResetEvent(false);
        private readonly ManualResetEvent getSented = new ManualResetEvent(false);
        private readonly ManualResetEvent getReceived = new ManualResetEvent(false);
        private string response = string.Empty;

        public string SymmetricKeyExchange(Algorithm alg, out string initialPublicKey)
        {
            string symmetricKey;

            try
            {
                using (Socket client = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.BeginConnect(new IPEndPoint(IpAddress, PortNumber), new AsyncCallback(ConnectCallback), client);
                    getConnected.WaitOne();

                    using (ECDiffieHellmanCng initializor = new ECDiffieHellmanCng())
                    {
                        initializor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                        initializor.HashAlgorithm = CngAlgorithm.Sha256;
                        initialPublicKey = Convert.ToBase64String(initializor.PublicKey.ToByteArray());
                        Send(client, TransactionFormat("exchange", initialPublicKey, "", alg.ToString(), ""));
                        getSented.WaitOne();

                        Receive(client, false);
                        getReceived.WaitOne();

                        Log(response);

                        MatchCollection matchCollection = transactionRegex.Matches(response);
                        if (matchCollection.Count != 0)
                        {
                            GroupCollection groupCollection = matchCollection[0].Groups;
                            byte[] serverPublicKey = Convert.FromBase64String(groupCollection[3].Value);
                            byte[] symKeyBytes = initializor.DeriveKeyMaterial(CngKey.Import(serverPublicKey, CngKeyBlobFormat.EccPublicBlob));
                            symmetricKey = Convert.ToBase64String(symKeyBytes);
                        }
                        else
                        {
                            symmetricKey = "";
                        }

                        Log($"symmetricKey = {symmetricKey}");

                    }
                    client.Shutdown(SocketShutdown.Both);
                }

                getConnected.Reset();
                getSented.Reset();
                getReceived.Reset();

                return symmetricKey;
            }
            catch (Exception exception)
            {
                Log(exception.Message);
                initialPublicKey = string.Empty;
                return string.Empty;
            }
        }

        public int CreateAccount(Algorithm algorithm, string symmetricKey, string publicKey, string username, string password)
        {
            int truth = -1;
            try
            {
                using (Socket client = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.BeginConnect(new IPEndPoint(IpAddress, PortNumber), new AsyncCallback(ConnectCallback), client);
                    getConnected.WaitOne();

                    string content = CreateAccountFormat(algorithm, symmetricKey, username, password, out string IV);

                    Send(client, TransactionFormat("createAccount", publicKey, IV, algorithm.ToString(), content));
                    getSented.WaitOne();

                    Receive(client, true, symmetricKey);
                    getReceived.WaitOne();
                    Log(response);

                    client.Shutdown(SocketShutdown.Both);
                }

                truth = IsAccountCreated(response);

                getConnected.Reset();
                getSented.Reset();
                getReceived.Reset();
            }
            catch (Exception exception)
            {
                Log(exception.Message);
                return truth;
            }

            return truth;
        }

        public int Authenicate(Algorithm algorithm, string symmetricKey, string publicKey, string username, string password)
        {
            int truth = -1;
            try
            {
                using (Socket client = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.BeginConnect(new IPEndPoint(IpAddress, PortNumber), new AsyncCallback(ConnectCallback), client);
                    getConnected.WaitOne();

                    string content = AuthenicateFormat(algorithm, symmetricKey, username, password, out string IV);

                    Send(client, TransactionFormat("authenticate", publicKey, IV, algorithm.ToString(), content));
                    getSented.WaitOne();

                    Receive(client, true, symmetricKey);
                    getReceived.WaitOne();

                    Log(GetMessage(response));

                    client.Shutdown(SocketShutdown.Both);
                }

                truth = IsAuthenticated(response);

                getConnected.Reset();
                getSented.Reset();
                getReceived.Reset();
            }
            catch (Exception exception)
            {
                Log(exception.Message);
                return truth;
            }

            return truth;
        }

        public void SendMessage(Algorithm algorithm, string symmetricKey, string publicKey, string message)
        {
            string truth = string.Empty;
            try
            {
                using (Socket client = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.BeginConnect(new IPEndPoint(IpAddress, PortNumber), new AsyncCallback(ConnectCallback), client);
                    getConnected.WaitOne();

                    string content = MessageFormat(algorithm, symmetricKey, message, out string IV);

                    Send(client, TransactionFormat("message", publicKey, IV, algorithm.ToString(), content));
                    getSented.WaitOne();

                    Receive(client, true, symmetricKey);
                    getReceived.WaitOne();

                    Log(GetMessage(response));

                    client.Shutdown(SocketShutdown.Both);
                }
                getConnected.Reset();
                getSented.Reset();
                getReceived.Reset();
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }
        }

        public void SetLogBox(TextBox logBox)
        {
            _logBox = logBox;
        }

        public void SendFile(Algorithm algorithm, Files file, string symmetricKey, string publicKey)
        {
            string logMessage;

            try
            {
                using (Socket client = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.BeginConnect(new IPEndPoint(IpAddress, PortNumber), new AsyncCallback(ConnectCallback), client);
                    getConnected.WaitOne();

                    string fileContent = ReadFile(file);

                    string content = MessageFormat(algorithm, symmetricKey, fileContent, out string IV);

                    Log(content);

                    Send(client, TransactionFormat($"sendfile{file.ToString()}", publicKey, IV, algorithm.ToString(), content));
                    getSented.WaitOne();

                    Receive(client, true, symmetricKey);
                    getReceived.WaitOne();

                    MatchCollection matchCollection = messageRegex.Matches(response);
                    if (matchCollection.Count != 0)
                    {
                        GroupCollection groupCollection = matchCollection[0].Groups;

                        string fileInput = groupCollection[1].Value;

                        logMessage = $"Recieved file {file.ToString()}.txt from client";

                        SaveFile(file, fileInput);
                    }
                    else
                    {
                        logMessage = "Unable to Access the selected file.";
                    }

                    Log($"{logMessage} {GetMessage(response)}");

                    client.Shutdown(SocketShutdown.Both);
                }
                getConnected.Reset();
                getSented.Reset();
                getReceived.Reset();
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }
        }

        public void RequestFile(Algorithm algorithm, Files file, string symmetricKey, string publicKey)
        {
            string logMessage;
            try
            {
                using (Socket client = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.BeginConnect(new IPEndPoint(IpAddress, PortNumber), new AsyncCallback(ConnectCallback), client);
                    getConnected.WaitOne();

                    string fileRequest = MessageFormat(algorithm, symmetricKey, file.ToString(), out string IV);

                    Send(client, TransactionFormat($"requestfile", publicKey, IV, algorithm.ToString(), fileRequest));
                    getSented.WaitOne();

                    Receive(client, true, symmetricKey);
                    getReceived.WaitOne();

                    MatchCollection matchCollection = messageRegex.Matches(response);
                    if (matchCollection.Count != 0)
                    {
                        GroupCollection groupCollection = matchCollection[0].Groups;

                        string fileInput = groupCollection[1].Value;

                        logMessage = $"Recieved file {file.ToString()} from client";

                        SaveFile(file, fileInput);
                    }
                    else
                    {
                        logMessage = "Unable to Access the selected file.";
                    }

                    Log($"{logMessage} {GetMessage(response)}");

                    client.Shutdown(SocketShutdown.Both);
                }
                getConnected.Reset();
                getSented.Reset();
                getReceived.Reset();
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }
        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket client = (Socket)asyncResult.AsyncState;
                client.EndConnect(asyncResult);
                Log($"Socket connected to {client.RemoteEndPoint.ToString()}");
                getConnected.Set();
            }
            catch (Exception exception)
            {
                Log(exception.Message);
                getConnected.Set();
            }
        }

        private void Send(Socket worker, string data)
        {
            byte[] byteArray = encoding.GetBytes(data);
            worker.BeginSend(byteArray, 0, byteArray.Length, 0, new AsyncCallback(SendCallback), worker);
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket worker = (Socket)asyncResult.AsyncState;
                int byteCount = worker.EndSend(asyncResult);
                Console.WriteLine($"Recieved {byteCount.ToString()} bytes from server.");
                getSented.Set();
            }
            catch (Exception exception)
            {
                Log(exception.Message);
                getSented.Set();
            }
        }

        private void Receive(Socket worker, bool decrypt, string symmetricKey = "")
        {
            try
            {
                StateObject stateObject = new StateObject { workSocket = worker, decrypt = decrypt, symmetricKey = symmetricKey };
                worker.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), stateObject);
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult aysncResult)
        {
            try
            {
                StateObject stateObject = (StateObject)aysncResult.AsyncState;
                Socket worker = stateObject.workSocket;

                int byteCount = worker.EndReceive(aysncResult);

                if (byteCount > 0)
                {
                    stateObject.stringBuilder.Append(encoding.GetString(stateObject.buffer, 0, byteCount));
                    worker.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), stateObject);
                }
                else
                {
                    if (stateObject.stringBuilder.Length > 1)
                    {
                        if (stateObject.decrypt)
                        {
                            MatchCollection matchCollection = transactionRegex.Matches(stateObject.stringBuilder.ToString());
                            if (matchCollection.Count != 0)
                            {
                                GroupCollection groupCollection = matchCollection[0].Groups;

                                string token = groupCollection[3].Value;
                                string algorithm = groupCollection[4].Value;
                                string content = groupCollection[5].Value;

                                response = Decrypt(ConvertStringToAlgorithm(algorithm), stateObject.symmetricKey, token, content);
                            }
                            else
                            {
                                response = stateObject.stringBuilder.ToString();
                            }
                        }
                        else
                        {
                            response = stateObject.stringBuilder.ToString();
                        }
                    }
                    getReceived.Set();
                }
            }
            catch (Exception exception)
            {
                Log(exception.Message);
                getReceived.Set();
            }
        }

        public Transaction GetTransaction(string data)
        {
            MatchCollection matchCollection = transactionRegex.Matches(data);

            if (matchCollection.Count != 0)
            {
                GroupCollection groupCollection = matchCollection[0].Groups;
                return new Transaction(groupCollection[1].Value, groupCollection[2].Value, "", groupCollection[3].Value);
            }

            return new Transaction();
        }

        public class Transaction
        {
            public Transaction()
            {
                Type = string.Empty;
                Key = string.Empty;
                Token = string.Empty;
                Content = string.Empty;
            }

            public Transaction(string type, string key, string token, string content)
            {
                Type = type;
                Key = key;
                Token = token;
                Content = content;
            }

            public string Type { get; }

            public string Key { get; }

            public string Token { get; }

            public string Content { get; }

        }

        private int IsAuthenticated(string content)
        {
            MatchCollection matchCollection = messageRegex.Matches(content);

            if (matchCollection.Count != 0)
            {
                GroupCollection groupCollection = matchCollection[0].Groups;
                string reply = groupCollection[1].Value;
                if (reply == "Authorized")
                    return 1;
                else if (reply == "InvalidUsername")
                    return 2;
                else if (reply == "InvalidPassword")
                    return 3;
                else if (reply == "InvalidFormatAuthorize")
                    return 4;
            }
            return 0;
        }

        private int IsAccountCreated(string content)
        {
            MatchCollection matchCollection = messageRegex.Matches(content);

            if (matchCollection.Count != 0)
            {
                GroupCollection groupCollection = matchCollection[0].Groups;
                string reply = groupCollection[1].Value;

                if (reply == "CreatedAccount")
                    return 1;
                else if (reply == "InvalidUsernameLength")
                    return 2;
                else if (reply == "InvalidUsernameExists")
                    return 3;
                else if (reply == "FailedAccount")
                    return 4;
                else if (reply == "InvalidFormatCreateAccount")
                    return 5;
            }
            return 0;
        }

        private string GetMessage(string content)
        {
            MatchCollection matchCollection = messageRegex.Matches(content);

            if (matchCollection.Count != 0)
            {
                GroupCollection groupCollection = matchCollection[0].Groups;
                return groupCollection[1].Value;
            }
            else
            {
                return "Unable to Read Message. This could be due to tampering across the web or exceptions.";
            }
        }

        private string AuthenicateFormat(Algorithm algorithm, string key, string username, string password, out string IV)
        {
            return Encrypt(algorithm, key, "{{Username='" + username + "'}{Password='" + password + "'}}", out IV);
        }

        private string CreateAccountFormat(Algorithm algorithm, string key, string username, string password, out string IV)
        {
            return Encrypt(algorithm, key, "{{Username='" + username + "'}{Password='" + password + "'}}", out IV);
        }

        private string MessageFormat(Algorithm algorithm, string symmetricKey, string message, out string IV)
        {
            return Encrypt(algorithm, symmetricKey, "{{Message = '" + message + "'}}", out IV);
        }

        private string ReadFile(Files file)
        {
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

        private void SaveFile(Files file, string data)
        {
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

        private void Log(string text)
        {
            if (_logBox != null)
            {
                _logBox.BeginInvoke(new InvokeDelegate(() => _logBox.Text = text));
            }
            Console.WriteLine(text);
        }

    }

}
