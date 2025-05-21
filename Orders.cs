using Microsoft.VisualBasic.ApplicationServices;
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
    public partial class Orders : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=iloveyou;database=informationsystem;";
        private object userID;

        public Orders()
        {
            InitializeComponent();
        }

        private void LoadOrders()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM orders";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {
            Dashboard adminForm = new Dashboard();
            adminForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Logout adminForm = new Logout();
            adminForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear(); // UserID
            textBox2.Clear(); // TotalAmount
            comboBox1.SelectedIndex = -1; // Status
            dateTimePicker1.Value = DateTime.Today;
        }


        private void button2_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            if (!int.TryParse(textBox1.Text, out int userId))
            {
                MessageBox.Show("UserID must be a valid number.");
                return;
            }

            if (!decimal.TryParse(textBox2.Text, out decimal totalAmount))
            {
                MessageBox.Show("Total Amount must be a valid decimal number.");
                return;
            }

            string status = comboBox1.SelectedItem.ToString();
            DateTime orderDate = dateTimePicker1.Value;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO orders (UserID, OrderDate, TotalAmount, Status) VALUES (@userID, @orderDate, @totalAmount, @status)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userID", userId);
                cmd.Parameters.AddWithValue("@orderDate", orderDate);
                cmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Order added successfully!");
                LoadOrders(); // Refresh DataGridView
                ClearInputs();
            }
        }

        private bool UserExists(object userID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM users WHERE UserID = @userID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter OrderID to delete:", "Delete Order", "");

            if (!int.TryParse(input, out int orderId))
            {
                MessageBox.Show("Invalid OrderID.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM orders WHERE OrderID = @id", conn);
                    cmd.Parameters.AddWithValue("@id", orderId);
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                        MessageBox.Show("Order deleted!");
                    else
                        MessageBox.Show("OrderID not found.");

                    LoadOrders();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451)
            {
                MessageBox.Show("Cannot delete order because it is referenced in another table.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting order: " + ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter OrderID to update:", "Update Order", "");
            if (!int.TryParse(input, out int orderId))
            {
                MessageBox.Show("Invalid OrderID.");
                return;
            }

            string newUserID = Microsoft.VisualBasic.Interaction.InputBox("Enter new UserID:", "Update Order", "");
            string newDateStr = Microsoft.VisualBasic.Interaction.InputBox("Enter new Order Date (yyyy-mm-dd):", "Update Order", "");
            string newAmountStr = Microsoft.VisualBasic.Interaction.InputBox("Enter new Total Amount:", "Update Order", "");
            string newStatus = Microsoft.VisualBasic.Interaction.InputBox("Enter new Status:", "Update Order", "");

            if (!int.TryParse(newUserID, out int userID) ||
                !DateTime.TryParse(newDateStr, out DateTime orderDate) ||
                !decimal.TryParse(newAmountStr, out decimal totalAmount))
            {
                MessageBox.Show("Invalid input. Please check values.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE orders SET UserID=@uid, OrderDate=@date, TotalAmount=@amt, Status=@status WHERE OrderID=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@uid", userID);
                    cmd.Parameters.AddWithValue("@date", orderDate);
                    cmd.Parameters.AddWithValue("@amt", totalAmount);
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Order updated!");
                    LoadOrders();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating order: " + ex.Message);
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void ClearInputs()
        {
            textBox1.Clear(); // UserID
            textBox2.Clear(); // TotalAmount
            textBox3.Clear(); // Customer Name
            comboBox1.SelectedIndex = -1; // Status
            dateTimePicker1.Value = DateTime.Today; // Reset to today's date
        }




        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["UserID"].Value.ToString();
                textBox2.Text = row.Cells["TotalAmount"].Value.ToString();
                comboBox1.Text = row.Cells["Status"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["OrderDate"].Value);
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int userID))
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT CONCAT(FirstName, ' ', LastName) AS FullName FROM users WHERE UserID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", userID);

                    object result = cmd.ExecuteScalar();
                    textBox3.Text = result?.ToString() ?? "";
                }
            }
            else
            {
                textBox3.Clear();
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private bool UserExists(int userID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM users WHERE UserID = @userID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
