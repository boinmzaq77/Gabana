using CoreGraphics;
using Foundation;
using Gabana.iOS;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.POS.Cart
{
    public partial class ChangeController : UIViewController
    {
        public static decimal Change, Cash;
        UIView TopAmountView, ChangeView,buttonView , PaymentView;
        ReceiptController RecieptPage = null;
        UILabel lblTxtAmount, lblAmount, lblTxtpayment;
        UILabel lblTxtChange, lblChange, lblBaht;
        UIButton btnViewRecept;
        TransManage transManage = new TransManage();
        TranWithDetailsLocal tranWithDetails;
        UIBarButtonItem backButton;
        UICollectionView PaymentCollection;
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        ChangeDataSource changeDataSource;
        UIScrollView _scrollView;
        UIView _contentView;
        private UIButton btnBacktopos;

        public ChangeController()
        {
            this.tranWithDetails = POSController.tranWithDetails;
        }
        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);
            if (parent == null)
            {
                
                Utils.SetTitle(this.NavigationController, "POS");
                //var childVCs = ChildViewControllers;
                //foreach (var childVC in childVCs)
                //{
                //    childVC.RemoveFromParentViewController();
                //}
                //this.NavigationController.PopToViewController(DataCaching.posPage, false);
                //if (IsMovingFromParentViewController)
                //{

                //}

                Console.WriteLine("Method override: Going back Shell");
            }
        }

        private void BackBarButtonItem_Clicked(object sender, EventArgs e)
        {
            Utils.ShowMessage("ssd");
        }

        //public override void ViewDidDisappear(bool animated)
        //{
        //    Utils.ShowMessage("sdasd");
        //    base.ViewDidDisappear(animated);
        //    if (IsMovingFromParentViewController)
        //    {
        //        this.NavigationController.PopToViewController(DataCaching.posPage, false);
        //    }

        //}
        private void ViewControllerPopped(object sender, EventArgs e)
        {
           
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);


            Utils.SetTitle(this.NavigationController, Utils.TextBundle("change", "Items"));
            //this.NavigationController.NavigationItem.BackBarButtonItem.Clicked += BackBarButtonItem_Clicked;
            //((MainNavigationController)NavigationController).PoppedViewController += ViewControllerPopped;
            this.NavigationController.SetNavigationBarHidden(false, false);
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
                    // open select customer page
                    //if (selectCustomerPage == null)
                    //{
                //    selectCustomerPage = new POSCustomerController();
                //}
                //    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
            else
            {
                UIBarButtonItem selectCustomer = new UIBarButtonItem();
                selectCustomer.Image = UIImage.FromBundle("Cust");
                selectCustomer.Clicked += (sender, e) => {
                    // open select customer page
                    //if (selectCustomerPage == null)
                    //{
                    //selectCustomerPage = new POSCustomerController();
                    ////}
                    //this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
        }
        public override async void ViewDidLoad()
        {
            try
            {
                this.NavigationController.SetNavigationBarHidden(false, false);
                
                //this.NavigationController.NavigationBar.TopItem.Title = "Change";
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                #region TopAmountView
                TopAmountView = new UIView();
                TopAmountView.BackgroundColor = UIColor.White;
                TopAmountView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(TopAmountView);

                lblTxtAmount = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    TextColor = UIColor.FromRGB(162,162,162),
                    TranslatesAutoresizingMaskIntoConstraints = false 
                };
                lblTxtAmount.Font = lblTxtAmount.Font.WithSize(15);
                lblTxtAmount.Text = Utils.TextBundle("totalamount", "Items");
                TopAmountView.AddSubview(lblTxtAmount);

                lblAmount = new UILabel
                {
                    TextAlignment = UITextAlignment.Right,
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false 
                };
                lblAmount.Font = lblAmount.Font.WithSize(20);
                TopAmountView.AddSubview(lblAmount);
                #endregion

                _scrollView = new UIScrollView();
                _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
                _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                _contentView = new UIView();
                _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
                _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);


                #region paymentView
                PaymentView = new UIView();
                PaymentView.BackgroundColor = UIColor.White;
                PaymentView.TranslatesAutoresizingMaskIntoConstraints = false;
                

                lblTxtpayment = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    TextColor = UIColor.FromRGB(247, 86, 0),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblTxtpayment.Font = lblTxtAmount.Font.WithSize(15);
                lblTxtpayment.Text = Utils.TextBundle("paymentmethods", "Items");
                PaymentView.AddSubview(lblTxtpayment);


                UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 50);
                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemflowLayoutList.MinimumInteritemSpacing = 0;
                itemflowLayoutList.MinimumLineSpacing = 0;

                PaymentCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
                PaymentCollection.BackgroundColor = UIColor.White;
                PaymentCollection.ShowsVerticalScrollIndicator = false;
                PaymentCollection.TranslatesAutoresizingMaskIntoConstraints = false;
                PaymentCollection.RegisterClassForCell(cellType: typeof(ChangeCollectionViewCell), reuseIdentifier: "ChangeCollectionViewCell");
                PaymentCollection.ScrollEnabled = false;
                changeDataSource = new ChangeDataSource(tranWithDetails.tranPayments);
                PaymentCollection.DataSource = changeDataSource;
                PaymentView.AddSubview(PaymentCollection);
                #endregion

                #region ChangeView
                ChangeView = new UIView();
                ChangeView.BackgroundColor = UIColor.Clear;
                ChangeView.TranslatesAutoresizingMaskIntoConstraints = false;
                


                lblTxtChange = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblTxtChange.Font = lblTxtAmount.Font.WithSize(15);
                lblTxtChange.Text = Utils.TextBundle("change", "Items");
                ChangeView.AddSubview(lblTxtChange);

                lblChange = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblChange.Font = lblChange.Font.WithSize(60);
                lblChange.Text = "XX.XX";
                ChangeView.AddSubview(lblChange);

                lblBaht = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblBaht.Font = lblBaht.Font.WithSize(15);
                lblBaht.Text = Utils.TextCURRENCYSYMBOLS(CURRENCYSYMBOLS);
                ChangeView.AddSubview(lblBaht);

                #endregion
                _contentView.AddSubview(PaymentView);
                _contentView.AddSubview(ChangeView);
                _scrollView.AddSubview(_contentView);
                View.AddSubview(_scrollView);
                #region buttonView
                buttonView = new UIView();
                buttonView.BackgroundColor = UIColor.White;
                buttonView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(buttonView);

                btnViewRecept = new UIButton();
                btnViewRecept.Layer.CornerRadius = 5;
                btnViewRecept.ClipsToBounds = true;
                btnViewRecept.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnViewRecept.Layer.BorderWidth = 0.5f;
                btnViewRecept.SetTitle(Utils.TextBundle("viewreceipt", "Items"), UIControlState.Normal);
                btnViewRecept.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                btnViewRecept.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnViewRecept.TranslatesAutoresizingMaskIntoConstraints = false;
            
                buttonView.AddSubview(btnViewRecept);

                btnBacktopos = new UIButton();
                btnBacktopos.Layer.CornerRadius = 5;
                btnBacktopos.ClipsToBounds = true;
                btnBacktopos.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnBacktopos.Layer.BorderWidth = 0.5f;
                btnBacktopos.SetTitle(Utils.TextBundle("backtopos", "Items"), UIControlState.Normal);
                btnBacktopos.BackgroundColor = UIColor.White;
                btnBacktopos.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnBacktopos.TranslatesAutoresizingMaskIntoConstraints = false;
                btnBacktopos.TouchUpInside += async (sender, e) => {
                    MainController.POS = true;
                    this.NavigationController.PopToRootViewController(false);
                    //this.NavigationController.PopToViewController(DataCaching.posPage, false);
                };
                buttonView.AddSubview(btnBacktopos);
                #endregion

                lblAmount.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandPayment);
            lblChange.Text = Utils.DisplayDecimal(Change);
            setUpAutoLayout();

            decimal payAmount = 0;
            var list = tranWithDetails.tranPayments.ToList();
            foreach (var item in list)
            {
                payAmount += item.PaymentAmount;
            }
                var change = payAmount - tranWithDetails.tran.GrandPayment;
                var index = list.FindLastIndex(x => x.PaymentType == "CH");
                if (index == list.Count - 1)
                {
                    change = payAmount - tranWithDetails.tran.GrandPayment;
                }
                else
                {
                    change = payAmount - tranWithDetails.tran.GrandTotal;
                }

                decimal payByCash = list.Where(x => x.PaymentType == "CH").Sum(x => x.PaymentAmount);
                decimal payByGiftvoucher = list.Where(x => x.PaymentType == "GV").Sum(x => x.PaymentAmount);

                if (payByGiftvoucher > 0 && payAmount < payByGiftvoucher)
                {
                    change = 0;
                }

                //if (payByCash > 0)
                //{
                //    textreceive.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandPayment);
                //}

                if (payByCash < change)
                {
                    change = payByCash;
                }


                
                if (change < 0 && tranWithDetails.tran.GrandTotal > 0)
                {
                        // จ่ายไม่ครบ 
                        //txtdescrpit_amount.Text = GetString(Resource.String.balance_activity_balance);
                        //textChange.Text = (change * -1).ToString("#,##0.00");
                        //tranWithDetails.tran.Change = (decimal)change * -1;
                        //btnViewREceipt.Text = GetString(Resource.String.balance_activity_addpayment);
                        //DataCashing.ChangePayment = true;
                        //btnViewREceipt.Click += AddPayment;
                        lblTxtChange.Text = Utils.TextBundle("balance", "Items");
                        lblChange.Text = (Change*-1).ToString("#,##0.00");
                        btnViewRecept.SetTitle(Utils.TextBundle("addpayment", "Items"), UIControlState.Normal);
                        btnViewRecept.TouchUpInside += async (sender, e) => {
                            //print reciept
                            this.NavigationController.PopToViewController(DataCaching.paymentpage, false);
                        };
                        this.NavigationItem.HidesBackButton = true;
                    UIImageView uIImageView = new UIImageView(new CGRect(0, 0, 50, 50));
                    uIImageView.Image = UIImage.FromBundle("BackB");
                    uIImageView.TintColor = UIColor.Black;
                    if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))  uIImageView.Image.ApplyTintColor(UIColor.Black);

                    UIButton btn = new UIButton();
                    //btn.SetImage(UIImage.FromBundle("Cust"), default);
                    btn.ImageView.BackgroundColor = UIColor.Black;
                    btn.Frame = new CGRect(0, 0, 200, 50);
                    //btn.Layer.CornerRadius = 5f;
                    //btn.Layer.BorderWidth = 0.5f;
                    //btn.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    //btn.BackgroundColor = UIColor.Red;
                    UILabel lab = new UILabel();
                    lab.TextColor = UIColor.Black;
                    lab.Text = Utils.TextBundle("paymentbalance", "Items");
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
                        // open select customer page
                        //if (selectCustomerPage == null)
                        //{
                        //    selectCustomerPage = new POSCustomerController();
                        //}
                           this.NavigationController.PopToViewController(DataCaching.paymentpage, false);
                    };
                    //this.NavigationItem.RightBarButtonItem = selectCustomer;

                    
                        this.NavigationItem.LeftBarButtonItem = selectCustomer;
                    //UIBarButtonItem* newBackButton = [[UIBarButtonItem alloc] initWithTitle: @"Back" style: UIBarButtonItemStyleBordered target:self action:@selector(back:)];
                    //self.navigationItem.leftBarButtonItem = newBackButton;
                }
                else
                {
                    //จ่ายครบ
                    GabanaLoading.SharedInstance.Show(this);
                    //var count = this.NavigationController.ViewControllers.Length;
                    //var navigation = this.NavigationController.ViewControllers;

                    //for (int i = 2; i < count; i++)
                    //{
                    //    if (i != count - 1)
                    //    {

                    //        //this.NavigationController.NavigationBar.Items[2] = null;
                    //        navigation[2].View.RemoveFromSuperview();
                    //        navigation[2].RemoveFromParentViewController();

                    //    }

                    //}

                    var view = new UIView();
                    var button = new UIButton(UIButtonType.Custom);
                    button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                    button.SetTitle("  Back", UIControlState.Normal);
                    button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    button.TouchUpInside += Button_TouchUpInside;
                    button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                    button.SizeToFit();
                    view.AddSubview(button);
                    view.Frame = button.Bounds;
                    NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);

                    //var count = this.NavigationController.ViewControllers.Length;
                    //for (int i = 1; i < count; i++)
                    //{
                    //    if (i != count - 1)
                    //    {
                    //        this.NavigationController.ViewControllers[1].View.RemoveFromSuperview();
                    //        this.NavigationController.ViewControllers[1].RemoveFromParentViewController();
                    //    }
                    //}
                    var usernamelogin = Preferences.Get("User", "");
                    tranWithDetails.tran.SellerName = usernamelogin;
                    tranWithDetails.tran.LastUserModified = usernamelogin;
                    ItemsController.Ismodify = true;
                    MainController.POS = true;
                    //Insert to LocalDB
                    tranWithDetails.tran.Change = (decimal)change;
                    lblChange.Text = Utils.DisplayDecimal(tranWithDetails.tran.Change);
                    //CustomerName
                    if (string.IsNullOrEmpty(tranWithDetails.tran.CustomerName))
                    {
                        tranWithDetails.tran.CustomerName = Utils.TextBundle("Cusnomal", "Items");
                    }
                    PaymentTranOrder();
                    var result = await InsertToTrans();
                    //if (!result)
                    //{
                    //    Utils.ShowMessage("บันทึกบิลไม่สำเร็จ");
                    //}
                    btnViewRecept.TouchUpInside += async (sender, e) => {
                        //print reciept
                        if (RecieptPage == null)
                        {
                            RecieptPage = new ReceiptController(tranWithDetails);
                        }
                        Utils.SetTitle2(this.NavigationController, "POS");
                        Utils.SetTitle(this.NavigationController, "Reciept");
                        this.NavigationController.PushViewController(RecieptPage, false);
                    };
                    Utils.SetConstant(btnBacktopos.Constraints, NSLayoutAttribute.Height, 45);
                    Utils.SetConstant(buttonView.Constraints, NSLayoutAttribute.Height, 120);
                    
                    btnBacktopos.BottomAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
                    //backButton = new UIBarButtonItem();
                    //backButton.TintColor = UIColor.FromRGB(64, 64, 64);
                    //backButton.Clicked += LeftBarButtonItem_Clicked; 
                    //this.NavigationController.NavigationBar.TopItem.BackBarButtonItem = backButton;
                    GabanaLoading.SharedInstance.Hide();

                }
            }
            catch (Exception ex )
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            this.NavigationController.PopToViewController(DataCaching.posPage, false);
        }

        async void PaymentTranOrder()
        {
            //เลือก order มาแล้วต้องการจ่ายเงิน
            //ทำการสร้าง Order ใหม่ขึ้นมาแทน
            try
            {
                var insertTran = false;
                var updatetTran = false;
                Model.TranWithDetailsLocal TranWithDetailsnewTran = new Model.TranWithDetailsLocal();

                if (tranWithDetails.tran.FWaitSending == 0 & tranWithDetails.tran.Status == 100)
                {
                    //Old Tran
                    tranWithDetails.tran.Status = 120;
                    tranWithDetails.tran.FWaitSending = 0;
                    tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                    updatetTran = await transManage.UpdateTran(tranWithDetails.tran);

                    //New Tran
                    TranWithDetailsnewTran = await Utils.initialData();
                    string newTranNo = TranWithDetailsnewTran.tran.TranNo;
                    if (TranWithDetailsnewTran != null)
                    {
                        TranWithDetailsnewTran.tran = tranWithDetails.tran;
                        TranWithDetailsnewTran.tran.TranNo = newTranNo;
                        TranWithDetailsnewTran.tran.TranType = 'B';
                        TranWithDetailsnewTran.tran.FWaitSending = 1;
                        TranWithDetailsnewTran.tran.Status = 10;
                        TranWithDetailsnewTran.tran.LocalDataStatus = 'I';
                        TranWithDetailsnewTran.tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings;
                        TranWithDetailsnewTran.tranPayments = tranWithDetails.tranPayments;
                        TranWithDetailsnewTran.tranTradDiscounts = tranWithDetails.tranTradDiscounts;

                        //แก้ไข tranNo ให้เป็นตัวใหม่
                        foreach (var item in TranWithDetailsnewTran.tranDetailItemWithToppings)
                        {
                            //TranDetail
                            item.tranDetailItem.TranNo = newTranNo;

                            //TranDetailTopping
                            foreach (var itemTopping in item.tranDetailItemToppings)
                            {
                                itemTopping.TranNo = newTranNo;
                            }
                        }

                        //TranDiscount
                        foreach (var item in TranWithDetailsnewTran.tranTradDiscounts)
                        {
                            //TranDetail
                            item.TranNo = newTranNo;
                        }

                        //TranPayment
                        foreach (var item in TranWithDetailsnewTran.tranPayments)
                        {
                            //TranDetail
                            item.TranNo = newTranNo;
                        }
                    }
                }

                //Set TranWithDetailsnewTran to  tranWithDetails เพื่อไปใช้งานต่อ
                if (updatetTran)
                {
                    tranWithDetails = TranWithDetailsnewTran;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                //_ = TinyInsights.TrackErrorAsync(ex);
                //_ = TinyInsights.TrackPageViewAsync("PaymentTranOrder at Payment");
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }
        private void LeftBarButtonItem_Clicked(object sender, EventArgs e)
        {
            this.NavigationController.PopToViewController(DataCaching.posPage,false);
        }

        async Task<bool> InsertToTrans()
        {
            try
            {
                
                
                
                //this.NavigationController.NavigationBar.TopItem.BackBarButtonItem = backButton;
                //Insert Trans
                if (tranWithDetails == null)
                {
                    return false;
                }

                MerchantManage merchantManage = new MerchantManage();
                var getMerchant = await merchantManage.GetMerchant(DataCashingAll.MerchantId);


                var result = await transManage.InsertTran(tranWithDetails);
                if (!result)
                {
                    POSController.tranWithDetails = null;
                    POSController.SelectedCustomer = null;
                    POSController.clearData();
                    return false;
                }

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendTrans((int)tranWithDetails.tran.MerchantID, (int)tranWithDetails.tran.SysBranchID, tranWithDetails.tran.TranNo);
                }
                else
                {
                    tranWithDetails.tran.Status = 10;
                    tranWithDetails.tran.FWaitSending = 2;
                    var updatetTran = await transManage.UpdateTran(tranWithDetails.tran);
                }
                POSController.tranWithDetails = null;
                POSController.SelectedCustomer = null;
                POSController.clearData();
                return true;
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                POSController.tranWithDetails = null;
                POSController.SelectedCustomer = null;
                POSController.clearData();
                _ = TinyInsights.TrackErrorAsync(ex);
                return false;
            }
        }
        public void Setitem(double change, double cash)
        {
            Change = Convert.ToDecimal(change);
            Cash = Convert.ToDecimal(cash);



        }
        void setUpAutoLayout()
        {
            #region TopAmountView
            TopAmountView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor,0).Active=true;
            TopAmountView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TopAmountView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            TopAmountView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height*97/1000).Active = true;

            lblTxtAmount.CenterYAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblTxtAmount.LeftAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.LeftAnchor, 25).Active = true;
            lblTxtAmount.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lblAmount.CenterYAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblAmount.RightAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            lblAmount.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            _scrollView.TopAnchor.ConstraintEqualTo(TopAmountView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(buttonView.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region TopAmountView
            PaymentView.TopAnchor.ConstraintEqualTo(PaymentView.Superview.TopAnchor).Active = true;
            PaymentView.LeftAnchor.ConstraintEqualTo(PaymentView.Superview.LeftAnchor, 0).Active = true;
            PaymentView.RightAnchor.ConstraintEqualTo(PaymentView.Superview.RightAnchor, 0).Active = true;
            PaymentView.HeightAnchor.ConstraintEqualTo((tranWithDetails.tranPayments.Count * 50)+35).Active = true;
            //PaymentView.HeightAnchor.ConstraintEqualTo().Active = true;

            lblTxtpayment.TopAnchor.ConstraintEqualTo(PaymentView.SafeAreaLayoutGuide.TopAnchor , 5).Active = true;
            lblTxtpayment.LeftAnchor.ConstraintEqualTo(PaymentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblTxtpayment.RightAnchor.ConstraintEqualTo(PaymentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblTxtpayment.HeightAnchor.ConstraintEqualTo(20).Active = true;

            PaymentCollection.TopAnchor.ConstraintEqualTo(lblTxtpayment.SafeAreaLayoutGuide.BottomAnchor , 5  ).Active = true;
            PaymentCollection.LeftAnchor.ConstraintEqualTo(PaymentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            PaymentCollection.RightAnchor.ConstraintEqualTo(PaymentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            PaymentCollection.BottomAnchor.ConstraintEqualTo(PaymentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            #endregion

            #region ChangeView
            ChangeView.TopAnchor.ConstraintEqualTo(PaymentView.BottomAnchor, 5).Active = true;
            ChangeView.LeftAnchor.ConstraintEqualTo(ChangeView.Superview.LeftAnchor, 0).Active = true;
            ChangeView.RightAnchor.ConstraintEqualTo(ChangeView.Superview.RightAnchor, 0).Active = true;
            ChangeView.BottomAnchor.ConstraintEqualTo(ChangeView.Superview.BottomAnchor, 0).Active = true;
            ChangeView.HeightAnchor.ConstraintEqualTo(200).Active = true;

            // lblTxtChange, lblChange, lblBaht;
            lblChange.CenterYAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblChange.CenterXAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblTxtChange.BottomAnchor.ConstraintEqualTo(lblChange.SafeAreaLayoutGuide.TopAnchor , -20).Active = true;
            lblTxtChange.CenterXAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblBaht.TopAnchor.ConstraintEqualTo(lblChange.SafeAreaLayoutGuide.BottomAnchor,20).Active = true;
            lblBaht.CenterXAnchor.ConstraintEqualTo(ChangeView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            #region buttonView
            buttonView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            buttonView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            buttonView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            buttonView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnBacktopos.LeftAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnBacktopos.RightAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnBacktopos.HeightAnchor.ConstraintEqualTo(0).Active = true;
            btnBacktopos.BottomAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            
            btnViewRecept.LeftAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnViewRecept.RightAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnViewRecept.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnViewRecept.BottomAnchor.ConstraintEqualTo(btnBacktopos.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;

           
            
            #endregion
        }
    }
}