using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class CustomerCollectionViewCell : UICollectionViewCell
    {
        UIImageView ProfileImg;
        UILabel lblCustomer;
        UIView line;
        UIImageView  imageDelete;
        UIView  viewdelete, viewall;

        public CustomerCollectionViewCell(IntPtr handle) : base(handle)
        {
            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);

            ProfileImg = new UIImageView();
            ProfileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ProfileImg.Layer.CornerRadius = 20;
            ProfileImg.ClipsToBounds = true;
            //   ProfileImg.BackgroundColor = UIColor.FromRGB(226,226,226);
            ProfileImg.Image = UIImage.FromFile("defaultcust.png");
            viewall.AddSubview(ProfileImg);

            ProfileImg.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("image:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ProfileImg.AddGestureRecognizer(tapGesture4);

            lblCustomer = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCustomer.TextColor = UIColor.FromRGB(64,64,64);
            lblCustomer.TextAlignment = UITextAlignment.Left;
            lblCustomer.Font = lblCustomer.Font.WithSize(15);
            viewall.AddSubview(lblCustomer);

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
            setupView();

            var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
            this.AddGestureRecognizer(swipeRight);

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
            ProfileImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ProfileImg.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            ProfileImg.HeightAnchor.ConstraintEqualTo(40).Active = true;
            ProfileImg.WidthAnchor.ConstraintEqualTo(40).Active = true;

            lblCustomer.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblCustomer.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblCustomer.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -0).Active = true;

            viewdelete.TopAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewdelete.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.WidthAnchor.ConstraintEqualTo(80).Active = true;
            viewdelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.BottomAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            imageDelete.TopAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            imageDelete.BottomAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            imageDelete.LeftAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            imageDelete.RightAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

        }
        public Customer Imageghavenet
        {
            //get { return imageView.Image.ToString(); }
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrEmpty(value.ThumbnailLocalPath))
                    {
                        Utils.SetImageCus(ProfileImg, value);

                    }
                    else
                    {
                        Utils.SetImageCuslocal(ProfileImg, value);
                    }
                }
                else
                {
                    ProfileImg.Image = UIImage.FromFile("defaultcust.png");
                }
            }
        }
        public Customer Imagegnothavenet
        {
            //get { return imageView.Image.ToString(); }
            set
            {
                if (value != null)
                {
                    Utils.SetImageCuslocal(ProfileImg, value);
                    //if (string.IsNullOrEmpty(value.ThumbnailLocalPath))
                    //{
                    //    Utils.SetImageItem(imageView, value);

                    //}
                    //else
                    //{
                    //    Utils.SetImageItemlocal(imageView, value);
                    //}
                }
                else
                {
                    ProfileImg.Image = UIImage.FromFile("defaultcust.png");
                    //ProfileImg.Image = null;
                }
            }
        }
        public string Name
        {
            get { return lblCustomer.Text; }
            set {
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
                lblCustomer.Text = value; }
        }
        public Customer Image
        {
            //get { return ProfileImg.Image.ToString(); }
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrEmpty(value.ThumbnailLocalPath))
                    {
                        Utils.SetImageCus(ProfileImg, value);

                    }
                    else
                    {
                        Utils.SetImageCuslocal(ProfileImg, value);
                    }
                }
                else
                {
                    ProfileImg.Image = UIImage.FromFile("defaultcust.png");
                }
                //ProfileImg.Image = UIImage.FromFile(value);
                //Utils.SetImageURL(ProfileImg, value);
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
        [Export("image:")]
        public void image(UIGestureRecognizer sender)
        {
            OnItem?.Invoke(this);
        }
        public delegate void CustomerSelectedDelegate(CustomerCollectionViewCell indexPath);
        public event CustomerSelectedDelegate OnItemSwipe;

        public delegate void CustomerdeleteDelegate(CustomerCollectionViewCell indexPath);
        public event CustomerdeleteDelegate OnDeleteItem;

        public delegate void CustomerDelegate(CustomerCollectionViewCell indexPath);
        public event CustomerdeleteDelegate OnItem;
    }
}