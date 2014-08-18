using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prototype.Model.Gazetteer_Sub_System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using Prototype.Model.Global_Container_Classes;
using Prototype.Model.Incident_Sub_System;
using Prototype.Model.Incident_Sub_System.Interfaces;
using Prototype.View;
using Prototype.Model.Global_Classes;
using Prototype.View.Popup_Windows;

namespace Prototype.ViewModel
{
    /// <summary>
    /// The ViewModel for the NewIncidentView.  XAML controls in the View are bound to properties in this class.
    /// This class allows the user to search for incident types on a database and address via Google Maps.
    /// </summary>
    class NewIncidentViewModel : WorkspaceViewModel
    {
        #region Fields

        private List<IncidentType> fullList; //contains a reference list of incident types
        private ObservableSortedList<IncidentType> filteredIncidentTypes; //a list of incident types that is filtered by the user     
        private RelayCommand searchAddressCommand; //the button to search for addresses is bound to this property in XAML to execute SearchAddress()
        private ObservableCollection<Address> matchingAddresses; //backing field of the readonly MatchingAddresses property. Updated when SearchAddress() is executed.
        private Address selectedAddress; //backing field for the SelectedAddress property.  Bound to the addresses listbox via the SelectedItem property.
        private RelayCommand createIncidentCommand; //the button to create the incident in the database is bound to this property in XAML to execute SearchAddress()
        private Incident incident; //the newly created incident
        private bool incidentCreated; //a bool to check if the incident has been created.  An incident can only be created once.

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gazetterDatabase">Object that enables connection to the database to retrieve gazetteer related information</param>
        /// <param name="incidentDatabase">Object that enables connection to the database to retrieve information for a single incident.</param>
        public NewIncidentViewModel()
        {
            //Set default values for properties that are bound to text box controls
            County = "Northamptonshire";

            //instantiates the incident type list
            matchingAddresses = new ObservableCollection<Address>();

            //the ObservableSortedList Contains() method only checks if the reference types are the same.  
            //Thus, these two lists must share the same reference to stop duplicate entries when adding/removing
            fullList = BuildIncidentTypes();
            filteredIncidentTypes = new ObservableSortedList<IncidentType>(fullList);

            incidentCreated = false;

        }

        #region Properties

        /// <summary>
        /// The text that appears in the tab header
        /// </summary>
        public override string DisplayName
        {
            get { return "New Incident"; }
        }

        /// <summary>
        /// The list of incident types - bound to the incident type list box in XAML.
        /// This list is filtered through the IncidentSearchKeyword property.
        /// </summary>
        public ObservableSortedList<IncidentType> IncidentTypes
        {
            get { return filteredIncidentTypes; }
        }

        /// <summary>
        /// A property bound to the XAML incident type keyword search text box that filters the incident list.
        /// BuildIncidentTypesList() filters items in the matchingAddresses list using the fullList field.
        /// </summary>
        public string IncidentSearchKeyword
        {
            set { BuildIncidentTypesList(value); }
        }

        /// <summary>
        /// The phone number of the original caller
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// The exchange that passed the details of the original caller
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// The keywords used by the operator to find an address
        /// </summary>
        public string AddressSearchKeyword { get; set; }

        /// <summary>
        /// The county to filter results to
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Any other relevant incident information
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Stores the selected IncidentType object in the incident type list.  Bound via the SelectItem property.
        /// </summary>
        public IncidentType SelectedIncidentType { get; set; }

        /// <summary>
        /// Contains the currently selected Address object in the matching addresses list box.  Bound via the SelectedItem property.
        /// </summary>
        public Address SelectedAddress 
        { 
            get { return selectedAddress; }
            set
            {
                selectedAddress = value;  //assign the selected Address object to the selectedAddress field

                //notify the XAML that the SelectedAddress property has changed.  
                //This will update all XAML controls bound to specific data within this object.
                NotifyPropertyChanged("SelectedAddress"); 

                //The 'County' property is a special case as the user updates this field when searching for addresses in different counties.
                //Assign the county property the appropriate value in the selectedAddress object and then notify the XAML that this property has changed.
                if (selectedAddress != null)
                {
                    County = selectedAddress.County;
                    NotifyPropertyChanged("County");
                }

                //search for any urgent messages associated with the address.  Display any found messages
                UrgentMessage[] messages = Tools.GazetteerDB.FindAdditionalInfo(selectedAddress);
                if (messages.Count() > 0)
                    new UrgentMessageWindow(messages).Show();
            }
        }

        /// <summary>
        /// The list of incident types - bound to the incident type list box in XAML
        /// </summary>
        public ObservableCollection<Address> MatchingAddresses
        {
            get { return matchingAddresses; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a list of incidents types to be displayed to the user.
        /// </summary>
        /// <param name="searchText">
        /// An optional search parameter.  
        /// If left blank, all incident types are returned.
        /// If specified, both the incident type name and any associtated keywords are matched.</param>
        /// <returns>A list of incident types.</returns>
        internal void BuildIncidentTypesList(string searchText = "")
        {
            //first search through the filtered list and remove any incident types that don't have a matching name or keyword
            List<IncidentType> toDelete = new List<IncidentType>();
            foreach (IncidentType type in IncidentTypes)
            {
                bool matching = false;
                if (type.Name.ToUpper().Contains(searchText.ToUpper()))
                    matching = true;

                foreach (string keyword in type.Keywords)
                {
                    if (keyword.ToUpper().Contains(searchText.ToUpper()))
                        matching = true;
                }

                if (!matching)
                    toDelete.Add(type);
            }

            foreach (IncidentType type in toDelete)
                filteredIncidentTypes.Remove(type);

            //then search through the full list and add any incident types that DO have a matching name or keyword
            List<IncidentType> toAdd = new List<IncidentType>();
            foreach (IncidentType type in fullList)
            {
                bool matching = false;
                if (type.Name.ToUpper().Contains(searchText.ToUpper()))
                    matching = true;

                foreach (string keyword in type.Keywords)
                {
                    if (keyword.ToUpper().Contains(searchText.ToUpper()))
                        matching = true;
                }

                if (matching)
                    toAdd.Add(type);
            }

            foreach (IncidentType type in toAdd)
            {
                if (!filteredIncidentTypes.Contains(type))
                    filteredIncidentTypes.Add(type);
            }
        }

        /// <summary>
        /// Adds all incident types definitions to the incident type list
        /// </summary>
        internal List<IncidentType> BuildIncidentTypes()
        {
            return Tools.IncidentControllerDB.GetAllIncidentTypes();
        }

        #endregion

        #region Search Address Command

        /// <summary>
        /// Returns a command that searches for matching addresses.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand SearchAddressCommand
        {
            get
            {
                if (searchAddressCommand == null)
                {
                    searchAddressCommand = new RelayCommand(
                        param => this.searchAddress()
                        );
                }
                return searchAddressCommand;
            }
        }

        /// <summary>
        /// Searches Google Maps for a matching address.
        /// </summary>
        internal void searchAddress()
        {
            matchingAddresses.Clear(); //first, clear any previous results

            //Use the gazetteer controller to find and matching addresses
            foreach (Address address in Tools.AddressSearch.FindAddress(AddressSearchKeyword, County))
                matchingAddresses.Add(address);
        }

        #endregion

        #region Create Incident Command

        /// <summary>
        /// Returns a command that searches for matching addresses.  Bound to the command of a button that fires when clicked.
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        public ICommand CreateIncidentCommand
        {
            get
            {
                if (createIncidentCommand == null)
                {
                    createIncidentCommand = new RelayCommand(
                        param => this.createIncident()
                        );
                }
                return createIncidentCommand;
            }
        }

        /// <summary>
        /// Creates the incident in the database
        /// </summary>
        internal void createIncident()
        {
            if (SelectedIncidentType != null && SelectedAddress != null)
            {
                if (!incidentCreated)
                {
                    incident = Tools.IncidentControllerDB.CreateIncident(Properties.Settings.Default.LoggedInUserId, TelephoneNumber, Exchange, SelectedIncidentType, SelectedAddress, Details, -1);
                    incidentCreated = true;
                    ApplianceSelectionWindow mobiliser = new ApplianceSelectionWindow(incident);
                    mobiliser.Show();           
                }
            }
            else
            {
                MessageBox.Show("An incident type and address must be selected before an incident can be created.");
            }
        }

        #endregion

        #region Close Command that also opens the new incident tab

        RelayCommand newIncidentCloseCommand; //the command that executed OnRequestClose()

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand NewIncidentCloseCommand
        {
            get
            {
                if (newIncidentCloseCommand == null)
                    newIncidentCloseCommand = new RelayCommand(param => this.OnRequestClose());

                return newIncidentCloseCommand;
            }
        }

        /// <summary>
        /// Fires the RequestClose event.
        /// </summary>
        void OnRequestClose()
        {
            if (incident != null)
                this.openTab(new SingleIncidentViewModel(new Incident(incident.IncidentNumber)));
            CloseCommand.Execute(null);
        }

        #endregion
    }
}