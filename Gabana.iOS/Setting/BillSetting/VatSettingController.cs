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
    public partial class VatSettingController : UIViewController
    {
        bool flagIncludeVatSet = true;
        int dotCount = 0;
        UIView dummyNumberView,bottomView,numpadView;
        string txtDummyStr="0";
        string strValue = "0.00";
        int count = 0;
        private char taxType;
        public static double vat; //amount คือ ยอดที่ต้องจ่าย   
        UILabel lblDummy, lblAmount;
        UIButton btnAddDummy,btnIncludeVat,btnExcludeVat, btnDot;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete;
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string Vat;
        string VatType;
        public VatSettingController()
        {
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
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
                lblAmount.Text = "VAT (%)";
                dummyNumberView.AddSubview(lblAmount);

                btnIncludeVat = new UIButton();
                btnIncludeVat.Layer.CornerRadius = 7;
                btnIncludeVat.SetTitle("Include Vat", UIControlState.Normal);
                btnIncludeVat.Layer.BorderColor = UIColor.FromRGB(51, 170, 225).CGColor;
                btnIncludeVat.Layer.BorderWidth = 0.4f;
                btnIncludeVat.TranslatesAutoresizingMaskIntoConstraints = false;
                btnIncludeVat.TouchUpInside += async (sender, e) => {
                    flagIncludeVatSet = true;
                    btnIncludeVat.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                    btnIncludeVat.SetTitleColor(UIColor.White, UIControlState.Normal);

                    btnExcludeVat.BackgroundColor = UIColor.White;
                    btnExcludeVat.SetTitleColor(UIColor.FromRGB(51, 170, 225), UIControlState.Normal);
                    taxType = 'I';
                };
                dummyNumberView.AddSubview(btnIncludeVat);

                btnExcludeVat = new UIButton();
                btnExcludeVat.Layer.CornerRadius = 7;
                btnExcludeVat.SetTitle("Exclude Vat", UIControlState.Normal);
                btnExcludeVat.Layer.BorderColor  = UIColor.FromRGB(51, 170, 225).CGColor;
                btnExcludeVat.Layer.BorderWidth =0.4f;
                btnExcludeVat.TranslatesAutoresizingMaskIntoConstraints = false;
                btnExcludeVat.TouchUpInside += async (sender, e) => {
                    flagIncludeVatSet = false;
                    btnExcludeVat.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                    btnExcludeVat.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnIncludeVat.BackgroundColor = UIColor.White;
                    btnIncludeVat.SetTitleColor(UIColor.FromRGB(51, 170, 225), UIControlState.Normal);
                    taxType = 'E';
                };
                dummyNumberView.AddSubview(btnExcludeVat);

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
                        //string[] substring = lblDummy.Text.Split(".");
                        decimal TAXRATE = decimal.Parse(lblDummy.Text);
                        lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                        merchantConfig = new ORM.Master.MerchantConfig()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            CfgKey = "TAXRATE",
                            CfgFloat = TAXRATE
                        };
                        lstmerchantConfig.Add(merchantConfig);

                        merchantConfig = new ORM.Master.MerchantConfig()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            CfgKey = "TAXTYPE",
                            CfgString = taxType.ToString()
                        };
                        lstmerchantConfig.Add(merchantConfig);

                        var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                        if (update.Status)
                        {
                            //Insert to Local DB
                            List<MerchantConfig> lstlocal = new List<MerchantConfig>();
                            MerchantConfig localConfig = new MerchantConfig()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                CfgKey = "TAXRATE",
                                CfgFloat = TAXRATE
                            };
                            lstlocal.Add(localConfig);

                            MerchantConfig localConfig2 = new MerchantConfig()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                CfgKey = "TAXTYPE",
                                CfgString = taxType.ToString()
                            };
                            lstlocal.Add(localConfig2);

                            var localVAT = await configManage.InsertorReplaceListMerchantConfig(lstlocal);
                            if (localVAT)
                            {
                                Vat = TAXRATE.ToString();
                                VatType = taxType.ToString();
                                DataCashingAll.setmerchantConfig.TAXRATE = Vat;
                                DataCashingAll.setmerchantConfig.TAXTYPE = VatType;
                                Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                                this.NavigationController.PopViewController(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        Utils.ShowMessage(ex.Message);
                        //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }

                };
                View.AddSubview(btnAddDummy);
                #endregion
                Textboxfocus(View);
                SetupAutoLayout();

               

                Vat = DataCashingAll.setmerchantConfig.TAXRATE;
                VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
                if (VatType == "I")
                {
                    btnIncludeVat.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                    btnIncludeVat.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnExcludeVat.BackgroundColor = UIColor.White;
                    btnExcludeVat.SetTitleColor(UIColor.FromRGB(51, 170, 225), UIControlState.Normal);
                }
                else
                {
                    btnExcludeVat.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                    btnExcludeVat.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnIncludeVat.BackgroundColor = UIColor.White;
                    btnIncludeVat.SetTitleColor(UIColor.FromRGB(51, 170, 225), UIControlState.Normal);
                }
                if (VatType == null )
                {
                    taxType = 'I';
                }
                else
                {
                    taxType = char.Parse(VatType);
                }
                lblDummy.Text = decimal.Parse(Vat).ToString("#,##0.00");
                strValue = lblDummy.Text;
                
                
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        private void SetBtnSave()
        {
            Vat = DataCashingAll.setmerchantConfig.TAXRATE;
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

            btnIncludeVat.TopAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnIncludeVat.WidthAnchor.ConstraintEqualTo(140).Active = true;
            btnIncludeVat.RightAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, -5).Active = true;
            btnIncludeVat.HeightAnchor.ConstraintEqualTo(40).Active = true;

            btnExcludeVat.TopAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnExcludeVat.WidthAnchor.ConstraintEqualTo(140).Active = true;
            btnExcludeVat.LeftAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, 5).Active = true;
            btnExcludeVat.HeightAnchor.ConstraintEqualTo(40).Active = true;

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
                double damount;
                string amount;
                //double damount = Convert.ToDouble(txtServiceCharge.Text) * 100;
                if (lblDummy.Text.Contains('%'))
                {
                    string[] split = lblDummy.Text.Split('%');
                    damount = Convert.ToDouble(split[0]) * 1000;
                }
                else
                {
                    damount = Convert.ToDouble(lblDummy.Text) * 100;
                }
                strValue = damount.ToString();
                if (strValue != string.Empty && strValue.Length > 1)
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDouble(amount);
                    lblDummy.Text = (damount * 0.01).ToString("#,##0.00");
                    //lblDummy.Focusable = true;
                    indexclear = lblDummy.Text.LastIndexOf(".");
                }
                else
                {
                    strValue = "0.00";
                    lblDummy.Text = strValue;
                    //txtServiceCharge.Focusable = true;
                }

                if (indexpoint > indexclear)
                {
                    count = 0;
                }
                SetBtnSave();
            };
            numpadView.AddSubview(btndelete);

            btnDot = new UIButton();
            btnDot.BackgroundColor = UIColor.White;
            btnDot.SetTitle("", UIControlState.Normal);
            btnDot.TitleLabel.Font = btnDot.TitleLabel.Font.WithSize(30);
            btnDot.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnDot.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDot.TouchUpInside += (sender, e) => {

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
                    lblDummy.Text = "0.00";
                }
                string amount;
                if (lblDummy.Text != "")
                {
                    if (lblDummy.Text.Contains('%'))
                    {
                        string[] split = lblDummy.Text.Split('%');
                        var damount = Convert.ToDouble(split[0]);
                        amount = (damount * 1000).ToString();
                    }
                    else
                    {
                        var damount = Convert.ToDouble(lblDummy.Text);
                        amount = (damount * 100).ToString();
                    }
                }
                else
                {
                    amount = "";
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
                    //string[] splitper = amount.Split('%');
                    //lblDummy.Text = ((Convert.ToInt32(splitper[0])) * 0.01).ToString("#,##0.00") + "%";
                    //strValue = lblDummy.Text;

                    if ((decimal)(Convert.ToDouble(amount) * 0.01) < 100)
                    {
                        string[] splitper = amount.Split('%');
                        lblDummy.Text = ((Convert.ToInt32(splitper[0])) * 0.01).ToString("#,##0.00");
                        strValue = lblDummy.Text;
                    }
                    else
                    {
                        string[] splitper = amount.Split('%');
                        lblDummy.Text = ((Convert.ToInt32(9999)) * 0.01).ToString("#,##0.00");
                        strValue = lblDummy.Text;
                    }
                }
                else
                {

                    if ((decimal)(Convert.ToDouble(amount) * 0.01) < 100)
                    {
                        lblDummy.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble(amount) * 0.01));
                        strValue = lblDummy.Text;
                    }
                    else
                    {
                        lblDummy.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble("9999") * 0.01));
                        strValue = lblDummy.Text;
                    }
                }

                //SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}