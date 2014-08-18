using Prototype.Model.Gazetteer_Sub_System;
using Prototype.Model.Incident_Sub_System.Data_Classes;
using Prototype.Model.Incident_Sub_System.Interfaces;
using Prototype.Model.Resource_Sub_System;
using Prototype.Model.Resource_Sub_System.Data_Classes;
using Prototype.View;
using Prototype.View.Popup_Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Prototype.ViewModel
{
    /// <summary>
    /// The ViewModel responsible for displaying the main window.  
    /// Contains many properties that are bound to ribbon bar buttons and logic for adding/removing tabs
    /// Much code is taken from http://msdn.microsoft.com/en-us/library/vstudio/ms229614(v=vs.100).aspx
    /// </summary>
    class MainWindowViewModel : BaseViewModel
    {
        #region Fields
        
        private RelayCommand newIncident; //the button to open a new incident is bound to this property in XAML to execute openTab()
        private RelayCommand viewMap; //the button to open the map is bound to this property in XAML to execute openTab()
        private RelayCommand allIncidents; //the button to open the incidents tab is bound to this property in XAML to execute openTab()
        private RelayCommand allAppliances; //the button to open the appliances tab is bound to this property in XAML to execute openTab()
        private RelayCommand displayMessageBoxCommand; //the button to open the appliances tab is bound to this property in XAML to execute openTab()

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            //set default value
            totalTabs = 0;

            //set the first tab
            TabIndex = totalTabs -1;
        }

        #endregion

        /// <summary>
        /// the text that appears in the tab header
        /// </summary>
        public override string DisplayName
        {
            get { return "Prototype Mobilising System"; }
        }

        #region Commands

        /// <summary>
        /// Returns a command that opens a new incident tab.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand NewIncidentCommand
        {
            get
            {
                if (newIncident == null)
                {
                    newIncident = new RelayCommand(
                        param => this.openTab(new NewIncidentViewModel())
                        );
                }
                return newIncident;
            }
        }

        /// <summary>
        /// Returns a command that opens the map.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand ViewMapCommand
        {
            get
            {
                if (viewMap == null)
                {
                    viewMap = new RelayCommand(
                        param => this.openTab(new MapViewModel())
                        );
                }
                return viewMap;
            }
        }

        /// <summary>
        /// Returns a command that opens a list of all incidents.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand AllIncidentsCommand
        {
            get
            {
                if (allIncidents == null)
                {
                    allIncidents = new RelayCommand(
                        param => this.openTab(new IncidentsViewModel())
                        );
                }
                return allIncidents;
            }
        }

        /// <summary>
        /// Returns a command that opens a list of all appliances.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand AllAppliancesCommand
        {
            get
            {
                if (allAppliances == null)
                {
                    allAppliances = new RelayCommand(
                        param => this.openTab(new AppliancesViewModel())
                        );
                }
                return allAppliances;
            }
        }

        /// <summary>
        /// Returns a command that opens the window to enter an incident menu.  Bound to the command of a button that fires when clicked.
        /// Adapted from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand DisplayMessageBoxCommand
        {
            get
            {
                if (displayMessageBoxCommand == null)
                {
                    displayMessageBoxCommand = new RelayCommand(
                        param => new EnterMessageWindow(false).Show()
                        );
                }
                return displayMessageBoxCommand;
            }
        }

        #endregion
    }
}