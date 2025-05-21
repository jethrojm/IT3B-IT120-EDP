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
    public partial class Products : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=iloveyou;database=informationsystem;";
        int selectedProductID = 0;

        public Products()
        {
            InitializeComponent();
        }

        private void LoadProducts()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM products"; // Include joins if needed for CategoryName
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProducts(); // Load and display products when the form starts
        }



        private void Products_Load(object sender, EventArgs e)
        {
            LoadProducts();
            LoadCategories(); // This line loads real CategoryIDs
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                selectedProductID = Convert.ToInt32(row.Cells["ProductID"].Value);
                textBox1.Text = row.Cells["ProductName"].Value.ToString();
                comboBox1.SelectedValue = row.Cells["CategoryID"].Value;
                textBox2.Text = row.Cells["Price"].Value.ToString();
                textBox3.Text = row.Cells["Stock"].Value.ToString();
            }
        }



        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.SelectedIndex = -1;
            selectedProductID = 0;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            ClearInputs();
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Prompt for the Product ID to update
            string productIdStr = Microsoft.VisualBasic.Interaction.InputBox("Enter the Product ID to update:", "Update Product", "");
            if (!int.TryParse(productIdStr, out int productId))
            {
                MessageBox.Show("Invalid Product ID.");
                return;
            }

            // Prompt for new field values
            string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter new Product Name:", "Update Product", "");
            string newCategoryIdStr = Microsoft.VisualBasic.Interaction.InputBox("Enter new Category ID (numeric):", "Update Product", "");
            string newPriceStr = Microsoft.VisualBasic.Interaction.InputBox("Enter new Price:", "Update Product", "");
            string newStockStr = Microsoft.VisualBasic.Interaction.InputBox("Enter new Stock Quantity:", "Update Product", "");

            // Validate input
            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newCategoryIdStr) ||
                string.IsNullOrWhiteSpace(newPriceStr) || string.IsNullOrWhiteSpace(newStockStr))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            // Convert and validate numeric inputs
            if (!int.TryParse(newCategoryIdStr, out int newCategoryId) ||
                !decimal.TryParse(newPriceStr, out decimal newPrice) ||
                !int.TryParse(newStockStr, out int newStock))
            {
                MessageBox.Show("Invalid numeric input for Category ID, Price, or Stock.");
                return;
            }

            // Perform the update
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"UPDATE products 
                             SET ProductName = @name, 
                                 CategoryID = @cat, 
                                 Price = @price, 
                                 Stock = @stock 
                             WHERE ProductID = @id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", newName);
                    cmd.Parameters.AddWithValue("@cat", newCategoryId);
                    cmd.Parameters.AddWithValue("@price", newPrice);
                    cmd.Parameters.AddWithValue("@stock", newStock);
                    cmd.Parameters.AddWithValue("@id", productId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Product ID not found.");
                    }

                    LoadProducts();  // Refresh the DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product: " + ex.Message);
            }
        }





        private void button2_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO products (ProductName, Category, Price, Stock) VALUES (@name, @cat, @price, @stock)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", textBox1.Text);
                cmd.Parameters.AddWithValue("@cat", Convert.ToInt32(comboBox1.SelectedValue));
                cmd.Parameters.AddWithValue("@price", Convert.ToDecimal(textBox2.Text));
                cmd.Parameters.AddWithValue("@stock", Convert.ToInt32(textBox3.Text));
                cmd.ExecuteNonQuery();
                MessageBox.Show("Product added!");
                LoadProducts();
                ClearInputs();
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            // Prompt the user to enter the ProductID to delete
            string input = Microsoft.VisualBasic.Interaction.InputBox("Please enter the ProductID to delete:", "Delete Product", "");

            // Validate input
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("ProductID cannot be empty. Please provide a valid ProductID.");
                return;
            }

            // Try parsing the input to an integer ProductID
            if (!int.TryParse(input, out int productID))
            {
                MessageBox.Show("Invalid ProductID. Please enter a numeric value.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("sp_DeleteProduct", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pass the ProductID as parameter
                    cmd.Parameters.AddWithValue("@p_ProductID", productID);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Product deleted successfully!");

                    // Refresh the product list after deletion
                    LoadProducts();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product: " + ex.Message);
            }
        }

        private void LoadCategories()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT CategoryID, CategoryName FROM categories";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "CategoryName";
                comboBox1.ValueMember = "CategoryID";
            }
        }



        private void button6_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

    }
}
