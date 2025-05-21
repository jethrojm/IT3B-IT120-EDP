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
    public partial class Logout : Form
    {
        public Logout()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // You can remove this if unused
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // close logout form

            // Close all other open forms except login
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                if (!(form is Admin)) // don't close login form if already open
                {
                    form.Close();
                }
            }

            // Show login form
            Admin loginForm = new Admin();
            loginForm.Show();
        }



        private void button2_Click(object sender, EventArgs e) // Cancel Logout
        {
            this.Close(); // Just close the logout form and return to previous form
        }
    }
}