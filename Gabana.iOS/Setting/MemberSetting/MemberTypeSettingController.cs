using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class MemberTypeSettingController : UIViewController
    {
        NotificationManager notificationManager = new NotificationManager();
        UICollectionView MembertypeCollection;
        List<MemberType> listmemberTypes = new List<MemberType>();
        UIButton addBranch;
        MemberTypeManage memberTypeManage = new MemberTypeManage();
        UIImageView addMembertype; 
        BranchManage BranchManager = new BranchManage();
        private UILabel lbltype;
        private UILabel lblmax;

        public MemberTypeSettingController()
        {
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
             
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (MembertypeCollection != null)
            {
                
                var data = MembertypeCollection?.DataSource as MemberTypeSettingDataSource;
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
                // 
                listmemberTypes = await GetListMembertype();
                ((MemberTypeSettingDataSource)MembertypeCollection.DataSource).ReloadData(listmemberTypes);
                MembertypeCollection.ReloadData();
                 

                if (listmemberTypes.Count >= 3 )
                {
                   //addMembertype.AddGestureRecognizer(null);
                    addMembertype.UserInteractionEnabled = false;
                    addMembertype.Layer.Opacity = 0.2f;
                }

                 
            }
            // 

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            //MembertypeCollection.ReloadData();
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public async override void ViewDidLoad()
        {
            try
            {
                View.BackgroundColor = UIColor.White;
                base.ViewDidLoad();

                //listmemberTypes = await GetListBranch();
                lbltype = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
                lbltype.TextColor = UIColor.FromRGB(247, 86, 0); 
                lbltype.TextAlignment = UITextAlignment.Left;
                lbltype.Font = lbltype.Font.WithSize(15);
                lbltype.Text = Utils.TextBundle("membertype", "Member Type");
                View.AddSubview(lbltype);

                lblmax = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
                lblmax.TextColor = UIColor.FromRGB(200, 200, 200);
                lblmax.TextAlignment = UITextAlignment.Left;
                lblmax.Font = lblmax.Font.WithSize(15);
                lblmax.Text = Utils.TextBundle("max3", "Maximum 3");
                View.AddSubview(lblmax);

                #region BranchCollection
                UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
                itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 60, height: 60);
                itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemflowLayoutList.MinimumInteritemSpacing = 1;
                itemflowLayoutList.MinimumLineSpacing = 1;
                MembertypeCollection = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList); ;
                MembertypeCollection.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                MembertypeCollection.ShowsVerticalScrollIndicator = false;
                MembertypeCollection.TranslatesAutoresizingMaskIntoConstraints = false;
                MembertypeCollection.RegisterClassForCell(cellType: typeof(MemberTypeSettingViewCell), reuseIdentifier: "MemberTypeSettingViewCell");


                MemberTypeSettingDataSource BranchDataList = new MemberTypeSettingDataSource(listmemberTypes); // ส่ง list ไป
                BranchDataList.OnCardCellDelete += BranchDataList_OnCardCellDelete;
                MembertypeCollection.DataSource = BranchDataList;
                MemberTypeSettingCollectionDelegate memberTypeSettingCollectionDelegate = new MemberTypeSettingCollectionDelegate();
                memberTypeSettingCollectionDelegate.OnItemSelected += async (indexPath) => {
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("editmembertype", "Edit Member Type"));
                    AddMemberTypeSettingController addMemberTypeSetting = new AddMemberTypeSettingController(listmemberTypes[(int)indexPath.Row]);
                    this.NavigationController.PushViewController(addMemberTypeSetting, false);
                };
                MembertypeCollection.Delegate = memberTypeSettingCollectionDelegate;
                View.AddSubview(MembertypeCollection);
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

                addMembertype = new UIImageView();
                addMembertype.Image = UIImage.FromBundle("Add");
                addMembertype.TranslatesAutoresizingMaskIntoConstraints = false;

                addMembertype.UserInteractionEnabled = true;
                var tapGesture = new UITapGestureRecognizer(this,
                        new ObjCRuntime.Selector("AddBmember:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                addMembertype.AddGestureRecognizer(tapGesture);
                View.AddSubview(addMembertype);

                setupAutoLayout();

                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget(async (obj, sender) => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.MemberTypeChange();
                    }

                    listmemberTypes = await GetListMembertype();
                    ((MemberTypeSettingDataSource)MembertypeCollection.DataSource).ReloadData(listmemberTypes);
                    MembertypeCollection.ReloadData();

                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                MembertypeCollection.AlwaysBounceVertical = true;
                MembertypeCollection.AddSubview(refreshControl);


            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        [Export("AddBmember:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("addmembertype", "Add Member Type"));
            AddMemberTypeSettingController addMemberTypeSetting = new AddMemberTypeSettingController(null);
            this.NavigationController.PushViewController(addMemberTypeSetting, false);
        }
        private async void BranchDataList_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "Are you sure you want to delete?"), UIAlertControllerStyle.Alert);

                //Add Actions
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => DeleteBranch(indexPath)));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                //Present Alert
                PresentViewController(okCancelAlertController, true, null);
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("failedtodelete", "Failed to delete"));
                return;
            }
        }

        private async void DeleteBranch(NSIndexPath indexPath)
        {
            
            try
            {
                List<Gabana.ORM.Master.MemberType> lstmemberType = new List<Gabana.ORM.Master.MemberType>();

                var configlocal = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap< ORM.MerchantDB.MemberType , Gabana.ORM.Master.MemberType>();
                });

                var Imapperlocal = configlocal.CreateMapper();
                var Branchlocal = Imapperlocal.Map< ORM.MerchantDB.MemberType, Gabana.ORM.Master.MemberType>(listmemberTypes[indexPath.Row]);

                lstmemberType.Add(Branchlocal);
                
                if (lstmemberType != null)
                {
                    var DeleteonCloud = await GabanaAPI.DeleteDataMemberType(lstmemberType);
                    if (DeleteonCloud == null)
                    {
                        await GabanaAPI.DeleteDataMemberType(lstmemberType);
                    }
                    if (!DeleteonCloud.Status)
                    {
                        Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                        return;
                    }
                    if (DeleteonCloud.Status)
                    {
                        //Set Null ที่ customer ที่มีการใช้ membertype 
                        CustomerManage customerManage = new CustomerManage();
                        var check = await customerManage.UpdateNullCustomerandDeleteMembeytype(DataCashingAll.MerchantId, lstmemberType[0].MemberTypeNo);
                        if (!check)
                        {
                            Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                            return;
                        }
                        if (check)
                        {
                            Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
                            MyQrCodeManage myQrCodeManage = new MyQrCodeManage();
                            //var deleteqr = await myQrCodeManage.DeleteMyQrCodefromBranch(DataCashingAll.MerchantId, (int)SysbranchID);
                            listmemberTypes = await GetListMembertype();
                            ((MemberTypeSettingDataSource)MembertypeCollection.DataSource).ReloadData(listmemberTypes);
                            MembertypeCollection.ReloadData();
                            addMembertype.UserInteractionEnabled = true;
                            addMembertype.Layer.Opacity = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                return;
            }
        }

        async Task<List<MemberType>> GetListMembertype()
        {
            try
            {
                var lstmemberTypes = new List<MemberType>();
                if (await GabanaAPI.CheckNetWork())
                {
                    var listmembertype = await GabanaAPI.GetDataMemberType();
                    if (listmembertype != null && listmembertype.Count > 0)
                    {
                        
                        var Allmember = await memberTypeManage.DeleteAllMemberType(DataCashingAll.MerchantId);
                        var lstmember = new List<MemberType>();
                        foreach (var item in listmembertype)
                        {
                            MemberType memberType = new MemberType()
                            {
                                DateModified = item.DateModified,
                                LinkProMaxxID = item.LinkProMaxxID,
                                MemberTypeName = item.MemberTypeName,
                                MemberTypeNo = item.MemberTypeNo,
                                MerchantID = item.MerchantID,
                                PercentDiscount = item.PercentDiscount
                            };
                            var InsertorReplace = await memberTypeManage.InsertorReplacrMemberType(memberType);
                            lstmember.Add(memberType);
                        }
                        
                        lstmemberTypes.AddRange(lstmember);
                        lstmemberTypes = lstmemberTypes.OrderBy(x => x.MemberTypeNo).ToList();
                    }
                }
                else
                {
                    
                    lstmemberTypes = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                }
                
                return lstmemberTypes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        void setupAutoLayout()
        {
            lbltype.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            lbltype.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lbltype.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lbltype.WidthAnchor.ConstraintLessThanOrEqualTo (200).Active = true;

            lblmax.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            lblmax.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblmax.LeftAnchor.ConstraintEqualTo(lbltype.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lblmax.WidthAnchor.ConstraintGreaterThanOrEqualTo(50).Active = true;

            MembertypeCollection.TopAnchor.ConstraintEqualTo(lbltype.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            MembertypeCollection.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            MembertypeCollection.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            MembertypeCollection.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            //addBranch.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            //addBranch.WidthAnchor.ConstraintEqualTo(110).Active = true;
            //addBranch.HeightAnchor.ConstraintEqualTo(45).Active = true;
            //addBranch.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

            addMembertype.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            addMembertype.WidthAnchor.ConstraintEqualTo(45).Active = true;
            addMembertype.HeightAnchor.ConstraintEqualTo(45).Active = true;
            addMembertype.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}