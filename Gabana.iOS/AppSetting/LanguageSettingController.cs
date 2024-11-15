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
        UIView bottomView;
        UIButton btnSelect;
        UILabel lblEng, lblThai;
        UIImageView  EngImg, ThaiImg,selectThaiImg,selectEngImg;
        bool isThai=false;
        string initButton = null;
        string select = null;
        public LanguageSettingController()
        {
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
        }
        public async override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                this.NavigationController.SetNavigationBarHidden(false, false);
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
            catch (Exception ex)
            {
                await TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);
            }
           
           
        }
        void checkLanguage()
        {
            // check language here
            initButton = Preferences.Get("Language", "");
            if (initButton.ToLower() == "th")
            {
                //thai
                selectEngImg.Hidden = true;
                selectThaiImg.Hidden = false;
                isThai = true;
            }
            else
            {
                //en
                selectEngImg.Hidden = false;
                selectThaiImg.Hidden = true;
                isThai = false;
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
            ThaiImg.Image = UIImage.FromBundle("LangTH");
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
            EngImg.Image = UIImage.FromBundle("LangEN");
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

            #region BottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomView.BackgroundColor = UIColor.FromRGB(248,248,248);
            View.AddSubview(bottomView);

            btnSelect = new UIButton();
            btnSelect.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
            btnSelect.Layer.BorderColor = UIColor.FromRGB(0,149,218).CGColor;
            btnSelect.BackgroundColor = UIColor.White;
            btnSelect.Layer.CornerRadius = 5f;
            btnSelect.Layer.BorderWidth = 0.5f;
            btnSelect.Enabled = false;
            btnSelect.SetTitle(" Save ", UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                SaveLanguage();
            };
            View.AddSubview(btnSelect);
            #endregion
        }
        void SaveLanguage()
        {
            if(isThai)
            {
                Preferences.Set("Language", "th");
            }
            else
            {
                Preferences.Set("Language", "eng");
            }
            SplashLoadingController SplashLoading = new SplashLoadingController();
            UIWindow uIWindowRoot = UIApplication.SharedApplication.Windows.First();
            uIWindowRoot.RootViewController = SplashLoading;
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
            selectThaiImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            selectThaiImg.RightAnchor.ConstraintEqualTo(thaiView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            selectThaiImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region EngView
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
            selectEngImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            selectEngImg.RightAnchor.ConstraintEqualTo(EngView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            selectEngImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnSelect.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSelect.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnSelect.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSelect.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
        }

        [Export("EngView:")]
        public void English(UIGestureRecognizer sender)
        {
            select = "eng";
            selectEngImg.Hidden = false;
            selectThaiImg.Hidden = true;

            isThai = false;
            if(initButton.ToLower() != select)
            {
                btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                btnSelect.Enabled = true;
            }
            else
            {
                btnSelect.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelect.BackgroundColor = UIColor.White;
                btnSelect.Enabled = false;
            }
        }
        [Export("thaiView:")]
        public async void thai(UIGestureRecognizer sender)
        {
            select = "th";
            selectEngImg.Hidden = true;
            selectThaiImg.Hidden = false;

            isThai = true; ;
            if (initButton.ToLower() != select)
            {
                btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                btnSelect.Enabled = true;
            }
            else
            {
                btnSelect.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelect.BackgroundColor = UIColor.White;
                btnSelect.Enabled = false;
            }
        }
        
    }
}