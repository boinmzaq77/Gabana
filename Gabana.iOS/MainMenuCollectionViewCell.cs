using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class MainMenuCollectionViewCell : UICollectionViewCell
    {
        UILabel lblmenu;
        UIImageView iconImg;

        public MainMenuCollectionViewCell(IntPtr handle) : base(handle)
        {
            lblmenu = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblmenu.TextColor = UIColor.FromRGB(64,64,64);
            lblmenu.Font = lblmenu.Font.WithSize(12);
            lblmenu.TextAlignment = UITextAlignment.Center;
            ContentView.AddSubview(lblmenu);

            iconImg = new UIImageView() { TranslatesAutoresizingMaskIntoConstraints = false };
            ContentView.AddSubview(iconImg);

            ContentView.BackgroundColor = UIColor.White;
            ContentView.Layer.BorderColor = UIColor.FromRGB(51,170,225).CGColor;
            ContentView.Layer.BorderWidth = 1;
            ContentView.Layer.CornerRadius = 8;
            SetupLayout();
        }
        void SetupLayout()
        {
            iconImg.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 15).Active = true;
            iconImg.HeightAnchor.ConstraintEqualTo(48).Active = true;
            iconImg.WidthAnchor.ConstraintEqualTo(48).Active = true;
            iconImg.CenterXAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblmenu.TopAnchor.ConstraintEqualTo(iconImg.SafeAreaLayoutGuide.BottomAnchor, 6).Active = true;
            lblmenu.HeightAnchor.ConstraintEqualTo(14).Active = true;
            lblmenu.LeadingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeadingAnchor, 5).Active = true;
            lblmenu.TrailingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TrailingAnchor, -5).Active = true;
        }
        public string Image
        {
            get { return iconImg.Image.ToString(); }
            set
            {
                SetImage(iconImg, value);
            }
        }
        public string Name
        {
            get { return lblmenu.Text; }
            set { lblmenu.Text = value; }
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            if (value != null && value != "")
            {
                ImageView.Image = UIImage.FromFile(value);
            }
        }
    }
}