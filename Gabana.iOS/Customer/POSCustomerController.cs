using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
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
    public partial class POSCustomerController : UIViewController
    {
        UICollectionView CustomerCollection;
        List<Customer> listCustomer;
        UpdateCustomerController AddCustomerPage;
        ListCustomer lstCustomer;
        public static Customer SelectedCustomer = null;
        CustomerManage CustomerManager = new CustomerManage();
        UIButton removeCustomer;
        NSIndexPath nSIndexPath;
        UIView SearchBarView;
        UIButton btnSearch;
        UITextField txtSearch;
        private string LoginType;

        public POSCustomerController()
        {
             POSCustomerController.SelectedCustomer = POSController.SelectedCustomer;
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //CustomerCollection.ReloadData();
            
            this.NavigationController.SetNavigationBarHidden(false, false);
            listCustomer = await GetListCustomer();
            ((POSCustomerDataSource)CustomerCollection.DataSource).ReloadData(listCustomer);
            CustomerCollection.ReloadData();

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            CustomerCollection.ReloadData();
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public async Task funtionbutton()
        {
                
        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LoginType = Preferences.Get("LoginType", "");
            View.BackgroundColor = UIColor.White;
         //   this.Title = "Customer";
         
            UIBarButtonItem selectCustomer = new UIBarButtonItem();
            selectCustomer.Image = UIImage.FromBundle("AddCust");
            selectCustomer.Clicked += (sender, e) => {
                // open select customer page
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
                if (check)
                {
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("addcustomer", "addcustomer"));
                    AddCustomerPage = new UpdateCustomerController();
                    this.NavigationController.PushViewController(AddCustomerPage, false);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                }
                //AddCustomerPage = new UpdateCustomerController();
                //this.NavigationController.PushViewController(AddCustomerPage, false);
            };
            this.NavigationItem.RightBarButtonItem = selectCustomer;


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
                    SearchBytxt();
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
            //txtSearch.EditingDidEndOnExit += CustomerCollection_Scrolled;
            SearchBarView.AddSubview(txtSearch);

            btnSearch = new UIButton();
            btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside += (sender, e) =>
            {

                if (btnSearch.ImageView.Image == UIImage.FromBundle("Search"))
                {
                    txtSearch.BecomeFirstResponder();
                }
                else
                {
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    txtSearch.Text = "";
                    SearchBytxt();
                }



            };
            SearchBarView.AddSubview(btnSearch);
            #endregion scroll = new UIScrollView();

            listCustomer = await GetListCustomer();
            #region CustomerCollection
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 80);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            CustomerCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            CustomerCollection.BackgroundColor = UIColor.White;
            CustomerCollection.ShowsVerticalScrollIndicator = false;
            CustomerCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            CustomerCollection.RegisterClassForCell(cellType: typeof(POSCustomerCollectionViewCell), reuseIdentifier: "posCustomerCollectionViewCell");


            POSCustomerDataSource DataList = new POSCustomerDataSource(listCustomer); // ส่ง list ไป
            CustomerCollection.DataSource = DataList;
            POSCustomerCollectionDelegate CustomerCollectionDelegate = new POSCustomerCollectionDelegate();
            CustomerCollectionDelegate.OnItemSelected += async (indexPath) => {
                if (POSController.SelectedCustomer?.SysCustomerID == listCustomer[(int)indexPath.Row].SysCustomerID)
                {
                    POSCustomerController.SelectedCustomer = listCustomer[(int)indexPath.Row];
                    nSIndexPath = indexPath;
                    CustomerCollection.ReloadData();
                    removeCustomer.SetTitle(Utils.TextBundle("removecustomer", "removecustomer"), UIControlState.Normal);
                    removeCustomer.BackgroundColor = UIColor.White;
                    removeCustomer.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
                    removeCustomer.SetTitleColor(UIColor.FromRGB(74, 74, 74), UIControlState.Normal);
                    removeCustomer.Layer.BorderWidth = 1;
                    removeCustomer.TouchUpInside -= RemoveCustomer_TouchUpInside1;
                    removeCustomer.TouchUpInside -= RemoveCustomer_TouchUpInside;
                    removeCustomer.TouchUpInside += RemoveCustomer_TouchUpInside1;
                }
                else
                {
                    POSCustomerController.SelectedCustomer = listCustomer[(int)indexPath.Row];
                    nSIndexPath = indexPath;
                    CustomerCollection.ReloadData();
                    removeCustomer.Hidden = false;
                    Utils.SetConstant(removeCustomer.Constraints, NSLayoutAttribute.Height, 45);
                    removeCustomer.SetTitle(Utils.TextBundle("choosecustomer", "Choose Customer"), UIControlState.Normal);
                    removeCustomer.SetTitleColor(UIColor.White, UIControlState.Normal);
                    removeCustomer.Layer.CornerRadius = 5f;
                    removeCustomer.BackgroundColor = UIColor.FromRGB(51, 172, 225);
                    removeCustomer.TouchUpInside -= RemoveCustomer_TouchUpInside1;
                    removeCustomer.TouchUpInside -= RemoveCustomer_TouchUpInside;
                    removeCustomer.TouchUpInside += RemoveCustomer_TouchUpInside;
                }
                
                


            };
            CustomerCollection.Delegate = CustomerCollectionDelegate;
            View.AddSubview(CustomerCollection);
            #endregion

            removeCustomer = new UIButton();
            //removeCustomer.Hidden = true;
            removeCustomer.Font = removeCustomer.Font.WithSize(16);
            removeCustomer.Layer.CornerRadius = 5;
            removeCustomer.SetTitle(Utils.TextBundle("removecustomer", "Remove Customer"), UIControlState.Normal);
            removeCustomer.BackgroundColor = UIColor.White;
            removeCustomer.Layer.BorderColor = UIColor.FromRGB(226, 226, 226).CGColor;
            removeCustomer.Layer.BorderWidth = 1;
            removeCustomer.ClipsToBounds = true;
            removeCustomer.SetTitleColor(UIColor.FromRGB(74, 74, 74), UIControlState.Normal);
            removeCustomer.TranslatesAutoresizingMaskIntoConstraints = false;

            removeCustomer.TouchUpInside += RemoveCustomer_TouchUpInside1; 
                
            View.AddSubview(removeCustomer);

            setupAutoLayout();

            if(POSController.SelectedCustomer!= null)
            {
                Utils.SetConstant(removeCustomer.Constraints,NSLayoutAttribute.Height , 45);
                removeCustomer.Hidden = false;
            }
        }

        private async void RemoveCustomer_TouchUpInside1(object sender, EventArgs e)
        {
            POSController.SelectedCustomer = null;
            POSCustomerController.SelectedCustomer = null;
            CustomerCollection.ReloadData();
            removeCustomer.Hidden = true;
            POSController.tranWithDetails = await BLTrans.RemovePerson(POSController.tranWithDetails);
            this.NavigationController.PopViewController(false);
        }
        async void SearchBytxt()
        {
            //GetCustomerSearch
            try
            {
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    listCustomer = await GetListCustomer();
                }
                else
                {
                    listCustomer = await CustomerManager.GetCustomerSearch((int)MainController.merchantlocal.MerchantID, txtSearch.Text);
                }

                if (listCustomer == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
                }
                else
                {
                    ((POSCustomerDataSource)CustomerCollection.DataSource).ReloadData(listCustomer);
                    CustomerCollection.ReloadData();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
            }
        }
        private async void RemoveCustomer_TouchUpInside(object sender, EventArgs e)
        {
            removeCustomer.Enabled = false;
            POSController.SelectedCustomer = listCustomer[(int)nSIndexPath.Row];
            if (POSController.tranWithDetails is null)
            {
                DataCaching.posPage.initialData();
            }
            POSController.tranWithDetails = await BLTrans.ChoosePerson(POSController.tranWithDetails, listCustomer[(int)nSIndexPath.Row]);
            this.NavigationController.PopViewController(false);
        }

        async Task<List<Customer>> GetListCustomer()
        {
            try
            {
                listCustomer = new List<Customer>();
                CustomerManage customerManage = new CustomerManage();
                listCustomer = await customerManage.GetAllCustomer();
                if (listCustomer == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "failed"), Utils.TextBundle("cannotload", "cannotload"));
                    return null;
                }
                return listCustomer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        void setupAutoLayout()
        {
            #region SearchBar
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

            #endregion

            removeCustomer.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            removeCustomer.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            removeCustomer.HeightAnchor.ConstraintEqualTo(0).Active = true;
            removeCustomer.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            CustomerCollection.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CustomerCollection.BottomAnchor.ConstraintEqualTo(removeCustomer.SafeAreaLayoutGuide.TopAnchor, -5).Active = true;
            CustomerCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CustomerCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public class ChooseCustomer : ORM.Master.Customer
        {
            public bool Choose { get; set; }
        }
        public class ListCustomer
        {
            public List<ORM.MerchantDB.Customer> customers;
            static List<ORM.MerchantDB.Customer> builitem;
            public ListCustomer(List<ORM.MerchantDB.Customer> lstcustomer)
            {
                builitem = lstcustomer;
                this.customers = builitem;

            }
            public int Count
            {
                get
                {
                    return customers == null ? 0 : customers.Count;
                }
            }
            public ORM.MerchantDB.Customer this[int i]
            {
                get { return customers == null ? null : customers[i]; }
            }
        }

    }
}