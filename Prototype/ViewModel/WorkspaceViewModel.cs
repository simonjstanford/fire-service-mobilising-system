using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prototype.ViewModel
{
    /// <summary>
    /// An abstract base class used by all workspace classes.  
    /// Allows polymorphism and contains logic for destroying the workspace when it's been removed from the workspace list
    /// Much code is taken from http://msdn.microsoft.com/en-us/library/vstudio/ms229614(v=vs.100).aspx
    /// </summary>
    abstract class WorkspaceViewModel : BaseViewModel, INotifyPropertyChanged, IDisposable
    {
        #region CloseCommand

        RelayCommand closeCommand; //the command that executed OnRequestClose()

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new RelayCommand(param => this.OnRequestClose());

                return closeCommand;
            }
        }

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        /// <summary>
        /// Fires the RequestClose event.
        /// </summary>
        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }


        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        #endregion 
    }
}
