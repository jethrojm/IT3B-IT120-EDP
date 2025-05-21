using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FirstProject
{
    public partial class Admin : Form
    {
        // 💡 Your connection string (change as needed)
        string connectionString = "server=localhost;uid=root;pwd=iloveyou;database=informationsystem;";

        public Admin()
        {
            InitializeComponent();
            textBox2.UseSystemPasswordChar = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*';
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        // 🔙 Cancel / Back button
        private void button2_Click(object sender, EventArgs e)
        {
            Login adminForm = new Login();
            adminForm.Show();
            this.Hide();
        }

        // 🔐 Login button
        private void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

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
                        string hashedPassword = reader["Password"].ToString();

                        // ✅ Compare using BCrypt
                        if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                        {
                            MessageBox.Show("Welcome, " + reader["FirstName"].ToString() + "!", "Login Successful");

                            // Open dashboard form
                            Dashboard dashboard = new Dashboard();
                            dashboard.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid password.", "Login Failed");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email not found.", "Login Failed");
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection error: " + ex.Message);
                }
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            ForgotPassword adminForm = new ForgotPassword();
            adminForm.Show();
            this.Hide();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
