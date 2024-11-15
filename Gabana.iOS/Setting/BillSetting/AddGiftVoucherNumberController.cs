using Foundation;
using Gabana.iOS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class AddGiftVoucherNumberController : UIViewController
    {
        bool flagIncludeVatSet = true;
        int dotCount = 0;
        UIView dummyNumberView,bottomView,numpadView;
        string txtDummyStr="0";
        string strValue = "0.00";
        private string newDiscount;
        int count = 0;
        decimal dec;
        private char taxType;
        public static double vat; //amount คือ ยอดที่ต้องจ่าย   
        UILabel lblDummy, lblAmount;
        UIButton btnAddDummy, btnDot;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete;
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string Vat;
        string VatType, DECIMALPOINTDISPLAY;
        string currency;
        CashTemplate cashguide;
        CashTemplateManage CashTemplateManage = new CashTemplateManage();

        public UILabel lblAmount2 { get; private set; }

        public AddGiftVoucherNumberController()
        {
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (cashguide==null)
            {
                Utils.SetTitle(this.NavigationController, "Add Cash");
            }
            else
            {
                Utils.SetTitle(this.NavigationController, "Edit Cash");
            }
            
            this.NavigationController.SetNavigationBarHidden(false, false);
            DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;
        }
        public override async void ViewDidLoad()
        {
            try
            {
                UIBarButtonItem clearVat = new UIBarButtonItem();
                clearVat.Title = "Clear";
                clearVat.Clicked += (sender, e) => {
                    txtDummyStr = "0.00";
                    lblDummy.Text = "0.00";
                    dotCount = 0;
                };
                this.NavigationItem.RightBarButtonItem = clearVat;

                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.NavigationBar.TintColor = UIColor.FromRGB(0, 149, 218);
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.White;

                #region dummyNumberView
                dummyNumberView = new UIView();
                dummyNumberView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                dummyNumberView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(dummyNumberView);

                lblDummy = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblDummy.Text = txtDummyStr;
                lblDummy.Font = lblDummy.Font.WithSize(60);
                dummyNumberView.AddSubview(lblDummy);

                lblAmount = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(162, 162, 162),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblAmount.Font = lblDummy.Font.WithSize(15);
                lblAmount.Text = Utils.TextBundle("giftvoucher", "Gift Voucher");
                dummyNumberView.AddSubview(lblAmount);


                



                #endregion

                #region numpadView
                numpadView = new UIView();
                numpadView.BackgroundColor = UIColor.White;
                numpadView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(numpadView);
                #endregion

                #region bottomView
                bottomView = new UIView();
                bottomView.BackgroundColor = UIColor.White;
                bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(bottomView);

                btnAddDummy = new UIButton();
                btnAddDummy.Layer.CornerRadius = 5;
                btnAddDummy.Layer.BorderColor = UIColor.FromRGB(51, 170, 225).CGColor;
                btnAddDummy.Layer.BorderWidth = 0.4f;
                btnAddDummy.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
                btnAddDummy.BackgroundColor = UIColor.White;
                btnAddDummy.SetTitleColor(UIColor.FromRGB(51, 170, 225), UIControlState.Normal);
                btnAddDummy.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAddDummy.TouchUpInside += async (sender, e) => {
                    try
                    {
                        AddGiftVoucherController.editnum = true;
                        AddGiftVoucherController.num = lblDummy.Text;
                        this.NavigationController.PopViewController(false);
                    }
                    catch (Exception ex)
                    {
                        btnAddDummy.Enabled = true;
                        _ = TinyInsights.TrackErrorAsync(ex);
                        Utils.ShowMessage(ex.Message);
                        //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }

                };
                View.AddSubview(btnAddDummy);
                #endregion
                Textboxfocus(View);
                SetupAutoLayout();
                lblDummy.Text = Utils.DisplayDecimal(0);
                
                strValue = lblDummy.Text;
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;
                currency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        dec = (decimal)0.0001;
                    }
                    else
                    {
                        dec = (decimal)0.01;
                    }
                }
                else
                {
                    dec = (decimal)0.01;
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        

        
        
        private void SetBtnSave()
        {
            Vat = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
            if (lblDummy.Text != Vat && lblDummy.Text != "")
            {
                btnAddDummy.Enabled = true;
                btnAddDummy.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                btnAddDummy.SetTitleColor(UIColor.White, UIControlState.Normal);
            }
            else
            {
                btnAddDummy.Enabled = false;
                btnAddDummy.BackgroundColor = UIColor.White;
                btnAddDummy.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            }
        }

        void SetupAutoLayout()
        {
            dummyNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            dummyNumberView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            dummyNumberView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            dummyNumberView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*3)/10).Active = true;

            lblDummy.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor,0).Active = true;
            lblDummy.CenterYAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterYAnchor,-10).Active = true;

            
            lblAmount.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor,0).Active = true;
            lblAmount.BottomAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.TopAnchor,-17).Active = true;

            //lblAmount2.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            //lblAmount2.TopAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.BottomAnchor, 17).Active = true;


            numpadView.TopAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            numpadView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            numpadView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            numpadView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*97)/1000).Active = true;

            btnAddDummy.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddDummy.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddDummy.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor,-10).Active = true;
            btnAddDummy.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor,-10).Active = true;

            NumberpadSetup();
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        void NumberpadSetup()
        {
            btnone = new UIButton();
            btnone.BackgroundColor = UIColor.White;
            btnone.TitleLabel.Font = btnone.TitleLabel.Font.WithSize(30);
            btnone.SetTitle("1", UIControlState.Normal);
            btnone.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnone.TranslatesAutoresizingMaskIntoConstraints = false;
            btnone.TouchUpInside += (sender, e) => {
                //1 press
                SetValue("1");

            };
            numpadView.AddSubview(btnone);

            btntwo = new UIButton();
            btntwo.BackgroundColor = UIColor.White;
            btntwo.TitleLabel.Font = btntwo.TitleLabel.Font.WithSize(30);
            btntwo.SetTitle("2", UIControlState.Normal);
            btntwo.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btntwo.TranslatesAutoresizingMaskIntoConstraints = false;
            btntwo.TouchUpInside += (sender, e) => {
                //2 press
                SetValue("2");

            };
            numpadView.AddSubview(btntwo);

            btnthree = new UIButton();
            btnthree.BackgroundColor = UIColor.White;
            btnthree.SetTitle("3", UIControlState.Normal);
            btnthree.TitleLabel.Font = btnthree.TitleLabel.Font.WithSize(30);
            btnthree.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnthree.TranslatesAutoresizingMaskIntoConstraints = false;
            btnthree.TouchUpInside += (sender, e) => {
                //3 press
                SetValue("3");
            };
            numpadView.AddSubview(btnthree);

            btnfour = new UIButton();
            btnfour.BackgroundColor = UIColor.White;
            btnfour.SetTitle("4", UIControlState.Normal);
            btnfour.TitleLabel.Font = btnfour.TitleLabel.Font.WithSize(30);
            btnfour.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnfour.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfour.TouchUpInside += (sender, e) => {
                //4 press
                SetValue("4");
            };
            numpadView.AddSubview(btnfour);

            btnfive = new UIButton();
            btnfive.BackgroundColor = UIColor.White;
            btnfive.SetTitle("5", UIControlState.Normal);
            btnfive.TitleLabel.Font = btnfive.TitleLabel.Font.WithSize(30);
            btnfive.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnfive.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfive.TouchUpInside += (sender, e) => {
                //5 press
                SetValue("5");
            };
            numpadView.AddSubview(btnfive);

            btnsix = new UIButton();
            btnsix.BackgroundColor = UIColor.White;
            btnsix.TitleLabel.Font = btnsix.TitleLabel.Font.WithSize(30);
            btnsix.SetTitle("6", UIControlState.Normal);
            btnsix.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnsix.TranslatesAutoresizingMaskIntoConstraints = false;
            btnsix.TouchUpInside += (sender, e) => {
                SetValue("6");
            };
            numpadView.AddSubview(btnsix);

            btnseven = new UIButton();
            btnseven.BackgroundColor = UIColor.White;
            btnseven.SetTitle("7", UIControlState.Normal);
            btnseven.TitleLabel.Font = btnseven.TitleLabel.Font.WithSize(30);
            btnseven.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnseven.TranslatesAutoresizingMaskIntoConstraints = false;
            btnseven.TouchUpInside += (sender, e) => {
                //7 press
                SetValue("7");
            };
            numpadView.AddSubview(btnseven);

            btneight = new UIButton();
            btneight.BackgroundColor = UIColor.White;
            btneight.TitleLabel.Font = btneight.TitleLabel.Font.WithSize(30);
            btneight.SetTitle("8", UIControlState.Normal);
            btneight.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btneight.TranslatesAutoresizingMaskIntoConstraints = false;
            btneight.TouchUpInside += (sender, e) => {
                //8 press
                SetValue("8");
            };
            numpadView.AddSubview(btneight);

            btnnine = new UIButton();
            btnnine.BackgroundColor = UIColor.White;
            btnnine.TitleLabel.Font = btnnine.TitleLabel.Font.WithSize(30);
            btnnine.SetTitle("9", UIControlState.Normal);
            btnnine.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnnine.TranslatesAutoresizingMaskIntoConstraints = false;
            btnnine.TouchUpInside += (sender, e) => {
                //9 press
                SetValue("9");
            };
            numpadView.AddSubview(btnnine);

            btnzero = new UIButton();
            btnzero.BackgroundColor = UIColor.White;
            btnzero.TitleLabel.Font = btnzero.TitleLabel.Font.WithSize(30);
            btnzero.SetTitle("0", UIControlState.Normal);
            btnzero.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnzero.TranslatesAutoresizingMaskIntoConstraints = false;
            btnzero.TouchUpInside += (sender, e) => {
                //0 press
                SetValue("0");
            };
            numpadView.AddSubview(btnzero);

            btndelete = new UIButton();
            btndelete.SetImage(UIImage.FromBundle("Del"), UIControlState.Normal);
            btndelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            btndelete.BackgroundColor = UIColor.White;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.TouchUpInside += (sender, e) => {

                int indexpoint = strValue.LastIndexOf(".");
                int indexclear = 0;
                decimal damount;
                string amount;
                //double damount = Convert.ToDouble(txtServiceCharge.Text) * 100;
                if (strValue.Contains('%'))
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    //txtAmoount.Text = Utils.DisplayDecimal(damount * (decimal)0.01);
                    lblDummy.Text = Utils.DisplayDecimal(damount);
                    strValue = lblDummy.Text;
                    //lblDummy.Focusable = true;
                    indexclear = lblDummy.Text.LastIndexOf(".");
                    return;
                }
                if (strValue != string.Empty && strValue.Length > 1)
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    lblDummy.Text = Utils.DisplayDecimal(damount);
                    strValue = lblDummy.Text;
                    //txtAmoount.Focusable = true;
                    indexclear = lblDummy.Text.LastIndexOf(".");
                }
                else
                {
                    strValue = "";
                    lblDummy.Text = strValue;
                    //lblDummy.Focusable = true;
                }
                if (lblDummy.Text != "")
                {
                    damount = Convert.ToDecimal(lblDummy.Text);
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        amount = (damount * 1000).ToString();
                    }
                    else
                    {
                        amount = (damount * 10).ToString();
                    }
                }
                else
                {
                    amount = "0";
                }

                lblDummy.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                strValue = lblDummy.Text;

                if (indexpoint > indexclear)
                {
                    count = 0;
                }
                SetBtnSave();
            };
            numpadView.AddSubview(btndelete);

            btnDot = new UIButton();
            btnDot.BackgroundColor = UIColor.White;
            btnDot.SetTitle("%", UIControlState.Normal);
            btnDot.TitleLabel.Font = btnDot.TitleLabel.Font.WithSize(30);
            btnDot.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnDot.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDot.TouchUpInside += (sender, e) => {
                SetValue("%");
            };
            numpadView.AddSubview(btnDot);

            btnone.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnone.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnone.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnone.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btntwo.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btntwo.LeftAnchor.ConstraintEqualTo(btnone.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btntwo.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btntwo.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnthree.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnthree.LeftAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnthree.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnthree.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnfour.TopAnchor.ConstraintEqualTo(btnone.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnfour.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnfour.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnfour.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnfive.TopAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnfive.LeftAnchor.ConstraintEqualTo(btnfour.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnfive.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnfive.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnsix.TopAnchor.ConstraintEqualTo(btnthree.BottomAnchor, 1).Active = true;
            btnsix.LeftAnchor.ConstraintEqualTo(btnfive.RightAnchor, 1).Active = true;
            btnsix.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnsix.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnseven.TopAnchor.ConstraintEqualTo(btnfour.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnseven.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnseven.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnseven.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btneight.TopAnchor.ConstraintEqualTo(btnfive.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btneight.LeftAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btneight.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btneight.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnnine.TopAnchor.ConstraintEqualTo(btnsix.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnnine.LeftAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnnine.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnnine.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnzero.TopAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnzero.LeftAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnzero.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnzero.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btndelete.TopAnchor.ConstraintEqualTo(btnnine.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btndelete.LeftAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btndelete.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btndelete.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnDot.TopAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnDot.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnDot.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnDot.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public async void SetValue(string btn)
        {
            try
            {
                


                if (strValue == "0" && btn != ".")
                {
                    strValue = string.Empty;
                    lblDummy.Text = strValue;
                }
                string amount;
                if (lblDummy.Text != "")
                {
                    if (lblDummy.Text.Contains('%'))
                    {
                        var data = lblDummy.Text.Replace("%", "");
                        var damount = Convert.ToDouble(data);
                        if (DECIMALPOINTDISPLAY == "4")
                        {
                            amount = (damount * 10000).ToString() + "%";
                        }
                        else
                        {
                            amount = (damount * 100).ToString() + "%";
                        }
                    }
                    else
                    {
                        var damount = Convert.ToDouble(lblDummy.Text);
                        if (DECIMALPOINTDISPLAY == "4")
                        {
                            amount = (damount * 10000).ToString();
                        }
                        else
                        {
                            amount = (damount * 100).ToString();
                        }
                    }
                }
                else
                {
                    amount = "0";
                }
                var num = btn.ToString();


                if (count == 0)
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            count++;
                            break;
                    }
                }
                else
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            break;
                    }
                }
                if (amount.Contains('%'))
                {
                    var data = amount.Replace("%", "");
                    lblDummy.Text = Utils.DisplayDecimal(Convert.ToDecimal(data) * dec) + "%";
                    strValue = lblDummy.Text;
                    newDiscount = strValue;
                }
                else
                {
                    lblDummy.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                    strValue = lblDummy.Text;
                    newDiscount = strValue;
                }
                    

                //CheckDataChange();
                SetBtnSave();

                
                if (strValue.Contains('%'))
                {
                    decimal maxValue = 100;
                    var data = strValue.Replace("%", "");
                    if (maxValue < Convert.ToDecimal(data))
                    {
                        var checklenght = Utils.CheckLenghtValue(strValue);
                        //Toast.MakeText(this, Application.Context.GetString(Resource.String.maxdiscount) + Utils.DisplayDecimal(maxValue) + "%", ToastLength.Short).Show();
                        lblDummy.Text = Utils.DisplayDecimal(maxValue) + "%";
                        strValue = lblDummy.Text;
                        return;
                    }
                }
                else
                {
                    //Currency
                    // 6 หลัก 999,999.displayDecimal  
                    string maxdata;
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        maxdata = Utils.DisplayDecimal((decimal)999999.9999);
                    }
                    else
                    {
                        maxdata = Utils.DisplayDecimal((decimal)999999.99);
                    }

                    if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                    {
                        //Toast.MakeText(this, Application.Context.GetString(Resource.String.maxservicecharg) + maxdata, ToastLength.Short).Show();
                        lblDummy.Text = maxdata;
                        strValue = lblDummy.Text;
                        return;
                    }
                }
                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at addDiscount");
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}