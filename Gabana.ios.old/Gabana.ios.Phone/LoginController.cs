using Foundation;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Gabana.Controller;
using Gabana.Model;

namespace Gabana.ios.Phone
{
    public partial class LoginController : UIViewController
    {
        public LoginController (IntPtr handle) : base (handle)
        {
        }
        public async override void ViewDidLoad()
        {
           
            base.ViewDidLoad();
            CultureInfo ci = new CultureInfo("th-TH");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            try
            {

                GabanaAPI.Jwt1 = await GetToken.Get_JWT_1();

                segLogin.SelectedSegment = 0;

                var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(true,false);
                this.NavigationItem.BackButtonDisplayMode = UINavigationItemBackButtonDisplayMode.Minimal;

                BtnGoRegister.Layer.BorderWidth = 1;
                BtnGoRegister.Layer.BorderColor = new UIColor(red:0f,green: 149f/255f,blue: 225f/255f,alpha:1f).CGColor;

                btnRegis.Layer.BorderWidth = 1;
                btnRegis.Layer.BorderColor = new UIColor(red: 0f, green: 149f / 255f, blue: 225f / 255f, alpha: 1f).CGColor;
                txtEmpMer.ShouldReturn = (textField) =>
                    {
                        textField.ResignFirstResponder();
                        return true;
                    };
                txtEmpUser.ShouldReturn = (textField) =>
                {
                    textField.ResignFirstResponder();
                    return true;
                };
                txtEmpPass.ShouldReturn = (textField) =>
                {
                    textField.ResignFirstResponder();
                    return true;
                };
                txtOwnerTel.ShouldReturn = (textField) =>
                {
                    textField.ResignFirstResponder();
                    return true;
                };
                txtRegisTel.ShouldReturn = (textField) =>
                {
                    textField.ResignFirstResponder();
                    return true;
                };
                var g = new UITapGestureRecognizer(() => View.EndEditing(true));
                g.CancelsTouchesInView = false;
                View.AddGestureRecognizer(g);
            }
            
            
            catch (Exception ex)
            {

                throw;
            }

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(true, false);
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        partial void SegmenteControl_ValueChanged(UISegmentedControl sender)
        {
            var index = segLogin.SelectedSegment;
            if (index == 0)
            {
                viewOwner.Hidden = false;
                viewEmployee.Hidden = true;
                viewRegister.Hidden = true;
                viewMainLogin.Hidden = true;
                segLogin.Hidden = false;
                btnRegisBack.Hidden = false;
                btnOwnerLogin.Enabled = true;
                btnEmpLogIn.Enabled = false;
            }
            else if (index == 1)
            {
                viewOwner.Hidden = true;
                viewEmployee.Hidden = false;
                viewRegister.Hidden = true;
                viewMainLogin.Hidden = true;
                segLogin.Hidden = false;
                btnRegisBack.Hidden = false;
                btnOwnerLogin.Enabled = false;
                btnEmpLogIn.Enabled = true;
            }
        }
        internal static void OnKeyboardChanged(UIView view, bool visible, nfloat nfloat)
        {
            if (!visible)
                view.Frame = new CoreGraphics.CGRect(0, 0, view.Frame.Width, view.Frame.Height);
            else
                view.Frame = new CoreGraphics.CGRect(0, 0 - nfloat, view.Frame.Width, view.Frame.Height);
        }
        async partial void BtnGoRegister_TouchUpInside(UIButton sender)
        {
            viewRegister.Hidden = false;
            viewEmployee.Hidden = true;
            viewOwner.Hidden = true;
            viewMainLogin.Hidden = true;
            btnRegisBack.Hidden = false;
            segLogin.Hidden = true;
        }
        async partial void BtnEmpLogIn_TouchUpInside(UIButton sender)
        {
            //check tel
            // then go to main
            try
            {
                if (txtEmpMer.Text != null || txtEmpMer.Text != "")
                {
                    ShowAlert(this, "ไม่สำเร็จ ! ", "รหัสร้านค้าไม่ถูกต้อง");
                    return;
                }
                if(txtEmpUser.Text == "" || txtEmpUser.Text == null)
                {
                    ShowAlert(this, "ไม่สำเร็จ ! ", "กรุณากรอกUserNameที่ถูกต้อง");
                    return;
                }
                if (txtEmpPass.Text == "" || txtEmpPass.Text == null)
                {
                    ShowAlert(this, "ไม่สำเร็จ ! ", "กรุณากรอกPasswordที่ถูกต้อง");
                    return;
                }
                else
                {
                    //go to check tel + pass + user
                    MainPageController main = this.Storyboard.InstantiateViewController("MainPageController") as MainPageController;
                    this.NavigationController.PushViewController(main, true);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
                //LoginController.ShowAlert(this, "ไม่สำเร็จ ! ", ex.Message);
            }
        }
        public async Task ClicksentOTP(char typeBtn)
        {
            try
            {
                if(typeBtn == 'R')
                {
                    if (txtRegisTel.Text == Preferences.Get("PHONE", "") && Preferences.Get("PHONE", "") != "")
                    {
                        OpenMainPage();
                    }
                    else
                    {
                        string emailstring;
                        if (txtRegisTel.Text != "")
                        {
                            emailstring = txtRegisTel.Text;
                            var refstring = await Sentotp(emailstring);
                            LogInOTPController otp = this.Storyboard?.InstantiateViewController("LogInOTPController") as LogInOTPController;
                            otp.SetRefString(refstring);
                            NavigationController.PushViewController(otp, true);
                        }
                        else
                        {
                            ShowAlert(this, "", "Please enter your phone number.");
                        }
                    }
                }
                else
                {
                    if (txtOwnerTel.Text == Preferences.Get("PHONE", "") && Preferences.Get("PHONE", "") != "")
                    {
                        OpenMainPage();
                    }
                    else
                    {
                        string emailstring;
                        if (txtOwnerTel.Text != "")
                        {
                            emailstring = txtOwnerTel.Text;
                            var refstring = await Sentotp(emailstring);

                            LogInOTPController otp = this.Storyboard.InstantiateViewController("LogInOTP") as LogInOTPController;
                            otp.SetRefString(refstring);
                            NavigationController.PushViewController(otp, true);
                        }
                        else
                        {
                            ShowAlert(this, "", "Please enter your phone number.");
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                var x = ex.GetType();
                ShowAlert(this, "ไม่สำเร็จ ! ", ex.Message);

            }
        }
        public async void OpenMainPage()
        {
            Preferences.Set("AppState", "logon");
        }
        partial void BtnRegisBack_TouchUpInside(UIButton sender)
        {
            segLogin.SelectedSegment = 0;
            viewRegister.Hidden = true;
            viewEmployee.Hidden = true;
            viewOwner.Hidden = true;
            viewMainLogin.Hidden = false;
            txtRegisTel.Text = null;
            txtEmpPass.Text = null;
            txtEmpMer.Text = null;
            txtEmpUser.Text = null;
            txtOwnerTel.Text = null;
            btnRegisBack.Hidden = true;
            segLogin.Hidden = true;
        }
        internal static void ShowAlert(UIViewController uIViewController, string title, string detail)
        {
            var alert = UIAlertController.Create(title, detail, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            uIViewController.PresentViewController(alert, animated: true, completionHandler: null);
        }
        public async Task<VerifyOTP> Sentotp(string emailstring)
        {

            string Id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
            var verify = new SendOTP() { OwnerID = emailstring, UDID = Id };
            var refstring = await GabanaAPI.InitialGabana(verify);
            VerifyOTP verifyOTP = new VerifyOTP() {OwnerID = emailstring , RefOTP = refstring };
            
            return verifyOTP;
        }
        async partial void BtnRegis_TouchUpInside(UIButton sender)
        {
            //check tel
            // then go to otp
            try
            {
                if (txtRegisTel.Text.Length != 10)
                {
                    ShowAlert(this, "ไม่สำเร็จ ! ", "เบอร์โทรศัพท์ไม่ถูกต้อง");
                    return;
                }
                await ClicksentOTP('R');
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
                //LoginController.ShowAlert(this, "ไม่สำเร็จ ! ", ex.Message);
            }
        }
        public static bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // MUST CONTAIN ONE AND ONLY ONE @
            var atCount = input.Count(c => c == '@');
            if (atCount != 1) return false;

            // MUST CONTAIN PERIOD
            if (!input.Contains(".")) return false;

            // @ MUST OCCUR BEFORE LAST PERIOD
            var indexOfAt = input.IndexOf("@", StringComparison.Ordinal);
            var lastIndexOfPeriod = input.LastIndexOf(".", StringComparison.Ordinal);
            var atBeforeLastPeriod = lastIndexOfPeriod > indexOfAt;
            if (!atBeforeLastPeriod) return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }

        async partial void BtnGoOwnerLogIn_TouchUpInside(UIButton sender)
        {
            viewOwner.Hidden = false;
            viewEmployee.Hidden = true;
            viewRegister.Hidden = true;
            viewMainLogin.Hidden = true;
            segLogin.Hidden = false;
            btnRegisBack.Hidden = false;
            btnRegis.Hidden = false;
            btnOwnerLogin.Enabled = true;
        }

        async partial void BtnOwnerLogin_TouchUpInside(UIButton sender)
        {
            try
            {
                if (txtOwnerTel.Text.Length != 10)
                {
                    ShowAlert(this, "ไม่สำเร็จ! ", "เบอร์โทรศัพท์ไม่ถูกต้อง");
                    return;
                }
                else
                    await ClicksentOTP('O');
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
                //LoginController.ShowAlert(this, "ไม่สำเร็จ ! ", ex.Message);
            }
        }
    }
}