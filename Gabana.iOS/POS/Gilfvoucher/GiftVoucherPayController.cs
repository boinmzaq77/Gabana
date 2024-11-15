using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class GiftVoucherPayController : UIViewController
    {

        UIImageView emptyView;
        
        UILabel lblempty;
        ChooseGiftVoucher GiftSelect;
        double amount;
        TranWithDetailsLocal tranWithDetails;
        public static List<ChooseGiftVoucher> lstvouchers;
        
        UICollectionView GiftVoucherCollection = null;
        LstItemGiftVoucher listgiftvoucher;
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
        UIScrollView scroll;
        UIImageView btnAdd;
        UIButton ApplyGift; 
        public static bool isModifyGift = false;
        //   MerchantConfigManage configManage = new MerchantConfigManage();
        //VoucherList
        public GiftVoucherPayController() {
            this.tranWithDetails = POSController.tranWithDetails;
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            POSController.tranWithDetails = this.tranWithDetails;

        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (isModifyGift)
            {
                //refresh collection
                setupGiftVoucherData();
                isModifyGift = false;
            }
            if (GiftVoucherCollection != null)
            {
                var data = GiftVoucherCollection?.DataSource as GiftVoucherPayDataSource;
                if(data!= null)
                {
                    if (data.choosecell != null)
                    {

                        var frame2 = data.choosecell.Frame;
                        frame2.X = 0;
                        UIView.Animate(0.7, () =>
                        {
                            data.choosecell.showbtndelete = false;
                            data.choosecell.Frame = frame2;
                        });
                    }
                    setupGiftVoucherData();
                }
            }
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
                    var selectCustomerPage = new POSCustomerController();
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
        }
        public async override void ViewDidLoad()
        {
            try
            {


                base.ViewDidLoad();
                View.BackgroundColor = UIColor.White;

                initAttribute();
                SetupAutoLayout();
                setupGiftVoucherData();

                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget((obj, sender) => {
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                scroll.AddSubview(refreshControl);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        async void setupGiftVoucherData()
        {
            lstvouchers = new List<ChooseGiftVoucher>();
            if (await GabanaAPI.CheckNetWork())
            {
                List<ChooseGiftVoucher> gifts = new List<ChooseGiftVoucher>();
                List<ORM.Master.GiftVoucher> giftVouchers = await GabanaAPI.GetDataGiftVoucher();
                if (giftVouchers != null)
                {
                    var lst = giftVouchers.OrderBy(x => x.FmlAmount).ToList();
                    foreach (var item in lst)
                    {
                        var giftVoucher = new ChooseGiftVoucher()
                        {
                            DateCreated = item.DateCreated,
                            DateModified = item.DateModified,
                            FmlAmount = item.FmlAmount,
                            GiftVoucherCode = item.GiftVoucherCode,
                            GiftVoucherName = item.GiftVoucherName,
                            MerchantID = item.MerchantID,
                            Ordinary = item.Ordinary,
                            UserNameModified = item.UserNameModified
                        };
                        await giftVoucherManage.InsertOrReplaceGiftVoucher(giftVoucher);
                        gifts.Add(giftVoucher);
                    }
                    lstvouchers = new List<ChooseGiftVoucher>();
                    lstvouchers.AddRange(gifts);
                }
                else
                {
                    GiftVoucherCollection.Hidden = true;
                    emptyView.Hidden = false;
                    lblempty.Hidden = false;
                }
            }
            else
            {
                var lstvouchers2 = await giftVoucherManage.GetAllGiftVoucher();

                var mapperConfiguration = new MapperConfiguration(xx => xx.CreateMap<GiftVoucher, ChooseGiftVoucher>());
                var mapper = mapperConfiguration.CreateMapper();
                lstvouchers = mapper.Map<List<GiftVoucher>, List<ChooseGiftVoucher>>(lstvouchers2);
                if (lstvouchers == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Unable to load data."));
                    GiftVoucherCollection.Hidden = true;
                    emptyView.Hidden = false;
                    lblempty.Hidden = false;
                }
                else
                {
                    GiftVoucherCollection.Hidden = false;
                    emptyView.Hidden = true;
                    lblempty.Hidden = true;
                }
            }

            //listgiftvoucher = new LstItemGiftVoucher(lstvouchers);
            GiftVoucherPayDataSource GiftVoucherDataList = new GiftVoucherPayDataSource(lstvouchers);
            GiftVoucherDataList.OnCardCellDelete += GiftVoucher_OnCardCellDelete;
            GiftVoucherCollection.DataSource = GiftVoucherDataList;
            GiftVoucherCollection.ReloadData();
        }
        private async void GiftVoucher_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "Items")+" GiftVoucher Code : " + lstvouchers[(int)indexPath.Row].GiftVoucherCode + " ?", UIAlertControllerStyle.Alert);

                //Add Actions
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default, Action => delete_click((int)indexPath.Row)));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel Delete")));

                //Present Alert
                PresentViewController(okCancelAlertController, true, null);
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotdelete", "You can delete GiftVoucher data."));
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private async void delete_click(int position)
        {
            if (await GabanaAPI.CheckNetWork())
            {
                var vouchercode = lstvouchers[position].GiftVoucherCode;
                var result = await GabanaAPI.DeleteDataGiftVoucher(vouchercode);
                if (result.Status)
                {
                    Utils.ShowMessage(Utils.TextBundle("successfullydeleted", "Successfully deleted"));
                   // Utils.ShowAlert(this, "สำเร็จ !", Utils.TextBundle("successfullydeleted", "Successfully deleted"));
                    GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                    var deletelocal = await giftVoucherManage.DeleteGiftVoucher(DataCashingAll.MerchantId, vouchercode);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                    //Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("failedtodelete", "Failed to delete"));
                }
                setupGiftVoucherData();
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "No Internet ไม่สามารถแก้ไขข้อมูลได้");
                return;
            }
        }
        void initAttribute()
        {
            scroll = new UIScrollView();
            scroll.UserInteractionEnabled = true;
            scroll.ShowsVerticalScrollIndicator = true;
            scroll.ScrollEnabled = true;
            scroll.BackgroundColor = UIColor.White;
            scroll.ContentSize = new CGSize(View.Frame.Width, View.Frame.Height + 100);
            scroll.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(scroll);

            #region GiftVoucherCollection
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width)+80, height: 80);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            GiftVoucherCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            GiftVoucherCollection.BackgroundColor = UIColor.White;
            GiftVoucherCollection.ShowsVerticalScrollIndicator = false;
            GiftVoucherCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            GiftVoucherCollection.RegisterClassForCell(cellType: typeof(GiftVoucherPayViewCell), reuseIdentifier: "GiftVoucherViewCell");

            GiftVoucherPayCollectionDelegate GiftCollectionDelegate = new GiftVoucherPayCollectionDelegate();
            GiftCollectionDelegate.OnItemSelected += (indexPath) => {

                lstvouchers.ConvertAll(x => x.Choose = false);
                lstvouchers[indexPath.Row].Choose = true;
                ((GiftVoucherPayDataSource)GiftVoucherCollection.DataSource).ReloadData(lstvouchers);
                GiftVoucherCollection.ReloadData();
                GiftSelect = lstvouchers[indexPath.Row];

                //ApplyGift.SetTitle("Choose Customer", UIControlState.Normal);
                ApplyGift.Enabled = true;
                ApplyGift.SetTitleColor(UIColor.White, UIControlState.Normal);
                ApplyGift.Layer.CornerRadius = 5f;
                ApplyGift.BackgroundColor = UIColor.FromRGB(51, 172, 225);
            };
            GiftVoucherCollection.Delegate = GiftCollectionDelegate;
            scroll.AddSubview(GiftVoucherCollection);
            #endregion

            #region emptyView
            emptyView = new UIImageView();
            emptyView.Hidden = true;
            emptyView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyView.Image = UIImage.FromFile("DefaultGiftVoucher.png");
            emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
            scroll.AddSubview(emptyView);

            lblempty = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblempty.Hidden = true;
            lblempty.Lines = 2;
            lblempty.Font = lblempty.Font.WithSize(16);
            lblempty.Text = "คุณยังไม่ได้เพิ่ม GiftVoucher สามารถเพิ่ม\n ได้ที่ปุ่ม Add ด้านล่าง";
            scroll.AddSubview(lblempty);

            #endregion

            //btnAdd = new UIImageView();
            //btnAdd.Image = UIImage.FromBundle("Add");
            //btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;

            //btnAdd.UserInteractionEnabled = true;
            //var tapGesture = new UITapGestureRecognizer(this,
            //        new ObjCRuntime.Selector("ADD:"))
            //{
            //    NumberOfTapsRequired = 1 // change number as you want 
            //};
            //btnAdd.AddGestureRecognizer(tapGesture);
            //scroll.AddSubview(btnAdd);


            ApplyGift = new UIButton();
            //removeCustomer.Hidden = true;
            ApplyGift.Font = ApplyGift.Font.WithSize(16);
            ApplyGift.Layer.CornerRadius = 5f;
            ApplyGift.SetTitle(Utils.TextBundle("applygiftvoucher", "Apply Gift Voucher"), UIControlState.Normal);
            ApplyGift.BackgroundColor = UIColor.White;
            ApplyGift.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            ApplyGift.Layer.BorderWidth = 1;
            ApplyGift.ClipsToBounds = true;
            ApplyGift.Enabled = false; 
            ApplyGift.SetTitleColor(UIColor.FromRGB(74, 74, 74), UIControlState.Normal);
            ApplyGift.TranslatesAutoresizingMaskIntoConstraints = false;
            ApplyGift.TouchUpInside += (sender, e) => {
                ApplyGift.Enabled = false;

                decimal Cash = 0;

                var check = GiftSelect.FmlAmount.IndexOf('%');
                if (check == -1)
                {
                    Cash = Convert.ToDecimal(GiftSelect.FmlAmount);
                }
                else
                {
                    Cash = CalculateAmountPercent(GiftSelect.FmlAmount, tranWithDetails.tran.GrandTotal);
                }

                var tranPayment = new TranPayment()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    SysBranchID = DataCashingAll.SysBranchId,
                    TranNo = tranWithDetails.tran.TranNo,
                    PaymentNo = tranWithDetails.tranPayments.Count + 1 ,
                    PaymentType = "GV",
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
                //คำนวณยอดเงินที่ชำระ บันทึกลง tranpayment  cash (strValue) >= amount (ยอดจ่าย)


                //PaymentNo = DataCashing.PaymentNo;
                //PaymentNo++;



                //Change = CalculateAmount(Cash, amount); // Cash เงินที่จ่าย, amount(ยอดจ่าย)
                //tranPayment.PaymentNo = PaymentNo;
                tranPayment.PaymentAmount = Cash; //เงินที่จ่าย
                tranPayment.ReferenceNo1 = GiftSelect.GiftVoucherCode;
                tranPayment.Comments = GiftSelect.GiftVoucherName;
                tranWithDetails.tranPayments.Add(tranPayment);

                var Change = (double)Cash - amount;
                ChangeController ChangePage = new ChangeController();
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("change", "Change"));
                ChangePage.Setitem(Change, (double)Cash);
                //this.NavigationController.ViewControllers)
                this.NavigationController.PushViewController(ChangePage, false);
            };
            scroll.AddSubview(ApplyGift);

            decimal paymentAmount = 0;
            foreach (var item in tranWithDetails.tranPayments)
            {
                paymentAmount += item.PaymentAmount;
            }
            //amount คือ ยอดที่ต้องจ่าย      


            amount = Convert.ToDouble(tranWithDetails.tran.GrandTotal - paymentAmount);
        }
        private decimal CalculateAmountPercent(string Cash, decimal Amount)
        {
            decimal discount = 0;
            var check = Cash.IndexOf('%');
            if (check != -1)
            {
                discount = ((Convert.ToDecimal(Cash.Remove(check)) / 100) * Amount);
            }
            return discount;
        }
        [Export("ADD:")]
        public void ADD(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("addgiftvoucher", "Add Gift Voucher"));
            var add = new AddGiftVoucherController();
            this.NavigationController.PushViewController(add, false);
        }
        
        void SetupAutoLayout()
        {

            scroll.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor).Active = true;
            scroll.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            scroll.WidthAnchor.ConstraintEqualTo(View.Frame.Width).Active = true;
            scroll.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblempty.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lblempty.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lblempty.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lblempty.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            GiftVoucherCollection.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            GiftVoucherCollection.BottomAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            GiftVoucherCollection.LeftAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            GiftVoucherCollection.RightAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            ApplyGift.BottomAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            ApplyGift.LeftAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            ApplyGift.HeightAnchor.ConstraintEqualTo(45).Active = true;
            ApplyGift.RightAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            //btnAdd.BottomAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            //btnAdd.RightAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            //btnAdd.WidthAnchor.ConstraintEqualTo(45).Active = true;
            //btnAdd.HeightAnchor.ConstraintEqualTo(45).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public class ChooseGiftVoucher : ORM.MerchantDB.GiftVoucher
        {
            public bool Choose { get; set; }
        }
    }
}