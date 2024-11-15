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
    public partial class ServiceChargeController : UIViewController
    {
        UIView dummyNumberView,bottomView,numpadView;
        string txtDummyStr="0";
        int count = 0;
        string strValue = "0.00";
        UILabel lblDummy, lblAmount;
        UIButton btnAddDummy,btnBath,btnPercent;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete, btnper;
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string SERVICECHARGERATE;
        string SERVICECHARGETYPE;
        string typeServiceCharge;
        public ServiceChargeController()
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
                    lblDummy.Text = "0.00";
                    lblDummy.TextColor = UIColor.FromRGB(138, 211, 245);
                };
                this.NavigationItem.RightBarButtonItem = clearVat;

                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                SERVICECHARGERATE = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE; //อัตราการคิด service charge
                SERVICECHARGETYPE = DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE; //รูปแบบการคิด service charge A,B           
                typeServiceCharge = SERVICECHARGETYPE;
                #region dummyNumberView
                dummyNumberView = new UIView();
                dummyNumberView.BackgroundColor = UIColor.Clear;
                dummyNumberView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(dummyNumberView);

                lblDummy = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    Text = "0.00",
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                if (string.IsNullOrEmpty( SERVICECHARGERATE))
                {
                    lblDummy.Text = "0.00";
                }
                else
                {
                    lblDummy.Text = SERVICECHARGERATE;

                }
                if (SERVICECHARGERATE != "0.00")
                {
                    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    lblDummy.TextColor = UIColor.FromRGB(138, 211, 245);
                }
                lblDummy.Font = lblDummy.Font.WithSize(60);
                dummyNumberView.AddSubview(lblDummy);



                lblAmount = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(162, 162, 162),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblAmount.Font = lblDummy.Font.WithSize(15);
                lblAmount.Text = Utils.TextBundle("servicecharge", "Service Charge");
                dummyNumberView.AddSubview(lblAmount);

                //btnBath,btnPercent;
                btnBath = new UIButton();
                btnBath.BackgroundColor = UIColor.White;
                btnBath.Layer.CornerRadius = 7;
                btnBath.Layer.BorderWidth = 1;
                btnBath.Layer.BorderColor = UIColor.FromRGB(0,149,218).CGColor;
                btnBath.ClipsToBounds = true;
                btnBath.TitleLabel.Font = btnBath.TitleLabel.Font.WithSize(20);
                btnBath.SetTitle(Utils.TextBundle("beforedis", "Before Discount"), UIControlState.Normal);
                btnBath.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
                btnBath.TranslatesAutoresizingMaskIntoConstraints = false;
                btnBath.TouchUpInside += BtnBefore_Click;
                dummyNumberView.AddSubview(btnBath);

                btnPercent = new UIButton();
                btnPercent.BackgroundColor = UIColor.White;
                btnPercent.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnPercent.Layer.BorderWidth = 1;
                btnPercent.Layer.CornerRadius = 7;
                btnPercent.ClipsToBounds = true;
                btnPercent.TitleLabel.Font = btnPercent.TitleLabel.Font.WithSize(20);
                btnPercent.SetTitle(Utils.TextBundle("afterdis", "After Discount"), UIControlState.Normal);
                btnPercent.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
                btnPercent.TranslatesAutoresizingMaskIntoConstraints = false;
                btnPercent.TouchUpInside += BtnAfter_Click;
                dummyNumberView.AddSubview(btnPercent);
                if (typeServiceCharge == "A")
                {
                    btnPercent.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    btnPercent.SetTitleColor(UIColor.White, UIControlState.Normal);
                }
                else
                {
                    btnBath.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    btnBath.SetTitleColor(UIColor.White, UIControlState.Normal);
                }
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
                    UpdateMerchantConfig();
                };
                View.AddSubview(btnAddDummy);
                #endregion
                Textboxfocus(View);
                SetupAutoLayout();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        private void BtnAfter_Click(object sender, EventArgs e)
        {
            typeServiceCharge = "A";
            btnPercent.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnPercent.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnBath.BackgroundColor = UIColor.White;
            btnBath.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
        }

        private void BtnBefore_Click(object sender, EventArgs e)
        {
            typeServiceCharge = "B";
            btnBath.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnBath.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnPercent.BackgroundColor = UIColor.White;
            btnPercent.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
        }
        async void UpdateMerchantConfig()
        {
            try
            {
                //string[] substring = strValue.Split(".");
                //int TAXRATE = Convert.ToInt32(substring[0]);
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SERVICECHARGE_TYPE",
                    CfgString = typeServiceCharge
                };
                lstmerchantConfig.Add(merchantConfig);

                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SERVICECHARGE_RATE",
                    CfgString = lblDummy.Text
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
                        CfgKey = "SERVICECHARGE_TYPE",
                        CfgString = typeServiceCharge
                    };
                    lstlocal.Add(localConfig);

                    MerchantConfig localConfig2 = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "SERVICECHARGE_RATE",
                        CfgString = lblDummy.Text
                    };
                    lstlocal.Add(localConfig2);

                    var localVAT = await configManage.InsertorReplaceListMerchantConfig(lstlocal);
                    if (localVAT)
                    {
                        SERVICECHARGERATE = lblDummy.Text;
                        SERVICECHARGETYPE = typeServiceCharge.ToString();
                       
                    }
                    DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE = SERVICECHARGERATE;
                    DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE= SERVICECHARGETYPE;
                    Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                    this.NavigationController.PopViewController(false);
                }
                else
                {
                    Utils.ShowMessage(update.Message);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        void SetupAutoLayout()
        {
            dummyNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            dummyNumberView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            dummyNumberView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            dummyNumberView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*3)/10).Active = true;

            lblDummy.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor,0).Active = true;
            lblDummy.CenterYAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDummy.HeightAnchor.ConstraintEqualTo(71).Active = true;

            lblAmount.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblAmount.BottomAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.TopAnchor,-17).Active = true;
            

            btnBath.RightAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, -5).Active = true;
            btnBath.LeftAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnBath.HeightAnchor.ConstraintEqualTo(40).Active = true;
            //btnBath.WidthAnchor.ConstraintEqualTo(100).Active = true;
            btnBath.TopAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;

            btnPercent.LeftAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, 5).Active = true;
            btnPercent.RightAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnPercent.HeightAnchor.ConstraintEqualTo(40).Active = true;
            //btnPercent.WidthAnchor.ConstraintEqualTo(100).Active = true;
            btnPercent.TopAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;

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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '1';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
                //btnAddDummy.SetTitle("Charge ฿" + txtDummyStr, UIControlState.Normal);
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '2';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '3';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '4';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '5';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    //6 press
                //    txtDummyStr = txtDummyStr + '6';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '7';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '8';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '9';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
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
                //if (txtDummyStr.Length > 0 && txtDummyStr.Length < 10)
                //{
                //    if (txtDummyStr.Length == 1 && txtDummyStr == "0")
                //    {
                //        txtDummyStr = "";
                //    }
                //    txtDummyStr = txtDummyStr + '0';
                //    lblDummy.Text = txtDummyStr;
                //    lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
                //}
                SetValue("0");
            };
            numpadView.AddSubview(btnzero);

            btndelete = new UIButton();
            btndelete.SetImage(UIImage.FromBundle("PincodeDelete.png"), UIControlState.Normal);
            btndelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            btndelete.BackgroundColor = UIColor.White;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.TouchUpInside += (sender, e) => {
                //0 press
                //if (txtDummyStr.Length > 0 && txtDummyStr != "0")
                //{
                //    txtDummyStr = txtDummyStr.Remove(txtDummyStr.Length - 1, 1);
                //}
                //if (txtDummyStr.Length == 0 && txtDummyStr == "")
                //{
                //    txtDummyStr = "0"; 
                //    lblDummy.TextColor = UIColor.FromRGB(138, 211, 245);
                //}
                //lblDummy.Text = txtDummyStr;
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
                SetBtnClear();
                SetBtnSave();

            };
            numpadView.AddSubview(btndelete);

            btnper = new UIButton();
            btnper.BackgroundColor = UIColor.White;
            btnper.TitleLabel.Font = btnnine.TitleLabel.Font.WithSize(30);
            btnper.SetTitle("%", UIControlState.Normal);
            btnper.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnper.TranslatesAutoresizingMaskIntoConstraints = false;
            btnper.TouchUpInside += (sender, e) => {
                //9 press
                SetValue("%");
            };
            numpadView.AddSubview(btnper);


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

            //btndelete.TopAnchor.ConstraintEqualTo(btnnine.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            //btndelete.LeftAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            //btndelete.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            //btndelete.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btndelete.CenterYAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btndelete.CenterXAnchor.ConstraintEqualTo(btnnine.SafeAreaLayoutGuide.CenterXAnchor).Active = true;          
            btndelete.HeightAnchor.ConstraintEqualTo(30).Active = true;
            btndelete.WidthAnchor.ConstraintEqualTo(30).Active = true;

            btnper.CenterYAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnper.CenterXAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnper.HeightAnchor.ConstraintEqualTo(30).Active = true;
            btnper.WidthAnchor.ConstraintEqualTo(30).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        private void SetBtnClear()
        {
            if (lblDummy.Text != "0.00")
            {
                lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                lblDummy.TextColor = UIColor.FromRGB(138, 211, 245);
            }
        }

        private void SetBtnSave()
        {
            SERVICECHARGERATE = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
            if (lblDummy.Text != SERVICECHARGERATE && lblDummy.Text != "")
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
        public async void SetValue(string btn)
        {
            try
            {
                if (strValue == "0" && btn!= ".")
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
                    string[] splitper = amount.Split('%');
                    if ((decimal)(Convert.ToDouble(splitper[0]) * 0.01) < 100)
                    {
                        lblDummy.Text = ((Convert.ToInt32(splitper[0])) * 0.01).ToString("#,##0.00") + "%";
                        strValue = lblDummy.Text;
                    }
                    else
                    {
                        lblDummy.Text = ((Convert.ToInt32(9999)) * 0.01).ToString("#,##0.00") + "%";
                        strValue = lblDummy.Text;
                    }
                }
                else
                {
                    if ((decimal)(Convert.ToDouble(amount) * 0.01) < 1000000)
                    {
                        lblDummy.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble(amount) * 0.01));
                        strValue = lblDummy.Text;
                    }
                    else
                    {
                        lblDummy.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble("99999999") * 0.01));
                        strValue = lblDummy.Text;
                    }
                    
                }

                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}