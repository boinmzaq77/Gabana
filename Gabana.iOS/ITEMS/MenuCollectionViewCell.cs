using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class MenuCollectionViewCell : UICollectionViewCell
    {
        UILabel lblmenu;
        UIView viewBar;

        public MenuCollectionViewCell(IntPtr handle) : base(handle)
        {
            lblmenu = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblmenu.TextColor = UIColor.FromRGB(160, 160, 160);
            lblmenu.Font = lblmenu.Font.WithSize(13);
            lblmenu.TextAlignment = UITextAlignment.Center;
            ContentView.AddSubview(lblmenu);

            viewBar = new UIView() { TranslatesAutoresizingMaskIntoConstraints = false };
            viewBar.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            viewBar.Hidden = true;
            ContentView.AddSubview(viewBar);

            ContentView.BackgroundColor = UIColor.White;

            SetupAutoLayout();
        }
        void SetupAutoLayout()
        {
            lblmenu.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            lblmenu.HeightAnchor.ConstraintEqualTo(33).Active = true;
            lblmenu.LeadingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeadingAnchor, 2).Active = true;
            lblmenu.TrailingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TrailingAnchor, 2).Active = true;

            viewBar.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            viewBar.HeightAnchor.ConstraintEqualTo(2).Active = true;
            viewBar.LeadingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeadingAnchor, 2).Active = true;
            viewBar.TrailingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TrailingAnchor, 2).Active = true;
        }
        public string Name
        {
            get { return lblmenu.Text; }
            set { lblmenu.Text = value; }
        }
      
        public void ShowSelected(bool isSelected)
        {
            if (isSelected)
            {
                lblmenu.TextColor = UIColor.FromRGB(0, 149, 218);
                viewBar.Hidden = false;
            }
            else
            {
                lblmenu.TextColor = UIColor.FromRGB(160, 160, 160);
                viewBar.Hidden = true;
            }
        }

    }
}