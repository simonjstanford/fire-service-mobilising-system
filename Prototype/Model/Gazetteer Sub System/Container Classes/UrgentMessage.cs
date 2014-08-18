using System;

namespace Prototype.Model.Gazetteer_Sub_System
{
    /// <summary>
    /// A container class that stores a single piece of urgent information for an address.
    /// </summary>
	public class UrgentMessage 
    {
        /// <summary>
        /// The name of the urgent information
        /// </summary>
      public string Name { get; private set; }

        /// <summary>
        /// The urgent message
        /// </summary>
      public string Text { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the urgent information</param>
        /// <param name="text">The urgent message</param>
      public UrgentMessage(string name, string text)
        {
            Name = name;
            Text = text;
        }

	}

}
