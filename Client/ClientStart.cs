using System;
using System.Windows.Forms;

namespace Client
{
    static class ClientStart
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            formLogin formLogin = new formLogin();
            Boolean flag = true;

                if (formLogin.ShowDialog() == DialogResult.OK)
                {
                    if (formLogin.Textb() != "")
                    {
                        flag = false;
                        formMainCl form = new formMainCl();
                        form.setName(formLogin.Textb());
                        Application.Run(form);
                    }
                    else
                    {
                        formLogin.slblU("Please enter");
                        //MessageBox.Show("Please enter");
                    }
                }
                else
                {
                    Application.Exit();
                }
        }
    }
}
