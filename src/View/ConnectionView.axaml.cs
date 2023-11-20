using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using SceneryStream.src.ViewModel;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SceneryStream.src.View
{
    public partial class ConnectionView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ConnectionView()
        {
            InitializeComponent();
            StatusIndicator.AddHandler(PointerPressedEvent, ConnectionViewModel.CViewModel.ToggleConnection, RoutingStrategies.Bubble);
        }
    }
}
