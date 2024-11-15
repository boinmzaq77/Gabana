using Foundation;
using System;
using Gabana.Model;
using UIKit;
using System.Collections.Generic;

namespace Gabana.ios.Phone
{
    public partial class POSController : UIViewController
    {
        int flag = 0;
        public static int countItems = 0;
        public static double sumItem = 0;
        public static List<POSMenuitem> menuItem = new List<POSMenuitem>();
        public static List<itemPOS> itemPOSList = new List<itemPOS>();
        public static List<itemPOS> cart = new List<itemPOS>();
        public POSController (IntPtr handle) : base (handle)
        {
        }
        public async override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationItem.Title = "";
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.NavigationBar.TintColor = new UIColor(red: 0f, green: 149f / 255f, blue: 218f / 255f, alpha: 1f);
            try
            {
                // menu header
                UICollectionViewFlowLayout itemViewLayout = POSMenuCollection.CollectionViewLayout as UICollectionViewFlowLayout;
                itemViewLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
                POSMenuCollection.ShowsHorizontalScrollIndicator = false;
                POSMenuCollection.ReloadData();
                btnItemSum.Enabled = false;
                if(menuItem.Count == 0)
                {
                    menuItem.Add(new POSMenuitem("ALL"));
                    menuItem.Add(new POSMenuitem("Discount"));
                    menuItem.Add(new POSMenuitem("Favourite"));
                    menuItem.Add(new POSMenuitem("Ice coffee"));
                    menuItem.Add(new POSMenuitem("Hot coffee"));
                    menuItem.Add(new POSMenuitem("Drink"));
                    menuItem.Add(new POSMenuitem("Other"));
                }
             
 //--------------------------------------------------------------------------------------------------------

                UICollectionViewFlowLayout itemViewPOS = itemPOSCollection.CollectionViewLayout as UICollectionViewFlowLayout;
                itemViewPOS.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemViewPOS.ItemSize= new CoreGraphics.CGSize((View.Frame.Width - 60) / 3, ((View.Frame.Width) / 3));
                itemPOSCollection.ShowsHorizontalScrollIndicator = false;
                itemPOSCollection.ReloadData();
                if(itemPOSList.Count == 0)
                {
                    itemPOSList.Add(new itemPOS("item1", 100, "Discount", ""));
                    itemPOSList.Add(new itemPOS("item2", 100, "Discount", ""));
                    itemPOSList.Add(new itemPOS("item3", 100, "Favourite", ""));
                    itemPOSList.Add(new itemPOS("item4", 100, "Favourite", ""));
                    itemPOSList.Add(new itemPOS("item5", 100, "Ice coffee", ""));
                    itemPOSList.Add(new itemPOS("item6", 100, "Ice coffee", ""));
                    itemPOSList.Add(new itemPOS("item7", 100, "Hot coffee", ""));
                    itemPOSList.Add(new itemPOS("item8", 100, "Hot coffee", ""));
                    itemPOSList.Add(new itemPOS("item9", 100, "Drink", ""));
                    itemPOSList.Add(new itemPOS("item10", 100, "Drink", ""));
                    itemPOSList.Add(new itemPOS("item11", 100, "Other", ""));

                    itemPOSList.Add(new itemPOS("+", 0, "X", "AddImg.png"));
                }
                
                if(countItems != 0)
                {
                    btnItemSum.SetTitle(countItems.ToString() + "item , " + sumItem.ToString() + "ß", UIControlState.Normal);
                    btnItemSum.Enabled = true;
                }
                
                itemPOSCollectionDelegate ItemPOSCollectionDelegate = new itemPOSCollectionDelegate();
                ItemPOSCollectionDelegate.OnItemPOSSelected += (indexPathOfMyCoupon) =>
                {
                    var x = indexPathOfMyCoupon;
                    if(x.Item != itemPOSList.Count-1)
                    {
                        countItems += 1;
                        sumItem += Convert.ToDouble(itemPOSList[(int)indexPathOfMyCoupon.Row].itemCost);
                        btnItemSum.SetTitle(countItems.ToString() + "item , " + sumItem.ToString() + "ß", UIControlState.Normal);
                        //µÐ¡ÃéÒ
                        cart.Add(itemPOSList[(int)indexPathOfMyCoupon.Row]);
                        btnItemSum.Enabled = true;

                    }
                    else
                    {
                        // open add new item pos 
                        AddNewItemPOSController NewItem = this.Storyboard?.InstantiateViewController("AddNewItemPOSController") as AddNewItemPOSController;
                        this.NavigationController.PushViewController(NewItem, true);
                    }
                   // UIViewAnimatingPosition.
                };
                itemPOSCollection.Delegate = ItemPOSCollectionDelegate;
                itemPOSCollection.DataSource = new ItemPOSViewDataSource(itemPOSList, null,0);
                //-------------------------------------------------------------------------------------
                itemPOSVertical.Delegate = ItemPOSCollectionDelegate;
                itemPOSVertical.DataSource = new ItemPOSViewDataSource(itemPOSList,null,0);
//--------------------------------------------------------------------------------------
                POSMainitemCollectionDelegate POSmainItemCollectionDelegate = new POSMainitemCollectionDelegate();
                POSmainItemCollectionDelegate.OnPOSmainItemSelected += (indexPathOfMyCoupon) =>
                {
                    var x = indexPathOfMyCoupon;
                    if(x.Item == 0)
                    {
                        itemPOSCollection.DataSource = new ItemPOSViewDataSource(itemPOSList, null,0);
                    }
                    else
                    {
                        itemPOSCollection.DataSource = new ItemPOSViewDataSource(itemPOSList, menuItem[(int)x.Row].POSMenuName,0);
                    }
                    itemPOSCollection.ReloadData();
                };
                POSMenuCollection.Delegate = POSmainItemCollectionDelegate;
                POSMenuCollection.DataSource = new POSMainItemViewDataSource(menuItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

    

        partial void BtnList_TouchUpInside(UIButton sender)
        {
            //btn list
            if (flag==0)
            {
                //grid to vertical
                itemPOSCollection.Hidden = true;
                itemPOSVertical.Hidden = false;
                btnList.SetImage(UIImage.FromFile("Email.png"), UIControlState.Normal);
                flag += 1;
            }
            else
            {
                //vertical to grid
                itemPOSVertical.Hidden = true;
                itemPOSCollection.Hidden = false;
                btnList.SetImage(UIImage.FromFile("sort2blue.png"), UIControlState.Normal);
                flag -= 1;
            }
         //   itemPOSCollection.ReloadData();
         //   itemPOSVertical.ReloadData();

        }

        partial void BtnBasket_TouchUpInside(UIButton sender)
        {
            // basket
            
        }

        partial void BtnSearch_TouchUpInside(UIButton sender)
        {
        }

        partial void BtnQr_TouchUpInside(UIButton sender)
        {
            ScanItemsQrController Barcode = this.Storyboard?.InstantiateViewController("ScanItemsQrController") as ScanItemsQrController;
            this.NavigationController.PushViewController(Barcode, true);
        }

        partial void BtnUser_TouchUpInside(UIButton sender)
        {
            DummyItemController Dummy = this.Storyboard?.InstantiateViewController("DummyItemController") as DummyItemController;
            this.NavigationController.PushViewController(Dummy, true);
        }

        partial void BtnItemSum_TouchUpInside(UIButton sender)
        {
            //  cart
            CartResultController sum = this.Storyboard?.InstantiateViewController("CartResultController") as CartResultController;
            this.NavigationController.PushViewController(sum, true);
        }
    }
}