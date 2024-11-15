using AudioToolbox;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Gabana.iOS;
using Gabana.iOS.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Mobile;

namespace Gabana.POS.Cart
{

    public class CartController : UIViewController
    {
        UICollectionView CartCollectionview;
        List<CartItem> cartItemlist;
        private ZXingScannerView m_scannerView;
        CartDataSource cartDataSource;
        UIView viewfoot, vatView, TotalView, RemarkView, DiscountView, EmptyView, DialogBGView;
        UILabel lbltotal , lbltxttotal , lblvat , lbltxtvat,lblRemark, lblRemark2, lbldiscount, lbltxtDiscount;
        UIButton btnchackout, btndetail;
        UIView line;
        public bool Is = true;
        UILabel lblEmpty;
        UIImageView EmptyImg;
        UIImageView moreImg,RemarkImg;
        private TranWithDetailsLocal tranWithDetails;
        public static char disCountType='N';
        public static bool Ismodify;
        public static Customer SelectedCustomer = null;
        POSCustomerController selectCustomerPage = null;
        public static decimal disCountAmount=0;
        UIView DialogView, OptionView, ClearCartView, AddDiscountView, AddRemarkView, CheckOutView, DiscountCusView, serviceView;
        UILabel lblOption , lbldiscountCus , lbltxtDiscountCus;
        UIButton btnCancel, btnMore, btnCheckOut, btnDeleteRemark;
        UILabel lblAddRemark, lblAddDiscount, lblClearCart, lblDialogRemark, lblservice, lbltxtservice;
        UIImageView DialogRemarkImg, DiscountImg, ClearImage, DialogmoreImg, RemarkImg2;
        public static decimal disCountPercent=0;
        PaymentController paymentPage = null;
        bool scan = false;
        public static bool alert = false;
        private List<string> lstSysItemIdStatusD;
        private UILabel lblDiscount;
        private UIButton btnDeleteDiscount;

        //   OptionController OptionPage = null;
        public CartController(bool scan)
        {
            this.tranWithDetails = POSController.tranWithDetails;
            this.scan = scan;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            POSController.tranWithDetails = this.tranWithDetails;
            m_scannerView.StopScanning();
            //m_scannerView.PauseAnalysis();
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.tranWithDetails=POSController.tranWithDetails;
        }
        public override async void  ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {

            
                Utils.SetTitle(this.NavigationController, "Cart");
                Is = true;
                this.tranWithDetails.tran.Comments =  POSController.tranWithDetails.tran.Comments;
                POSController.tranWithDetails = this.tranWithDetails;
                viewfoot.Hidden = false;
                //if (Ismodify)
                //{
                    ((CartDataSource)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItemWithToppings);
                    CartCollectionview.ReloadData();
                //}
                //if (scan)
                //{
                //    m_scannerView?.ResumeAnalysis();
                //    //m_scannerView.StartScanning();
                //}
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
                        selectCustomerPage = new POSCustomerController();
                        //}
                        this.NavigationController.PushViewController(selectCustomerPage, false);
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
                        selectCustomerPage = new POSCustomerController();
                        //}
                        this.NavigationController.PushViewController(selectCustomerPage, false);
                    };
                    this.NavigationItem.RightBarButtonItem = selectCustomer;
                }
                if (scan)
                {
                    scannerInit();
                    //m_scannerView?.ResumeAnalysis();

                }
                if (this.tranWithDetails == null)
                {
                    btnchackout.Enabled = false;
                    btndetail.Enabled = false;
                    EmptyView.Hidden = false;
                    TotalView.Hidden = true;
                    //vatView.Hidden = true;
                    //DiscountView.Hidden = true;
                    //RemarkView.Hidden = true;
                    //CartCollectionview.Hidden = true;
                }
                else
                {
                    btnchackout.Enabled = true;
                    btndetail.Enabled = true;
                    EmptyView.Hidden = true;
                    TotalView.Hidden = false;
                    RemarkView.Hidden = false;
                    CartCollectionview.Hidden = false;

                    if (string.IsNullOrEmpty( POSController.tranWithDetails.tran.Comments))
                    {
                        RemarkView.Hidden = true;
                        Utils.SetConstant(RemarkView.Constraints, NSLayoutAttribute.Height, 0);
                        lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                        //lblRemark.HeightAnchor.ConstraintEqualTo(0).Active = true;
                        btnDeleteRemark.Hidden = true;
                        lblAddRemark.Text = Utils.TextBundle("addremark", "Add Remark");
                    }
                    else
                    {
                        RemarkView.Hidden = false;
                        lblRemark.Text = POSController.tranWithDetails.tran.Comments;
                        lblRemark2.Text = POSController.tranWithDetails.tran.Comments;
                        Utils.SetConstant(RemarkView.Constraints, NSLayoutAttribute.Height, 45);
                        lblAddRemark.Text = Utils.TextBundle("remark", "Remark");
                        lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor, -9).Active = true;
                        //lblRemark.HeightAnchor.ConstraintEqualTo(18).Active = true;
                        btnDeleteRemark.Hidden = false;
                        lblRemark2.Hidden = false;
                        RemarkImg2.Hidden = false;
                        lblRemark.Hidden = false;
                    }

                
                    DiscountView.Hidden = true;
                    Utils.SetConstant(DiscountView.Constraints, NSLayoutAttribute.Height, 0);
                    DiscountCusView.Hidden = true;
                    Utils.SetConstant(DiscountCusView.Constraints, NSLayoutAttribute.Height, 0);


                    lblAddDiscount.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                    //lDiscount.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    btnDeleteDiscount.Hidden = true;
                    lblDiscount.Hidden = true;
                    lblAddDiscount.Text = Utils.TextBundle("adddiscount", "Items");

                    if (this.tranWithDetails.tranTradDiscounts.Count > 0 )
                    {
                        foreach (var discount in this.tranWithDetails.tranTradDiscounts)
                        {
                            if (discount.DiscountType == "MD")
                            {
                                string CartDiscount;
                                double discount2, disDiscont;
                                DiscountView.Hidden = false;
                                var check = discount.FmlDiscount.IndexOf('%');
                                if (check == -1)
                                {
                                    CartDiscount = discount.FmlDiscount;
                                    discount2 = Convert.ToDouble(CartDiscount);
                                    lbltxtDiscount.Text = Utils.TextBundle("discountx", "Items") + discount.FmlDiscount;
                                    disDiscont = discount2;
                                }
                                else
                                {

                                    discount2 = Convert.ToDouble(discount.FmlDiscount.Remove(check));
                                    lbltxtDiscount.Text = Utils.TextBundle("discountx", "Items") + discount.FmlDiscount;
                                    discount2 = discount2 / 100;
                                    disDiscont = Convert.ToDouble(this.tranWithDetails.tran.GrandTotal) * discount2;
                                }
                            
                            
                            
                                lblAddDiscount.Text = Utils.TextBundle("discount", "Items");
                                lblAddDiscount.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor, -9).Active = true;
                                lblDiscount.HeightAnchor.ConstraintEqualTo(18).Active = true;

                                btnDeleteDiscount.Hidden = false;
                                //lblRemark2.Hidden = false;
                                //RemarkImg2.Hidden = false;
                                lblDiscount.Hidden = false;
                                lblDiscount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(discount.TradeDiscHaveVat + discount.TradeDiscNoneVat));
                                lbldiscount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(discount.TradeDiscHaveVat+ discount.TradeDiscNoneVat));
                                //lbldiscount.Text = "-" + discount.FmlDiscount + "%";
                                Utils.SetConstant(DiscountView.Constraints, NSLayoutAttribute.Height, ((int)View.Frame.Height * 44) / 1000);


                            }
                            if (discount.DiscountType == "PS")
                            {
                                var disMember = Convert.ToDouble(discount.TradeDiscNoneVat + discount.TradeDiscHaveVat);
                                lbltxtDiscountCus.Text = Utils.TextBundle("memberx", "Items") + discount.FmlDiscount;
                                ////textNumMember.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                                DiscountCusView.Hidden = false;
                                lbldiscountCus.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                                Utils.SetConstant(DiscountCusView.Constraints, NSLayoutAttribute.Height, ((int)View.Frame.Height * 44) / 1000);
                            }
                        }
                    }
                
                    //if (disCountType == 'N')
                    //{
                    //    DiscountView.Hidden = true;
                    //    DiscountView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    //}
                    //else if (disCountType == 'P')
                    //{
                    //    DiscountView.Hidden = false;
                    //    lbldiscount.Text = "-" + disCountPercent + "%";
                    //    DiscountView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 44) / 1000).Active = true;
                    //}
                    //else
                    //{
                    //    DiscountView.Hidden = false;
                    //    lbldiscount.Text = "-" + disCountAmount;
                    //    DiscountView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 44) / 1000).Active = true;
                    //}

                    lbltotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);


                    lbltxtvat.Text = Utils.TextBundle("vat", "Items")+" " + Utils.DisplayDecimal( tranWithDetails.tran.TaxRate ?? 0) + " %";
                    lblvat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                    if (tranWithDetails.tran.TotalVat== 0)
                    {
                        Utils.SetConstant(vatView.Constraints, NSLayoutAttribute.Height, 0);
                        lbltxtvat.Hidden = true;
                        lblvat.Hidden = true;
                    }
                    if (DialogView!=null && DialogBGView!=null)
                    {
                        DialogView.Hidden = true;
                        DialogBGView.Hidden = true;
                    }
                    if (tranWithDetails.tran.ServiceCharge != 0 )
                    {
                        Utils.SetConstant(serviceView.Constraints, NSLayoutAttribute.Height, 30);
                        lblservice.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                        lbltxtservice.Hidden = false;
                        if (tranWithDetails.tran.FmlServiceCharge.Contains("%"))
                        {
                            lbltxtservice.Text = Utils.TextBundle("servicecharge", "Items")+" " + tranWithDetails.tran.FmlServiceCharge;
                        }
                        else
                        {
                            lbltxtservice.Text = Utils.TextBundle("servicecharge", "Items");
                        }

                    }
                    else
                    {
                        Utils.SetConstant(serviceView.Constraints, NSLayoutAttribute.Height, 0);
                        lbltxtservice.Hidden = true;
                        lblservice.Hidden = true;
                    }
                    //if (alert)
                    //{

                    //}
                    bool flagitemstatusd = false, flagcartupdate = false;

                    //1. เคสสินค้าถูกลบ 
                    lstSysItemIdStatusD = Utils.CheckStatusIteminCart(tranWithDetails);
                    //lstSysItemIdStatusD.Add("73000051");
                    if (lstSysItemIdStatusD.Count > 0)
                    {
                        flagitemstatusd = true;
                    }
                    HasChangeinCart hasChange = await Utils.CheckSettingConfiginCart(tranWithDetails);
                    if (hasChange.FlagChange)
                    {
                        flagcartupdate = true;
                        tranWithDetails = hasChange.tranWithDetailsLocal;
                    }

                    if (!flagitemstatusd && !flagcartupdate)
                    {
                    
                    }
                    else if (flagitemstatusd && !flagcartupdate)
                    {
                        //DialogShowItemStatusDClick("itemstatusd");
                        Utils.ShowAlert(this, "", Utils.TextBundle("cartitemdelete", ""));
                    }
                    else if (!flagitemstatusd && flagcartupdate)
                    {
                        //DialogShowItemStatusDClick("cartupdate");
                        Utils.ShowAlert(this, "", Utils.TextBundle("cartitemupdate", ""));
                    }
                    else
                    {
                        //DialogShowItemStatusDClick("updatecart");
                        Utils.ShowAlert(this, "", Utils.TextBundle("cartitemdeleteandupdate", ""));
                    }
                    //if(tranWithDetails.tran.SysCustomerID != null)
                }
                checkbtn();
            }
            catch (Exception ex)
            {

            }
        }
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            string x = "6.00%";
            var y = x.Remove(4);
            View.BackgroundColor = new UIColor(248 / 255f, 248 / 255f, 248 / 255f, 1);
            
            try
            {
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
                
                initAttribute();
                SetupAutoLayout();
                CheckOutDetailDialog();
                
                if (this.tranWithDetails == null)
                {
                    btnchackout.Enabled = false;
                    btndetail.Enabled = false;
                    EmptyView.Hidden = false;
                    TotalView.Hidden = true;
                    vatView.Hidden = true;
                    DiscountView.Hidden = true;
                    RemarkView.Hidden = true;
                    CartCollectionview.Hidden = true;

                    //if (disCountType == 'N')
                    //{
                    //    DiscountView.Hidden = true;
                    //    DiscountView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    //}
                    //else if (disCountType == 'P')
                    //{
                    //    DiscountView.Hidden = false;
                    //    lbldiscount.Text = "-" + disCountPercent + "%";
                    //    DiscountView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 44) / 1000).Active = true;
                    //}
                    //else
                    //{
                    //    DiscountView.Hidden = false;
                    //    lbldiscount.Text = "-" + disCountAmount;
                    //    DiscountView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 44) / 1000).Active = true;
                    //}
                }
                else
                {
                    btnchackout.Enabled = true;
                    btndetail.Enabled = true;
                    EmptyView.Hidden = true;
                    TotalView.Hidden = false;
                    vatView.Hidden = false;
                    DiscountView.Hidden = false;
                    RemarkView.Hidden = false;
                    CartCollectionview.Hidden = false;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        void CheckOutDetailDialog()
        {
            //open checkoutcontroller
            #region initAttribute
            DialogBGView = new UIView();
            DialogBGView.TranslatesAutoresizingMaskIntoConstraints = false;
            DialogBGView.BackgroundColor = UIColor.Black;
            DialogBGView.Hidden = true;
            DialogBGView.Layer.Opacity = 0.4f;
            View.AddSubview(DialogBGView);

            DialogBGView.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Close:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DialogBGView.AddGestureRecognizer(tapGesture1);

            DialogView = new UIView();
            DialogView.TranslatesAutoresizingMaskIntoConstraints = false;
            DialogView.BackgroundColor = UIColor.White;
            DialogView.Hidden = true;
            View.AddSubview(DialogView);

            #region OptionView
            OptionView = new UIView();
            OptionView.TranslatesAutoresizingMaskIntoConstraints = false;
            OptionView.BackgroundColor = UIColor.White;
            DialogView.AddSubview(OptionView);
            

            lblOption = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.Black,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblOption.Font = lblOption.Font.WithSize(15);
            lblOption.TranslatesAutoresizingMaskIntoConstraints = false;
            lblOption.Text = Utils.TextBundle("option", "Items");
            OptionView.AddSubview(lblOption);

            btnCancel = new UIButton();
            btnCancel.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            btnCancel.SetTitleColor(UIColor.FromRGB(226, 226, 226), UIControlState.Normal);
            btnCancel.SetTitle(Utils.TextBundle("textcancel", "Cancel"), UIControlState.Normal);
            btnCancel.TranslatesAutoresizingMaskIntoConstraints = false;
            btnCancel.TouchUpInside += (sender, e) => {
                // select type
                DialogView.Hidden = true;
                DialogBGView.Hidden = true;
            };
            OptionView.AddSubview(btnCancel);
            #endregion

            #region AddRemark
            AddRemarkView = new UIView();
            AddRemarkView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddRemarkView.BackgroundColor = UIColor.White;
            DialogView.AddSubview(AddRemarkView);

            lblAddRemark = new UILabel();
            lblAddRemark.TranslatesAutoresizingMaskIntoConstraints = false;
            lblAddRemark.TextAlignment = UITextAlignment.Left;
            lblAddRemark.Font = lblAddRemark.Font.WithSize(15);
            lblAddRemark.TextColor = UIColor.FromRGB(64, 64, 64);
            AddRemarkView.AddSubview(lblAddRemark);

            lblRemark = new UILabel();
            //lblRemark.Text = POSController.remark;
            lblRemark.TranslatesAutoresizingMaskIntoConstraints = false;
            lblRemark.TextAlignment = UITextAlignment.Left;
            lblRemark.Font = lblAddRemark.Font.WithSize(15);
            lblRemark.TextColor = UIColor.FromRGB(0, 149, 218);
            AddRemarkView.AddSubview(lblRemark);

            btnDeleteRemark = new UIButton();
            btnDeleteRemark.SetBackgroundImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDeleteRemark.BackgroundColor = UIColor.White;
            btnDeleteRemark.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDeleteRemark.TouchUpInside += (sender, e) => {
                tranWithDetails.tran.Comments = null;
                lblRemark.Text = "";
                lblAddRemark.Text = Utils.TextBundle("addremark", "Items");

                lblRemark.HeightAnchor.ConstraintEqualTo(0).Active = true;
                lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                //     lblRemark.HeightAnchor.ConstraintEqualTo(0).Active = true;
                lblRemark.Hidden = true;
                btnDeleteRemark.Hidden = true;
                Utils.SetConstant(RemarkView.Constraints,NSLayoutAttribute.Height , 0);
                RemarkImg2.Hidden = false;
                lblRemark2.Hidden = true;
                RemarkImg2.Hidden = true;
                lblRemark.Hidden = true;


            };
            AddRemarkView.AddSubview(btnDeleteRemark);

           

            RemarkImg = new UIImageView();
            RemarkImg.TranslatesAutoresizingMaskIntoConstraints = false;
            RemarkImg.Image = UIImage.FromBundle("Remark");
            AddRemarkView.AddSubview(RemarkImg);

            AddRemarkView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Remark:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            AddRemarkView.AddGestureRecognizer(tapGesture0);
            #endregion

            #region AddDiscount

            AddDiscountView = new UIView();
            AddDiscountView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddDiscountView.BackgroundColor = UIColor.White;
            DialogView.AddSubview(AddDiscountView);

            lblAddDiscount = new UILabel();
            lblAddDiscount.Text = Utils.TextBundle("adddiscount", "Items");
            lblAddDiscount.TranslatesAutoresizingMaskIntoConstraints = false;
            lblAddDiscount.TextAlignment = UITextAlignment.Left;
            lblAddDiscount.Font = lblAddDiscount.Font.WithSize(15);
            lblAddDiscount.TextColor = UIColor.FromRGB(64, 64, 64);
            AddDiscountView.AddSubview(lblAddDiscount);

            DiscountImg = new UIImageView();
            DiscountImg.TranslatesAutoresizingMaskIntoConstraints = false;
            DiscountImg.Image = UIImage.FromBundle("DiscountCart");
            AddDiscountView.AddSubview(DiscountImg);

            AddDiscountView.UserInteractionEnabled = true;
            var tap = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Discount:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            AddDiscountView.AddGestureRecognizer(tap);
            
            lblDiscount = new UILabel();
            //lblRemark.Text = POSController.remark;
            lblDiscount.TranslatesAutoresizingMaskIntoConstraints = false;
            lblDiscount.TextAlignment = UITextAlignment.Left;
            lblDiscount.Font = lblAddRemark.Font.WithSize(15);
            lblDiscount.TextColor = UIColor.FromRGB(0, 149, 218);
            AddDiscountView.AddSubview(lblDiscount);

            btnDeleteDiscount = new UIButton();
            btnDeleteDiscount.SetBackgroundImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDeleteDiscount.BackgroundColor = UIColor.White;
            btnDeleteDiscount.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDeleteDiscount.TouchUpInside += (sender, e) => {
                POSController.tranWithDetails = BLTrans.RemoveDiscount(POSController.tranWithDetails, "MD");

                lblDiscount.Text = "";
                lblAddDiscount.Text = Utils.TextBundle("adddiscount", "Items");

                //lblDiscount.HeightAnchor.ConstraintEqualTo(0).Active = true;
                lblAddDiscount.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                //     lblRemark.HeightAnchor.ConstraintEqualTo(0).Active = true;
                lblDiscount.Hidden = true;
                btnDeleteDiscount.Hidden = true;
                Utils.SetConstant(DiscountView.Constraints,NSLayoutAttribute.Height , 0);
                //RemarkImg2.Hidden = false;
                //lblRemark2.Hidden = true;
                //RemarkImg2.Hidden = true;
                //lblDiscount.Hidden = true;


            };
            AddDiscountView.AddSubview(btnDeleteDiscount);

            #endregion

            #region ClearCart

            ClearCartView = new UIView();
            ClearCartView.TranslatesAutoresizingMaskIntoConstraints = false;
            ClearCartView.BackgroundColor = UIColor.White;
            DialogView.AddSubview(ClearCartView);

            lblClearCart = new UILabel();
            lblClearCart.Text = Utils.TextBundle("clearcart", "Items");
            lblClearCart.TranslatesAutoresizingMaskIntoConstraints = false;
            lblClearCart.TextAlignment = UITextAlignment.Left;
            lblClearCart.Font = lblAddDiscount.Font.WithSize(15);
            lblClearCart.TextColor = UIColor.FromRGB(227, 45, 73);
            ClearCartView.AddSubview(lblClearCart);

            ClearImage = new UIImageView();
            ClearImage.TranslatesAutoresizingMaskIntoConstraints = false;
            ClearImage.Image = UIImage.FromBundle("ClearCart");
            ClearCartView.AddSubview(ClearImage);

            ClearCartView.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Clear:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ClearCartView.AddGestureRecognizer(tapGesture2);
            #endregion

            #region CheckOut
            CheckOutView = new UIView();
            CheckOutView.TranslatesAutoresizingMaskIntoConstraints = false;
            CheckOutView.BackgroundColor = UIColor.White;
            DialogView.AddSubview(CheckOutView);

            btnMore = new UIButton();
            btnMore.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnMore.Layer.CornerRadius = 5;
            btnMore.TranslatesAutoresizingMaskIntoConstraints = false;
            btnMore.TouchUpInside += (sender, e) => {
                //   DataCaching.CheckOutNavigation.DismissViewController(true, null);
                DialogView.Hidden = true;
                DialogBGView.Hidden = true;
             //   View.BringSubviewToFront(DialogBGView);
                View.BringSubviewToFront(DialogView);
            };
            CheckOutView.AddSubview(btnMore);

            moreImg = new UIImageView();
            moreImg.Image = UIImage.FromBundle("Option");
            moreImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            moreImg.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            moreImg.TranslatesAutoresizingMaskIntoConstraints = false;
            btnMore.AddSubview(moreImg);

            btnCheckOut = new UIButton();
            btnCheckOut.Layer.CornerRadius = 5;
            btnCheckOut.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            btnCheckOut.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnCheckOut.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            btnCheckOut.SetTitle(Utils.TextBundle("pay", "Items"), UIControlState.Normal);
            btnCheckOut.TranslatesAutoresizingMaskIntoConstraints = false;
            btnCheckOut.TouchUpInside += (sender, e) => {
                // select type
                // this.NavigationController.DismissViewController(true,null);
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("payment", "Items"));
                DataCaching.paymentpage = new PaymentController();
                this.NavigationController.PushViewController(DataCaching.paymentpage, false);
            };
            CheckOutView.AddSubview(btnCheckOut);
            #endregion
            #endregion

            #region setup layout
            DialogBGView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
            DialogBGView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            DialogBGView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            DialogBGView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;


            DialogView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
            DialogView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DialogView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            DialogView.HeightAnchor.ConstraintEqualTo(320).Active = true;

            #region OptionView
            OptionView.BottomAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            OptionView.LeftAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            OptionView.RightAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            OptionView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            lblOption.CenterYAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblOption.LeftAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblOption.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblOption.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnCancel.CenterYAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnCancel.RightAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            btnCancel.WidthAnchor.ConstraintEqualTo(80).Active = true;
            btnCancel.HeightAnchor.ConstraintEqualTo(30).Active = true;
            #endregion

            #region AddRemarkView
            AddRemarkView.BottomAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            AddRemarkView.LeftAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AddRemarkView.RightAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            AddRemarkView.HeightAnchor.ConstraintEqualTo(70).Active = true;

            RemarkImg.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            RemarkImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            RemarkImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            RemarkImg.LeftAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            btnDeleteRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnDeleteRemark.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnDeleteRemark.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnDeleteRemark.RightAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

            lblAddRemark.RightAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            // lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblAddRemark.LeftAnchor.ConstraintEqualTo(RemarkImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;

            lblRemark.RightAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            lblRemark.TopAnchor.ConstraintEqualTo(lblAddRemark.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblRemark.LeftAnchor.ConstraintEqualTo(RemarkImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            #endregion

            #region AddDiscount
            AddDiscountView.BottomAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            AddDiscountView.LeftAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AddDiscountView.RightAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            AddDiscountView.HeightAnchor.ConstraintEqualTo(70).Active = true;

            DiscountImg.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            DiscountImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            DiscountImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            DiscountImg.LeftAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            btnDeleteDiscount.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnDeleteDiscount.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnDeleteDiscount.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnDeleteDiscount.RightAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

            lblAddDiscount.RightAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            // lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblAddDiscount.LeftAnchor.ConstraintEqualTo(DiscountImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;

            lblDiscount.RightAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            lblDiscount.TopAnchor.ConstraintEqualTo(lblAddDiscount.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblDiscount.LeftAnchor.ConstraintEqualTo(DiscountImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;

            //lblAddDiscount.RightAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            //lblAddDiscount.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            //lblAddDiscount.LeftAnchor.ConstraintEqualTo(DiscountImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;


            #endregion

            #region ClearCart
            ClearCartView.BottomAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            ClearCartView.LeftAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ClearCartView.RightAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ClearCartView.HeightAnchor.ConstraintEqualTo(70).Active = true;

            ClearImage.CenterYAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ClearImage.WidthAnchor.ConstraintEqualTo(28).Active = true;
            ClearImage.HeightAnchor.ConstraintEqualTo(28).Active = true;
            ClearImage.LeftAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            lblClearCart.RightAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            lblClearCart.CenterYAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblClearCart.LeftAnchor.ConstraintEqualTo(ClearImage.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            #endregion

            #region CheckOut
            CheckOutView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            CheckOutView.LeftAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CheckOutView.RightAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CheckOutView.BottomAnchor.ConstraintEqualTo(DialogView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            btnMore.TopAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnMore.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnMore.BottomAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnMore.LeftAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;

            moreImg.TopAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            moreImg.RightAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;
            moreImg.BottomAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            moreImg.LeftAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;

            btnCheckOut.TopAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnCheckOut.RightAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnCheckOut.BottomAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnCheckOut.LeftAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            #endregion
            #endregion
        }
        [Export("Remark:")]
        public void Remark(UIGestureRecognizer sender)
        {
           
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("addremark", "Items"));
            RemarkController remark = new RemarkController();
            //   DataCaching.CheckOutNavigation.DismissViewController(false, null);
            this.NavigationController.PushViewController(remark, false);
        }
        [Export("Close:")]
        public void Close(UIGestureRecognizer sender)
        {
            DialogBGView.Hidden = true;
            DialogView.Hidden = true;
        }
        [Export("Discount:")]
        public void Discount(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("adddiscount", "Items"));
            DiscountController Discount = new DiscountController(POSController.tranWithDetails);
         //   DataCaching.CheckOutNavigation.DismissViewController(false, null);
            this.NavigationController.PushViewController(Discount, false);
        }
        [Export("Clear:")]
        public void Clear(UIGestureRecognizer sender)
        {

            var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wantclearcart", "Items"), UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                async alert => await ClearCart()));
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
            PresentViewController(okCancelAlertController, true, null);

        }

        private async Task ClearCart()
        {
            if (tranWithDetails.tran.TranType == 'O')
            {
                await Utils.CancelTranOrder(tranWithDetails);
               
            }

            POSController.tranWithDetails = null;
            POSController.SelectedCustomer = null;
            
            this.tranWithDetails = null;
            btnchackout.Enabled = false;

            //((CartDataSource)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItemWithToppings);
            //CartCollectionview.ReloadData();
            DialogBGView.Hidden = true;
            DialogView.Hidden = true;
            DataCaching.posPage.initialData();
            this.NavigationController.PopViewController(false);
        }

        void initAttribute()
        {

            #region Bottom
            viewfoot = new UIView();
            viewfoot.BackgroundColor = UIColor.White;
            viewfoot.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(viewfoot);

            btnchackout = new UIButton();
            btnchackout.SetTitle(Utils.TextBundle("pay", "Pay"), UIControlState.Normal);
            btnchackout.TranslatesAutoresizingMaskIntoConstraints = false;
            btnchackout.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnchackout.Layer.CornerRadius = 5f;
            btnchackout.BackgroundColor = UIColor.FromRGB(51, 172, 225);
            btnchackout.TouchUpInside += (sender, e) => {
                btnchackout_TouchUpInside();
            };
            viewfoot.AddSubview(btnchackout);

            btndetail = new UIButton();
            btndetail.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btndetail.TranslatesAutoresizingMaskIntoConstraints = false;
            //btndetail.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            //btndetail.ImageEdgeInsets = new UIEdgeInsets(10, 10,10 , 10);
            //btndetail.SetBackgroundImage(UIImage.FromBundle("Option"), UIControlState.Normal);
            btndetail.Layer.CornerRadius = 5f;
            btndetail.TouchUpInside += (sender, e) => {
                DialogView.Hidden = false;
                DialogBGView.Hidden = false;
            };
            viewfoot.AddSubview(btndetail);

            moreImg = new UIImageView();
            moreImg.Image = UIImage.FromBundle("Option");
            moreImg.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            moreImg.TranslatesAutoresizingMaskIntoConstraints = false;
            moreImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            btndetail.AddSubview(moreImg);
            #endregion

            #region EmptyView;
            EmptyView = new UIView();
            EmptyView.BackgroundColor = UIColor.White;
            EmptyView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(EmptyView);

            EmptyImg = new UIImageView();
            EmptyImg.TranslatesAutoresizingMaskIntoConstraints = false;
            EmptyImg.Image = UIImage.FromBundle("EmptyCart");
            EmptyView.AddSubview(EmptyImg);

            lblEmpty = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblEmpty.Text = Utils.TextBundle("emptycart", "Items");
            lblEmpty.Font = lblEmpty.Font.WithSize(15);
            EmptyView.AddSubview(lblEmpty);
            #endregion

            #region Total
            TotalView = new UIView();
            TotalView.BackgroundColor = UIColor.White;
            TotalView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(TotalView);

            lbltotal = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbltotal.TextColor = UIColor.FromRGB(138, 211, 245);
            lbltotal.TextAlignment = UITextAlignment.Right;
            lbltotal.Font = lbltotal.Font.WithSize(15);
            lbltotal.BackgroundColor = UIColor.White;
            //    lbltotal.Text = tranWithDetails.tran.Total.ToString("#,##0.00") ;

            TotalView.AddSubview(lbltotal);

            lbltxttotal = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbltxttotal.TextColor = UIColor.FromRGB(64, 64, 64);
            lbltxttotal.TextAlignment = UITextAlignment.Left;
            lbltxttotal.Font = lbltotal.Font.WithSize(15);
            lbltxttotal.BackgroundColor = UIColor.White;
            lbltxttotal.Text = Utils.TextBundle("total", "Items");
            TotalView.AddSubview(lbltxttotal);
            #endregion

            #region Vat
            vatView = new UIView();
            vatView.BackgroundColor = UIColor.White;
            vatView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(vatView);

            lblvat = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblvat.TextColor = UIColor.FromRGB(162, 162, 162);
            lblvat.TextAlignment = UITextAlignment.Right;
            lblvat.Font = lblvat.Font.WithSize(15);
            lblvat.BackgroundColor = UIColor.White;
            // lblvat.Text = tranWithDetails.tran.Total.ToString("#,##0.00");

            vatView.AddSubview(lblvat);

            lbltxtvat = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbltxtvat.TextColor = UIColor.FromRGB(162, 162, 162);
            lbltxtvat.TextAlignment = UITextAlignment.Left;
            lbltxtvat.Font = lbltxtvat.Font.WithSize(15);
            lbltxtvat.BackgroundColor = UIColor.White;
            lbltxtvat.Text = Utils.TextBundle("vat", "Items");
            vatView.AddSubview(lbltxtvat);
            #endregion

            #region service
            serviceView = new UIView();
            serviceView.BackgroundColor = UIColor.White;
            serviceView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(serviceView);

            lblservice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblservice.TextColor = UIColor.FromRGB(162, 162, 162);
            lblservice.TextAlignment = UITextAlignment.Right;
            lblservice.Font = lblvat.Font.WithSize(15);
            lblservice.BackgroundColor = UIColor.White;
            // lblvat.Text = tranWithDetails.tran.Total.ToString("#,##0.00");

            serviceView.AddSubview(lblservice);

            lbltxtservice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbltxtservice.TextColor = UIColor.FromRGB(162, 162, 162);
            lbltxtservice.TextAlignment = UITextAlignment.Left;
            lbltxtservice.Font = lbltxtvat.Font.WithSize(15);
            lbltxtservice.BackgroundColor = UIColor.White;
            lbltxtservice.Text = Utils.TextBundle("servicechange", "Items");
            lbltxtservice.Hidden = true; 
            serviceView.AddSubview(lbltxtservice);
            #endregion

            #region Discount
            DiscountView = new UIView();
            DiscountView.BackgroundColor = UIColor.White;
            DiscountView.TranslatesAutoresizingMaskIntoConstraints = false;
            DiscountView.Hidden = true;
            View.AddSubview(DiscountView);

            lbldiscount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbldiscount.TextColor = UIColor.FromRGB(162, 162, 162);
            lbldiscount.TextAlignment = UITextAlignment.Right;
            lbldiscount.Font = lblvat.Font.WithSize(15);
            lbldiscount.BackgroundColor = UIColor.White;
            DiscountView.AddSubview(lbldiscount);

            lbltxtDiscount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbltxtDiscount.TextColor = UIColor.FromRGB(162, 162, 162);
            lbltxtDiscount.TextAlignment = UITextAlignment.Left;
            lbltxtDiscount.Font = lbltxtDiscount.Font.WithSize(15);
            lbltxtDiscount.BackgroundColor = UIColor.White;
            lbltxtDiscount.Text = Utils.TextBundle("discount", "Items");
            DiscountView.AddSubview(lbltxtDiscount);
            #endregion

            #region DiscountCus
            DiscountCusView = new UIView();
            DiscountCusView.BackgroundColor = UIColor.White;
            DiscountCusView.TranslatesAutoresizingMaskIntoConstraints = false;
            DiscountCusView.Hidden = true;
            View.AddSubview(DiscountCusView);

            lbldiscountCus = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbldiscountCus.TextColor = UIColor.FromRGB(162, 162, 162);
            lbldiscountCus.TextAlignment = UITextAlignment.Right;
            lbldiscountCus.Font = lblvat.Font.WithSize(15);
            lbldiscountCus.BackgroundColor = UIColor.White;
            DiscountCusView.AddSubview(lbldiscountCus);

            lbltxtDiscountCus = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbltxtDiscountCus.TextColor = UIColor.FromRGB(162, 162, 162);
            lbltxtDiscountCus.TextAlignment = UITextAlignment.Left;
            lbltxtDiscountCus.Font = lbltxtDiscountCus.Font.WithSize(15);
            lbltxtDiscountCus.BackgroundColor = UIColor.White;
            lbltxtDiscountCus.Text = Utils.TextBundle("discount", "Items");
            DiscountCusView.AddSubview(lbltxtDiscountCus);
            #endregion

            #region RemarkView
            RemarkView = new UIView();
            //if (POSController.remark == null || POSController.remark == "")
            //{
            //    RemarkView.Hidden = true;
            //    //  lblRemark.Text = "";
            //}
            //else
            //{
            //    RemarkView.Hidden = false;
            //}
            RemarkView.BackgroundColor = UIColor.White;
            RemarkView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(RemarkView);

            RemarkImg2 = new UIImageView();
            RemarkImg2.Image = UIImage.FromBundle("Remark");
            RemarkImg2.BackgroundColor = UIColor.White;
            RemarkImg2.TranslatesAutoresizingMaskIntoConstraints = false;
            RemarkView.AddSubview(RemarkImg2);

            lblRemark2 = new UILabel();
            //lblRemark2.Text = POSController.remark;
            lblRemark2.TranslatesAutoresizingMaskIntoConstraints = false;
            lblRemark2.TextAlignment = UITextAlignment.Right;
            lblRemark2.Font = lblRemark2.Font.WithSize(15);
            lblRemark2.TextColor = UIColor.FromRGB(162, 162, 162);
            RemarkView.AddSubview(lblRemark2);
            #endregion

            #region CollectionView
            UICollectionViewFlowLayout itemflowLayout = new UICollectionViewFlowLayout();
            itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayout.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize ;
            CartCollectionview = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
            CartCollectionview.BackgroundColor = new UIColor(248 / 255f, 248 / 255f, 248 / 255f, 1);
            CartCollectionview.TranslatesAutoresizingMaskIntoConstraints = false;
           // CartCollectionview.SizeToFit();



            CartCollectionview.RegisterClassForCell(cellType: typeof(CartCollectionViewCell2), reuseIdentifier: "CartCell2");
            CartCollectionview.RegisterClassForCell(cellType: typeof(CartCollectionViewCell3), reuseIdentifier: "CartCell3");
            var cartItem = new CartItem();
            cartItemlist = new List<CartItem>();
            cartDataSource = new CartDataSource(tranWithDetails.tranDetailItemWithToppings, CartCollectionview.Frame);
            cartDataSource.OnCardCellbtnIndex0 += (indexPath) =>
            {
                if (!Is)
                {
                    return;
                }
                Is = false;
                var item = tranWithDetails.tranDetailItemWithToppings[indexPath.Row];
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("quantity", "Items"));
                var quantityController = new CartQuantityController(item.tranDetailItem);
                this.NavigationController.PushViewController(quantityController, false);
            };
            cartDataSource.OnCardCellbtnIndex1 += (indexPath) =>
            {
                if (!Is)
                {
                    return;
                }
                Is = false;
               
                var item = tranWithDetails.tranDetailItemWithToppings[indexPath.Row];
                if (item.tranDetailItem.SysItemID == null)
                {
                    return;
                }
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("editoption", "Items"));
                var optionController = new OptionController(item, true);
                this.NavigationController.PushViewController(optionController, false);
            };
            cartDataSource.OnCardCellbtnIndex2 += (indexPath) =>
            {
                if (!Is)
                {
                    return;
                }
                Is = false;
                var item = tranWithDetails.tranDetailItemWithToppings[indexPath.Row];
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("changeprice", "Items"));
                var quantityController = new CartDummyController(item.tranDetailItem);
                this.NavigationController.PushViewController(quantityController, false);
            };
            cartDataSource.OnCardCellbtnIndex3 += (indexPath) =>
            {
                if (!Is)
                {
                    return;
                }
                Is = false;
                var item = tranWithDetails.tranDetailItemWithToppings[indexPath.Row];
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("discountitem", "Items"));
                var quantityController = new DiscountController(item.tranDetailItem);
                this.NavigationController.PushViewController(quantityController, false);
            };
            cartDataSource.OnCardCellbtnIndex4 += (indexPath) =>
            {
                if (!(tranWithDetails.tranDetailItemWithToppings[indexPath.Row] is null))
                {
                    var item = tranWithDetails.tranDetailItemWithToppings[indexPath.Row];
                    POSController.tranWithDetails = BLTrans.RemoveDetailItem(POSController.tranWithDetails, item);
                    POSController.tranWithDetails = BLTrans.Caltran(POSController.tranWithDetails);
                    tranWithDetails = POSController.tranWithDetails;
                    CartCollectionview.ReloadData();
                    lbltotal.Text = tranWithDetails.tran.GrandTotal.ToString("#,##0.00");
                    lblvat.Text = tranWithDetails.tran.TotalVat.ToString("#,##0.00");
                    lblservice.Text = tranWithDetails.tran.ServiceCharge.ToString("#,##0.00");
                    if (tranWithDetails.tranDetailItemWithToppings.Count<1)
                    {
                        btnchackout.Enabled = false;
                        btnchackout.BackgroundColor = UIColor.FromRGBA(51, 172, 225, 100);
                    }
                }


            };
            CartCollectionview.DataSource = cartDataSource;
            
           
            CartCollectionDelegate cartDelegate = new CartCollectionDelegate();
            cartDelegate.OnItemSelected += async (indexPath) => {
                if (tranWithDetails.tranDetailItemWithToppings[indexPath.Row].tranDetailItem.Comments == "delete")
                {
                    return;
                }
                if (!tranWithDetails.tranDetailItemWithToppings[indexPath.Row].tranDetailItem.choose)
                {
                    tranWithDetails.tranDetailItemWithToppings.ConvertAll(x => x.tranDetailItem.choose = false);
                    tranWithDetails.tranDetailItemWithToppings[indexPath.Row].tranDetailItem.choose = true;
                }
                else
                {
                    tranWithDetails.tranDetailItemWithToppings.ConvertAll(x => x.tranDetailItem.choose = false);
                }
                //((CartDataSource)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItemWithToppings);

                CartCollectionview.ReloadInputViews();
                CartCollectionview.ReloadData();
                //CartCollectionview.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, true);


            };
            CartCollectionview.Delegate = cartDelegate;
            View.AddSubview(CartCollectionview);
            #endregion

            line = new UIView();
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(line);

           
            m_scannerView =
            new ZXingScannerView(new CGRect(0, 0, View.Frame.Width, (View.Frame.Height / 2) - 100))
            {
                UseCustomOverlayView = true,
            };
            m_scannerView.TopText = "";
            m_scannerView.AutoFocus();
            m_scannerView.TranslatesAutoresizingMaskIntoConstraints = false;
            m_scannerView.ResizePreview(UIInterfaceOrientation.LandscapeLeft);
            View.AddSubview(m_scannerView);

        }
        
        private void btnchackout_TouchUpInside()
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("payment", "Items"));
            DataCaching.paymentpage = new PaymentController();
            //if (paymentPage == null)
            //{
            //    paymentPage = new PaymentController();
            //}
            this.NavigationController.PushViewController(DataCaching.paymentpage, false);
        }
        private void SetupAutoLayout()
        {
            #region EmptyView
            EmptyView.BackgroundColor = UIColor.Green;
            EmptyView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            EmptyView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            EmptyView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            EmptyView.BottomAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            EmptyImg.TopAnchor.ConstraintEqualTo(EmptyView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height*38)/1000).Active = true;
            EmptyImg.CenterXAnchor.ConstraintEqualTo(EmptyView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            EmptyImg.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 8) / 10).Active = true;
            EmptyImg.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 262) / 1000).Active = true;

            lblEmpty.TopAnchor.ConstraintEqualTo(EmptyImg.SafeAreaLayoutGuide.BottomAnchor, ((int)View.Frame.Height * 28) / 1000).Active = true;
            lblEmpty.CenterXAnchor.ConstraintEqualTo(EmptyView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblEmpty.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width * 8) / 10).Active = true;
            #endregion

            m_scannerView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            m_scannerView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            m_scannerView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            if (scan)
            {
                m_scannerView.HeightAnchor.ConstraintEqualTo((View.Frame.Height/2)-100).Active = true;
            }
            else
            {
                m_scannerView.HeightAnchor.ConstraintEqualTo(0).Active = true;
            }


            CartCollectionview.TopAnchor.ConstraintEqualTo(m_scannerView.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            CartCollectionview.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CartCollectionview.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CartCollectionview.BottomAnchor.ConstraintEqualTo(RemarkView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            

            

            #region viewfoot
            viewfoot.HeightAnchor.ConstraintEqualTo(65).Active = true;
            viewfoot.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            viewfoot.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewfoot.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            //viewfoot.BackgroundColor = UIColor.Red;

            btndetail.BottomAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btndetail.TopAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btndetail.LeftAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.LeftAnchor,10).Active = true;
            btndetail.WidthAnchor.ConstraintEqualTo(45).Active = true;

            moreImg.TopAnchor.ConstraintEqualTo(btndetail.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            moreImg.RightAnchor.ConstraintEqualTo(btndetail.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;
            moreImg.BottomAnchor.ConstraintEqualTo(btndetail.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            moreImg.LeftAnchor.ConstraintEqualTo(btndetail.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;

            btnchackout.TopAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnchackout.LeftAnchor.ConstraintEqualTo(btndetail.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            btnchackout.RightAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnchackout.BottomAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            #endregion

            #region serviceView
            serviceView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            serviceView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            serviceView.BottomAnchor.ConstraintEqualTo(viewfoot.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            serviceView.HeightAnchor.ConstraintEqualTo(0).Active = true;

            lbltxtservice.LeftAnchor.ConstraintEqualTo(serviceView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lbltxtservice.CenterYAnchor.ConstraintEqualTo(serviceView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtservice.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lblservice.RightAnchor.ConstraintEqualTo(serviceView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblservice.CenterYAnchor.ConstraintEqualTo(serviceView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblservice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            #endregion

            #region vatView
            vatView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            vatView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            vatView.BottomAnchor.ConstraintEqualTo(serviceView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            vatView.HeightAnchor.ConstraintEqualTo(30).Active = true;

            lbltxtvat.LeftAnchor.ConstraintEqualTo(vatView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lbltxtvat.CenterYAnchor.ConstraintEqualTo(vatView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtvat.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblvat.RightAnchor.ConstraintEqualTo(vatView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblvat.CenterYAnchor.ConstraintEqualTo(vatView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblvat.WidthAnchor.ConstraintEqualTo(100).Active = true;
            #endregion

            #region DiscountView
            DiscountView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DiscountView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            DiscountView.BottomAnchor.ConstraintEqualTo(vatView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            DiscountView.HeightAnchor.ConstraintEqualTo(0).Active = true;

            lbltxtDiscount.LeftAnchor.ConstraintEqualTo(DiscountView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lbltxtDiscount.CenterYAnchor.ConstraintEqualTo(DiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtDiscount.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lbldiscount.RightAnchor.ConstraintEqualTo(DiscountView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lbldiscount.CenterYAnchor.ConstraintEqualTo(DiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbldiscount.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region DiscountcusView
            DiscountCusView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DiscountCusView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            DiscountCusView.BottomAnchor.ConstraintEqualTo(DiscountView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            DiscountCusView.HeightAnchor.ConstraintEqualTo(30).Active = true;

            lbltxtDiscountCus.LeftAnchor.ConstraintEqualTo(DiscountCusView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lbltxtDiscountCus.CenterYAnchor.ConstraintEqualTo(DiscountCusView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxtDiscountCus.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lbldiscountCus.RightAnchor.ConstraintEqualTo(DiscountCusView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lbldiscountCus.CenterYAnchor.ConstraintEqualTo(DiscountCusView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbldiscountCus.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            

            #region TotalView
            TotalView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            TotalView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TotalView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            TotalView.BottomAnchor.ConstraintEqualTo(DiscountCusView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;

            lbltxttotal.LeftAnchor.ConstraintEqualTo(TotalView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lbltxttotal.CenterYAnchor.ConstraintEqualTo(TotalView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltxttotal.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lbltotal.RightAnchor.ConstraintEqualTo(TotalView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lbltotal.CenterYAnchor.ConstraintEqualTo(TotalView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltotal.WidthAnchor.ConstraintEqualTo(100).Active = true;
            #endregion

            line.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 7) / 1000).Active = true;
            line.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            line.BottomAnchor.ConstraintEqualTo(TotalView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;

            #region RemarkView
            RemarkView.HeightAnchor.ConstraintEqualTo(45).Active = true;
            RemarkView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            RemarkView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            RemarkView.BottomAnchor.ConstraintEqualTo(line.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            //RemarkView.BackgroundColor = UIColor.Red;

            RemarkImg2.LeftAnchor.ConstraintEqualTo(RemarkView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            RemarkImg2.CenterYAnchor.ConstraintEqualTo(RemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            RemarkImg2.WidthAnchor.ConstraintEqualTo(24).Active = true;
            RemarkImg2.HeightAnchor.ConstraintEqualTo(24).Active = true;

            lblRemark2.RightAnchor.ConstraintEqualTo(RemarkView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblRemark2.CenterYAnchor.ConstraintEqualTo(RemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblRemark2.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 2).Active = true;
            #endregion

            

        }
        CameraResolution HandleCameraResolutionSelectorDelegate(List<CameraResolution> availableResolutions)
        {
            //Don't know if this will ever be null or empty
            if (availableResolutions == null || availableResolutions.Count < 1)
                return new CameraResolution() { Width = 800, Height = 600 };

            //Debugging revealed that the last element in the list
            //expresses the highest resolution. This could probably be more thorough.
            return availableResolutions[availableResolutions.Count - 1];
        }
        public void checkbtn() 
        {
            if (tranWithDetails!=null)
            {
                if (tranWithDetails.tranDetailItemWithToppings.Count >0)
                {
                    btnchackout.Enabled = true;
                    btnchackout.BackgroundColor = UIColor.FromRGB(51, 172, 225);

                }
                else
                {
                    btnchackout.Enabled = false;
                    btnchackout.BackgroundColor = UIColor.FromRGB(51, 172, 225);
                    btnchackout.BackgroundColor = UIColor.FromRGBA(51, 172, 225, 100);

                }
                
            }

        }
        async void scannerInit()
        {
            try
            {
                var barcodeOptions = new ZXing.Mobile.MobileBarcodeScanningOptions
                {
                    CameraResolutionSelector = HandleCameraResolutionSelectorDelegate,
                    AutoRotate = true,//PossibleFormats = { ZXing.BarcodeFormat.QR_CODE, ZXing.BarcodeFormat.CODE_128,ZXing.BarcodeFormat. },
                    UseNativeScanning = true,
                    TryHarder = true,
                    DisableAutofocus = false,
                };

                if (scan)
                {
                    
                    m_scannerView.StartScanning(
                        async result =>
                        {
                        //m_scannerView.StopScanning();
                        if (result != null && !string.IsNullOrEmpty(result.Text))
                            {
                            // Handle scaneed result
                            ItemManage itemManage = new ItemManage();
                                var Itemresult = await itemManage.GetItemPOSfScanBarcode(result.Text);

                                if (Itemresult.Count == 1)
                                {
                                    var itemchoose = Itemresult[0];
                                    var trandetail = new TranDetailItemWithTopping()
                                    {
                                        tranDetailItem = new TranDetailItemNew()
                                        {
                                            SysItemID = itemchoose.SysItemID,
                                            MerchantID = DataCashingAll.MerchantId,
                                            SysBranchID = DataCashingAll.SysBranchId,
                                            ItemName = itemchoose.ItemName,
                                            SaleItemType = itemchoose.SaleItemType,
                                            FProcess = 1,
                                            TranNo = tranWithDetails.tran.TranNo,
                                            TaxType = itemchoose.TaxType,
                                            Quantity = 1,
                                            Price = itemchoose.Price,
                                            ItemPrice = itemchoose.Price,
                                            Discount = 0,
                                            EstimateCost = itemchoose.EstimateCost,
                                            DetailNo = (tranWithDetails.tranDetailItemWithToppings.Count+1)
                                        },
                                        tranDetailItemToppings = new List<TranDetailItemTopping>()
                                    };

                                    if (Itemresult[0].FDisplayOption == 1)
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            var cartPage = new OptionController(trandetail,false);
                                            this.NavigationController.PushViewController(cartPage, false);
                                        });
                                    }
                                    else
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, trandetail);
                                            tranWithDetails = BLTrans.Caltran(tranWithDetails);
                                        
                                            ((CartDataSource)CartCollectionview.DataSource).ReloadData(tranWithDetails.tranDetailItemWithToppings);
                                            CartCollectionview.ReloadData();
                                            CartCollectionview.LayoutIfNeeded();
                                            var index = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == trandetail.tranDetailItem.SysItemID);
                                            if (index >= 0) CartCollectionview.ScrollToItem(NSIndexPath.FromRowSection(index, 0), UICollectionViewScrollPosition.Top, true);


                                            lbltotal.Text = tranWithDetails.tran.GrandTotal.ToString("#,##0.00");
                                            lblvat.Text = tranWithDetails.tran.TotalVat.ToString("#,##0.00");
                                            var disMemberlist = this.tranWithDetails.tranTradDiscounts.Where(x=>x.DiscountType == "PS").FirstOrDefault();
                                            if (disMemberlist != null)
                                            {
                                                var disMember = Convert.ToDouble(disMemberlist.TradeDiscNoneVat + disMemberlist.TradeDiscHaveVat);
                                                lbltxtDiscountCus.Text = Utils.TextBundle("Member", "Items")+" " + disMemberlist.FmlDiscount;
                                                ////textNumMember.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                                                DiscountCusView.Hidden = false;
                                                lbldiscountCus.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                                                Utils.SetConstant(DiscountCusView.Constraints, NSLayoutAttribute.Height, ((int)View.Frame.Height * 44) / 1000);
                                            }
                                            //lbldiscountCus.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                                            CartController.Ismodify = true;
                                            var duration = TimeSpan.FromSeconds(1);
                                            Vibration.Vibrate(duration);
                                            UIView.Animate(0.2, () =>
                                            {
                                                m_scannerView.Alpha = 0; 
                                            }, () =>
                                            {
                                                m_scannerView.Alpha = 1;
                                            });
                                            Utils.ShowMessageDown(Utils.TextBundle("scansuccess", "Items"));
                                            checkbtn();

                                        });
                                    }
                                }
                                else if (Itemresult.Count > 1)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        POSController.scan = true;
                                        POSController.itemcode = result.Text;
                                        this.NavigationController.PopViewController(false);
                                        Utils.ShowMessage(Utils.TextBundle("more1item", "Items"));
                                    });
                                }
                                else
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        Utils.ShowAlert(this,Utils.TextBundle("notfinditem", "Items"), "");
                                        var duration = TimeSpan.FromSeconds(1);
                                        Vibration.Vibrate(duration);
                                        var sound = new SystemSound(1000);
                                        sound.PlaySystemSound();
                                        UIView.Animate(0.2, () =>
                                        {
                                            m_scannerView.Alpha = 0;
                                        }, () =>
                                        {
                                            m_scannerView.Alpha = 1;
                                        });
                                    });
                                }
                            }
                        }, barcodeOptions);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, "Error !", "10001");
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
    }
}