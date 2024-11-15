using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class ToppingReceiptCollectionViewCell : UICollectionViewCell
    {
        UILabel lblText, lblPrice, lblPriceperamount, lblamount , lbldelete;
        public ToppingReceiptCollectionViewCell(IntPtr handle) : base(handle)
        {
            //ContentView.BackgroundColor = UIColor.Gray;
            lblText = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblText.TextColor = UIColor.FromRGB(200, 200, 200);
            lblText.TextAlignment = UITextAlignment.Left;
            lblText.Font = lblText.Font.WithSize(16);
            //lblText.BackgroundColor = UIColor.Green;
            ContentView.AddSubview(lblText);

            lblPrice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPrice.TextColor = UIColor.FromRGB(200,200,200);
            lblPrice.TextAlignment = UITextAlignment.Left;
            lblPrice.Font = lblText.Font.WithSize(16);
            lblPrice.TextAlignment = UITextAlignment.Right;
            //lblText.BackgroundColor = UIColor.Green;
            ContentView.AddSubview(lblPrice);


            //ContentView.BackgroundColor = UIColor.Blue;
            setupListView();
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }

        private void setupListView()
        {

            lblText.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor).Active = true;
            lblText.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor,10).Active = true;
            //lblText.WidthAnchor.ConstraintEqualTo(200).Active = true;
            //lblText.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblText.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, -10).Active = true;
            lblText.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
            //lblText.BackgroundColor = UIColor.Green;

            //lblPrice.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor).Active = true;
            //lblPrice.LeftAnchor.ConstraintEqualTo(lblText.RightAnchor, 5).Active = true;
            //lblPrice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            //lblPrice.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, -10).Active = true;
            //lblPrice.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
            //lblPrice.BackgroundColor = UIColor.Blue;
        }
        public string Name
        {
            get { return lblText.Text; }
            set { lblText.Text = value;
                
            }
        }
        public string price
        {
            get { return lblPrice.Text; }
            set {
                if (value == "100")
                {
                    lblPriceperamount.HeightAnchor.ConstraintEqualTo(0).Active = true;
                }
                lblPrice.Text = "+"+value; }
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
        public nfloat size
        {
            get { return 200; }
            set { Utils.SetConstant(lblText.Constraints , NSLayoutAttribute.Width , (int)value-30); }
        }
        


    }
}