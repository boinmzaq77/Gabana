using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using TinyInsightsLib;

namespace Gabana.iOS
{
    public class GiftVoucherPayViewCell : UICollectionViewCell
    {
        UILabel lblCost,lblGiftName,lblGiftCode;
        UIImageView IconImage;
        UIImageView imageDelete;
        UIView viewdelete, viewall;
        UIImageView settingImg;

        public GiftVoucherPayViewCell(IntPtr handle) : base(handle)
        {
            try
            {
                viewall = new UIView();
                viewall.TranslatesAutoresizingMaskIntoConstraints = false;
                viewall.BackgroundColor = UIColor.White;
                viewall.Alpha = 1;
                ContentView.AddSubview(viewall);


                settingImg = new UIImageView();
                settingImg.Image = UIImage.FromBundle("Check");
                settingImg.TranslatesAutoresizingMaskIntoConstraints = false;
                viewall.AddSubview(settingImg);

                IconImage = new UIImageView();
                IconImage.TranslatesAutoresizingMaskIntoConstraints = false;
                IconImage.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                viewall.AddSubview(IconImage);

                lblCost = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextAlignment = UITextAlignment.Center };
                lblCost.TextColor = UIColor.FromRGB(0, 149, 218);
                lblCost.Font = lblCost.Font.WithSize(12);
                viewall.AddSubview(lblCost);

                lblGiftName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextAlignment = UITextAlignment.Left };
                lblGiftName.TextColor = UIColor.FromRGB(64, 64, 64);
                lblGiftName.Font = lblGiftName.Font.WithSize(15);
                viewall.AddSubview(lblGiftName);


                lblGiftCode = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextAlignment = UITextAlignment.Left };
                lblGiftCode.TextColor = UIColor.FromRGB(172, 172, 172);
                lblGiftCode.Font = lblGiftCode.Font.WithSize(15);
                viewall.AddSubview(lblGiftCode);

                viewdelete = new UIView();
                viewdelete.TranslatesAutoresizingMaskIntoConstraints = false;
                viewdelete.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                viewdelete.Alpha = 1;
                ContentView.AddSubview(viewdelete);

                #region imagedelete
                imageDelete = new UIImageView();
                imageDelete.TranslatesAutoresizingMaskIntoConstraints = false;
                imageDelete.Image = UIImage.FromFile("DeleteBt2.png");
                viewdelete.AddSubview(imageDelete);

                viewdelete.UserInteractionEnabled = true;
                var tapGesture2 = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("imageDelete:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                viewdelete.AddGestureRecognizer(tapGesture2);
                #endregion
                setupView();

                //var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
                //swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
                //this.AddGestureRecognizer(swipeRight);


                //var swipeLEFT = new UISwipeGestureRecognizer((s) => { swipedleft(s); });
                //swipeLEFT.Direction = UISwipeGestureRecognizerDirection.Right;
                //this.AddGestureRecognizer(swipeLEFT);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //                throw;
            }
         
        }

        [Export("imageDelete:")]
        public void DeleteItem(UIGestureRecognizer sender)
        {
            OnDeleteItem?.Invoke(this);
        }
        private void swiped(UISwipeGestureRecognizer s)
        {
            OnItemSwipe?.Invoke(this);
        }
        private void swipedleft(UISwipeGestureRecognizer s)
        {
            OnItemClear?.Invoke(this);
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

        }
        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();

        }
        private void setupView()
        {
            IconImage.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            IconImage.HeightAnchor.ConstraintEqualTo(50).Active = true;
            IconImage.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            IconImage.WidthAnchor.ConstraintEqualTo(67).Active = true;

            lblCost.CenterYAnchor.ConstraintEqualTo(IconImage.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblCost.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblCost.CenterXAnchor.ConstraintEqualTo(IconImage.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            lblCost.WidthAnchor.ConstraintEqualTo(60).Active = true;

            lblGiftName.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblGiftName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblGiftName.LeftAnchor.ConstraintEqualTo(IconImage.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblGiftName.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lblGiftCode.TopAnchor.ConstraintEqualTo(lblGiftName.SafeAreaLayoutGuide.BottomAnchor ,2).Active = true;
            lblGiftCode.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblGiftCode.LeftAnchor.ConstraintEqualTo(IconImage.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblGiftCode.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;

            viewdelete.TopAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewdelete.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.WidthAnchor.ConstraintEqualTo(80).Active = true;
            viewdelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.BottomAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            imageDelete.TopAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            imageDelete.BottomAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.BottomAnchor, -5).Active = true;
            imageDelete.LeftAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            imageDelete.RightAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            settingImg.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            settingImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            settingImg.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            settingImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            //settingImg.BackgroundColor = UIColor.Red;

        }
        public bool show
        {
            set
            {
                if (!value)
                {
                    settingImg.Hidden = true;
                }
                else
                {
                    settingImg.Hidden = false;
                }
            }
        }
        public string Name
        {
            get { return lblGiftName.Text; }
            set
            {
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
                lblGiftName.Text = value;
            }
        }
        public bool showbtndelete
        {
            get { return viewdelete.Hidden; }
            set
            {
                if (value)
                {
                    viewdelete.Alpha = 1;
                }
                else
                {
                    viewdelete.Alpha = 0;
                }
            }
        }
        public string Code
        {
            get { return lblGiftCode.Text; }
            set { lblGiftCode.Text = value; }
        }
        public string Cost
        {
            get { return lblCost.Text; }
            set { lblCost.Text = value; }
        }

        public delegate void EmpSelectedDelegate(GiftVoucherPayViewCell indexPath);
        public event EmpSelectedDelegate OnItemSwipe;

        public delegate void EmpClearDelegate(GiftVoucherPayViewCell indexPath);
        public event EmpClearDelegate OnItemClear;

        public delegate void EmpdeleteDelegate(GiftVoucherPayViewCell indexPath);
        public event EmpdeleteDelegate OnDeleteItem;
    }
}