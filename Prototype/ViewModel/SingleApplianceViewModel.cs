using Prototype.Model.Resource_Sub_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using Prototype.Model.Global_Container_Classes;
using System.Windows;
using Prototype.Model;
using Prototype.Model.Global_Classes;

namespace Prototype.ViewModel
{
    /// <summary>
    /// The ViewModel for the SingleApplianceView.  XAML controls in the View are bound to properties in this class.
    /// This class allows the user to view details of a single appliance.
    /// </summary>
    class SingleApplianceViewModel : WorkspaceViewModel
    {
        #region Fields

        private Appliance appliance; //the resource to display information for
        private ApplianceType[] applianceTypes; //backing field for property that contains a list of all incident types - bound to a combo box
        private ResourceStatus[] statusTypes;//backing field for property that contains a list of all status types - bound to a combo box
        private Address[] addresses; //backing field for property that contains a list of addresses - bound to a combo box
        private Base[] bases; //backing field for property that contains a list of all bases - bound to a combo box

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appliance">The appliance to display information for</param>
        /// <param name="controller">A database object to retrieve database data</param>
        public SingleApplianceViewModel(Appliance appliance)
        {
            this.appliance = appliance;
            applianceTypes = Tools.ResourceControllerDB.GetAllApplianceTypes();
            statusTypes = Tools.ResourceControllerDB.GetAllResourceStates();
            addresses = Tools.ResourceControllerDB.GetAllResourceAddresses();
            bases = Tools.ResourceControllerDB.GetAllResourceBases();
        }

        #region Properties

        /// <summary>
        /// The text that appears in the tab header
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return Appliance.CallSign;
            }
        }

        /// <summary>
        /// The resource to display information for
        /// </summary>
        public Appliance Appliance 
        {
            get { return appliance; } 
        }

        /// <summary>
        /// A collection of historical data for the appliance, detailing movements and status changes.
        /// </summary>
        public LogDataView[] History 
        { 
            get { return appliance.History; }
        }

        #endregion

        #region Properties for the appliance types combo box

        /// <summary>
        /// List of all appliances types - bound the appliance types combo box
        /// </summary>
        public ApplianceType[] ApplianceTypes
        {
            get { return applianceTypes; }
        }

        /// <summary>
        /// An individual ApplianceType - bound to the appliance types combo box SelectedItem.
        /// Used to automatically select the appropriate appliance type for the appliance, 
        /// and expose a way to change the type.
        /// </summary>
        public ApplianceType ApplianceTypeSelectedItem
        {
            get 
            {
                foreach (ApplianceType type in applianceTypes)
                {
                    if (type.Name == appliance.Type.Name)
                        return type;
                }

                return null; 
            }
            set { Appliance.Type = value; }
        }

        #endregion

        #region Properties for the status types combo box

        /// <summary>
        /// List of all status types - bound the status types combo box
        /// </summary>
        public ResourceStatus[] StatusTypes
        {
            get { return statusTypes; }
        }

        /// <summary>
        /// An individual ResourceStatus - bound to the status types combo box SelectedItem.
        /// Used to automatically select the appropriate status type for the appliance, 
        /// and expose a way to change the type.
        /// </summary>
        public ResourceStatus StatusTypeSelectedItem
        {
            get
            {
                foreach (ResourceStatus type in statusTypes)
                {
                    if (type.Code == appliance.CurrentResourceStatus.Code)
                        return type;
                }

                return null;
            }
            set { Appliance.CurrentResourceStatus = value; }
        }

        #endregion

        #region Properties for the addresses combo box

        /// <summary>
        /// List of all addressses - bound the addressses combo box
        /// </summary>
        public Address[] Addresses
        {
            get { return addresses; }
        }

        /// <summary>
        /// An individual Addresses - bound to the addresses combo box SelectedItem.
        /// Used to automatically select the appropriate address for the appliance, 
        /// and expose a way to change the current address.
        /// </summary>
        public Address CurrentAddressSelectedItem
        {
            get
            {
                foreach (Address address in addresses)
                {
                    if (address.Equals(appliance.CurrentAddress))
                        return address;
                }

                return null;
            }
            set { Appliance.CurrentAddress = value; }
        }

        #endregion

        #region Properties for the bases combo box

        /// <summary>
        /// List of all bases - bound the bases combo box
        /// </summary>
        public Base[] Bases
        {
            get { return bases; }
        }

        /// <summary>
        /// An individual Base - bound to the bases combo box SelectedItem.
        /// Used to automatically select the appropriate base for the appliance, 
        /// and expose a way to change the current base.
        /// </summary>
        public Base BaseSelectedItem
        {
            get
            {
                foreach (Base item in bases)
                {
                    if (item.Equals(appliance.Base))
                        return item;
                }

                return null;
            }
            set { Appliance.Base = value; }
        }

        #endregion
    }
}
