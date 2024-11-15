using CoreFoundation;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace Gabana.iOS.Test
{

    public class MainTest : UIViewController
    {
        UICollectionView menuPOSCollectionview, itemPOSCollectionview, itemPOSCollectionviewList;
        ItemPosDataSource itemPosData;
        public List<Item> Menu;
        UIButton btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9;
        public MainTest()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public async override void ViewDidLoad()
        {

            base.ViewDidLoad();




            //var emp = new LoginEmp() { MerchantID = "99900011", Username = "aaa", Password = "aaa" };
            //var token = await GetToken.GetTokenForEmp(emp);
            //GabanaAPI.Jwt2 = token;
            //jwt

            //LocalDBTransaction a = new LocalDBTransaction();
            //a.ConnectLocalBase(1);

            MerchantManage merchant = new MerchantManage();
            var res = await merchant.GetAllMerchant(); 
            if (res == null ||res.Count == 0 )
            {
               //DataforTest.testinsert(MainController.merchantlocal.MerchantID);
            }
            

            ItemManage item = new ItemManage();
            Menu = await item.GetAllItem();
            
            View.BackgroundColor = UIColor.White;
            UICollectionViewFlowLayout itemflowLayout = new UICollectionViewFlowLayout();
            itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayout.SectionInset = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            itemflowLayout.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width - 60) / 3, height: (View.Frame.Height - 20) / 5);
            itemPOSCollectionview = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
            itemPOSCollectionview.Hidden = false;
            itemPOSCollectionview.BackgroundColor = UIColor.White;
            itemPOSCollectionview.ShowsVerticalScrollIndicator = false;
            itemPOSCollectionview.TranslatesAutoresizingMaskIntoConstraints = false;
            
            itemPOSCollectionview.RegisterClassForCell(cellType: typeof(ItemPOSCollectionViewCell), reuseIdentifier: "itemPosCell");
            itemPosData = new ItemPosDataSource(Menu);
            itemPOSCollectionview.DataSource = itemPosData;
            ItemPOSCollectionDelegate itemposCollectionDelegate = new ItemPOSCollectionDelegate();
            itemposCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing

            };
            itemPOSCollectionview.Delegate = itemposCollectionDelegate;
            View.AddSubview(itemPOSCollectionview);

            // reloaddata 
            //((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Menu);
            // Perform any additional setup after loading the view

            btn1 = new UIButton();
            btn1.SetTitle("Retive", UIControlState.Normal);
            btn1.BackgroundColor = UIColor.Black;
            btn1.TranslatesAutoresizingMaskIntoConstraints = false;
            btn1.TouchUpInside += async (sender, e) => {

                Menu = await item.GetAllItem();
                ((ItemPosDataSource)itemPOSCollectionview.DataSource).ReloadData(Menu);
                itemPOSCollectionview.ReloadData();

            };
            View.AddSubview(btn1);

            btn2 = new UIButton();
            btn2.SetTitle("insert", UIControlState.Normal);
            btn2.BackgroundColor = UIColor.Black;
            btn2.TranslatesAutoresizingMaskIntoConstraints = false;
            btn2.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn2);

            btn3 = new UIButton();
            btn3.SetTitle("insert", UIControlState.Normal);
            btn3.BackgroundColor = UIColor.Black;
            btn3.TranslatesAutoresizingMaskIntoConstraints = false;
            btn3.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn3);

            btn4 = new UIButton();
            btn4.SetTitle("test1", UIControlState.Normal);
            btn4.BackgroundColor = UIColor.Black;
            btn4.TranslatesAutoresizingMaskIntoConstraints = false;
            btn4.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn4);

            btn5 = new UIButton();
            btn5.SetTitle("update", UIControlState.Normal);
            btn5.BackgroundColor = UIColor.Black;
            btn5.TranslatesAutoresizingMaskIntoConstraints = false;
            btn5.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn5);

            btn6 = new UIButton();
            btn6.SetTitle("update", UIControlState.Normal);
            btn6.BackgroundColor = UIColor.Black;
            btn6.TranslatesAutoresizingMaskIntoConstraints = false;
            btn6.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn6);

            btn7 = new UIButton();
            btn7.SetTitle("test2", UIControlState.Normal);
            btn7.BackgroundColor = UIColor.Black;
            btn7.TranslatesAutoresizingMaskIntoConstraints = false;
            btn7.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn7);

            btn8 = new UIButton();
            btn8.SetTitle("delete", UIControlState.Normal);
            btn8.BackgroundColor = UIColor.Black;
            btn8.TranslatesAutoresizingMaskIntoConstraints = false;
            btn8.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn8);

            btn9 = new UIButton();
            btn9.SetTitle("delete", UIControlState.Normal);
            btn9.BackgroundColor = UIColor.Black;
            btn9.TranslatesAutoresizingMaskIntoConstraints = false;
            btn9.TouchUpInside += (sender, e) => {
                // search function
            };
            View.AddSubview(btn9);

            SetupAutoLayout();
        }
        void SetupAutoLayout() 
        {
            itemPOSCollectionview.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            itemPOSCollectionview.BottomAnchor.ConstraintEqualTo(btn1.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            itemPOSCollectionview.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            itemPOSCollectionview.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btn1.TopAnchor.ConstraintEqualTo(itemPOSCollectionview.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn1.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn1.WidthAnchor.ConstraintEqualTo((View.Frame.Width-30)/3).Active = true;
            btn1.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            btn2.TopAnchor.ConstraintEqualTo(itemPOSCollectionview.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn2.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn2.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;
            btn2.LeftAnchor.ConstraintEqualTo(btn1.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;

            btn3.TopAnchor.ConstraintEqualTo(itemPOSCollectionview.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn3.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn3.LeftAnchor.ConstraintEqualTo(btn2.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            btn3.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            btn3.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;

            btn4.TopAnchor.ConstraintEqualTo(btn3.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn4.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn4.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            btn4.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;

            btn5.TopAnchor.ConstraintEqualTo(btn3.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn5.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn5.LeftAnchor.ConstraintEqualTo(btn4.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            btn5.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;

            btn6.TopAnchor.ConstraintEqualTo(btn3.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn6.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn6.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            btn6.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;

            btn7.TopAnchor.ConstraintEqualTo(btn6.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn7.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn7.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            btn7.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn7.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;

            btn8.TopAnchor.ConstraintEqualTo(btn6.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn8.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn8.LeftAnchor.ConstraintEqualTo(btn7.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            btn8.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn8.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;

            btn9.TopAnchor.ConstraintEqualTo(btn6.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn9.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btn9.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            btn9.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btn9.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 3).Active = true;

        }
    }
}