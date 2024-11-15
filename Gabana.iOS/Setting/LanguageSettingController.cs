using Foundation;
using Gabana.iOS;
using System;
using System.Linq;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.POS
{

    public partial class LanguageSettingController : UIViewController
    {
        UIView  EngView, thaiView;
        UILabel lblEng, lblThai;
        UIImageView  EngImg, ThaiImg,selectThaiImg,selectEngImg;
        public LanguageSettingController()
        {
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.FromRGB(64,64,64)
            };
            this.NavigationController.NavigationBar.TopItem.Title = NSBundle.MainBundle.GetLocalizedString("ContactTitle", "Language Setting");
            initAttribute();
            setupAutoLayout();
            checkLanguage();

            thaiView.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("thaiView:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            thaiView.AddGestureRecognizer(tapGesture);

            EngView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("EngView:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            EngView.AddGestureRecognizer(tapGesture0);

           
        }
        void checkLanguage()
        {
            // check language here
            if(Preferences.Get("Language", "") =="th")
            {
                //thai
                selectEngImg.Hidden = false;
                selectThaiImg.Hidden = true;
            }
            else
            {
                //en
                selectEngImg.Hidden = true;
                selectThaiImg.Hidden = false;
            }
        }
        void initAttribute()
        {
            #region thaiView
            thaiView = new UIView();
            thaiView.TranslatesAutoresizingMaskIntoConstraints = false;
            thaiView.BackgroundColor = UIColor.White;
            View.AddSubview(thaiView);

            ThaiImg = new UIImageView();
            ThaiImg.Image = UIImage.FromFile("ContactUsTel.png");
            ThaiImg.TranslatesAutoresizingMaskIntoConstraints = false;
            thaiView.AddSubview(ThaiImg);

            lblThai = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblThai.Font = lblThai.Font.WithSize(15);
            lblThai.Text = "ภาษาไทย";
            thaiView.AddSubview(lblThai);

            selectThaiImg = new UIImageView();
            selectThaiImg.Image = UIImage.FromBundle("Check");
            selectThaiImg.TranslatesAutoresizingMaskIntoConstraints = false;
            thaiView.AddSubview(selectThaiImg);
            #endregion

            #region EngView
            EngView = new UIView();
            EngView.TranslatesAutoresizingMaskIntoConstraints = false;
            EngView.BackgroundColor = UIColor.White;
            View.AddSubview(EngView);

            EngImg = new UIImageView();
            EngImg.Image = UIImage.FromFile("ContactUsEmail.png");
            EngImg.TranslatesAutoresizingMaskIntoConstraints = false;
            EngView.AddSubview(EngImg);

            lblEng = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblEng.Font = lblEng.Font.WithSize(15);
            lblEng.Text = "English";
            EngView.AddSubview(lblEng);

            selectEngImg = new UIImageView();
            selectEngImg.Image = UIImage.FromBundle("Check");
            selectEngImg.TranslatesAutoresizingMaskIntoConstraints = false;
            EngView.AddSubview(selectEngImg);
            #endregion
        }
        void setupAutoLayout()
        {
            #region thaiView
            thaiView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            thaiView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            thaiView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            thaiView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            ThaiImg.CenterYAnchor.ConstraintEqualTo(thaiView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ThaiImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            ThaiImg.LeftAnchor.ConstraintEqualTo(thaiView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            ThaiImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblThai.TopAnchor.ConstraintEqualTo(thaiView.SafeAreaLayoutGuide.TopAnchor, 22).Active = true;
            lblThai.HeightAnchor.ConstraintEqualTo(21).Active = true;
            lblThai.LeftAnchor.ConstraintEqualTo(ThaiImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblThai.WidthAnchor.ConstraintEqualTo(200).Active = true;

            selectThaiImg.CenterYAnchor.ConstraintEqualTo(thaiView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            selectThaiImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            selectThaiImg.RightAnchor.ConstraintEqualTo(thaiView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            selectThaiImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region EmailView
            EngView.TopAnchor.ConstraintEqualTo(thaiView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            EngView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            EngView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            EngView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            EngImg.CenterYAnchor.ConstraintEqualTo(EngView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            EngImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            EngImg.LeftAnchor.ConstraintEqualTo(EngView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            EngImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblEng.TopAnchor.ConstraintEqualTo(EngView.SafeAreaLayoutGuide.TopAnchor, 22).Active = true;
            lblEng.HeightAnchor.ConstraintEqualTo(21).Active = true;
            lblEng.LeftAnchor.ConstraintEqualTo(EngImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblEng.WidthAnchor.ConstraintEqualTo(200).Active = true;

            selectEngImg.CenterYAnchor.ConstraintEqualTo(EngView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            selectEngImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            selectEngImg.RightAnchor.ConstraintEqualTo(EngView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            selectEngImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            #endregion
        }

        [Export("EngView:")]
        public void English(UIGestureRecognizer sender)
        {
            selectEngImg.Hidden = false;
            selectThaiImg.Hidden = true;
            //Preferences.Set("Language", "eng");
            //SplashLoadingController SplashLoading = new SplashLoadingController();
            //UIWindow uIWindowRoot = UIApplication.SharedApplication.Windows.First();
            //uIWindowRoot.RootViewController = SplashLoading;
        }
        [Export("thaiView:")]
        public async void thai(UIGestureRecognizer sender)
        {
            selectEngImg.Hidden = true;
            selectThaiImg.Hidden = false;
            //Preferences.Set("Language", "th");
            //SplashLoadingController SplashLoading = new SplashLoadingController();
            //UIWindow uIWindowRoot = UIApplication.SharedApplication.Windows.First();
            //uIWindowRoot.RootViewController = SplashLoading;
        }
        
    }
}