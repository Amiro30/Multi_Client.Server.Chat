using System;
using System.Windows.Forms;

namespace Client
{
    public partial class formLogin : Form
    {
        //bool userExist = false;
        public bool UserExist  { get; set; }

        public formLogin()
        {
            InitializeComponent();
            LoginBtn.Enabled = false;
            RegisterBtn.Enabled = false;
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
            LoginBtn.Enabled = true;
            RegisterBtn.Enabled = true;
        }

       

        private void lblName_Click(object sender, EventArgs e)
        {

        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            UserExist = false;
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            UserExist = true;
        }
    }
}
