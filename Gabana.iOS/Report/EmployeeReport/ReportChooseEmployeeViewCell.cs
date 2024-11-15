using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class ReportChooseEmployeeViewCell : UICollectionViewCell
    {
        UILabel lblCustomer;
        UIView line;
        UIImageView SelectOwnerImg,ProfileImg;
        private UILabel lblrole;

        public ReportChooseEmployeeViewCell(IntPtr handle) : base(handle)
        {

            ProfileImg = new UIImageView();
            ProfileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ProfileImg.Layer.CornerRadius = 20;
            ProfileImg.ClipsToBounds = true;
            ProfileImg.Image = UIImage.FromFile("defaultcust.png");
            ContentView.AddSubview(ProfileImg);

            lblCustomer = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCustomer.TextColor = UIColor.FromRGB(64,64,64);
            lblCustomer.TextAlignment = UITextAlignment.Left;
            lblCustomer.Font = lblCustomer.Font.WithSize(15);
            ContentView.AddSubview(lblCustomer);

            lblrole = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblrole.TextColor = UIColor.FromRGB(64, 64, 64);
            lblrole.TextAlignment = UITextAlignment.Left;
            lblrole.Font = lblrole.Font.WithSize(15);
            ContentView.AddSubview(lblrole);


            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226,226,226);
            ContentView.AddSubview(line);

            SelectOwnerImg = new UIImageView();
            SelectOwnerImg.Image = UIImage.FromBundle("Check");
            SelectOwnerImg.Hidden = true;
            SelectOwnerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(SelectOwnerImg);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            ProfileImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ProfileImg.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            ProfileImg.HeightAnchor.ConstraintEqualTo(40).Active = true;
            ProfileImg.WidthAnchor.ConstraintEqualTo(40).Active = true;

            lblCustomer.BottomAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.CenterYAnchor , -2).Active = true;
            lblCustomer.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblCustomer.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-25).Active = true;

            lblrole.TopAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.CenterYAnchor , 2).Active = true;
            lblrole.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblrole.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            SelectOwnerImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            SelectOwnerImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            SelectOwnerImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectOwnerImg.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
        }
        public string Name
        {
            get { return lblCustomer.Text; }
            set { lblCustomer.Text = value; }
        }
        public string role
        {
            get { return lblrole.Text; }
            set { lblrole.Text = value; }
        }
        public string Image
        {
            get { return ProfileImg.Image.ToString(); }
            set
            {
                Utils.SetImage(ProfileImg, value);
            }
        }
        public bool status
        {
            get { return SelectOwnerImg.Hidden; }
            set
            {
                if(value == true)
                {
                    SelectOwnerImg.Hidden = false;
                }
                else
                {
                    SelectOwnerImg.Hidden = true;
                }
            }
        }
    }
}