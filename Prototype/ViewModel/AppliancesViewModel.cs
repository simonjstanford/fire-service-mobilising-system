using Prototype.Model.Resource_Sub_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using Prototype.Model.Global_Container_Classes;
using System.Windows.Input;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using Prototype.Model;
using Prototype.Model.Global_Classes;

namespace Prototype.ViewModel
{
    /// <summary>
    /// The ViewModel for the AppliancesView.  XAML controls in the View are bound to properties in this class.
    /// This class allows the user to view a list of all appliances, and select one to see further information.
    /// </summary>
    class AppliancesViewModel : WorkspaceViewModel
    {
        #region Fields

        private RelayCommand viewApplianceCommand; //the button to search for addresses is bound to this property in XAML to view an individual appliance()

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database">The database object used to retrieve info</param>
        /// <param name="resourceDatabase">Field used to pass to all resource objects so that they can access the database</param>
        /// <param name="applianceDatabase">Field used to pass to all appliance objects so that they can access the database</param>
        public AppliancesViewModel()
        {
            SelectedItem = Tools.Appliances[0];
        }

        #region Properties

        /// <summary>
        /// The text that appears in the tab header
        /// </summary>
        public override string DisplayName
        {
            get { return "All Appliances"; }
        }

        /// <summary>
        /// Retrieves a list of appliances.  Bound to the XAML dataset.
        /// </summary>
        public ObservableCollection<ApplianceDataView> Appliances
        {
            get 
            {
                return Tools.Appliances; 
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The item selected in the dataset
        /// </summary>
        public ApplianceDataView SelectedItem { get; set; }

        /// <summary>
        /// Returns a command that searches for matching addresses.  Bound to the command of a button that fires when clicked.
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
    }
}
