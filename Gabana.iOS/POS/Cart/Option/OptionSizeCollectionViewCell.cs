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
    public class OptionSizeCollectionViewCell : UICollectionViewCell
    {
        UILabel  lbltxtSizeName, lbltxtPrice;
        UIImageView RatioCheck;
      
        public OptionSizeCollectionViewCell(IntPtr handle) : base(handle)
        {
            InitAttribute();
            setupView();
        }
        private void InitAttribute()
        {
            RatioCheck = new UIImageView(); 
            RatioCheck.Image = UIImage.FromBundle("RadioBlank");
            RatioCheck.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(RatioCheck);

            RatioCheck.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("Select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            RatioCheck.AddGestureRecognizer(tapGesture0);

            lbltxtSizeName = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtSizeName.Font = lbltxtSizeName.Font.WithSize(15);
            lbltxtSizeName.Text = "Size Name";
            ContentView.AddSubview(lbltxtSizeName);

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

            OnSizeCellSelectBtn(this);
        }
        #region Events
        public delegate void CardCellDelegate(OptionSizeCollectionViewCell optionSizeCollectionViewCell);
        public event CardCellDelegate OnSizeCellSelectBtn;
        #endregion
        private void setupView()
        {
            RatioCheck.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            RatioCheck.HeightAnchor.ConstraintEqualTo(20).Active = true;
            RatioCheck.WidthAnchor.ConstraintEqualTo(20).Active = true;
            RatioCheck.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;


            lbltxtSizeName.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtSizeName.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lbltxtSizeName.LeftAnchor.ConstraintEqualTo(RatioCheck.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;

            lbltxtPrice.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtPrice.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lbltxtPrice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
        }
        public string SizeName
        {
            get { return lbltxtSizeName.Text; }
            set { lbltxtSizeName.Text = value; }
        }
        public bool SelectSize
        {
            set { 
            if(value) // select = true
                {
                    RatioCheck.Image = UIImage.FromBundle("RadioCheck");
                    lbltxtSizeName.TextColor = UIColor.FromRGB(0,149,218);
                    lbltxtPrice.TextColor = UIColor.FromRGB(0, 149, 218);
                }
            else
                {
                    RatioCheck.Image = UIImage.FromBundle("RadioBlank");
                    lbltxtSizeName.TextColor = UIColor.FromRGB(64, 64, 64);
                    lbltxtPrice.TextColor = UIColor.FromRGB(64, 64, 64);
                }
            }
        }
        public string Price
        {
            get { return lbltxtPrice.Text; }
            set { lbltxtPrice.Text = value; }
        }
    }
}