using Foundation;
using Gabana.ios;
using Gabana.iOS;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.POS.Cart
{
    public partial class SidebarMenuController : UIViewController
    {
        UIView contentBarView;
        UIView UpperView, PasswordView, LanguageView, ContactUSView, TermView, VersionView, LogoutView;
        UIImageView logoImg,gabanatxtImg,PassImg,LangImg,ContactImg,TermImg,VersionImg,LogoutImg;
        UILabel lblPassword, lblLang, lblContact, lblTerm, lblVersion, lblLogout, lblVersionNo;
        public SidebarMenuController()
        {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        public async override void ViewDidLoad()
        {
            DataCaching.MainNavigation.SetNavigationBarHidden(true,false);
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.Clear;

            initAttribute();
            setupAutoLayout();

            View.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Close:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            View.AddGestureRecognizer(tapGesture0);

           
           
        }
        void initAttribute()
        {
            contentBarView = new UIView();
            contentBarView.TranslatesAutoresizingMaskIntoConstraints = false;
            contentBarView.BackgroundColor = UIColor.White;
            View.AddSubview(contentBarView);

            #region UpperView
            UpperView = new UIView();
            UpperView.BackgroundColor = UIColor.FromRGB(241,250,255);
            UpperView.TranslatesAutoresizingMaskIntoConstraints = false;
            contentBarView.AddSubview(UpperView);

            //  logoImg,gabanatxtImg;
            logoImg = new UIImageView();
            logoImg.TranslatesAutoresizingMaskIntoConstraints = false;
            logoImg.Image = UIImage.FromBundle("GabanaMain");
            UpperView.AddSubview(logoImg);

            gabanatxtImg = new UIImageView();
            gabanatxtImg.TranslatesAutoresizingMaskIntoConstraints = false;
            gabanatxtImg.Image = UIImage.FromBundle("GabanaTxt");
            UpperView.AddSubview(gabanatxtImg);
            #endregion

            #region PasswordView
            PasswordView = new UIView();
            PasswordView.TranslatesAutoresizingMaskIntoConstraints = false;
            PasswordView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(PasswordView);

            PassImg = new UIImageView();
            PassImg.TranslatesAutoresizingMaskIntoConstraints = false;
            PassImg.Image = UIImage.FromBundle("MenuPassword");
            PasswordView.AddSubview(PassImg);

            lblPassword = new UILabel();
            lblPassword.Text = "Change Password";
            lblPassword.TranslatesAutoresizingMaskIntoConstraints = false;
            lblPassword.TextAlignment = UITextAlignment.Left;
            lblPassword.Font = lblPassword.Font.WithSize(13);
            lblPassword.TextColor = UIColor.FromRGB(64, 64, 64);
            PasswordView.AddSubview(lblPassword);

            PasswordView.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Pass:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            PasswordView.AddGestureRecognizer(tapGesture);
            #endregion

            #region LanguageView
            LanguageView = new UIView();
            LanguageView.TranslatesAutoresizingMaskIntoConstraints = false;
            LanguageView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(LanguageView);

            LangImg = new UIImageView();
            LangImg.TranslatesAutoresizingMaskIntoConstraints = false;
            LangImg.Image = UIImage.FromBundle("MenuLanguage");
            LanguageView.AddSubview(LangImg);

            lblLang = new UILabel();
            lblLang.Text = "Language Setting";
            lblLang.TranslatesAutoresizingMaskIntoConstraints = false;
            lblLang.TextAlignment = UITextAlignment.Left;
            lblLang.Font = lblLang.Font.WithSize(13);
            lblLang.TextColor = UIColor.FromRGB(64, 64, 64);
            LanguageView.AddSubview(lblLang);

            LanguageView.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Lang:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            LanguageView.AddGestureRecognizer(tapGesture1);
            #endregion

            #region ContactUSView
            ContactUSView = new UIView();
            ContactUSView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContactUSView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(ContactUSView);

            ContactImg = new UIImageView();
            ContactImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ContactImg.Image = UIImage.FromBundle("MenuContact");
            ContactUSView.AddSubview(ContactImg);

            lblContact = new UILabel();
            lblContact.Text = "Contact Us";
            lblContact.TranslatesAutoresizingMaskIntoConstraints = false;
            lblContact.TextAlignment = UITextAlignment.Left;
            lblContact.Font = lblContact.Font.WithSize(15);
            lblContact.TextColor = UIColor.FromRGB(64, 64, 64);
            ContactUSView.AddSubview(lblContact);

            ContactUSView.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Contact:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ContactUSView.AddGestureRecognizer(tapGesture2);
            #endregion

            #region TermView
            TermView = new UIView();
            TermView.TranslatesAutoresizingMaskIntoConstraints = false;
            TermView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(TermView);

            TermImg = new UIImageView();
            TermImg.TranslatesAutoresizingMaskIntoConstraints = false;
            TermImg.Image = UIImage.FromBundle("MenuPolicy");
            TermView.AddSubview(TermImg);

            lblTerm = new UILabel();
            lblTerm.Text = "Term & Conditions";
            lblTerm.TranslatesAutoresizingMaskIntoConstraints = false;
            lblTerm.TextAlignment = UITextAlignment.Left;
            lblTerm.Font = lblTerm.Font.WithSize(15);
            lblTerm.TextColor = UIColor.FromRGB(64, 64, 64);
            TermView.AddSubview(lblTerm);

            TermView.UserInteractionEnabled = true;
            var tapGesture3 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Term:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            TermView.AddGestureRecognizer(tapGesture3);
            #endregion

            #region VersionView
            VersionView = new UIView();
            VersionView.TranslatesAutoresizingMaskIntoConstraints = false;
            VersionView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(VersionView);

            VersionImg = new UIImageView();
            VersionImg.TranslatesAutoresizingMaskIntoConstraints = false;
            VersionImg.Image = UIImage.FromBundle("MenuVersion");
            VersionView.AddSubview(VersionImg);

            lblVersion = new UILabel();
            lblVersion.Text = "Version";
            lblVersion.TranslatesAutoresizingMaskIntoConstraints = false;
            lblVersion.TextAlignment = UITextAlignment.Left;
            lblVersion.Font = lblVersion.Font.WithSize(15);
            lblVersion.TextColor = UIColor.FromRGB(64, 64, 64);
            VersionView.AddSubview(lblVersion);

            lblVersionNo = new UILabel();
            lblVersionNo.Text = "x.x.x";
            lblVersionNo.TranslatesAutoresizingMaskIntoConstraints = false;
            lblVersionNo.TextAlignment = UITextAlignment.Right;
            lblVersionNo.Font = lblVersionNo.Font.WithSize(15);
            lblVersionNo.TextColor = UIColor.FromRGB(162, 162, 162);
            VersionView.AddSubview(lblVersionNo);
            #endregion

            #region LogoutView
            LogoutView = new UIView();
            LogoutView.TranslatesAutoresizingMaskIntoConstraints = false;
            LogoutView.BackgroundColor = UIColor.White;
            contentBarView.AddSubview(LogoutView);

            LogoutImg = new UIImageView();
            LogoutImg.TranslatesAutoresizingMaskIntoConstraints = false;
            LogoutImg.Image = UIImage.FromBundle("MenuLogout");
            LogoutView.AddSubview(LogoutImg);

            lblLogout = new UILabel();
            lblLogout.Text = "Log Out";
            lblLogout.TranslatesAutoresizingMaskIntoConstraints = false;
            lblLogout.TextAlignment = UITextAlignment.Left;
            lblLogout.Font = lblLogout.Font.WithSize(15);
            lblLogout.TextColor = UIColor.FromRGB(64, 64, 64);
            LogoutView.AddSubview(lblLogout);

            LogoutView.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Logout:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            LogoutView.AddGestureRecognizer(tapGesture4);
            #endregion
        }
        void setupAutoLayout()
        {
            contentBarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            contentBarView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            contentBarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            contentBarView.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width*867)/1000).Active = true;

            #region UpperView
            UpperView.TopAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;
            UpperView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            UpperView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            UpperView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 149) / 1000).Active = true;

            logoImg.CenterYAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            logoImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 128) / 1000).Active = true;
            logoImg.LeftAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            logoImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 128) / 1000).Active = true;

            gabanatxtImg.CenterYAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            gabanatxtImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 213) / 1000).Active = true;
            gabanatxtImg.LeftAnchor.ConstraintEqualTo(logoImg.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            gabanatxtImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 45) / 1000).Active = true;
            #endregion

            #region PasswordView
            PasswordView.TopAnchor.ConstraintEqualTo(UpperView.SafeAreaLayoutGuide.BottomAnchor, 8).Active = true;
            PasswordView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            PasswordView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            PasswordView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            PassImg.CenterYAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            PassImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            PassImg.LeftAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            PassImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblPassword.CenterYAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblPassword.LeftAnchor.ConstraintEqualTo(PassImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblPassword.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion

            #region LanguageView
            LanguageView.TopAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            LanguageView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            LanguageView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            LanguageView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            LangImg.CenterYAnchor.ConstraintEqualTo(LanguageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            LangImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            LangImg.LeftAnchor.ConstraintEqualTo(LanguageView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            LangImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblLang.CenterYAnchor.ConstraintEqualTo(LanguageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblLang.LeftAnchor.ConstraintEqualTo(LangImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblLang.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion

            #region ContactUSView
            ContactUSView.TopAnchor.ConstraintEqualTo(LanguageView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            ContactUSView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            ContactUSView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            ContactUSView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            ContactImg.CenterYAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ContactImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            ContactImg.LeftAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            ContactImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblContact.CenterYAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblContact.LeftAnchor.ConstraintEqualTo(ContactImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblContact.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion

            #region TermView
            TermView.TopAnchor.ConstraintEqualTo(ContactUSView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            TermView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            TermView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            TermView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            TermImg.CenterYAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            TermImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            TermImg.LeftAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            TermImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblTerm.CenterYAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblTerm.LeftAnchor.ConstraintEqualTo(TermImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblTerm.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion

            #region VersionView
            VersionView.TopAnchor.ConstraintEqualTo(TermView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            VersionView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            VersionView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            VersionView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            VersionImg.CenterYAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            VersionImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            VersionImg.LeftAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            VersionImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblVersion.CenterYAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblVersion.LeftAnchor.ConstraintEqualTo(VersionImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblVersion.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 29) / 1000).Active = true;

            lblVersionNo.CenterYAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblVersionNo.RightAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblVersionNo.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion

            #region LogoutView
            LogoutView.TopAnchor.ConstraintEqualTo(VersionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            LogoutView.RightAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            LogoutView.LeftAnchor.ConstraintEqualTo(contentBarView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            LogoutView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 72) / 1000).Active = true;

            LogoutImg.CenterYAnchor.ConstraintEqualTo(LogoutView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            LogoutImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;
            LogoutImg.LeftAnchor.ConstraintEqualTo(LogoutView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            LogoutImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Width * 74) / 1000).Active = true;

            lblLogout.CenterYAnchor.ConstraintEqualTo(LogoutView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblLogout.LeftAnchor.ConstraintEqualTo(LogoutImg.SafeAreaLayoutGuide.RightAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblLogout.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
      
        [Export("Close:")]
        public void Close(UIGestureRecognizer sender)
        {
            DataCaching.MainNavigation.DismissViewController(true, null);
        }
        [Export("Pass:")]
        public void Pass(UIGestureRecognizer sender)
        {
            ChangePasswordController change = new ChangePasswordController();
            DataCaching.MainNavigation.DismissViewController(false, null);
            DataCaching.MainNavigation.PushViewController(change,false);
        }
        [Export("Lang:")]
        public void Lang(UIGestureRecognizer sender)
        {
            LanguageSettingController Lang = new LanguageSettingController();
            DataCaching.MainNavigation.DismissViewController(false, null);
            DataCaching.MainNavigation.PushViewController(Lang, false);
        }
        [Export("Contact:")]
        public void Contact(UIGestureRecognizer sender)
        {
            SeniorContactController Contact = new SeniorContactController();
            DataCaching.MainNavigation.DismissViewController(false, null);
            DataCaching.MainNavigation.PushViewController(Contact, false);
        }
        [Export("Term:")]
        public void Term(UIGestureRecognizer sender)
        {
            
        }
        [Export("Logout:")]
        public void Logout(UIGestureRecognizer sender)
        {
           // DataCaching.MainNavigation.DismissViewController(false, null);
            var okCancelAlertController = UIAlertController.Create("", "Are you sure you want to exit the application?" , UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => logOutSetting()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                //Present Alert

                PresentViewController(okCancelAlertController, true, null);
        }
        public async void  logOutSetting()
        {
            Preferences.Set("AppState", "logout");

            await BellNotificationHelper.UnRegisterBellNotification(GabanaAPI.gbnJWT);

            SplashLoadingController SplashLoading = new SplashLoadingController();

            UIWindow uIWindowRoot = UIApplication.SharedApplication.Windows.First();
            uIWindowRoot.RootViewController = SplashLoading;
        }

    }
}