using Foundation;
using System;
using UIKit;
using Gabana.Model;
using System.Collections.Generic;
using LinqToDB;
using Gabana.Controller;
using System.IO;
using Gabana.ORM.Local;

namespace Gabana.ios.Phone
{
    public partial class MainPageController : UIViewController
    {
        public static List<Menuitem> menuItem = new List<Menuitem>();
        public int flag = 0;
        public static string pathdb;
        LocalDBTransaction conn;
        public static Merchant a;
        public MainPageController (IntPtr handle) : base (handle)
        {
        }
        public async override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(true, false);
        }
        public async override void ViewDidLoad()
        {
           // SQLiteConnection connection;
            base.ViewDidLoad();
            this.NavigationController.SetNavigationBarHidden(true, false);
            try
            {
                // API connect 
                conn = new LocalDBTransaction();
                conn.ConnectLocalBase();
                a = new Merchant();
                a = await conn.GetData(1); // ต้องเปลี่ยน merID ตาม jwt ที่ login ได้มา
                lblMemname.Text = a.Name;
                lblCompName.Text = a.Name;
                if (a.Logo != null || a.Logo !="")
                {
                    imageProfile.Image = UIImage.FromBundle(a.Logo);
                }
                else
                {
                    imageProfile.Image = UIImage.FromBundle("Username.png");
                }

                lblBranch.Text = "merID :"+a.MerchantID.ToString();
                viewMemberCard.Layer.ShadowRadius = 3f;
                viewMemberCard.Layer.ShadowColor = new UIColor(red: 0f, green: 0f, blue: 0f, alpha: 0.16f).CGColor;
                viewMemberCard.Layer.ShadowOffset = new CoreGraphics.CGSize(2, 2);
                viewMemberCard.Layer.ShadowOpacity = 0.80f;
                Profile profile = new Profile();
                imageProfile.Layer.BorderWidth = 3f; //set border thickness
                imageProfile.Layer.CornerRadius = 50; //make corner's rounded
                imageProfile.Layer.BorderColor = new UIColor(red: 0f, green: 149f/255f, blue: 218f/255f, alpha: 1f).CGColor;
                UICollectionViewFlowLayout itemViewLayout = menuCollection.CollectionViewLayout as UICollectionViewFlowLayout;
                itemViewLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                
                itemViewLayout.ItemSize = new CoreGraphics.CGSize((View.Frame.Width-58) / 3, ((View.Frame.Width-58) / 3));
                itemViewLayout.MinimumInteritemSpacing = 3;
                menuCollection.ReloadData();

                //get api data
                if(menuItem.Count == 0)
                {
                    menuItem.Add(new Menuitem("POS.png", "POS"));
                    menuItem.Add(new Menuitem("LogOut.png", "ITEM"));
                    menuItem.Add(new Menuitem(null, "MENU3"));
                    menuItem.Add(new Menuitem(null, "MENU4"));
                    menuItem.Add(new Menuitem(null, "MENU5"));
                    menuItem.Add(new Menuitem(null, "MENU6"));
                    menuItem.Add(new Menuitem(null, "MENU7"));
                    menuItem.Add(new Menuitem(null, "MENU8"));
                    menuItem.Add(new Menuitem(null, "MENU9"));
                    menuItem.Add(new Menuitem(null, "MENU10"));
                    menuItem.Add(new Menuitem(null, "MENU11"));
                    menuItem.Add(new Menuitem(null, "MENU12"));
                }
             

                menuCollection.DataSource = new MainItemViewDataSource(menuItem);

                MainitemCollectionDelegate mainItemCollectionDelegate = new MainitemCollectionDelegate();
                mainItemCollectionDelegate.OnmainItemSelected += (indexPathOfMyCoupon) =>
                {
                    var x = indexPathOfMyCoupon;
                    // do sth after select item
                    if (x.Item == 0)
                    {
                            var storyboard = UIStoryboard.FromName("POS", null);
                        POSController controller = (POSController)storyboard.InstantiateInitialViewController();
                        NavigationController.PushViewController(controller, true);
                    }
                };
                menuCollection.Delegate = mainItemCollectionDelegate;
               
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        partial void UIButton371357_TouchUpInside(UIButton sender)
        {
            editProfileController edit = this.Storyboard?.InstantiateViewController("editProfileController") as editProfileController;
            this.NavigationController.PushViewController(edit, true);
        }
    }
}