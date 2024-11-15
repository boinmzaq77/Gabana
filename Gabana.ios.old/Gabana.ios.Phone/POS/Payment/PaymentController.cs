using Foundation;
using Gabana.Model;
using System;
using System.Collections.Generic;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class PaymentController : UIViewController
    {
        public static List<MenuPayment> menuItem = new List<MenuPayment>();
        public PaymentController (IntPtr handle) : base (handle)
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
                lblsumAmount.Text = "ß "+POSController.sumItem.ToString("N2");
                UICollectionViewFlowLayout itemViewLayout = menuPaymentCollect.CollectionViewLayout as UICollectionViewFlowLayout;
                itemViewLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemViewLayout.ItemSize = new CoreGraphics.CGSize((View.Frame.Width - 60) / 3, ((View.Frame.Width) / 3));
                menuPaymentCollect.ShowsHorizontalScrollIndicator = false;
                menuPaymentCollect.ReloadData();

                menuItem.Add(new MenuPayment("cash",""));
                menuItem.Add(new MenuPayment("credits", ""));
                menuItem.Add(new MenuPayment("debits", ""));
                menuItem.Add(new MenuPayment("Store debit", ""));
                menuItem.Add(new MenuPayment("check", ""));
                menuItem.Add(new MenuPayment("other", ""));

                PaymentCollectionDelegate PaymentCollectionDelegate = new PaymentCollectionDelegate();
                PaymentCollectionDelegate.OnPaymentselected += (indexPath) =>
                {
                    var x = indexPath;
                    // do sth after select item
                };
                menuPaymentCollect.Delegate = PaymentCollectionDelegate;
                menuPaymentCollect.DataSource = new PaymentViewDataSource(menuItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}