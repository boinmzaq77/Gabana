using Foundation;
using Gabana.iOS;
using Gabana.ShareSource;
using System;
using System.Linq;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.POS
{

    public partial class TermSettingController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView, checkView;
        UIView BottomView;
        UILabel lblTerm, lblPrivacy, lblHeadTerm,lblContext;
        UIImageView TermSelect, PrivacySelect;
        UIButton btnsave; 
        bool term;
        bool policy ;
        bool fbtn;
        public static Gabana3.JAM.Merchant.Merchants MerchantDetail;

        UILabel lblContextH1, lblContext2, lblContextH2, lblContext3, lblContextH3, lblContext4, lblContextH4, lblContext5, lblContextH5;
        UILabel lblContext6, lblContextH6, lblContext7, lblContextH7, lblContext8;
        UILabel lblContextT8, lblContextTH7, lblContextT7, lblContextTH6, lblContextT6, lblContextTH5, lblHeadCondition, lblContextT1, lblContextTH1, lblContextT2, lblContextTH2, lblContextT3, lblContextTH3, lblContextT4, lblContextTH4, lblContextT5;


        public TermSettingController(bool fbtn)
        {
            this.fbtn = fbtn; 
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            View.BackgroundColor = UIColor.White;
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
            {

                var navBarAppearance = new UINavigationBarAppearance();
                navBarAppearance.ConfigureWithOpaqueBackground();
                navBarAppearance.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
                navBarAppearance.LargeTitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
                navBarAppearance.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                this.NavigationController.NavigationBar.StandardAppearance = navBarAppearance;
                this.NavigationController.NavigationBar.ScrollEdgeAppearance = navBarAppearance;
                this.NavigationController.NavigationBar.TintColor = UIColor.White;
            }
            else
            {
                this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
                this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
                this.NavigationController.NavigationBar.TintColor = UIColor.White;
            }
            //this.NavigationController.NavigationBar.Translucent = true;

            //this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
            //this.NavigationController.NavigationBar.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            //this.NavigationController.NavigationBar.TopItem.Title = "Update Merchant";
            //this.NavigationController.NavigationBar.TintColor = UIColor.White;
            ////Utils.SetTitle(this.NavigationController, "Choose Branch");

            
            //this.NavigationController.SetNavigationBarHidden(false, false);
            //this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
            //{
            //    ForegroundColor = UIColor.White
            //    //BackgroundColor = UIColor.FromRGB(51, 170, 225)
            //};
            //this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
        }
        public async override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
                this.NavigationController.NavigationBar.TopItem.Title = "Terms of Service & Privacy Policy";
                //this.NavigationController.NavigationBar.TintColor = UIColor.White;
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                this.NavigationController.SetNavigationBarHidden(false, false);

                //get
                SaveLanguage();
                getPolicy();

                initAttribute();
                setupAutoLayout();

                if (DataCashingAll.Merchant != null)
                {
                    MerchantDetail = DataCashingAll.Merchant;
                    if (MerchantDetail.Merchant.FTermConditions == 'Y')
                    {
                        term = true;
                    }
                    else
                    {
                        term = false;
                    }
                    if (MerchantDetail.Merchant.FPrivacyPolicy == 'Y')
                    {
                        policy = true;
                    }
                    else
                    {
                        policy = false;

                    }
                }

                SetImageTermPolicy();


                #region click
                TermSelect.UserInteractionEnabled = true;
                var tapGesture = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("TermClick:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                TermSelect.AddGestureRecognizer(tapGesture);

                PrivacySelect.UserInteractionEnabled = true;
                var tapGesture0 = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("PrivacyClick:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                PrivacySelect.AddGestureRecognizer(tapGesture0);
                #endregion
                if (fbtn)
                {
                    Utils.SetConstant(BottomView.Constraints, NSLayoutAttribute.Height, 80);


                    var view = new UIView();
                    var button = new UIButton(UIButtonType.Custom);
                    button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                    //button.SetTitle("  " + Utils.TextBundle("back", "Back"), UIControlState.Normal);
                    button.SetTitleColor(UIColor.White, UIControlState.Normal);
                    button.TouchUpInside += Button_TouchUpInside;
                    button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                    button.SizeToFit();
                    view.AddSubview(button);
                    view.Frame = button.Bounds;
                    NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);
                }
                else
                {
                    Utils.SetConstant(BottomView.Constraints, NSLayoutAttribute.Height, 0);
                }
            }
            catch (Exception ex)
            {
                await TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            Preferences.Set("AppState", "logout");
            Preferences.Set("Branch", "");
            this.NavigationController.DismissViewController(false, null);
        }

        void getPolicy()
        {
            term = false;
            policy = false;
        }
        void initAttribute()
        {
            #region scroll bar
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.White;

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.White;

            lblHeadTerm = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblHeadTerm.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblHeadTerm.Font = lblHeadTerm.Font.WithSize(15);
            lblHeadTerm.Lines = 1;
            lblHeadTerm.Text = "นโยบายความเป็นส่วนตัว";
            _contentView.AddSubview(lblHeadTerm);

            #region policy
            lblContext = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext.Font = lblContext.Font.WithSize(12);
            lblContext.Lines = 8;
            lblContext.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext.Text = "ข้อมูลผู้ใช้งานบริการ Gabana โดยบริษัท ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด เลขที่ 200" +
                " อาคาร ทศพล ชั้น1 ซีดี ถนนรัชดาภิเษก แขวงห้วยขวาง เขตห้วยขวาง กรุงเทพฯ 10310 " +
                "จะถูกนำไปใช้ และเก็บรักษาเป็นความลับ โดยมีเงื่อนไขดังต่อไปนี้\n\n"+
                "ความเป็นส่วนตัวของข้อมูลของท่านมีความสำคัญกับเรา ดังนั้นเราจึงได้พัฒนานโยบายความเป็นส่วนตัวของเรา" +
                " เพื่อที่จะอธิบายถึงหัวข้อสำคัญต่างๆ แก่ท่าน ดังนี้";
            _contentView.AddSubview(lblContext);

            lblContextH1 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextH1.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextH1.Font = lblHeadTerm.Font.WithSize(15);
            lblContextH1.Lines = 2;
            lblContextH1.Text = "ข้อมูลที่เราเก็บรวบรวม\nร้านค้าที่ใช้งาน Gabana";
            _contentView.AddSubview(lblContextH1);

            lblContext2 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext2.Font = lblContext2.Font.WithSize(12);
            lblContext2.Lines =10;
            lblContext2.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext2.Text = "- ชื่อ-นามสกุล / ชื่อบริษัท / เลขประจำตัวผู้เสียภาษี / ที่อยู่ / เบอร์โทรศัพท์ / อีเมลล์ / โลโก้ร้านค้า / ชื่อสาขา \n"
                +"- บันทึกข้อมูล จากการสมัครใช้งาน\n"+
                "- การเข้าถึงเว็บไซต์ และ เว็บคุ๊กกี้ ที่สามารถระบุ เบราว์เซอร์ หรือ บัญชีของท่าน (กรณีใช้งานระบบ myMerchat เพื่อจัดการ License การใช้งาน)\n"+
                "- ข้อมูลของอุปกรณ์ที่เข้าใช้งานได้แก่ Device Model, operating system version,\n" +
                "-  mobile network, Internet Protocol(IP)address และ Location\n" +
                "- ข้อมูลส่วนบุคคลอื่นๆที่ท่านได้ให้ไว้แก่บริษัท";
            _contentView.AddSubview(lblContext2);

            lblContextH2 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextH2.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextH2.Font = lblHeadTerm.Font.WithSize(15);
            lblContextH2.Lines = 2;
            lblContextH2.Text = "ผู้ใช้บริการที่สมัครเป็นสมาชิก หรือยินยอมให้ข้อมูลแก่ร้านค้าที่ใช้งาน Gabana";
            _contentView.AddSubview(lblContextH2);

            lblContext3 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext3.Font = lblContext3.Font.WithSize(12);
            lblContext3.Lines = 5;
            lblContext3.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext3.Text = "- ชื่อ-นามสกุล / เลขประจำตัวผู้เสียภาษี / หมายเลขบัตรประชาชน/ ที่อยู่ / เบอร์โทรศัพท์ / อีเมล์ / วันเกิด / เพศ / รูปถ่าย" +
                "\n- ข้อมูลการติดต่อทางสื่อสังคมออนไลน์(เช่น LINE ID, Facebook ID)" +
                "\n- ข้อมูลส่วนบุคคลอื่นๆที่ลูกค้าของร้านค้า ยินยอมให้ข้อมูลแก่ร้านค้าที่ใช้งาน Gabana";
            _contentView.AddSubview(lblContext3);

            lblContextH3 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextH3.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextH3.Font = lblHeadTerm.Font.WithSize(15);
            lblContextH3.Lines = 1;
            lblContextH3.Text = "วิธีการนำข้อมูลของท่านไปใช้งาน";
            _contentView.AddSubview(lblContextH3);

            lblContext4 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext4.Font = lblContext4.Font.WithSize(12);
            lblContext4.Lines = 9;
            lblContext4.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext4.Text = "- เพื่อแจ้งข่าวสารเกี่ยวกับสินค้า / บริการใหม่ รวมถึงการจัดงานอีเว้นท์ และการจัดฝึกอบรมต่าง ๆ (ซึ่งท่านสามารถเลือกที่จะไม่รับข่าวสารดังกล่าวได้)" +
                "\n- เพื่อพัฒนา / ปรับปรุง สินค้าและบริการของเรา" +
                "\n- เพื่อให้คำแนะนำเกี่ยวกับการให้บริการ รวมถึงข้อมูลการทำโฆษณาจากบริษัทคู่ค้าทางธุรกิจของเรา" +
                "\n- เพื่อยืนยันการแสดงตัวตน ก่อนการให้บริการแก่ท่าน" +
                "\n- สำหรับการประมวลข้อมูลภายในองค์กร และการทำวิจัยอื่นๆ"
                + "\n- หากท่านไม่ให้ข้อมูลส่วนบุคคลของท่านแก่บริษัทฯ อาจทำให้บริษัทฯ ไม่สามารถให้บริการแก่ท่านได้ในบางกรณี";
            _contentView.AddSubview(lblContext4);

            lblContextH4 = new UILabel
            {
                TextAlignment = UITextAlignment.Left ,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextH4.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextH4.Font = lblHeadTerm.Font.WithSize(15);
            lblContextH4.Lines = 1;
            lblContextH4.Text = "การเปิดเผยข้อมูลแก่บุคคลที่สาม";
            _contentView.AddSubview(lblContextH4);

            lblContext5 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext5.Font = lblContext5.Font.WithSize(12);
            lblContext5.Lines = 5;
            lblContext5.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext5.Text = "เราไม่มีการให้ข้อมูลของท่านแก่ บริษัท องค์กร และ บุคคล อื่น ๆ ภายนอกบริษัท ยกเว้นในกรณีต่าง ๆ ดังต่อไปนี้\n" +
                "\t- ได้รับการอนุญาตจากท่าน\n" +
                "\t- สำหรับการประมวลผลภายนอก และ บริษัทคู่ค้าทางธุรกิจซึ่งให้บริการแก่ท่าน\n" +
                "\t- สำหรับเหตุผลทางกฎหมาย";
            _contentView.AddSubview(lblContext5);

            lblContextH5 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextH5.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextH5.Font = lblHeadTerm.Font.WithSize(15);
            lblContextH5.Lines = 1;
            lblContextH5.Text = "วิธีการเก็บรักษาข้อมูล";
            _contentView.AddSubview(lblContextH5);


            lblContext6 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext6.Font = lblContext6.Font.WithSize(12);
            lblContext6.Lines = 8;
            lblContext6.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext6.Text = "เรามีระบบปกป้องข้อมูลของท่าน จากการถูกใช้โดยไม่ได้รับอนุญาต การดัดแปลง\n และการถูกทำลาย ด้วยวิธีการป้องกัน ดังนี้\n" +
                "มีระบบป้องกันการเข้าถึงข้อมูลโดยไม่ได้รับอนุญาตจากบุคคลภายนอก\n" +
                "การเข้ารหัสข้อมูลสำคัญขณะรับ - ส่งข้อมูลด้วย SSL(สำหรับ Browser กรณีใช้งาน myMerchant)\n" +
                "การเข้ารหัสข้อมูลสำคัญขณะรับ - ส่งข้อมูลด้วย XXX(สำหรับ Application)";
            _contentView.AddSubview(lblContext6);

            lblContextH6 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextH6.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextH6.Font = lblHeadTerm.Font.WithSize(15);
            lblContextH6.Lines = 1;
            lblContextH6.Text = "การเปลี่ยนแปลง";
            _contentView.AddSubview(lblContextH6);

            lblContext7 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext7.Font = lblContext7.Font.WithSize(12);
            lblContext7.Lines = 7;
            lblContext7.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext7.Text = "วัตถุประสงค์ของนโยบายความเป็นส่วนตัวของเรา คือ การปกป้องรักษาข้อมูลส่วนตัวของท่าน อย่างไรก็ตามเราขอสงวนสิทธิ์ในการเปลี่ยนแปลงนโยบายความเป็นส่วนตัวของเรา" +
                " โดยไม่มีการแจ้งล่วงหน้า\n" +
                "ในกรณีที่ข้อมูลส่วนบุคคล หรือข้อมูลที่เกี่ยวข้องของผู้ใช้งานถูกจารกรรมทางคอมพิวเตอร์(Hack) สูญหาย หรือเสียหายซึ่งมิใช่ความผิดของบริษัท" +
                "\n บริษัทขอสงวนสิทธิ์ที่จะปฏิเสธความรับผิดใดๆอันเป็นผลมาจากการดังกล่าว";
            _contentView.AddSubview(lblContext7);

            lblContextH7 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextH7.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextH7.Font = lblContextH7.Font.WithSize(15);
            lblContextH7.Lines = 1;
            lblContextH7.Text = "สอบถามข้อมูล";
            _contentView.AddSubview(lblContextH7);

            lblContext8 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContext8.Font = lblContext8.Font.WithSize(12);
            lblContext8.Lines = 2;
            lblContext8.LineBreakMode = UILineBreakMode.WordWrap;
            lblContext8.Text = "ท่านสามารถติดต่อเรา เพื่อสอบถาม หรือเปลี่ยนแปลงแก้ไขข้อมูล\n ได้ที่ info@seniorsoft.co.th";
            _contentView.AddSubview(lblContext8);
            #endregion
            
            #region term
            lblHeadCondition = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblHeadCondition.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblHeadCondition.Font = lblHeadCondition.Font.WithSize(15);
            lblHeadCondition.Lines = 2;
            lblHeadCondition.Text = "ข้อตกลงและเงื่อนไขการใช้บริการระบบ Gabana \n(Gabana Terms and Conditions)";
            _contentView.AddSubview(lblHeadCondition);

            lblContextT1 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT1.Font = lblContextT1.Font.WithSize(12);
            lblContextT1.Lines = 20;
            lblContextT1.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT1.Text = "Gabana เป็นระบบบริหารงานขาย (POS) ที่ให้บริการโดย บริษัท ซีเนียร์ซอฟท์" +
                " ดีเวลลอปเม้นท์ จำกัด เลขที่ 200 อาคาร ทศพล ชั้น1 ซีดี ถนนรัชดาภิเษกแขวงห้วยขวาง " +
                "เขตห้วยขวางกรุงเทพฯ 10310 ซึ่งต่อไปนี้เรียกว่า “บริษัท”" +
                "ผู้ใช้สามารถสมัครใช้บริการระบบ Gabana เพื่อเริ่มต้นบริหารจัดการร้านค้าของตน จัดการด้านงานขาย " +
                "จัดการสต๊อกสินค้า และออกบิลใบเสร็จรับเงิน\n" +
                "เมื่อผู้ใช้บริการของบริษัท(“ผู้ใช้บริการ”) ได้เข้าถึง และ/ หรือ เข้าสู่แอพพลิเคชั่น Gabana เพื่อใช้บริการ" +
                " บริษัทจะถือว่า ผู้ใช้บริการได้ให้คำยืนยันและรับรองต่อบริษัทว่า ผู้ใช้บริการได้เข้าถึง ตรวจสอบ ศึกษา และ / หรือ รับทราบ" +
                " รายละเอียดต่าง ๆ ของเงื่อนไขในการใช้บริการก่อนการเข้าใช้บริการ พร้อมทั้งผู้ใช้บริการยังได้ตกลง ยินยอม" +
                "และ จะปฏิบัติตามเงื่อนไขในการใช้บริการต่าง ๆ(รวมถึง เงื่อนไขในการใช้บริการที่บริษัทอาจจะได้มีการปรับปรุงและแก้ไขเพิ่มเติมต่อไปในอนาคต) อย่างเคร่งครัด\n\n" +
                "ผู้ใช้บริการรับทราบและจะปฏิบัติตามเงื่อนไขในการใช้บริการที่มีรายละเอียดดังต่อไปนี้";
            _contentView.AddSubview(lblContextT1);

            lblContextTH1 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextTH1.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextTH1.Font = lblHeadTerm.Font.WithSize(15);
            lblContextTH1.Lines = 1;
            lblContextTH1.Text = "1. ผู้ใช้บริการ ";
            _contentView.AddSubview(lblContextTH1);

            lblContextT2 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT2.Font = lblContextT2.Font.WithSize(12);
            lblContextT2.Lines = 45;
            lblContextT2.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT2.Text = "1.1 ผู้ใช้บริการรับทราบและจะปฏิบัติตามเงื่อนไขดังต่อไปนี้\n" +
                "- ผู้ใช้บริการ หมายถึง ห้างร้าน องค์กรต่างๆ นิติบุคคล หรือบุคคล ที่สมัคร และ/ หรือ เปิดใช้ระบบ สำหรับร้านค้า หรือธุรกิจของตน เพื่อนำไปใช้เป็นระบบบริหารงานขาย(POS)\n" +
                "- ร้านค้าที่สมัครใช้งาน Gabana จะต้องเป็น นิติบุคคลหรือองค์กรที่ถูกต้องตามกฎหมาย โดย ผู้ติดต่อและ/ หรือเข้าทำสัญญาใดๆกับ บริษัท " +
                "เพื่อใช้บริการต่างๆ เป็นผู้มีอำนาจผูกพัน และ/ หรือ ได้รับการมอบอำนาจให้กระทำการแทนนิติบุคคลหรือองค์กรดังกล่าวอย่างสมบูรณ์\n" +
                "- บริษัทสงวนสิทธิ์ที่จะอนุมัติการให้บริการ เฉพาะ ธุรกิจที่ไม่ขัดต่อกฎหมาย, ศีลธรรมและวัฒนธรรมของประเทศที่นำบริการนี้ไปใช้ รวมทั้งต้องสามารถยืนยันตัวตนของธุรกิจนั้นได้\n" +
                "- ธุรกิจที่ใช้บริการ ต้องไม่นำบริการ Gabana ไปใช้อย่างไม่ถูกต้อง สร้างความรำคาญ ความเสียหายต่อผู้อื่น และขัดต่อกฎหมายของประเทศไทย\n" +
                "\n1.2 การให้คำยืนยันและคำรับรองต่อการใช้บริการ Gabana" +
                "\nผู้ใช้บริการขอให้คำยืนยันและคำรับรองต่อ บริษัท ดังต่อไปนี้\n" +
                "- ผู้ใช้บริการเป็นผู้ที่มีความรับผิดชอบที่ดีต่อสังคม และ ในการใช้บริการ ผู้ใช้บริการจะไม่สร้างความเสียหายเดือดร้อนหรือละเมิดสิทธิ์อื่นใดแก่บุคคลอื่นใด\n" +
                "- เพื่อใช้บริการ ผู้ใช้บริการจะให้และ จะเปิดเผยข้อมูลต่างๆ ของผู้ใช้บริการให้ครบถ้วน ถูกต้องและ เป็นปัจจุบัน และผู้ใช้บริการจะจัดให้มีอีเมล และ / หรือ " +
                "ช่องทางการสื่อสารทางระบบอินเทอร์เน็ตอย่างอื่น เพื่อการรับข้อมูลข่าวสารต่างๆ และ เป็นช่องทางติดต่อกับบริษัท อนึ่ง ในกรณีที่ข้อมูลต่างๆ ที่ผู้ใช้บริการได้ให้และ / หรือ เปิดเผย" +
                " แก่บริษัทได้เปลี่ยนแปลงไป ผู้ใช้บริการมีหน้าที่และตกลงจะแก้ไขข้อมูลต่างๆ ดังกล่าวให้เป็นปัจจุบันอยู่เสมอ ซึ่งผู้ใช้บริการสามารถแก้ไขข้อมูลดังกล่าวผ่านทางระบบหรือช่องทางที่บริษัทได้จัดสร้างขึ้น" +
                " ในกรณีที่บริษัทจัดให้มีระบบการจัดการให้แก้ไขข้อมูล หรือผู้ใช้บริการจะแจ้งการแก้ไขข้อมูลดังกล่าวไปยังเจ้าหน้าที่ของบริษัทที่ให้บริการแก่ผู้ใช้บริการ\n" +
                "- ผู้ใช้บริการได้ศึกษาและเข้าใจในคุณลักษณะ ข้อจำกัด และเงื่อนไขต่างๆ ของบริการ ที่บริษัทได้จัดแสดงใน Gabana Application " +
                "และผู้ใช้บริการได้ตัดสินใจใช้บริการด้วยคุณลักษณะ ข้อจำกัด และเงื่อนไขต่าง ๆ ที่มีอยู่นี้ และยอมรับการเปลี่ยนแปลงคุณลักษณะ ข้อจำกัด " +
                "และเงื่อนไขต่าง ๆ ที่จะมีในอนาคตซึ่งอาจจะมีผลทำให้ผู้ใช้บริการได้ใช้บริการที่แตกต่างจากที่เคยได้ใช้บริการมาก่อนเช่น การใช้บริการผ่านการทำงานในรูปแบบ หรือ Feature ที่แตกต่างไปจากเดิม\n" +
                "- ผู้ใช้บริการเป็นผู้ตัดสินใจเลือกใช้บริการด้วยตัวเอง และ ในกรณีที่การใช้บริการของผู้ใช้บริการที่เป็นนิติบุคคลหรือองค์กรใดๆ บริษัทจะถือว่า" +
                " ผู้ใช้บริการเป็นผู้รับผิดชอบต่อการตัดสินใจเลือกใช้บริการเอง หากมีปัญหาเกิดขึ้นระหว่างผู้ใช้บริการกับผู้ติดต่อหรือตัวแทนของผู้ใช้บริการ" +
                " ผู้ใช้บริการรับทราบว่า บริษัทไม่มีหน้าที่ใดๆในการดำเนินการแก้ไขปัญหาดังกล่าวของผู้ใช้บริการ\n" +
                "- เมื่อได้รับการร้องขอจากบริษัท ผู้ใช้บริการจะส่งมอบเอกสาร และ / หรือ หลักฐานใดๆ ที่จำเป็นเพื่อแสดงว่าผู้ใช้บริการมีความสามารถ" +
                " และ / หรือ อำนาจในการใช้บริการที่ผู้ใช้บริการได้เลือกใช้บริการ เช่น บัตรประชาชน ทะเบียนบ้าน หนังสือรับรองของบริษัท เป็นต้น " +
                "รวมถึง ผู้ใช้บริการจะลงนามในเอกสารใดๆ เพื่อรับทราบและตกลงยินยอมผูกพันตามข้อตกลงและเงื่อนไขในการใช้บริการของผู้ใช้บริการ " +
                "ตามที่บริษัทพิจารณาเห็นสมควรและได้ร้องขอให้ดำเนินการ";
            _contentView.AddSubview(lblContextT2);

            lblContextTH2 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextTH2.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextTH2.Font = lblHeadTerm.Font.WithSize(15);
            lblContextTH2.Lines = 1;
            lblContextTH2.Text = "2. การใช้บริการ";
            _contentView.AddSubview(lblContextTH2);

            lblContextT3 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT3.Font = lblContextT3.Font.WithSize(12);
            lblContextT3.Lines = 100;
            lblContextT3.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT3.Text = "- บริษัท จะไม่รับผิดชอบต่อความสูญเสีย และ/หรือ ความเสียหายไม่ว่าในลักษณะใดที่เป็นผล และ/หรือ อาจจะเป็นผลมาจากการใช้บริการ ของผู้ใช้บริการ (ทั้งนี้ไม่ว่าทางตรงหรือทางอ้อม)" +
                "\n- บริษัท ได้จัดให้มีข้อกำหนดด้านนโยบายความเป็นส่วนตัว(Privacy Policy) (รวมถึงที่จะได้มีการแก้ไขในอนาคต)ผู้ใช้บริการควรศึกษา ทำความเข้าใจ และ รับทราบรายละเอียดต่าง ๆ ของข้อกำหนดดังกล่าวก่อนการใช้บริการของผู้ใช้บริการ ซึ่งผู้ใช้บริการสามารถเข้าถึง ศึกษา ตรวจสอบ และ จัดพิมพ์ ข้อกำหนดด้านนโยบายความเป็นส่วนตัว(Privacy Policy) ได้ที่ลิงก์ Privacy Policy ใน Gabana Application" +
                "\n- บริษัท จะใช้ความพยายามอย่างเต็มที่ในการให้บริการแก่ผู้ใช้บริการอย่างไรก็ตาม บริษัท ไม่สามารถรับรองผลลัพธ์ หรือ ผลสำเร็จใด(ไม่ว่าทางธุรกิจ กำไร หรือ อย่างอื่นใด) ที่ผู้ใช้บริการมุ่งหวังหรือประสงค์ที่จะได้รับจากการใช้บริการที่ผู้ใช้บริการได้เลือกใช้บริการ และบริษัทจะไม่รับผิดชอบต่อความสูญเสียและ/ หรือ ความเสียหายใดๆ ที่มีหรืออาจจะเกิดขึ้นไม่ว่าในกรณีใดๆ" +
                "\n- ไม่ว่าระหว่าง หรือ ภายหลังระยะเวลา การใช้บริการของผู้ใช้บริการ ผู้ใช้บริการขอยืนยันต่อ บริษัท ว่า ผู้ใช้บริการจะไม่ให้ข้อมูล และ / หรือ แสดงความคิดเห็นใดๆ ในที่สาธารณะ หรือ ในระบบหรือรูปแบบที่ทำให้ข้อมูลสามารถถูกเข้าถึงได้ในวงกว้าง(รวมถึง สื่อสารมวลชน สื่อออนไลน์ และ/ หรือ โซเชียลมีเดีย)ที่อาจจะทำให้เกิดความเสียหาย หรือ เสื่อมเสียต่อ บริษัท(ไม่ว่าในรูปแบบใดๆ) อย่างไรก็ตาม บริษัท ยินดีน้อมรับคำติชมและคำแนะนำต่างๆ จากผู้ใช้บริการเพื่อนำมาปรับปรุงการให้บริการของ บริษัท ต่อไป ซึ่งผู้ใช้บริการสามารถส่งคำติชมและคำแนะนำต่างๆให้แก่ บริษัท มาได้ที่ info@seniorsoft.co.th" +
                "\n- ไม่ว่าระหว่างหรือ ภายหลังระยะเวลาการใช้บริการของผู้ใช้บริการหากมีปัญหา และ/ หรือ ข้อพิพาทใดๆ เกิดขึ้นระหว่างผู้ใช้บริการกับลูกค้าของผู้ใช้บริการและ/ หรือบุคคลอื่นใดก็ตาม ที่เป็นผลมาจากการใช้บริการของผู้ใช้บริการ ผู้ใช้บริการรับทราบและ จะรับผิดชอบในการดำเนินการใดๆ กับปัญหาดังกล่าวเอง บริษัท ไม่มีหน้าที่ และ / หรือความรับผิดชอบในดำเนินการกับปัญหา และ / หรือ ข้อพิพาทดังกล่าวอนึ่ง ในกรณีที่ บริษัท ต้องเสียหาย(ไม่ว่าในรูปแบบใดๆ อันเป็นผลมาจากการดำเนินการใดๆ เกี่ยวปัญหาดังกล่าว ผู้ใช้บริการตกลงที่จะดำเนินการแก้ไขความเสียหายดังกล่าวให้หมดสิ้นไปและ/ หรือ ชดเชยค่าเสียหายใดๆ ที่มีหรืออาจจะมีขึ้นแก่บริษัท รวมถึงกรรมการ ผู้บริหาร เจ้าหน้าที่ และ ตัวแทน ของ บริษัท" +
                "\n- รูปภาพ รูปแบบ ข้อความ ข้อมูล ความเห็น และ / หรือ เนื้อหาต่างๆ ที่ผู้ใช้บริการนำมาใช้(รวมถึง รูปภาพ รูปแบบ ข้อความ ข้อมูล ความเห็น และ/ หรือ เนื้อหาต่างๆ ที่มีผู้แสดงออก และ / หรือ โพสไว้ในพื้นที่แสดงข้อมูลธุรกิจของผู้ใช้บริการ) ไม่ว่าในการใช้บริการใดๆ ของผู้ใช้บริการ เช่น ใน เว็บไซต์หรือในเนื้อหาโฆษณานั้น จะถือเป็นความรับผิดชอบของผู้ใช้บริการแต่เพียงผู้เดียว บริษัท ไม่มีหน้าที่ตรวจสอบ และ / หรือ คัดกรอง รูปภาพ รูปแบบ ข้อมูล และ เนื้อหาต่างๆ ดังกล่าว และ บริษัท จะไม่รับผิดชอบต่อความเสียหายที่เกิดขึ้นเนื่องจากการใช้งาน การรุกล้ำสิทธิ์ส่วนบุคคล หรือปัญหาต่างๆ ที่เกิดขึ้นระหว่างผู้ใช้บริการกับบุคคลอื่นอีกทั้ง ผู้ใช้บริการจะต้องไม่นำมาใช้ และ / หรือ ยินยอมให้ใช้ รูปภาพ รูปแบบ ข้อความ ข้อมูล ความเห็น และ / หรือ เนื้อหาที่ผิดหรือ อาจจะผิดกฎหมาย และ / หรือศีลธรรมของสังคม ไม่ว่าในกรณีใดๆ ในกรณีที่ บริษัท พบว่า ผู้ใช้บริการไม่ได้ดำเนินการให้เป็นไปตามเงื่อนไขดังกล่าว บริษัท ขอสงวนสิทธิ์ในการลบรูปภาพ รูปแบบ ข้อความ ข้อมูล ความเห็น และ/ หรือ เนื้อหา เหล่านั้น หรือยกเลิกการใช้บริการใดๆ ของผู้ใช้บริการที่ก่อหรือ อาจจะก่อให้เกิดปัญหา และ/ หรือ ความเสียหายต่างๆ ดังกล่าว โดยบริษัทไม่จำเป็นต้องแจ้งให้ผู้ใช้บริการทราบล่วงหน้า" +
                "\n- เมื่อได้รับการร้องขอจากบริษัท(ไม่ว่าระหว่าง หรือ ภายหลัง การใช้บริการของผู้ใช้บริการ) ผู้ใช้บริการยินดีจะให้ความร่วมมือกับบริษัท ในการให้ และ เปิดเผย ข้อมูล เอกสาร และ / หรือ หลักฐานต่างๆ ที่เกี่ยวกับการใช้บริการของผู้ใช้บริการ แก่บริษัท และ ให้บริษัทเปิดเผย และ/ หรือ ส่งมอบข้อมูล เอกสาร และ / หรือ หลักฐานดังกล่าว ให้แก่หน่วยงานราชการ และ องค์กรใดๆ ที่มีอำนาจตามกฎหมาย เพื่อปฏิบัติตาม กฎ ระเบียบ กฎหมาย และ/ หรือ คำสั่ง ของหน่วยงานราชการ และ องค์กรใดๆ ที่มีอำนาจตามกฎหมายที่เกี่ยวข้องดังกล่าว" +
                "\n- ในการใช้บริการของผู้ใช้บริการ ผู้ใช้บริการรับทราบและตกลงว่า บริษัทไม่ได้ให้คำรับรองหรือยืนยันถึงคุณภาพ คุณค่า มูลค่า และ / หรือ คุณสมบัติใดๆ ของสินค้า ผลิตภัณฑ์ และ/ หรือ บริการ ที่ผู้ใช้บริการ และ / หรือ บุคคลภายนอก นำมาเสนอหรือแสดงไว้ เกี่ยวกับการใช้บริการของผู้ใช้บริการ เช่น การแสดงไว้ในเว็บไซต์หรือในเนื้อหาโฆษณา อนึ่ง ในกรณีที่บริษัทต้องเสียหาย(ไม่ว่าในรูปแบบใดๆ) อันเป็นผลมาจากการดำเนินการเกี่ยวกรณีดังกล่าว ผู้ใช้บริการตกลงที่จะดำเนินการแก้ไขความเสียหายดังกล่าวให้หมดสิ้นไป หรือชดเชยค่าเสียหายใดๆ ที่มีหรืออาจจะมีขึ้นแก่บริษัทรวมถึง กรรมการ ผู้บริหาร เจ้าหน้าที่ และ ตัวแทน ของบริษัท" +
                "\n- ในการใช้บริการของผู้ใช้บริการ ผู้ใช้บริการรับทราบและตกลงว่า บริษัทไม่ได้ให้คำรับรองหรือยืนยันว่าระบบเครือข่ายอินเทอร์เน็ต และ/ หรือ การให้บริการโดยผู้ให้บริการในต่างประเทศ จะสามารถดำเนินการได้อย่างสมบูรณ์แบบ ไม่มีข้อบกพร่อง และ/ หรือ มีเสถียรภาพตลอดเวลาการใช้บริการ พร้อมกันนี้ บริษัทขอสงวนสิทธิ์ที่จะไม่รับผิดชอบถึงความสูญเสีย หรือ ความเสียหายใดๆ ที่มี หรือ อาจจะมีขึ้นจากการกรณีดังกล่าวทั้งสิ้นในกรณีที่ผู้ใช้บริการพบข้อบกพร่องใดๆ ของการใช้บริการผู้ใช้บริการสามารถแจ้งให้บริษัททราบถึงข้อบกพร่องดังกล่าวผ่านช่องทางการติดต่อสื่อสารในเงื่อนไขการให้บริการ" +
                "\n- ผู้ใช้บริการขอให้คำรับรองและยืนยันต่อบริษัทว่า ผู้ใช้บริการเข้าใจ รับทราบถึง และ จะปฏิบัติตาม บทบัญญัติของกฎหมายที่เกี่ยวข้องกับการใช้บริการของผู้ใช้บริการ(รวมถึง กฎ ระเบียบ กฎหมาย และ / หรือ คำสั่ง ของหน่วยงานราชการที่เกี่ยวข้อง) อย่างเคร่งครัด" +
                "\n- ผู้ใช้บริการรับทราบและเข้าใจว่า ในการให้บริการต่างๆ แก่ผู้ใช้บริการ บริษัทไม่สามารถรับรองและยืนยันได้ว่า จะไม่มีบุคคลอื่นสามารถเข้าถึง ทำสำเนา แก้ไข เปลี่ยนแปลง และ/ หรือ นำไปใช้ซึ่ง ข้อมูล เอกสาร และ/ หรือ หลักฐานต่างๆ ที่เกี่ยวกับการใช้บริการของผู้ใช้บริการ(ไม่ว่าที่เป็นความลับ หรือไม่ก็ตาม) และ / หรือ ทำ หรือ อาจจะทำให้เกิดความเสียหายใดๆ แก่ผู้ใช้บริการ";
            _contentView.AddSubview(lblContextT3);

            lblContextTH3 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextTH3.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextTH3.Font = lblHeadTerm.Font.WithSize(15);
            lblContextTH3.Lines = 1;
            lblContextTH3.Text = "3. การติดต่อระหว่างผู้ใช้บริการกับบริษัท";
            _contentView.AddSubview(lblContextTH3);

            lblContextT4 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT4.Font = lblContextT4.Font.WithSize(12);
            lblContextT4.Lines = 5;
            lblContextT4.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT4.Text = "การติดต่อระหว่างบริษัทกับผู้ใช้บริการ จะดำเนินการผ่านช่องทางการติดต่อสื่อสารทางอีเมลเป็นหลัก" +
                " โดยผู้ใช้บริการท่านใดมีปัญหาการใช้บริการหรือประสงค์จะสอบถามเกี่ยวกับบริการ สามารถส่งอีเมลมาได้ที่ info@seniorsoft.co.th และ Tel. 02-692-5899";
            _contentView.AddSubview(lblContextT4);

            lblContextTH4 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextTH4.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextTH4.Font = lblHeadTerm.Font.WithSize(15);
            lblContextTH4.Lines = 1;
            lblContextTH4.Text = "4. การเปลี่ยนแปลงกฎ ระเบียบ และเงื่อนไขการให้บริการ";
            _contentView.AddSubview(lblContextTH4);

            lblContextT5 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT5.Font = lblContextT5.Font.WithSize(12);
            lblContextT5.Lines = 10;
            lblContextT5.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT5.Text = "บริษัทอาจจะมีการเปลี่ยนแปลง กฎ ระเบียบ และ เงื่อนไข การให้บริการ (รวมถึง เงื่อนไขในการใช้บริการ" +
                " และ ข้อกำหนดด้านนโยบายความเป็นส่วนตัว (Privacy Policy) เพื่อความเหมาะสมอยู่เสมอ แม้ว่า เพื่ออำนวยความสะดวกให้แก่ผู้ใช้บริการ" +
                " บริษัท จะพยายามแจ้งให้ผู้ใช้บริการทราบถึงการเปลี่ยนแปลง กฎ ระเบียบ และ เงื่อนไข ผ่านทาง Gabana Application " +
                "บริษัทขอสงวนสิทธิ์ในการเปลี่ยนแปลง กฎ ระเบียบ และ เงื่อนไข การให้บริการดังกล่าว โดยไม่ต้องแจ้งให้ผู้ใช้บริการทราบล่วงหน้า" +
                " และ จะไม่รับผิดชอบความเสียหายที่เกิดแก่ผู้ใช้บริการหรือบุคคลอื่นใดอันเนื่องมาจากการเปลี่ยนแปลง กฎ ระเบียบ และ เงื่อนไข การให้บริการไม่ว่าในกรณีใดๆ";
            _contentView.AddSubview(lblContextT5);

            lblContextTH5 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextTH5.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextTH5.Font = lblHeadTerm.Font.WithSize(15);
            lblContextTH5.Lines = 1;
            lblContextTH5.Text = "5. อัตราค่าบริการ";
            _contentView.AddSubview(lblContextTH5);

            lblContextT6 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT6.Font = lblContextT6.Font.WithSize(12);
            lblContextT6.Lines = 4;
            lblContextT6.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT6.Text = "สำหรับผู้ใช้บริการที่ต้องการสมัครใช้งาน Gabana เพื่อเริ่มต้นบริหารจัดการร้านค้าของตน จัดการด้านงานขาย จัดการสต๊อกสินค้า และออกบิลใบเสร็จรับเงิน " +
                "สามารถดูรายละเอียดเกี่ยวกับแพกเกจค่าบริการได้ที่หน้าเว็บไซต์ www.SeniorSoft.com หรือติดต่อสอบถามที่ฝ่ายขายของบริษัท";
            _contentView.AddSubview(lblContextT6);

            lblContextTH6 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextTH6.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextTH6.Font = lblHeadTerm.Font.WithSize(15);
            lblContextTH6.Lines = 1;
            lblContextTH6.Text = "6. โฆษณา";
            _contentView.AddSubview(lblContextTH6);

            lblContextT7 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT7.Font = lblContextT7.Font.WithSize(12);
            lblContextT7.Lines = 3;
            lblContextT7.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT7.Text = "บริษัท สงวนสิทธิ์ในการแสดงโฆษณาของบริษัทหรือบุคคลภายนอกให้กับผู้ใช้บริการผ่านการให้บริการ";
            _contentView.AddSubview(lblContextT7);

            lblContextTH7 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false

            };
            lblContextTH7.Font = UIFont.BoldSystemFontOfSize(size: 15);
            lblContextTH7.Font = lblHeadTerm.Font.WithSize(15);
            lblContextTH7.Lines = 1;
            lblContextTH7.Text = "7. การยกเลิกการใช้บริการ";
            _contentView.AddSubview(lblContextTH7);

            lblContextT8 = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblContextT8.Font = lblContextT8.Font.WithSize(12);
            lblContextT8.Lines = 30;
            lblContextT8.LineBreakMode = UILineBreakMode.WordWrap;
            lblContextT8.Text = "บริษัทขอสงวนสิทธิ์ในการยกเลิกการใช้บริการของผู้ใช้บริการ หากบริษัทเห็นว่าผู้ใช้บริการไม่ปฏิบัติตาม กฎ ระเบียบ และเงื่อนไขการให้บริการ" +
                " (รวมถึง เงื่อนไขในการใช้บริการ และ ข้อกำหนดด้านนโยบายความเป็นส่วนตัว (Privacy Policy) โดยบริษัทไม่จำเป็นต้องแจ้งให้ผู้ใช้บริการทราบล่วงหน้า " +
                "และบริษัทจะไม่รับผิดชอบความเสียหายใดๆ ที่เกิด หรือ อาจจะเกิดขึ้น แก่ผู้ใช้บริการหรือบุคคลอื่นใดอันเนื่องมาจากการดำเนินการดังกล่าว" +
                "\n\t- กรณีท่านต้องการยกเลิกบริการ ทางบริษัท ขอสงวนสิทธิ์ไม่คืนเงินในทุกๆกรณี" +
                "\n\t- ค่าธรรมเนียมการใช้บริการ กรณีผู้ใช้ตกลงจะชำระค่าธรรมเนียมการใช้บริการซอฟต์แวร์ Gabana สามารถชำระผ่านช่องทางตามที่บริษัทซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด กำหนด" +
                "\n\t- ค่าธรรมเนียมจะรวมถึง VAT และภาษีที่เกี่ยวข้องอื่นๆ เว้นแต่ตกลงกันไว้เป็นอย่างอื่น ผู้ใช้จะรับผิดชอบค่าภาษีเหล่านั้นเอง ผู้ใช้รับทราบและตกลงบริษัท ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด สามารถหักค่าธรรมเนียมและภาษีที่เกี่ยวข้อง และบริษัท ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด จะออกใบเสร็จรับเงินหรือใบกำกับภาษีสำหรับค่าธรรมเนียมและภาษีที่ผู้ขายชำระไว้เมื่อมีการร้องขอ" +
                "\nภายหลังจากผู้ใช้ได้ชำระค่าธรรมเนียมการใช้บริการซอฟต์แวร์ให้แก่ษริษัทฯ แล้ว เราขอสงวนสิทธิ์ไม่คืนเงินค่าธรรมเนียมดังกล่าวทุกกรณี" +
                "\n\nติดต่อ SENIORSOFT" +
                "\nหากท่านต้องการเพิกถอนความยินยอมในการใช้ข้อมูลส่วนตัวของท่าน ขอเรียกดูข้อมูลและ/ หรือข้อมูลส่วนตัว มีคำถาม ความคิดเห็น ข้อกังวล หรือขอความช่วยเหลือทางด้านเทคนิค กรุณาติดต่อเราได้ที่นี่ Tel. 02 - 692 - 5899 หรือ supportteam@seniorsoft.co.th";
            _contentView.AddSubview(lblContextT8);

            #endregion

            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
            #endregion

            #region checkView
            checkView = new UIView();
            checkView.TranslatesAutoresizingMaskIntoConstraints = false;
            checkView.BackgroundColor = UIColor.White;
            View.AddSubview(checkView);

            TermSelect = new UIImageView();
            TermSelect.Image = UIImage.FromBundle("OptionBlank");
            TermSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            checkView.AddSubview(TermSelect);


            lblTerm = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblTerm.Font = lblTerm.Font.WithSize(15);
            lblTerm.Text = "Term of Service";
            checkView.AddSubview(lblTerm);

            //OptionCheck,OptionBlank
            PrivacySelect = new UIImageView();
            PrivacySelect.Image = UIImage.FromBundle("OptionBlank");
            PrivacySelect.TranslatesAutoresizingMaskIntoConstraints = false;
            checkView.AddSubview(PrivacySelect);

            lblPrivacy = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblPrivacy.Font = lblPrivacy.Font.WithSize(15);
            lblPrivacy.Text = "Privacy Policy";
            checkView.AddSubview(lblPrivacy);

            #endregion

            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.White;
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomView);

            btnsave = new UIButton();
            btnsave.Layer.CornerRadius = 5;
            btnsave.ClipsToBounds = true;
            btnsave.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnsave.Layer.BorderWidth = 0.5f;
            btnsave.SetTitle("Accept", UIControlState.Normal);
            btnsave.BackgroundColor = UIColor.White;
            btnsave.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnsave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnsave.TouchUpInside += async (sender, e) => {
                UpdateMerchant();
            };
            BottomView.AddSubview(btnsave);
        }
        void SaveLanguage()
        {
            // get ภาษา ไว้ตอนดึง string 2 ภาษา
        }
        async void UpdateMerchant()
        {
            try
            {
                string usernamelogin = Preferences.Get("User", "");

                if (term)
                {
                    MerchantDetail.Merchant.FTermConditions = 'Y';
                }
                else
                {
                    MerchantDetail.Merchant.FTermConditions = 'N';

                }
                if (policy)
                {
                    MerchantDetail.Merchant.FPrivacyPolicy = 'Y';
                }
                else
                {
                    MerchantDetail.Merchant.FPrivacyPolicy = 'N';
                }

                MerchantDetail.Merchant.UserNameModified = usernamelogin;

                if (!term || !policy)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("cannotsave", "Failed to save"), "กรุณายอมรับเงื่อนไขก่อนใช้งาน App");
                }
                else
                {
                    var updatemerchant = await GabanaAPI.PutMerchant(MerchantDetail,null);
                    if (updatemerchant.Status)
                    {
                        Utils.ShowMessage("บันทึกข้อมูลสำเร็จ");
                        DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;
                        this.NavigationController.DismissViewController(false, null);
                        
                        //this.NavigationController.PopToRootViewController(false);
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotsave", "Failed to save"));
                        return;
                    }
                }


            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
               
            }
        }
        void setupAutoLayout()
        {
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnsave.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnsave.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnsave.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnsave.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;


            #region checkView
            checkView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            checkView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            checkView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            checkView.HeightAnchor.ConstraintEqualTo(90).Active = true;

            TermSelect.TopAnchor.ConstraintEqualTo(checkView.SafeAreaLayoutGuide.TopAnchor, 16).Active = true;
            TermSelect.WidthAnchor.ConstraintEqualTo(20).Active = true;
            TermSelect.LeftAnchor.ConstraintEqualTo(checkView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            TermSelect.HeightAnchor.ConstraintEqualTo(20).Active = true;

            lblTerm.TopAnchor.ConstraintEqualTo(checkView.SafeAreaLayoutGuide.TopAnchor, 15).Active = true;
            lblTerm.RightAnchor.ConstraintEqualTo(checkView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblTerm.LeftAnchor.ConstraintEqualTo(TermSelect.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lblTerm.HeightAnchor.ConstraintEqualTo(21).Active = true;

            PrivacySelect.TopAnchor.ConstraintEqualTo(TermSelect.SafeAreaLayoutGuide.BottomAnchor, 20).Active = true;
            PrivacySelect.WidthAnchor.ConstraintEqualTo(20).Active = true;
            PrivacySelect.LeftAnchor.ConstraintEqualTo(checkView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            PrivacySelect.HeightAnchor.ConstraintEqualTo(20).Active = true;

            lblPrivacy.TopAnchor.ConstraintEqualTo(lblTerm.SafeAreaLayoutGuide.BottomAnchor, 20).Active = true;
            lblPrivacy.RightAnchor.ConstraintEqualTo(checkView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblPrivacy.LeftAnchor.ConstraintEqualTo(PrivacySelect.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lblPrivacy.HeightAnchor.ConstraintEqualTo(21).Active = true;

            #endregion

            #region scroll bar
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(checkView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region policy
            lblHeadTerm.TopAnchor.ConstraintEqualTo(lblHeadTerm.Superview.TopAnchor, 10).Active = true;
            lblHeadTerm.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            lblHeadTerm.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            lblContext.TopAnchor.ConstraintEqualTo(lblHeadTerm.SafeAreaLayoutGuide.BottomAnchor,5).Active = true;
            lblContext.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextH1.TopAnchor.ConstraintEqualTo(lblContext.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextH1.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextH1.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContext2.TopAnchor.ConstraintEqualTo(lblContextH1.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContext2.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext2.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextH2.TopAnchor.ConstraintEqualTo(lblContext2.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextH2.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextH2.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContext3.TopAnchor.ConstraintEqualTo(lblContextH2.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContext3.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext3.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextH3.TopAnchor.ConstraintEqualTo(lblContext3.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextH3.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextH3.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContext4.TopAnchor.ConstraintEqualTo(lblContextH3.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContext4.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext4.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextH4.TopAnchor.ConstraintEqualTo(lblContext4.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextH4.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextH4.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContext5.TopAnchor.ConstraintEqualTo(lblContextH4.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContext5.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext5.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextH5.TopAnchor.ConstraintEqualTo(lblContext5.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextH5.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextH5.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContext6.TopAnchor.ConstraintEqualTo(lblContextH5.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContext6.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext6.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextH6.TopAnchor.ConstraintEqualTo(lblContext6.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextH6.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextH6.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContext7.TopAnchor.ConstraintEqualTo(lblContextH6.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContext7.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext7.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextH7.TopAnchor.ConstraintEqualTo(lblContext7.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextH7.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextH7.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContext8.TopAnchor.ConstraintEqualTo(lblContextH7.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContext8.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContext8.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;
            #endregion
            #region term
            lblHeadCondition.TopAnchor.ConstraintEqualTo(lblContext8.SafeAreaLayoutGuide.BottomAnchor, 20).Active = true;
            lblHeadCondition.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblHeadCondition.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT1.TopAnchor.ConstraintEqualTo(lblHeadCondition.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            lblContextT1.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT1.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextTH1.TopAnchor.ConstraintEqualTo(lblContextT1.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextTH1.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextTH1.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT2.TopAnchor.ConstraintEqualTo(lblContextTH1.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextT2.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT2.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextTH2.TopAnchor.ConstraintEqualTo(lblContextT2.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextTH2.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextTH2.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT3.TopAnchor.ConstraintEqualTo(lblContextTH2.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextT3.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT3.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextTH3.TopAnchor.ConstraintEqualTo(lblContextT3.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextTH3.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextTH3.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT4.TopAnchor.ConstraintEqualTo(lblContextTH3.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextT4.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT4.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextTH4.TopAnchor.ConstraintEqualTo(lblContextT4.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextTH4.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextTH4.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT5.TopAnchor.ConstraintEqualTo(lblContextTH4.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextT5.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT5.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextTH5.TopAnchor.ConstraintEqualTo(lblContextT5.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextTH5.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextTH5.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT6.TopAnchor.ConstraintEqualTo(lblContextTH5.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextT6.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT6.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextTH6.TopAnchor.ConstraintEqualTo(lblContextT6.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextTH6.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextTH6.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT7.TopAnchor.ConstraintEqualTo(lblContextTH6.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextT7.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT7.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextTH7.TopAnchor.ConstraintEqualTo(lblContextT7.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextTH7.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextTH7.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;

            lblContextT8.TopAnchor.ConstraintEqualTo(lblContextTH7.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblContextT8.RightAnchor.ConstraintEqualTo(lblContext.Superview.RightAnchor, -20).Active = true;
            lblContextT8.LeftAnchor.ConstraintEqualTo(lblContext.Superview.LeftAnchor, 20).Active = true;
            lblContextT8.BottomAnchor.ConstraintEqualTo(lblContext.Superview.BottomAnchor, 20).Active = true;
            #endregion
            #endregion

        }
        private void SetImageTermPolicy()
        {
            //setimageTerm
            if (term)
            {
                TermSelect.Image = UIImage.FromBundle("OptionCheck");
            }
            else
            {
                TermSelect.Image = UIImage.FromBundle("OptionBlank");
            }

            if (policy)
            {
                PrivacySelect.Image = UIImage.FromBundle("OptionCheck");
            }
            else
            {
                PrivacySelect.Image = UIImage.FromBundle("OptionBlank");
            }
        }
        #region Select
        [Export("TermClick:")]
        public void TermClick(UIGestureRecognizer sender)
        {
            if (term)
            {
                term = false;
                TermSelect.Image = UIImage.FromBundle("OptionBlank");
            }
            else
            {
                term = true;
                TermSelect.Image = UIImage.FromBundle("OptionCheck");
            }
        }
        [Export("PrivacyClick:")]
        public async void PrivacyClick(UIGestureRecognizer sender)
        {
            if (policy)
            {
                policy = false;
                PrivacySelect.Image = UIImage.FromBundle("OptionBlank");
            }
            else
            {
                policy = true;
                PrivacySelect.Image = UIImage.FromBundle("OptionCheck");
            }
        }
        #endregion
        
    }
}