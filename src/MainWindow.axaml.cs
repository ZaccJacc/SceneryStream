using Avalonia.Controls;
using Avalonia.Input;
using SceneryStream.src.Model;
using Avalonia;

namespace SceneryStream.src
{
    public partial class MainWindow : Window
    {

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            this.Hide();
            e.Cancel = true;
            await PreferencesModel.SavePreferences();
            RegionHandling.Regions.SaveSelectedRegions();
        }

        public MainWindow()
        {
            InitializeComponent();
            MovementRegion.PointerPressed += MovementRegion_PointerPressed;
            MovementRegion.PointerMoved += MovementRegion_PointerMoved;
            MovementRegion.PointerReleased += MovementRegion_PointerReleased;
        }

        private bool _mouseDownForWindowMoving = false;
        private PointerPoint _originalPoint;

        private void MovementRegion_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_mouseDownForWindowMoving) return;
            WindowState = WindowState.Normal;
            PointerPoint currentPoint = e.GetCurrentPoint(this);
            Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - _originalPoint.Position.X),
                Position.Y + (int)(currentPoint.Position.Y - _originalPoint.Position.Y));
        }

        private void MovementRegion_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Normal ? WindowState.FullScreen : WindowState.Normal;
                return;
            }
            _mouseDownForWindowMoving = true;
            _originalPoint = e.GetCurrentPoint(this);
            
        }

        private void MovementRegion_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _mouseDownForWindowMoving = false;
        }
    }
}