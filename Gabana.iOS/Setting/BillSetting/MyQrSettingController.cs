using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class MyQrSettingController : UIViewController
    {
        UICollectionView QRcollectionview;
        UIImageView EmptyQrView;
        UILabel lbl_empty_QR;
        UIPageControl uIPageControl;
        ListMyQRCodeIOS ListmyQRCode;
        MyQrCodeManage QrCodeManager = new MyQrCodeManage();
        List<MyQrCode> lstQrCodes = new List<MyQrCode>();
        public static bool isModifyQR = false;


        UIImageView btnAddQr;

       UIBarButtonItem backButton;

        public MyQrSettingController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                this.NavigationController.SetNavigationBarHidden(false, false);
                if (isModifyQR)
                {
                    GabanaLoading.SharedInstance.Show(this);
                    await SetDatamyQR();
                    isModifyQR = false;
                    GabanaLoading.SharedInstance.Hide();
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
            
        }
        public override async void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                this.NavigationController.SetNavigationBarHidden(false, false);
                View.BackgroundColor = UIColor.FromRGB(255, 255, 255);


                initAttribute();
                setupAutoLayout();
                await SetDatamyQR();

                

                showList();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
                return;
            }
            
        }
        async Task SetDatamyQR()
        {
            try
            {
                lstQrCodes = new List<MyQrCode>();
                if (await GabanaAPI.CheckNetWork())
                {
                    List<MyQrCode> qrcodes = new List<MyQrCode>();
                    List<ORM.Master.MyQrCode> myqrcodes = await GabanaAPI.GetDataMyQrCode();
                    if (myqrcodes != null)
                    {
                        var lst = myqrcodes.OrderBy(x => x.MyQrCodeNo).ToList();
                        foreach (var item in lst)
                        {
                            ORM.MerchantDB.MyQrCode myQrCode = new ORM.MerchantDB.MyQrCode()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified,
                                Comments = item.Comments,
                                FMyQrAllBranch = item.FMyQrAllBranch,
                                MyQrCodeName = item.MyQrCodeName,
                                MyQrCodeNo = item.MyQrCodeNo,
                                PicturePath = item.PicturePath,
                                SysBranchID = item.SysBranchID  // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : DataCashingAll.SysBranchId 
                            };
                            await QrCodeManager.InsertOrReplaceMyQrCode(myQrCode);
                            await Utils.InsertLocalPictureQrcode(myQrCode);
                            qrcodes.Add(myQrCode);
                        }
                        lstQrCodes = new List<MyQrCode>();
                        lstQrCodes.AddRange(qrcodes);
                    }
                }
                else
                {
                    lstQrCodes = await QrCodeManager.GetAllMyQrCode(DataCashingAll.MerchantId);
                    if (lstQrCodes == null)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                    }
                }
                List<MyQrCode> lisQRAllBranch = new List<MyQrCode>();
                List<MyQrCode> lisQRThisBranch = new List<MyQrCode>();
                List<MyQrCode> lisQRAnotherBranch = new List<MyQrCode>();

                lisQRAllBranch = lstQrCodes.Where(x => x.FMyQrAllBranch == 'A').ToList();
                lisQRAllBranch = lisQRAllBranch.OrderBy(x => x.MyQrCodeNo).ToList();
                lisQRThisBranch = lstQrCodes.Where(x => x.SysBranchID == DataCashingAll.SysBranchId).ToList();
                lisQRThisBranch = lisQRThisBranch.OrderBy(x => x.MyQrCodeNo).ToList();
                lisQRAnotherBranch = lstQrCodes.Where(x => x.SysBranchID != DataCashingAll.SysBranchId && x.FMyQrAllBranch != 'A').ToList();
                lisQRAnotherBranch = lisQRAnotherBranch.OrderBy(x => x.MyQrCodeNo).ToList();

                lstQrCodes = new List<MyQrCode>();
                lstQrCodes.AddRange(lisQRThisBranch);
                lstQrCodes.AddRange(lisQRAllBranch);
                lstQrCodes.AddRange(lisQRAnotherBranch);
                uIPageControl.Pages = lstQrCodes.Count;
                ListmyQRCode = new ListMyQRCodeIOS(lstQrCodes);
                MYQRDataSourceList QRDataList = new MYQRDataSourceList(ListmyQRCode);
                QRcollectionview.DataSource = QRDataList;
                QRcollectionview.ReloadData();
                showList();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), ex.Message);
                return;
            }
        }
        void showList()
        {
           if (lstQrCodes == null || lstQrCodes.Count == 0)
            {
                QRcollectionview.Hidden = true;
                EmptyQrView.Hidden = false;
                lbl_empty_QR.Hidden = false;
            }
            else
            {
                QRcollectionview.Hidden = false;
                EmptyQrView.Hidden = true;
                lbl_empty_QR.Hidden = true;
            }
        }
        void initAttribute()
        {
            #region QRcollectionview
            UICollectionViewFlowLayout QRLayoutList = new UICollectionViewFlowLayout();
            QRLayoutList.ItemSize = new CoreGraphics.CGSize(width: (int)View.Frame.Width - 10, height: (int)View.Frame.Height-150 );
            QRLayoutList.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            QRLayoutList.MinimumLineSpacing = 10;
            QRLayoutList.MinimumInteritemSpacing = 5;


            QRcollectionview = new UICollectionView(frame: View.Frame, layout: QRLayoutList);
            QRcollectionview.BackgroundColor = UIColor.White;
            QRcollectionview.PagingEnabled = true;
            QRcollectionview.Scrolled += QRcollectionview_Scrolled;
            QRcollectionview.AlwaysBounceHorizontal = true;
            QRcollectionview.AutomaticallyAdjustsScrollIndicatorInsets = true;
            QRcollectionview.ShowsVerticalScrollIndicator = false;
            QRcollectionview.ShowsHorizontalScrollIndicator = false;
            QRcollectionview.TranslatesAutoresizingMaskIntoConstraints = false;
            QRcollectionview.RegisterClassForCell(cellType: typeof(MYQRCollectionViewCellList), reuseIdentifier: "MYQRCollectionViewCellList");

           
            QRSettingCollectionDelegate QRCollectionDelegate = new QRSettingCollectionDelegate(); 
            QRCollectionDelegate.OnItemSelected += async (indexPath) => {
                
                Utils.SetTitle(this.NavigationController, "Edit myQR");
                var qrCode = lstQrCodes[(int)indexPath.Row];
                AddMyQrController additem = new AddMyQrController(qrCode);
                this.NavigationController.PushViewController(additem, false);
            };
            QRcollectionview.Delegate = QRCollectionDelegate;

            View.AddSubview(QRcollectionview);

            uIPageControl = new UIPageControl();
            //uIPageControl.SizeForNumberOfPages(10);
            
            uIPageControl.ContentMode = UIViewContentMode.Center;
            uIPageControl.CurrentPage = 0;
            uIPageControl.BackgroundStyle = UIPageControlBackgroundStyle.Minimal;
            //uIPageControl.TintColor = UIColor.Red;
            uIPageControl.PageIndicatorTintColor = UIColor.Gray;
            uIPageControl.CurrentPageIndicatorTintColor = UIColor.Black;
            uIPageControl.TranslatesAutoresizingMaskIntoConstraints = false;
            //uIPageControl.ScrollEnabled = true;
            //uIPageControl.PagingEnabled = true;
            //uIPageControl.BackgroundColor = UIColor.Red;
            View.AddSubview(uIPageControl);

            #endregion

            #region EmptyQrView
            EmptyQrView = new UIImageView();
            EmptyQrView.Hidden = true;
            EmptyQrView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            EmptyQrView.Image = UIImage.FromFile("DefaultMyQR.png");
            EmptyQrView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(EmptyQrView);

            lbl_empty_QR = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_empty_QR.Hidden = true;
            lbl_empty_QR.Lines = 2;
            lbl_empty_QR.Font = lbl_empty_QR.Font.WithSize(16);
            lbl_empty_QR.Text = "คุณยังไม่ได้ตั้งค่า myQR \nสามารถเพิ่มได้ที่ ปุ่ม Add ด้านล่าง";
            View.AddSubview(lbl_empty_QR);
            #endregion

            btnAddQr = new UIImageView();
            btnAddQr.Image = UIImage.FromBundle("Add");
            btnAddQr.TranslatesAutoresizingMaskIntoConstraints = false;

            btnAddQr.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("AddQR:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAddQr.AddGestureRecognizer(tapGesture2);

            View.AddSubview(btnAddQr);
        }
        private void QRcollectionview_Scrolled(object sender, EventArgs e)
        {
            var x = QRcollectionview.IndexPathsForVisibleItems;
            uIPageControl.CurrentPage = x.FirstOrDefault().Row;
        }

        [Export("AddQR:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            btnAddQr.UserInteractionEnabled = false;
            Utils.SetTitle(this.NavigationController, "Setting myQR");
            AddMyQrController additem = new AddMyQrController();
            this.NavigationController.PushViewController(additem, false);
            btnAddQr.UserInteractionEnabled = true;
        }
        void setupAutoLayout()
        {
            uIPageControl.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            uIPageControl.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            uIPageControl.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            //uIPageControl.WidthAnchor.ConstraintEqualTo(300).Active = true;
            uIPageControl.HeightAnchor.ConstraintEqualTo(30).Active = true;
            #region QRcollectionview
            QRcollectionview.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            //QRcollectionview.HeightAnchor.ConstraintEqualTo(595).Active = true;
            QRcollectionview.BottomAnchor.ConstraintEqualTo(uIPageControl.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            QRcollectionview.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            QRcollectionview.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            
            #endregion

            #region EmptyQrView
            EmptyQrView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 38).Active = true;
            EmptyQrView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            EmptyQrView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            EmptyQrView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_QR.TopAnchor.ConstraintEqualTo(EmptyQrView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_QR.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_QR.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_QR.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            btnAddQr.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            btnAddQr.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            btnAddQr.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnAddQr.HeightAnchor.ConstraintEqualTo(45).Active = true;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}