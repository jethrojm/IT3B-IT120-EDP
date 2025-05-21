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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;



namespace FirstProject
{
    public partial class Summary : Form
    {
        string connectionString = "server=localhost;uid=root;pwd=iloveyou;database=informationsystem;";
        public Summary()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Force date to first of the month
            DateTime dt = dateTimePicker1.Value;
            DateTime firstOfMonth = new DateTime(dt.Year, dt.Month, 1);
            dateTimePicker1.Value = firstOfMonth;

            // Optionally display total sales for that month in label6 (call a helper function)
            DisplayTotalSalesForMonth(firstOfMonth);
        }

        private void DisplayTotalSalesForMonth(DateTime monthYear)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Query sales_summary for selected monthYear
                    string query = "SELECT TotalSales FROM sales_summary WHERE MonthYear = @monthYear";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@monthYear", monthYear);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        decimal totalSales = Convert.ToDecimal(result);
                        label6.Text = $"Total Sales: {totalSales:C2}"; // format currency
                    }
                    else
                    {
                        label6.Text = "Total Sales: 0.00";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving total sales: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime selectedMonth = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, 1);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Calculate total sales from orders table for the month
                    string totalSalesQuery = @"
                        SELECT IFNULL(SUM(TotalAmount), 0) 
                        FROM orders 
                        WHERE YEAR(OrderDate) = @year AND MONTH(OrderDate) = @month";

                    MySqlCommand cmdTotal = new MySqlCommand(totalSalesQuery, conn);
                    cmdTotal.Parameters.AddWithValue("@year", selectedMonth.Year);
                    cmdTotal.Parameters.AddWithValue("@month", selectedMonth.Month);

                    decimal totalSales = Convert.ToDecimal(cmdTotal.ExecuteScalar());

                    // Check if summary already exists for that month
                    string checkQuery = "SELECT COUNT(*) FROM sales_summary WHERE MonthYear = @monthYear";
                    MySqlCommand cmdCheck = new MySqlCommand(checkQuery, conn);
                    cmdCheck.Parameters.AddWithValue("@monthYear", selectedMonth);

                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        // Update existing record
                        string updateQuery = "UPDATE sales_summary SET TotalSales=@totalSales WHERE MonthYear=@monthYear";
                        MySqlCommand cmdUpdate = new MySqlCommand(updateQuery, conn);
                        cmdUpdate.Parameters.AddWithValue("@totalSales", totalSales);
                        cmdUpdate.Parameters.AddWithValue("@monthYear", selectedMonth);
                        cmdUpdate.ExecuteNonQuery();
                    }
                    else
                    {
                        // Insert new record
                        string insertQuery = "INSERT INTO sales_summary (MonthYear, TotalSales) VALUES (@monthYear, @totalSales)";
                        MySqlCommand cmdInsert = new MySqlCommand(insertQuery, conn);
                        cmdInsert.Parameters.AddWithValue("@monthYear", selectedMonth);
                        cmdInsert.Parameters.AddWithValue("@totalSales", totalSales);
                        cmdInsert.ExecuteNonQuery();
                    }

                    MessageBox.Show($"Sales summary for {selectedMonth:MMMM yyyy} generated successfully.\nTotal Sales: {totalSales:C2}");

                    DisplayTotalSalesForMonth(selectedMonth); // update label6
                    LoadSalesSummary(); // refresh grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating sales summary: " + ex.Message);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DateTime monthYear = Convert.ToDateTime(row.Cells["MonthYear"].Value);
                dateTimePicker1.Value = monthYear;
                DisplayTotalSalesForMonth(monthYear);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadSalesSummary();
        }
        private void LoadSalesSummary()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT SummaryID, MonthYear, TotalSales FROM sales_summary ORDER BY MonthYear DESC";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    // Format MonthYear column to display Month-Year nicely
                    dataGridView1.Columns["MonthYear"].DefaultCellStyle.Format = "MMMM yyyy";
                    dataGridView1.Columns["TotalSales"].DefaultCellStyle.Format = "C2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sales summary: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.");
                return;
            }

            try
            {
                // Create Excel application
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                if (excelApp == null)
                {
                    MessageBox.Show("Excel is not installed.");
                    return;
                }

                excelApp.Visible = false;
                Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Sales Summary";

                // Add column headers
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
                }

                // Add rows
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value?.ToString();
                    }
                }

                // Auto-fit columns
                worksheet.Columns.AutoFit();

                // Show Save File Dialog
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Workbook|*.xlsx";
                saveFileDialog.Title = "Save Excel File";
                saveFileDialog.FileName = "SalesSummary.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    Marshal.ReleaseComObject(worksheet);
                    Marshal.ReleaseComObject(workbook);
                    Marshal.ReleaseComObject(excelApp);

                    MessageBox.Show("Excel file exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to Excel: " + ex.Message);
            }
        }




    }
}
