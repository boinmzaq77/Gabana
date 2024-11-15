using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using System.Drawing;

namespace Gabana.iOS
{
    public class OptionToppingCollectionViewCell : UICollectionViewCell
    {
        UILabel lbltxtName, lbltxtPrice;
        UIImageView RatioCheck,minus,plus;
        int count=1;
        UILabel lblCount;
      
        public OptionToppingCollectionViewCell(IntPtr handle) : base(handle)
        {
            InitAttribute();
            setupView();
        }
        private void InitAttribute()
        {
            RatioCheck = new UIImageView(); 
            RatioCheck.Image = UIImage.FromBundle("OptionBlank");
            RatioCheck.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(RatioCheck);

            ContentView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ContentView.AddGestureRecognizer(tapGesture0);

            lbltxtName = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtName.Font = lbltxtName.Font.WithSize(15);
            lbltxtName.Text = "Size Name";
            ContentView.AddSubview(lbltxtName);

            minus = new UIImageView();
            minus.Image = UIImage.FromBundle("DelQty1");
            minus.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(minus);

            minus.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("minus:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            minus.AddGestureRecognizer(tapGesture1);

            lblCount = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCount.Font = lblCount.Font.WithSize(15);
            lblCount.Text = count.ToString();
            ContentView.AddSubview(lblCount);

            plus = new UIImageView();
            plus.Image = UIImage.FromBundle("AddQty1");
            plus.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(plus);

            plus.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("plus:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            plus.AddGestureRecognizer(tapGesture2);

            lbltxtPrice = new UILabel
            {
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtPrice.Font = lbltxtPrice.Font.WithSize(15);
            lbltxtPrice.Text = "00.00";
            ContentView.AddSubview(lbltxtPrice);


        }
        [Export("Select:")]
        public void Select(UIGestureRecognizer sender)
        {
            OnToppingCellSelectBtn(this);
            //OptionCheck
        }
        [Export("plus:")]
        public void plusPress(UIGestureRecognizer sender)
        {

            OnToppingCellplus(this);
        }
        [Export("minus:")]
        public void minusPress(UIGestureRecognizer sender)
        {
            OnToppingCellminus(this);
        }
        #region Events
        public delegate void CardToppingCellDelegate(OptionToppingCollectionViewCell optionToppingCollectionViewCell);
        public event CardToppingCellDelegate OnToppingCellSelectBtn;

        //public delegate void CardToppingCellDelegate(OptionToppingCollectionViewCell optionToppingCollectionViewCell);
        public event CardToppingCellDelegate OnToppingCellplus;

        //public delegate void CardToppingCellDelegate(OptionToppingCollectionViewCell optionToppingCollectionViewCell);
        public event CardToppingCellDelegate OnToppingCellminus;
        #endregion
        private void setupView()
        {
            RatioCheck.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            RatioCheck.HeightAnchor.ConstraintEqualTo(20).Active = true;
            RatioCheck.WidthAnchor.ConstraintEqualTo(20).Active = true;
            RatioCheck.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;

            lbltxtName.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtName.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lbltxtName.LeftAnchor.ConstraintEqualTo(RatioCheck.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;

            minus.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            minus.HeightAnchor.ConstraintEqualTo(20).Active = true;
            minus.WidthAnchor.ConstraintEqualTo(20).Active = true;
            minus.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 200).Active = true;

            lblCount.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblCount.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblCount.WidthAnchor.ConstraintEqualTo(40).Active = true;
            //lblCount.RightAnchor.ConstraintEqualTo(plus.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            lblCount.LeftAnchor.ConstraintEqualTo(minus.SafeAreaLayoutGuide.RightAnchor,0).Active = true;
            //plus.BackgroundColor = UIColor.Red; 
            plus.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            plus.HeightAnchor.ConstraintEqualTo(20).Active = true;
            plus.WidthAnchor.ConstraintEqualTo(20).Active = true;
            plus.LeftAnchor.ConstraintEqualTo(lblCount.SafeAreaLayoutGuide.RightAnchor,0).Active = true;

            lbltxtPrice.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtPrice.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lbltxtPrice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
        }
        public string SizeName
        {
            get { return lbltxtName.Text; }
            set { lbltxtName.Text = value; }
        }
        public string setCount
        {
            get { return lblCount.Text; }
            set { lblCount.Text = value; }
        }
        public bool SelectSize
        {
            set { 
            if(value) // select = true
                {
                    RatioCheck.Image = UIImage.FromBundle("OptionCheck");
                }
            else
                {
                    RatioCheck.Image = UIImage.FromBundle("OptionBlank");
                }
            }
        }
        public string Price
        {
            get { return lbltxtPrice.Text; }
            set { lbltxtPrice.Text = "+"+value; }
        }
    }
}