using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using TinyInsightsLib;

namespace Gabana.iOS
{
    public class OptionNoteCollectionViewCell : UICollectionViewCell
    {
        UILabel lblmenu;
        public OptionNoteCollectionViewCell(IntPtr handle) : base(handle)
        {
                lblmenu = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
                lblmenu.TextColor = UIColor.FromRGB(160, 160, 160);
                lblmenu.Font = lblmenu.Font.WithSize(18);
                lblmenu.TextAlignment = UITextAlignment.Center;
                ContentView.AddSubview(lblmenu);


                ContentView.BackgroundColor = UIColor.White;
                ContentView.Layer.BorderColor = UIColor.FromRGB(0,149,218).CGColor;
                ContentView.Layer.BorderWidth = 0.7f;
                ContentView.Layer.CornerRadius = 5;
                ContentView.ClipsToBounds = true;
                SetupAutoLayout();
            
        }
        void SetupAutoLayout()
        {
            lblmenu.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            lblmenu.HeightAnchor.ConstraintEqualTo(33).Active = true;
            lblmenu.LeadingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeadingAnchor, 10).Active = true;
            lblmenu.TrailingAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TrailingAnchor, -10).Active = true;

        }
        public string Name
        {
            get { return lblmenu.Text; }
            set { lblmenu.Text = value; }
        }
        public nfloat size
        {
            set { lblmenu.WidthAnchor.ConstraintLessThanOrEqualTo(value).Active = true; }
        }
    }
}