using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;

namespace LoginSystem
{
    public partial class LoginForm : Form
    {
        private readonly DatabaseHelper dbHelper;

        private TextBox txtUsername, txtPassword;
        private Button btnLogin, btnGoogleLogin, btnCreateAccount;
        private Label lblStatus, lblTitle;
        private LinkLabel linkForgot;


        private readonly string _googleClientId;
        private readonly string _googleClientSecret;

        public LoginForm()
        {
            InitializeUI();
            dbHelper = new DatabaseHelper();

            _googleClientId = ConfigurationManager.AppSettings["GoogleClientId"];
            _googleClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];

            if (string.IsNullOrEmpty(_googleClientId) || string.IsNullOrEmpty(_googleClientSecret))
            {
                MessageBox.Show("Google authentication is not configured properly", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeUI()
        {
            
            this.Text = "Modern Login System";
            this.ClientSize = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;


            lblTitle = new Label
            {
                Text = "Hi, Welcome!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(150, 30)
            };

         
            var lblUser = new Label
            {
                Text = "Username/Email:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, 90),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(50, 115),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            
            var lblPass = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, 160),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(50, 185),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };


            linkForgot = new LinkLabel
            {
                Text = "Forgot password?",
                Location = new Point(50, 220),
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                LinkColor = Color.FromArgb(0, 120, 215),
                Cursor = Cursors.Hand
            };
            linkForgot.Click += LinkForgot_Click;

           
            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(50, 240),
                Size = new Size(350, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnGoogleLogin = new Button
            {
                Text = "Continue with Google",
                Location = new Point(50, 290),
                Size = new Size(350, 40),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(66, 133, 244),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnGoogleLogin.FlatAppearance.BorderColor = Color.FromArgb(66, 133, 244);
            btnGoogleLogin.FlatAppearance.BorderSize = 1;
            btnGoogleLogin.Click += BtnGoogleLogin_Click;


            btnCreateAccount = new Button
            {
                Text = "Create New Account",
                Location = new Point(50, 340),
                Size = new Size(350, 30),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 120, 215),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand
            };
            btnCreateAccount.FlatAppearance.BorderSize = 0;
            btnCreateAccount.Click += BtnCreateAccount_Click;


            lblStatus = new Label
            {
                Location = new Point(50, 380),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Red
            };

            
            this.Controls.AddRange(new Control[] {
                lblTitle,
                lblUser, txtUsername,
                lblPass, txtPassword,
                linkForgot,
                btnLogin,
                btnGoogleLogin,
                btnCreateAccount,
                lblStatus
            });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Please enter both username/email and password";
                return;
            }

            if (dbHelper.ValidateUser(username, password))
            {
                lblStatus.Text = "Login successful!";
                MessageBox.Show($"Welcome!", "Login Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Invalid username/email or password";
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private async void BtnGoogleLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = _googleClientId,
                    ClientSecret = _googleClientSecret
                };

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    new[] { "email", "profile" },
                    "user",
                    CancellationToken.None);

                var service = new Oauth2Service(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "LoginSystem"
                });

                Userinfo userInfo = await service.Userinfo.Get().ExecuteAsync();

                if (dbHelper.UserExists(userInfo.Email))
                {
                    lblStatus.Text = $"Welcome back, {userInfo.Name}!";
                    MessageBox.Show($"Google login successful!\nEmail: {userInfo.Email}",
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var registerForm = new RegisterForm(dbHelper, userInfo.Email, userInfo.Name);
                    if (registerForm.ShowDialog() == DialogResult.OK)
                    {
                        lblStatus.Text = $"Account created for {userInfo.Name}";
                        MessageBox.Show($"New account created!\nEmail: {userInfo.Email}",
                                      "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Google login failed";
                MessageBox.Show($"Error during Google login: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LinkForgot_Click(object sender, EventArgs e)
        {
            string email = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email first", "Info",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dbHelper.UserExists(email))
            {
                MessageBox.Show($"Password reset instructions would be sent to {email}",
                               "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Email not found in our system", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreateAccount_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm(dbHelper);
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                txtUsername.Text = registerForm.Email;
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

    }
}