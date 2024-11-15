//using Foundation;
using CoreGraphics;
using Foundation;
using Gabana.AppSetting;
using Gabana.iOS.Test;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Merchant;
using System;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class LoginController : UIViewController
    {
        UITextField txtMobile, txtMerID, txtUsername, txtPassword;
        UIButton btnGoLogin, btnGoRegister, btnRegister, btnBack, btnLoginOwner, btnLoginEmp;
        UIImageView logoImg, telImg, merchantImg, usernameImg, passwordImg, GabanaTextImg, powerbySNS, MerImg,passImg , userImg;
        UISegmentedControl loginSegment;
        UIView buttonBar, buttonBar2;
        UIButton btnowner, btnExpire;
        CGRect framelogo;
        Gabana3.JAM.Merchant.Merchants merchants = new Merchants();
        private UILabel lblversion;

        public LoginController()
        {
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
             
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(true, false);
        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (await GabanaAPI.CheckNetWork())
            {
                GabanaAPI.ccJWT = await GetToken.Get_ccJWT();
            }
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);
            // Perform any additional setup after loading the view, typically from a nib.
            // navbar = new UINavigationController();

            View.BackgroundColor = UIColor.White;

            logoImg = new UIImageView();
            // logoImg.Image = UIImage.FromFile("GabanaLogIn.png");
            logoImg.Image = UIImage.FromBundle("GabanaLogIn");
            logoImg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(logoImg);

            GabanaTextImg = new UIImageView();
            GabanaTextImg.Image = UIImage.FromFile("GabanaTxt.png");
            GabanaTextImg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(GabanaTextImg);

            #region Menu
            btnGoLogin = new UIButton();
            btnGoLogin.SetTitle(Utils.TextBundle("login", "Log In"), UIControlState.Normal);
            btnGoLogin.Layer.CornerRadius = 20f;
            btnGoLogin.BackgroundColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1);
            btnGoLogin.TranslatesAutoresizingMaskIntoConstraints = false;
            btnGoLogin.TouchUpInside += (sender, e) => {
                UIView.Animate(0.7, () =>
                {
                    btnGoLogin_TouchUpInside();
                    View.LayoutIfNeeded();
                });
            };
            View.AddSubview(btnGoLogin);

            btnGoRegister = new UIButton();
            btnGoRegister.SetTitle(Utils.TextBundle("createaccount", "Create Account"), UIControlState.Normal);
            btnGoRegister.BackgroundColor = new UIColor(red: 255 / 255f, green: 255 / 255f, blue: 255 / 255f, alpha: 1);
            btnGoRegister.Layer.BorderWidth = 0.5f;
            btnGoRegister.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
            btnGoRegister.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
            btnGoRegister.Layer.CornerRadius = 20f;
            btnGoRegister.TranslatesAutoresizingMaskIntoConstraints = false;
            btnGoRegister.TouchUpInside += (sender, e) => {
                UIView.Animate(0.7, () =>
                {
                    btnGoRegister_TouchUpInside();
                    View.LayoutIfNeeded();
                });

            };
            View.AddSubview(btnGoRegister);
            #endregion
            #region Login

            btnowner = new UIButton();
            btnowner.BackgroundColor = UIColor.White;
            btnowner.SetTitle(Utils.TextBundle("owner", "Owner"), UIControlState.Normal);
            btnowner.Font = btnowner.Font.WithSize(14);
            btnowner.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
            btnowner.TranslatesAutoresizingMaskIntoConstraints = false;
            btnowner.TouchUpInside += (sender, e) => {
                btnowner.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
                btnExpire.SetTitleColor(UIColor.FromRGB(162, 162, 162), UIControlState.Normal);
                btnLoginOwner.Hidden = false;
                txtMerID.Text = null;
                txtUsername.Text = null;
                txtPassword.Text = null;
                txtMerID.Hidden = true;
                txtPassword.Hidden = true;
                txtUsername.Hidden = true;
                btnLoginEmp.Hidden = true;
                telImg.Hidden = false;
                passImg.Hidden = true;
                userImg.Hidden = true;
                merchantImg.Hidden = true;
                txtMobile.Hidden = false;
                buttonBar.Hidden = false;
                buttonBar2.Hidden = true;
                //change to history
            };
            View.AddSubview(btnowner);

            btnExpire = new UIButton();
            btnExpire.BackgroundColor = UIColor.White;
            btnExpire.SetTitle(Utils.TextBundle("employee", "Employee"), UIControlState.Normal);
            btnExpire.Font = btnExpire.Font.WithSize(14);
            btnExpire.SetTitleColor(UIColor.FromRGB(162, 162, 162), UIControlState.Normal);
            btnExpire.TranslatesAutoresizingMaskIntoConstraints = false;
            btnExpire.TouchUpInside += (sender, e) => {
                btnowner.SetTitleColor(UIColor.FromRGB(162, 162, 162), UIControlState.Normal);
                btnExpire.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
                telImg.Hidden = true;
                passImg.Hidden = false;
                userImg.Hidden = false;
                merchantImg.Hidden = false;
                btnLoginEmp.Hidden = false;
                btnLoginOwner.Hidden = true;
                txtMerID.Text = null;
                txtUsername.Text = null;
                txtPassword.Text = null;
                txtMerID.Hidden = false;
                txtMobile.Hidden = true;
                txtMobile.Text = null;
                txtPassword.Hidden = false;
                txtUsername.Hidden = false;
                buttonBar.Hidden = true;
                buttonBar2.Hidden = false;
                //change to Expiry
            };
            View.AddSubview(btnExpire);
            btnowner.Hidden = true;
            btnExpire.Hidden = true;
            buttonBar = new UIView();
            // This needs to be false since we are using auto layout constraints
            buttonBar.TranslatesAutoresizingMaskIntoConstraints = false;
            buttonBar.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            buttonBar.Hidden = true;
            View.AddSubview(buttonBar);

            buttonBar2 = new UIView();
            // This needs to be false since we are using auto layout constraints
            buttonBar2.TranslatesAutoresizingMaskIntoConstraints = false;
            buttonBar2.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            buttonBar2.Hidden = true;
            View.AddSubview(buttonBar2);


            //loginSegment.ValueChanged += async (sender, e) =>
            //{
            //    var selectedSegmentId = (sender as UISegmentedControl).SelectedSegment;



            #region Owner Login
            btnLoginOwner = new UIButton();
            btnLoginOwner.SetTitle(Utils.TextBundle("login", "Log In"), UIControlState.Normal);
            btnLoginOwner.Layer.CornerRadius = 20f;
            btnLoginOwner.BackgroundColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1);
            btnLoginOwner.TranslatesAutoresizingMaskIntoConstraints = false;
            btnLoginOwner.Hidden = true;
            btnLoginOwner.TouchUpInside += (sender, e) => {
                btnLoginOwner_TouchUpInside();

            };
            View.AddSubview(btnLoginOwner);
            txtMobile = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                Placeholder = Utils.TextBundle("mobilephone", "Mobile Phone"),
                BorderStyle = UITextBorderStyle.None,
                BackgroundColor = UIColor.White,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
                Hidden = true
            };
            txtMobile.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 10;
            };
            txtMobile.Layer.BorderWidth = 0.5f;
            txtMobile.KeyboardType = UIKeyboardType.NumberPad;
            txtMobile.Layer.CornerRadius = 20;
            txtMobile.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            View.AddSubview(txtMobile);
            #endregion

            #region Employee Login
            btnLoginEmp = new UIButton();
            btnLoginEmp.SetTitle(Utils.TextBundle("login", "Log In"), UIControlState.Normal);
            btnLoginEmp.Layer.CornerRadius = 20f;
            btnLoginEmp.BackgroundColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1);
            btnLoginEmp.TranslatesAutoresizingMaskIntoConstraints = false;
            btnLoginEmp.Hidden = true;
            btnLoginEmp.TouchUpInside += (sender, e) => {
                btnLoginEmp_TouchUpInside();
            };
            View.AddSubview(btnLoginEmp);

            txtMerID = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                Placeholder = Utils.TextBundle("merchantid", "Merchant ID"),
                BorderStyle = UITextBorderStyle.None,
                BackgroundColor = UIColor.White,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
                Hidden = true
            };
            txtMerID.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 8;
            };
            txtMerID.ReturnKeyType = UIReturnKeyType.Next;
            txtMerID.ShouldReturn = (tf) =>
            {
                txtUsername.BecomeFirstResponder();
                return true;
            };
            txtMerID.KeyboardType = UIKeyboardType.NumberPad;
            txtMerID.Layer.BorderWidth = 0.5f;
            txtMerID.Layer.CornerRadius = 20;
            txtMerID.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            View.AddSubview(txtMerID);

            MerImg = new UIImageView();
            //telImg.Image = UIImage.FromFile("Tel.png");
            MerImg.Image = UIImage.FromBundle("MerchantId");
            MerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            MerImg.Hidden = true;
            MerImg.Layer.ZPosition = 1;
            txtMerID.AddSubview(MerImg);


            txtUsername = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                Placeholder = Utils.TextBundle("username", "Username"),
                BorderStyle = UITextBorderStyle.None,
                BackgroundColor = UIColor.White,
                AutocapitalizationType = UITextAutocapitalizationType.None,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
                Hidden = true
            };
            txtUsername.ReturnKeyType = UIReturnKeyType.Next;
            txtUsername.ShouldReturn = (tf) =>
            {
                txtPassword.BecomeFirstResponder();
                return true;
            };
            txtUsername.Layer.BorderWidth = 0.5f;
            txtUsername.Layer.CornerRadius = 20;
            txtUsername.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            View.AddSubview(txtUsername);

            userImg = new UIImageView();
            //telImg.Image = UIImage.FromFile("Tel.png");
            userImg.Image = UIImage.FromBundle("Username");
            userImg.TranslatesAutoresizingMaskIntoConstraints = false;
            userImg.Hidden = true;
            userImg.Layer.ZPosition = 1;
            txtUsername.AddSubview(userImg);

            txtPassword = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                Placeholder = Utils.TextBundle("password", "Password"),
                BorderStyle = UITextBorderStyle.None,
                BackgroundColor = UIColor.White,
                SecureTextEntry = true,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
                Hidden = true,
                AutocapitalizationType = UITextAutocapitalizationType.None
            };
            txtPassword.ReturnKeyType = UIReturnKeyType.Done;
            txtPassword.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtPassword.Layer.BorderWidth = 0.5f;
            txtPassword.Layer.CornerRadius = 20;
            txtPassword.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            // txtPassword.ReturnKeyType = UIReturnKeyType.Done;
            View.AddSubview(txtPassword);

            passImg = new UIImageView();
            //telImg.Image = UIImage.FromFile("Tel.png");
            passImg.Image = UIImage.FromBundle("Password");
            passImg.TranslatesAutoresizingMaskIntoConstraints = false;
            passImg.Hidden = true;
            passImg.Layer.ZPosition = 1;
            txtPassword.AddSubview(passImg);
            #endregion

            #endregion
            #region Register
            btnRegister = new UIButton();
            btnRegister.SetTitle(Utils.TextBundle("createaccount", "Create Account"), UIControlState.Normal);
            btnRegister.BackgroundColor = new UIColor(red: 255 / 255f, green: 255 / 255f, blue: 255 / 255f, alpha: 1);
            btnRegister.Layer.BorderWidth = 0.5f;
            btnRegister.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
            btnRegister.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
            btnRegister.Layer.CornerRadius = 20f;
            btnRegister.TranslatesAutoresizingMaskIntoConstraints = false;
            btnRegister.Hidden = true;
            btnRegister.TouchUpInside += (sender, e) =>
            {
                btnRegister_TouchUpInside();
            };
            View.AddSubview(btnRegister);
            #endregion                                                                                                         
            #region Icon Image
            telImg = new UIImageView();
            //telImg.Image = UIImage.FromFile("Tel.png");
            telImg.Image = UIImage.FromBundle("Tel");
            telImg.TranslatesAutoresizingMaskIntoConstraints = false;
            telImg.Hidden = true;
            telImg.Layer.ZPosition = 1;
            txtMobile.AddSubview(telImg);

            //merchantImg,usernameImg,passwordImg
            merchantImg = new UIImageView();
            //merchantImg.Image = UIImage.FromFile("MerchantId.png");
            merchantImg.Image = UIImage.FromBundle("MerchantId");
            merchantImg.TranslatesAutoresizingMaskIntoConstraints = false;
            merchantImg.Hidden = true;
            merchantImg.Layer.ZPosition = 1;
            txtMerID.AddSubview(merchantImg);

            usernameImg = new UIImageView();
            //usernameImg.Image = UIImage.FromFile("Username.png");
            usernameImg.Image = UIImage.FromBundle("Username");
            usernameImg.TranslatesAutoresizingMaskIntoConstraints = false;
            usernameImg.Hidden = true;
            usernameImg.Layer.ZPosition = 1;
            txtUsername.AddSubview(usernameImg);

            passwordImg = new UIImageView();
            //passwordImg.Image = UIImage.FromFile("Password.png");
            passwordImg.Image = UIImage.FromBundle("Password");
            passwordImg.TranslatesAutoresizingMaskIntoConstraints = false;
            passwordImg.Hidden = true;
            passwordImg.Layer.ZPosition = 1;
            txtPassword.AddSubview(passwordImg);
            #endregion

            btnBack = new UIButton();
            btnBack.SetBackgroundImage(UIImage.FromFile("BackB.png"), UIControlState.Normal);
            btnBack.TranslatesAutoresizingMaskIntoConstraints = false;
            btnBack.Hidden = true;
            btnBack.TouchUpInside += (sender, e) =>
            {



                UIView.Animate(0.7, () =>
                {
                    btnBack_TouchUpInside();
                    View.LayoutIfNeeded();
                });
            };
            View.AddSubview(btnBack);

            powerbySNS = new UIImageView();
            powerbySNS.Image = UIImage.FromBundle("PoweredBySNS");
            powerbySNS.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(powerbySNS);

            lblversion = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblversion.TextColor = UIColor.FromRGB(64, 64, 64);
            lblversion.Font = lblversion.Font.WithSize(12);
            lblversion.TextAlignment = UITextAlignment.Center;
            View.AddSubview(lblversion);
            var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
            var version2 = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
            lblversion.Text =Utils.TextBundle("version", "Version ") + version + "." + version2;

            View.AddSubview(lblversion);

            setupAutoLayout();
            framelogo = logoImg.Frame;

            Setkeyboard();
             
        }
        private void Setkeyboard()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            void OnKeyboardNotification(NSNotification notification)
            {
                if (!IsViewLoaded) return;
                

                    //Check if the keyboard is becoming visible
                    var visible = notification.Name == UIKeyboard.WillShowNotification;

                    //Start an animation, using values from the keyboard
                    //UIView.BeginAnimations("AnimateForKeyboard");
                    //UIView.SetAnimationBeginsFromCurrentState(true);
                    UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
                    UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

                    //Pass the notification, calculating keyboard height, etc.
                    bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
                    var keyboardFrame = visible
                                            ? UIKeyboard.FrameEndFromNotification(notification)
                                            : UIKeyboard.FrameBeginFromNotification(notification);

                    OnKeyboardChanged(View, visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
                
                //Commit the animation
                //UIView.CommitAnimations();
            }
        }
        public  void OnKeyboardChanged(UIView view, bool visible, nfloat nfloat)
        {
            if (!visible)
                view.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);
            else

                view.Frame = new CGRect(0, 0 - 100, view.Frame.Width, view.Frame.Height);
        }
        void setupAutoLayout()
        {
            btnLoginOwner.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnLoginOwner.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 480).Active = true;
            btnLoginOwner.WidthAnchor.ConstraintEqualTo(265).Active = true;
            btnLoginOwner.HeightAnchor.ConstraintEqualTo(40).Active = true;

            btnLoginEmp.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnLoginEmp.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 480).Active = true;
            btnLoginEmp.WidthAnchor.ConstraintEqualTo(265).Active = true;
            btnLoginEmp.HeightAnchor.ConstraintEqualTo(40).Active = true;

            logoImg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            logoImg.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 72).Active = true;
            logoImg.WidthAnchor.ConstraintEqualTo(160).Active = true;
            logoImg.HeightAnchor.ConstraintEqualTo(160).Active = true;


            GabanaTextImg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            GabanaTextImg.TopAnchor.ConstraintEqualTo(logoImg.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            GabanaTextImg.WidthAnchor.ConstraintEqualTo(160).Active = true;
            GabanaTextImg.HeightAnchor.ConstraintEqualTo(60).Active = true;


            btnowner.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, -66).Active = true;
            btnowner.TopAnchor.ConstraintEqualTo(GabanaTextImg.SafeAreaLayoutGuide.BottomAnchor, 32).Active = true;
            btnowner.WidthAnchor.ConstraintEqualTo(132).Active = true;
            btnowner.HeightAnchor.ConstraintEqualTo(45).Active = true;



            btnExpire.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 66).Active = true;
            btnExpire.TopAnchor.ConstraintEqualTo(GabanaTextImg.SafeAreaLayoutGuide.BottomAnchor, 32).Active = true;
            btnExpire.WidthAnchor.ConstraintEqualTo(133).Active = true;
            btnExpire.HeightAnchor.ConstraintEqualTo(45).Active = true;

            buttonBar.TopAnchor.ConstraintEqualTo(btnowner.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            buttonBar.HeightAnchor.ConstraintEqualTo(2.5f).Active = true;

            //Constrain the button bar to the left side of the segmented control
            buttonBar.LeftAnchor.ConstraintEqualTo(btnowner.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            //Constrain the button bar to the width of the segmented control divided by the number of segments
            buttonBar.WidthAnchor.ConstraintEqualTo(133).Active = true;


            buttonBar2.TopAnchor.ConstraintEqualTo(btnExpire.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            buttonBar2.HeightAnchor.ConstraintEqualTo(2.5f).Active = true;

            //Constrain the button bar to the left side of the segmented control
            buttonBar2.LeftAnchor.ConstraintEqualTo(btnExpire.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            //Constrain the button bar to the width of the segmented control divided by the number of segments
            buttonBar2.WidthAnchor.ConstraintEqualTo(133).Active = true;


            telImg.LeftAnchor.ConstraintEqualTo(txtMobile.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            telImg.CenterYAnchor.ConstraintEqualTo(txtMobile.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            telImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            telImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            MerImg.LeftAnchor.ConstraintEqualTo(txtMerID.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            MerImg.CenterYAnchor.ConstraintEqualTo(txtMerID.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            MerImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            MerImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            userImg.LeftAnchor.ConstraintEqualTo(txtUsername.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            userImg.CenterYAnchor.ConstraintEqualTo(txtUsername.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            userImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            userImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            passImg.LeftAnchor.ConstraintEqualTo(txtPassword.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            passImg.CenterYAnchor.ConstraintEqualTo(txtPassword.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            passImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            passImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            merchantImg.LeftAnchor.ConstraintEqualTo(txtMerID.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            merchantImg.CenterYAnchor.ConstraintEqualTo(txtMerID.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            merchantImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            merchantImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            usernameImg.LeftAnchor.ConstraintEqualTo(txtUsername.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            usernameImg.CenterYAnchor.ConstraintEqualTo(txtUsername.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            usernameImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            usernameImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            passwordImg.LeftAnchor.ConstraintEqualTo(txtPassword.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            passwordImg.CenterYAnchor.ConstraintEqualTo(txtPassword.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            passwordImg.WidthAnchor.ConstraintEqualTo(18).Active = true;
            passwordImg.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnBack.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 14).Active = true;
            btnBack.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 21).Active = true;
            btnBack.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnBack.HeightAnchor.ConstraintEqualTo(28).Active = true;

            btnGoLogin.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnGoLogin.TopAnchor.ConstraintEqualTo(GabanaTextImg.SafeAreaLayoutGuide.BottomAnchor, 34).Active = true;
            btnGoLogin.WidthAnchor.ConstraintEqualTo(265).Active = true;
            btnGoLogin.HeightAnchor.ConstraintEqualTo(40).Active = true;

            btnGoRegister.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnGoRegister.TopAnchor.ConstraintEqualTo(btnGoLogin.SafeAreaLayoutGuide.BottomAnchor, 40).Active = true;
            btnGoRegister.WidthAnchor.ConstraintEqualTo(265).Active = true;
            btnGoRegister.HeightAnchor.ConstraintEqualTo(40).Active = true;


            txtMobile.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            txtMobile.TopAnchor.ConstraintEqualTo(btnowner.SafeAreaLayoutGuide.BottomAnchor, 58).Active = true;

            txtMobile.WidthAnchor.ConstraintEqualTo(265).Active = true;
            txtMobile.HeightAnchor.ConstraintEqualTo(40).Active = true;

            btnRegister.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnRegister.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 450).Active = true;
            btnRegister.WidthAnchor.ConstraintEqualTo(265).Active = true;
            btnRegister.HeightAnchor.ConstraintEqualTo(40).Active = true;

            txtMerID.SendSubviewToBack(View);
            txtMerID.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            txtMerID.TopAnchor.ConstraintEqualTo(btnowner.SafeAreaLayoutGuide.BottomAnchor, 20).Active = true;
            txtMerID.WidthAnchor.ConstraintEqualTo(265).Active = true;
            txtMerID.HeightAnchor.ConstraintEqualTo(40).Active = true;

            txtUsername.SendSubviewToBack(View);
            txtUsername.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            txtUsername.TopAnchor.ConstraintEqualTo(txtMerID.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            txtUsername.WidthAnchor.ConstraintEqualTo(265).Active = true;
            txtUsername.HeightAnchor.ConstraintEqualTo(40).Active = true;

            txtPassword.SendSubviewToBack(View);
            txtPassword.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            txtPassword.TopAnchor.ConstraintEqualTo(txtUsername.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            txtPassword.WidthAnchor.ConstraintEqualTo(265).Active = true;
            txtPassword.HeightAnchor.ConstraintEqualTo(40).Active = true;

            powerbySNS.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            powerbySNS.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -32).Active = true;
            powerbySNS.WidthAnchor.ConstraintEqualTo(175).Active = true;
            powerbySNS.HeightAnchor.ConstraintEqualTo(25).Active = true;

            lblversion.TopAnchor.ConstraintEqualTo(powerbySNS.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lblversion.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            lblversion.HeightAnchor.ConstraintEqualTo(10).Active = true;
            lblversion.WidthAnchor.ConstraintEqualTo(240).Active = true;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        void btnGoRegister_TouchUpInside()
        {

            Utils.SetConstant(logoImg.Constraints, NSLayoutAttribute.Width, 140);
            Utils.SetConstant(logoImg.Constraints, NSLayoutAttribute.Height, 140);
            Utils.SetConstant(GabanaTextImg.Constraints, NSLayoutAttribute.Width, 140);
            Utils.SetConstant(GabanaTextImg.Constraints, NSLayoutAttribute.Height, 53);
            Utils.SetConstant(btnowner.Constraints, NSLayoutAttribute.Height, 0);


            btnBack.Hidden = false;
            txtMobile.Hidden = false;
            btnRegister.Hidden = false;
            btnGoRegister.Hidden = true;
            btnGoLogin.Hidden = true;
            telImg.Hidden = false;
            passImg.Hidden = true;
            userImg.Hidden = true;
            merchantImg.Hidden = true;
        }
        async void btnRegister_TouchUpInside()
        {
            try
            {
                btnRegister.Enabled = false;
                GabanaLoading.SharedInstance.Show(this.NavigationController);
                if (txtMobile.Text.Length != 10)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("entermoblie", "entermoblie"), "");
                    return;
                }
                else
                {
                    // startactivity otp
                    //SeAuth2API CheckSingUpOrLoginGabana เช็คว่ามี merchantId หรือไม่?
                    var CheckSingUpOrLogin = await GabanaAPI.CheckSingUpOrLoginGabana(txtMobile.Text);
                    if (!CheckSingUpOrLogin.Status)
                    {
                        //if (CheckSingUpOrLogin.Message.ToLower() == "un" )
                        //{
                        //    Utils.TextBundle("un","");
                        //}

                        //InitialGabana
                        VerifyOTP verify = await Sentotp(txtMobile.Text, "regis");
                        if (verify.OwnerID == null)
                        {
                            string exception = verify.RefOTP;
                            switch (exception)
                            {
                                case "NotFound Tel Please contract Admin Invalid ReSendCode. ( ไม่พบเบอร์โทรนี้ในระบบ โปรดขอ OTP ใหม่ )":
                                case "Login Error Please contract Admin Invalid ReSendCode. ( OTP หมดอายุ โปรดขอ OTP ใหม่ )":
                                case "Invalid User, password โปรดลองใหม่":
                                case "Login Error Please contract Admin ReSendCode more time":
                                case "Login Error Please contract Admin 'Login Over time'":
                                    Utils.ShowMessage(exception);

                                    break;
                                default:
                                    Utils.ShowMessage(exception);

                                    break;
                            }
                            return;
                        }
                        //refstring คือ RefOTP
                        if (verify.OwnerID != null)
                        {


                            var refstring = verify.RefOTP.ToString();
                            LoginOTPController loginOTP = new LoginOTPController(verify, "register");
                            DataCaching.LoginNavigation.PushViewController(loginOTP, false);
                        }
                        else
                        {
                            string exception = verify.RefOTP;
                            Utils.ShowAlert(this, "exception", "");
                        }
                    }
                    else
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("samephone", "samephone"), "");
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                btnRegister.Enabled = true ;
                GabanaLoading.SharedInstance.Hide();
            }
        }

        private void OpenMainPage()
        {
            var gbnJWT = Preferences.Get("gbnJWT", "");
            Preferences.Set("LoginType", "owner");
            GabanaAPI.gbnJWT = gbnJWT;
            Preferences.Set("AppState", "logon");
            DataCaching.LoginNavigation.DismissViewController(false, null);
        }

        async void btnLoginEmp_TouchUpInside()
        {
            InvokeOnMainThread(() =>
            {
                GabanaLoading.SharedInstance.Show(this);
            });
            try
            {

                if (await GabanaAPI.CheckNetWork())
                {
                    var emp = new LoginEmp() { MerchantID = txtMerID.Text, Username = txtUsername.Text, Password = txtPassword.Text };
                    GabanaAPI.gbnJWT = await GetToken.GetgbnJWTForEmp(emp);
                    if (GabanaAPI.gbnJWT == null)
                    {
                        Utils.ShowMessage(Utils.TextBundle("entertrue", "กรุณากรอกข้อมูลให้ถูกต้อง"));
                        //Toast.MakeText(Application.Context, "กรุณากรอกข้อมูลให้ถูกต้อง", ToastLength.Short).Show();
                        btnLoginEmp.Enabled = true;
                        return;
                    }

                    //Get Merchant Detail from API
                    merchants = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                    if (merchants == null)
                    {
                        btnLoginEmp.Enabled = true;
                        //fragment.Dismiss();
                        return;
                    }

                    DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                    if (DataCashingAll.UserAccountInfo == null)
                    {
                        DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                    }

                    string LoginType = "employee";
                    string username = string.Empty;
                    var data = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == emp.Username.ToLower()).FirstOrDefault();
                    if (data != null)
                    {
                        LoginType = data.MainRoles;
                        username = data.UserName;
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("connectadmin", "OK"));
                        //Toast.MakeText(this, "กรุณาติดต่อผู้ดูแลระบบ", ToastLength.Short).Show();
                        return;
                    }
                    Preferences.Set("CreateDB", "");
                    Preferences.Set("ViewPos", "Grid");
                    Preferences.Set("AppState", "login");
                    Preferences.Set("LoginType", LoginType);
                    Preferences.Set("User", username);
                    DataCaching.LoginNavigation.DismissViewController(false, null);
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("connectnet", "OK"), "");
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);

                string ShowError = UtilsAll.CheckErrorGetToken(ex.Message);
                if (ShowError == "invalid_grant: invalid_username_or_password" || ShowError == "UserPassIncorrect")
                {
                    Utils.ShowMessage(Utils.TextBundle("dataincorrect", ""));
                }
                else if (ShowError == "invalid_grant: User Can't Access CloudProduct" || ShowError == "UserCanNotAccessCloudProduct")
                {
                    Utils.ShowMessage(Utils.TextBundle("permissionincorrect", ""));
                }
                //else if (ShowError == "CloudProductExpired" || ShowError == "invalid_grant: Cloud Product Expired")
                //{
                //    // แสดง dialog expire ของ employee  -> ไปหน้า login

                //}
                else
                {
                    Utils.ShowMessage(ShowError);

                }
                return;

                //Utils.ShowAlert(this, Utils.TextBundle("cannotlogin", "OK"), ex.Message);
            }
            finally 
            {
                InvokeOnMainThread(() =>
                {
                    GabanaLoading.SharedInstance.Hide();
                });
            }


        }
        async void btnLoginOwner_TouchUpInside()
        {
            InvokeOnMainThread(() =>
            {
                GabanaLoading.SharedInstance.Show(this);
            });
            try
            {

                
                if (await GabanaAPI.CheckNetWork())
                {
                    btnLoginOwner.Enabled = false;
                    GabanaLoading.SharedInstance.Show(this.NavigationController);
                    if (txtMobile.Text.Length != 10)
                    {
                        Utils.ShowAlert(this, "Error !", Utils.TextBundle("entermoblie", "entermoblie"));
                        btnLoginOwner.Enabled = true;
                        GabanaLoading.SharedInstance.Hide();
                        return;
                    }
                    else
                    {
                        // startactivity otp
                        //SeAuth2API CheckSingUpOrLoginGabana เช็คว่ามี merchantId หรือไม่?
                        var CheckSingUpOrLogin = await GabanaAPI.CheckSingUpOrLoginGabana(txtMobile.Text);
                        if (CheckSingUpOrLogin.Status)
                        {
                            int.TryParse(CheckSingUpOrLogin.Message, out int result);
                            DataCashingAll.MerchantId = Convert.ToInt32(result);

                            //if (txtMobile.Text == Preferences.Get("PHONE", "") && Preferences.Get("PHONE", "") != "" && Preferences.Get("LoginType", "") == "owner")
                            //{
                            //    OpenMainPage();
                            //}
                            //else
                            //{

                                //InitialGabana
                                VerifyOTP verify = await Sentotp(txtMobile.Text, "login");
                                if (verify.OwnerID == null)
                                {
                                    string exception = verify.RefOTP;
                                    switch (exception)
                                    {
                                        
                                        case "invalid_grant: Cloud Product Expired":
                                            Utils.ShowMessage(exception);
                                            OpenRenew();
                                            break;
                                        default:
                                            Utils.ShowMessage(exception);
                                            
                                            break;
                                    }
                                    btnLoginOwner.Enabled = true;
                                    GabanaLoading.SharedInstance.Hide();
                                    return;
                                }
                                //refstring คือ RefOTP
                                if (verify.OwnerID != null)
                                {

                                    //GiftLoading.SharedInstance.Show(NavigationController);
                                    string emailstring;
                                    if (txtMobile.Text != "")
                                    {
                                        var refstring = verify.RefOTP.ToString();
                                        LoginOTPController loginOTP = new LoginOTPController(verify, "login");
                                        DataCaching.LoginNavigation.PushViewController(loginOTP, false);
                                    }
                                    else
                                    {
                                        Utils.ShowAlert(this, "", Utils.TextBundle("enterphone", "OK"));
                                    }


                                    //var refstring = verify.RefOTP.ToString();

                                    //    LoginOTPController loginOTP = new LoginOTPController(verify,"login");
                                    //    DataCaching.LoginNavigation.PushViewController(loginOTP, false);

                                }
                                else
                                {
                                    string exception = verify.RefOTP;
                                    Utils.ShowAlert(this, "Error !", exception);
                                }
                            //}
                        }
                        else
                        {
                            Utils.ShowAlert(this, Utils.TextBundle("regisbefore", "regisbefore"), "");
                        }
                    }
                    GabanaLoading.SharedInstance.Hide();
                    btnLoginOwner.Enabled = true;
                }
                else
                {
                    Utils.ShowAlert(this,"", Utils.TextBundle("connectnet", "connectnet"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                btnLoginOwner.Enabled = true;
            }
            finally
            {
                InvokeOnMainThread(() =>
                {
                    GabanaLoading.SharedInstance.Hide();
                });
            }
        }

        private void OpenRenew()
        {
            PackageController Edititem = new PackageController();
            this.NavigationController.PushViewController(Edititem, false);
        }

        private async Task<VerifyOTP> Sentotp(string text, string type)
        {
            try
            {
                //get UDID 
                if (type == "login")
                {
                    string Id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                    var platform = "APNS";
                    var verify = new SendOTP() { OwnerID = text, UDID = Id };
                    var refstring = await GabanaAPI.urlInitialLoginGabana(verify);
                    VerifyOTP verifyOTP = new VerifyOTP() { OwnerID = text, RefOTP = refstring.Message };
                    return verifyOTP;
                }
                else
                {
                    string Id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                    var platform = "APNS";
                    var verify = new SendOTP() { OwnerID = text, UDID = Id };
                    var refstring = await GabanaAPI.urlInitialCreateGabana(verify);
                    VerifyOTP verifyOTP = new VerifyOTP() { OwnerID = text, RefOTP = refstring.Message };
                    return verifyOTP;
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                VerifyOTP verify = new VerifyOTP();
                verify.RefOTP = ex.Message.ToString();
                return verify;
            }

            
        }
        void btnBack_TouchUpInside()
        {


            //  View.EndEditing(true);
            btnLoginEmp.Hidden = true;
            txtMobile.Hidden = true;
            btnRegister.Hidden = true;
            btnGoRegister.Hidden = false;
            btnGoLogin.Hidden = false;
            telImg.Hidden = true;
            passImg.Hidden = true;
            userImg.Hidden = true;
            merchantImg.Hidden = true;
            txtMobile.Text = null;
            txtMerID.Text = null;
            txtUsername.Text = null;
            txtPassword.Text = null;
            txtMerID.Hidden = true;
            txtPassword.Hidden = true;
            txtUsername.Hidden = true;
            btnBack.Hidden = true;
            btnLoginOwner.Hidden = true;
            btnowner.Hidden = true;
            btnExpire.Hidden = true;
            buttonBar.Hidden = true;
            buttonBar2.Hidden = true;

            Utils.SetConstant(logoImg.Constraints, NSLayoutAttribute.Width, 160);
            Utils.SetConstant(logoImg.Constraints, NSLayoutAttribute.Height, 160);
            Utils.SetConstant(GabanaTextImg.Constraints, NSLayoutAttribute.Width, 160);
            Utils.SetConstant(GabanaTextImg.Constraints, NSLayoutAttribute.Height, 60);
            Utils.SetConstant(btnowner.Constraints, NSLayoutAttribute.Height, 45);
            setupAutoLayout();



        }
        void btnGoLogin_TouchUpInside()
        {


            Utils.SetConstant(logoImg.Constraints, NSLayoutAttribute.Width, 100);
            Utils.SetConstant(logoImg.Constraints, NSLayoutAttribute.Height, 100);
            Utils.SetConstant(GabanaTextImg.Constraints, NSLayoutAttribute.Width, 100);
            Utils.SetConstant(GabanaTextImg.Constraints, NSLayoutAttribute.Height, 38);

            btnowner.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);

            buttonBar.Hidden = false;
            btnowner.Hidden = false;
            btnExpire.Hidden = false;
            txtMobile.Hidden = false;
            btnRegister.Hidden = true;
            btnGoRegister.Hidden = true;
            btnGoLogin.Hidden = true;
            telImg.Hidden = false;
            passImg.Hidden = true;
            userImg.Hidden = true;
            merchantImg.Hidden = true;
            btnBack.Hidden = false;
            btnLoginOwner.Hidden = false;




        }
    }
    class SquareSegmentedControl : UISegmentedControl
    {
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.Layer.CornerRadius = 0;
        }
    }
}