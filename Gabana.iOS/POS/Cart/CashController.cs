using CoreGraphics;
using Foundation;
using Gabana.iOS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.POS.Cart
{
    public partial class CashController : UIViewController
    {
        UIView dummyNumberView,bottomView,numpadView;
        string txtDummyStr="0" , strValue;
        public static double Change, Cash;//เงินทอน
        private UITextField txtDummy;
        UILabel lblDummy, lblBath;
        UIButton btnAddDummy;
        int dotCount = 0, count;
        private bool frist;
        double amount;
        decimal pay;
        List<CashTemplate> cashlist = new List<CashTemplate>(); 
        public static Customer SelectedCustomer = null;
        POSCustomerController selectCustomerPage = null;
        TranWithDetailsLocal tranWithDetails;
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete, btnDot;
        private UICollectionView CashCollectionView;
        private bool havedot;

        public CashController()
        {
            this.tranWithDetails = POSController.tranWithDetails;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            POSController.tranWithDetails = this.tranWithDetails;

        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("cash", "Items"));
            this.NavigationController.SetNavigationBarHidden(false, false);
            //decimal paymentAmount = 0;
            //paymentAmount = tranWithDetails.tranPayments.Sum(x => x.PaymentAmount);

            ////amount คือ ยอดที่ต้องจ่าย  
            //var amount = Convert.ToDouble(tranWithDetails.tran.GrandTotal - paymentAmount);

            if (tranWithDetails != null)
            {
                tranWithDetails = await BLTrans.CalDecimal(tranWithDetails);
            }

            decimal paymentAmount2 = 0;
            foreach (var item in tranWithDetails.tranPayments)
            {
                paymentAmount2 += item.PaymentAmount;
            }
            //amount คือ ยอดที่ต้องจ่าย      
            frist = true; 
            amount = Convert.ToDouble(tranWithDetails.tran.GrandPayment - paymentAmount2);

            lblBath.Text = Utils.TextBundle("paymentamount", "Items")+" : " + Utils.DisplayDecimal((decimal)amount) +" "+  Utils.TextCURRENCYSYMBOLS(CURRENCYSYMBOLS);
            pay = tranWithDetails.tran.GrandTotal - paymentAmount2;
            lblDummy.Text = Utils.DisplayDecimal((decimal)amount);
            lblDummy.TextColor = UIColor.FromRGB(134, 206, 239);
            btnAddDummy.SetTitle(Utils.TextBundle("charge", "Items") + CURRENCYSYMBOLS + Utils.DisplayDecimal((decimal)amount), UIControlState.Normal);
            if (POSController.SelectedCustomer != null)
            {
                UIImageView uIImageView = new UIImageView(new CGRect(0, 0, 50, 50));
                uIImageView.Image = UIImage.FromBundle("CustB");
                UIButton btn = new UIButton();
                //btn.SetImage(UIImage.FromBundle("Cust"), default);
                btn.ImageView.BackgroundColor = UIColor.Black;
                btn.Frame = new CGRect(0, 0, 200, 50);
                btn.Layer.CornerRadius = 5f;
                btn.Layer.BorderWidth = 0.5f;
                btn.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                //btn.BackgroundColor = UIColor.Red;
                UILabel lab = new UILabel();
                lab.TextColor = UIColor.FromRGB(0, 149, 218);
                lab.Text = POSController.SelectedCustomer.CustomerName;
                lab.TextAlignment = UITextAlignment.Right;
                lab.TranslatesAutoresizingMaskIntoConstraints = false;
                uIImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                btn.AddSubview(uIImageView);
                btn.AddSubview(lab);

                lab.RightAnchor.ConstraintEqualTo(btn.RightAnchor, -5).Active = true;
                lab.HeightAnchor.ConstraintEqualTo(50).Active = true;
                lab.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;
                uIImageView.RightAnchor.ConstraintEqualTo(lab.LeftAnchor).Active = true;
                uIImageView.LeftAnchor.ConstraintEqualTo(btn.LeftAnchor).Active = true;
                uIImageView.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;

                UIBarButtonItem selectCustomer = new UIBarButtonItem(btn);
                btn.TouchUpInside += (sender, e) => {
                    var selectCustomerPage = new POSCustomerController();
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
            else
            {
                UIBarButtonItem selectCustomer = new UIBarButtonItem();
                selectCustomer.Image = UIImage.FromBundle("Cust");
                selectCustomer.Clicked += (sender, e) => {
                    selectCustomerPage = new POSCustomerController();
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
        }
        public override async void ViewDidLoad()
        {
            try
            {
                havedot = false;
                Utils.SetTitle(this.NavigationController, "Cash");
                strValue = "0";
                   UIBarButtonItem selectCustomer = new UIBarButtonItem();
                selectCustomer.Image = UIImage.FromBundle("Cust");
                selectCustomer.Clicked += (sender, e) => {
                    // open select customer page
                    //if (selectCustomerPage == null)
                    //{
                        selectCustomerPage = new POSCustomerController();
                    //}
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;

                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                
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
                    TextColor = UIColor.FromRGB(134, 206, 239),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    
                };
                //lblDummy
                //lblDummy.numberOfLines = 1;
                lblDummy.MinimumFontSize = 40;
                lblDummy.AdjustsFontSizeToFitWidth = true;
                //lblDummy.Text = txtDummyStr;
                lblDummy.Font = lblDummy.Font.WithSize(45);
                dummyNumberView.AddSubview(lblDummy);


                

                lblBath = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblBath.Font = lblDummy.Font.WithSize(15);
                //lblBath.Text = "Payment amount : " + tranWithDetails.tran.GrandTotal.ToString("#,##0.00") + " Bath";
                dummyNumberView.AddSubview(lblBath);
                #endregion

                TopAlignedCollectionViewFlowLayout cashLayoutList = new TopAlignedCollectionViewFlowLayout();
                //NoteLayoutList.ItemSize = new CoreGraphics.CGSize(width: (int)View.Frame.Width/4, height: 40);
                cashLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                cashLayoutList.SectionInset = UIEdgeInsets.Zero;
                
                cashLayoutList.MinimumLineSpacing = 3f;
                cashLayoutList.MinimumInteritemSpacing = 3f;
                cashLayoutList.ItemSize = new CoreGraphics.CGSize((View.Frame.Width-30)/4, 35);


                CashTemplateManage cashTemplateManage = new CashTemplateManage();
                cashlist = await cashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                cashlist = cashlist.OrderBy(x => x.Amount).ToList();
                CashCollectionView = new UICollectionView(frame: View.Frame, layout: cashLayoutList);
                CashCollectionView.ScrollEnabled = true;
                //CashCollectionView.SemanticContentAttribute = UISemanticContentAttribute.;
                CashCollectionView.BackgroundColor = UIColor.Clear;
                CashCollectionView.ShowsVerticalScrollIndicator = false;
                CashCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;

                CashCollectionView.RegisterClassForCell(cellType: typeof(CashguideCashCollectionViewCell), reuseIdentifier: "CashguideCashCollectionViewCell");
                CashguideCashDataSource cashDataList = new CashguideCashDataSource(cashlist);
                CashCollectionView.DataSource = cashDataList;
                CashguideCashCollectionDelegate cashCollectionDelegate = new CashguideCashCollectionDelegate();
                cashCollectionDelegate.OnItemSelected += (indexPath) =>
                {
                    decimal value = 0;
                    if (frist)
                    {

                        strValue = cashlist[indexPath.Row].Amount.ToString();
                    }
                    else
                    {

                        strValue = (decimal.Parse(strValue) + (decimal)(Convert.ToDouble(cashlist[indexPath.Row].Amount))).ToString();

                    }
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

                    SetBtnSave();
                };
                CashCollectionView.Delegate = cashCollectionDelegate;
                dummyNumberView.AddSubview(CashCollectionView);

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
                //btnAddDummy.SetTitle("Charge ฿" + txtDummyStr, UIControlState.Normal);
                btnAddDummy.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                btnAddDummy.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAddDummy.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAddDummy.TouchUpInside += async (sender, e) => {
                    // search function
                    if (string.IsNullOrEmpty(strValue))
                    {
                        Cash = Convert.ToDouble(0);
                    }
                    else
                    {
                        Cash = Convert.ToDouble(lblDummy.Text);
                    }

                    
                    var PaymentNo = tranWithDetails.tranPayments.Count;
                    PaymentNo++;
                    var tranPayment = initialData();
                    Change = CalculateAmount(Cash, amount); // Cash เงินที่จ่าย, amount(ยอดจ่าย)
                    tranPayment.PaymentNo = PaymentNo;
                    tranPayment.PaymentAmount = (decimal)Cash; //เงินที่จ่าย
                    tranWithDetails.tranPayments.Add(tranPayment);

                    //this.NavigationController.PopViewController(false);
                    ChangeController ChangePage = new ChangeController();
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("charge", "Items"));
                    ChangePage.Setitem(Change, Cash);
                    //this.NavigationController.ViewControllers)
                    this.NavigationController.PushViewController(ChangePage, false);


                    //if (Convert.ToDouble(lblDummy.Text) > 0 && lblDummy.Text.Length > 0)
                    //{
                    //    Change = CalculateAmount(Cash, (double)tranWithDetails.tran.GrandTotal);
                    //    try
                    //    {
                    //        TransManage trans = new TransManage();
                    //        await trans.InsertTran(tranWithDetails);
                    //        ChangeController ChangePage = new ChangeController();
                    //        ChangePage.Setitem(Change, Cash);
                    //        this.NavigationController.PushViewController(ChangePage, false);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Utils.ShowMessage(ex.Message);
                    //    }

                    //}

                };
                View.AddSubview(btnAddDummy);
                #endregion
                Textboxfocus(View);
                SetupAutoLayout();

                decimal paymentAmount = 0;
                foreach (var item in tranWithDetails.tranPayments)
                {
                    paymentAmount += item.PaymentAmount;
                }
                //amount คือ ยอดที่ต้องจ่าย      
                
                    
                amount = Convert.ToDouble(tranWithDetails.tran.GrandTotal - paymentAmount);
                //lblAmount.Hint = Utils.DisplayDecimal(Convert.ToDecimal(amount));
                //lblAmount.Text = "Payment amount : " + Utils.DisplayDecimal(Convert.ToDecimal(amount)) + " " ;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        private double CalculateAmount(double Cash, double Amount)
        {
            Change = Cash - Amount;
            return Change;
        }
        private TranPayment initialData()
        {
            var tranPayment = new TranPayment()
            {
                MerchantID = DataCashingAll.MerchantId,
                SysBranchID = DataCashingAll.SysBranchId,
                TranNo = tranWithDetails.tran.TranNo,
                PaymentType = "CH",
                PaymentAmount = (decimal)0, //เงินที่ต้องจ่าย
                CreditCardType = null,
                CardNo = null,
                ExprieDateYYYYMM = null,
                ApproveCode = null,
                TotalRedeemPoint = null,
                
                RequestNum = null,
                RequestDateTime = null,
                FEPaymentCancel = 0,
                ReferenceNo1 = null,
                ReferenceNo2 = null,
                ReferenceNo3 = null,
                ReferenceNo4 = null,
                Comments = null,
            };
            return tranPayment; 
        }
        void SetupAutoLayout()
        {
            dummyNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            dummyNumberView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            dummyNumberView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            dummyNumberView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*3)/10).Active = true;

            lblDummy.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblDummy.BottomAnchor.ConstraintEqualTo(lblBath.SafeAreaLayoutGuide.TopAnchor,-10).Active = true;

            lblBath.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblBath.BottomAnchor.ConstraintEqualTo(CashCollectionView.SafeAreaLayoutGuide.TopAnchor, -20).Active = true;

            //CashCollectionView.LeftAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.LeftAnchor,5).Active = true;
            //CashCollectionView.RightAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.RightAnchor,-5).Active = true;
            CashCollectionView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            nfloat cashwidth = 0;
            if (cashlist.Count>4)
            {
                cashwidth = (4 * ((View.Frame.Width - 30) / 4)) + 10;
            }
            else
            {
                cashwidth = (cashlist.Count * ((View.Frame.Width - 30) / 4))+ 10;
            }
            CashCollectionView.WidthAnchor.ConstraintEqualTo(cashwidth).Active = true;
            CashCollectionView.BottomAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor,-10).Active = true;
            //CashCollectionView.BackgroundColor = UIColor.Red;
            nfloat height = 0;
            if (cashlist.Count==0)
            {
                height = 0;
            }
            else
            {
                decimal x = (decimal)cashlist.Count / 4;
                var row = Math.Ceiling((decimal)x);
                height = (nfloat)row * 40; 
            }
            CashCollectionView.HeightAnchor.ConstraintEqualTo(height).Active = true;


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
            btndelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            //btndelete.ImageEdgeInsets = new UIEdgeInsets(20, 30, 20, 30);
            btndelete.BackgroundColor = UIColor.White;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.TouchUpInside += (sender, e) => {
                //0 press
                if (strValue.Length == 0 )
                {
                    return;
                }
                strValue = strValue.Substring(0 , strValue.Length-1);
                //lblDummy.Text = strValue;




                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                    lblDummy.Text = Utils.DisplayDecimal((decimal)amount);
                    lblDummy.TextColor = UIColor.FromRGB(134, 206, 239);
                    frist = true;
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

            btnDot.TopAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnDot.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnDot.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
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
        public async void SetValue(string btn)
        {
            try
            {
                if (strValue.IndexOf(".")!=-1 )
                {
                    var check = strValue.Split(".");
                    if (check[1].Length==2)
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
                if ((decimal)(Convert.ToDouble(amount)) <= 100000000)
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
                    else if(check[1].Length == 1)
                    {
                        lblDummy.Text = double.Parse(strValue).ToString("#,###.0");
                    }
                    else
                    {
                        lblDummy.Text = double.Parse(strValue).ToString("#,###")+".";
                    }
                }
                else
                {
                    lblDummy.Text = decimal.Parse(amount).ToString("#,###");
                }
                
                

                //SetBtnCharge();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetBtnSave()
        {
            if (frist)
            {
                frist = false;
            }
            if (string.IsNullOrEmpty(strValue) )
            {
                return;
            }
            if (decimal.Parse(strValue) > 0)
            {
                btnAddDummy.SetTitle(Utils.TextBundle("realpay", "")+" " + CURRENCYSYMBOLS + Utils.DisplayDecimal(decimal.Parse(strValue)), UIControlState.Normal);
                lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                btnAddDummy.SetTitle(Utils.TextBundle("realpay", "")+" " + CURRENCYSYMBOLS + Utils.DisplayDecimal(pay), UIControlState.Normal);
                lblDummy.TextColor = UIColor.FromRGB(134, 206, 239);
            }
            
        }
    }
}