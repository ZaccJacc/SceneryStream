using Avalonia.Controls;
using System;

namespace SceneryStream.src.View
{
    public partial class SceneryView : UserControl
    {
        public SceneryView()
        {
            InitializeComponent();
            Map.AddHandler(PointerPressedEvent, ViewModel.SceneryViewModel.SViewModel.FocusRegion, Avalonia.Interactivity.RoutingStrategies.Bubble);
        }
    }
}
