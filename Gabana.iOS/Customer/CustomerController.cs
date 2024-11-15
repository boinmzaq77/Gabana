using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource.Sync;
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
    public partial class CustomerController : UIViewController
    {
        public static bool Ismodify ;
        NotificationManager notificationManager = new NotificationManager();
        UICollectionView CustomerCollection;
        List<Customer> listCustomer;
        UpdateCustomerController AddCustomerPage;
        UIImageView addCustomer;
        UIImageView emptyView;
        UILabel lbl_empty_cus;
        CustomerManage CustomerManager = new CustomerManage();
        UIView SearchBarView;
        UIButton btnSearch;
        UITextField txtSearch;
        UIScrollView scroll;
        UIView oriView;
        private string LoginType;
        private bool Editchange;
        public static bool checknet = true;
        public CustomerController()
        {
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
             
            try
            {
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
                LoginType = Preferences.Get("LoginType", "");
                checknet = await GabanaAPI.CheckNetWork();
                Utils.SetTitle(this.NavigationController,Utils.TextBundle("customer", "Customer")); 
                txtSearch.Text = null;

                var data = CustomerCollection?.DataSource as CustomerDataSource;
                if (data.choosecell != null)
                {
                    var frame2 = data.choosecell.Frame;
                    frame2.X = 0;
                    UIView.Animate(0.7, () =>
                    {
                        data.choosecell.showbtndelete = false;
                        data.choosecell.Frame = frame2;
                    });
                };

                listCustomer = await GetListCustomer();
                ((CustomerDataSource)CustomerCollection.DataSource).ReloadData(listCustomer);
                CustomerCollection.ReloadData();

                showList();
                
                if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    var pinCodePage = new PinCodeController("Pincode");
                    pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    await this.PresentViewControllerAsync(pinCodePage, false);
                }
                if (!UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer"))
                {
                    addCustomer.Alpha = 0.5f;

                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
        }
        
        public async override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();

                

                
                View.BackgroundColor = UIColor.FromRGB(248,248,248);
                Utils.SetTitle(this.NavigationController,Utils.TextBundle("customer", "Customer"));
                oriView = new UIView(this.View.Frame); 


                #region SearchBarView
                SearchBarView = new UIView();
                SearchBarView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
                SearchBarView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(SearchBarView);

                bool clearSearch = false;
                txtSearch = new UITextField
                {
                    TextColor = UIColor.FromRGB(0,149,218),
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
                txtSearch.Placeholder = Utils.TextBundle("customerplace", "");
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

                scroll = new UIScrollView();
                scroll.UserInteractionEnabled = true;
                scroll.ShowsVerticalScrollIndicator = true;
                scroll.ScrollEnabled = true;
                scroll.BackgroundColor = UIColor.White;
                scroll.ContentSize = new CGSize(View.Frame.Width, View.Frame.Height + 100);
                scroll.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(scroll);

                #region emptyView
                emptyView = new UIImageView();
                emptyView.Hidden = true;
                emptyView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                emptyView.Image = UIImage.FromBundle("DefaultCustomer");
                emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
                scroll.AddSubview(emptyView);

                lbl_empty_cus = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(160, 160, 160),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lbl_empty_cus.Hidden = true;
                lbl_empty_cus.Lines = 2;
                lbl_empty_cus.Font = lbl_empty_cus.Font.WithSize(16);
                lbl_empty_cus.Text = Utils.TextBundle("nullcustomer", "คุณยังไม่มีลูกค้าสามารถเพิ่ม\nได้ที่ปุ่มเพิ่มด้านล่าง");
                scroll.AddSubview(lbl_empty_cus);
                #endregion

                listCustomer = await GetListCustomer();

               

                #region CustomerCollection
                UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width)+80, height: 80);
                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemflowLayoutList.MinimumLineSpacing = 1;


                CustomerCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
                CustomerCollection.BackgroundColor = UIColor.FromRGB(248,248,248);
                CustomerCollection.ShowsVerticalScrollIndicator = false;
                CustomerCollection.AlwaysBounceVertical = true;
                CustomerCollection.DraggingStarted += CustomerCollection_Scrolled;
                CustomerCollection.DraggingEnded += CustomerCollection_Scrolled;
                CustomerCollection.WillEndDragging += CustomerCollection_Scrolled;
                CustomerCollection.Scrolled += CustomerCollection_Scrolled;
                CustomerCollection.ScrolledToTop += CustomerCollection_Scrolled;
                CustomerCollection.TranslatesAutoresizingMaskIntoConstraints = false;
                CustomerCollection.RegisterClassForCell(cellType: typeof(CustomerCollectionViewCell), reuseIdentifier: "CustomerViewCell");
                //CustomerCollection. += CustomerCollection_Scrolled;
                CustomerDataSource CustomerDataList = new CustomerDataSource(listCustomer); // ส่ง list ไป
                CustomerDataList.OnCardCellDelete += Customer_OnCardCellDelete;
                CustomerDataList.OnCardCell += CustomerDataList_OnCardCell;
                CustomerDataList.OnCardscrollCell += CustomerDataList_OnCardscrollCell; 
                CustomerCollectionDelegate CustomerCollectionDelegate = new CustomerCollectionDelegate();
                
                CustomerCollectionDelegate.OnItemSelected += (indexPath) => {

                    Utils.SetTitle(this.NavigationController,Utils.TextBundle("editcustomer", "Edit Customer"));
                    AddCustomerPage = new UpdateCustomerController(listCustomer[(int)indexPath.Row]);
                    this.NavigationController.PushViewController(AddCustomerPage, false);
                };
                CustomerCollection.Delegate = CustomerCollectionDelegate;
                CustomerCollection.DataSource = CustomerDataList;
               
                View.AddSubview(CustomerCollection);
                #endregion

                addCustomer = new UIImageView();
                addCustomer.Image = UIImage.FromBundle("Add");
                addCustomer.TranslatesAutoresizingMaskIntoConstraints = false;
                addCustomer.UserInteractionEnabled = true;
                var tapGesture = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("AddCus:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                addCustomer.AddGestureRecognizer(tapGesture);
                View.AddSubview(addCustomer);

               

                setupAutoLayout();
                showList();
                Setkeyboard();
                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget(async (obj, sender) => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.CustomerChange();
                    }
                    listCustomer = await GetListCustomer();
                    ((CustomerDataSource)CustomerCollection.DataSource).ReloadData(listCustomer);
                    CustomerCollection.ReloadData();
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                CustomerCollection.AddSubview(refreshControl);
            }
            catch (Exception ex )
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }

        private void CustomerDataList_OnCardscrollCell(NSIndexPath indexPath)
        {
            View.EndEditing(true);
        }

        private void CustomerCollection_Scrolled(object sender, EventArgs e)
        {
            View.EndEditing(true);
        }

        private void CustomerDataList_OnCardCell(NSIndexPath indexPath)
        {
            var item = listCustomer?.Where(x => x.SysCustomerID == listCustomer[(int)indexPath.Row].SysCustomerID).FirstOrDefault();
            if (!string.IsNullOrEmpty(item?.PicturePath))
            {
                GabanaShowImage.SharedInstance.Show(this, item.PicturePath, "defaultcust.png");
            }
        }

        [Export("showimage:")]
        public void Close2(UIGestureRecognizer sender)
        {
            GabanaShowImage.SharedInstance.Hide();
        }
        private void Setkeyboard()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            void OnKeyboardNotification(NSNotification notification)
            {
                if (!IsViewLoaded) return;


                //Check if the keyboard is becoming visible
                var visible = notification.Name == UIKeyboard.WillShowNotification;

                //Start an animation, using values from the keyboard
                //UIView.BeginAnimations("AnimateForKeyboard");
                //UIView.SetAnimationBeginsFromCurrentState(true);
                UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
                UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

                //Pass the notification, calculating keyboard height, etc.
                bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
                var keyboardFrame = visible
                                        ? UIKeyboard.FrameEndFromNotification(notification)
                                        : UIKeyboard.FrameBeginFromNotification(notification);

                OnKeyboardChanged(View, visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);

                //Commit the animation
                //UIView.CommitAnimations();
            }
        }
        public void OnKeyboardChanged(UIView view, bool visible, nfloat nfloat)
        {
            if (!visible)
                view.Frame = new CGRect(0, 0, oriView.Frame.Width, oriView.Frame.Height);  
            else

                view.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height- nfloat);
        }
        private async void Customer_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
                if (check)
                {
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", " you sure you want to delete customer?"), UIAlertControllerStyle.Alert);
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => DeleteCus(indexPath)));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                    PresentViewController(okCancelAlertController, true, null);
                }
                else
                {
                  Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                }
            
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูล Customer ได้");
                return;
            }
            
        }

        private async void DeleteCus(NSIndexPath indexPath)
        {
            try
            {
                
                CustomerManage CustomerManage = new CustomerManage();
                var cusdelete = listCustomer[(int)indexPath.Row];
                cusdelete.DataStatus = 'D';
                cusdelete.FWaitSending = 1;
                cusdelete.LastDateModified = DateTime.UtcNow;
                var result = await CustomerManage.UpdateCustomer(cusdelete);
                if (result)
                {
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Report"));
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendCustomer((int)MainController.merchantlocal.MerchantID, (int)cusdelete.SysCustomerID);
                    }
                    //await CustomerSync.SentCustomer((int)MainController.merchantlocal.MerchantID, (int)cusdelete.SysCustomerID, null);
                    listCustomer = await GetListCustomer();
                    ((CustomerDataSource)CustomerCollection.DataSource).ReloadData(listCustomer);
                    CustomerCollection.ReloadData();
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotdelete", "Report"));
                }
                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(Utils.TextBundle("cannotdelete", "Report"));
                return;
            }
        }

        [Export("AddCus:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("addcustomer", "Report"));
                AddCustomerPage = new UpdateCustomerController();
                this.NavigationController.PushViewController(AddCustomerPage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        void showList()
        {
            if (listCustomer == null || listCustomer.Count == 0)
            {
              //  SearchBarView.Hidden = true;
                CustomerCollection.Hidden = true;
                emptyView.Hidden = false;
                lbl_empty_cus.Hidden = false;
                scroll.Hidden = false;
            }
            else
            {
              //  SearchBarView.Hidden = false;
                CustomerCollection.Hidden = false;
                emptyView.Hidden = true;
                lbl_empty_cus.Hidden = true;
                scroll.Hidden = true;
            }
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
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Report"));
                }
                else
                {
                    ((CustomerDataSource)CustomerCollection.DataSource).ReloadData(listCustomer);
                    CustomerCollection.ReloadData();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Report"));
            }
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
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "Report"));
                    return null;
                }
                return listCustomer;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
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

            scroll.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor).Active = true;
            scroll.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            scroll.WidthAnchor.ConstraintEqualTo(View.Frame.Width).Active = true;
            scroll.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.TopAnchor, 38).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_cus.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_cus.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_cus.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_cus.CenterXAnchor.ConstraintEqualTo(scroll.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            CustomerCollection.TopAnchor.ConstraintEqualTo(SearchBarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CustomerCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CustomerCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            CustomerCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;


            addCustomer.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            addCustomer.WidthAnchor.ConstraintEqualTo(45).Active = true;
            addCustomer.HeightAnchor.ConstraintEqualTo(45).Active = true;
            addCustomer.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
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