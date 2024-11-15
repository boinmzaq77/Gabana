using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class CurrencyViewCell : UICollectionViewCell
    {
        UILabel lblBranch;
        UIImageView settingImg,IconImage;

        public CurrencyViewCell(IntPtr handle) : base(handle)
        {
            IconImage = new UIImageView();
           // IconImage.Image = UIImage.FromBundle("Setting");
            IconImage.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(IconImage);

            lblBranch = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblBranch.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
            lblBranch.Font = lblBranch.Font.WithSize(15);
            ContentView.AddSubview(lblBranch);

            settingImg = new UIImageView();
            settingImg.Image = UIImage.FromFile("Check.png");
            settingImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(settingImg);


            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            IconImage.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            IconImage.HeightAnchor.ConstraintEqualTo(28).Active = true;
            IconImage.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            IconImage.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBranch.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblBranch.LeftAnchor.ConstraintEqualTo(IconImage.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblBranch.WidthAnchor.ConstraintEqualTo(150).Active = true;

            settingImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            settingImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            settingImg.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            settingImg.WidthAnchor.ConstraintEqualTo(20).Active = true;

        }
        public string Name
        {
            get { return lblBranch.Text; }
            set { lblBranch.Text = value; }
        }
        public string Image
        {
            get { return IconImage.Image.ToString(); }
            set
            {
                Utils.SetImage(IconImage, value);
            }
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