using Avalonia.Controls;
using System;

namespace SceneryStream.src.View
{
    public partial class SceneryView : UserControl
    {
        public SceneryView()
        {
            InitializeComponent();
            /*
            WorldMap.PointerPressed += (OArgs, args) =>
            {
                var point = args.GetCurrentPoint(WorldMap);
                var x = point.Position.X;
                var y = point.Position.Y;
                if (point.Properties.IsLeftButtonPressed)
                {
                    Console.WriteLine(point);
                }
                if (point.Properties.IsRightButtonPressed)
                {
                    // right button pressed
                }
            };*/
            WorldMap.AddHandler(PointerPressedEvent, ViewModel.SceneryViewModel.SViewModel.FocusRegion, Avalonia.Interactivity.RoutingStrategies.Bubble);
        }
    }
}
