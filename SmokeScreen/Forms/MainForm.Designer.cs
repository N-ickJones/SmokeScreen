namespace SmokeScreen
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.outLabel = new System.Windows.Forms.Label();
            this.outBox = new System.Windows.Forms.TextBox();
            this.inLabel = new System.Windows.Forms.Label();
            this.inBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.logoutButton = new System.Windows.Forms.Button();
            this.retrieveFileBox = new System.Windows.Forms.ComboBox();
            this.retrieveFileBtn = new System.Windows.Forms.Button();
            this.sendFileBtn = new System.Windows.Forms.Button();
            this.sendFileBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // outLabel
            // 
            this.outLabel.AutoSize = true;
            this.outLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outLabel.Location = new System.Drawing.Point(134, 60);
            this.outLabel.Name = "outLabel";
            this.outLabel.Size = new System.Drawing.Size(159, 20);
            this.outLabel.TabIndex = 0;
            this.outLabel.Text = "Outgoing Message";
            // 
            // outBox
            // 
            this.outBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outBox.Location = new System.Drawing.Point(135, 83);
            this.outBox.Multiline = true;
            this.outBox.Name = "outBox";
            this.outBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outBox.Size = new System.Drawing.Size(518, 100);
            this.outBox.TabIndex = 1;
            // 
            // inLabel
            // 
            this.inLabel.AutoSize = true;
            this.inLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inLabel.Location = new System.Drawing.Point(134, 218);
            this.inLabel.Name = "inLabel";
            this.inLabel.Size = new System.Drawing.Size(168, 20);
            this.inLabel.TabIndex = 2;
            this.inLabel.Text = "Incoming Response";
            // 
            // inBox
            // 
            this.inBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.inBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inBox.Location = new System.Drawing.Point(135, 241);
            this.inBox.Multiline = true;
            this.inBox.Name = "inBox";
            this.inBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.inBox.Size = new System.Drawing.Size(518, 176);
            this.inBox.TabIndex = 3;
            // 
            // sendButton
            // 
            this.sendButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.Location = new System.Drawing.Point(533, 189);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(120, 25);
            this.sendButton.TabIndex = 4;
            this.sendButton.Text = "Send Message";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // logoutButton
            // 
            this.logoutButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logoutButton.Location = new System.Drawing.Point(675, 25);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(113, 33);
            this.logoutButton.TabIndex = 5;
            this.logoutButton.Text = "Logout";
            this.logoutButton.UseVisualStyleBackColor = true;
            this.logoutButton.Click += new System.EventHandler(this.LogoutButton_Click);
            // 
            // retrieveFileBox
            // 
            this.retrieveFileBox.FormattingEnabled = true;
            this.retrieveFileBox.Location = new System.Drawing.Point(667, 327);
            this.retrieveFileBox.Name = "retrieveFileBox";
            this.retrieveFileBox.Size = new System.Drawing.Size(121, 21);
            this.retrieveFileBox.TabIndex = 6;
            // 
            // retrieveFileBtn
            // 
            this.retrieveFileBtn.Location = new System.Drawing.Point(688, 370);
            this.retrieveFileBtn.Name = "retrieveFileBtn";
            this.retrieveFileBtn.Size = new System.Drawing.Size(75, 23);
            this.retrieveFileBtn.TabIndex = 8;
            this.retrieveFileBtn.Text = "Retrieve File";
            this.retrieveFileBtn.UseVisualStyleBackColor = true;
            this.retrieveFileBtn.Click += new System.EventHandler(this.RetrieveFileBtn_Click);
            // 
            // sendFileBtn
            // 
            this.sendFileBtn.Location = new System.Drawing.Point(688, 150);
            this.sendFileBtn.Name = "sendFileBtn";
            this.sendFileBtn.Size = new System.Drawing.Size(75, 23);
            this.sendFileBtn.TabIndex = 10;
            this.sendFileBtn.Text = "Send File";
            this.sendFileBtn.UseVisualStyleBackColor = true;
            this.sendFileBtn.Click += new System.EventHandler(this.SendFileBtn_Click);
            // 
            // sendFileBox
            // 
            this.sendFileBox.FormattingEnabled = true;
            this.sendFileBox.Location = new System.Drawing.Point(667, 107);
            this.sendFileBox.Name = "sendFileBox";
            this.sendFileBox.Size = new System.Drawing.Size(121, 21);
            this.sendFileBox.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AcceptButton = this.sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.sendFileBtn);
            this.Controls.Add(this.sendFileBox);
            this.Controls.Add(this.retrieveFileBtn);
            this.Controls.Add(this.retrieveFileBox);
            this.Controls.Add(this.logoutButton);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.inBox);
            this.Controls.Add(this.inLabel);
            this.Controls.Add(this.outBox);
            this.Controls.Add(this.outLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label outLabel;
        private System.Windows.Forms.TextBox outBox;
        private System.Windows.Forms.Label inLabel;
        private System.Windows.Forms.TextBox inBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.ComboBox retrieveFileBox;
        private System.Windows.Forms.Button retrieveFileBtn;
        private System.Windows.Forms.Button sendFileBtn;
        private System.Windows.Forms.ComboBox sendFileBox;
    }
}