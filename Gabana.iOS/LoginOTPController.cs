using Foundation;
using Gabana.AppSetting;
using Gabana.Controller;
using Gabana.Model;
using Gabana.ShareSource;
using GlobalToast;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{

    public partial class LoginOTPController : UIViewController
    {

        UIImageView otpImg;
        MainController mainController;
        UILabel lblVertify;
        UITextField otp1,otp2, otp3, otp4, otp5, otp6, newotp1;
        public VerifyOTP verifyOTP;
        string type;
        private UILabel lblerror;

        public LoginOTPController(VerifyOTP verify,string type)
        {
            verifyOTP = verify;
            this.type = type; 
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.Translucent = true;

            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
            this.NavigationController.NavigationBar.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            this.NavigationController.NavigationBar.TopItem.Title = "OTP";
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            //Utils.SetTitle(this.NavigationController, "Choose Branch");

            View.BackgroundColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White
                //BackgroundColor = UIColor.FromRGB(51, 170, 225)
            };

            //this.NavigationController.NavigationBar.BarTintColor =  new UIColor(51 / 255f, 172 / 255f, 225 / 255f, 1); ;
            //this.NavigationController.NavigationBar.TopItem.Title = "OTP";
            //this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidLoad()
        {
            this.NavigationController.NavigationBar.BarTintColor = new UIColor(51 / 255f, 172 / 255f, 225 / 255f, 1); ;
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            #region LayoutAttribute

            otpImg = new UIImageView();
            otpImg.Image = UIImage.FromBundle("OTP");
            otpImg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(otpImg);

            lblVertify = new UILabel();
            lblVertify.Text = Utils.TextBundle("sentotp", "Verify Your Mobile Number\nAn 6-digit OTP has been sent to\n");
            lblVertify.Font = lblVertify.Font.WithSize(15);
            lblVertify.TextColor = UIColor.Black;
            lblVertify.Lines = 3;
            lblVertify.TextAlignment = UITextAlignment.Center;
            lblVertify.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblVertify);

            lblerror = new UILabel();
            lblerror.Text = "";
            lblerror.Font = lblerror.Font.WithSize(15);
            lblerror.TextColor = UIColor.Red;
            lblerror.Lines = 4;
            lblerror.TextAlignment = UITextAlignment.Center;
            lblerror.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblerror);
            #endregion

            #region OTPAttribute
            var tapGesture1 = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("Edit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            var tapGesture2 = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("Edit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            var tapGesture3 = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("Edit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            var tapGesture4 = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("Edit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            var tapGesture5 = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("Edit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            var tapGesture6 = new UITapGestureRecognizer(this, new ObjCRuntime.Selector("Edit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };

            otp1 = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                BorderStyle = UITextBorderStyle.None,
                //TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            otp1.Layer.BorderWidth = 0.5f;
            otp1.KeyboardType = UIKeyboardType.NumberPad;
            otp1.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            otp1.TextColor = UIColor.FromRGB(0, 149, 218); 
            otp1.UserInteractionEnabled = true;
            otp1.AddGestureRecognizer(tapGesture1);
            View.AddSubview(otp1);

            otp2 = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                BorderStyle = UITextBorderStyle.None,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            otp2.Layer.BorderWidth = 0.5f;
            otp2.KeyboardType = UIKeyboardType.NumberPad;
            otp2.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            otp2.UserInteractionEnabled = true;
            otp2.TextColor = UIColor.FromRGB(0, 149, 218);
            otp2.AddGestureRecognizer(tapGesture2);
            View.AddSubview(otp2);

            otp3 = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                BorderStyle = UITextBorderStyle.None,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            otp3.Layer.BorderWidth = 0.5f;
            otp3.KeyboardType = UIKeyboardType.NumberPad;
            otp3.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            otp3.UserInteractionEnabled = true;
            otp3.TextColor = UIColor.FromRGB(0, 149, 218);
            otp3.AddGestureRecognizer(tapGesture3);
            View.AddSubview(otp3);

            otp4 = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                BorderStyle = UITextBorderStyle.None,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            otp4.Layer.BorderWidth = 0.5f;
            otp4.KeyboardType = UIKeyboardType.NumberPad;
            otp4.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            otp4.UserInteractionEnabled = true;
            otp4.TextColor = UIColor.FromRGB(0, 149, 218);
            otp4.AddGestureRecognizer(tapGesture4);
            View.AddSubview(otp4);

            otp5 = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                BorderStyle = UITextBorderStyle.None,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            otp5.Layer.BorderWidth = 0.5f;
            otp5.KeyboardType = UIKeyboardType.NumberPad;
            otp5.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            otp5.UserInteractionEnabled = true;
            otp5.TextColor = UIColor.FromRGB(0, 149, 218);
            otp5.AddGestureRecognizer(tapGesture5);
            View.AddSubview(otp5);

            otp6 = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                BorderStyle = UITextBorderStyle.None,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            otp6.Layer.BorderWidth = 0.5f;
            otp6.KeyboardType = UIKeyboardType.NumberPad;
            otp6.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            otp6.UserInteractionEnabled = true;
            otp6.TextColor = UIColor.FromRGB(0, 149, 218); 
            otp6.AddGestureRecognizer(tapGesture6);
            View.AddSubview(otp6);

            newotp1 = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                BorderStyle = UITextBorderStyle.None,
                TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            newotp1.Layer.BorderWidth = 0.5f;
            newotp1.Hidden = true;
            newotp1.KeyboardType = UIKeyboardType.NumberPad;
            newotp1.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
            newotp1.EditingChanged += async (sender, e) => {
                var otp = newotp1.Text.Trim();
                switch (otp.Length)
                {
                    case 0:
                        otp1.Text = "";
                        otp2.Text = "";
                        otp3.Text = "";
                        otp4.Text = "";
                        otp5.Text = "";
                        otp6.Text = "";
                        otp1.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        otp2.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp3.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp4.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp5.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp6.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        break;
                    case 1:
                        otp1.Text = otp.Substring(0, 1);
                        otp2.Text = "";
                        otp3.Text = "";
                        otp4.Text = "";
                        otp5.Text = "";
                        otp6.Text = "";
                        otp1.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp2.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        otp3.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp4.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp5.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp6.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        break;
                    case 2:
                        otp1.Text = otp.Substring(0, 1);
                        otp2.Text = otp.Substring(1, 1);
                        otp3.Text = "";
                        otp4.Text = "";
                        otp5.Text = "";
                        otp6.Text = "";
                        otp1.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp2.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp3.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        otp4.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp5.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp6.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        break;
                    case 3:
                        otp1.Text = otp.Substring(0, 1);
                        otp2.Text = otp.Substring(1, 1);
                        otp3.Text = otp.Substring(2, 1);
                        otp4.Text = "";
                        otp5.Text = "";
                        otp6.Text = "";
                        otp1.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp2.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp3.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp4.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        otp5.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp6.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        break;
                    case 4:
                        otp1.Text = otp.Substring(0, 1);
                        otp2.Text = otp.Substring(1, 1);
                        otp3.Text = otp.Substring(2, 1);
                        otp4.Text = otp.Substring(3, 1);
                        otp5.Text = "";
                        otp6.Text = "";
                        otp1.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp2.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp3.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp4.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp5.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        otp6.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        break;
                    case 5:
                        otp1.Text = otp.Substring(0, 1);
                        otp2.Text = otp.Substring(1, 1);
                        otp3.Text = otp.Substring(2, 1);
                        otp4.Text = otp.Substring(3, 1);
                        otp5.Text = otp.Substring(4, 1);
                        otp6.Text = "";
                        otp1.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp2.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp3.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp4.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp5.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp6.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        break;
                    case 6:
                        otp1.Text = otp.Substring(0, 1);
                        otp2.Text = otp.Substring(1, 1);
                        otp3.Text = otp.Substring(2, 1);
                        otp4.Text = otp.Substring(3, 1);
                        otp5.Text = otp.Substring(4, 1);
                        otp6.Text = otp.Substring(5, 1);
                        otp1.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        otp2.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp3.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp4.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp5.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        otp6.Layer.BorderColor = new UIColor(red: 202 / 255f, green: 202 / 255f, blue: 202 / 255f, alpha: 1).CGColor;
                        await otpCheckAsync(otp1.Text, otp2.Text, otp3.Text, otp4.Text, otp5.Text, otp6.Text);
                        break;
                    default:
                        break;
                }
            };
            View.AddSubview(newotp1);
            #endregion


            SetupAutoLayout();
            if (verifyOTP.RefOTP.Length > 5)
            {
                lblerror.Text = "Error Code : " + verifyOTP.RefOTP;
            }
            else
            {
                lblVertify.Text = lblVertify.Text + verifyOTP.OwnerID + " Ref = " + verifyOTP.RefOTP;
            }
            
            

        }
        async System.Threading.Tasks.Task otpCheckAsync(string otp1,string otp2,string otp3,string otp4 , string otp5,string otp6)
        {
            try
            {

           
                var opttext = otp1 + otp2 + otp3 + otp4 + otp5 + otp6;
                if (opttext.Length == 6)
                {

                    //if (opttext == "123456")
                    //{
                    // //   this.NavigationController.PopViewController(false);
                    //    MainPageController main = this.Storyboard.InstantiateViewController("MainPageController") as MainPageController;
                    //   this.NavigationController.PushViewController(main, true);
                    //}
                    //else
                    //    return;

                    verifyOTP.OTP = opttext;

                    ResultAPI result; 
                    if (type == "register")
                    {
                        result = await GabanaAPI.CreateGabana(verifyOTP);
                    }
                    else 
                    {
                        result = await GabanaAPI.LoginGabana(verifyOTP);
                    }
                    if (result.Status)
                    {
                        var gbnJWT = await GetToken.GetgbnJWTForOwner(verifyOTP);
                        Preferences.Set("PHONE", verifyOTP.OwnerID);
                        Preferences.Set("gbnJWT", gbnJWT);
                        Preferences.Set("LoginType", "owner");
                        GabanaAPI.gbnJWT = gbnJWT;
                        Preferences.Set("CreateDB", "");
                        Preferences.Set("AppState", "logon");
                        Preferences.Set("User", "Owner");
                        DataCaching.LoginNavigation.DismissViewController(false, null);
                    }
                    else
                    {
                        var resultapi = result.Message;
                        string ShowError = UtilsAll.CheckErrorGetToken(resultapi);
                        if (ShowError == "CloudProductExpired" || ShowError == "invalid_grant: Cloud Product Expired")
                        {
                            PackageController Edititem = new PackageController();
                            this.NavigationController.PushViewController(Edititem, false);
                        }
                        
                            
                           
                        
                        Utils.ShowAlert(this, "error !", result.Message);
                    }
                }
            }   
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, "Error !", ex.Message);
            }
        }
    
        void SetupAutoLayout()
        {
            otpImg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            otpImg.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 90).Active = true;
            otpImg.WidthAnchor.ConstraintEqualTo(80).Active = true;
            otpImg.HeightAnchor.ConstraintEqualTo(80).Active = true;

            lblVertify.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblVertify.TopAnchor.ConstraintEqualTo(otpImg.SafeAreaLayoutGuide.BottomAnchor, 29).Active = true;
            lblVertify.WidthAnchor.ConstraintEqualTo(300).Active = true;
            lblVertify.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblerror.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblerror.TopAnchor.ConstraintEqualTo(lblVertify.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            //lblerror.BackgroundColor = UIColor.Green;
            lblerror.WidthAnchor.ConstraintEqualTo(300).Active = true;
            lblerror.HeightAnchor.ConstraintGreaterThanOrEqualTo(30).Active = true;

            otp1.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, (View.Frame.Width - 290) / 2).Active = true;
            otp1.TopAnchor.ConstraintEqualTo(lblerror.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            otp1.WidthAnchor.ConstraintEqualTo(40).Active = true;
            otp1.HeightAnchor.ConstraintEqualTo(55).Active = true;

            otp2.LeftAnchor.ConstraintEqualTo(otp1.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            otp2.TopAnchor.ConstraintEqualTo(lblerror.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            otp2.WidthAnchor.ConstraintEqualTo(40).Active = true;
            otp2.HeightAnchor.ConstraintEqualTo(55).Active = true;

            otp3.LeftAnchor.ConstraintEqualTo(otp2.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            otp3.TopAnchor.ConstraintEqualTo(lblerror.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            otp3.WidthAnchor.ConstraintEqualTo(40).Active = true;
            otp3.HeightAnchor.ConstraintEqualTo(55).Active = true;

            otp4.LeftAnchor.ConstraintEqualTo(otp3.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            otp4.TopAnchor.ConstraintEqualTo(lblerror.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            otp4.WidthAnchor.ConstraintEqualTo(40).Active = true;
            otp4.HeightAnchor.ConstraintEqualTo(55).Active = true;

            otp5.LeftAnchor.ConstraintEqualTo(otp4.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            otp5.TopAnchor.ConstraintEqualTo(lblerror.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            otp5.WidthAnchor.ConstraintEqualTo(40).Active = true;
            otp5.HeightAnchor.ConstraintEqualTo(55).Active = true;

            otp6.LeftAnchor.ConstraintEqualTo(otp5.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            otp6.TopAnchor.ConstraintEqualTo(lblerror.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            otp6.WidthAnchor.ConstraintEqualTo(40).Active = true;
            otp6.HeightAnchor.ConstraintEqualTo(55).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        [Export("Edit:")]
        public void Edit(UIGestureRecognizer sender)
        {
            newotp1.BecomeFirstResponder();
        }
    }
}