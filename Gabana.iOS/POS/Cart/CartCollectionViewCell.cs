using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class CartCollectionViewCell : UICollectionViewCell
    {
        UILabel lblText, lblPrice, lblPriceperamount, lblamount , lbldelete;
        UICollectionView ToppingCollection;
        public CartCollectionViewCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.White;
            lblText = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblText.TextColor = UIColor.FromRGB(64,64,64);
            lblText.TextAlignment = UITextAlignment.Left;
            lblText.Font = lblText.Font.WithSize(16);
            //lblText.BackgroundColor = UIColor.Green;
            ContentView.AddSubview(lblText);
            
            lblPrice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPrice.TextColor = UIColor.FromRGB(0, 149, 218);
            lblPrice.TextAlignment = UITextAlignment.Right;
            //lblPrice.BackgroundColor = UIColor.Black;
            lblPrice.Font = lblText.Font.WithSize(16);
            ContentView.AddSubview(lblPrice);

            lblamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblamount.TextColor = UIColor.Black;
            lblamount.TextAlignment = UITextAlignment.Right;
            lblamount.Font = lblText.Font.WithSize(16);
            //lblamount.BackgroundColor = UIColor.Gray;
            ContentView.AddSubview(lblamount);

            lblPriceperamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPriceperamount.TextColor = UIColor.FromRGB(138, 221, 245);
            lblPriceperamount.TextAlignment = UITextAlignment.Left;
            lblPriceperamount.Font = lblText.Font.WithSize(16);
            //lblPriceperamount.BackgroundColor = UIColor.Brown;
            ContentView.AddSubview(lblPriceperamount);

            lbldelete = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbldelete.TextColor = UIColor.Black;
            lbldelete.TextAlignment = UITextAlignment.Right;
            lbldelete.Font = lblText.Font.WithSize(16);
            //lblamount.BackgroundColor = UIColor.Gray;
            ContentView.AddSubview(lbldelete);
            //ContentView.BackgroundColor = UIColor.Red;
            setupListView();
            var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            this.AddGestureRecognizer(swipeRight);

            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (ContentView.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            ToppingCollection = new UICollectionView(frame: ContentView.Frame, layout: itemflowLayoutList);
            ToppingCollection.BackgroundColor = UIColor.White;
            ToppingCollection.ShowsVerticalScrollIndicator = false;
            ToppingCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            ToppingCollection.RegisterClassForCell(cellType: typeof(ToppingCartCollectionViewCell), reuseIdentifier: "ToppingCartCollectionViewCell");

            //List<string> x = new List<string>() {"a","b","c" };
            //ToppingCartDataSource BranchDataList = new ToppingCartDataSource(x); // ส่ง list ไป
            //ToppingCollection.DataSource = BranchDataList;
            CustomerCollectionDelegate CustomerCollectionDelegate = new CustomerCollectionDelegate();

            ContentView.AddSubview(ToppingCollection);


        }
        private void swiped(UISwipeGestureRecognizer s)
        {

            if (s.Direction == UISwipeGestureRecognizerDirection.Left)
            {
                lbldelete.Text = "Left";
            }
            else if (s.Direction == UISwipeGestureRecognizerDirection.Right)
            {
                lbldelete.Text = "Right";
            }
            else
            {
                lbldelete.Text = s.Direction.ToString();
            }
        }
        private void setupListView()
        {

            lbldelete.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            lbldelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            lbldelete.WidthAnchor.ConstraintEqualTo(40).Active = true;
            lbldelete.BottomAnchor .ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            lblamount.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblamount.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            lblamount.WidthAnchor.ConstraintEqualTo(40).Active = true;
            lblamount.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblText.TopAnchor.ConstraintEqualTo(lblamount.TopAnchor).Active = true;
            lblText.LeftAnchor.ConstraintEqualTo(lblamount.RightAnchor, 5).Active = true;
            //lblText.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblText.RightAnchor.ConstraintEqualTo(lblPrice.LeftAnchor, -5).Active = true;
            lblText.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblPrice.TopAnchor.ConstraintEqualTo(lblamount.TopAnchor).Active = true;
            lblPrice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblPrice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;
            lblPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblPriceperamount.TopAnchor.ConstraintEqualTo(lblamount.BottomAnchor, 5).Active = true;
            lblPriceperamount.LeftAnchor.ConstraintEqualTo(lblText.LeftAnchor).Active = true; 
            lblPriceperamount.WidthAnchor.ConstraintEqualTo(lblText.WidthAnchor).Active = true;
            lblPriceperamount.HeightAnchor.ConstraintEqualTo(18).Active = true;
            

            ToppingCollection.TopAnchor.ConstraintEqualTo(lblPriceperamount.BottomAnchor, 5).Active = true;
            ToppingCollection.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor).Active = true;
            ToppingCollection.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
            ToppingCollection.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

        }

        public nfloat size
        {
            set { lblText.WidthAnchor.ConstraintEqualTo(value - 180).Active = true; }
        }
        public string Name
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }
        public string price
        {
            get { return lblPrice.Text; }
            set {
                if (value == "100")
                {
                    lblPriceperamount.HeightAnchor.ConstraintEqualTo(0).Active = true;
                }
                lblPrice.Text = value; }
        }
        public string priceperamount
        {
            get { return lblPriceperamount.Text; }
            set { lblPriceperamount.Text = value; }
        }
        public string amount
        {
            get { return lblamount.Text; }
            set { lblamount.Text = value; }
        }
        //public string size
        //{
        //    get { return lblText.Text; }
        //    set { lblText.WidthAnchor.ConstraintEqualTo(nfloat.Parse(value).Active = true; }
        //}


    }
}