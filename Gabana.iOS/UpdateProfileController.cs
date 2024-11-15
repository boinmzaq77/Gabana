using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public partial class UpdateProfileController : UIViewController
    {
        UIImageView profileImg;
        UIView line;
        UITextField txtMerchantName, txtUserName;
        UIButton btnSave,btnEditImage;
        UILabel lblMerchantName, lblUserName;
        public UpdateProfileController() { }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            profileImg = new UIImageView();
            profileImg.Image = UIImage.FromFile("test.jpg");
            profileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            profileImg.Layer.CornerRadius = 75f;
            profileImg.ClipsToBounds = true;
            View.AddSubview(profileImg);

            btnEditImage = new UIButton();
            btnEditImage.Layer.CornerRadius = 25f;
            btnEditImage.SetBackgroundImage(UIImage.FromFile("Album.png"), UIControlState.Normal);
            btnEditImage.TranslatesAutoresizingMaskIntoConstraints = false;
            btnEditImage.TouchUpInside += (sender, e) =>
            {
                btnEditImage_TouchUpInside();
            };
            View.AddSubview(btnEditImage);

            line = new UIView();
            line.BackgroundColor = new UIColor(red:248/255f,green:248/255f,blue:248/255f,alpha:1f);
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(line);

            lblMerchantName = new UILabel();
            lblMerchantName.Text = "Merchant Name";
            lblMerchantName.Font = lblMerchantName.Font.WithSize(15);
            lblMerchantName.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
            lblMerchantName.Lines = 1;
            lblMerchantName.TextAlignment = UITextAlignment.Left;
            lblMerchantName.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblMerchantName);

            lblUserName = new UILabel();
            lblUserName.TranslatesAutoresizingMaskIntoConstraints = false;
            lblUserName.Text = "User Name";
            lblUserName.Font = lblUserName.Font.WithSize(15);
            lblUserName.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
            lblUserName.Lines = 1;
            lblUserName.TextAlignment = UITextAlignment.Left;
            lblUserName.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblUserName);

            txtMerchantName = new UITextField
            {
                TextAlignment = UITextAlignment.Left,
                Placeholder = "user name",
                BackgroundColor = UIColor.White,
                TextColor = UIColor.Black
            };
            txtMerchantName.Font = txtMerchantName.Font.WithSize(15);
            txtMerchantName.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(txtMerchantName);

            txtUserName = new UITextField {
                TextAlignment = UITextAlignment.Left,
                Placeholder = "merchant name",
                BackgroundColor = UIColor.White,
                TextColor = UIColor.Black
            };
            txtUserName.Font = txtUserName.Font.WithSize(15);
            txtUserName.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(txtUserName);

            btnSave = new UIButton();
            btnSave.SetTitle("Save", UIControlState.Normal);
            btnSave.BackgroundColor = new UIColor(red: 226 / 255f, green: 226 / 255f, blue: 226 / 255f, alpha: 1);
            btnSave.Layer.CornerRadius = 5;
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += (sender, e) =>
            {
                btnSave_TouchUpInside();
            };
            View.AddSubview(btnSave);

            setupAutoLayout();
        }
        void btnEditImage_TouchUpInside()
        {

        }
        void btnSave_TouchUpInside()
        {

        }
        void setupAutoLayout()
        {
            profileImg.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            profileImg.TopAnchor.ConstraintEqualTo(View.TopAnchor, 38).Active = true;
            profileImg.WidthAnchor.ConstraintEqualTo(150).Active = true;
            profileImg.HeightAnchor.ConstraintEqualTo(150).Active = true;

            line.TopAnchor.ConstraintEqualTo(profileImg.BottomAnchor, 37).Active = true;
            line.LeftAnchor.ConstraintEqualTo(View.LeftAnchor,0).Active = true;
            line.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(5).Active = true;

            //lblMerchantName
            lblMerchantName.TopAnchor.ConstraintEqualTo(line.BottomAnchor, 11).Active = true;
            lblMerchantName.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 15).Active = true;
            lblMerchantName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            //txtMerchantName
            txtMerchantName.TopAnchor.ConstraintEqualTo(lblMerchantName.BottomAnchor, 2).Active = true;
            txtMerchantName.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 15).Active = true;
            txtMerchantName.RightAnchor.ConstraintEqualTo(View.RightAnchor, -15).Active = true;
            txtMerchantName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            //lblUserName
            lblUserName.TopAnchor.ConstraintEqualTo(txtMerchantName.BottomAnchor, 22).Active = true;
            lblUserName.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 15).Active = true;
            lblUserName.RightAnchor.ConstraintEqualTo(View.RightAnchor, -15).Active = true;
            lblUserName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            //txtUserName
            txtUserName.TopAnchor.ConstraintEqualTo(lblUserName.BottomAnchor, 2).Active = true;
            txtUserName.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 15).Active = true;
            txtUserName.RightAnchor.ConstraintEqualTo(View.RightAnchor, -15).Active = true;
            txtUserName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            //btnSave
            btnSave.BottomAnchor.ConstraintEqualTo(View.BottomAnchor,-10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(View.RightAnchor, -10).Active = true;
            btnSave.HeightAnchor.ConstraintEqualTo(45).Active = true;

            btnEditImage.TopAnchor.ConstraintEqualTo(profileImg.TopAnchor, 106).Active = true;
            btnEditImage.LeftAnchor.ConstraintEqualTo(profileImg.LeftAnchor, 106).Active = true;
            btnEditImage.HeightAnchor.ConstraintEqualTo(44).Active = true;
            btnEditImage.WidthAnchor.ConstraintEqualTo(44).Active = true;

        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}