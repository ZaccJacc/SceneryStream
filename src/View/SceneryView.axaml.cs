using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Data;
using System.Linq;
using System;
using static SceneryStream.src.Model.ChildRegion;
using static SceneryStream.src.Model.Region;
using SceneryStream.src.Model;

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
