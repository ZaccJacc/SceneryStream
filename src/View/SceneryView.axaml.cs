using Avalonia.Controls;

namespace SceneryStream.src.View
{
    public partial class SceneryView : UserControl
    {
        public SceneryView()
        {
            InitializeComponent();
            DisplayControl.AddHandler(PointerPressedEvent, ViewModel.SceneryViewModel.SViewModel.FocusRegion, Avalonia.Interactivity.RoutingStrategies.Bubble);
        }
    }
}
