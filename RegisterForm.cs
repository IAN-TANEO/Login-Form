using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoginSystem
{
    public partial class RegisterForm : Form
    {
        private readonly DatabaseHelper dbHelper;
        private TextBox txtUsername, txtPassword, txtConfirmPassword, txtDisplayName, txtEmail;
        private readonly bool isGoogleRegistration;
        private readonly string googleEmail;

        public string Email => isGoogleRegistration ? googleEmail : txtEmail.Text.Trim();

        public RegisterForm(DatabaseHelper dbHelper, string googleEmail = null, string googleName = null)
        {
            this.dbHelper = dbHelper;
            this.isGoogleRegistration = !string.IsNullOrEmpty(googleEmail);
            this.googleEmail = googleEmail;
            InitializeUI(googleEmail, googleName);
        }

        private void InitializeUI(string googleEmail, string googleName)
        {
            
            this.Text = isGoogleRegistration ? "Complete Google Registration" : "Create New Account";
            this.ClientSize = new Size(400, isGoogleRegistration ? 300 : 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;


            var lblTitle = new Label
            {
                Text = this.Text,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(100, 20)
            };

            int yPos = 60;

            
            if (!isGoogleRegistration)
            {
                var lblEmail = new Label
                {
                    Text = "Email:",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(50, yPos),
                    AutoSize = true
                };

                txtEmail = new TextBox
                {
                    Location = new Point(50, yPos + 25),
                    Size = new Size(300, 30),
                    Font = new Font("Segoe UI", 10)
                };

                this.Controls.Add(lblEmail);
                this.Controls.Add(txtEmail);
                yPos += 60;
            }


            var lblDisplayName = new Label
            {
                Text = "Display Name:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, yPos),
                AutoSize = true
            };

            txtDisplayName = new TextBox
            {
                Location = new Point(50, yPos + 25),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10),
                Text = googleName ?? ""
            };

            yPos += 60;

            
            if (!isGoogleRegistration)
            {
                var lblUsername = new Label
                {
                    Text = "Username:",
                    Font = new Font("Segoe UI", 9),
                    Location = new Point(50, yPos),
                    AutoSize = true
                };

                txtUsername = new TextBox
                {
                    Location = new Point(50, yPos + 25),
                    Size = new Size(300, 30),
                    Font = new Font("Segoe UI", 10)
                };

                yPos += 60;

                this.Controls.Add(lblUsername);
                this.Controls.Add(txtUsername);
            }


            var lblPassword = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, yPos),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(50, yPos + 25),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '*'
            };

            yPos += 60;

            
            var lblConfirm = new Label
            {
                Text = "Confirm Password:",
                Font = new Font("Segoe UI", 9),
                Location = new Point(50, yPos),
                AutoSize = true
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(50, yPos + 25),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10),
                PasswordChar = '*'
            };

            yPos += 70;

            
            var btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(50, yPos),
                Size = new Size(300, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            this.Controls.AddRange(new Control[] {
                lblTitle,
                lblDisplayName, txtDisplayName,
                lblPassword, txtPassword,
                lblConfirm, txtConfirmPassword,
                btnRegister
            });

            if (isGoogleRegistration)
            {
                var lblGoogleInfo = new Label
                {
                    Text = $"Registering with: {googleEmail}",
                    Font = new Font("Segoe UI", 8),
                    ForeColor = Color.Gray,
                    Location = new Point(50, yPos + 40),
                    AutoSize = true
                };
                this.Controls.Add(lblGoogleInfo);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string email = Email;
            string username = isGoogleRegistration ? email : txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirm = txtConfirmPassword.Text.Trim();
            string displayName = txtDisplayName.Text.Trim();

            if (string.IsNullOrEmpty(displayName))
            {
                MessageBox.Show("Please enter your display name", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!isGoogleRegistration && string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!isGoogleRegistration && string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter a password", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Passwords do not match", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dbHelper.UserExists(email))
            {
                MessageBox.Show("Email already registered", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!isGoogleRegistration && dbHelper.UserExists(username))
            {
                MessageBox.Show("Username already taken", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dbHelper.CreateUser(username, password, displayName, email, isGoogleRegistration))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}