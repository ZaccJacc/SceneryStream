using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Topten.RichTextKit.Utils;

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