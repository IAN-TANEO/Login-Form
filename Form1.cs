using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoginSystem
{
    public partial class LoginForm : Form
    {
        private readonly DatabaseHelper dbHelper;


        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblStatus;

        public LoginForm()
        {
            InitializeUI();
            dbHelper = new DatabaseHelper();
        }

        private void InitializeUI()
        {
    
            this.Text = "Login System";
            this.ClientSize = new Size(350, 200);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;


            var lblUser = new Label
            {
                Text = "Username:",
                Location = new Point(30, 30),
                AutoSize = true
            };


            txtUsername = new TextBox
            {
                Location = new Point(120, 30),
                Size = new Size(180, 25)
            };


            var lblPass = new Label
            {
                Text = "Password:",
                Location = new Point(30, 70),
                AutoSize = true
            };


            txtPassword = new TextBox
            {
                Location = new Point(120, 70),
                Size = new Size(180, 25),
                PasswordChar = '*'
            };


            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(120, 110),
                Size = new Size(100, 30)
            };
            btnLogin.Click += BtnLogin_Click;


            lblStatus = new Label
            {
                Location = new Point(30, 150),
                Size = new Size(290, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };


            this.Controls.AddRange(new Control[] {
                lblUser, txtUsername,
                lblPass, txtPassword,
                btnLogin, lblStatus
            });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Please enter both username and password";
                return;
            }

            if (dbHelper.ValidateUser(username, password))
            {
                lblStatus.Text = "Login successful!";
                MessageBox.Show($"Welcome, {username}!", "Login Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                lblStatus.Text = "Invalid username or password";
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}