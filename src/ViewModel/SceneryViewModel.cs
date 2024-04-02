using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SceneryStream.src.Model;
using System;
using System.Diagnostics;
using static SceneryStream.src.Model.RegionHandling;

namespace SceneryStream.src.ViewModel;

internal partial class SceneryViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{


    [ObservableProperty]
    private bool _helpOpen = false;

    [ObservableProperty]
    private bool _selectSceneryDialogOpen = false;

    [ObservableProperty]
    private ChildRegion _selectSceneryDialogRegion = Regions.GetRegionByID(ChildRegion.ChildRegionID.WA, Region.RegionID.USA);

    [RelayCommand]
    private void ToggleHelp() => HelpOpen = !HelpOpen;

    [RelayCommand]
    private void DismissSceneryDialog() => SViewModel.SelectSceneryDialogOpen = false;

    private static readonly SceneryViewModel _sViewModel = new();
    public static SceneryViewModel SViewModel
    {
        get => _sViewModel;
    }

    [ObservableProperty]
    private Region _displayedRegion = Regions.GetRegionByID(Region.RegionID.GLOBE);


    internal void FocusRegion(object? sender, PointerPressedEventArgs args)
    {

        try
        {
            var point = args.GetCurrentPoint((Visual?)args.Source);
            var x = point.Position.X;
            var y = point.Position.Y;

            switch (SViewModel.DisplayedRegion.ID)
            {
                case Region.RegionID.GLOBE:
                    if (((x < 530 && y > 228) || (x < 192 && y < 228)) && y < 404)
                    {
                        DisplayRegion(Regions.GetRegionByID(Region.RegionID.USA));
                    }
                    else
                    {
                        if (x < 530 && y > 404)
                        {
                            Debug.WriteLine("LATAM region");
                        }
                        else
                        {
                            if (x > 192 && x < 530 && y < 228)
                            {
                                Debug.WriteLine("CAN region");
                            }
                            else
                            {
                                if (x > 530 && x < 833 && y < 284)
                                {
                                    Debug.WriteLine("EUR region");
                                }
                                else
                                {
                                    if (x > 530 && x < 833 && y > 284)
                                    {
                                        Debug.WriteLine("AFR region");
                                    }
                                    else
                                    {
                                        if (x > 833 && y < 465)
                                        {
                                            Debug.WriteLine("ASI region");
                                        }
                                        else
                                        {
                                            Debug.WriteLine("OCE region");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case Region.RegionID.USA:
                    Debug.WriteLine($"X:{x}, Y:{y}");

                    if (x > 208 && x < 431 && y < 147)
                    {
                        Debug.WriteLine("Washington");
                        RationalisePointerClick(point, args, ChildRegion.ChildRegionID.WA, Region.RegionID.USA);
                            
                    }
                    else
                    {
                        if (x > 208 && x < 399 && y > 147 && y < 281)
                        {
                            Debug.WriteLine("Oregon");
                            RationalisePointerClick(point, args, ChildRegion.ChildRegionID.OR, Region.RegionID.USA);
                        }
                        else
                        {
                            if (x > 208 && (x < 325 && y > 281 && y < 292 || x < 313 && y > 292 && y < 338 || x < 308 && y > 338 && y < 361 || x < 325 && y > 361 && y < 389 || x < 347 && y > 389 && y < 426 || x < 364 && y > 426 && y < 450 || x < 389 && y > 450 && y < 487 || x < 413 && y > 487 && y < 522 || x < 397 && y > 522 && y < 597))
                            {
                                Debug.WriteLine("California");
                                RationalisePointerClick(point, args, ChildRegion.ChildRegionID.CA, Region.RegionID.USA);
                            }
                            else
                            {
                                if (x > 332 && x < 472 && y > 300 && y < 332 || x > 308 && x < 453 && y > 332 && y < 390 || x > 328 && x < 452 && y > 390 && y < 406 || x > 355 && x < 443 && y > 406 && y < 442 || x > 388 & x < 428 && y > 442 && y < 497)
                                {
                                    Debug.WriteLine("Nevada");
                                    RationalisePointerClick(point, args, ChildRegion.ChildRegionID.NV, Region.RegionID.USA);
                                }
                                else
                                {
                                    if (x > 451 && x < 468 && y > 58 || x > 446 && x < 248 && y > 58 && y < 80 || x > 440 && x < 468 && y > 80 && y < 99 || x > 439 && x < 475 && y > 99 && y < 118 || x > 433 && x < 491 && y > 118 && y < 143 || x > 424 && x < 485 && y > 143 && y < 173 || x > 418 && x < 506 && y > 173 && y < 206 || x > 415 && x < 557 && y > 206 && y < 227 || x > 399 && x < 545 && y > 227 && y < 304)
                                    {
                                        Debug.WriteLine("Idaho");
                                        RationalisePointerClick(point, args, ChildRegion.ChildRegionID.ID, Region.RegionID.USA);
                                    }
                                    else
                                    {
                                        if (x > 465 && x < 590 && y > 312 && y < 353 || x > 451 && x < 590 && y > 312 && y < 353 || x > 451 && x < 584 && y > 353 && y < 419 || x > 441 && x < 576 && y > 419 && y < 486)
                                        {
                                            Debug.WriteLine("Utah");
                                            RationalisePointerClick(point, args, ChildRegion.ChildRegionID.UT, Region.RegionID.USA);
                                        }
                                        else
                                        {
                                            if (x > 399 && x < 553 && y > 486 && y < 630 || x > 476 && x < 546 && y > 630 && y < 672)
                                            {
                                                Debug.WriteLine("Arizona");
                                                RationalisePointerClick(point, args, ChildRegion.ChildRegionID.AZ, Region.RegionID.USA);
                                            }
                                            else
                                            {
                                                if (x > 470 && x < 736 && y > 100 && y < 146 || x > 498 && x < 729 && y > 146 && y < 201 || x > 505 && x < 726 && y > 201 && y < 230)
                                                {
                                                    Debug.WriteLine("Montana");
                                                    RationalisePointerClick(point, args, ChildRegion.ChildRegionID.MT, Region.RegionID.USA);
                                                }
                                                else
                                                {
                                                    if (x > 558 && x < 724 && y > 230 && y < 242 || x > 540 && x < 713 && y > 242 && y < 362)
                                                    {
                                                        Debug.WriteLine("Wyoming");
                                                        RationalisePointerClick(point, args, ChildRegion.ChildRegionID.WY, Region.RegionID.USA);
                                                    }
                                                    else
                                                    {
                                                        if (x > 590 && x < 765 && y > 364 && y < 382 || x > 576 && x < 757 && y > 382 && y < 499)
                                                        {
                                                            Debug.WriteLine("Colorado");
                                                            RationalisePointerClick(point, args, ChildRegion.ChildRegionID.CO, Region.RegionID.USA);
                                                        }
                                                        else
                                                        {
                                                            if (x > 572 && x < 727 && y > 499 && y < 519 || x > 549 && x < 719 && y > 519 && y < 669)
                                                            {
                                                                Debug.WriteLine("New Mexico");
                                                                RationalisePointerClick(point, args, ChildRegion.ChildRegionID.NM, Region.RegionID.USA);
                                                            }
                                                            else
                                                            {
                                                                if (x > 737 && x < 889 && y > 109 && y < 207)
                                                                {
                                                                    Debug.WriteLine("North Dakota");
                                                                    RationalisePointerClick(point, args, ChildRegion.ChildRegionID.ND, Region.RegionID.USA);
                                                                }
                                                                else
                                                                {
                                                                    if (x > 719 && x < 904 && y > 207 && y < 308)
                                                                    {
                                                                        Debug.WriteLine("South Dakota");
                                                                        RationalisePointerClick(point, args, ChildRegion.ChildRegionID.SD, Region.RegionID.USA);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (x > 715 && x < 919 && y > 308 && y < 373 || x > 764 && x < 948 && y > 373 && y < 410)
                                                                        {
                                                                            Debug.WriteLine("Nebraska");
                                                                            RationalisePointerClick(point, args, ChildRegion.ChildRegionID.NE, Region.RegionID.USA);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (x > 760 && x < 953 && y > 410 && y < 511)
                                                                            {
                                                                                Debug.WriteLine("Kansas");
                                                                                RationalisePointerClick(point, args, ChildRegion.ChildRegionID.KS, Region.RegionID.USA);
                                                                            }
                                                                            else
                                                                            {
                                                                                if (x > 810 && x < 958 && y > 511 && y < 604)
                                                                                {
                                                                                    Debug.WriteLine("Oklahoma");
                                                                                    RationalisePointerClick(point, args, ChildRegion.ChildRegionID.OK, Region.RegionID.USA);
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (x > 731 && x < 810 && y > 522 && y < 595 || x > 718 && x < 971 && y > 595 && y < 678 || x > 615 && x < 987 && y > 678 && y < 764 || x > 680 && x < 969 && y > 764 && y < 857)
                                                                                    {
                                                                                        Debug.WriteLine("Texas");
                                                                                        RationalisePointerClick(point, args, ChildRegion.ChildRegionID.TX, Region.RegionID.USA);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (x > 14 && x < 475 && y > 672 && y < 905)
                                                                                        {
                                                                                            Debug.WriteLine("Alaska");
                                                                                            RationalisePointerClick(point, args, ChildRegion.ChildRegionID.AK, Region.RegionID.USA);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (x > 497 && x < 701 && y > 799 && y < 918)
                                                                                            {
                                                                                                Debug.WriteLine("Hawaii");
                                                                                                RationalisePointerClick(point, args, ChildRegion.ChildRegionID.HI, Region.RegionID.USA);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
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
        catch (Exception)
        {
            Debug.WriteLine("[!] Malformed pointer event args!");
        }
    }

    private void RationalisePointerClick(PointerPoint point, PointerPressedEventArgs args, ChildRegion.ChildRegionID ID, Region.RegionID ParentID)
    {
        SViewModel.SelectSceneryDialogOpen = false;
        if (point.Properties.IsLeftButtonPressed && args.ClickCount == 2)
        {
            SelectChildRegion(Regions.GetRegionByID(ID, ParentID));
        }
        else
        {
            if (point.Properties.IsRightButtonPressed)
            {
                SViewModel.SelectSceneryDialogRegion = Regions.GetRegionByID(ID, ParentID);
                SViewModel.SelectSceneryDialogOpen = true;
            }
        }
    }

    private void DisplayRegion(Region region)
    {
        SViewModel.DisplayedRegion = region;
        Regions.LoadSelectedRegions();
    }

    internal void ResetMap()
    {
        DisplayRegion(Regions.GetRegionByID(Region.RegionID.GLOBE));
    } 
    
    internal void SelectChildRegion(ChildRegion region)
    {
        region.Selected = !region.Selected;
        switch (region.Selected)
        {
            case true:
                if (!SelectedChildRegions.Contains(region))
                {
                    SelectedChildRegions.Add(region);
                }
                else
                {
                    foreach(ChildRegion childregion in SelectedChildRegions)
                    {
                        if(region.ID == childregion.ID && region.ParentID == childregion.ParentID)
                        {
                            childregion.Selected = true;
                        }
                    }
                }
                break;

            case false:
                foreach (ChildRegion childregion in SelectedChildRegions)
                {
                    if (region.ID == childregion.ID && region.ParentID == childregion.ParentID)
                    {
                        SelectedChildRegions.Remove(childregion);
                    }
                }
                break;

        }
        
    }

    internal void SelectSceneryItem(object? region)
    {
        foreach(SceneryItem item in ((ChildRegion)region).SceneryItems)
        {
            if(item.Selected && !SelectedSceneryItems.Contains(item))
            {
                SelectedSceneryItems.Add(item);
            } 
            else
            {
                if(!item.Selected && SelectedSceneryItems.Contains(item)) 
                {
                    SelectedSceneryItems.Remove(item);
                }
            }
        }
    }

}
