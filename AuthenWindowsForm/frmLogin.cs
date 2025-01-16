using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AuthenWindowsForm
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private string connectionString = @"Server=DESKTOP-A87MT3L\SQLEXPRESS; Database=AuthWFP; Trusted_Connection=True; TrustServerCertificate=True;";

        private void textUserName_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void backToRegistration_Click(object sender, System.EventArgs e)
        {
            new frmRegister().Show();
            this.Hide();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Hash the entered password
                    string hashedPassword = HashPassword(textPassword.Text);

                    // Query to check if the username and hashed password match
                    string loginQuery = "SELECT username FROM users WHERE username=@username AND password=@password";
                    using (SqlCommand cmd = new SqlCommand(loginQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@username", textUserName.Text);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // Login successful
                                new Dashboard().Show();
                                this.Hide();
                            }
                            else
                            {
                                // Login failed
                                MessageBox.Show("Invalid Username or Password, Please Try Again", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textUserName.Text = "";
                                textPassword.Text = "";
                                textUserName.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to hash passwords (same as in the registration form)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes); // Returns a 44-character string
            }
        }


        private void button2_Click(object sender, System.EventArgs e)
        {
            textUserName.Text = "";
            textPassword.Text = "";
            textUserName.Focus();
        }

        private void checkBoxShowPassword_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBoxShowPassword.Checked)
            {
                textPassword.PasswordChar = '\0';
            }
            else
            {
                textPassword.PasswordChar = '*';
            }
        }

        private void textPassword_TextChanged(object sender, System.EventArgs e)
        {

        }
    }
}
