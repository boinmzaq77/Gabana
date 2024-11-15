using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class ReportChooseBranchViewCell : UICollectionViewCell
    {
        UILabel lblBranch;
        UIView line;
        UIImageView SelectOwnerImg;

        public ReportChooseBranchViewCell(IntPtr handle) : base(handle)
        {

            lblBranch = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblBranch.TextColor = UIColor.FromRGB(64,64,64);
            lblBranch.TextAlignment = UITextAlignment.Left;
            lblBranch.Font = lblBranch.Font.WithSize(15);
            ContentView.AddSubview(lblBranch);

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
            lblBranch.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor,0).Active = true;
            lblBranch.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            lblBranch.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-25).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            SelectOwnerImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            SelectOwnerImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            SelectOwnerImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectOwnerImg.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -30).Active = true;
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