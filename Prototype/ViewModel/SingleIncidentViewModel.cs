using Prototype.Model;
using Prototype.Model.Global_Classes;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System;
using Prototype.Model.Incident_Sub_System.Interfaces;
using Prototype.Model.Resource_Sub_System;
using Prototype.View.Popup_Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Prototype.ViewModel
{
    /// <summary>
    /// View Model responsible for displaying information for a single incident
    /// </summary>
    class SingleIncidentViewModel : WorkspaceViewModel
    {
        #region Fields

        private Incident incident; //the incident to display information for
        private IncidentType[] incidentTypes; //a list of all incident types
        private RelayCommand viewApplianceCommand;
        private RelayCommand assignAppliancesCommand;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="incident">The incident to display information for</param>
        public SingleIncidentViewModel(Incident incident)
        {
            this.incident = incident;
            incidentTypes = Tools.IncidentControllerDB.GetAllIncidentTypes().ToArray();
        }

        #region Properties

        /// <summary>
        /// The text that appears in the tab header
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return "Inc. " + incident.IncidentNumber.ToString();
            }
        }

        /// <summary>
        /// The incident that this tab relates to
        /// </summary>
        public Incident Incident
        {
            get { return incident; }
        }

        /// <summary>
        /// The collection of messages for the incident.
        /// </summary>
        public LogDataView[] Messages
        {
            get { return incident.History; }
        }
        #endregion

        #region Properties for the incident types combo box

        /// <summary>
        /// List of all appliances types - bound the incident types combo box
        /// </summary>
        public IncidentType[] IncidentTypes
        {
            get { return incidentTypes; }
        }

        /// <summary>
        /// An individual IncidentType - bound to the incident types combo box SelectedItem.
        /// Used to automatically select the appropriate incident type for the incident, 
        /// and expose a way to change the type.
        /// </summary>
        public IncidentType IncidentTypeSelectedItem
        {
            get
            {
                foreach (IncidentType type in incidentTypes)
                {
                    if (type.Name == incident.Type.Name)
                        return type;
                }

                return null;
            }
            set { incident.Type = value; }
        }

        #endregion

        #region Properties for viewing an assigned appliance

        /// <summary>
        /// The resource selected in the dataset
        /// </summary>
        public AssignedResource SelectedItem { get; set; }

        /// <summary>
        /// Returns a command that opens a new appliance tab.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand ViewApplianceCommand
        {
            get
            {
                if (viewApplianceCommand == null)
                {
                    viewApplianceCommand = new RelayCommand(
                        param => this.openTab(new SingleApplianceViewModel(new Appliance(SelectedItem.CallSign, Tools.ApplianceDB.GetApplianceInfo(SelectedItem.CallSign))))
                        );
                }
                return viewApplianceCommand;
            }
        }

        #endregion

        #region Properties for the incident state combo box

        /// <summary>
        /// List of all incident states - bound the incident states combo box
        /// </summary>
        public string[] IncidentStates
        {
            get { return new string[] { "Open", "Stop Received", "Closed" }; }
        }

        /// <summary>
        /// The current state of the incident. 
        /// </summary>
        public string CurrentState
        {
            get 
            {
                if (Incident.IncidentClosedTime != DateTime.MinValue)
                    return "Closed";
                else if (Incident.StopTime != DateTime.MinValue)
                    return "Stop Received";
                else
                    return "Open";
            }
            set 
            {
                switch (value)
                {
                    case "Closed":
                        Incident.Close();
                        break;
                    case "Open":
                        Incident.Reopen();
                        break;
                    case "Stop Received":
                        new EnterMessageWindow(true, Incident.AssignedResources.First(x => x.ClosedDown == DateTime.MinValue).CallSign).Show();
                        break;
                    default:
                        break;
                }
            }
        }
        

        #endregion

        #region Properties for assigning additional appliances

        /// <summary>
        /// Returns a command that opens the window to assign more resources.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand AssignAppliancesCommand
        {
            get
            {
                if (assignAppliancesCommand == null)
                {
                    assignAppliancesCommand = new RelayCommand(
                        param => new ApplianceSelectionWindow(incident).Show()
                        );
                }
                return assignAppliancesCommand;
            }
        }

        #endregion

        #region Properties for enabling/disabling controls

        /// <summary>
        /// Determines if the editable fields of the incident page should be enabled or not based on if the incident has been closed.
        /// </summary>
        public bool ButtonEnabled
        {
            get 
            {
                if (Incident.IncidentClosedTime == DateTime.MinValue)
                    return true;
                else
                    return false;
            }
        }
        #endregion
    }
}
