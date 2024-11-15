using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class POSCustomerCollectionViewCell : UICollectionViewCell
    {
        UILabel lblCustomer;
        UIView line;
        UIImageView SelectBtn;
        UIImageView ProfileImg;

        public POSCustomerCollectionViewCell(IntPtr handle) : base(handle)
        {
            ProfileImg = new UIImageView();
            ProfileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ProfileImg.Layer.CornerRadius = 20;
            ProfileImg.ClipsToBounds = true;
            //   ProfileImg.BackgroundColor = UIColor.FromRGB(226,226,226);
            ProfileImg.Image = UIImage.FromFile("defaultcust.png");
            ContentView.AddSubview(ProfileImg);

            lblCustomer = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCustomer.TextColor = UIColor.FromRGB(64,64,64);
            lblCustomer.Font = lblCustomer.Font.WithSize(15);
            lblCustomer.TextAlignment = UITextAlignment.Left;
            ContentView.AddSubview(lblCustomer);

            SelectBtn = new UIImageView();
            SelectBtn.Hidden = true;
            SelectBtn.TranslatesAutoresizingMaskIntoConstraints = false;
            SelectBtn.Image = UIImage.FromBundle("Check");
            ContentView.AddSubview(SelectBtn);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226,226,226);
            ContentView.AddSubview(line);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            ProfileImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ProfileImg.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            ProfileImg.HeightAnchor.ConstraintEqualTo(40).Active = true;
            ProfileImg.WidthAnchor.ConstraintEqualTo(40).Active = true;
            //ProfileImg.BackgroundColor = UIColor.Red;

            lblCustomer.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblCustomer.LeftAnchor.ConstraintEqualTo(ProfileImg.RightAnchor , 10 ).Active = true;
            lblCustomer.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            lblCustomer.WidthAnchor.ConstraintEqualTo(100).Active = true;
            //lblCustomer.BackgroundColor = UIColor.Green;

            SelectBtn.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            SelectBtn.HeightAnchor.ConstraintEqualTo(20).Active = true;
            SelectBtn.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            SelectBtn.WidthAnchor.ConstraintEqualTo(20).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public string Name
        {
            get { return lblCustomer.Text; }
            set { lblCustomer.Text = value; }
        }
        public string Image
        {
            get { return ProfileImg.Image.ToString(); }
            set
            {
                ProfileImg.Image = UIImage.FromFile(value);
                //Utils.SetImageURL(ProfileImg, value);
            }
        }
        public bool selectCheck
        {
            set
            {
                if (value == true)
                {
                    SelectBtn.Hidden = false;
                }
                else
                {
                    SelectBtn.Hidden = true;
                }
            }
        }

    }
}