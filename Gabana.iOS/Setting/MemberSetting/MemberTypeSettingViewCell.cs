using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class MemberTypeSettingViewCell : UICollectionViewCell
    {
        UILabel lblBranch;
        //UIView line;
        UIView viewdelete, viewall;
        UIImageView imageDelete;

        public MemberTypeSettingViewCell(IntPtr handle) : base(handle)
        {
            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);

            lblBranch = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblBranch.TextColor = UIColor.FromRGB(64,64,64);
            lblBranch.TextAlignment = UITextAlignment.Left;
            lblBranch.Font = lblBranch.Font.WithSize(15);
            viewall.AddSubview(lblBranch);

            //line = new UIView();
            //line.TranslatesAutoresizingMaskIntoConstraints = false;
            //line.BackgroundColor = UIColor.FromRGB(226,226,226);
            //viewall.AddSubview(line);


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

            lblBranch.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor,0).Active = true;
            lblBranch.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-20).Active = true;

            viewdelete.TopAnchor.ConstraintEqualTo(viewall.TopAnchor, -4).Active = true;
            viewdelete.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.WidthAnchor.ConstraintEqualTo(60).Active = true;
            viewdelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.BottomAnchor.ConstraintEqualTo(viewall.BottomAnchor, 4).Active = true;

            imageDelete.TopAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            imageDelete.BottomAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            imageDelete.LeftAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            imageDelete.RightAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            //line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            //line.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            //line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            //line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            //ContentView.BackgroundColor = UIColor.Red;
            //viewdelete.BackgroundColor = UIColor.Black;
            //imageDelete.BackgroundColor = UIColor.Blue;
        }
        public string Name
        {
            get { return lblBranch.Text; }
            set { lblBranch.Text = value;
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
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
        private void swiped(UISwipeGestureRecognizer s)
        {
            OnItemSwipe?.Invoke(this);
        }
        public delegate void branchSelectedDelegate(MemberTypeSettingViewCell indexPath);
        public event branchSelectedDelegate OnItemSwipe;
        public delegate void branchdeleteDelegate(MemberTypeSettingViewCell indexPath);
        public event branchdeleteDelegate OnDeleteItem;
    }
}