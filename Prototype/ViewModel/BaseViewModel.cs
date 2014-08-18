using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prototype.ViewModel
{
    /// <summary>
    /// An abstract base class used by MainWindowViewModel and WorkspaceViewModel.
    /// Provides INotifyPropertyChanged implementation and control naming functionality
    /// Much code is taken from http://msdn.microsoft.com/en-us/library/vstudio/ms229614(v=vs.100).aspx
    /// </summary>
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Fields

        protected static ObservableCollection<WorkspaceViewModel> workspaces; //the collection of tabs in the Tab Control.  Bound to the XAML tab control
        protected static int totalTabs; //the total number of tabs that is currently in the tab control.  Used to select the last added tab

        #endregion

        #region Properties

        /// <summary>
        /// The text that appears in the tab header - overriden by each sub class
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        /// <summary>
        /// The index of the currently selected tab.  Bound to the XAML Tab control
        /// </summary>
        public static int TabIndex { get; set; }

        /// <summary>
        /// A singleton that returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                //If there is no workspace, a new object is created and returned that contains default windows
                if (workspaces == null)
                {
                    workspaces = new ObservableCollection<WorkspaceViewModel>();
                    //workspaces.Add(new MapViewModel());

                    //link the Observable collection to CollectionChanged() - it fires whenever the collection changes
                    workspaces.CollectionChanged += workspaces_CollectionChanged;

                    this.openTab(new MapViewModel());
                }
                //else the current object is returned.
                return workspaces;
            }
        }

        #endregion

        #region Methods for updating the tab when a property changes

        /// <summary>
        /// Executes when the Workspaces observable collection changes.
        /// For each new workspace that has been added to the collection, the RequestClose event is linked to OnWorkspaceRequestClose()
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        void workspaces_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // for each new tab link the RequestClose event to the OnWorkspaceRequestClose() method
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            //for each removed tab, remove the OnWorkspaceRequestClose() from their RequestClose event
            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        //logic for instance properties

        /// <summary>
        /// Event for when a property value is changed in this class.  Forces the bound XAML control to update.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method that is called when a property value is changed.
        /// </summary>
        /// <param name="info">The name of the property that has changed</param>
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //logic for static properties

        /// <summary>
        /// Event for when a property value is changed in this class.  Forces the bound XAML control to update.
        /// </summary>
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        /// <summary>
        /// Method that is called when a property value is changed.
        /// </summary>
        /// <param name="info">The name of the property that has changed</param>
        protected void NotifyStaticPropertyChanged(String info)
        {
            if (StaticPropertyChanged != null)
            {
                StaticPropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new workspace to the list of workspaces, so that they are displayed as a tab item.
        /// Shared by all command properties.
        /// </summary>
        /// <param name="workspace">The workspace type to add</param>
        protected void openTab(WorkspaceViewModel workspace)
        {
            workspaces.Add(workspace);

            //Set the selected tab index to the last tab
            TabIndex = ++totalTabs - 1;
            //MessageBox.Show(TabIndex.ToString());
            NotifyStaticPropertyChanged("TabIndex"); //notify the XAML that this property has changed.
        }

        /// <summary>
        /// Removes a workspace from the Workspace observable collection - executes whenever a workspace requests to be closed
        /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        /// </summary>
        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel; //retrieve the workspace that is closing
            workspace.Dispose(); //destroy it
            this.Workspaces.Remove(workspace); //remove it from the list
            totalTabs--; //reduce the total number of tabs by one.
        }

        #endregion
    }
}
