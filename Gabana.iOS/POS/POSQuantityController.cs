using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public partial class POSQuantityController : UIViewController
    {
        UIView quantityView, bottomView, numpadView,emptyView;
        UILabel  lblQuantity;
        string setQuantity;
        UIButton btnAddQuantity,btnIncrase,btnDecrease;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete;
        public POSQuantityController()
        {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            setQuantity = POSController.txtQuantity;
            this.lblQuantity.Text = setQuantity;
        }
        public override void ViewDidLoad()
        {
            setQuantity = POSController.txtQuantity;

            Utils.SetTitle(this.NavigationController, "Quantity");
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            //  View.BackgroundColor = UIColor.White;
            View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region quantityView
            quantityView = new UIView();
             quantityView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            // quantityView.BackgroundColor = UIColor.Red;
            quantityView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(quantityView);

            lblQuantity = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = new UIColor(red: 0 / 225f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblQuantity.Text = setQuantity;
            lblQuantity.Font = lblQuantity.Font.WithSize(54);
            View.AddSubview(lblQuantity);

            btnIncrase = new UIButton();
            btnIncrase.Layer.CornerRadius = 25;
            btnIncrase.SetImage(UIImage.FromBundle("Add"), UIControlState.Normal);
            btnIncrase.ImageView.ContentMode = UIViewContentMode.ScaleToFill;
            btnIncrase.TranslatesAutoresizingMaskIntoConstraints = false;
            btnIncrase.TouchUpInside += (sender, e) => {
                //increase
                int x = Convert.ToInt32(setQuantity);
                if (x>=0)
                {
                    x++;
                }
                setQuantity = x.ToString();
                lblQuantity.Text = setQuantity;
                checkbtn();
            };
            View.AddSubview(btnIncrase);

            btnDecrease = new UIButton();
            btnDecrease.Layer.CornerRadius = 25;
            btnDecrease.SetImage(UIImage.FromBundle("Minus"), UIControlState.Normal);
            btnDecrease.ImageView.ContentMode = UIViewContentMode.ScaleToFill;
            btnDecrease.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDecrease.TouchUpInside += (sender, e) => {
                //decrease
                int x = Convert.ToInt32(setQuantity);
                if (x > 0)
                {
                    x--;
                }
                setQuantity = x.ToString();
                lblQuantity.Text = setQuantity;
                checkbtn();
            };
            View.AddSubview(btnDecrease);
            #endregion
            #region numpadView
            numpadView = new UIView();
            numpadView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            numpadView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(numpadView);
            #endregion

            #region bottomView
            bottomView = new UIView();
            bottomView.BackgroundColor = UIColor.White;
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(bottomView);

            btnAddQuantity = new UIButton();
            btnAddQuantity.Layer.CornerRadius = 5;
            btnAddQuantity.SetTitle(Utils.TextBundle("done", "Done"), UIControlState.Normal);
            btnAddQuantity.BackgroundColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1);
            btnAddQuantity.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddQuantity.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddQuantity.TouchUpInside += (sender, e) => {
                // search function
                if (Convert.ToInt32(lblQuantity.Text) > 0 && lblQuantity.Text.Length > 0)
                {
                    POSController.txtQuantity = lblQuantity.Text;
                    POSController.Quantity = Int32.Parse( lblQuantity.Text);
                    this.NavigationController.PopViewController(false);
                }
            };
            View.AddSubview(btnAddQuantity);
            #endregion
            SetupAutoLayout();
        }
        void SetupAutoLayout()
        {
            quantityView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            quantityView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            quantityView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            quantityView.BottomAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;

            lblQuantity.CenterXAnchor.ConstraintEqualTo(quantityView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblQuantity.CenterYAnchor.ConstraintEqualTo(quantityView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblQuantity.WidthAnchor.ConstraintEqualTo(190).Active = true;

            btnDecrease.RightAnchor.ConstraintEqualTo(lblQuantity.SafeAreaLayoutGuide.LeftAnchor,-15).Active = true;
            btnDecrease.CenterYAnchor.ConstraintEqualTo(quantityView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnDecrease.WidthAnchor.ConstraintEqualTo(35).Active = true;
            btnDecrease.HeightAnchor.ConstraintEqualTo(35).Active = true;

            btnIncrase.LeftAnchor.ConstraintEqualTo(lblQuantity.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            btnIncrase.CenterYAnchor.ConstraintEqualTo(quantityView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnIncrase.WidthAnchor.ConstraintEqualTo(35).Active = true;
            btnIncrase.HeightAnchor.ConstraintEqualTo(35).Active = true;

            numpadView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*50)/100).Active = true;
            numpadView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            numpadView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            numpadView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height/10).Active = true;

            btnAddQuantity.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddQuantity.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddQuantity.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnAddQuantity.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

            NumberpadSetup();
        }
        void NumberpadSetup()
        {
            btnone = new UIButton();
            btnone.BackgroundColor = UIColor.White;
            btnone.SetTitle("1", UIControlState.Normal);
            btnone.TitleLabel.Font = btnone.TitleLabel.Font.WithSize(30);
            btnone.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnone.TranslatesAutoresizingMaskIntoConstraints = false;
            btnone.TouchUpInside += (sender, e) => {
                //1 press
                setvalue(1);
                //setQuantity = setQuantity + '1';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnone);

            btntwo = new UIButton();
            btntwo.BackgroundColor = UIColor.White;
            btntwo.SetTitle("2", UIControlState.Normal);
            btntwo.TitleLabel.Font = btntwo.TitleLabel.Font.WithSize(30);
            btntwo.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btntwo.TranslatesAutoresizingMaskIntoConstraints = false;
            btntwo.TouchUpInside += (sender, e) => {
                //2 press
                setvalue(2);
                //setQuantity = setQuantity + '2';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btntwo);

            btnthree = new UIButton();
            btnthree.BackgroundColor = UIColor.White;
            btnthree.SetTitle("3", UIControlState.Normal);
            btnthree.TitleLabel.Font = btnthree.TitleLabel.Font.WithSize(30);
            btnthree.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnthree.TranslatesAutoresizingMaskIntoConstraints = false;
            btnthree.TouchUpInside += (sender, e) => {
                //3 press
                setvalue(3);
                //setQuantity = setQuantity + '3';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnthree);

            btnfour = new UIButton();
            btnfour.BackgroundColor = UIColor.White;
            btnfour.SetTitle("4", UIControlState.Normal);
            btnfour.TitleLabel.Font = btnfour.TitleLabel.Font.WithSize(30);
            btnfour.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnfour.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfour.TouchUpInside += (sender, e) => {
                //4 press
                setvalue(4);
                //setQuantity = setQuantity + '4';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnfour);

            btnfive = new UIButton();
            btnfive.BackgroundColor = UIColor.White;
            btnfive.SetTitle("5", UIControlState.Normal);
            btnfive.TitleLabel.Font = btnfive.TitleLabel.Font.WithSize(30);
            btnfive.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnfive.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfive.TouchUpInside += (sender, e) => {
                //5 press
                setvalue(5);
                //setQuantity = setQuantity + '5';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnfive);

            btnsix = new UIButton();
            btnsix.BackgroundColor = UIColor.White;
            btnsix.SetTitle("6", UIControlState.Normal);
            btnsix.TitleLabel.Font = btnsix.TitleLabel.Font.WithSize(30);
            btnsix.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnsix.TranslatesAutoresizingMaskIntoConstraints = false;
            btnsix.TouchUpInside += (sender, e) => {
                //6 press
                setvalue(6);
                //setQuantity = setQuantity + '6';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnsix);

            btnseven = new UIButton();
            btnseven.BackgroundColor = UIColor.White;
            btnseven.SetTitle("7", UIControlState.Normal);
            btnseven.TitleLabel.Font = btnseven.TitleLabel.Font.WithSize(30);
            btnseven.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnseven.TranslatesAutoresizingMaskIntoConstraints = false;
            btnseven.TouchUpInside += (sender, e) => {
                //7 press
                setvalue(7);
                //setQuantity = setQuantity + '7';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnseven);

            emptyView = new UIView();
            emptyView.BackgroundColor = UIColor.White;
            emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
            numpadView.AddSubview(emptyView);

            btneight = new UIButton();
            btneight.BackgroundColor = UIColor.White;
            btneight.SetTitle("8", UIControlState.Normal);
            btneight.TitleLabel.Font = btneight.TitleLabel.Font.WithSize(30);
            btneight.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btneight.TranslatesAutoresizingMaskIntoConstraints = false;
            btneight.TouchUpInside += (sender, e) => {
                //8 press
                setvalue(8);
                //setQuantity = setQuantity + '8';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btneight);

            btnnine = new UIButton();
            btnnine.BackgroundColor = UIColor.White;
            btnnine.SetTitle("9", UIControlState.Normal);
            btnnine.TitleLabel.Font = btnnine.TitleLabel.Font.WithSize(30);
            btnnine.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnnine.TranslatesAutoresizingMaskIntoConstraints = false;
            btnnine.TouchUpInside += (sender, e) => {
                //9 press
                setvalue(9);
                //setQuantity = setQuantity + '9';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnnine);

            btnzero = new UIButton();
            btnzero.BackgroundColor = UIColor.White;
            btnzero.SetTitle("0", UIControlState.Normal);
            btnzero.TitleLabel.Font = btnzero.TitleLabel.Font.WithSize(30);
            btnzero.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnzero.TranslatesAutoresizingMaskIntoConstraints = false;
            btnzero.TouchUpInside += (sender, e) => {
                //0 press
                setvalue(0);
                //setQuantity = setQuantity + '0';
                //lblQuantity.Text = setQuantity;
            };
            numpadView.AddSubview(btnzero);

            btndelete = new UIButton();
            btndelete.SetImage(UIImage.FromBundle("Del"), UIControlState.Normal);
            btndelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.BackgroundColor = UIColor.White;
            btndelete.TouchUpInside += (sender, e) => {
                //0 press
                if (setQuantity.Length > 0 && setQuantity != "0")
                {
                    setQuantity = setQuantity.Remove(setQuantity.Length - 1,1); ;
                }
                if(setQuantity.Length == 0 && setQuantity =="")
                {
                    setQuantity = "0";
                }
                lblQuantity.Text = setQuantity;
                checkbtn();
            };
            numpadView.AddSubview(btndelete);


            btnone.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnone.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnone.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnone.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width/3).Active = true;

            btntwo.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btntwo.LeftAnchor.ConstraintEqualTo(btnone.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btntwo.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btntwo.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnthree.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnthree.LeftAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnthree.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnthree.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnfour.TopAnchor.ConstraintEqualTo(btnone.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnfour.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnfour.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnfour.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnfive.TopAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnfive.LeftAnchor.ConstraintEqualTo(btnfour.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnfive.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnfive.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnsix.TopAnchor.ConstraintEqualTo(btnthree.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnsix.LeftAnchor.ConstraintEqualTo(btnfive.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnsix.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnsix.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnseven.TopAnchor.ConstraintEqualTo(btnfour.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnseven.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnseven.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnseven.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btneight.TopAnchor.ConstraintEqualTo(btnfive.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btneight.LeftAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btneight.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btneight.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnnine.TopAnchor.ConstraintEqualTo(btnsix.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnnine.LeftAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnnine.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnnine.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            emptyView.TopAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            emptyView.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            emptyView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnzero.TopAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnzero.CenterXAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnzero.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,1).Active = true;
            btnzero.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btndelete.TopAnchor.ConstraintEqualTo(btnnine.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btndelete.LeftAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btndelete.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btndelete.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
        }

        private void setvalue(int v)
        {
            if (Convert.ToInt32(setQuantity) > 0)
            {
                setQuantity = setQuantity + v.ToString();
                if (Int32.Parse(setQuantity) > 999999) setQuantity = "999999";
            }
            else
            {
                setQuantity = v.ToString();
            }

            lblQuantity.Text = setQuantity;
            checkbtn();
        }

        private void checkbtn()
        {
            if (lblQuantity.Text.Trim() =="0" )
            {
                btnAddQuantity.Enabled = false;
                btnAddQuantity.Layer.BorderWidth = 1f;
                btnAddQuantity.Layer.BorderColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1).CGColor;
                btnAddQuantity.BackgroundColor = UIColor.White;
                btnAddQuantity.SetTitleColor(new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1), UIControlState.Normal);
            }
            else
            {
                btnAddQuantity.Enabled = true;
                btnAddQuantity.BackgroundColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1);
                btnAddQuantity.SetTitleColor(UIColor.White, UIControlState.Normal);
            }
            
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}