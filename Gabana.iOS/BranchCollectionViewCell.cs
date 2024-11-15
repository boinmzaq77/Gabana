using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class BranchCollectionViewCell : UICollectionViewCell
    {
        UILabel lblBranch;
        UIImageView settingImg;
        UIView line;

        public BranchCollectionViewCell(IntPtr handle) : base(handle)
        {
            lblBranch = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblBranch.TextColor = UIColor.FromRGB(64, 64, 64);
            lblBranch.Font = lblBranch.Font.WithSize(15);
            ContentView.AddSubview(lblBranch);

            settingImg = new UIImageView();
            settingImg.Image = UIImage.FromBundle("Check");
            settingImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(settingImg);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(248,248,248);
            ContentView.AddSubview(line);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            lblBranch.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblBranch.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(settingImg.SafeAreaLayoutGuide.LeftAnchor,-25).Active = true;

            settingImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            settingImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            settingImg.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            settingImg.WidthAnchor.ConstraintEqualTo(20).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

        }
        public string Name
        {
            get { return lblBranch.Text; }
            set { lblBranch.Text = value; }
        }
        public bool show
        {
            set
            {
                if (!value)
                {
                    settingImg.Hidden = true;
                }
                else
                {
                    settingImg.Hidden = false;
                }
            }
        }
    }
}