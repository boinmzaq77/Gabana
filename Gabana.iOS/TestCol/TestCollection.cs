using CoreFoundation;
using Foundation;
using Gabana.iOS;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace Gabana
{
    
    public class TestCollection : UIViewController
    {
        UICollectionView CartCollectionview;
        List<CartItem> cartItemlist;
        CartDataSource2 cartDataSource;
        private TranWithDetailsLocal tranWithDetails;
        public TestCollection()
        {
            this.tranWithDetails = POSController.tranWithDetails  ;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
       

        public override void ViewDidLoad()
        {
          

            base.ViewDidLoad();
            UICollectionViewFlowLayout itemflowLayout = new UICollectionViewFlowLayout();
            itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            
            //itemflowLayout.EstimatedItemSize
            itemflowLayout.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
            //itemflowLayout.ItemSize = new CoreGraphics.CGSize(View.Frame.Width - 10, 61);
            CartCollectionview = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
            CartCollectionview.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            //CartCollectionview.BackgroundColor = new UIColor(248 / 255f, 248 / 255f, 248 / 255f, 1);
            CartCollectionview.TranslatesAutoresizingMaskIntoConstraints = false;
            CartCollectionview.RegisterClassForCell(cellType: typeof(CartCollectionViewCell2), reuseIdentifier: "CartCell");
            CartCollectionview.RegisterClassForCell(cellType: typeof(CartCollectionViewCell3), reuseIdentifier: "CartCell3");
            var cartItem = new CartItem();
            cartItemlist = new List<CartItem>();
            //tranWithDetails.tranDetailItems[0].choose = true; 
            cartDataSource = new CartDataSource2(tranWithDetails.tranDetailItemWithToppings, View.Frame); 
            cartDataSource.OnCardCellbtnIndex0 += (indexPath) =>
            {
                var cartPage = new TestCollection();
                this.NavigationController.PushViewController(cartPage, false);
            };
            cartDataSource.OnCardCellbtnIndex1 += (indexPath) =>
            {

            };
            cartDataSource.OnCardCellbtnIndex2 += (indexPath) =>
            {

            };
            cartDataSource.OnCardCellbtnIndex3 += (indexPath) =>
            {

            };
            cartDataSource.OnCardCellbtnIndex4 += (indexPath) =>
            {

            };
            CartCollectionview.DataSource = cartDataSource;
            CartCollectionDelegate2 cartDelegate = new CartCollectionDelegate2();
            cartDelegate.OnItemSelected += (indexPath) => {


                tranWithDetails.tranDetailItemWithToppings.ConvertAll(x => x.tranDetailItem.choose = false);
                tranWithDetails.tranDetailItemWithToppings[indexPath.Row].tranDetailItem.choose = true;
                ((CartDataSource2)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItemWithToppings);
                CartCollectionview.ReloadData();
                CartCollectionview.ScrollToItem(indexPath, UICollectionViewScrollPosition.Top, true);
                

                //CartCollectionview.EndEditing(false);
                //if (tranWithDetails.tranDetailItems[indexPath.Row].choose)
                //{
                //    tranWithDetails.tranDetailItems.ConvertAll(x => x.choose = false);
                //    ((CartDataSource2)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItems);
                //    CartCollectionview.ReloadItems(new NSIndexPath[] { indexPath });

                //}
                //else
                //{
                //    var index = tranWithDetails.tranDetailItems.FindIndex(x => x.choose);
                //    if (index != -1)
                //    {
                //        tranWithDetails.tranDetailItems[index].choose = false;
                //        NSIndexPath nSIndex = NSIndexPath.FromRowSection(index, 0);
                //        ((CartDataSource2)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItems);
                //        CartCollectionview.ReloadItems(new NSIndexPath[] { nSIndex });
                //    }
                //    tranWithDetails.tranDetailItems.ConvertAll(x => x.choose = false);
                //    tranWithDetails.tranDetailItems[indexPath.Row].choose = true;
                //    ((CartDataSource2)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItems);
                //    //CartCollectionview.SizeToFit();
                //    //CartCollectionview.CellForItem(indexPath).t();
                //    //CartCollectionview.ReloadData();
                //    CartCollectionview.ReloadItems(new NSIndexPath[] { indexPath });

                //    //CartCollectionview.ReloadInputViews();
                //}

                //CartCollectionview.EndEditing(true);
            };


            CartCollectionview.Delegate = cartDelegate;
            View.AddSubview(CartCollectionview);
            // Perform any additional setup after loading the view
        }
    }
}