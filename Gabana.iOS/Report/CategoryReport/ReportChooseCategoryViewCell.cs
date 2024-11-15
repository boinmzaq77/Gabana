using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class ReportChooseCategoryViewCell : UICollectionViewCell
    {
        UILabel lblBranch,lblItem;
        UIView line;
        UIImageView SelectOwnerImg;

        public ReportChooseCategoryViewCell(IntPtr handle) : base(handle)
        {

            lblBranch = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblBranch.TextColor = UIColor.FromRGB(64,64,64);
            lblBranch.TextAlignment = UITextAlignment.Left;
            lblBranch.Font = lblBranch.Font.WithSize(15);
            ContentView.AddSubview(lblBranch);

            lblItem = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblItem.TextColor = UIColor.FromRGB(162,162,162);
            lblItem.TextAlignment = UITextAlignment.Left;
            lblItem.Font = lblItem.Font.WithSize(15);
            ContentView.AddSubview(lblItem);

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
            lblBranch.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor,-12).Active = true;
            lblBranch.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-25).Active = true;

            lblItem.TopAnchor.ConstraintEqualTo(lblBranch.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblItem.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            lblItem.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            SelectOwnerImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            SelectOwnerImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            SelectOwnerImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectOwnerImg.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
        }
        public string Item
        {
            get { return lblItem.Text; }
            set { lblItem.Text = value; }
        }
        public string Name
        {
            get { return lblBranch.Text; }
            set { lblBranch.Text = value; }
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