using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class DiscountController : UIViewController
    {
        UIView dummyNumberView,bottomView,discpuntTypeView,numpadView;
        static int flag=0; //0 = amount , 1 = percent
        char type;
        
        int count = 0;
        string strValue = "0.00";
        string txtDummyStr="0";
        UILabel lblDescrip, lblDummy, txtDescription,lblDiscount,lblType;
        UIImageView typeImg;
        UIButton btnbath ,btnpersen , btnAddDummy;
        int dotCount = 0;
        public bool fnew = true  ; 
        TranTradDiscount thisdiscount;
        Model.TranDetailItemNew tranDetailItem;
        Model.TranWithDetailsLocal tranWithDetailsLocal;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete, btnDot;
        public DiscountController(Model.TranDetailItemNew tranDetailItem)
        {

            this.tranDetailItem = tranDetailItem;
            this.tranWithDetailsLocal = null;
        }
        public DiscountController(Model.TranWithDetailsLocal tranWithDetailsLocal)
        {
            this.tranWithDetailsLocal = tranWithDetailsLocal;
            this.tranDetailItem = null;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("discount", "Items"));
        }
        public override void ViewDidLoad()
        {
            UIBarButtonItem clearVat = new UIBarButtonItem();
            clearVat.Title = Utils.TextBundle("clear", "Items");
            clearVat.Clicked += (sender, e) => {
                strValue = Utils.DisplayDecimal(0);
                lblDummy.Text = Utils.DisplayDecimal(0);
                //dotCount = 0; 
            };
            this.NavigationItem.RightBarButtonItem = clearVat;
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
          //  this.NavigationController.NavigationBar.TopItem.Title = "Dummy";
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region dummyNumberView
            dummyNumberView = new UIView();
            dummyNumberView.BackgroundColor = UIColor.FromRGB(248,248,248);
            dummyNumberView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(dummyNumberView);
            // dummyNumberView.BackgroundColor = UIColor.Red;

            lblDummy = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = new UIColor(red: 0 / 225f, green: 149 / 255f, blue: 218 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblDummy.Text = Utils.DisplayDecimal(0);
            //lblDummy.Text = strValue;
            lblDummy.Font = lblDummy.Font.WithSize(60);
            dummyNumberView.AddSubview(lblDummy);
            

            //lblDiscount,lblType;
            lblDiscount = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblDiscount.Text = Utils.TextBundle("discount", "Items");
            lblDiscount.Font = lblDiscount.Font.WithSize(15);
            dummyNumberView.AddSubview(lblDiscount);

            //lblType = new UILabel
            //{
            //    TextAlignment = UITextAlignment.Center,
            //    TextColor = UIColor.FromRGB(64, 64, 64),
            //    TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            //};
            //lblType.Font = lblType.Font.WithSize(15);
            //dummyNumberView.AddSubview(lblType);

            btnbath = new UIButton();
            btnbath.Layer.CornerRadius = 5;
            btnbath.SetTitle("฿" , UIControlState.Normal);
            btnbath.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            btnbath.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnbath.TranslatesAutoresizingMaskIntoConstraints = false;
            btnbath.TouchUpInside += async (sender, e) => {
                type = 'B';
                dotCount = 1;
                btnDot.Enabled = true;
                btnpersen.Layer.BorderWidth = 0.5f;
                btnpersen.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnpersen.BackgroundColor = UIColor.White;
                btnpersen.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnbath.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                btnbath.SetTitleColor(UIColor.White, UIControlState.Normal);

            };
            View.AddSubview(btnbath);

            btnpersen = new UIButton();
            btnpersen.Layer.CornerRadius = 5;
            btnpersen.SetTitle("%" , UIControlState.Normal);
            btnpersen.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            btnpersen.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnpersen.TranslatesAutoresizingMaskIntoConstraints = false;
            btnpersen.TouchUpInside += async (sender, e) => {
                type = 'P';
                dotCount = 0;
                btnDot.Enabled = false;
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
                if ((decimal)(Convert.ToDouble(amount) * 0.01) < 100)
                {
                    string[] splitper = amount.Split('%');
                    lblDummy.Text = ((Convert.ToInt32(splitper[0])) * 0.01).ToString("#,##0.00");
                    strValue = lblDummy.Text;
                }
                else
                {
                    string[] splitper = amount.Split('%');
                    lblDummy.Text = ((Convert.ToInt32(10000)) * 0.01).ToString("#,##0.00");
                    strValue = lblDummy.Text;
                }
               
                btnbath.Layer.BorderWidth = 0.5f;
                btnbath.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnbath.BackgroundColor = UIColor.White;
                btnbath.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnpersen.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                btnpersen.SetTitleColor(UIColor.White, UIControlState.Normal);
            };
            View.AddSubview(btnpersen);
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
            btnAddDummy.SetTitle(Utils.TextBundle("applydiscount", "Items"), UIControlState.Normal);
            btnAddDummy.BackgroundColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1);
            btnAddDummy.SetTitleColor(UIColor.White,UIControlState.Normal);
            btnAddDummy.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddDummy.TouchUpInside += (sender, e) => {
                

                if (tranDetailItem is null)
                {
                    if (Convert.ToDecimal(strValue) >0)
                    {
                        if (type == 'P' )
                        {
                            strValue = strValue + "%";
                        }
                        thisdiscount.FmlDiscount = strValue;
                        if (fnew)
                        {
                            POSController.tranWithDetails = BLTrans.AddDiscount(POSController.tranWithDetails, thisdiscount);
                        }
                        else
                        {
                            POSController.tranWithDetails = BLTrans.Caltran(POSController.tranWithDetails);
                        }
                    }
                    else
                    {
                        if (!fnew)
                        {
                            POSController.tranWithDetails = BLTrans.RemoveDiscount(POSController.tranWithDetails, "MD");
                        }
                    }
                    this.NavigationController.PopViewController(false);
                }
                else
                {

                    POSController.tranWithDetails = BLTrans.AddDiscountDetailItem(POSController.tranWithDetails, tranDetailItem, Convert.ToDecimal(strValue), type);
                    POSController.tranWithDetails = BLTrans.Caltran(POSController.tranWithDetails);
                    this.NavigationController.PopViewController(false);
                }

            };
            View.AddSubview(btnAddDummy);
            #endregion
            Textboxfocus(View);
            SetupAutoLayout();

            string discount = "0" ;
            
            
            if (tranDetailItem != null)
            {
                var fmldis = tranDetailItem.FmlDiscountRow;
                if (tranDetailItem.FmlDiscountRow != null)
                {
                    var checkdiscount = tranDetailItem.FmlDiscountRow.IndexOf('%');
                    if (checkdiscount == -1)
                    {
                        type = 'B';
                        dotCount = 1;
                        btnDot.Enabled = true;
                        discount = Convert.ToDecimal(fmldis).ToString();
                        btnpersen.Layer.BorderWidth = 0.5f;
                        btnpersen.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        btnpersen.BackgroundColor = UIColor.White;
                        btnpersen.SetTitleColor(UIColor.FromRGB(0, 149, 218) , UIControlState.Normal );
                    }
                    else
                    {
                        type = 'P';
                        dotCount = 0;
                        btnDot.Enabled = false;
                        discount = fmldis.Remove(checkdiscount);
                        btnbath.Layer.BorderWidth = 0.5f;
                        btnbath.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                        btnbath.BackgroundColor = UIColor.White;
                        btnbath.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    }
                }
                else
                {
                    type = 'B';
                    dotCount = 1;
                    btnDot.Enabled = true;
                    btnpersen.Layer.BorderWidth = 0.5f;
                    btnpersen.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    btnpersen.BackgroundColor = UIColor.White;
                    btnpersen.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                }

            }
            else
            {
               
                thisdiscount = tranWithDetailsLocal.tranTradDiscounts.Where(x=>x.DiscountType == "MD").FirstOrDefault();
                string fmldis = "0";
                if (thisdiscount == null )
                {
                    thisdiscount = new TranTradDiscount()
                    {
                        MerchantID = tranWithDetailsLocal.tran.MerchantID,
                        SysBranchID = tranWithDetailsLocal.tran.SysBranchID,
                        TranNo = tranWithDetailsLocal.tran.TranNo,
                        PriorityNo = 0,
                        FOnTop = 0,
                        FmlDiscount = "0" ,
                        DiscountType = "MD",
                        //FmlDiscount = getcustomerType.PercentDiscount.ToString() + "%"

                    };
                    fnew = true;
                    fmldis = "0";
                   
                }
                else
                {
                    fnew = false;
                    fmldis = thisdiscount.FmlDiscount;
                }
                var checkdiscount = thisdiscount.FmlDiscount.IndexOf('%');
                if (checkdiscount == -1)
                {
                    type = 'B';
                    dotCount = 1;
                    btnDot.Enabled = true;
                    discount = Convert.ToDecimal(fmldis).ToString("#,##0.00");
                    btnpersen.Layer.BorderWidth = 0.5f;
                    btnpersen.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    btnpersen.BackgroundColor = UIColor.White;
                    btnpersen.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                }
                else
                {
                    type = 'P';
                    dotCount = 0;
                    btnDot.Enabled = false;
                    discount = fmldis.Remove(checkdiscount);
                    btnbath.Layer.BorderWidth = 0.5f;
                    btnbath.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    btnbath.BackgroundColor = UIColor.White;
                    btnbath.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                }
            }
            discount = Convert.ToDecimal(discount).ToString("#,##0.00");
            lblDummy.Text = discount;
            dotCount = 0;
            strValue = discount;
            
            //if (flag == 1)
            //{
            //    //txtDescription.Text = "Percent";
            //    //lblType.Text = "Percent";
            //    lblDummy.Text = discount;
            //    txtDummyStr = "1";
            //    dotCount = 0;

            //    btnDot.Enabled = false;
            //}
            //else
            //{
            //    //txtDescription.Text = "Amount";
            //    //lblType.Text = "Baht";
            //    lblDummy.Text = discount;
            //    dotCount = 1;
            //    btnDot.Enabled = true;
            //}
        }
        void SetupAutoLayout()
        {
            dummyNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            dummyNumberView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            dummyNumberView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            dummyNumberView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*32 )/100).Active = true;

            lblDiscount.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblDiscount.BottomAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.TopAnchor,-2).Active = true;
            lblDiscount.WidthAnchor.ConstraintEqualTo(200).Active = true;

            lblDummy.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblDummy.CenterYAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDummy.HeightAnchor.ConstraintEqualTo(84).Active = true;

            //lblType.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            //lblType.TopAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            //lblType.WidthAnchor.ConstraintEqualTo(200).Active = true;

            btnbath.RightAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor,-10).Active = true;
            btnbath.TopAnchor.ConstraintEqualTo(lblDummy.BottomAnchor , 5).Active = true;
            btnbath.HeightAnchor.ConstraintEqualTo(40).Active = true;
            btnbath.WidthAnchor.ConstraintEqualTo(100).Active = true;

            btnpersen.LeftAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, 10).Active = true;
            btnpersen.TopAnchor.ConstraintEqualTo(lblDummy.BottomAnchor, 5).Active = true;
            btnpersen.HeightAnchor.ConstraintEqualTo(40).Active = true;
            btnpersen.WidthAnchor.ConstraintEqualTo(100).Active = true;

            numpadView.TopAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            numpadView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            numpadView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            numpadView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

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
        public async void SetValue(string btn)
        {
            try
            {
                if (strValue == "0" && btn != ".")
                {
                    strValue = string.Empty;
                    lblDummy.Text = Utils.DisplayDecimal(0);
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
                decimal max = 0;
                if (tranDetailItem != null)
                {
                    max = tranDetailItem.SubAmount;
                }
                else
                {
                    max = tranWithDetailsLocal.tran.SubTotalHaveVat+ tranWithDetailsLocal.tran.SubTotalNoneVat;
                }

                if (type == 'P')
                {
                    if ((decimal)(Convert.ToDouble(amount) * 0.01) < 100)
                    {
                        string[] splitper = amount.Split('%');
                        lblDummy.Text = ((Convert.ToInt32(splitper[0])) * 0.01).ToString("#,##0.00") ;
                        strValue = lblDummy.Text;
                    }
                    else
                    {
                        string[] splitper = amount.Split('%');
                        lblDummy.Text = ((Convert.ToInt32(10000)) * 0.01).ToString("#,##0.00") ;
                        strValue = lblDummy.Text;
                    }
                   
                }
                else
                {
                    if ((decimal)(Convert.ToDouble(amount) * 0.01) < max)
                    {
                        lblDummy.Text = ((Convert.ToInt32(amount)) * 0.01).ToString("#,##0.00");
                        strValue = lblDummy.Text;
                    }
                    else
                    {
                        lblDummy.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble(max*100) * 0.01));
                        strValue = lblDummy.Text;
                    }
                    
                }

                //SetBtnClear();
                //SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
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
            btndelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            
            btndelete.BackgroundColor = UIColor.White;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.TouchUpInside += (sender, e) => {
                //0 press
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
                    strValue = lblDummy.Text;
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
                //SetBtnClear();
                //SetBtnSave();
            };
            numpadView.AddSubview(btndelete);

            btnDot = new UIButton();
            btnDot.BackgroundColor = UIColor.White;
            btnDot.SetTitle("", UIControlState.Normal);
            btnDot.TitleLabel.Font = btnDot.TitleLabel.Font.WithSize(30);
            btnDot.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnDot.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDot.TouchUpInside += (sender, e) => {
                //. press
               
                
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
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        [Export("Discount:")]
        public void Remark(UIGestureRecognizer sender)
        {
            //select discount type
            if(flag == 0)
            {
                txtDescription.Text = "Percent";
                lblType.Text = "Percent";
                lblDummy.Text = "1";
                txtDummyStr = "1";
                flag = 1;
                btnDot.Enabled = false;
                dotCount = 0;
            }
            else
            {
                txtDescription.Text = "Amount";
                lblType.Text = "Baht";
                lblDummy.Text = "5.00";
                txtDummyStr = "5.00";
                flag = 0;
                btnDot.Enabled = true;
                dotCount = 1;
            }
        }
    }
}