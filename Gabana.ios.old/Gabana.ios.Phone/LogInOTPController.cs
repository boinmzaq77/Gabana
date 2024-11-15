using Foundation;
using Gabana.Controller;
using Gabana.Model;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.ios.Phone
{
    public partial class LogInOTPController : UIViewController
    {
        public static Refstatus refstatus2 = new Refstatus();
        public static string email;
        private Task<Refstatus> refstring;
        public VerifyOTP verifyOTP; 
        public LogInOTPController (IntPtr handle) : base (handle)
        {
        }
       
        public override void ViewDidLoad()
        {
           
            base.ViewDidLoad();
            this.NavigationController.SetNavigationBarHidden(false, true);
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.NavigationBar.BarTintColor = new UIColor(red: 51f/255f, green: 170f/255f, blue: 225f / 255f, alpha: 1f);
           
            

            lblOTPtel.Text = email + " ";
            lblOTPref.Text = refstatus2.Refstring;
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            g.CancelsTouchesInView = false;
            View.AddGestureRecognizer(g);
        }
        internal static void ShowAlert(UIViewController uIViewController, string title, string detail)
        {
            var alert = UIAlertController.Create(title, detail, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            uIViewController.PresentViewController(alert, animated: true, completionHandler: null);
        }
        partial void otpchange1(UIKit.UITextField sender)
        {
            if (sender.Text != "")
            {
                otp2.BecomeFirstResponder();
                return;
            }
            otp1.BecomeFirstResponder();
        }
        partial void otpchange2(UIKit.UITextField sender)
        {
            if (sender.Text != "")
            {
                otp3.BecomeFirstResponder();
                return;
            }
            otp1.BecomeFirstResponder();
        }
        partial void otpchange3(UIKit.UITextField sender)
        {
            if (sender.Text != "")
            {
                otp4.BecomeFirstResponder();
                return;
            }
            otp2.BecomeFirstResponder();
        }
        partial void otpchange4(UIKit.UITextField sender)
        {
            if (sender.Text != "")
            {
                otp5.BecomeFirstResponder();
                return;
            }
            otp3.BecomeFirstResponder();
        }
        partial void otpchange5(UIKit.UITextField sender)
        {
            if (sender.Text != "")
            {
                otp6.BecomeFirstResponder();
                return;
            }
            otp4.BecomeFirstResponder();
        }

        async partial void otpchange6(UIKit.UITextField sender)
        {
            // check then go to main pages
            try
            {
                var text1 = otp1.Text;
                var text2 = otp2.Text;
                var text3 = otp3.Text;
                var text4 = otp4.Text;
                var text5 = otp5.Text;
                var text6 = otp6.Text;
                var opttext = text1 + text2 + text3 + text4 + text5 + text6;
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
                    var result = await GabanaAPI.CreateOrLoginGabana(verifyOTP);
                    if (result!=null)
                    {
                        MainPageController main = this.Storyboard.InstantiateViewController("MainPageController") as MainPageController;
                        this.NavigationController.PushViewController(main, true);
                        Preferences.Set("AppState", "logon");
                    }
                }
                GabanaLoading.SharedInstance.Hide();
                this.NavigationController.DismissViewController(false, null);
            }
            catch (Exception ex)
            {
                otp1.Text = "";
                otp2.Text = "";
                otp3.Text = "";
                otp4.Text = "";
                otp5.Text = "";
                otp6.Text = "";
                otp1.BecomeFirstResponder();
                GabanaLoading.SharedInstance.Hide();
                ShowAlert(this, "ไม่สำเร็จ ! ", "ไม่สามารถเข้าสู่ระบบกรุณาติดต่อ Admin");
            }
        }

        internal void SetRefString(VerifyOTP refstring)
        {
            verifyOTP = refstring; 
        }
    }
    public class Refstatus
    {
        public string Refstring { get; set; }   // 
        public string type { get; set; }        // "create" or "login"
    }
}