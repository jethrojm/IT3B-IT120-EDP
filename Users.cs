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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace $safeprojectname$
{
    public partial class Users : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=iloveyou;database=informationsystem;";
        int selectedUserId = 0;

        public Users()
        {
            InitializeComponent();
        }

        // Load users into DataGridView
        private void LoadUsers()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserID, Email, FirstName, LastName, Role FROM users";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load users: " + ex.Message);
                }
            }
        }

        // Form Load (Populate DataGridView on startup)
        private void Form4_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure there's a valid row selected
            if (e.RowIndex >= 0)
            {
                // Get the selected UserID (the first column in your DataGridView)
                string userID = dataGridView1.Rows[e.RowIndex].Cells["UserID"].Value.ToString();

                // Populate the textboxes and combo box with the selected user's data
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["FirstName"].Value.ToString();
                textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["LastName"].Value.ToString();
                textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                comboBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Role"].Value.ToString();

                // Optionally, store the selected UserID for later use (e.g., when updating or deleting)
                selectedUserId = Convert.ToInt32(userID);
            }
        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        // Logout (Button 5)
        private void button5_Click(object sender, EventArgs e)
        {
            Logout logoutForm = new Logout();
            logoutForm.Show();
            this.Hide();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {
            Dashboard adminForm = new Dashboard();
            adminForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Show a prompt for the user to enter the UserID they want to delete
            string userID = Microsoft.VisualBasic.Interaction.InputBox("Please enter the UserID to delete:", "Delete User", "");

            // Check if the UserID is valid
            if (string.IsNullOrEmpty(userID))
            {
                MessageBox.Show("UserID cannot be empty. Please provide a valid UserID.");
                return;
            }

            // Attempt to delete the user
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("sp_DeleteUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pass the UserID to the stored procedure
                    cmd.Parameters.AddWithValue("@p_UserID", userID);

                    // Execute the command
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User deleted successfully!");

                    // Refresh the DataGridView after deletion
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting user: " + ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Ask for the UserID to update
            string userID = Microsoft.VisualBasic.Interaction.InputBox("Please enter the UserID to update:", "Update User", "");

            if (string.IsNullOrWhiteSpace(userID))
            {
                MessageBox.Show("UserID is required.");
                return;
            }

            // Ask for the new values
            string email = Microsoft.VisualBasic.Interaction.InputBox("Enter new Email:", "Update User", "");
            string firstName = Microsoft.VisualBasic.Interaction.InputBox("Enter new First Name:", "Update User", "");
            string lastName = Microsoft.VisualBasic.Interaction.InputBox("Enter new Last Name:", "Update User", "");
            string role = Microsoft.VisualBasic.Interaction.InputBox("Enter new Role:", "Update User", "");

            // Basic validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("sp_UpdateUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_UserID", userID);
                    cmd.Parameters.AddWithValue("@p_Email", email);
                    cmd.Parameters.AddWithValue("@p_FirstName", firstName);
                    cmd.Parameters.AddWithValue("@p_LastName", lastName);
                    cmd.Parameters.AddWithValue("@p_Role", role);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("User updated successfully!");

                    LoadUsers(); // Refresh the user list
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user: " + ex.Message);
            }
        }


        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
        }



        // Add user (Button 2)
        private void button2_Click(object sender, EventArgs e)
        {
            string email = textBox3.Text.Trim();
            string firstName = textBox1.Text.Trim();
            string lastName = textBox2.Text.Trim();
            string role = comboBox1.Text;
            string defaultPassword = "123456"; // You can generate or request a password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("sp_AddUser", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_Email", email);
                    cmd.Parameters.AddWithValue("@p_Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@p_FirstName", firstName);
                    cmd.Parameters.AddWithValue("@p_LastName", lastName);
                    cmd.Parameters.AddWithValue("@p_Role", role);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User added successfully!");

                    LoadUsers(); // Refresh DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding user: " + ex.Message);
                }
            }
        }





        // Clear fields (Button 3)
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        // Refresh user list (Button 6)
        private void button6_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

    }
}
