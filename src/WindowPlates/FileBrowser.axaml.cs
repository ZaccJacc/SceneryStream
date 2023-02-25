using Avalonia.Controls;

namespace SceneryStream.src.WindowPlates
{
    public partial class FileBrowser : Window
    {
        public FileBrowser()
        {
            InitializeComponent();
            Utility.FileBrowser browser = new Utility.FileBrowser();
        }
    }
}
