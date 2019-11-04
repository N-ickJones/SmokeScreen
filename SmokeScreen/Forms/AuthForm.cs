using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using static SmokeScreen.Modules.Cryptography;

/* TODO
 * OneWay Hash Password - Complete
 * Login user / pass    - Complete
 * store in array/DB    - Complete
 * Diffee/Hellemen exc  - Complete <- Check SKIP 1024 bits
 * 3 encryptions algor  - Complete
 * File Transfer 2way   - TODO
 */


namespace SmokeScreen
{
    public partial class AuthForm : Form
    {
        public static string SymmetricKey = string.Empty;
        public static string PublicKey = string.Empty;

        private static readonly Color Default = Color.White;
        private static readonly Color Warning = Color.FromArgb(245, 144, 66);

        private readonly AsynchronousClient asyncClient;

        public AuthForm()
        {
            InitializeComponent();
            algBox.DataSource = Enum.GetValues(typeof(Algorithm));
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            TransparentBackColor(
                authenicationLabel,
                passwordLabel,
                usernameLabel,
                usernameError,
                passwordError,
                exceptionError
            );
            signinButton.BackColor = Color.Azure;
            asyncClient = new AsynchronousClient();
        }

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            ClearErrors(usernameError, passwordError, exceptionError);
            ClearErrors(usernameInput, passwordInput);
            GetSymmetricKey();

            bool isInvalid = false;

            if (usernameInput.Text.Length < 8)
            {
                usernameError.Text = "Username must be 8 chars in length";
                usernameInput.BackColor = Warning;
                isInvalid = true;
            }
            if (passwordInput.Text.Length < 8)
            {
                passwordError.Text = "Password must be 8 chars in length";
                passwordInput.BackColor = Warning;
                isInvalid = true;
            }
            if (isInvalid)
            {
                exceptionError.Text = $"Unable to Create an Account... Invalid Input Detected";
            }
            else
            {
                int truth = asyncClient.CreateAccount((Algorithm)algBox.SelectedItem, SymmetricKey, PublicKey, usernameInput.Text, HashPassword(passwordInput.Text));


                if (truth == 1)
                {
                    usernameInput.Text = string.Empty;
                    passwordInput.Text = string.Empty;
                    exceptionError.Text = $"Account Created, You may now authenicate...";
                }
                else if (truth == 2)
                {
                    usernameError.Text = "Username must be 8 chars in length";
                    usernameInput.BackColor = Warning;
                }
                else if (truth == 3)
                {
                    usernameError.Text = "Username already Exists";
                    usernameInput.BackColor = Warning;
                }
                else if (truth == 4)
                    exceptionError.Text = $"Unable to Create an Account... Database Error";
                else if (truth == 5)
                    exceptionError.Text = $"Unable to Create an Account... Transaction Error";
                else
                {
                    exceptionError.Text = $"Unable to Create an Account... Exception Occurred";
                }
            }

        }

        private void SigninButton_Click(object sender, EventArgs e)
        {
            try
            {
                ClearErrors(usernameError, passwordError, exceptionError);
                ClearErrors(usernameInput, passwordInput);
                GetSymmetricKey();

                int truth = asyncClient.Authenicate((Algorithm)algBox.SelectedItem, SymmetricKey, PublicKey, usernameInput.Text, HashPassword(passwordInput.Text));

                if (truth == 1)
                {
                    LogIn((Algorithm)algBox.SelectedItem, usernameInput.Text, SymmetricKey, PublicKey);
                }
                else if (truth == 2)
                {
                    usernameError.Text = "Invalid Username";
                    usernameInput.BackColor = Warning;
                }
                else if (truth == 3)
                {
                    passwordError.Text = "Invalid Password";
                    passwordInput.BackColor = Warning;
                }
                else if (truth == 4)
                    exceptionError.Text = $"A Connection Issue has occured... Transaction Error";
                else
                {
                    exceptionError.Text = $"A Connection Issue has occured... Exception Occured";

                }
            }
            catch(Exception ex)
            {
                exceptionError.Text = ex.Message;
            }

        }

        private void LogIn(Algorithm algorithm, string username, string symmetricKey, string publicKey)
        {
            using (MainForm mainForm = new MainForm(asyncClient, algorithm, username, symmetricKey, publicKey))
            {
                Hide();
                mainForm.ShowDialog();
                Close();
            }
        }

        private static void ClearErrors(params Label[] list)
        {
            foreach (Label label in list)
            {
                label.Text = "";
            }
        }

        private static void ClearErrors(params TextBox[] list)
        {
            foreach (TextBox textBox in list)
            {
                textBox.BackColor = Default;
            }
        }

        private static void TransparentBackColor(params Label[] list)
        {
            foreach(Label label in list)
            {
                label.BackColor = Color.Transparent;
                label.ForeColor = Color.Azure;
            }
        }

        private void GetSymmetricKey()
        {
            if (SymmetricKey == string.Empty)
            {
                SymmetricKey = asyncClient.SymmetricKeyExchange((Algorithm)algBox.SelectedItem, out PublicKey);
            }
            if (SymmetricKey == null)
            {
                throw new Exception("Cannot Communicate With the Server... Invalid Symmmetric Key");
            }
        }

        private string HashPassword(string password)
        {
            if (password != string.Empty)
            {
                password = Sha256Hash.Generate(passwordInput.Text);
                return password;
            }
            return string.Empty;
        }
        
    }

}

//NOTE: You should be able to dynamically add new user/password combinations during the demonstration

/*
Once users are authenticated, they wish to be able to exchange an arbitrary number of communications with the manufacturer using symmetric encryption.  The following protocol has been agreed upon to determine an encryption algorithm and secret key.  You should be able to dynamically add new user/password combinations during the demonstration.

Protocol:
1)	Using a socket, customers contact the server (manufacturer) and login in for validation using their account name and password.  
2)	Upon validation, the manufacturer and customer determine a secret session key via a two part process.  First they determine an initial secret key based on the work of Diffie-Helleman.  It has been decided to use the SKIP protocol with a 1024 bit key.  You may use a different protocol than Diffie-Helleman with permission from the instructor.  Once the initial key has been determined, you may utilize hashing or another technique of your choice to reduce it to a size appropriate (number of bytes) to initialize the symmetric key encryption algorithm.  The rest of the communications are completed using symmetric encryption.
3)	The client should determine the symmetric algorithm to be used and communicate their choice to the server.  The server should support at least three symmetric encryption Algorithm.  You are free to take advantage of code from examples used in class as long as you document the source (do not plagiarize).  You may not use code from other sources!

Implement the client and server.  Both client and server should print sufficient material to show they have:
1)	Properly completed the login protocol.
2)	Generated the same initial key using SKIP (or alternate technique).
3)	The client must encrypt a file and transmit it to the server.  The server must then decrypt the file.  You must convenience me this communication has been accomplished during a live demonstration.
4)	The server must encrypt a file (with different contents) and transmit it to the client.  The client must then decrypt the file.  You must convenience me this communication has been accomplished during a live demonstration.
*/
