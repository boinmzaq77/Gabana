using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class DecimalDetailController : UIViewController
    {
        UIView Decimal0View, Decimal1View, Decimal2View, Decimal3View, Decimal4View, Decimal5View, _contentView;
        UIButton btnSelectType;
        UIScrollView _scrollView;
        UILabel lbl_Decimal0_head, lbl_Decimal1_head, lbl_Decimal2_head, lbl_Decimal3_head, lbl_Decimal4_head, lbl_Decimal5_head;
        UILabel lbl_Decimal0, lbl_Decimal1, lbl_Decimal2, lbl_Decimal3, lbl_Decimal4, lbl_Decimal5;
        UIImageView Decimal0_img, Decimal1_img, Decimal2_img, Decimal3_img, Decimal4_img, Decimal5_img;
        UILabel lbl_Decimal4_multiply;

        UILabel lblD1_use,lblD2_use,lblD3_use,lblD4_use,lblD5_use;

        int selectdigi = 0;
        int firstSelect = 0;
        private UITextField txtx;

        public DecimalDetailController(int select)
        {
            selectdigi = select;
            firstSelect = select;
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public async override void ViewDidLoad()
        {
            try
            {

                Utils.SetTitle(this.NavigationController,Utils.TextBundle("roundtype", "Rounding Type") );
                //this.NavigationController.NavigationBar.TopItem.Title = "รูปแบบการปัดเศษ";
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248,248,248);
                initAttribute();
                setSelect();
                setupAutoLayout();
                Textboxfocus(View);
                Setkeyboard();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
           
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        void setSelect()
        {
            if (selectdigi == 0)
            {
                Decimal0_img.Hidden = false;
                Decimal1_img.Hidden = true;
                Decimal2_img.Hidden = true;
                Decimal3_img.Hidden = true;
                Decimal4_img.Hidden = true;
                Decimal5_img.Hidden = true;
            }
            else if (selectdigi == 1)
            {
                Decimal0_img.Hidden = true;
                Decimal1_img.Hidden = false;
                Decimal2_img.Hidden = true;
                Decimal3_img.Hidden = true;
                Decimal4_img.Hidden = true;
                Decimal5_img.Hidden = true;

            }
            else if (selectdigi == 2)
            {
                Decimal0_img.Hidden = true;
                Decimal1_img.Hidden = true;
                Decimal2_img.Hidden = false;
                Decimal3_img.Hidden = true;
                Decimal4_img.Hidden = true;
                Decimal5_img.Hidden = true;
            }
            else if (selectdigi == 3)
            {
                Decimal0_img.Hidden = true;
                Decimal1_img.Hidden = true;
                Decimal2_img.Hidden = true;
                Decimal3_img.Hidden = false;
                Decimal4_img.Hidden = true;
                Decimal5_img.Hidden = true;
            }
            else if (selectdigi == 4)
            {
                Decimal0_img.Hidden = true;
                Decimal1_img.Hidden = true;
                Decimal2_img.Hidden = true;
                Decimal3_img.Hidden = true;
                Decimal4_img.Hidden = false;
                var multiply = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT;
                if (!string.IsNullOrEmpty(multiply))
                {
                    txtx.Text = multiply;
                }
                txtx.Hidden = false;
                Decimal5_img.Hidden = true;
            }
            else if (selectdigi == 5)
            {
                Decimal0_img.Hidden = true;
                Decimal1_img.Hidden = true;
                Decimal2_img.Hidden = true;
                Decimal3_img.Hidden = true;
                Decimal4_img.Hidden = true;
                Decimal5_img.Hidden = false;
            }
        }
        void CheckBtn()
        {
            // firstSelect
            if(selectdigi == firstSelect)
            {
                btnSelectType.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelectType.BackgroundColor = UIColor.White;
                btnSelectType.Enabled = false;
            }
            else
            {
                btnSelectType.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSelectType.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                btnSelectType.Enabled = true;
            }
        }
        #region btnClick
        [Export("Digi0:")]
        public void Digi0(UIGestureRecognizer sender)
        {
            selectdigi = 0;
            setSelect();
            CheckBtn();

        }
        [Export("Digi1:")]
        public void Digi1(UIGestureRecognizer sender)
        {
            selectdigi = 1;
            setSelect();
            CheckBtn();
        }
        [Export("Digi2:")]
        public void Digi2(UIGestureRecognizer sender)
        {
            selectdigi = 2;
            setSelect();
            CheckBtn();
        }
        [Export("Digi3:")]
        public void Digi3(UIGestureRecognizer sender)
        {
            selectdigi = 3;
            setSelect();
            CheckBtn();
        }
        [Export("Digi4:")]
        public void Digi4(UIGestureRecognizer sender)
        {
            selectdigi = 4;
            setSelect();
            CheckBtn();
        }
        [Export("Digi5:")]
        public void Digi5(UIGestureRecognizer sender)
        {
            selectdigi = 5;
            setSelect();
            CheckBtn();
        }

        #endregion
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
        public void OnKeyboardChanged(UIView view, bool visible, nfloat nfloat)
        {
            if (!visible)
                view.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);
            else

                view.Frame = new CGRect(0, 0 - 100, view.Frame.Width, view.Frame.Height);
        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248,248,248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region View Layout

            #region Decimal0View
            Decimal0View = new UIView();
            Decimal0View.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal0View.BackgroundColor = UIColor.White;
            Decimal0View.Layer.ShadowColor = UIColor.FromRGB(248,248,248).CGColor;
            Decimal0View.Layer.ShadowOpacity = 1;
            //  OwnerView.Layer.CornerRadius = 7;
            //   OwnerView.ClipsToBounds = true;
            Decimal0View.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            Decimal0View.Layer.MasksToBounds = false;

            lbl_Decimal0_head = new UILabel
            {
                TextColor = UIColor.FromRGB(0,149,218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal0_head.Font = lbl_Decimal0_head.Font.WithSize(16);
            lbl_Decimal0_head.Text = Utils.TextBundle("notrounded", "Not Rounded");
            Decimal0View.AddSubview(lbl_Decimal0_head);

            lbl_Decimal0 = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal0.Font = lbl_Decimal0.Font.WithSize(12);
            lbl_Decimal0.Text = "เช่น 3.25 ไม่ปัดเศษเป็น 3.25";
            Decimal0View.AddSubview(lbl_Decimal0);

            Decimal0_img = new UIImageView();
            Decimal0_img.Image = UIImage.FromBundle("Check");
            Decimal0_img.Hidden = true;
            Decimal0_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal0View.AddSubview(Decimal0_img);

            Decimal0View.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Digi0:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Decimal0View.AddGestureRecognizer(tapGesture0);
            #endregion

            #region Decimal1View
            Decimal1View = new UIView();
            Decimal1View.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal1View.BackgroundColor = UIColor.White;
            Decimal1View.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            Decimal1View.Layer.ShadowOpacity = 1;
            //  OwnerView.Layer.CornerRadius = 7;
            //   OwnerView.ClipsToBounds = true;
            Decimal1View.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            Decimal1View.Layer.MasksToBounds = false;

            lbl_Decimal1_head = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal1_head.Font = lbl_Decimal1_head.Font.WithSize(16);
            lbl_Decimal1_head.Text = Utils.TextBundle("option1", "Option 1");
            Decimal1View.AddSubview(lbl_Decimal1_head);

            lbl_Decimal1 = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal1.Lines = 4;
            lbl_Decimal1.Font = lbl_Decimal1.Font.WithSize(12);
            lbl_Decimal1.Text = "0.00 < X <= 0.25 = 0.25\n" +
                                "0.25 < X <= 0.50 = 0.50\n" +
                                "0.50 < X <= 0.75 = 0.75\n" +
                                "X > 0.75 = 1";
            Decimal1View.AddSubview(lbl_Decimal1);


            lblD1_use = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblD1_use.Lines = 4;
            lblD1_use.Font = lblD1_use.Font.WithSize(12);
            lblD1_use.Text = "เช่น 3.15 ปัดเศษเป็น 3.25\n" +
                             "เช่น 3.35 ปัดเศษเป็น 3.50\n" +
                             "เช่น 3.55 ปัดเศษเป็น 3.75\n" +
                              "เช่น 3.85 ปัดเศษเป็น 4.00";
            Decimal1View.AddSubview(lblD1_use);


            Decimal1_img = new UIImageView();
            Decimal1_img.Image = UIImage.FromBundle("Check");
            Decimal1_img.Hidden = true;
            Decimal1_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal1View.AddSubview(Decimal1_img);

            Decimal1View.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Digi1:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Decimal1View.AddGestureRecognizer(tapGesture1);
            #endregion

            #region Decimal2View
            Decimal2View = new UIView();
            Decimal2View.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal2View.BackgroundColor = UIColor.White;
            Decimal2View.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            Decimal2View.Layer.ShadowOpacity = 1;
            //  OwnerView.Layer.CornerRadius = 7;
            //   OwnerView.ClipsToBounds = true;
            Decimal2View.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            Decimal2View.Layer.MasksToBounds = false;

            lbl_Decimal2_head = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal2_head.Font = lbl_Decimal2_head.Font.WithSize(16);
            lbl_Decimal2_head.Text = Utils.TextBundle("option2", "Type");
            Decimal2View.AddSubview(lbl_Decimal2_head);

            lbl_Decimal2 = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal2.Font = lbl_Decimal2.Font.WithSize(12);
            lbl_Decimal2.Text = "0 < X < 1 = 1";
            Decimal2View.AddSubview(lbl_Decimal2);

            lblD2_use = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblD2_use.Font = lblD2_use.Font.WithSize(12);
            lblD2_use.Text = "เช่น 3.25 ปัดเศษเป็น 4.00";
            Decimal2View.AddSubview(lblD2_use);

            Decimal2_img = new UIImageView();
            Decimal2_img.Image = UIImage.FromBundle("Check");
            Decimal2_img.Hidden = true;
            Decimal2_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal2View.AddSubview(Decimal2_img);

            Decimal2View.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Digi2:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Decimal2View.AddGestureRecognizer(tapGesture2);

            #endregion

            #region Decimal3View
            Decimal3View = new UIView();
            Decimal3View.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal3View.BackgroundColor = UIColor.White;
            Decimal3View.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            Decimal3View.Layer.ShadowOpacity = 1;
            //  OwnerView.Layer.CornerRadius = 7;
            //   OwnerView.ClipsToBounds = true;
            Decimal3View.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            Decimal3View.Layer.MasksToBounds = false;

            lbl_Decimal3_head = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal3_head.Font = lbl_Decimal3_head.Font.WithSize(16);
            lbl_Decimal3_head.Text = Utils.TextBundle("option3", "Type");
            Decimal3View.AddSubview(lbl_Decimal3_head);

            lbl_Decimal3 = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal3.Lines = 2;
            lbl_Decimal3.Font = lbl_Decimal3.Font.WithSize(12);
            lbl_Decimal3.Text = "0 < X < 0.5 = 0\n" +
                                 "X >= 0.5 = 1";
            Decimal3View.AddSubview(lbl_Decimal3);

            lblD3_use = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblD3_use.Lines = 2;
            lblD3_use.Font = lblD3_use.Font.WithSize(12);
            lblD3_use.Text = "เช่น 3.35 ปัดเศษเป็น 3.00\nเช่น 3.55 ปัดเศษเป็น 4.00";
            Decimal3View.AddSubview(lblD3_use);

            Decimal3_img = new UIImageView();
            Decimal3_img.Image = UIImage.FromBundle("Check");
            Decimal3_img.Hidden = true;
            Decimal3_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal3View.AddSubview(Decimal3_img);

            Decimal3View.UserInteractionEnabled = true;
            var tapGesture3 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Digi3:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Decimal3View.AddGestureRecognizer(tapGesture3);
            #endregion

            #region Decimal4View
            Decimal4View = new UIView();
            Decimal4View.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal4View.BackgroundColor = UIColor.White;
            Decimal4View.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            Decimal4View.Layer.ShadowOpacity = 1;
            //  OwnerView.Layer.CornerRadius = 7;
            //   OwnerView.ClipsToBounds = true;
            Decimal4View.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            Decimal4View.Layer.MasksToBounds = false;

            lbl_Decimal4_head = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal4_head.Font = lbl_Decimal3_head.Font.WithSize(16);
            lbl_Decimal4_head.Text = Utils.TextBundle("option4", "Type");
            Decimal4View.AddSubview(lbl_Decimal4_head);

            lbl_Decimal4_multiply = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal4_multiply.Font = lbl_Decimal4_multiply.Font.WithSize(16);
            lbl_Decimal4_multiply.Text = "ตัวคูณ";
            Decimal4View.AddSubview(lbl_Decimal4_multiply);

            txtx = new UITextField
            {
                
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtx.KeyboardType = UIKeyboardType.PhonePad;
            txtx.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            txtx.BackgroundColor = UIColor.White;
            txtx.Layer.CornerRadius = 5f;
            txtx.TextAlignment = UITextAlignment.Center;
            txtx.Layer.BorderWidth = 0.5f;
            txtx.Hidden = true;
            //txtx.AttributedPlaceholder = new NSAttributedString("Customer ID", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtx.Font = txtx.Font.WithSize(15);
            txtx.ReturnKeyType = UIReturnKeyType.Done;
            txtx.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            Decimal4View.AddSubview(txtx);

            lbl_Decimal4 = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal4.Lines = 2;
            lbl_Decimal4.Font = lbl_Decimal4.Font.WithSize(12);
            lbl_Decimal4.Text = "0 < X < 0.5 = 1\n" +
                                "0.5 < X < 1";
            Decimal4View.AddSubview(lbl_Decimal4);

            lblD4_use = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblD4_use.Lines = 2;
            lblD4_use.Font = lblD4_use.Font.WithSize(12);
            lblD4_use.Text = "เช่น ตัวคูณ 10, 4.00 ปัดเศษเป็น 0.00\n"+
                             "เช่น ตัวคูณ 10, 5.00 ปัดเศษเป็น 10.00";
            Decimal4View.AddSubview(lblD4_use);

            Decimal4_img = new UIImageView();
            Decimal4_img.Image = UIImage.FromBundle("Check");
            Decimal4_img.Hidden = true;
            Decimal4_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal4View.AddSubview(Decimal4_img);

            Decimal4View.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Digi4:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Decimal4View.AddGestureRecognizer(tapGesture4);
            #endregion

            #region Decimal5View
            Decimal5View = new UIView();
            Decimal5View.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal5View.BackgroundColor = UIColor.White;
            Decimal5View.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            Decimal5View.Layer.ShadowOpacity = 1;
            //  OwnerView.Layer.CornerRadius = 7;
            //   OwnerView.ClipsToBounds = true;
            Decimal5View.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            Decimal5View.Layer.MasksToBounds = false;

            lbl_Decimal5_head = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal5_head.Font = lbl_Decimal5_head.Font.WithSize(16);
            lbl_Decimal5_head.Text = Utils.TextBundle("option5", "Type");
            Decimal5View.AddSubview(lbl_Decimal5_head);

            lbl_Decimal5 = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Decimal5.Font = lbl_Decimal5.Font.WithSize(12);
            lbl_Decimal5.Text = "X < 1.9 = 1 ";
            Decimal5View.AddSubview(lbl_Decimal5);

            lblD5_use = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblD5_use.Font = lblD5_use.Font.WithSize(12);
            lblD5_use.Text = "เช่น 3.90 ปัดเศษเป็น 3.00";
            Decimal5View.AddSubview(lblD5_use);

            Decimal5_img = new UIImageView();
            Decimal5_img.Image = UIImage.FromBundle("Check");
            Decimal5_img.Hidden = true;
            Decimal5_img.TranslatesAutoresizingMaskIntoConstraints = false;
            Decimal5View.AddSubview(Decimal5_img);

            Decimal5View.UserInteractionEnabled = true;
            var tapGesture5 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Digi5:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            Decimal5View.AddGestureRecognizer(tapGesture5);
            #endregion

            #endregion

            _contentView.AddSubview(Decimal0View);
            _contentView.AddSubview(Decimal1View);
            _contentView.AddSubview(Decimal2View);
            _contentView.AddSubview(Decimal3View);
            _contentView.AddSubview(Decimal4View);
            _contentView.AddSubview(Decimal5View);

            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);

            btnSelectType = new UIButton();
            btnSelectType.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnSelectType.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSelectType.BackgroundColor = UIColor.White;
            btnSelectType.Layer.CornerRadius = 5f;
            btnSelectType.Layer.BorderWidth = 0.5f;
            btnSelectType.Enabled = false;
            btnSelectType.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSelectType.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectType.TouchUpInside += (sender, e) => {
                DecimalController.DigiCutTxtSelect = selectdigi;
                DecimalController.multiply = txtx.Text;
                DecimalController.isModifyDigi = true;
                this.NavigationController.PopViewController(false);
            };
            View.AddSubview(btnSelectType);
        }
        void setupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(btnSelectType.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region Decimal0View
            Decimal0View.TopAnchor.ConstraintEqualTo(Decimal0View.Superview.TopAnchor, 10).Active = true;
            Decimal0View.LeftAnchor.ConstraintEqualTo(Decimal0View.Superview.LeftAnchor, 10).Active = true;
            Decimal0View.RightAnchor.ConstraintEqualTo(Decimal0View.Superview.RightAnchor, -10).Active = true;
            Decimal0View.HeightAnchor.ConstraintEqualTo(70).Active = true;


            lbl_Decimal0_head.TopAnchor.ConstraintEqualTo(lbl_Decimal0_head.Superview.TopAnchor, 14).Active = true;
            lbl_Decimal0_head.LeftAnchor.ConstraintEqualTo(lbl_Decimal0_head.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal0_head.RightAnchor.ConstraintEqualTo(Decimal0_img.SafeAreaLayoutGuide.LeftAnchor,-15).Active = true;
            lbl_Decimal0_head.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lbl_Decimal0.TopAnchor.ConstraintEqualTo(lbl_Decimal0_head.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_Decimal0.LeftAnchor.ConstraintEqualTo(lbl_Decimal0_head.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal0.RightAnchor.ConstraintEqualTo(Decimal0_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lbl_Decimal0.HeightAnchor.ConstraintEqualTo(17).Active = true;

            Decimal0_img.TopAnchor.ConstraintEqualTo(Decimal0_img.Superview.TopAnchor, 10).Active = true;
            Decimal0_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            Decimal0_img.RightAnchor.ConstraintEqualTo(Decimal0_img.Superview.RightAnchor, -15).Active = true;
            Decimal0_img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            #endregion

            #region Decimal1View
            Decimal1View.TopAnchor.ConstraintEqualTo(Decimal0View.BottomAnchor, 10).Active = true;
            Decimal1View.LeftAnchor.ConstraintEqualTo(Decimal1View.Superview.LeftAnchor, 10).Active = true;
            Decimal1View.RightAnchor.ConstraintEqualTo(Decimal1View.Superview.RightAnchor, -10).Active = true;
            Decimal1View.HeightAnchor.ConstraintEqualTo(120).Active = true;


            lbl_Decimal1_head.TopAnchor.ConstraintEqualTo(lbl_Decimal1_head.Superview.TopAnchor, 10).Active = true;
            lbl_Decimal1_head.LeftAnchor.ConstraintEqualTo(lbl_Decimal1_head.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal1_head.RightAnchor.ConstraintEqualTo(Decimal1_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lbl_Decimal1_head.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lbl_Decimal1.TopAnchor.ConstraintEqualTo(lbl_Decimal1_head.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lbl_Decimal1.LeftAnchor.ConstraintEqualTo(lbl_Decimal1.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal1.RightAnchor.ConstraintEqualTo(lblD1_use.Superview.CenterXAnchor, -25).Active = true;
            lbl_Decimal1.HeightAnchor.ConstraintEqualTo(76).Active = true;

            lblD1_use.TopAnchor.ConstraintEqualTo(lbl_Decimal1_head.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblD1_use.LeftAnchor.ConstraintEqualTo(lblD1_use.Superview.CenterXAnchor, -25).Active = true;
            lblD1_use.RightAnchor.ConstraintEqualTo(Decimal1_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lblD1_use.HeightAnchor.ConstraintEqualTo(76).Active = true;

            Decimal1_img.TopAnchor.ConstraintEqualTo(Decimal1_img.Superview.TopAnchor, 10).Active = true;
            Decimal1_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            Decimal1_img.RightAnchor.ConstraintEqualTo(Decimal1_img.Superview.RightAnchor, -15).Active = true;
            Decimal1_img.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region Decimal2View
            Decimal2View.TopAnchor.ConstraintEqualTo(Decimal1View.BottomAnchor, 10).Active = true;
            Decimal2View.LeftAnchor.ConstraintEqualTo(Decimal2View.Superview.LeftAnchor, 10).Active = true;
            Decimal2View.RightAnchor.ConstraintEqualTo(Decimal2View.Superview.RightAnchor, -10).Active = true;
            Decimal2View.HeightAnchor.ConstraintEqualTo(70).Active = true;


            lbl_Decimal2_head.TopAnchor.ConstraintEqualTo(lbl_Decimal2_head.Superview.TopAnchor, 15).Active = true;
            lbl_Decimal2_head.LeftAnchor.ConstraintEqualTo(lbl_Decimal2_head.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal2_head.RightAnchor.ConstraintEqualTo(Decimal2_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lbl_Decimal2_head.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lbl_Decimal2.TopAnchor.ConstraintEqualTo(lbl_Decimal2_head.BottomAnchor, 4).Active = true;
            lbl_Decimal2.LeftAnchor.ConstraintEqualTo(lbl_Decimal2.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal2.RightAnchor.ConstraintEqualTo(lblD2_use.Superview.CenterXAnchor, -25).Active = true;
            lbl_Decimal2.HeightAnchor.ConstraintEqualTo(17).Active = true;

            lblD2_use.TopAnchor.ConstraintEqualTo(lbl_Decimal2_head.BottomAnchor, 4).Active = true;
            lblD2_use.LeftAnchor.ConstraintEqualTo(lblD2_use.Superview.CenterXAnchor, -25).Active = true;
            lblD2_use.RightAnchor.ConstraintEqualTo(Decimal2_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lblD2_use.HeightAnchor.ConstraintEqualTo(17).Active = true;

            Decimal2_img.TopAnchor.ConstraintEqualTo(Decimal2_img.Superview.TopAnchor, 10).Active = true;
            Decimal2_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            Decimal2_img.RightAnchor.ConstraintEqualTo(Decimal2_img.Superview.RightAnchor, -15).Active = true;
            Decimal2_img.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region Decimal3View
            Decimal3View.TopAnchor.ConstraintEqualTo(Decimal2View.BottomAnchor, 10).Active = true;
            Decimal3View.LeftAnchor.ConstraintEqualTo(Decimal3View.Superview.LeftAnchor, 10).Active = true;
            Decimal3View.RightAnchor.ConstraintEqualTo(Decimal3View.Superview.RightAnchor, -10).Active = true;
            Decimal3View.HeightAnchor.ConstraintEqualTo(70).Active = true;


            lbl_Decimal3_head.TopAnchor.ConstraintEqualTo(lbl_Decimal3_head.Superview.TopAnchor, 8).Active = true;
            lbl_Decimal3_head.LeftAnchor.ConstraintEqualTo(lbl_Decimal3_head.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal3_head.RightAnchor.ConstraintEqualTo(Decimal3_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lbl_Decimal3_head.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lbl_Decimal3.TopAnchor.ConstraintEqualTo(lbl_Decimal3_head.BottomAnchor, 4).Active = true;
            lbl_Decimal3.LeftAnchor.ConstraintEqualTo(lbl_Decimal3.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal3.RightAnchor.ConstraintEqualTo(lbl_Decimal3.Superview.CenterXAnchor, -25).Active = true;
            lbl_Decimal3.HeightAnchor.ConstraintEqualTo(32).Active = true;

            lblD3_use.TopAnchor.ConstraintEqualTo(lbl_Decimal3_head.BottomAnchor, 4).Active = true;
            lblD3_use.LeftAnchor.ConstraintEqualTo(lblD3_use.Superview.CenterXAnchor, -25).Active = true;
            lblD3_use.RightAnchor.ConstraintEqualTo(lblD4_use.Superview.RightAnchor, -10).Active = true;
            lblD3_use.HeightAnchor.ConstraintEqualTo(32).Active = true;

            Decimal3_img.TopAnchor.ConstraintEqualTo(Decimal3_img.Superview.TopAnchor, 10).Active = true;
            Decimal3_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            Decimal3_img.RightAnchor.ConstraintEqualTo(Decimal3_img.Superview.RightAnchor, -15).Active = true;
            Decimal3_img.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region Decimal4View
            Decimal4View.TopAnchor.ConstraintEqualTo(Decimal3View.BottomAnchor, 10).Active = true;
            Decimal4View.LeftAnchor.ConstraintEqualTo(Decimal4View.Superview.LeftAnchor, 10).Active = true;
            Decimal4View.RightAnchor.ConstraintEqualTo(Decimal4View.Superview.RightAnchor, -10).Active = true;
            Decimal4View.HeightAnchor.ConstraintEqualTo(70).Active = true;


            lbl_Decimal4_head.TopAnchor.ConstraintEqualTo(lbl_Decimal4_head.Superview.TopAnchor, 8).Active = true;
            lbl_Decimal4_head.LeftAnchor.ConstraintEqualTo(lbl_Decimal4_head.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal4_head.RightAnchor.ConstraintEqualTo(lbl_Decimal4_multiply.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            lbl_Decimal4_head.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lbl_Decimal4_multiply.TopAnchor.ConstraintEqualTo(lbl_Decimal4_multiply.Superview.TopAnchor, 8).Active = true;
            lbl_Decimal4_multiply.LeftAnchor.ConstraintEqualTo(lbl_Decimal4_multiply.Superview.CenterXAnchor,-25).Active = true;
            lbl_Decimal4_multiply.WidthAnchor.ConstraintGreaterThanOrEqualTo(50).Active = true;
            lbl_Decimal4_multiply.HeightAnchor.ConstraintEqualTo(19).Active = true;

            lbl_Decimal4.TopAnchor.ConstraintEqualTo(lbl_Decimal4_head.BottomAnchor, 4).Active = true;
            lbl_Decimal4.LeftAnchor.ConstraintEqualTo(lbl_Decimal4.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal4.RightAnchor.ConstraintEqualTo(lblD4_use.Superview.CenterXAnchor, -25).Active = true;
            lbl_Decimal4.HeightAnchor.ConstraintEqualTo(32).Active = true;

            lblD4_use.TopAnchor.ConstraintEqualTo(lbl_Decimal4_head.BottomAnchor, 4).Active = true;
            lblD4_use.LeftAnchor.ConstraintEqualTo(lblD4_use.Superview.CenterXAnchor, -25).Active = true;
            lblD4_use.RightAnchor.ConstraintEqualTo(lblD4_use.Superview.RightAnchor, -5).Active = true;
            lblD4_use.HeightAnchor.ConstraintEqualTo(32).Active = true;

            Decimal4_img.TopAnchor.ConstraintEqualTo(Decimal4_img.Superview.TopAnchor, 10).Active = true;
            Decimal4_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            Decimal4_img.RightAnchor.ConstraintEqualTo(Decimal4_img.Superview.RightAnchor, -15).Active = true;
            Decimal4_img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            txtx.TopAnchor.ConstraintEqualTo(lbl_Decimal4_multiply.Superview.TopAnchor, 8).Active = true;
            txtx.LeftAnchor.ConstraintEqualTo(lbl_Decimal4_multiply.RightAnchor, 10).Active = true;
            txtx.RightAnchor.ConstraintEqualTo(Decimal4_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            //txtx.HeightAnchor.ConstraintEqualTo(19).Active = true;
            txtx.WidthAnchor.ConstraintGreaterThanOrEqualTo(50).Active = true; 
            //txtx.BackgroundColor = UIColor.Red;
            #endregion

            #region Decimal5View
            Decimal5View.TopAnchor.ConstraintEqualTo(Decimal4View.BottomAnchor, 10).Active = true;
            Decimal5View.LeftAnchor.ConstraintEqualTo(Decimal5View.Superview.LeftAnchor, 10).Active = true;
            Decimal5View.RightAnchor.ConstraintEqualTo(Decimal5View.Superview.RightAnchor, -10).Active = true;
            Decimal5View.BottomAnchor.ConstraintEqualTo(Decimal5View.Superview.BottomAnchor, -10).Active = true;
            Decimal5View.HeightAnchor.ConstraintEqualTo(70).Active = true;


            lbl_Decimal5_head.TopAnchor.ConstraintEqualTo(lbl_Decimal5_head.Superview.TopAnchor, 15).Active = true;
            lbl_Decimal5_head.LeftAnchor.ConstraintEqualTo(lbl_Decimal5_head.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal5_head.RightAnchor.ConstraintEqualTo(Decimal5_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lbl_Decimal5_head.HeightAnchor.ConstraintEqualTo(16).Active = true;

            lbl_Decimal5.TopAnchor.ConstraintEqualTo(lbl_Decimal5_head.BottomAnchor,4).Active = true;
            lbl_Decimal5.LeftAnchor.ConstraintEqualTo(lbl_Decimal5.Superview.LeftAnchor, 15).Active = true;
            lbl_Decimal5.RightAnchor.ConstraintEqualTo(lbl_Decimal5.Superview.CenterXAnchor, -25).Active = true;
            lbl_Decimal5.HeightAnchor.ConstraintEqualTo(17).Active = true;

            lblD5_use.TopAnchor.ConstraintEqualTo(lbl_Decimal5_head.BottomAnchor,2).Active = true;
            lblD5_use.LeftAnchor.ConstraintEqualTo(lblD5_use.Superview.CenterXAnchor, -25).Active = true;
            lblD5_use.RightAnchor.ConstraintEqualTo(Decimal5_img.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            lblD5_use.HeightAnchor.ConstraintEqualTo(17).Active = true;

            Decimal5_img.TopAnchor.ConstraintEqualTo(Decimal5_img.Superview.TopAnchor, 10).Active = true;
            Decimal5_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            Decimal5_img.RightAnchor.ConstraintEqualTo(Decimal5_img.Superview.RightAnchor, -15).Active = true;
            Decimal5_img.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            btnSelectType.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnSelectType.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSelectType.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnSelectType.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            
        }

    }
}