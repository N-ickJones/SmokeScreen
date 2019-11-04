using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Controls;

namespace HotSpot.Modules
{
    /// <summary>
    /// Interface For Logging Results of Client/Server Operations...
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// Client Generated Log Messages
        /// </summary>
        public class ClientLog
        {
            public ClientLog(TextBox logBox)
            {
                _logBox = logBox;
            }

            private readonly TextBox _logBox;

            public void ResponseRecieved(string response)
            {
                _logBox.Text = $"Response received : {response}";
            }

            public void ConnectionSuccess(Socket client)
            {
                _logBox.Text = $"Socket connected to {client.RemoteEndPoint.ToString()}";
            }

            public void Issue(Exception exception)
            {
                _logBox.Text = $"{exception}";
            }

            public void ByteCount(int length)
            {
                _logBox.Text = $"Sent {length} bytes to client.";
            }

        }
        
    }
}
