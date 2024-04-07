using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SceneryStream.src.View;

public partial class ServerFormattingView : UserControl
{
    public ServerFormattingView()
    {
        InitializeComponent();
    }

    public void Next(object source, RoutedEventArgs args)
    {
        CentrePages.Next();
    }

    public void Previous(object source, RoutedEventArgs args)
    {
        CentrePages.Previous();
    }

}