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

    public partial class ReportSelectCustomerController : UIViewController
    {
        UICollectionView CustomerCollectionView;

        UIView bottomView;
        UIButton btnSelect;
        CustomerManage setCus = new CustomerManage();
        List<Customer> lstCustomer = new List<Customer>();
        public static List<Customer> listChooseCustomer = new List<Customer>();
        UIView SearchbarView;
        UITextField txtSearch;
        UIButton btnSearch, btnAll;
        private string customerSelect ="";

        CustomerManage customer = new CustomerManage();


        int catcount = 0;
        private static ListCustomer listCustomer;



        public ReportSelectCustomerController()
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
                SetCustomerData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        async void SetCustomerData()
        {
            lstCustomer = await GetListCustomer();
            listCustomer = new ListCustomer(lstCustomer);

            catcount = lstCustomer.Count;
            listChooseCustomer = CustomerReportController.listChooseCustomer;

            if (CustomerReportController.listChooseCustomer.Count == catcount)
            {
                customerSelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                customerSelect = Utils.TextBundle("all", "Items");
                btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAll.BackgroundColor = UIColor.White;
            }

            

            CustomerReportSelectDataSource CategoryDataList = new CustomerReportSelectDataSource(listCustomer);
            CustomerCollectionView.DataSource = CategoryDataList;
        }
        async Task<List<Customer>> GetListCustomer()
        {
            try
            {
                lstCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                lstCustomer = await customerManage.GetAllCustomer();
                if (lstCustomer == null)
                {
                    return null;
                }
                return lstCustomer;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return null;
            }
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
            txtSearch.Font = txtSearch.Font.WithSize(15);
            txtSearch.ReturnKeyType = UIReturnKeyType.Done;
            txtSearch.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                SearchBytxt();
                return true;
            };
            SearchbarView.AddSubview(txtSearch);

            btnSearch = new UIButton();
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
                if (customerSelect != Utils.TextBundle("all", "Items") && customerSelect == "" )
                {
                    customerSelect = Utils.TextBundle("all", "Items");
                    listChooseCustomer = new List<Customer>();
                    listChooseCustomer = lstCustomer;

                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    listChooseCustomer = new List<Customer>();
                    customerSelect = "";

                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
                }
                listCustomer = new ListCustomer(lstCustomer);

                CustomerReportSelectDataSource report_adapter_customer = new CustomerReportSelectDataSource(listCustomer);
                CustomerCollectionView.DataSource = report_adapter_customer;
                CustomerCollectionView.ReloadData();
            };
            SearchbarView.AddSubview(btnAll);

            #endregion

            #region CustomerCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 60);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            CustomerCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            CustomerCollectionView.BackgroundColor = UIColor.White;
            CustomerCollectionView.ShowsVerticalScrollIndicator = false;
            CustomerCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            CustomerCollectionView.RegisterClassForCell(cellType: typeof(ReportChooseCustomerViewCell), reuseIdentifier: "ReportChooseCustomerViewCell");
            View.AddSubview(CustomerCollectionView);

            ReportCustomerCollectionDelegate CollectionDelegate = new ReportCustomerCollectionDelegate();
            CollectionDelegate.OnItemSelected += async (indexPath) => {
                var cusotmer = listCustomer[(int)indexPath.Row];
                var search = listChooseCustomer.FindIndex(x => x.SysCustomerID == cusotmer.SysCustomerID && x.MerchantID == (int)MainController.merchantlocal.MerchantID);
                if (search == -1)
                {
                    listChooseCustomer.Add(cusotmer);
 
                }
                else
                {
                    listChooseCustomer.RemoveAt(search);
                }

                customerSelect = "";

                if (catcount == listChooseCustomer.Count)
                {
                    customerSelect = Utils.TextBundle("all", "Items");
                    btnAll.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    customerSelect = "";
                    btnAll.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAll.BackgroundColor = UIColor.White;
                    foreach (var item in listChooseCustomer)
                    {
                        if (customerSelect != "")
                        {
                            customerSelect += "," + item.CustomerName;
                        }
                        else
                        {
                            customerSelect = item.CustomerName;
                        }
                    }
                }
                lstCustomer = await GetListCustomer();
                listCustomer = new ListCustomer(lstCustomer);

                CustomerReportSelectDataSource report_adapter_customer = new CustomerReportSelectDataSource(listCustomer);
                CustomerCollectionView.DataSource = report_adapter_customer;
                CustomerCollectionView.ReloadData();
            };
            CustomerCollectionView.Delegate = CollectionDelegate;
            
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
            btnSelect.SetTitle(Utils.TextBundle("applycustomer", "Items"), UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                CustomerReportController.listChooseCustomer = listChooseCustomer;
                CustomerReportController.isModifyCustomer = true;
                this.NavigationController.PopViewController(false);
            };
            View.AddSubview(btnSelect);
            #endregion
        }
        async void SearchBytxt()
        {
            var list = await GetFilterBranchList();

            if (list!= null)
            {
                listCustomer = new ListCustomer(list);
                
            }
            CustomerReportSelectDataSource report_adapter_customer = new CustomerReportSelectDataSource(listCustomer);
            CustomerCollectionView.DataSource = report_adapter_customer;
            CustomerCollectionView.ReloadData();
        }
        async Task<List<Customer>> GetFilterBranchList()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var itemlst = await setCus.GetCustomerSearch((int)MainController.merchantlocal.MerchantID,txtSearch.Text);
                    return itemlst;
                }
                var itm = await setCus.GetAllCustomer();
                if (itm == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
                    return null;
                }
                return itm;
            }
            catch (Exception ex)
            {

                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Items"));
                return null;
            }
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

            CustomerCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CustomerCollectionView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
            CustomerCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CustomerCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

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