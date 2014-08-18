using Prototype.Model.Global_Classes;
using Prototype.Model.Resource_Sub_System.Container_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Model.Global_Container_Classes
{
    /// <summary>
    /// A container class that stores information relating to a specific incident type
    /// </summary>
    public class IncidentType : IEquatable<IncidentType>, IComparable, INotifyPropertyChanged
    {
        private List<string> keywords; //backing field for the Keywords property

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The unique name of the incident type</param>
        /// <param name="keywords">Any keywords associated with the incident type.  This is used to help filter an incident type list</param>
        /// <param name="description">A description of the incident type.</param>
        public IncidentType(string name, string description)
        {
            Name = name;
            Description = description;
            keywords = new List<string>();
        }

        #region Properties

        /// <summary>
        /// The unique name of the incident type
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Any keywords associated with the incident type.  This is used to help filter an incident type list
        /// </summary>
        public string[] Keywords { get { return keywords.ToArray(); } }

        /// <summary>
        /// A description of the incident type.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// A collection of appliance types and quantites required for this incident
        /// </summary>
        public Dictionary<ApplianceType, int> Response
        {
            get
            {
                return Tools.IncidentControllerDB.GetIncidentTypeResonse(Name);
            }
        }

        /// <summary>
        /// Creates a formatted description string of the Response property
        /// </summary>
        public string ResponseDescription
        {
            get
            {
                string text = "";

                //add each appliance type to the string
                foreach (KeyValuePair<ApplianceType, int> item in Response)
                    text += item.Value + "x " + item.Key.Name + ", ";

                //remove the last ', '
                text = text.Substring(0, text.Length - 2);

                return text;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a keyword to this incident type.  
        /// Note this does not add the keywords into the database.  Instead, it is used as a way to construct this object.
        /// </summary>
        /// <param name="newKeyword"></param>
        public void AddKeyword(string newKeyword)
        {
            keywords.Add(newKeyword);
        }

        /// <summary>
        /// Implementation of IComparable.  Used to help with ordering incident types by name.
        /// </summary>
        public int CompareTo(object obj)
        {
            IncidentType other = (IncidentType)obj;
            return String.Compare(this.Name, other.Name);
        }

        /// <summary>
        /// Implementation of IEquatable.
        /// </summary>
        public bool Equals(IncidentType other)
        {
            if (other == null)
                return false;
            return
                this.Name.Equals(other.Name);
        }

        /// <summary>
        /// Overrides ToString() to display just the incident type name;
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion

        //Used to enable WPF two way binding of controls
        #region INotifyPropertyChanged Implementation

        //from http://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        public event PropertyChangedEventHandler PropertyChanged;

        private void changed()
        {
            OnPropertyChanged("changed");
        }

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
