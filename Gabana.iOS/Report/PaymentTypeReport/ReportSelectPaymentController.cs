using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{

    public partial class ReportSelectPaymentController : UIViewController
    {
        UICollectionView PaymentCollectionView;

        UIView bottomView;
        UIButton btnSelect;
        TranPaymentManage setPayment = new TranPaymentManage();

        private static ListPaymentType listPayment;
        List<PaymentType> lstPayment = new List<PaymentType>();
        public static List<PaymentType> listChoosePayment = new List<PaymentType>();
        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch, btnAll;
        private string paymentSelect;


        int catcount = 0;


        public ReportSelectPaymentController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("choosebranch", "Items"));
        }
        public async override void ViewDidLoad()
        {
            // this.NavigationController.NavigationBar.TopItem.Title = "Choose Branch";
            try
            {
                View.BackgroundColor = UIColor.White;

                base.ViewDidLoad();

                initAttribute();
                SetupAutoLayout();
                SetPaymentData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        async void SetPaymentData()
        {
            lstPayment = await SetListPament();
            listPayment = new ListPaymentType(lstPayment);

            listChoosePayment = PaymentTypeReportController.listChoosePayment;

            if (PaymentTypeReportController.listChoosePayment.Count == 5)
            {
                paymentSelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                paymentSelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.White;
            }

            //paymentSelect = "All";
            //btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
            //btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);

            PaymentReportSelectDataSource report_adapter_Payment = new PaymentReportSelectDataSource(listPayment);
            PaymentCollectionView.DataSource = report_adapter_Payment;
            PaymentCollectionView.ReloadData();

        }
        private async Task<List<PaymentType>> SetListPament()
        {
            List<Gabana.Model.PaymentType> listPayments = new List<Gabana.Model.PaymentType>
            { //1=PaymentCash.png,2=PaymentCredit.png,3=PaymentGiftVoucher.png,4=PaymentQr.png
                new Model.PaymentType(){ Type ="CH" ,Detail = "Cash" ,Logo = 1},
                new Model.PaymentType(){ Type ="Dr" ,Detail = "Debit Card", Logo = 2},
                new Model.PaymentType(){ Type ="Cr" ,Detail = "Credit Card", Logo = 2},
                new Model.PaymentType(){ Type ="GV" ,Detail = "Gift Voucher" , Logo = 3},
                new Model.PaymentType(){ Type ="MYQR",Detail = "myQR", Logo = 4},
            };

            return listPayments;
        }

        void initAttribute()
        {
            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.White;
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchbarView);

            txtSearch = new UITextField
            {
                Placeholder = "",
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtSearch.BackgroundColor = UIColor.Clear;
            txtSearch.Hidden = true;
            txtSearch.Font = txtSearch.Font.WithSize(15);
            txtSearch.ReturnKeyType = UIReturnKeyType.Done;
            txtSearch.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
             //   SearchBytxt();
                return true;
            };
            SearchbarView.AddSubview(txtSearch);

            btnSearch = new UIButton();
            btnSearch.Hidden = true;
            btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside += (sender, e) =>
            {
                txtSearch.BecomeFirstResponder();
            };
            SearchbarView.AddSubview(btnSearch);

            btnAll = new UIButton();
            btnAll.SetTitle(Utils.TextBundle("all", "Items"), UIControlState.Normal);
            btnAll.TitleLabel.Font = UIFont.SystemFontOfSize(15);
            btnAll.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnAll.Layer.BorderWidth = 1;
            btnAll.ClipsToBounds = true;
            btnAll.Layer.CornerRadius = 5;
            btnAll.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAll.TouchUpInside += (sender, e) =>
            {
                //selectAllCategory
                if (paymentSelect != Utils.TextBundle("all", "Items") && paymentSelect == "" )
                {
                    paymentSelect = Utils.TextBundle("all", "Items");
                    listChoosePayment = new List<PaymentType>();
                    listChoosePayment = lstPayment;

                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    listChoosePayment = new List<PaymentType>();
                    paymentSelect = "";

                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
                }
                listPayment = new ListPaymentType(lstPayment);

                PaymentReportSelectDataSource report_adapter_customer = new PaymentReportSelectDataSource(listPayment);
                PaymentCollectionView.DataSource = report_adapter_customer;
                PaymentCollectionView.ReloadData();
            };
            SearchbarView.AddSubview(btnAll);

            #endregion

            #region PaymentCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            PaymentCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            PaymentCollectionView.BackgroundColor = UIColor.White;
            PaymentCollectionView.ShowsVerticalScrollIndicator = false;
            PaymentCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentCollectionView.RegisterClassForCell(cellType: typeof(ReportChooseEmployeeViewCell), reuseIdentifier: "ReportChooseEmployeeViewCell");
            View.AddSubview(PaymentCollectionView);

            ReportEmployeeCollectionDelegate CollectionDelegate = new ReportEmployeeCollectionDelegate();
            CollectionDelegate.OnItemSelected += async (indexPath) => {
                var payment = listPayment[(int)indexPath.Row];
                var search = listChoosePayment.FindIndex(x => x.Type == payment.Type);
                if (search == -1)
                {
                    listChoosePayment.Add(payment);
 
                }
                else
                {
                    listChoosePayment.RemoveAt(search);
                }

                paymentSelect = "";

                if (5 == listChoosePayment.Count)
                {
                    paymentSelect = Utils.TextBundle("all", "Items");
                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    paymentSelect = "";
                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
                    foreach (var item in listChoosePayment)
                    {
                        if (paymentSelect != "")
                        {
                            paymentSelect += "," + item.Type;
                        }
                        else
                        {
                            paymentSelect = item.Type;
                        }
                    }
                }
                lstPayment = await SetListPament();
                listPayment = new ListPaymentType(lstPayment);

                PaymentReportSelectDataSource report_adapter_customer = new PaymentReportSelectDataSource(listPayment);
                PaymentCollectionView.DataSource = report_adapter_customer;
                PaymentCollectionView.ReloadData();
            };
            PaymentCollectionView.Delegate = CollectionDelegate;
            
            #endregion

            #region BottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomView.BackgroundColor = UIColor.White;
            View.AddSubview(bottomView);

            btnSelect = new UIButton();
            btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSelect.Layer.CornerRadius = 5f;
            btnSelect.Layer.BorderWidth = 0.5f;
            btnSelect.Enabled = true;
            btnSelect.SetTitle(Utils.TextBundle("applypayment", "Items"), UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                PaymentTypeReportController.listChoosePayment = listChoosePayment;
                PaymentTypeReportController.isModifyPayment = true;
                this.NavigationController.PopViewController(false);
            };
            View.AddSubview(btnSelect);
            #endregion
        }
        
        void SetupAutoLayout()
        {
            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            btnSearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            txtSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            txtSearch.RightAnchor.ConstraintEqualTo(btnAll.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            txtSearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtSearch.HeightAnchor.ConstraintEqualTo(36).Active = true;

            btnAll.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnAll.WidthAnchor.ConstraintEqualTo(38).Active = true;
            btnAll.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnAll.HeightAnchor.ConstraintEqualTo(30).Active = true;
            #endregion

            PaymentCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            PaymentCollectionView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
            PaymentCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PaymentCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnSelect.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSelect.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnSelect.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSelect.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}