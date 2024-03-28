using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using SceneryStream.src.Model;
using System;
using static SceneryStream.src.Model.RegionHandling;

namespace SceneryStream.src.ViewModel;

internal class SceneryViewModel : ObservableObject
{

    private string test = "Hello World";
    public string Test
    {
        get => test;
        set
        {
            test = value;
            NotifyPropertyChanged(nameof(Test));
        }
    }

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


    private Region _displayedRegion = Region.GLOBE;
    public Region DisplayedRegion
    {
        get => _displayedRegion;
        set
        {
            _displayedRegion = value;
            NotifyPropertyChanged(nameof(DisplayedRegion));
        }
    }

    internal void FocusRegion(object? sender, PointerPressedEventArgs args)
    {

        try
        {
            var point = args.GetCurrentPoint((Visual?)args.Source);
            var x = point.Position.X;
            var y = point.Position.Y;
            if (point.Properties.IsLeftButtonPressed)
            {
                switch (SViewModel.DisplayedRegion.ID)
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
                                Regions.USA_WA.Selected = !Regions.USA_WA.Selected;
                            }
                        }
                        else
                        {
                            if (x > 208 && x < 399 && y > 147 && y < 281)
                            {
                                Console.WriteLine("Oregon");
                                if (args.ClickCount == 2)
                                {
                                    Regions.USA_OR.Selected = !Regions.USA_OR.Selected;
                                }
                            }
                            else
                            {
                                if (x > 208 && (x < 325 && y > 281 && y < 292 || x < 313 && y > 292 && y < 338 || x < 308 && y > 338 && y < 361 || x < 325 && y > 361 && y < 389 || x < 347 && y > 389 && y < 426 || x < 364 && y > 426 && y < 450 || x < 389 && y > 450 && y < 487 || x < 413 && y > 487 && y < 522 || x < 397 && y > 522 && y < 597))
                                {
                                    Console.WriteLine("California");
                                    if (args.ClickCount == 2)
                                    {
                                        Regions.USA_CA.Selected = !Regions.USA_CA.Selected;
                                    }
                                }
                                else
                                {
                                    if (x > 332 && x < 472 && y > 300 && y < 332 || x > 308 && x < 453 && y > 332 && y < 390 || x > 328 && x < 452 && y > 390 && y < 406 || x > 355 && x < 443 && y > 406 && y < 442 || x > 388 & x < 428 && y > 442 && y < 497)
                                    {
                                        Console.WriteLine("Nevada");
                                        if (args.ClickCount == 2)
                                        {
                                            Regions.USA_NV.Selected = !Regions.USA_NV.Selected;
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

    internal void SelectChildRegion(ChildRegion region)
    {
        region.Selected = !region.Selected;
        SelectedChildRegions.Add(region);
    }

    private void DisplayRegion(Region region)
    {
        SViewModel.MapSource = region.Map;
        SViewModel.DisplayedRegion = region;
    }

    internal void ResetMap()
    {
        DisplayRegion(Region.GLOBE);
    }
}
