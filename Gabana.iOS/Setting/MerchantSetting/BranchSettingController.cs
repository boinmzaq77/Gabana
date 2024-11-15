using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
using Newtonsoft.Json;
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
    public partial class BranchSettingController : UIViewController
    {
        UICollectionView BranchCollection;
        private string emplogin;
        private string LoginType;
        List<Branch> listBranch;
        NotificationManager notificationManager = new NotificationManager();
        UIButton addBranch;
        UIImageView addCustomer; 
        BranchManage BranchManager = new BranchManage();

        BranchPolicyManage policyManage = new BranchPolicyManage();
        public BranchSettingController()
        {
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
             
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (BranchCollection != null)
            {
                 
                var data = BranchCollection?.DataSource as BranchSettingDataSource;
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
                 
                listBranch = await GetListBranch();
                ((BranchSettingDataSource)BranchCollection.DataSource).ReloadData(listBranch);
                BranchCollection.ReloadData();


            }


        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //BranchCollection.ReloadData();
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public async override void ViewDidLoad()
        {
            try
            {
                View.BackgroundColor = UIColor.White;
                base.ViewDidLoad();
                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                await GetGabanaInfo();
                listBranch = await GetListBranch();

                #region BranchCollection
                UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 60, height: 60);
                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemflowLayoutList.MinimumInteritemSpacing = 1;
                itemflowLayoutList.MinimumLineSpacing = 1;
                BranchCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList); ;
                BranchCollection.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                BranchCollection.ShowsVerticalScrollIndicator = false;
                BranchCollection.TranslatesAutoresizingMaskIntoConstraints = false;
                BranchCollection.RegisterClassForCell(cellType: typeof(BranchSettingViewCell), reuseIdentifier: "branchSettingViewCell");


                BranchSettingDataSource BranchDataList = new BranchSettingDataSource(listBranch); // ส่ง list ไป
                BranchDataList.OnCardCellDelete += BranchDataList_OnCardCellDelete;
                BranchCollection.DataSource = BranchDataList;
                BranchSettingCollectionDelegate BranchCollectionDelegate = new BranchSettingCollectionDelegate();
                BranchCollectionDelegate.OnItemSelected += async (indexPath) => {

                    var check = UtilsAll.CheckPermissionRoleUser(LoginType, "update", "branch");
                    if (check)
                    {
                        Utils.SetTitle(this.NavigationController, Utils.TextBundle("editbranch", "Edit Branch"));
                        var branch = await BranchManager.GetBranch((int)MainController.merchantlocal.MerchantID, (int)listBranch[(int)indexPath.Row].SysBranchID);
                        AddBranchController UpdateBranchPage = new AddBranchController(branch);
                        this.NavigationController.PushViewController(UpdateBranchPage, false);
                    }
                    else
                    {
                        var branchPolicies = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, emplogin.ToLower());
                        if (branchPolicies != null && branchPolicies.Count > 0)
                        {
                            var index = branchPolicies.FindIndex(x => x.SysBranchID == listBranch[indexPath.Row].SysBranchID);
                            if (index != -1)
                            {
                                Utils.SetTitle(this.NavigationController, Utils.TextBundle("editbranch", "Edit Branch"));
                                var branch = await BranchManager.GetBranch((int)MainController.merchantlocal.MerchantID, (int)listBranch[(int)indexPath.Row].SysBranchID);
                                AddBranchController UpdateBranchPage = new AddBranchController(branch);
                                this.NavigationController.PushViewController(UpdateBranchPage, false);
                            }
                            else
                            {
                                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                            }
                        }
                    }
                        
                };
                BranchCollection.Delegate = BranchCollectionDelegate;
                View.AddSubview(BranchCollection);
                #endregion

                //addBranch = new UIButton();
                //addBranch.Font = addBranch.Font.WithSize(14);
                //addBranch.Layer.CornerRadius = 23;
                //addBranch.SetTitle("Add Branch", UIControlState.Normal);
                //addBranch.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                //addBranch.SetTitleColor(UIColor.White, UIControlState.Normal);
                //addBranch.TranslatesAutoresizingMaskIntoConstraints = false;
                //addBranch.TouchUpInside += (sender, e) => {
                //    // go to add Branch page
                //    AddBranchController  addBranchPage = new AddBranchController();
                //    this.NavigationController.PushViewController(addBranchPage, false);
                //};
                //View.AddSubview(addBranch);

                addCustomer = new UIImageView();
                addCustomer.Image = UIImage.FromBundle("Add");
                addCustomer.TranslatesAutoresizingMaskIntoConstraints = false;
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "branch");
                if (!check || listBranch.Count > DataCashingAll.GetGabanaInfo.TotalBranch)
                {
                    addCustomer.Alpha = 0.5f;
                }
                addCustomer.UserInteractionEnabled = true;
                var tapGesture = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("AddBranch:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                addCustomer.AddGestureRecognizer(tapGesture);
                View.AddSubview(addCustomer);

                setupAutoLayout();

                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget(async (obj, sender) => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.BranchChange();
                    }
                    listBranch = await GetListBranch();
                    ((BranchSettingDataSource)BranchCollection.DataSource).ReloadData(listBranch);
                    BranchCollection.ReloadData();
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                BranchCollection.AlwaysBounceVertical = true;
                BranchCollection.AddSubview(refreshControl);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        private async Task GetGabanaInfo() 
        {
            try
            {
                Gabana.Model.GabanaInfo gabanaInfo = new Gabana.Model.GabanaInfo();
                gabanaInfo = await GabanaAPI.GetDataGabanaInfo();
                DataCashingAll.GetGabanaInfo = gabanaInfo;
                var GabanaInfo = JsonConvert.SerializeObject(gabanaInfo);
                Preferences.Set("GabanaInfo", GabanaInfo);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetCloudProductLicence at Package");
            }
        }
        [Export("AddBranch:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            if (listBranch.Count < DataCashingAll.GetGabanaInfo.TotalBranch)
            {
                Utils.ShowMessage(Utils.TextBundle("maxbranch", "The maximum number of branches."));
                return;
            }


            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "branch");
            if (check)
            {
                if (listBranch.Count <= 500) //branch มีได้สูงสุด 500 สาขา
                {
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("addbranch", "Add Branch"));
                    AddBranchController addBranchPage = new AddBranchController();
                    this.NavigationController.PushViewController(addBranchPage, false);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("maxbranch", "The maximum number of branches."));
                    //Toast.MakeText(this, Application.Context.GetString(Resource.String.maxbranch), ToastLength.Short).Show();
                }
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                //Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
            
        }
        private async void BranchDataList_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "branch");
                if (check )
                {
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodeletebranch", "Are you sure you want to delete branch?")  , UIAlertControllerStyle.Alert);

                    //Add Actions
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => DeleteBranch(indexPath)));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                    //Present Alert
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

        private async void DeleteBranch(NSIndexPath indexPath)
        {
            try
            {
                var SysbranchID = listBranch[indexPath.Row].SysBranchID;
                if (listBranch[indexPath.Row].SysBranchID == 1)
                {
                    Utils.ShowMessage(Utils.TextBundle("cantdelheadoffice", "Can't delete Head Office"));
                    //Toast.MakeText(this.Activity, $"Can't delete Head Office", ToastLength.Short).Show();
                }
                else
                {
                    var DeleteonCloud = await GabanaAPI.DeleteDataBranch((int)SysbranchID);
                    if (DeleteonCloud.Status)
                    {
                        BranchManage branchManage = new BranchManage();
                        var branchdata = await branchManage.GetBranch(DataCashingAll.MerchantId, (int)SysbranchID);
                        branchdata.Status = 'D';
                        var DeleteonLocal = await branchManage.UpdateBranch(branchdata);
                        if (DeleteonLocal)
                        {
                            Utils.ShowMessage(Utils.TextBundle("successfullydeleted", "Successfully deleted"));
                            //Toast.MakeText(this.Activity, $Utils.TextBundle("deletesuccessfully", "Delete data successfully"), ToastLength.Short).Show();

                            MyQrCodeManage myQrCodeManage = new MyQrCodeManage();
                            var deleteqr = await myQrCodeManage.DeleteMyQrCodefromBranch(DataCashingAll.MerchantId, (int)SysbranchID);
                            listBranch = await GetListBranch();
                            ((BranchSettingDataSource)BranchCollection.DataSource).ReloadData(listBranch);
                            BranchCollection.ReloadData();
                        }
                        else
                        {
                            Utils.ShowMessage(Utils.TextBundle("cannotdelete", "Failed to delete"));
                        }
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotdelete", "Failed to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cantdeletecustomer", "Can't delete customer"));
                return;
            }
        }

        async Task<List<Branch>> GetListBranch()
        {
            try
            {
                listBranch = new List<Branch>();
                BranchManage branchManage = new BranchManage();
                listBranch = await branchManage.GetAllBranch((int)MainController.merchantlocal.MerchantID);
                if (listBranch == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotloadbranch", "Unable to load branch data."));
                    return null;
                }
                return listBranch;
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
            BranchCollection.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            BranchCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BranchCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BranchCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            //addBranch.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            //addBranch.WidthAnchor.ConstraintEqualTo(110).Active = true;
            //addBranch.HeightAnchor.ConstraintEqualTo(45).Active = true;
            //addBranch.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

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
    }
}