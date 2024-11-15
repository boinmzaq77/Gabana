using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Xamarin.Forms.Platform.iOS;

namespace Gabana.iOS
{
    public class PaymentReportDataViewCell : UICollectionViewCell
    {
        UILabel lblname,lblSaleTotal;
        UIImageView ProfileImg;
        private UILabel lblper;
        private UIView lineview;

        public PaymentReportDataViewCell(IntPtr handle) : base(handle)
        {
            ProfileImg = new UIImageView();
            ProfileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            //ProfileImg.BackgroundColor = UIColor.FromRGB(0,149,218);
            ProfileImg.Layer.BorderWidth = 0;
            ContentView.AddSubview(ProfileImg);

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false , TextAlignment = UITextAlignment.Left };
            lblname.TextColor = UIColor.FromRGB(64, 64, 64);
            lblname.Font = lblname.Font.WithSize(15);
            ContentView.AddSubview(lblname);

            lblper = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextAlignment = UITextAlignment.Right };
            lblper.TextColor = UIColor.FromRGB(64, 64, 64);
            lblper.Font = lblper.Font.WithSize(15);
            ContentView.AddSubview(lblper);

            lblSaleTotal = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false,TextAlignment = UITextAlignment.Right };
            lblSaleTotal.TextColor = UIColor.FromRGB(64, 64, 64);
            lblSaleTotal.Font = lblSaleTotal.Font.WithSize(15);
            ContentView.AddSubview(lblSaleTotal);


            lineview = new UIView();
            lineview.Alpha = 0.1f;
            lineview.BackgroundColor = UIColor.Black;
            lineview.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(lineview);
            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
      
        private void setupListView()
        {
            ProfileImg.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            ProfileImg.WidthAnchor.ConstraintEqualTo(12).Active = true;
            ProfileImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ProfileImg.HeightAnchor.ConstraintEqualTo(12).Active = true;

            lblname.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblname.WidthAnchor.ConstraintEqualTo(90).Active = true;
            lblname.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(16).Active = true;
            //lblname.BackgroundColor = UIColor.Blue;

            lblper.LeftAnchor.ConstraintEqualTo(lblname.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblper.WidthAnchor.ConstraintEqualTo(65).Active = true;
            lblper.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblper.HeightAnchor.ConstraintEqualTo(16).Active = true;
            //lblper.BackgroundColor = UIColor.Gray;

            lblSaleTotal.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblSaleTotal.LeftAnchor.ConstraintEqualTo(lblper.SafeAreaLayoutGuide.RightAnchor,5).Active = true;
            lblSaleTotal.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblSaleTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;


            lineview.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            lineview.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor,0).Active = true;
            lineview.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,0).Active = true;
            lineview.HeightAnchor.ConstraintEqualTo(1).Active = true;

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

        public string persent 
        {
            get { return lblper.Text; }
            set
            {
                lblper.Text = value;
            }
        }
    }

    }