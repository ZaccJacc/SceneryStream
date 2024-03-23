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

    //--Selected Regions--//
    private bool _washington;
    public bool Washington
    {
        get => _washington;
        set
        {
            _washington = value;
            NotifyPropertyChanged(nameof(Washington));
        }
    }

    private bool _oregon;
    public bool Oregon
    {
        get => _oregon;
        set
        {
            _oregon = value;
            NotifyPropertyChanged(nameof(Oregon));
        }
    }

    private bool _california;
    public bool California
    {
        get => _california;
        set
        {
            _california = value;
            NotifyPropertyChanged(nameof(California));
        }
    }

    private bool _nevada;
    public bool Nevada
    {
        get => _nevada; 
        set
        {
            _nevada = value;
            NotifyPropertyChanged(nameof(Nevada));
        }
    }
    //--//

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
                        Console.WriteLine($"X:{x}, Y:{y}");
                        
                        if (x > 208 && x < 431 && y < 147)
                        {
                            Console.WriteLine("Washington");
                            if (args.ClickCount == 2)
                            {
                                Washington = !Washington;
                            }
                        }
                        else
                        {
                            if (x > 208 && x < 399 && y > 147 && y < 281)
                            {
                                Console.WriteLine("Oregon");
                                if (args.ClickCount == 2)
                                {
                                    Oregon = !Oregon;
                                }
                            }
                            else
                            {
                                if (x > 208 && (x < 325 && y > 281 && y < 292 || x < 313 && y > 292 && y < 338 || x < 308 && y > 338 && y < 361 || x < 325 && y > 361 && y < 389 || x < 347 && y > 389 && y < 426 || x < 364 && y > 426 && y < 450 || x < 389 && y > 450 && y < 487 || x < 413 && y > 487 && y < 522 || x < 397 && y > 522 && y < 597))
                                {
                                    Console.WriteLine("California");
                                    if (args.ClickCount == 2)
                                    {
                                        California = !California;
                                    }
                                }
                                else
                                {
                                    if (x > 332 && x < 472 && y > 300 && y < 332 || x > 308 && x < 453 && y > 332 && y < 390 || x > 328 && x < 452 && y > 390 && y < 406 || x > 355 && x < 443 && y > 406 && y < 442 || x > 388 & x < 428 && y > 442 && y < 497)
                                    {
                                        Console.WriteLine("Nevada");
                                        if (args.ClickCount == 2)
                                        {
                                            Nevada = !Nevada;
                                        }
                                    }
                                }
                            }
                        }
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
