using Prototype.Model;
using Prototype.Model.Global_Classes;
using Prototype.Model.Incident_Sub_System;
using Prototype.Model.Incident_Sub_System.Container_Classes;
using Prototype.Model.Incident_Sub_System.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prototype.ViewModel
{
    class IncidentsViewModel : WorkspaceViewModel
    {
        //private ObservableCollection<IncidentDataView> incidents; //backing field containing a list of all appliances
        private RelayCommand viewIncidentsCommand; //the button to view an incident is bound to this property in XAML

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The database object used to retrieve info</param>
        public IncidentsViewModel()
        {
            //incidents = new ObservableCollection<IncidentDataView>(Tools.IncidentControllerDB.GetAllIncidents());
            //SelectedItem = incidents[0];
        }

        /// <summary>
        /// the text that appears in the tab header
        /// </summary>
        public override string DisplayName
        {
            get { return "All Incidents"; }
        }

        /// <summary>
        /// Retrieves a list of appliances.  Bound to the XAML dataset.
        /// </summary>
        public ObservableCollection<IncidentDataView> Incidents
        {
            get 
            {
                return Tools.Incidents; 
            }
        }

        /// <summary>
        /// The item selected in the dataset
        /// </summary>
        public IncidentDataView SelectedItem { get; set; }

        /// <summary>
        /// Returns a command that searches for matching addresses.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand ViewIncidentCommand
        {
            get
            {
                if (viewIncidentsCommand == null)
                {
                    viewIncidentsCommand = new RelayCommand(
                        param => this.openTab(new SingleIncidentViewModel(new Incident(SelectedItem.IncidentNumber)))
                        );
                }
                return viewIncidentsCommand;
            }
        }
    }
}
