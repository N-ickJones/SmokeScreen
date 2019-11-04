namespace SmokeScreen
{
    partial class AuthForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthForm));
            this.usernameInput = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordInput = new System.Windows.Forms.TextBox();
            this.signinButton = new System.Windows.Forms.Button();
            this.authenicationLabel = new System.Windows.Forms.Label();
            this.usernameError = new System.Windows.Forms.Label();
            this.passwordError = new System.Windows.Forms.Label();
            this.exceptionError = new System.Windows.Forms.Label();
            this.createAccountButton = new System.Windows.Forms.Button();
            this.algBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // usernameInput
            // 
            this.usernameInput.Location = new System.Drawing.Point(304, 193);
            this.usernameInput.Name = "usernameInput";
            this.usernameInput.Size = new System.Drawing.Size(191, 20);
            this.usernameInput.TabIndex = 0;
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameLabel.Location = new System.Drawing.Point(304, 170);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(83, 20);
            this.usernameLabel.TabIndex = 1;
            this.usernameLabel.Text = "Username";
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordLabel.Location = new System.Drawing.Point(306, 223);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(78, 20);
            this.passwordLabel.TabIndex = 3;
            this.passwordLabel.Text = "Password";
            // 
            // passwordInput
            // 
            this.passwordInput.Location = new System.Drawing.Point(304, 246);
            this.passwordInput.Name = "passwordInput";
            this.passwordInput.Size = new System.Drawing.Size(191, 20);
            this.passwordInput.TabIndex = 2;
            // 
            // signinButton
            // 
            this.signinButton.BackColor = System.Drawing.SystemColors.Window;
            this.signinButton.Location = new System.Drawing.Point(339, 272);
            this.signinButton.Name = "signinButton";
            this.signinButton.Size = new System.Drawing.Size(110, 23);
            this.signinButton.TabIndex = 4;
            this.signinButton.Text = "SignIn";
            this.signinButton.UseVisualStyleBackColor = false;
            this.signinButton.Click += new System.EventHandler(this.SigninButton_Click);
            // 
            // authenicationLabel
            // 
            this.authenicationLabel.AutoSize = true;
            this.authenicationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authenicationLabel.Location = new System.Drawing.Point(304, 120);
            this.authenicationLabel.Name = "authenicationLabel";
            this.authenicationLabel.Size = new System.Drawing.Size(191, 33);
            this.authenicationLabel.TabIndex = 5;
            this.authenicationLabel.Text = "Authenication";
            // 
            // usernameError
            // 
            this.usernameError.AutoSize = true;
            this.usernameError.Location = new System.Drawing.Point(393, 175);
            this.usernameError.Name = "usernameError";
            this.usernameError.Size = new System.Drawing.Size(0, 13);
            this.usernameError.TabIndex = 6;
            // 
            // passwordError
            // 
            this.passwordError.AutoSize = true;
            this.passwordError.Location = new System.Drawing.Point(393, 228);
            this.passwordError.Name = "passwordError";
            this.passwordError.Size = new System.Drawing.Size(0, 13);
            this.passwordError.TabIndex = 7;
            // 
            // exceptionError
            // 
            this.exceptionError.AutoSize = true;
            this.exceptionError.Location = new System.Drawing.Point(307, 340);
            this.exceptionError.Name = "exceptionError";
            this.exceptionError.Size = new System.Drawing.Size(0, 13);
            this.exceptionError.TabIndex = 8;
            // 
            // createAccountButton
            // 
            this.createAccountButton.BackColor = System.Drawing.SystemColors.Window;
            this.createAccountButton.Location = new System.Drawing.Point(339, 301);
            this.createAccountButton.Name = "createAccountButton";
            this.createAccountButton.Size = new System.Drawing.Size(110, 23);
            this.createAccountButton.TabIndex = 9;
            this.createAccountButton.Text = "Create Account";
            this.createAccountButton.UseVisualStyleBackColor = false;
            this.createAccountButton.Click += new System.EventHandler(this.CreateAccountButton_Click);
            // 
            // algBox
            // 
            this.algBox.FormattingEnabled = true;
            this.algBox.Location = new System.Drawing.Point(104, 175);
            this.algBox.Name = "algBox";
            this.algBox.Size = new System.Drawing.Size(121, 21);
            this.algBox.TabIndex = 10;
            // 
            // AuthForm
            // 
            this.AcceptButton = this.signinButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(784, 451);
            this.Controls.Add(this.algBox);
            this.Controls.Add(this.createAccountButton);
            this.Controls.Add(this.exceptionError);
            this.Controls.Add(this.passwordError);
            this.Controls.Add(this.usernameError);
            this.Controls.Add(this.authenicationLabel);
            this.Controls.Add(this.signinButton);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.passwordInput);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.usernameInput);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AuthForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "SmokeScreen Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox usernameInput;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordInput;
        private System.Windows.Forms.Button signinButton;
        private System.Windows.Forms.Label authenicationLabel;
        private System.Windows.Forms.Label usernameError;
        private System.Windows.Forms.Label passwordError;
        private System.Windows.Forms.Label exceptionError;
        private System.Windows.Forms.Button createAccountButton;
        private System.Windows.Forms.ComboBox algBox;
    }
}

