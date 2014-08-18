using Prototype.Model.Gazetteer_Sub_System;
using System.Windows;

namespace Prototype.View.Popup_Windows
{
    /// <summary>
    /// Interaction logic for UrgentMessageWindow.xaml
    /// </summary>
    public partial class UrgentMessageWindow : Window
    {
        public UrgentMessageWindow(UrgentMessage[] messages)
        {
            InitializeComponent();

            //add each urgent message to the window
            foreach (UrgentMessage message in messages)
                listBoxUrgentMessages.Items.Add(message.Text);
        }
    }
}
