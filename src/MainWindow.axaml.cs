using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using SceneryStream.src.Model;
using SceneryStream.src.View;
using System;
using System.ComponentModel;
using System.IO;
using Utility;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using SceneryStream.src.ViewModel;
using System.Diagnostics;

namespace SceneryStream.src
{
    public partial class MainWindow : Window
    {

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            try
            {
                NetworkDrive.RemoveDriveByConsole(App.Preferences.DriveLetter);
                RegionHandling.Regions.RemoveShellLinks();
            }
            catch (Exception)
            {
                Debug.WriteLine("Shutdown incomplete");
            }
            //WindowState = WindowState.Minimized;
            //e.Cancel = true;
            await PreferencesModel.SavePreferences();
            RegionHandling.Regions.SaveSelectedRegions();
            ApplicationViewModel.ExitApplication();
        }

        public MainWindow()
        {
            InitializeComponent();
            MovementRegion.PointerPressed += MovementRegion_PointerPressed;
            MovementRegion.PointerMoved += MovemenetRegion_PointerMoved;
            MovementRegion.PointerReleased += MovementRegion_PointerReleased;
        }

        private bool _mouseDownForWindowMoving = false;
        private PointerPoint _originalPoint;

        private void MovemenetRegion_PointerMoved(object? sender, PointerEventArgs e)
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