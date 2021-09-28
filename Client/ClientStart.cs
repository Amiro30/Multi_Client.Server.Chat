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
            
                if (formLogin.ShowDialog() == DialogResult.OK)
                {
                    if (formLogin.GetName() != "" && formLogin.GetPass() != "")
                    {
                        formMainCl form = new formMainCl();
                        form.setCredentials(formLogin.GetName(), formLogin.GetPass(), formLogin.UserExist);
                    Application.Run(form);
                    }
                    else
                    {
                        formLogin.slblU("Please enter");
                    }
                }
                else
                {
                    Application.Exit();
                }
        }
    }
}
