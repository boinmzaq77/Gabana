using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS.ITEMS
{
    public partial class StockMovementController : UIViewController
    {
        UIView viewdetail;
        UIImageView Imageitem;
        UILabel lblname, lblmini, lblstock, lblbalace;
        UICollectionView stockmoveCollection;
        Item item;
        public StockMovementController(Item addItem)
        {
            item = addItem; 
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            try
            {
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                viewdetail = new UIView();
                viewdetail.TranslatesAutoresizingMaskIntoConstraints = false;
                viewdetail.BackgroundColor = UIColor.White;
                //line4.BackgroundColor = UIColor.Red;
                viewdetail.UserInteractionEnabled = true;
                var tapGesture = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("Stockmove:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                viewdetail.AddGestureRecognizer(tapGesture);
                View.AddSubview(viewdetail);

                Imageitem = new UIImageView();
                Imageitem.Image = UIImage.FromBundle("Next");
                Imageitem.TranslatesAutoresizingMaskIntoConstraints = false;
                viewdetail.AddSubview(Imageitem);

                lblname = new UILabel
                {
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblname.Font = lblname.Font.WithSize(14);
                lblname.TranslatesAutoresizingMaskIntoConstraints = false;
                lblname.Text = "ลาเต้ กลาง";
                viewdetail.AddSubview(lblname);

                lblmini = new UILabel
                {
                    TextColor = UIColor.FromRGB(162, 162, 162),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblmini.Font = lblname.Font.WithSize(14);
                lblmini.TranslatesAutoresizingMaskIntoConstraints = false;
                lblmini.Text = "Minimum Stock : 20";
                viewdetail.AddSubview(lblmini);

                lblstock = new UILabel
                {
                    TextColor = UIColor.FromRGB(162, 162, 162),
                    TextAlignment = UITextAlignment.Right,
                    TranslatesAutoresizingMaskIntoConstraints = false

                };
                lblstock.Font = lblname.Font.WithSize(14);
                lblstock.TranslatesAutoresizingMaskIntoConstraints = false;
                lblstock.Text = "Stock Balance";
                viewdetail.AddSubview(lblstock);

                lblbalace = new UILabel
                {
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    TextAlignment = UITextAlignment.Right,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblbalace.Font = lblbalace.Font.WithSize(20);
                lblbalace.TranslatesAutoresizingMaskIntoConstraints = false;
                lblbalace.Text = "100";

                viewdetail.AddSubview(lblbalace);

                UICollectionViewFlowLayout itemflowLayout = new UICollectionViewFlowLayout();
                itemflowLayout.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
                itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemflowLayout.MinimumLineSpacing = 1f;
                itemflowLayout.MinimumInteritemSpacing = 1f;
                //itemflowLayout.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;


                stockmoveCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
                stockmoveCollection.BackgroundColor = UIColor.White;
                stockmoveCollection.ShowsVerticalScrollIndicator = false;
                stockmoveCollection.TranslatesAutoresizingMaskIntoConstraints = false;
                stockmoveCollection.RegisterClassForCell(cellType: typeof(StockCollectionViewCell), reuseIdentifier: "StockCollectionViewCell");

                StockCollectionDelegate stockCollectionDelegate = new StockCollectionDelegate();
                stockCollectionDelegate.OnItemSelected += (indexPath) =>
                {

                    // do somthing


                };
                var stock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, (int)item.SysItemID);
                Utils.SetColor(Imageitem, (long)item.Colors);
                lblname.Text = item.ItemName;
                lblmini.Text = "Minimum Stock : " + stock.MinimumStock.ToString("#,###");
                lblbalace.Text = stock.BalanceStock.ToString("#,###");

                
                GetDataStock();

                
                stockmoveCollection.Delegate = stockCollectionDelegate;
                View.AddSubview(stockmoveCollection);
                Setlaout();
            }
            catch (Exception ex )
            {
                Utils.ShowMessage(ex.Message);
            }
            //AddItemControllerScroll

          
           

            

        }

        private async void GetDataStock()
        {
            //DialogLoading dialogLoading = new DialogLoading();
            try
            {

                //dialogLoading.Show(SupportFragmentManager, nameof(DialogLoading));

                List<Gabana.Model.ItemMovement> result = await GabanaAPI.GetDataStockItemMovement(DataCashingAll.SysBranchId, (int)item.SysItemID, 0);
                if (result == null)
                {
                    return;
                }

                stockmoveCollection.DataSource = new StockDataSource(result, this);
                ((StockDataSource)stockmoveCollection.DataSource).ReloadData(result);
                stockmoveCollection.ReloadData();


                //dialogLoading.Dismiss();


            }
            catch (Exception ex)
            {
                //dialogLoading.Dismiss();

                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Setlaout()
        {
            viewdetail.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            viewdetail.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            viewdetail.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            viewdetail.HeightAnchor.ConstraintEqualTo(80).Active = true;

            Imageitem.CenterYAnchor.ConstraintEqualTo(viewdetail.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            Imageitem.WidthAnchor.ConstraintEqualTo(56).Active = true;
            Imageitem.LeftAnchor.ConstraintEqualTo(viewdetail.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            Imageitem.HeightAnchor.ConstraintEqualTo(42).Active = true;

            lblname.TopAnchor.ConstraintEqualTo(Imageitem.TopAnchor).Active = true;
            lblname.LeftAnchor.ConstraintEqualTo(Imageitem.RightAnchor, 15).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lblstock.TopAnchor.ConstraintEqualTo(Imageitem.TopAnchor).Active = true;
            lblstock.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblstock.RightAnchor.ConstraintEqualTo(viewdetail.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblstock.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblstock.LeftAnchor.ConstraintEqualTo(lblname.RightAnchor,5).Active = true; 

            lblmini.BottomAnchor.ConstraintEqualTo(Imageitem.BottomAnchor).Active = true;
            lblmini.LeftAnchor.ConstraintEqualTo(Imageitem.RightAnchor, 15).Active = true;
            lblmini.HeightAnchor.ConstraintEqualTo(16).Active = true;

            lblbalace.BottomAnchor.ConstraintEqualTo(Imageitem.BottomAnchor).Active = true;
            lblbalace.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblbalace.RightAnchor.ConstraintEqualTo(viewdetail.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblbalace.HeightAnchor.ConstraintEqualTo(24).Active = true;

            
            stockmoveCollection.TopAnchor.ConstraintEqualTo(viewdetail.BottomAnchor,2).Active = true;
            stockmoveCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            stockmoveCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            stockmoveCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
        }
    }
}