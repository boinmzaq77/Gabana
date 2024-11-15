using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.Master;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class BillHistoryController : UIViewController
    {
        ListBillHistory Listbill;
        UICollectionView BillHistoryCollection=null;
        CultureInfo cultureUS = new CultureInfo("en-US");
        public static List<TransHistoryNew> lsttransHistory = new List<TransHistoryNew>();
        TranWithDetailsLocal tranWithDetails = new TranWithDetailsLocal();
        List<TranWithDetailsLocal> lsttranWithDetails = new List<TranWithDetailsLocal>();
        TransManage transManage;
        TranDetailItemManage tranDetailItemManage;
        TranPaymentManage tranPaymentManage;
        DateTime latestTranDate;
        UIView SearchBarView;
        UILabel lblempty;
        UIImageView  emptyView;
        List<TransHistory> TransHistories = new List<TransHistory>() ; 
        TranTradDiscountManage tranTradDiscountManage;
        ReceiptHistoryDetailController HistoryPage = null;
        UIButton btnSearch;
        UITextField txtSearch;
        TransHistoryNew transHistory;

        int offset;
        string textSearchBill;
        int position;
        int Last;
        UIBarButtonItem selectBranch;
        bool islast;
        public static Gabana.ORM.MerchantDB.Branch BranchSelect = null;
        public static bool isModifyBranch = false;
        BranchManage setBranch = new BranchManage();
        DashBoardBranchController DashBranchPage = null;
        private UIButton btnfill;
        internal bool FormMain;
        public BillHistoryController()
        {
        }

        public async override void ViewDidLoad()
        {
            try
            {
                DataCaching.BillHistoryNavigation = this.NavigationController;
                base.ViewDidLoad();
                latestTranDate = new DateTime();
                BranchSelect = await setBranch.GetBranch((int)MainController.merchantlocal.MerchantID, Convert.ToInt32(Preferences.Get("Branch", "")));
                selectBranch = new UIBarButtonItem();
                if (BranchSelect.BranchName.Length >= 15)
                {
                    selectBranch.Title = BranchSelect.BranchName.Substring(0, 15);
                }
                else
                {
                    selectBranch.Title = BranchSelect.BranchName;
                }
                selectBranch.TintColor = UIColor.FromRGB(0, 149, 218);

                selectBranch.Clicked += (sender, e) =>
                {
                    Utils.SetTitle(this.NavigationController, "Choose Branch");
                    if (DashBranchPage == null)
                    {
                        //Utils.SetTitle(this.NavigationController, "Choose Branch");
                        DashBranchPage = new DashBoardBranchController("billhistory");
                    }
                    this.NavigationController.PushViewController(DashBranchPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectBranch;
                View.BackgroundColor = UIColor.White;

                transManage = new TransManage();
                transHistory = new TransHistoryNew();
                lsttransHistory = new List<TransHistoryNew>();

                
                
                offset = 0;
                position = 0;

                initAttribute();
                setupAutoLayout();
                //setDataBill();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
          
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            GabanaLoading.SharedInstance.Show(this);
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("billhis", "BillHistory"));
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                var pinCodePage = new PinCodeController("Pincode");
                pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                await this.PresentViewControllerAsync(pinCodePage, false);
            }
            if (isModifyBranch)
            {
                BranchSelect = await setBranch.GetBranch((int)MainController.merchantlocal.MerchantID, Convert.ToInt32(BranchSelect.SysBranchID));
                if (BranchSelect.BranchName.Length >= 15)
                {
                    selectBranch.Title = BranchSelect.BranchName.Substring(0, 15);
                }
                else
                {
                    selectBranch.Title = BranchSelect.BranchName;
                }
                
                isModifyBranch = false;
            }
            latestTranDate = DateTime.UtcNow;
            //if (FormMain)
            //{
            //    txtSearch.Text = "";
            //    textSearchBill = "";
            //    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            //    FormMain =false;
            //}
                await ShowDetail();
            GabanaLoading.SharedInstance.Hide();
        }
        async void initAttribute()
        {
            #region SearchBarView
            SearchBarView = new UIView();
            SearchBarView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            SearchBarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchBarView);

            bool clearSearch = false;
            txtSearch = new UITextField
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtSearch.EditingChanged += (object sender, EventArgs e) =>
            {
                View.BackgroundColor = UIColor.White;
                if (txtSearch.Text.Length == 0)
                {
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    clearSearch = false;
                }
                else
                {
                    btnSearch.SetImage(UIImage.FromFile("DelTxt.png"), UIControlState.Normal);
                    clearSearch = true;
                }
            };
            txtSearch.BackgroundColor = UIColor.Clear;
            txtSearch.Font = txtSearch.Font.WithSize(15);
            txtSearch.ReturnKeyType = UIReturnKeyType.Done;
            txtSearch.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                SearchBytxt();
                return true;
            };
            txtSearch.Placeholder = Utils.TextBundle("billhisplace", "");
            SearchBarView.AddSubview(txtSearch);

            btnSearch = new UIButton();
            btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside += (sender, e) =>
            {
                if (clearSearch == false)
                {
                    txtSearch.BecomeFirstResponder();
                    btnSearch.SetImage(UIImage.FromFile("DelTxt.png"), UIControlState.Normal);
                }
                else
                {
                    txtSearch.Text = "";
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                }
                SearchBytxt();

            };
            SearchBarView.AddSubview(btnSearch);

            btnfill = new UIButton();
            btnfill.Enabled = false;
            btnfill.Hidden = true;
            btnfill.SetImage(UIImage.FromBundle("ReportFilter"), UIControlState.Normal);
            btnfill.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfill.TouchUpInside += (sender, e) =>
            {
                FilterbillController filterbill = new FilterbillController();
                this.NavigationController.PushViewController(filterbill, false);

            };
            SearchBarView.AddSubview(btnfill);
            #endregion scroll = new UIScrollView();

            #region emptyView
            emptyView = new UIImageView();
            emptyView.Hidden = true;
            emptyView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyView.Image = UIImage.FromBundle("DefaultBillHistory");
            emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(emptyView);

            lblempty = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblempty.Hidden = true;
            lblempty.Lines = 2;
            lblempty.Font = lblempty.Font.WithSize(16);
            lblempty.Text = "คุณยังไม่มี Bill History ";
            View.AddSubview(lblempty);
            #endregion

            #region BillHistoryCollection
            //UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            //itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 70);
            //itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            UICollectionViewFlowLayout itemflowLayout = new UICollectionViewFlowLayout();
            //itemflowLayout.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 0);
            itemflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            //itemflowLayout.MinimumLineSpacing = 1f;
            //itemflowLayout.MinimumInteritemSpacing = 1f;
            itemflowLayout.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;


            BillHistoryCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayout);
            BillHistoryCollection.BackgroundColor = UIColor.White;
            BillHistoryCollection.AlwaysBounceVertical = true;
            BillHistoryCollection.ShowsVerticalScrollIndicator = false;
            BillHistoryCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            BillHistoryCollection.RegisterClassForCell(cellType: typeof(BillhistoryCollectionViewCell), reuseIdentifier: "BillhistoryCollectionViewCell");
            BillHistoryCollection.RegisterClassForCell(cellType: typeof(BillListViewCell), reuseIdentifier: "BillListViewCell");
            BillHistoryCollectionDelegate BillCollectionDelegate = new BillHistoryCollectionDelegate();
            BillCollectionDelegate.OnItemSelected += (indexPath) =>
            {

                // do somthing
                //if (HistoryPage == null)
                //{

                Utils.SetTitle(this.NavigationController, Listbill.Trans[indexPath.Row].tranNo);
                HistoryPage = new ReceiptHistoryDetailController(Listbill.Trans[indexPath.Row]);
                //}
                this.NavigationController.PushViewController(HistoryPage, false);

            };
            
            BillHistoryCollection.Delegate = BillCollectionDelegate;
            
            View.AddSubview(BillHistoryCollection);
            var refreshControl = new UIRefreshControl();
            refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
            refreshControl.AddTarget(async (obj, sender) => {
                //setDataBill();
                await ShowDetail();
                refreshControl.EndRefreshing();
            }, UIControlEvent.ValueChanged);
            BillHistoryCollection.AddSubview(refreshControl);
            #endregion
        }

        

        async Task ShowDetail()
        {
            
            var lsttransHistoryNew = new List<TransHistoryNew>();
            try
            {
                if (await GabanaAPI.CheckNetWork())
                {
                    //Network Connect
                    //Search เมื่อ TranNo = ค่าว่าง
                    if (string.IsNullOrEmpty(textSearchBill))
                    {
                        lsttransHistoryNew = await GetOnlineHistory();
                        Last = lsttransHistory.Count;
                        Listbill = new ListBillHistory(lsttransHistoryNew);
                    }
                    else
                    {
                        transHistory = await GetOnlineSearch();
                        if (transHistory!=null)
                        {
                            lsttransHistoryNew.Add(transHistory);
                        }
                        
                        Listbill = new ListBillHistory(lsttransHistoryNew);
                    }

                    BillHistoryCollection.DataSource = new BillDateDataSource(Listbill,this);
                    ((BillDateDataSource)BillHistoryCollection.DataSource).ReloadData(Listbill);
                    BillHistoryCollection.ReloadData();

                    if (Listbill.Count == 0)
                    {
                        BillHistoryCollection.Hidden = true;
                        emptyView.Hidden = false;
                        lblempty.Hidden = false;
                    }
                    else
                    {
                        BillHistoryCollection.Hidden = false;
                        emptyView.Hidden = true;
                        lblempty.Hidden = true;
                    }
                }
                else
                {
                    //Not Connect
                    //Search เมื่อ TranNo = ค่าว่าง
                    if (string.IsNullOrEmpty(textSearchBill))
                    {
                        lsttransHistory = await GetOfflineHistory();

                        if (lsttransHistory == null)
                        {
                            Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                            BillHistoryCollection.Hidden = true;
                        }
                        else
                        {
                            BillHistoryCollection.Hidden = false;
                        }
                        Listbill = new ListBillHistory(lsttransHistory);
                    }
                    else
                    {
                        transHistory = await GetOfflineSearch();

                        if (transHistory == null)
                        {
                            Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                            BillHistoryCollection.Hidden = true;
                        }
                        else
                        {
                            BillHistoryCollection.Hidden = false;
                        }

                        lsttransHistoryNew.Add(transHistory);
                        Listbill = new ListBillHistory(lsttransHistoryNew);
                    }

                    BillHistoryCollection.DataSource = new BillDateDataSource(Listbill, this);
                    ((BillDateDataSource)BillHistoryCollection.DataSource).ReloadData(Listbill);
                    BillHistoryCollection.ReloadData();

                    if (Listbill.Count == 0)
                    {
                        BillHistoryCollection.Hidden = true;
                        emptyView.Hidden = false;
                        lblempty.Hidden = false;
                    }
                    else
                    {
                        BillHistoryCollection.Hidden = false;
                        emptyView.Hidden = true;
                        lblempty.Hidden = true;
                    }

                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        async void SearchBytxt()
        {
          
                textSearchBill = txtSearch.Text;
                await ShowDetail();
                //await SearchEmpty();
            
        }
        
        async Task<List<TransHistoryNew>> GetOfflineHistory()
        {
            try
            {
                List<TransHistoryNew> histories = new List<TransHistoryNew>();
                transManage = new TransManage();
                var histories2 = await transManage.GetTranHistoryNew(DataCashingAll.MerchantId, Convert.ToInt32(BranchSelect.BranchID));
                histories = await transManage.GetTranHistory(DataCashingAll.MerchantId, Int32.Parse(BranchSelect.BranchID));
                return histories;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return new List<TransHistoryNew>();
            }
        }
        async Task<List<TransHistoryNew>> GetOnlineHistory()
        {
            try
            {
                TransHistoryNew transHistory;
                var date = Utils.ChangeDateTime(latestTranDate);
                int row = offset;
                if (offset == 0 )
                {
                    lsttransHistory = new List<TransHistoryNew>();
                }
               
                //var tranConvert = await GabanaAPI.GetDataTranHistory(DataCashingAll.SysBranchId, offset, date);
                var tranConvert = await GabanaAPI.GetDataTranHistory(Int32.Parse( BranchSelect.BranchID), offset, date);

                if (tranConvert == null)
                {
                    return new List<TransHistoryNew>();
                }

                foreach (var gettransHistory in tranConvert)
                {
                    var count = lsttransHistory.Where(x => x.fhead == true && x.tranDate.Date == gettransHistory.tranDate.Date).Count();
                    if (count==0)
                    {
                        transHistory = new TransHistoryNew()
                        {
                            tranNo = gettransHistory.tranNo,
                            tranDate = gettransHistory.tranDate.Date,
                            customerName = gettransHistory.customerName,
                            grandTotal = gettransHistory.grandTotal,
                            fCancel = gettransHistory.fCancel,
                            paymentType = gettransHistory.paymentType,
                            fhead = true
                        };
                        lsttransHistory.Add(transHistory);
                    }
                    transHistory = new TransHistoryNew()
                    {
                        tranNo = gettransHistory.tranNo,
                        tranDate = gettransHistory.tranDate,
                        customerName = gettransHistory.customerName,
                        grandTotal = gettransHistory.grandTotal,
                        fCancel = gettransHistory.fCancel,
                        paymentType = gettransHistory.paymentType,
                        fhead = false
                    };
                    lsttransHistory.Add(transHistory);
                }
                return lsttransHistory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return new List<TransHistoryNew>();
            }
        }
        async Task<TransHistoryNew> GetOfflineSearch()
        {
            try
            {
                transManage = new TransManage();

                if (lsttransHistory.Count > 0)
                {
                    transHistory = lsttransHistory.Where(x => x.tranNo.ToString() == textSearchBill).FirstOrDefault();
                    return transHistory;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return new TransHistoryNew();
            }
        }

        async Task<TransHistoryNew> GetOnlineSearch()
        {
            try
            {
                string tranNo = textSearchBill;
                Gabana3.JAM.Trans.TranWithDetails tranConvert;

                tranConvert = await GabanaAPI.GetDataTranSearch(Int32.Parse( BranchSelect.BranchID), tranNo);

                if (tranConvert == null)
                {
                    return null;
                }

                var config = new MapperConfiguration(cfg =>
                {
                    //struct ของ table
                    cfg.CreateMap<Gabana3.JAM.Trans.TranWithDetails, Model.TranWithDetailsLocal>();
                    cfg.CreateMap<ORM.Period.Tran, Tran>();
                });

                var Imapper = config.CreateMapper();
                //TranWithDetails
                tranWithDetails.tran = Imapper.Map<ORM.Period.Tran, Tran>(tranConvert.tran);

                transHistory = new TransHistoryNew()
                {
                    tranNo = tranWithDetails.tran.TranNo,
                    tranDate = tranWithDetails.tran.TranDate,
                    customerName = tranWithDetails.tran.CustomerName,
                    grandTotal = tranWithDetails.tran.GrandTotal
                };

                return transHistory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return new TransHistoryNew();
            }
        }


        void setupAutoLayout()
        {
            SearchBarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchBarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchBarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchBarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.BottomAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            btnfill.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            btnfill.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnfill.RightAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnfill.BottomAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;



            BillHistoryCollection.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BillHistoryCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BillHistoryCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            BillHistoryCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, 58).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblempty.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lblempty.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lblempty.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lblempty.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        public class ListBillHistory
        {
            public List<TransHistoryNew> Trans;
            static List<TransHistoryNew> builitem;
            public ListBillHistory(List<TransHistoryNew> transHistories)
            {
                builitem = transHistories;
                this.Trans = builitem;
            }

            public int Count
            {
                get
                {
                    return Trans == null ? 0 : Trans.Count;
                }
            }

            public TransHistoryNew this[int i]
            {
                get { return Trans == null ? null : Trans[i]; }
            }
        }
    }
}