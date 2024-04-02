using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;

namespace SceneryStream.src.View
{
    public partial class PreferencesView : UserControl
    {



        public PreferencesView()
        {
            InitializeComponent();
        }

        

        public void RecallPathSelection(object? sender, SelectionChangedEventArgs e)
        {
            switch (((ListBox)sender).Name)
            {
                case "OtherInstallationList":
                    OtherInstallationField.Text = ((ListBox)sender).SelectedItem as string;
                    break;
            }

        }

    }
}
