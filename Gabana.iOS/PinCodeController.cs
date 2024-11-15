using Foundation;
using Gabana.Controller;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{

    public partial class PinCodeController : UIViewController
    {

        UIImageView PinCodeImg,GabanaLogoTextImg;
        MainController mainController = null;
        string PinTxt = "";
        int countPin=0;
        UIImageView pin1Img, pin2Img, pin3Img, pin4Img, pin5Img, pin6Img;
        UILabel lblPincode,lblIncorrect,lblforget, lblforget2;
        UIView numpadView;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete;

        string Pincode = string.Empty, PincodeCorrect;
        UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
        string typePage = "";
        string usernamelogin = "";
        string pin1 = null, pin2 = null;
        public PinCodeController(string v)
        {
            typePage = v;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (this.NavigationController != null)
            {
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
            }

            
        }
        public override void ViewDidLoad()
        {
            if (this.NavigationController != null)
            {
                this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
            }
            base.ViewDidLoad();
            //  this.Title = "Pin Code";
            

            View.BackgroundColor = UIColor.White;
            PinTxt = "";
            countPin = 0;

            usernamelogin = Preferences.Get("User", "");
            GetUserAccount();

            LayoutAttribute();
            SetupAutoLayout();
            SetPINCODE();
           
        }
        async void SetPINCODE()
        {
            Utils.SetTitle(this.NavigationController,"Pincode");
            switch (typePage)
            {
                case "NewPincode": 
                    lblPincode.Text = "Enter your PIN code";
                    lblforget.Hidden = true;
                    lblforget2.Hidden = true;
                    
                    break;
                case "ChangePincode": 
                    lblPincode.Text = "Enter your Old PIN code";
                    lblforget.Hidden = true;
                    lblforget2.Hidden = true;
                    Utils.SetConstant(lblforget.Constraints, NSLayoutAttribute.Width, 0);


                    break;
                case "Pincode":
                    lblPincode.Text = "Enter your PIN code";
                    lblforget.Hidden = false;
                    lblforget2.Hidden = true;
                    
                    break;
                case "OffPincode":
                    lblPincode.Text = "Enter your PIN code";
                    lblforget.Hidden = true;
                    lblforget2.Hidden = false;
                    
                    break;
                default:
                    break;
            }
        }
        async void GetUserAccount()
        {
            try
            {
                var Data = await accountInfoManage.GetUserAccount(DataCashingAll.MerchantId, usernamelogin);
                if (Data != null)
                {
                    PincodeCorrect = Data.PinCode;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetUserAccount at Pincode");
            }
        }
        void LayoutAttribute()
        {
            #region Pin Top
            PinCodeImg = new UIImageView();
            PinCodeImg.Image = UIImage.FromBundle("PinCode");
            PinCodeImg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(PinCodeImg);

            lblPincode = new UILabel();
            lblPincode.Text = "Enter your Pin Code";
            lblPincode.Font = lblPincode.Font.WithSize(15);
            lblPincode.TextColor = UIColor.FromRGB(112, 112, 112);
            lblPincode.TextAlignment = UITextAlignment.Center;
            lblPincode.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblPincode);

            pin1Img = new UIImageView();
            pin1Img.Image = UIImage.FromBundle("UserInactive");
            pin1Img.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(pin1Img);

            pin2Img = new UIImageView();
            pin2Img.Image = UIImage.FromBundle("UserInactive");
            pin2Img.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(pin2Img);

            pin3Img = new UIImageView();
            pin3Img.Image = UIImage.FromBundle("UserInactive");
            pin3Img.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(pin3Img);

            pin4Img = new UIImageView();
            pin4Img.Image = UIImage.FromBundle("UserInactive");
            pin4Img.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(pin4Img);

            pin5Img = new UIImageView();
            pin5Img.Image = UIImage.FromBundle("UserInactive");
            pin5Img.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(pin5Img);

            pin6Img = new UIImageView();
            pin6Img.Image = UIImage.FromBundle("UserInactive");
            pin6Img.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(pin6Img);
            #endregion

            lblIncorrect = new UILabel();
            lblIncorrect.Text = "Incorrect Pin Code.";
            lblIncorrect.Hidden = true;
            lblIncorrect.Font = lblIncorrect.Font.WithSize(12);
            lblIncorrect.TextColor = UIColor.FromRGB(247,86,0);
            lblIncorrect.TextAlignment = UITextAlignment.Right;
            lblIncorrect.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblIncorrect);

            lblforget = new UILabel();
            lblforget.Text = "Forgot Pin Code?";
            lblforget.Font = lblIncorrect.Font.WithSize(12);
            lblforget.TextColor = UIColor.FromRGB(0,149,218);
            lblforget.TextAlignment = UITextAlignment.Center;
            lblforget.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblforget);

            lblforget.UserInteractionEnabled = true;
            var tapGestureForget = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Forget:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            lblforget.AddGestureRecognizer(tapGestureForget);

            lblforget2 = new UILabel();
            lblforget2.Hidden = true;
            lblforget2.Text = "Forgot Pin Code?";
            lblforget2.Font = lblIncorrect.Font.WithSize(12);
            lblforget2.TextColor = UIColor.FromRGB(0, 149, 218);
            lblforget2.TextAlignment = UITextAlignment.Center;
            lblforget2.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(lblforget2);

            lblforget2.UserInteractionEnabled = true;
            var tapGestureForget2 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Forget:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            lblforget2.AddGestureRecognizer(tapGestureForget2);

            #region numpadView
            numpadView = new UIView();
            numpadView.BackgroundColor = UIColor.White;
            numpadView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(numpadView);

            NumberpadSetup();
            #endregion

            GabanaLogoTextImg = new UIImageView();
            GabanaLogoTextImg.Image = UIImage.FromBundle("PinCode_Gabana");
            GabanaLogoTextImg.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(GabanaLogoTextImg);
        }
        void NumberpadSetup()
        {
            btnone = new UIButton();
            btnone.BackgroundColor = UIColor.White;
            btnone.TitleLabel.Font = btnone.TitleLabel.Font.WithSize(23);
            btnone.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnone.Layer.BorderWidth = 1f;
            btnone.Layer.CornerRadius = 33;
            btnone.SetTitle("1", UIControlState.Normal);
            btnone.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btnone.TranslatesAutoresizingMaskIntoConstraints = false;
            btnone.TouchUpInside += (sender, e) => {
                //1 press
                if(countPin <7)
                {
                    PinTxt = PinTxt+"1";
                    countPin = countPin+1;
                }
                changeImage();
               
            };
            numpadView.AddSubview(btnone);

            btntwo = new UIButton();
            btntwo.BackgroundColor = UIColor.White;
            btntwo.TitleLabel.Font = btntwo.TitleLabel.Font.WithSize(23);
            btntwo.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btntwo.Layer.BorderWidth = 1f;
            btntwo.Layer.CornerRadius = 33;
            btntwo.SetTitle("2", UIControlState.Normal);
            btntwo.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btntwo.TranslatesAutoresizingMaskIntoConstraints = false;
            btntwo.TouchUpInside += (sender, e) => {
                //2 press 
                if (countPin < 7)
                {
                    PinTxt += "2";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btntwo);

            btnthree = new UIButton();
            btnthree.BackgroundColor = UIColor.White;
            btnthree.SetTitle("3", UIControlState.Normal);
            btnthree.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnthree.Layer.BorderWidth = 1f;
            btnthree.Layer.CornerRadius = 33;
            btnthree.TitleLabel.Font = btnthree.TitleLabel.Font.WithSize(23);
            btnthree.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btnthree.TranslatesAutoresizingMaskIntoConstraints = false;
            btnthree.TouchUpInside += (sender, e) => {
                //3 press
                if (countPin < 7)
                {
                    PinTxt += "3";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btnthree);

            btnfour = new UIButton();
            btnfour.BackgroundColor = UIColor.White;
            btnfour.SetTitle("4", UIControlState.Normal);
            btnfour.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnfour.Layer.BorderWidth = 1f;
            btnfour.Layer.CornerRadius = 33;
            btnfour.TitleLabel.Font = btnfour.TitleLabel.Font.WithSize(23);
            btnfour.SetTitleColor(UIColor.FromRGB(64,64,64), UIControlState.Normal);
            btnfour.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfour.TouchUpInside += (sender, e) => {
                //4 press
                if (countPin < 7)
                {
                    PinTxt += "4";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btnfour);

            btnfive = new UIButton();
            btnfive.BackgroundColor = UIColor.White;
            btnfive.SetTitle("5", UIControlState.Normal);
            btnfive.TitleLabel.Font = btnfive.TitleLabel.Font.WithSize(23);
            btnfive.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnfive.Layer.BorderWidth = 1f;
            btnfive.Layer.CornerRadius = 33;
            btnfive.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btnfive.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfive.TouchUpInside += (sender, e) => {
                //5 press
                if (countPin < 7)
                {
                    PinTxt += "5";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btnfive);

            btnsix = new UIButton();
            btnsix.BackgroundColor = UIColor.White;
            btnsix.TitleLabel.Font = btnsix.TitleLabel.Font.WithSize(23);
            btnsix.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnsix.Layer.BorderWidth = 1f;
            btnsix.Layer.CornerRadius = 33;
            btnsix.SetTitle("6", UIControlState.Normal);
            btnsix.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btnsix.TranslatesAutoresizingMaskIntoConstraints = false;
            btnsix.TouchUpInside += (sender, e) => {
                if (countPin < 7)
                {
                    PinTxt += "6";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btnsix);

            btnseven = new UIButton();
            btnseven.BackgroundColor = UIColor.White;
            btnseven.SetTitle("7", UIControlState.Normal);
            btnseven.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnseven.Layer.BorderWidth = 1f;
            btnseven.Layer.CornerRadius = 33;
            btnseven.TitleLabel.Font = btnseven.TitleLabel.Font.WithSize(23);
            btnseven.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btnseven.TranslatesAutoresizingMaskIntoConstraints = false;
            btnseven.TouchUpInside += (sender, e) => {
                //7 press
                if (countPin < 7)
                {
                    PinTxt += "7";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btnseven);

            btneight = new UIButton();
            btneight.BackgroundColor = UIColor.White;
            btneight.TitleLabel.Font = btneight.TitleLabel.Font.WithSize(23);
            btneight.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btneight.Layer.BorderWidth = 1f;
            btneight.Layer.CornerRadius = 33;
            btneight.SetTitle("8", UIControlState.Normal);
            btneight.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btneight.TranslatesAutoresizingMaskIntoConstraints = false;
            btneight.TouchUpInside += (sender, e) => {
                //8 press
                if (countPin < 7)
                {
                    PinTxt += "8";
                    countPin +=  + 1;
                }
                changeImage();
            };
            numpadView.AddSubview(btneight);

            btnnine = new UIButton();
            btnnine.BackgroundColor = UIColor.White;
            btnnine.TitleLabel.Font = btnnine.TitleLabel.Font.WithSize(23);
            btnnine.SetTitle("9", UIControlState.Normal);
            btnnine.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnnine.Layer.BorderWidth = 1f;
            btnnine.Layer.CornerRadius = 33;
            btnnine.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btnnine.TranslatesAutoresizingMaskIntoConstraints = false;
            btnnine.TouchUpInside += (sender, e) => {
                //9 press
                if (countPin < 7)
                {
                    PinTxt += "9";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btnnine);

            btnzero = new UIButton();
            btnzero.BackgroundColor = UIColor.White;
            btnzero.TitleLabel.Font = btnzero.TitleLabel.Font.WithSize(23);
            btnzero.SetTitle("0", UIControlState.Normal);
            btnzero.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btnzero.Layer.BorderWidth = 1f;
            btnzero.Layer.CornerRadius = 33;
            btnzero.SetTitleColor(UIColor.FromRGB(64, 64, 64), UIControlState.Normal);
            btnzero.TranslatesAutoresizingMaskIntoConstraints = false;
            btnzero.TouchUpInside += (sender, e) => {
                //0 press
                if (countPin < 7)
                {
                    PinTxt += "0";
                    countPin++;
                }
                changeImage();
            };
            numpadView.AddSubview(btnzero);

            btndelete = new UIButton();
            btndelete.SetImage(UIImage.FromBundle("Del"), UIControlState.Normal);
            btndelete.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            btndelete.Layer.BorderWidth = 1f;
            btndelete.Layer.CornerRadius = 33;
            btndelete.ContentMode = UIViewContentMode.ScaleAspectFit;
            btndelete.ImageEdgeInsets = new UIEdgeInsets(20, 20, 20, 20);
            btndelete.BackgroundColor = UIColor.White;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.TouchUpInside += (sender, e) => {
                //0 press
                if (countPin < 7 && countPin>0)
                {
                    PinTxt = PinTxt.Remove(PinTxt.Length - 1);
                    countPin--;
                }
                changeImage();
            };
            numpadView.AddSubview(btndelete);
        }
        void changeImage()
        {
            countPin = PinTxt.Length;
            if(countPin==1)
            {
                pin1Img.Image = UIImage.FromBundle("UserActive");
                pin2Img.Image = UIImage.FromBundle("UserInactive");
                pin3Img.Image = UIImage.FromBundle("UserInactive");
                pin4Img.Image = UIImage.FromBundle("UserInactive");
                pin5Img.Image = UIImage.FromBundle("UserInactive");
                pin6Img.Image = UIImage.FromBundle("UserInactive");
            }
            else if (countPin == 2)
            {
                pin1Img.Image = UIImage.FromBundle("UserActive");
                pin2Img.Image = UIImage.FromBundle("UserActive");
                pin3Img.Image = UIImage.FromBundle("UserInactive");
                pin4Img.Image = UIImage.FromBundle("UserInactive");
                pin5Img.Image = UIImage.FromBundle("UserInactive");
                pin6Img.Image = UIImage.FromBundle("UserInactive");
            }
            else if (countPin == 3)
            {
                pin1Img.Image = UIImage.FromBundle("UserActive");
                pin2Img.Image = UIImage.FromBundle("UserActive");
                pin3Img.Image = UIImage.FromBundle("UserActive");
                pin4Img.Image = UIImage.FromBundle("UserInactive");
                pin5Img.Image = UIImage.FromBundle("UserInactive");
                pin6Img.Image = UIImage.FromBundle("UserInactive");
            }
            else if (countPin == 4)
            {
                pin1Img.Image = UIImage.FromBundle("UserActive");
                pin2Img.Image = UIImage.FromBundle("UserActive");
                pin3Img.Image = UIImage.FromBundle("UserActive");
                pin4Img.Image = UIImage.FromBundle("UserActive");
                pin5Img.Image = UIImage.FromBundle("UserInactive");
                pin6Img.Image = UIImage.FromBundle("UserInactive");
            }
            else if (countPin == 5)
            {
                pin1Img.Image = UIImage.FromBundle("UserActive");
                pin2Img.Image = UIImage.FromBundle("UserActive");
                pin3Img.Image = UIImage.FromBundle("UserActive");
                pin4Img.Image = UIImage.FromBundle("UserActive");
                pin5Img.Image = UIImage.FromBundle("UserActive");
                pin6Img.Image = UIImage.FromBundle("UserInactive");
            }
            else if (countPin == 6)
            {
                pin1Img.Image = UIImage.FromBundle("UserActive");
                pin2Img.Image = UIImage.FromBundle("UserActive");
                pin3Img.Image = UIImage.FromBundle("UserActive");
                pin4Img.Image = UIImage.FromBundle("UserActive");
                pin5Img.Image = UIImage.FromBundle("UserActive");
                pin6Img.Image = UIImage.FromBundle("UserActive");
                checkPin();
            }
            else
            {
                pin1Img.Image = UIImage.FromBundle("UserInactive");
                pin2Img.Image = UIImage.FromBundle("UserInactive");
                pin3Img.Image = UIImage.FromBundle("UserInactive");
                pin4Img.Image = UIImage.FromBundle("UserInactive");
                pin5Img.Image = UIImage.FromBundle("UserInactive");
                pin6Img.Image = UIImage.FromBundle("UserInactive");
            }
        }
        void checkPin()
        {
            //check pin
            switch (typePage)
            {
                case "NewPincode":
                    if (pin1 == null)
                    {
                        pin1 = PinTxt;
                        PinTxt = "";
                        lblPincode.Text = "Confirm New PIN code";
                        changeImage();
                        return;
                    }
                    else
                    {
                        pin2 = PinTxt;
                    }

                    if (pin1 != pin2)
                    {
                        lblIncorrect.Hidden = false;
                        PinTxt = string.Empty;
                        changeImage();
                    }
                    else
                    {
                        ChangePasswordController.back = true;
                        ChangePasswordController.UsePincode = 1;
                        ChangePasswordController.NewPincode = PinTxt;
                        //ChangePasswordController.LocalUseraccount.FUsePincode = 1;

                        ChangePasswordController.ChangeSuccess = true; 
                        this.NavigationController?.PopViewController(false);
                    }
                    break;
                case "ChangePincode":
                    if (pin1 == null)
                    {
                        pin1 = PinTxt;
                        PinTxt = string.Empty;

                        if (pin1 == PincodeCorrect)
                        {
                            lblIncorrect.Hidden = true;
                            lblPincode.Text = "Enter New PIN code";
                            changeImage();
                        }
                        else
                        {
                            pin1 = null;
                            lblIncorrect.Hidden = false;
                            changeImage();
                            return;
                        }

                    }
                    else if (pin2 == null)
                    {
                        pin2 = PinTxt;
                        PinTxt = string.Empty;
                        lblPincode.Text = "Confirm New PIN code";
                        changeImage();
                    }
                    else
                    {
                        if (pin2 == PinTxt)
                        {
                            ChangePasswordController.UsePincode = 1;
                            ChangePasswordController.NewPincode = PinTxt;
                            //ChangePasswordController.LocalUseraccount.FUsePincode = Convert.ToInt64(pin2);
                            ChangePasswordController.ChangeSuccess = true;
                            this.NavigationController?.PopViewController(false);
                        }
                        else
                        {
                            PinTxt = string.Empty;
                            lblIncorrect.Hidden = false;
                            lblforget2.Hidden = false;
                            lblforget.Hidden = true;
                            lblPincode.Text = "Confirm New PIN code";
                            changeImage();
                            return;
                        }
                    }

                    break;
                case "OffPincode":
                    pin1 = PinTxt;
                    if (PincodeCorrect == pin1)
                    {
                        ChangePasswordController.back = true;
                        ChangePasswordController.UsePincode = 0;
                        ChangePasswordController.ChangeSuccess = true;
                        this.NavigationController?.PopViewController(false);
                    }
                    else
                    {
                        lblIncorrect.Hidden = false;
                        lblPincode.Text = "Enter PIN code";
                        PinTxt = string.Empty;
                        changeImage();
                        //PinTxt = string.Empty;
                    }


                    break;
                case "Pincode":
                    if (PinTxt == PincodeCorrect)
                    {
                        DataCashingAll.UserActive = DateTime.Now;
                        this.DismissViewControllerAsync(false);
                        //this.NavigationController?.PopViewController(false);
                    }
                    else
                    {
                        lblIncorrect.Hidden = false;
                        lblforget2.Hidden = false;
                        lblforget.Hidden = true;
                        Pincode = string.Empty;
                        lblPincode.Text = "รหัส PIN ไม่ถูกต้อง";
                        
                        PinTxt = string.Empty;
                        changeImage();
                    }


                    break;
                default:
                    break;
            }
        }
        [Export("Forget:")]
        public void Forget(UIGestureRecognizer sender)
        {
            // forget pass
        }
        void SetupAutoLayout()
        {
            #region Pin Top
            PinCodeImg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            PinCodeImg.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, (int)View.Frame.Height*35/1000).Active = true;
            PinCodeImg.WidthAnchor.ConstraintEqualTo(60).Active = true;
            PinCodeImg.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblPincode.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblPincode.TopAnchor.ConstraintEqualTo(PinCodeImg.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            lblPincode.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblPincode.HeightAnchor.ConstraintEqualTo(19).Active = true;

            pin1Img.RightAnchor.ConstraintEqualTo(pin2Img.SafeAreaLayoutGuide.LeftAnchor, -20).Active = true;
            pin1Img.TopAnchor.ConstraintEqualTo(lblPincode.SafeAreaLayoutGuide.BottomAnchor, 18).Active = true;
            pin1Img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            pin1Img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            pin2Img.RightAnchor.ConstraintEqualTo(pin3Img.SafeAreaLayoutGuide.LeftAnchor, -20).Active = true;
            pin2Img.TopAnchor.ConstraintEqualTo(lblPincode.SafeAreaLayoutGuide.BottomAnchor, 18).Active = true;
            pin2Img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            pin2Img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            pin3Img.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, -10).Active = true;
            pin3Img.TopAnchor.ConstraintEqualTo(lblPincode.SafeAreaLayoutGuide.BottomAnchor, 18).Active = true;
            pin3Img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            pin3Img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            pin4Img.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 10).Active = true;
            pin4Img.TopAnchor.ConstraintEqualTo(lblPincode.SafeAreaLayoutGuide.BottomAnchor, 18).Active = true;
            pin4Img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            pin4Img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            pin5Img.LeftAnchor.ConstraintEqualTo(pin4Img.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            pin5Img.TopAnchor.ConstraintEqualTo(lblPincode.SafeAreaLayoutGuide.BottomAnchor, 18).Active = true;
            pin5Img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            pin5Img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            pin6Img.LeftAnchor.ConstraintEqualTo(pin5Img.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            pin6Img.TopAnchor.ConstraintEqualTo(lblPincode.SafeAreaLayoutGuide.BottomAnchor, 18).Active = true;
            pin6Img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            pin6Img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            #endregion

            //lbl 17 - 16
            lblIncorrect.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor,-5).Active = true;
            lblIncorrect.TopAnchor.ConstraintEqualTo(pin6Img.SafeAreaLayoutGuide.BottomAnchor, 17).Active = true;
            lblIncorrect.WidthAnchor.ConstraintEqualTo(109).Active = true;
            lblIncorrect.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lblforget.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblforget.TopAnchor.ConstraintEqualTo(pin6Img.SafeAreaLayoutGuide.BottomAnchor, 17).Active = true;
            lblforget.WidthAnchor.ConstraintEqualTo(98).Active = true;
            lblforget.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lblforget2.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor,5).Active = true;
            lblforget2.TopAnchor.ConstraintEqualTo(pin6Img.SafeAreaLayoutGuide.BottomAnchor, 17).Active = true;
            lblforget2.WidthAnchor.ConstraintEqualTo(109).Active = true;
            lblforget2.HeightAnchor.ConstraintEqualTo(19).Active = true;

            #region numpadView

            numpadView.TopAnchor.ConstraintEqualTo(lblforget.SafeAreaLayoutGuide.BottomAnchor, 16).Active = true;
            numpadView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            numpadView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            numpadView.BottomAnchor.ConstraintEqualTo(GabanaLogoTextImg.SafeAreaLayoutGuide.TopAnchor, -30).Active = true;

            btnone.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            btnone.RightAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.LeftAnchor, -45).Active = true;
            btnone.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnone.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btntwo.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            btntwo.CenterXAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            btntwo.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btntwo.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btnthree.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            btnthree.LeftAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.RightAnchor,45).Active = true;
            btnthree.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnthree.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btnfour.TopAnchor.ConstraintEqualTo(btnone.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btnfour.RightAnchor.ConstraintEqualTo(btnfive.SafeAreaLayoutGuide.LeftAnchor, -45).Active = true;
            btnfour.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnfour.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btnfive.TopAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btnfive.CenterXAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            btnfive.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnfive.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btnsix.TopAnchor.ConstraintEqualTo(btnthree.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btnsix.LeftAnchor.ConstraintEqualTo(btnfive.SafeAreaLayoutGuide.RightAnchor, 45).Active = true;
            btnsix.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnsix.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btnseven.TopAnchor.ConstraintEqualTo(btnfour.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btnseven.RightAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.LeftAnchor, -45).Active = true;
            btnseven.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnseven.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btneight.TopAnchor.ConstraintEqualTo(btnfive.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btneight.CenterXAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            btneight.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btneight.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btnnine.TopAnchor.ConstraintEqualTo(btnsix.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btnnine.LeftAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.RightAnchor, 45).Active = true;
            btnnine.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnnine.WidthAnchor.ConstraintEqualTo(65).Active = true;


            btnzero.TopAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btnzero.CenterXAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnzero.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btnzero.WidthAnchor.ConstraintEqualTo(65).Active = true;

            btndelete.TopAnchor.ConstraintEqualTo(btnnine.SafeAreaLayoutGuide.BottomAnchor, 15).Active = true;
            btndelete.LeftAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.RightAnchor, 45).Active = true;
            btndelete.HeightAnchor.ConstraintEqualTo(65).Active = true;
            btndelete.WidthAnchor.ConstraintEqualTo(65).Active = true;
            #endregion

            GabanaLogoTextImg.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -23).Active = true;
            GabanaLogoTextImg.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            GabanaLogoTextImg.WidthAnchor.ConstraintEqualTo(130).Active = true;
            GabanaLogoTextImg.HeightAnchor.ConstraintEqualTo(40).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}