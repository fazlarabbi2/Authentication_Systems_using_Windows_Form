using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AuthenWindowsForm
{
    public partial class frmRegister : Form
    {
        public frmRegister()
        {
            InitializeComponent();
        }

        private string connectionString = @"Server=DESKTOP-A87MT3L\SQLEXPRESS; Database=AuthWFP; Trusted_Connection=True; TrustServerCertificate=True;";

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textUserName_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPassword.Checked)
            {
                textPassword.PasswordChar = '\0';
                textConPassword.PasswordChar = '\0';
            }
            else
            {
                textPassword.PasswordChar = '*';
                textConPassword.PasswordChar = '*';
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrEmpty(textUserName.Text) || string.IsNullOrEmpty(textPassword.Text) || string.IsNullOrEmpty(textConPassword.Text))
            {
                MessageBox.Show("Username and Password fields are empty", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if passwords match
            if (textPassword.Text != textConPassword.Text)
            {
                MessageBox.Show("Passwords do not match, Please Re-Enter", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textPassword.Text = "";
                textConPassword.Text = "";
                textPassword.Focus();
                return;
            }

            // Hash the password before storing it
            string hashedPassword = HashPassword(textPassword.Text);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check if the username already exists
                    string checkUserQuery = "SELECT COUNT(username) FROM users WHERE username=@username";
                    using (SqlCommand checkUserCmd = new SqlCommand(checkUserQuery, con))
                    {
                        checkUserCmd.Parameters.AddWithValue("@username", textUserName.Text);
                        int userCount = (int)checkUserCmd.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Username already exists, please choose a different username", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert new user into the database
                    string registerQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
                    using (SqlCommand cmd = new SqlCommand(registerQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@username", textUserName.Text);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Clear fields and show success message
                textUserName.Text = "";
                textPassword.Text = "";
                textConPassword.Text = "";

                MessageBox.Show("Your Account Successfully Created", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            textUserName.Text = "";
            textPassword.Text = "";
            textConPassword.Text = "";
            textUserName.Focus();
        }

        private void backToLogin_Click(object sender, EventArgs e)
        {
            new frmLogin().Show();
            this.Hide();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
