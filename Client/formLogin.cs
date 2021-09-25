using System;
using System.Windows.Forms;

namespace Client
{
    public partial class formLogin : Form
    {

        public formLogin()
        {
            InitializeComponent();
            btnAdd.Enabled = false;

        }

        public String GetName()
        {
            return logIn.Text;
        }

        public String GetPass()
        {
            return password.Text;
        }


        public void slblU(String v)
        {
            lblName.Text = v;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = true; 
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }
    }
}
