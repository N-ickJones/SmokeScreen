using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SmokeScreen.Modules.Cryptography;
using static SmokeScreen.Modules.Common;

namespace SmokeScreen
{
    public partial class MainForm : Form
    {
        private readonly string Username;
        private readonly string SymmetricKey;
        private readonly string PublicKey;
        private readonly Algorithm _Algorithm;

        private readonly AsynchronousClient asyncClient;

        public MainForm(AsynchronousClient _asyncClient, Algorithm algorithm, string username, string symmetricKey, string publicKey)
        {
            InitializeComponent();
            retrieveFileBox.DataSource = Enum.GetValues(typeof(Files));
            sendFileBox.DataSource = Enum.GetValues(typeof(Files));
            _Algorithm = algorithm;
            Username = username;
            SymmetricKey = symmetricKey;
            PublicKey = publicKey;
            Text = $"SmokeScreen {Username}";
            asyncClient = _asyncClient;
            asyncClient.SetLogBox(inBox);
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            asyncClient.SendMessage(_Algorithm, SymmetricKey, PublicKey, outBox.Text);
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            using (AuthForm authForm = new AuthForm())
            {
                Hide();
                authForm.ShowDialog();
                Close();
            }
        }

        private void SendFileBtn_Click(object sender, EventArgs e)
        {
            asyncClient.SendFile(_Algorithm, (Files)sendFileBox.SelectedItem, SymmetricKey, PublicKey);
        }

        private void RetrieveFileBtn_Click(object sender, EventArgs e)
        {
            asyncClient.RequestFile(_Algorithm, (Files)retrieveFileBox.SelectedItem, SymmetricKey, PublicKey);
        }
    }
}
