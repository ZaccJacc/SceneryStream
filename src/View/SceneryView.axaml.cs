using Avalonia.Controls;
using Avalonia.Interactivity;
using Mapsui;
using Mapsui.UI;
using Mapsui.UI.Avalonia;
using System;

namespace SceneryStream.src.View
{
    public partial class SceneryView : UserControl
    {

        MapControl mapControl = new MapControl();

        public SceneryView()
        {
            mapControl.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            mapControl.Map.BackColor = Mapsui.Styles.Color.Opacity(Mapsui.Styles.Color.Gray, 0);
            mapControl.Margin = new Avalonia.Thickness(0, 0, 7, 15);

            Content = mapControl;

            mapControl.DoubleTapped += MapControl_DoubleTapped;

        }

        private void MapControl_DoubleTapped(object? sender, RoutedEventArgs e)
        {
            //
        }
    }
}
