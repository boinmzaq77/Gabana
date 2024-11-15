﻿using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class POSDummyController : UIViewController
    {
        UIView dummyNumberView,bottomView,descripView,numpadView;
        UITextField txtDescription;
        string txtDummyStr= "0.00";
        UILabel lblDescrip, lblDummy;
        UIButton btnAddDummy;
        int dotCount = 0;
        string strValue;
        private string newDiscount;
        int count = 0;
        string VatType, DECIMALPOINTDISPLAY;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete, btnDot;
        private string currency;
        private decimal dec;

        public POSDummyController()
        {

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("dummy", "Dummy"));
            this.NavigationController.SetNavigationBarHidden(false, false);
            lblDummy.Text = Utils.DisplayDecimal((decimal)0.00);
            lblDummy.TextColor = UIColor.FromRGB(134, 206, 239);
            DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;
        }
        public override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            strValue = "0";
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region dummyNumberView
            dummyNumberView = new UIView();
            dummyNumberView.BackgroundColor = UIColor.FromRGB(248,248,248);
            dummyNumberView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(dummyNumberView);

            lblDummy = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = new UIColor(red: 0 / 225f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            //lDummy.Text = txtDummyStr;
            lblDummy.Text = Utils.DisplayDecimal(0);
            lblDummy.Font = lblDummy.Font.WithSize(60);
            View.AddSubview(lblDummy);
            #endregion
            #region descripView
            descripView = new UIView();
            descripView.BackgroundColor = UIColor.White;
            descripView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(descripView);

            lblDescrip = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblDescrip.Font = lblDescrip.Font.WithSize(15);
            lblDescrip.Text = Utils.TextBundle("description", "Description");
            descripView.AddSubview(lblDescrip);

            txtDescription = new UITextField
            {
                TextAlignment = UITextAlignment.Left,
                Placeholder = Utils.TextBundle("adddescription", "Add Description"),
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtDescription.Font = lblDescrip.Font.WithSize(15);
            txtDescription.ReturnKeyType = UIReturnKeyType.Done;
            descripView.AddSubview(txtDescription);
            #endregion
            #region numpadView
            numpadView = new UIView();
            numpadView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
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
            btnAddDummy.SetTitle(Utils.TextBundle("addcart", "Add to Cart"), UIControlState.Normal);
            btnAddDummy.BackgroundColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1);
            btnAddDummy.SetTitleColor(UIColor.White,UIControlState.Normal);
            btnAddDummy.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddDummy.TouchUpInside += (sender, e) => {

                if (POSController.tranWithDetails != null)
                {
                    //สินค้าด่วน
                    //ชื่อ ,ราคา ,จำนวน ,sysitemID = 0
                    String name = "Dummy";
                    if (!string.IsNullOrEmpty(txtDescription.Text))
                    {
                        name = txtDescription.Text;
                    }

                    Item item = new Item()
                    {
                        ItemName = name,
                        Price = Convert.ToDecimal(strValue),
                        SysItemID = 0
                    };

                    // add detail Item            
                    SelectItemtoCart(item);
                    this.NavigationController.PopViewController(false);
                    //DataCashing.flagDummy = true;

                    //StartActivity(typeof(PosActivity));
                    //this.Finish();
                }

            };
            View.AddSubview(btnAddDummy);
            #endregion
            Textboxfocus(View);
            SetupAutoLayout();
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
        private void SetBtnSave()
        {
           
            if (string.IsNullOrEmpty(strValue))
            {
                return;
            }
            if (decimal.Parse(strValue) > 0)
            {
                //btnAddDummy.SetTitle(Utils.TextBundle("realpay", "") + " " + CURRENCYSYMBOLS + Utils.DisplayDecimal(decimal.Parse(strValue)), UIControlState.Normal);
                lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                //btnAddDummy.SetTitle(Utils.TextBundle("realpay", "") + " " + CURRENCYSYMBOLS + Utils.DisplayDecimal(pay), UIControlState.Normal);
                lblDummy.TextColor = UIColor.FromRGB(134, 206, 239);
            }

        }
        public void SelectItemtoCart(Item ItemSelect)
        {
            try
            {
                //--------------------------
                //เปลี่ยนสีเมื่อมีสินค้าลงตะกร้า
                //lnPayment.SetBackgroundResource(Resource.Drawable.btnblue);
                //lnPayment.SetPadding(0, 5, 0, 5);
                //textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                //txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                //-------------------------

                //tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, ItemSelect, DataCashing.setQuantityToCart);

                try
                {
                   var DetailNo = POSController.tranWithDetails.tranDetailItemWithToppings.Count + 1;

                    TranDetailItemNew DetailItem = new TranDetailItemNew()
                    {
                        SysItemID = null,
                        MerchantID = DataCashingAll.MerchantId,
                        SysBranchID = DataCashingAll.SysBranchId,
                        TranNo = POSController.tranWithDetails.tran.TranNo,
                        ItemName = ItemSelect.ItemName,
                        SaleItemType = 'D',
                        FProcess = 1,
                        TaxType = 'V',
                        Quantity = (decimal)POSController.Quantity,
                        Price = ItemSelect.Price,
                        ItemPrice = ItemSelect.Price,
                        Discount = 0,
                        EstimateCost = 0,
                        SizeName = null,
                        Comments = null,
                        DetailNo = DetailNo,
                    };

                    List<TranDetailItemTopping> tranDetailItem = new List<TranDetailItemTopping>();
                    var tranDetailItemWithTopping = new TranDetailItemWithTopping()
                    {
                        tranDetailItem = DetailItem,
                        tranDetailItemToppings = tranDetailItem,
                    };

                    POSController.tranWithDetails = BLTrans.ChooseItemTran(POSController.tranWithDetails, tranDetailItemWithTopping);
                    //DataCashing.ModifyTranOrder = true;
                    POSController.tranWithDetails = BLTrans.Caltran(POSController.tranWithDetails);

                    //DataCashing.setQuantityToCart = 1;
                    POSController.Quantity = 1;
                    CartController.Ismodify = true;
                }
                catch (Exception ex)
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                    //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        void SetupAutoLayout()
        {
            dummyNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            dummyNumberView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            dummyNumberView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            dummyNumberView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*21)/100).Active = true;

            lblDummy.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblDummy.CenterYAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDummy.HeightAnchor.ConstraintEqualTo(84).Active = true;

            descripView.TopAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            descripView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            descripView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            descripView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*9)/100).Active = true;

            lblDescrip.TopAnchor.ConstraintEqualTo(descripView.SafeAreaLayoutGuide.TopAnchor, 9).Active = true;
            lblDescrip.LeftAnchor.ConstraintEqualTo(descripView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblDescrip.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblDescrip.HeightAnchor.ConstraintEqualTo(21).Active = true;

            txtDescription.TopAnchor.ConstraintEqualTo(lblDescrip.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            txtDescription.LeftAnchor.ConstraintEqualTo(descripView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtDescription.RightAnchor.ConstraintEqualTo(descripView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
           

            numpadView.TopAnchor.ConstraintEqualTo(descripView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            numpadView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            numpadView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            numpadView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height / 10).Active = true;

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
                //0 press
                if (strValue.Length == 0)
                {
                    return;
                }
                strValue = strValue.Substring(0, strValue.Length - 1);
                //lblDummy.Text = strValue;

                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                    lblDummy.Text = Utils.DisplayDecimal((decimal)0.00);
                    lblDummy.TextColor = UIColor.FromRGB(134, 206, 239);
                }
                else
                {


                    int indexpoint = strValue.LastIndexOf(".");
                    if (indexpoint != -1)
                    {
                        var check = strValue.Split(".");
                        if (check[1].Length == 2)
                        {
                            lblDummy.Text = double.Parse(strValue).ToString("#,###.00");
                        }
                        else if (check[1].Length == 1)
                        {
                            lblDummy.Text = double.Parse(strValue).ToString("#,###.0");
                        }
                        else
                        {
                            lblDummy.Text = double.Parse(strValue).ToString("#,###") + ".";
                        }
                    }
                    else
                    {
                        lblDummy.Text = decimal.Parse(strValue).ToString("#,###");
                    }
                }
                SetBtnSave();

            };
            numpadView.AddSubview(btndelete);

            btnDot = new UIButton();
            btnDot.BackgroundColor = UIColor.White;
            btnDot.SetTitle(".", UIControlState.Normal);
            btnDot.TitleLabel.Font = btnDot.TitleLabel.Font.WithSize(30);
            btnDot.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnDot.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDot.TouchUpInside += (sender, e) => {
                //. press
                SetValue(".");

            };
            numpadView.AddSubview(btnDot);


            btnone.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnone.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnone.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*12)/100).Active = true;
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

            btnDot.TopAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnDot.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnDot.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,1).Active = true;
            btnDot.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnzero.TopAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnzero.LeftAnchor.ConstraintEqualTo(btnDot.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnzero.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnzero.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btndelete.TopAnchor.ConstraintEqualTo(btnnine.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btndelete.LeftAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btndelete.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btndelete.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
        }
        public async void SetValue(string btn)
        {
            try
            {
                if (strValue.IndexOf(".") != -1)
                {
                    var check = strValue.Split(".");
                    if (check[1].Length == 2)
                    {
                        return;
                    }
                    if (btn.ToString() == ".")
                    {
                        return;
                    }
                }

                string amount = ""; ;
                if (strValue == "0")
                {
                    amount = "";
                }
                else
                {
                    amount = strValue;
                }

                var num = btn.ToString();
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

                if ((decimal)(Convert.ToDouble(amount)) < 100000000)
                {
                    strValue = amount;
                }
                else
                {
                    strValue = "99999999.99";
                }
                //strValue = amount;

                int indexpoint = strValue.LastIndexOf(".");
                if (indexpoint != -1)
                {
                    var check = strValue.Split(".");
                    if (check[1].Length == 2)
                    {
                        lblDummy.Text = double.Parse(strValue).ToString("#,###.00");
                    }
                    else if (check[1].Length == 1)
                    {
                        lblDummy.Text = double.Parse(strValue).ToString("#,###.0");
                    }
                    else
                    {
                        lblDummy.Text = double.Parse(strValue).ToString("#,###") + ".";
                    }
                }
                else
                {
                    lblDummy.Text = decimal.Parse(amount).ToString("#,###");
                }


                SetBtnSave();
                //SetBtnCharge();
                //SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}