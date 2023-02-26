using Avalonia.Controls;

namespace SceneryStream.src.View
{
    public partial class FileBrowserView : Window
    {
        public FileBrowserView()
        {
            InitializeComponent();
            Utility.FileBrowser browser = new Utility.FileBrowser();
        }
    }
}
