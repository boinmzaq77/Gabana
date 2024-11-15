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
    public class ItemStockReportViewCell : UICollectionViewCell
    {
        UILabel lblname, lblShortName;
        UIView line;
        UIImageView imageView,selectItem;

        public ItemStockReportViewCell(IntPtr handle) : base(handle)
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
            lblname.Font = lblname.Font.WithSize(14);
            ContentView.AddSubview(lblname);


            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

            selectItem = new UIImageView();
            selectItem.Hidden = true;
            selectItem.TranslatesAutoresizingMaskIntoConstraints = false;
            selectItem.Image = UIImage.FromBundle("Check");
            ContentView.AddSubview(selectItem);

            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
      
        private void setupListView()
        {

            imageView.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(42).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(56).Active = true;

            lblname.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblname.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(selectItem.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;

            lblShortName.CenterYAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortName.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.LeftAnchor,5).Active = true;
            lblShortName.RightAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;
            lblShortName.CenterXAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            selectItem.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            selectItem.WidthAnchor.ConstraintEqualTo(20).Active = true;
            selectItem.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -30).Active = true;
            selectItem.HeightAnchor.ConstraintEqualTo(20).Active = true;

        }

        public bool Select
        {
            get { return selectItem.Hidden; }
            set
            {
                selectItem.Hidden = true;
                if (value)
                {
                    selectItem.Hidden = false;
                }
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