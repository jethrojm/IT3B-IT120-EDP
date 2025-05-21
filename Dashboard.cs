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
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Products adminForm = new Products();
            adminForm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Orders adminForm = new Orders();
            adminForm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Users adminForm = new Users();
            adminForm.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Summary adminForm = new Summary();
            adminForm.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Logout adminForm = new Logout();
            adminForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Payments adminForm = new Payments();
            adminForm.Show();
            this.Hide();
        }
    }
}
