using Prototype.Model.Global_Classes;
using Prototype.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Prototype.View.Popup_Windows
{
    /// <summary>
    /// Interaction logic for LogOn.xaml
    /// </summary>
    public partial class LogOn : Window
    {
        public LogOn()
        {
            InitializeComponent();

            //save the database information
            Properties.Settings.Default.DatabaseName = "m08cde";
            Properties.Settings.Default.ServerIP = "127.0.0.1";
            Properties.Settings.Default.Username = "root";
            Properties.Settings.Default.Password = "mypassword";
            Properties.Settings.Default.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            try
            {
                id = Convert.ToInt32(textBoxUser.Text);

                if (Tools.Authentication.Logon(id, textBoxPassword.Password))
                {
                    Properties.Settings.Default.LoggedInUserId = id;
                    Properties.Settings.Default.LoggedInUserName = Tools.Authentication.GetName(id);
                    Properties.Settings.Default.Save();

                    // load the application
                    MainWindow window = new MainWindow();
                    MainWindowViewModel viewModel = new MainWindowViewModel();
                    window.DataContext = viewModel;
                    window.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("The username or password was incorrect." + Environment.NewLine + "Try '1' and 'mypassword'", "Authentication Unsuccessful");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter the numerical user ID");
            }
        }
    }
}
