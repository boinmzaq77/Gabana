using Foundation;
using System;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.POS
{

    public partial class SeniorContactController : UIViewController
    {
        UIView  WebView, TelView, EmailView, LineView, FacebookView, LocationView;
        UILabel lblTel, lblEmail, lblLine, lblFacebook, lblWeb, lblLocation;
        UIImageView TelImg, EmailImg, LineImg, FacebookImg, WebImg, LocationImg;
        public SeniorContactController()
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
            this.NavigationController.NavigationBar.TopItem.Title = NSBundle.MainBundle.GetLocalizedString("ContactTitle", "Contact Us");
            initAttribute();
            setupAutoLayout();

            LocationView.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("location:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            LocationView.AddGestureRecognizer(tapGesture);

            TelView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Tel:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            TelView.AddGestureRecognizer(tapGesture0);

            EmailView.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Email:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            EmailView.AddGestureRecognizer(tapGesture1);

            LineView.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Line:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            LineView.AddGestureRecognizer(tapGesture2);

            FacebookView.UserInteractionEnabled = true;
            var tapGesture3 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Facebook:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            FacebookView.AddGestureRecognizer(tapGesture3);

            WebView.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Website:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            WebView.AddGestureRecognizer(tapGesture4);
        }
        void initAttribute()
        {
            #region TelView
            TelView = new UIView();
            TelView.TranslatesAutoresizingMaskIntoConstraints = false;
            TelView.BackgroundColor = UIColor.White;
            View.AddSubview(TelView);

            TelImg = new UIImageView();
            TelImg.Image = UIImage.FromFile("ContactUsTel.png");
            TelImg.TranslatesAutoresizingMaskIntoConstraints = false;
            TelView.AddSubview(TelImg);

            lblTel = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblTel.Font = lblTel.Font.WithSize(15);
            lblTel.Text = "02-692-5899";
            TelView.AddSubview(lblTel);
            #endregion

            #region EmailView
            EmailView = new UIView();
            EmailView.TranslatesAutoresizingMaskIntoConstraints = false;
            EmailView.BackgroundColor = UIColor.White;
            View.AddSubview(EmailView);

            EmailImg = new UIImageView();
            EmailImg.Image = UIImage.FromFile("ContactUsEmail.png");
            EmailImg.TranslatesAutoresizingMaskIntoConstraints = false;
            EmailView.AddSubview(EmailImg);

            lblEmail = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblEmail.Font = lblEmail.Font.WithSize(15);
            lblEmail.Text = "giftory@seniorsoft.co.th";
            EmailView.AddSubview(lblEmail);
            #endregion

            #region LineView
            LineView = new UIView();
            LineView.TranslatesAutoresizingMaskIntoConstraints = false;
            LineView.BackgroundColor = UIColor.White;
            View.AddSubview(LineView);

            LineImg = new UIImageView();
            LineImg.Image = UIImage.FromFile("ContactUsLine.png");
            LineImg.TranslatesAutoresizingMaskIntoConstraints = false;
            LineView.AddSubview(LineImg);

            lblLine = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblLine.Font = lblLine.Font.WithSize(15);
            lblLine.Text = "seniorsoft";
            LineView.AddSubview(lblLine);
            #endregion

            #region FacebookView
            FacebookView = new UIView();
            FacebookView.TranslatesAutoresizingMaskIntoConstraints = false;
            FacebookView.BackgroundColor = UIColor.White;
            View.AddSubview(FacebookView);

            FacebookImg = new UIImageView();
            FacebookImg.Image = UIImage.FromFile("ContactUsFB.png");
            FacebookImg.TranslatesAutoresizingMaskIntoConstraints = false;
            FacebookView.AddSubview(FacebookImg);

            lblFacebook = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblFacebook.Font = lblFacebook.Font.WithSize(15);
            lblFacebook.Text = "Seniorsoft Development";
            FacebookView.AddSubview(lblFacebook);
            #endregion

            #region WebView
            WebView = new UIView();
            WebView.TranslatesAutoresizingMaskIntoConstraints = false;
            WebView.BackgroundColor = UIColor.White;
            View.AddSubview(WebView);

            WebImg = new UIImageView();
            WebImg.Image = UIImage.FromFile("ContactUsSNS.png");
            WebImg.TranslatesAutoresizingMaskIntoConstraints = false;
            WebView.AddSubview(WebImg);

            lblWeb = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblWeb.Font = lblWeb.Font.WithSize(15);
            lblWeb.Text = "Website";
            WebView.AddSubview(lblWeb);
            #endregion

            #region LocationView
            LocationView = new UIView();
            LocationView.TranslatesAutoresizingMaskIntoConstraints = false;
            LocationView.BackgroundColor = UIColor.White;
            View.AddSubview(LocationView);

            LocationImg = new UIImageView();
            LocationImg.Image = UIImage.FromFile("ContactUsAddress.png");
            LocationImg.TranslatesAutoresizingMaskIntoConstraints = false;
            LocationView.AddSubview(LocationImg);

            lblLocation = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblLocation.LineBreakMode = UILineBreakMode.WordWrap;
            lblLocation.Lines = 3;
            lblLocation.Text = "200 Thosapol Building 1CD floor, Ratchada Road, Huai khwang District Bangkok 10310";
            lblLocation.Font = lblLocation.Font.WithSize(12);
            LocationView.AddSubview(lblLocation);

            LocationView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("location:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            LocationView.AddGestureRecognizer(tapGesture0);
            #endregion
        }
        void setupAutoLayout()
        {
            #region TelView
            TelView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            TelView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            TelView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            TelView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            TelImg.CenterYAnchor.ConstraintEqualTo(TelView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            TelImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            TelImg.LeftAnchor.ConstraintEqualTo(TelView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            TelImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblTel.TopAnchor.ConstraintEqualTo(TelView.SafeAreaLayoutGuide.TopAnchor, 22).Active = true;
            lblTel.HeightAnchor.ConstraintEqualTo(21).Active = true;
            lblTel.LeftAnchor.ConstraintEqualTo(TelImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblTel.WidthAnchor.ConstraintEqualTo(200).Active = true;
            #endregion

            #region EmailView
            EmailView.TopAnchor.ConstraintEqualTo(TelView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            EmailView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            EmailView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            EmailView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            EmailImg.CenterYAnchor.ConstraintEqualTo(EmailView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            EmailImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            EmailImg.LeftAnchor.ConstraintEqualTo(EmailView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            EmailImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblEmail.TopAnchor.ConstraintEqualTo(EmailView.SafeAreaLayoutGuide.TopAnchor, 22).Active = true;
            lblEmail.HeightAnchor.ConstraintEqualTo(21).Active = true;
            lblEmail.LeftAnchor.ConstraintEqualTo(EmailImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblEmail.WidthAnchor.ConstraintEqualTo(200).Active = true;
            #endregion

            #region LineView
            LineView.TopAnchor.ConstraintEqualTo(EmailView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            LineView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            LineView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            LineView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            LineImg.CenterYAnchor.ConstraintEqualTo(LineView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            LineImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            LineImg.LeftAnchor.ConstraintEqualTo(LineView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            LineImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblLine.TopAnchor.ConstraintEqualTo(LineView.SafeAreaLayoutGuide.TopAnchor, 22).Active = true;
            lblLine.HeightAnchor.ConstraintEqualTo(21).Active = true;
            lblLine.LeftAnchor.ConstraintEqualTo(LineImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblLine.WidthAnchor.ConstraintEqualTo(200).Active = true;
            #endregion

            #region FacebookView
            FacebookView.TopAnchor.ConstraintEqualTo(LineView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            FacebookView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            FacebookView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            FacebookView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            FacebookImg.CenterYAnchor.ConstraintEqualTo(FacebookView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            FacebookImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            FacebookImg.LeftAnchor.ConstraintEqualTo(FacebookView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            FacebookImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblFacebook.TopAnchor.ConstraintEqualTo(FacebookView.SafeAreaLayoutGuide.TopAnchor, 22).Active = true;
            lblFacebook.HeightAnchor.ConstraintEqualTo(21).Active = true;
            lblFacebook.LeftAnchor.ConstraintEqualTo(FacebookImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblFacebook.WidthAnchor.ConstraintEqualTo(200).Active = true;
            #endregion

            #region WebView
            WebView.TopAnchor.ConstraintEqualTo(FacebookView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            WebView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            WebView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            WebView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            WebImg.CenterYAnchor.ConstraintEqualTo(WebView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            WebImg.HeightAnchor.ConstraintEqualTo(32).Active = true;
            WebImg.LeftAnchor.ConstraintEqualTo(WebView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            WebImg.WidthAnchor.ConstraintEqualTo(32).Active = true;

            lblWeb.TopAnchor.ConstraintEqualTo(WebView.SafeAreaLayoutGuide.TopAnchor, 22).Active = true;
            lblWeb.HeightAnchor.ConstraintEqualTo(21).Active = true;
            lblWeb.LeftAnchor.ConstraintEqualTo(WebImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblWeb.WidthAnchor.ConstraintEqualTo(200).Active = true;
            #endregion

            #region LocationView
            LocationView.TopAnchor.ConstraintEqualTo(WebView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            LocationView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            LocationView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            LocationView.HeightAnchor.ConstraintEqualTo(59).Active = true;

            LocationImg.CenterYAnchor.ConstraintEqualTo(LocationView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            LocationImg.LeftAnchor.ConstraintEqualTo(LocationView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            LocationImg.WidthAnchor.ConstraintEqualTo(32).Active = true;
            LocationImg.HeightAnchor.ConstraintEqualTo(32).Active = true;

            lblLocation.CenterYAnchor.ConstraintEqualTo(LocationView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblLocation.LeftAnchor.ConstraintEqualTo(LocationImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblLocation.WidthAnchor.ConstraintEqualTo(275).Active = true;
            lblLocation.HeightAnchor.ConstraintEqualTo(55).Active = true;
            #endregion
        }
        [Export("Tel:")]
        public void Tel(UIGestureRecognizer sender)
        {
            try
            {
                PhoneDialer.Open("026925899");
            }
            catch (Exception)
            {
                return;
            }
        }
        [Export("Email:")]
        public void Email(UIGestureRecognizer sender)
        {
            try
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://mail.google.com/mail/u/0/?view=cm&fs=1&tf=1&source=mailto&to=" + "giftory@seniorsoft.co.th"));
            }
            catch (Exception)
            {
                return;
            }
        }
        [Export("location:")]
        public async void location(UIGestureRecognizer sender)
        {
            try
            {
                //double latitud = Convert.ToDouble(MemberCardDetail.Lat);
                //double longitud = Convert.ToDouble(MemberCardDetail.Long);
                //string placeName = MemberCardDetail.Name;

                //var supportsUri = await Launcher.CanOpenAsync("comgooglemaps://");

                //if (supportsUri)
                //    await Launcher.OpenAsync($"comgooglemaps://?q={latitud},{longitud}({placeName})");

                //else
                //    await Map.OpenAsync(latitud, longitud, new MapLaunchOptions { Name = placeName });

            }
            catch (Exception)
            {
                return;
            }
        }
        [Export("Line:")]
        public void Line(UIGestureRecognizer sender)
        {
            try
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("line://seniorsoft"));
            }
            catch (Exception)
            {
                return;
            }
        }
        [Export("Facebook:")]
        public void Facebook(UIGestureRecognizer sender)
        {
            try
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://www.facebook.com/seniorsoft"));
            }
            catch (Exception)
            {
                return;
            }
        }
        [Export("Website:")]
        public void Website(UIGestureRecognizer sender)
        {
            try
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://www.seniorsoft.co.th/"));
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}