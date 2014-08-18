using Prototype.Model.Global_Classes;
using Prototype.Model.Incident_Sub_System;
using Prototype.Model.Resource_Sub_System;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for EnterMessageWindow.xaml
    /// </summary>
    public partial class EnterMessageWindow : Window
    {
        public EnterMessageWindow(bool selectStop, string selectedCallSign = "")
        {
            InitializeComponent();

            ComboBoxMessageType.ItemsSource = Tools.IncidentControllerDB.GetAllMessageTypes();
            List<ApplianceDataView> appliances = new List<ApplianceDataView>(Tools.ResourceControllerDB.GetAppliances(true));
            ComboBoxResources.ItemsSource = appliances;

            if (selectStop)
                ComboBoxMessageType.SelectedIndex = 2;

            if (selectedCallSign != "")
                ComboBoxResources.SelectedItem = appliances.Find(x => x.CallSign == selectedCallSign);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplianceDataView appliance = (ApplianceDataView)ComboBoxResources.SelectedItem;           
            string type = ComboBoxMessageType.SelectedItem.ToString();
            int user = Properties.Settings.Default.LoggedInUserId;

            if (textBoxMessage.Text.Count() > 0)
            {
                Incident incident = new Incident(appliance.Incident);
                incident.EnterMessage(user, textBoxMessage.Text, type, appliance.CallSign);
            }
            this.Close();
            
        }
    }
}
