using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class BluetoothViewCell : UICollectionViewCell
    {
        UILabel lblCode,lblStatus;
        UIImageView IconImage;

        public BluetoothViewCell(IntPtr handle) : base(handle)
        {
           
            lblCode = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false , TextAlignment=UITextAlignment.Left };
            lblCode.TextColor = UIColor.FromRGB(0, 149, 218);
            lblCode.Font = lblCode.Font.WithSize(12);
            //lblCode.BackgroundColor = UIColor.Red;
            ContentView.AddSubview(lblCode);

            lblStatus = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, TextAlignment = UITextAlignment.Right };
            lblStatus.TextColor = UIColor.FromRGB(64,64,64);
            lblStatus.Font = lblStatus.Font.WithSize(15);
            //lblStatus.BackgroundColor = UIColor.Black;
            ContentView.AddSubview(lblStatus);

            IconImage = new UIImageView();
            IconImage.TranslatesAutoresizingMaskIntoConstraints = false;
            IconImage.Image = UIImage.FromFile("BluetoothG.png");
            //IconImage.BackgroundColor = UIColor.Blue;
            ContentView.AddSubview(IconImage);


            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            lblCode.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblCode.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblCode.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            //lblCode.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterXAnchor,0).Active = true;

            lblStatus.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblStatus.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblStatus.LeftAnchor.ConstraintEqualTo(lblCode.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblStatus.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblStatus.RightAnchor.ConstraintEqualTo(IconImage.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;

            IconImage.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            IconImage.HeightAnchor.ConstraintEqualTo(28).Active = true;
            IconImage.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            IconImage.WidthAnchor.ConstraintEqualTo(28).Active = true;
        }
        public string Name
        {
            get { return lblCode.Text; }
            set { lblCode.Text = value; }
        }
        public bool Status
        {
            get {
                if (lblStatus.Text == "Connected")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { 
                if(value)
                {
                    lblStatus.Text = "Connected";
                    lblStatus.TextColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    lblStatus.Text = "Not Connected";
                    lblStatus.TextColor = UIColor.FromRGB(172, 172, 172);
                }
             }
        }
        public string Image
        {
            get { return IconImage.Image.ToString(); }
            set
            {
                Utils.SetImage(IconImage, value);
            }
        }
     
    }
}