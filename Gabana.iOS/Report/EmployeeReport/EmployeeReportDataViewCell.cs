using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;

namespace Gabana.iOS
{
    public class EmployeeReportDataViewCell : UICollectionViewCell
    {
        UILabel lblname,lblSaleTotal;
        UIImageView ProfileImg;
        private UILabel lblrole;

        public EmployeeReportDataViewCell(IntPtr handle) : base(handle)
        {
            ProfileImg = new UIImageView();
            ProfileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ProfileImg.Layer.CornerRadius = 20;
            ProfileImg.ClipsToBounds = true;
            ProfileImg.Image = UIImage.FromFile("defaultcust.png");
            ContentView.AddSubview(ProfileImg);

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false , TextAlignment = UITextAlignment.Left };
            lblname.TextColor = UIColor.FromRGB(64, 64, 64);
            lblname.Font = lblname.Font.WithSize(15);
            ContentView.AddSubview(lblname);

            lblrole = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextAlignment = UITextAlignment.Left };
            lblrole.TextColor = UIColor.FromRGB(64, 64, 64);
            lblrole.Font = lblname.Font.WithSize(15);
            ContentView.AddSubview(lblrole);

            lblSaleTotal = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false,TextAlignment = UITextAlignment.Right };
            lblSaleTotal.TextColor = UIColor.FromRGB(0,149,218);
            lblSaleTotal.Font = lblSaleTotal.Font.WithSize(15);
            ContentView.AddSubview(lblSaleTotal);


            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
      
        private void setupListView()
        {
            ProfileImg.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            ProfileImg.WidthAnchor.ConstraintEqualTo(40).Active = true;
            ProfileImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ProfileImg.HeightAnchor.ConstraintEqualTo(40).Active = true;

            lblname.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(lblSaleTotal.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            lblname.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, -2).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(16).Active = true;

            lblrole.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblrole.RightAnchor.ConstraintEqualTo(lblSaleTotal.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            lblrole.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 2).Active = true;
            lblrole.HeightAnchor.ConstraintEqualTo(16).Active = true;

            lblSaleTotal.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
       //     lblSaleTotal.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblSaleTotal.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblSaleTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

        }
        public string Total
        {
            get { return lblSaleTotal.Text; }
            set
            {
                lblSaleTotal.Text = value;
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

        public string Name
        {
            get { return lblname.Text; }
            set
            {
                lblname.Text = value;
            }
        }
        public string role
        {
            get { return lblrole.Text; }
            set
            {
                if (value == "")
                {
                    lblname.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 8).Active = true;
                }
                lblrole.Text = value;
            }
        }
    }

    }