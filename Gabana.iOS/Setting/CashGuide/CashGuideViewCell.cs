using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class CashGuideViewCell : UICollectionViewCell
    {
        UILabel lblname;
        UIImageView settingImg,IconImage;
        private UIView viewall;
        private UIView viewdelete;
        private UIImageView imageDelete;

        public CashGuideViewCell(IntPtr handle) : base(handle)
        {
            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblname.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
            lblname.Font = lblname.Font.WithSize(15);
            ContentView.AddSubview(lblname);

            ContentView.BackgroundColor = UIColor.White;
            viewdelete = new UIView();
            viewdelete.TranslatesAutoresizingMaskIntoConstraints = false;
            viewdelete.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            viewdelete.Alpha = 0;
            ContentView.AddSubview(viewdelete);

            #region imagedelete
            imageDelete = new UIImageView();
            imageDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            imageDelete.Image = UIImage.FromFile("DeleteBt2.png");
            imageDelete.ContentMode = UIViewContentMode.ScaleAspectFit;
            viewdelete.AddSubview(imageDelete);

            viewdelete.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("imageDelete:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            viewdelete.AddGestureRecognizer(tapGesture2);
            #endregion
            var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
            this.AddGestureRecognizer(swipeRight);
            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void swiped(UISwipeGestureRecognizer s)
        {
            OnItemSwipe?.Invoke(this);
        }
        [Export("imageDelete:")]
        public void DeleteItem(UIGestureRecognizer sender)
        {
            OnDeleteItem?.Invoke(this);
        }
        private void setupView()
        {
            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -5).Active = true;

            lblname.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblname.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

            //lblname.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            //lblname.HeightAnchor.ConstraintEqualTo(20).Active = true;
            //lblname.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            //lblname.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            //lblname.WidthAnchor.ConstraintEqualTo(150).Active = true;
            viewdelete.TopAnchor.ConstraintEqualTo(viewall.TopAnchor, -4).Active = true;
            viewdelete.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.WidthAnchor.ConstraintEqualTo(80).Active = true;
            viewdelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.BottomAnchor.ConstraintEqualTo(viewall.BottomAnchor, 4).Active = true;

            imageDelete.TopAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            imageDelete.BottomAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            imageDelete.LeftAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            imageDelete.RightAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
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
        public string Name
        {
            get { return lblname.Text; }
            set { lblname.Text = value;
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
            }
        }

        public delegate void CashSelectedDelegate(CashGuideViewCell indexPath);
        public event CashSelectedDelegate OnItemSwipe;

        public delegate void CashdeleteDelegate(CashGuideViewCell indexPath);
        public event CashdeleteDelegate OnDeleteItem;
    }
}