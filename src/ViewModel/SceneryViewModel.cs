using Avalonia.Input;
using Avalonia.Media.Imaging;
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

    private Bitmap _mapSource = new(@$"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Assets/Map/worldmap.png");
    public Bitmap MapSource
    {
        get => _mapSource;
        set
        {
            _mapSource = value;
            NotifyPropertyChanged(nameof(MapSource));
        }
    }

    private enum Region
    {
        GLOBE=0,
        USAN=1,
        CAN=2,
        USAS=3,
        EUR=4,
        AFR=5,
        ASI=6,
        OCE=7
    }

    private Region _displayedRegion = Region.GLOBE;


    internal void FocusRegion(object? sender, PointerPressedEventArgs args)
    {

        try
        {
            var point = args.GetCurrentPoint((Avalonia.Visual?)args.Source);
            var x = point.Position.X;
            var y = point.Position.Y;
            if (point.Properties.IsLeftButtonPressed)
            {
                switch (_displayedRegion)
                {
                    case Region.GLOBE:
                        if (((x < 530 && y > 228) || (x < 192 && y < 228)) && y < 404)
                        {
                            Console.WriteLine("USA region");
                            MapSource = new(@$"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Assets/Map/USAmap.png");
                            _displayedRegion = Region.USAN;
                        }
                        else
                        {
                            if (x < 530 && y > 404)
                            {
                                Console.WriteLine("USA-S region");
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

                    case Region.USAN:
                        break;

                    case Region.CAN:
                        break;

                    case Region.USAS: 
                        break;

                    case Region.EUR: 
                        break;

                    case Region.AFR: 
                        break;

                    case Region.ASI: 
                        break;

                    case Region.OCE: 
                        break;
                
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("[!] Malformed pointer event args!");
        }
    }
}
