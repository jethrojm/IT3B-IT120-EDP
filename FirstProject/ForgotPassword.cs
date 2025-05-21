using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstProject
{
    public partial class ForgotPassword : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=iloveyou;database=informationsystem;";

        public ForgotPassword()
        {
            InitializeComponent();
        }

        // 🔐 RESET PASSWORD
        private void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email.");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM users WHERE Email = @Email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        reader.Close();

                        // Prompt for new password
                        string newPassword = Microsoft.VisualBasic.Interaction.InputBox("Enter your new password:", "Reset Password");

                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                            string updateQuery = "UPDATE users SET Password = @Password WHERE Email = @Email";
                            MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                            updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
                            updateCmd.Parameters.AddWithValue("@Email", email);
                            updateCmd.ExecuteNonQuery();

                            MessageBox.Show("Password reset successfully!");
                            Admin loginForm = new Admin();
                            loginForm.Show(); ;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Password reset cancelled.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // ❌ CANCEL BUTTON
        private void button2_Click(object sender, EventArgs e)
        {
            Admin loginForm = new Admin();
            loginForm.Show();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}