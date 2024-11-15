using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class ItemTopSaleReportDataViewCell : UICollectionViewCell
    {
        UILabel lblname,lblSaleTotal, lblShortName;
        UIImageView ProfileImg;

        public ItemTopSaleReportDataViewCell(IntPtr handle) : base(handle)
        {
            ProfileImg = new UIImageView();
            ProfileImg.TranslatesAutoresizingMaskIntoConstraints = false;
         //   ProfileImg.BackgroundColor = UIColor.FromRGB(0,149,218);
            ContentView.AddSubview(ProfileImg);

            lblShortName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblShortName.TextColor = UIColor.White;
            lblShortName.TextAlignment = UITextAlignment.Center;
            lblShortName.Font = lblShortName.Font.WithSize(12);
            ContentView.AddSubview(lblShortName);

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false , TextAlignment = UITextAlignment.Left };
            lblname.TextColor = UIColor.FromRGB(64, 64, 64);
            lblname.Font = lblname.Font.WithSize(15);
            lblname.Lines = 2;
            ContentView.AddSubview(lblname);

            lblSaleTotal= new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false,TextAlignment = UITextAlignment.Right };
            lblSaleTotal.TextColor = UIColor.FromRGB(0,149,218);
            lblSaleTotal.Font = lblSaleTotal.Font.WithSize(15);
            ContentView.AddSubview(lblSaleTotal);


            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
      
        private void setupListView()
        {
            ProfileImg.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            ProfileImg.WidthAnchor.ConstraintEqualTo(56).Active = true;
            ProfileImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ProfileImg.HeightAnchor.ConstraintEqualTo(42).Active = true;

            lblShortName.CenterYAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortName.WidthAnchor.ConstraintEqualTo(60).Active = true;
            lblShortName.CenterXAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            

            lblSaleTotal.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblSaleTotal.WidthAnchor.ConstraintGreaterThanOrEqualTo(100).Active = true;
            lblSaleTotal.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblSaleTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblname.LeftAnchor.ConstraintEqualTo(ProfileImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(lblSaleTotal.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            lblname.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblname.HeightAnchor.ConstraintGreaterThanOrEqualTo(60).Active = true;
            lblname.WidthAnchor.ConstraintLessThanOrEqualTo(300).Active = true;
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
        public string ShortName
        {
            get { return lblShortName.Text; }
            set { lblShortName.Text = value; }
        }

        public long Colors
        {
            get { return Colors; }
            set { Utils.SetColor(ProfileImg, value); }
        }
        

        public string Name
        {
            get { return lblname.Text; }
            set
            {
                lblname.Text = value;
            }
        }
    }

    }