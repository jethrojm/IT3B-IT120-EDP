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

namespace $safeprojectname$
{
    public partial class Payments : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=iloveyou;database=informationsystem;";

        public Payments()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = row.Cells["PaymentID"].Value.ToString();
                textBox2.Text = row.Cells["OrderID"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["PaymentDate"].Value);
                textBox3.Text = row.Cells["AmountPaid"].Value.ToString();
            }
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

        private void button4_Click(object sender, EventArgs e)
        {
            // Prompt user to enter PaymentID to delete
            string input = Microsoft.VisualBasic.Interaction.InputBox("Please enter the PaymentID to delete:", "Delete Payment", "");

            if (!int.TryParse(input, out int paymentID))
            {
                MessageBox.Show("Invalid PaymentID. Please enter a valid number.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM payments WHERE PaymentID=@paymentID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@paymentID", paymentID);
                    int affectedRows = cmd.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Payment deleted successfully!");
                        LoadPayments();
                    }
                    else
                    {
                        MessageBox.Show("PaymentID not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting payment: " + ex.Message);
            }
        }




        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void ClearInputs()
        {
            textBox1.Clear(); // PaymentID
            textBox2.Clear(); // OrderID
            textBox3.Clear(); // AmountPaid
            dateTimePicker1.Value = DateTime.Today; // PaymentDate
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text.Trim(), out int orderID))
            {
                MessageBox.Show("Please enter a valid Order ID.");
                return;
            }

            if (!decimal.TryParse(textBox3.Text.Trim(), out decimal amountPaid))
            {
                MessageBox.Show("Please enter a valid amount.");
                return;
            }

            DateTime paymentDate = dateTimePicker1.Value;

            // Optional: check if OrderID exists before inserting
            if (!OrderExists(orderID))
            {
                MessageBox.Show("Order ID does not exist.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO payments (OrderID, PaymentDate, AmountPaid)
                             VALUES (@orderID, @paymentDate, @amountPaid)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@orderID", orderID);
                    cmd.Parameters.AddWithValue("@paymentDate", paymentDate);
                    cmd.Parameters.AddWithValue("@amountPaid", amountPaid);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Payment added successfully!");
                    LoadPayments();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding payment: " + ex.Message);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            // Prompt for PaymentID to update
            string paymentIDInput = Microsoft.VisualBasic.Interaction.InputBox("Enter PaymentID to update:", "Update Payment", "");
            if (!int.TryParse(paymentIDInput, out int paymentID))
            {
                MessageBox.Show("Invalid PaymentID.");
                return;
            }

            // Prompt for OrderID
            string orderIDInput = Microsoft.VisualBasic.Interaction.InputBox("Enter new OrderID:", "Update Payment", "");
            if (!int.TryParse(orderIDInput, out int orderID))
            {
                MessageBox.Show("Invalid OrderID.");
                return;
            }

            // Prompt for PaymentDate
            string paymentDateInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Payment Date (YYYY-MM-DD):", "Update Payment", DateTime.Today.ToString("yyyy-MM-dd"));
            if (!DateTime.TryParse(paymentDateInput, out DateTime paymentDate))
            {
                MessageBox.Show("Invalid date format.");
                return;
            }

            // Prompt for AmountPaid
            string amountPaidInput = Microsoft.VisualBasic.Interaction.InputBox("Enter Amount Paid:", "Update Payment", "");
            if (!decimal.TryParse(amountPaidInput, out decimal amountPaid))
            {
                MessageBox.Show("Invalid amount.");
                return;
            }

            // Optional: Check if OrderID exists
            if (!OrderExists(orderID))
            {
                MessageBox.Show("OrderID does not exist.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE payments 
                             SET OrderID=@orderID, PaymentDate=@paymentDate, AmountPaid=@amountPaid 
                             WHERE PaymentID=@paymentID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@orderID", orderID);
                    cmd.Parameters.AddWithValue("@paymentDate", paymentDate);
                    cmd.Parameters.AddWithValue("@amountPaid", amountPaid);
                    cmd.Parameters.AddWithValue("@paymentID", paymentID);

                    int affectedRows = cmd.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Payment updated successfully!");
                        LoadPayments();
                    }
                    else
                    {
                        MessageBox.Show("PaymentID not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating payment: " + ex.Message);
            }
        }




        private void button6_Click(object sender, EventArgs e)
        {
            LoadPayments();
        }

        private bool OrderExists(int orderID)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM orders WHERE OrderID = @orderID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@orderID", orderID);
                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void LoadPayments()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM payments";
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
