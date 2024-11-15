using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class itemDiscountCollectionViewCellList : UICollectionViewCell
    {
        UILabel lblDiscount;
        UIView line;
        public itemDiscountCollectionViewCellList(IntPtr handle) : base(handle)
        {
            lblDiscount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblDiscount.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
            lblDiscount.Font = lblDiscount.Font.WithSize(14);
            ContentView.AddSubview(lblDiscount);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            lblDiscount.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDiscount.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblDiscount.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -1).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public string DiscountItem
        {
            get { return lblDiscount.Text; }
            set { lblDiscount.Text = value; }
        }
    }
}