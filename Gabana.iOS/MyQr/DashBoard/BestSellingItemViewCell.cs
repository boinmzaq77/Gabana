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
    public class BestSellingItemViewCell : UICollectionViewCell
    {
        UILabel lblname, lblShortName,lblSaleTotal;
        UIView line;
        UIImageView imageView,StatusItem;

        public BestSellingItemViewCell(IntPtr handle) : base(handle)
        {
            imageView = new UIImageView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(imageView);

            lblShortName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblShortName.TextColor = UIColor.White;
            lblShortName.TextAlignment = UITextAlignment.Center;
            lblShortName.Font = lblShortName.Font.WithSize(12);
            imageView.AddSubview(lblShortName);

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblname.TextColor = UIColor.FromRGB(64, 64, 64);
            lblname.Font = lblname.Font.WithSize(15);
            ContentView.AddSubview(lblname);


            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

            lblSaleTotal= new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false,TextAlignment = UITextAlignment.Right };
            lblSaleTotal.TextColor = UIColor.FromRGB(64, 64, 64);
            lblSaleTotal.Font = lblSaleTotal.Font.WithSize(15);
            ContentView.AddSubview(lblSaleTotal);

            StatusItem = new UIImageView();
            StatusItem.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(StatusItem);

            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
      
        private void setupListView()
        {

            imageView.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(42).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(56).Active = true;

            lblname.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblname.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(lblSaleTotal.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;

            lblShortName.CenterYAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortName.WidthAnchor.ConstraintEqualTo(60).Active = true;
            lblShortName.CenterXAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            StatusItem.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            StatusItem.WidthAnchor.ConstraintEqualTo(24).Active = true;
            StatusItem.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            StatusItem.HeightAnchor.ConstraintEqualTo(24).Active = true;

            lblSaleTotal.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblSaleTotal.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblSaleTotal.RightAnchor.ConstraintEqualTo(StatusItem.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
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
        public string Status
        {
            get { return StatusItem.Image.ToString(); }
            set
            {
                Utils.SetImage(StatusItem, value);
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
        public string Image
        {
            get { return imageView.Image.ToString(); }
            set
            {
                Utils.SetImageURL(imageView, value);
            }
        }

        public long Colors
        {
            get { return Colors; }
            set { Utils.SetColor(imageView, value); }
        }
        public string ShortName
        {
            get { return lblShortName.Text; }
            set { lblShortName.Text = value; }
        }
        
    }

    }