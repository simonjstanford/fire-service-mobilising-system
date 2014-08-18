using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Prototype.ViewModel;
using Prototype.Model.Global_Classes;
using System.Collections.ObjectModel;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using Prototype.View.Popup_Windows;

namespace Prototype
{
  public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new LogOn().Show();
            Tools.Incidents = new ObservableCollection<IncidentDataView>(Tools.IncidentControllerDB.GetAllIncidents());
            Tools.Appliances = new ObservableCollection<ApplianceDataView>(Tools.ResourceControllerDB.GetAppliances(false));
        }
    }

}
