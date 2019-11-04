using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using static SmokeScreen.Modules.Cryptography;

namespace SmokeScreen.Modules
{
    public class Common
    {
        public static readonly int PortNumber = 11000;
        public static readonly int MaxPendingConnections = 100;
        public static readonly IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
        public static readonly Encoding encoding = Encoding.Unicode;

        // Group 0 is all, Group 1 is type, Group 2 is key, Group 3 is token, Group 4 is content
        public static readonly Regex transactionRegex1 = new Regex(
            @"<transaction\s+type='([\w\s]*)'\s+key='(.*)'\s+token='(.*)'\s*>(.*)<\/transaction>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Group 0 is all, Group 1 is type, Group 2 is key, Group 3 is token, Group 4 is alg, Group 5 is content
        public static readonly Regex transactionRegex = new Regex(
            @"<transaction\s+type='([\w\s]*)'\s+key='(.*)'\s+token='(.*)'\s+alg='([\w\s]*)'\s*>(.*)<\/transaction>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Group 0 is all, Group 1 is username, Group 2 is password
        public static readonly Regex createAccountRegex = new Regex(
            @"\s*\{\s*\{\s*Username\s*=\s*'(.*)'\s*.*\}\s*\{\s*Password\s*=\s*'(.*)'\s*\}\s*\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Group 0 is all, Group 1 is username, Group 2 is password
        public static readonly Regex authenicationRegex = new Regex(
            @"\s*\{\s*\{\s*Username\s*=\s*'(.*)'\s*.*\}\s*\{\s*Password\s*=\s*'(.*)'\s*\}\s*\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Group 0 is all, Group 1 is message
        public static readonly Regex messageRegex = new Regex(
            @"\s*\{\s*\{\s*Message\s*=\s*'(.*)'\s*.*\}\s*\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Client Specific Data Class
        /// </summary>
        public class ClientObject
        {
            public ClientObject(string publicKey, string symmetricKey)
            {
                PublicKey = publicKey;
                SymmetricKey = symmetricKey;
            }

            public bool Authenticated = false;

            public string PublicKey { get; }

            public string SymmetricKey { get; }

        }

        public class StateObject
        {
            public const int BufferSize = 256;

            public byte[] buffer = new byte[BufferSize];

            public Socket workSocket = null;

            public StringBuilder stringBuilder = new StringBuilder();

            public bool decrypt = false;

            public string symmetricKey = string.Empty;
        }

        public static string TransactionFormat(string type, string key = "", string token = "", string alg = "", string content = "")
        {
            return $"<transaction type='{type}' key='{key}' token='{token}' alg='{alg}'>{content}</transaction>";
        }

        public static string Decrypt(Algorithm algorithm, string symmetricKey, string token, string content)
        {
            if (algorithm == Algorithm.AES)
            {
                content = AES.Decrypt(symmetricKey, content, token);
            }
            else if (algorithm == Algorithm.RIJ)
            {
                content = RIJ.Decrypt(symmetricKey, content, token);
            }
            else if (algorithm == Algorithm.DES)
            {
                content = TDES.Decrypt(symmetricKey, content, token);
            }
            else
            {
                content = string.Empty;
            }
            return content;
        }

        public static string Encrypt(Algorithm algorithm, string symmetricKey, string message, out string IV)
        {
            if (algorithm == Algorithm.AES)
            {
                return AES.Encrypt(symmetricKey, message, out IV);
            }
            else if (algorithm == Algorithm.RIJ)
            {
                return RIJ.Encrypt(symmetricKey, message, out IV);
            }
            else if (algorithm == Algorithm.DES)
            {
                return TDES.Encrypt(symmetricKey, message, out IV);
            }
            else
            {
                IV = string.Empty;
                return "Algorithm Issue";
            }
        }

        public static Algorithm ConvertStringToAlgorithm(string algorithm)
        {
            try
            {
                Enum.TryParse(algorithm, out Algorithm selectedAlgorithm);
                return selectedAlgorithm;
            }
            catch
            {
                return Algorithm.AES;
            }
        }

        public enum Files
        {
            Sales = 0,
            Maps = 1,
            Budget = 2
        }

    }
}
