using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class itemCatagoryCollectionViewCellList : UICollectionViewCell
    {
        UIView line;
        UILabel lblCatagory,lblCatagoryItemSum;
        UIView  viewdelete, viewall;
        UIImageView imageDelete;
     //   UIButton btndeleteitem;
        public itemCatagoryCollectionViewCellList(IntPtr handle) : base(handle)
        {
            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);


            lblCatagory = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCatagory.TextColor = UIColor.FromRGB(64, 64, 64);
            lblCatagory.Font = lblCatagory.Font.WithSize(14);
            viewall.AddSubview(lblCatagory);


            lblCatagoryItemSum = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCatagoryItemSum.TextColor = UIColor.FromRGB(162, 162, 162);
            lblCatagoryItemSum.Font = lblCatagoryItemSum.Font.WithSize(14);
            viewall.AddSubview(lblCatagoryItemSum);


            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

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

            //lbldelete = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            //lbldelete.TextColor = UIColor.White;
            //lbldelete.Text = "Delete";
            //lbldelete.TextAlignment = UITextAlignment.Center;
            //lbldelete.Font = lbldelete.Font.WithSize(12);
            //viewdelete.AddSubview(lbldelete);

            viewdelete.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("imageDeleteCategory:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            viewdelete.AddGestureRecognizer(tapGesture2);
            #endregion

            ContentView.BackgroundColor = UIColor.White;
            setupListView();

            var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
            this.AddGestureRecognizer(swipeRight);

        }
        [Export("imageDeleteCategory:")]
        public void DeleteCategory(UIGestureRecognizer sender)
        {
            OnDeleteItem.Invoke(this);
        }

        private void swiped(UISwipeGestureRecognizer s)
        {
            OnItemSwipe?.Invoke(this);
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

        }
        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();

        }
        private void setupListView()
        {

            lblCatagory.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblCatagory.BottomAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor,-3).Active = true;
            lblCatagory.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblCatagory.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lblCatagoryItemSum.TopAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, 3).Active = true;
            lblCatagoryItemSum.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblCatagoryItemSum.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblCatagoryItemSum.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;


            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            viewdelete.TopAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewdelete.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.WidthAnchor.ConstraintEqualTo(80).Active = true;
            viewdelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.BottomAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            imageDelete.TopAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            imageDelete.BottomAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.BottomAnchor, -5).Active = true;
            imageDelete.LeftAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            imageDelete.RightAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
        }
        public bool showbtndelete
        {
            get { return viewdelete.Hidden; }
            set
            { //viewdelete.Hidden = !value;
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
            get { return lblCatagory.Text; }
            set { lblCatagory.Text = value;
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
                //lblname.Text = value;
            }
        }
        public string Sum
        {
            get { return lblCatagoryItemSum.Text; }
            set { lblCatagoryItemSum.Text = value; }
        }

        public delegate void itemCategorySelectedDelegate(itemCatagoryCollectionViewCellList indexPath);
        public event itemCategorySelectedDelegate OnItemSwipe;

        public delegate void itemCategorydeleteDelegate(itemCatagoryCollectionViewCellList indexPath);
        public event itemCategorydeleteDelegate OnDeleteItem;
    }
}