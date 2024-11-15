using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class EmployeeViewCell : UICollectionViewCell
    {
        UIImageView ProfileImg,StatusImg;
        UILabel lblEmployee,lblPermit;
        UIView line;
        UIImageView imageDelete;
        UIView viewdelete, viewall;

        public EmployeeViewCell(IntPtr handle) : base(handle)
        {
            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);

            ProfileImg = new UIImageView();
            ProfileImg.TranslatesAutoresizingMaskIntoConstraints = false;
           // ProfileImg.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            ProfileImg.Image = UIImage.FromFile("defaultcust.png");
            viewall.AddSubview(ProfileImg);

            StatusImg = new UIImageView();
            StatusImg.TranslatesAutoresizingMaskIntoConstraints = false;
            StatusImg.BackgroundColor = UIColor.FromRGB(200, 200, 200);
            StatusImg.ClipsToBounds = true;
            StatusImg.Layer.CornerRadius = 10;
            viewall.AddSubview(StatusImg);

            lblEmployee = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblEmployee.TextColor = UIColor.FromRGB(64,64,64);
            lblEmployee.TextAlignment = UITextAlignment.Left;
            lblEmployee.Font = lblEmployee.Font.WithSize(15);
            viewall.AddSubview(lblEmployee);

            lblPermit = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPermit.TextColor = UIColor.FromRGB(162,162,162);
            lblPermit.TextAlignment = UITextAlignment.Left;
            lblPermit.Font = lblEmployee.Font.WithSize(15);
            viewall.AddSubview(lblPermit);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226,226,226);
            viewall.AddSubview(line);

            ContentView.BackgroundColor = UIColor.White;

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
            ProfileImg.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ProfileImg.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            ProfileImg.HeightAnchor.ConstraintEqualTo(34).Active = true;
            ProfileImg.WidthAnchor.ConstraintEqualTo(34).Active = true;

            lblEmployee.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor,-8).Active = true;
            lblEmployee.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblEmployee.WidthAnchor.ConstraintEqualTo(300).Active = true;

            lblPermit.TopAnchor.ConstraintEqualTo(lblEmployee.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            lblPermit.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblPermit.WidthAnchor.ConstraintEqualTo(300).Active = true;

            StatusImg.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            StatusImg.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -30).Active = true;
            StatusImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            StatusImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -5).Active = true;

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
        public string Name
        {
            get { return lblEmployee.Text; }
            set {
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
                lblEmployee.Text = value; }
        }

        public bool swipe
        {
            
            set
            {
                if (value)
                {
                    var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
                    swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
                    this.AddGestureRecognizer(swipeRight);
                }
                
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
        public string Permit
        {
            get { return lblPermit.Text; }
            set { lblPermit.Text = value; }
        }
        public bool Status
        {
            get { 
                if (StatusImg.Image != null)
                    {
                        return true;
                    }
                    else
                        return false; 
                }
            set { 
                if(value == true)
                    {
                        StatusImg.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    }
                    else
                        StatusImg.BackgroundColor = UIColor.FromRGB(200, 200, 200);
            }
        }
        public string Image
        {
            get { return ProfileImg.Image.ToString(); }
            set
            {
                    Utils.SetImage(ProfileImg, value);
            }
        }

        public delegate void EmpSelectedDelegate(EmployeeViewCell indexPath);
        public event EmpSelectedDelegate OnItemSwipe;

        public delegate void EmpdeleteDelegate(EmployeeViewCell indexPath);
        public event EmpdeleteDelegate OnDeleteItem;
    }
}