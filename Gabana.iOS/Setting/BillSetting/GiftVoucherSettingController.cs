using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
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

namespace Gabana.iOS
{
    public partial class GiftVoucherSettingController : UIViewController
    {

        UIImageView emptyView;
        UIBarButtonItem backButton;
        UILabel lblempty;

        public static List<GiftVoucher> lstvouchers;
        
        UICollectionView GiftVoucherCollection = null;
        LstItemGiftVoucher listgiftvoucher;
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
        UIScrollView scroll;
        UIImageView btnAdd;
        public static bool isModifyGift = false;
        //   MerchantConfigManage configManage = new MerchantConfigManage();
        //VoucherList
        public GiftVoucherSettingController() {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (isModifyGift)
            {
                //refresh collection
                await setupGiftVoucherData();
                isModifyGift = false;
            }
            if (GiftVoucherCollection != null)
            {
                var data = GiftVoucherCollection?.DataSource as GiftVoucherDataSource;
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
                    await setupGiftVoucherData();
                }
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
                refreshControl.AddTarget(async (obj, sender) => {
                    setupGiftVoucherData();

                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                GiftVoucherCollection.AlwaysBounceVertical = true;
                GiftVoucherCollection.AddSubview(refreshControl);
                 
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        async Task setupGiftVoucherData()
        {
            lstvouchers = new List<GiftVoucher>();
            if (await GabanaAPI.CheckNetWork())
            {
                List<GiftVoucher> gifts = new List<GiftVoucher>();
                List<ORM.Master.GiftVoucher> giftVouchers = await GabanaAPI.GetDataGiftVoucher();
                if (giftVouchers?.Count>0)
                {
                    var lst = giftVouchers.OrderBy(x => x.FmlAmount).ToList();
                    foreach (var item in lst)
                    {
                        ORM.MerchantDB.GiftVoucher giftVoucher = new GiftVoucher()
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
                    lstvouchers = new List<GiftVoucher>();
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
                lstvouchers = await giftVoucherManage.GetAllGiftVoucher();
                if (lstvouchers.Count>0)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
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

            listgiftvoucher = new LstItemGiftVoucher(lstvouchers);
            GiftVoucherDataSource GiftVoucherDataList = new GiftVoucherDataSource(listgiftvoucher);
            GiftVoucherDataList.OnCardCellDelete += GiftVoucher_OnCardCellDelete;
            GiftVoucherCollection.DataSource = GiftVoucherDataList;
            GiftVoucherCollection.ReloadData();
        }
        private async void GiftVoucher_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                var okCancelAlertController = UIAlertController.Create("ต้องการลบ ?", "คุณแน่ใจหรือไม่ที่จะลบ GiftVoucher Code : " + lstvouchers[(int)indexPath.Row].GiftVoucherCode + " ?", UIAlertControllerStyle.Alert);

                //Add Actions
                okCancelAlertController.AddAction(UIAlertAction.Create("ลบ", UIAlertActionStyle.Default, Action => delete_click((int)indexPath.Row)));
                okCancelAlertController.AddAction(UIAlertAction.Create("ยกเลิก", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel Delete")));

                //Present Alert
                PresentViewController(okCancelAlertController, true, null);
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูล GiftVoucher ได้");
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
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
                   // Utils.ShowAlert(this, "สำเร็จ !", Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
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
            GiftVoucherCollection.RegisterClassForCell(cellType: typeof(GiftVoucherViewCell), reuseIdentifier: "GiftVoucherViewCell");

            GiftVoucherCollectionDelegate GiftCollectionDelegate = new GiftVoucherCollectionDelegate();
            GiftCollectionDelegate.OnItemSelected += (indexPath) => {
                if (lstvouchers.Count>0)
                {
                    Utils.SetTitle(this.NavigationController, "Edit Gift Voucher");
                    var voucher = lstvouchers[(int)indexPath.Row];
                    var edit = new AddGiftVoucherController(voucher);
                    this.NavigationController.PushViewController(edit, false);
                }
                
            };
            GiftVoucherCollection.Delegate = GiftCollectionDelegate;
            View.AddSubview(GiftVoucherCollection);
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
            lblempty.Lines = 3;
            lblempty.Font = lblempty.Font.WithSize(16);
            lblempty.Text = Utils.TextBundle("nullgiftvoucher", "");
            scroll.AddSubview(lblempty);

            #endregion

            btnAdd = new UIImageView();
            btnAdd.Image = UIImage.FromBundle("Add");
            btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;

            btnAdd.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("ADD:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAdd.AddGestureRecognizer(tapGesture);
            View.AddSubview(btnAdd);

        }
        [Export("ADD:")]
        public void ADD(UIGestureRecognizer sender)
        {
            
            Utils.SetTitle(this.NavigationController, "Add Gift Voucher");
            var add = new AddGiftVoucherController();
            this.NavigationController.PushViewController(add, false);
        }
        void SetupAutoLayout()
        {

            //scroll.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor).Active = true;
            scroll.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            scroll.WidthAnchor.ConstraintEqualTo(View.Frame.Width).Active = true;
            scroll.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 58).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblempty.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lblempty.HeightAnchor.ConstraintEqualTo(70).Active = true;
            lblempty.WidthAnchor.ConstraintEqualTo(300).Active = true;
            lblempty.CenterXAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            //lblempty.BackgroundColor = UIColor.Red;
            #endregion

            GiftVoucherCollection.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            GiftVoucherCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            GiftVoucherCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            GiftVoucherCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAdd.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            btnAdd.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            btnAdd.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnAdd.HeightAnchor.ConstraintEqualTo(45).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}