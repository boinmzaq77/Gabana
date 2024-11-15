using CoreGraphics;
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
    public partial class MyQrController : UIViewController
    {
        UICollectionView QRcollectionview;
        UIImageView EmptyQrView;
        UILabel lbl_empty_QR;
        MYQRDataSourceList QRDataList;
        UIPageControl uIPageControl; 
        UIScrollView scroll;
        UIButton ApplyQr;
        bool showbtn;
        
        MyQrCodeManage QrCodeManager = new MyQrCodeManage();
       // ListMyQRCode ListMyQRCode;
        List<MyQrCode> lstQrCodes;


        public MyQrController(bool showbtn)
        {
            this.showbtn = showbtn;
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("myqr", "MyQR"));
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(248, 248, 248);
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                var pinCodePage = new PinCodeController("Pincode");
                pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                await this.PresentViewControllerAsync(pinCodePage, false);
            }
        }
        public override async void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);

                await SetDatamyQR();
                initAttribute();
                setupAutoLayout();
                showList();

                

                 var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget((obj, sender) => {
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                scroll.AddSubview(refreshControl);


                if (showbtn && lstQrCodes.Count != 0 )
                {
                    ApplyQr = new UIButton();
                    //removeCustomer.Hidden = true;
                    ApplyQr.Font = ApplyQr.Font.WithSize(16);
                    ApplyQr.Layer.CornerRadius = 5f;
                    ApplyQr.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
                    ApplyQr.SetTitleColor(UIColor.White, UIControlState.Normal);
                    ApplyQr.Layer.CornerRadius = 5f;
                    ApplyQr.BackgroundColor = UIColor.FromRGB(51, 172, 225);
                    //ApplyQr.BackgroundColor = UIColor.White;
                    //AplpyQr.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
                    //ApplyQr.Layer.BorderWidth = 1;
                    ApplyQr.ClipsToBounds = true;
                    //ApplyQr.SetTitleColor(UIColor.FromRGB(74, 74, 74), UIControlState.Normal);
                    ApplyQr.TranslatesAutoresizingMaskIntoConstraints = false;
                    ApplyQr.TouchUpInside += (sender, e) =>
                    {
                        MyQrPayController myQrPayController = new MyQrPayController("myqr");
                        this.NavigationController.PushViewController(myQrPayController, false);
                        //var x = QRcollectionview.IndexPathsForVisibleItems;
                        
                        //MYQRCollectionViewCellList cell = QRcollectionview.CellForItem(x.FirstOrDefault()) as MYQRCollectionViewCellList;
                        //if (cell != null)
                        //{
                        //    MyQrPayController myQrPayController = new MyQrPayController();
                        //    this.NavigationController.PushViewController(myQrPayController, false);
                        //}
                    };
                    scroll.AddSubview(ApplyQr);

                    ApplyQr.BottomAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
                    ApplyQr.LeftAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
                    ApplyQr.HeightAnchor.ConstraintEqualTo(45).Active = true;
                    ApplyQr.RightAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

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
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
            
        }
        public class ListMyQRCode
        {
            public List<MyQrCode> myQrCodes;
            static List<MyQrCode> builitem;
            public ListMyQRCode(List<MyQrCode> myQrCodes)
            {
                builitem = myQrCodes;
                this.myQrCodes = builitem;
            }
            public int Count
            {
                get
                {
                    return myQrCodes == null ? 0 : myQrCodes.Count;
                }
            }
            public MyQrCode this[int i]
            {
                get { return myQrCodes == null ? null : myQrCodes[i]; }
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
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "All Branch"));
                    }
                }
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
            var listMyQr = new ListMyQRCodeIOS(lstQrCodes);
            QRDataList = new MYQRDataSourceList(listMyQr);
            QRcollectionview.DataSource = QRDataList;
            QRcollectionview.ReloadData();

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
            scroll = new UIScrollView();
            scroll.UserInteractionEnabled = true;
            scroll.ShowsVerticalScrollIndicator = true;
            scroll.ScrollEnabled = true;
            scroll.BackgroundColor = UIColor.White;
            scroll.ContentSize = new CGSize(View.Frame.Width, View.Frame.Height + 100);
            scroll.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(scroll);

            #region QRcollectionview
            UICollectionViewFlowLayout QRLayoutList = new UICollectionViewFlowLayout();
            QRLayoutList.ItemSize = new CoreGraphics.CGSize(width: (int)View.Frame.Width-10  , height: (int)View.Frame.Height - 150);
            QRLayoutList.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            QRLayoutList.MinimumLineSpacing = 10;
            QRLayoutList.MinimumInteritemSpacing = 10;
            
            //QRLayoutList.SectionInset = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);


            QRcollectionview = new UICollectionView(frame: View.Frame, layout: QRLayoutList);
            QRcollectionview.BackgroundColor = UIColor.White;
            //QRcollectionview.AllowsFocus = true;
            //QRcollectionview.AllowsFocusDuringEditing = true;
            QRcollectionview.PagingEnabled = true;
            QRcollectionview.Scrolled += QRcollectionview_Scrolled;
            QRcollectionview.AlwaysBounceHorizontal =      true;
            QRcollectionview.AutomaticallyAdjustsScrollIndicatorInsets = true;
            QRcollectionview.ShowsVerticalScrollIndicator = false;
            QRcollectionview.ShowsHorizontalScrollIndicator = false;
            QRcollectionview.TranslatesAutoresizingMaskIntoConstraints = false;
            QRcollectionview.RegisterClassForCell(cellType: typeof(MYQRCollectionViewCellList), reuseIdentifier: "MYQRCollectionViewCellList");
            scroll.AddSubview(QRcollectionview);


            uIPageControl = new UIPageControl();
            //uIPageControl.SizeForNumberOfPages(10);
            uIPageControl.Pages = lstQrCodes.Count;
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
            scroll.AddSubview(uIPageControl);


            #endregion

            #region EmptyQrView
            EmptyQrView = new UIImageView();
            EmptyQrView.Hidden = true;
            EmptyQrView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            EmptyQrView.Image = UIImage.FromFile("DefaultMyQR.png");
            EmptyQrView.TranslatesAutoresizingMaskIntoConstraints = false;
            scroll.AddSubview(EmptyQrView);

            lbl_empty_QR = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_empty_QR.Hidden = true;
            lbl_empty_QR.Font = lbl_empty_QR.Font.WithSize(16);
            lbl_empty_QR.Text = Utils.TextBundle("notsetqr", "All Branch");
            scroll.AddSubview(lbl_empty_QR);
            #endregion
        }

        private void QRcollectionview_Scrolled(object sender, EventArgs e)
        {
            var x = QRcollectionview.IndexPathsForVisibleItems;
            uIPageControl.CurrentPage = x.FirstOrDefault().Row;
        }

        private void QRcollectionview_ScrollAnimationEnded(object sender, EventArgs e)
        {
            
        }

        void setupAutoLayout()
        {
            scroll.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor).Active = true;
            scroll.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            scroll.WidthAnchor.ConstraintEqualTo(View.Frame.Width).Active = true;
            scroll.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            //uIPageControl.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            uIPageControl.BottomAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            uIPageControl.LeftAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            uIPageControl.RightAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            //uIPageControl.WidthAnchor.ConstraintEqualTo(300).Active = true;
            uIPageControl.HeightAnchor.ConstraintEqualTo(30).Active = true;
            //uIPageControl.BackgroundColor = UIColor.Red;
            #region QRcollectionview
            QRcollectionview.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            QRcollectionview.BottomAnchor.ConstraintEqualTo(uIPageControl.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            QRcollectionview.LeftAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            QRcollectionview.RightAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            
            #endregion

            #region EmptyQrView
            EmptyQrView.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 38).Active = true;
            EmptyQrView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            EmptyQrView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            EmptyQrView.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_QR.TopAnchor.ConstraintEqualTo(EmptyQrView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_QR.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_QR.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_QR.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}