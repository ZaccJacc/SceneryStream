using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SceneryStream.src.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SceneryStream.src.ViewModel;

internal class SceneryViewModel : ObservableObject
{

    private static readonly SceneryViewModel _sViewModel = new();
    public static SceneryViewModel SViewModel
    {
        get => _sViewModel;
    }

    private Bitmap _mapSource = Region.GLOBE.Map;
    public Bitmap MapSource
    {
        get => _mapSource;
        set
        {
            _mapSource = value;
            NotifyPropertyChanged(nameof(MapSource));
        }
    }


    private static Region _displayedRegion = Region.GLOBE;

    internal void FocusRegion(object? sender, PointerPressedEventArgs args)
    {

        try
        {
            var point = args.GetCurrentPoint((Avalonia.Visual?)args.Source);
            var x = point.Position.X;
            var y = point.Position.Y;
            if (point.Properties.IsLeftButtonPressed)
            {
                switch (_displayedRegion.ID)
                {
                    case Region.RegionID.GLOBE:
                        if (((x < 530 && y > 228) || (x < 192 && y < 228)) && y < 404)
                        {
                            DisplayRegion(Region.USA);
                        }
                        else
                        {
                            if (x < 530 && y > 404)
                            {
                                Console.WriteLine("LATAM region");
                            }
                            else
                            {
                                if (x > 192 && x < 530 && y < 228)
                                {
                                    Console.WriteLine("CAN region");
                                }
                                else
                                {
                                    if (x > 530 && x < 833 && y < 284)
                                    {
                                        Console.WriteLine("EUR region");
                                    }
                                    else
                                    {
                                        if (x > 530 && x < 833 && y > 284)
                                        {
                                            Console.WriteLine("AFR region");
                                        }
                                        else
                                        {
                                            if (x > 833 && y < 465)
                                            {
                                                Console.WriteLine("ASI region");
                                            }
                                            else
                                            {
                                                Console.WriteLine("OCE region");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case Region.RegionID.USA:
                        break;

                    case Region.RegionID.CAN:
                        break;

                    case Region.RegionID.LATAM: 
                        break;

                    case Region.RegionID.EUR: 
                        break;

                    case Region.RegionID.AFR: 
                        break;

                    case Region.RegionID.ASI: 
                        break;

                    case Region.RegionID.OCE: 
                        break;
                
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("[!] Malformed pointer event args!");
        }
    }

    private void DisplayRegion(Region region)
    {
        SViewModel.MapSource = region.Map;
        _displayedRegion = region;
    }

    internal void ResetMap()
    {
        DisplayRegion(Region.GLOBE);
    }
}
