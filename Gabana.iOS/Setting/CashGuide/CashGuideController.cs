using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class CashGuideController : UIViewController
    {
        UICollectionView CashGuideCollectionView;

        UIView bottomView;
        UIButton btnSelect;
        NotificationManager notificationManager = new NotificationManager();
        CashTemplateManage CashTemplateManage = new CashTemplateManage();
        public static List<CashTemplate> cashTemplates = new List<CashTemplate>();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string CURRENCYSYMBOLS;
        private UIImageView btnAdd;
        List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();
        private UILabel lblMax;
        private string LoginType;
        private UILabel lblCash;

        public CashGuideController() {
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            GabanaLoading.SharedInstance.Show(this);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            string currencySelec = "";
            
            if (CURRENCYSYMBOLS != null)
            {
                currencySelec = CURRENCYSYMBOLS;
            }
            //load cashTemplates 
            if (await GabanaAPI.CheckNetWork())
            {
                listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                if (listcashTemplate == null)
                {
                    
                    return;
                }
                //ลบข้อมูลทั้งหมด
                var delete = await CashTemplateManage.DeleteAllCashTemplatee(DataCashingAll.MerchantId);

                var lst = new List<CashTemplate>();
                foreach (var item in listcashTemplate)
                {
                    CashTemplate cashTemplate = new CashTemplate()
                    {
                        Amount = item.Amount,
                        CashTemplateNo = item.CashTemplateNo,
                        DateModified = item.DateModified,
                        MerchantID = item.MerchantID,
                    };
                    var InsertorReplace = await CashTemplateManage.InsertorReplaceCashTemplate(cashTemplate);
                    lst.Add(cashTemplate);
                }
                cashTemplates = new List<CashTemplate>();
                cashTemplates.AddRange(lst);
                cashTemplates = cashTemplates.OrderBy(x => x.CashTemplateNo).ToList();
            }
            if (!await GabanaAPI.CheckNetWork())
            {
                cashTemplates = new List<CashTemplate>();
                cashTemplates = await CashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
            }

            cashTemplates = cashTemplates.OrderBy(x => x.Amount).ToList();

            

            if (CashGuideCollectionView != null)
            {
                var data = CashGuideCollectionView?.DataSource as CashGuideDataSource;
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
                ((CashGuideDataSource)CashGuideCollectionView.DataSource).ReloadData(cashTemplates);
                CashGuideCollectionView.ReloadData();
            }

            GabanaLoading.SharedInstance.Hide();
        }
        public async override void ViewDidLoad()
        {
            
            

            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            LoginType = Preferences.Get("LoginType", "");

            lblCash = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCash.TextColor = UIColor.FromRGB(247, 86, 0);
            lblCash.Font = lblCash.Font.WithSize(15);
            lblCash.Text = Utils.TextBundle("cash", "Cash");
            lblCash.TextAlignment = UITextAlignment.Left;
            View.AddSubview(lblCash);


            lblMax = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblMax.TextColor = UIColor.FromRGB(200, 200, 200);
            lblMax.Font = lblMax.Font.WithSize(15);
            lblMax.TextAlignment = UITextAlignment.Left;
            lblMax.Text = Utils.TextBundle("maximum", "Maximum")+" 8";
            View.AddSubview(lblMax);


            #region CurrencyCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width)+80, height: 80);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayoutList.MinimumInteritemSpacing = 1;
            itemflowLayoutList.MinimumLineSpacing = 1;

            CashGuideCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            CashGuideCollectionView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            CashGuideCollectionView.ShowsVerticalScrollIndicator = false;
            CashGuideCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            CashGuideCollectionView.RegisterClassForCell(cellType: typeof(CashGuideViewCell), reuseIdentifier: "CashGuideViewCell");


            CashGuideDataSource cashGuideDataSource = new CashGuideDataSource(cashTemplates);
            cashGuideDataSource.OnCardCellDelete += CashGuideDataSource_OnCardCellDelete; ;
            CashGuideCollectionView.DataSource = cashGuideDataSource;
            
            CashGuideCollectionDelegate CashGuideCollectionDelegate = new CashGuideCollectionDelegate();
            CashGuideCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "update", "cash");
                if (check)
                {
                    var x = (int)indexPath.Item;
                    var add = new AddCashController(cashTemplates[x]);
                    this.NavigationController.PushViewController(add, false);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
                }
            };
            CashGuideCollectionView.Delegate = CashGuideCollectionDelegate;
            View.AddSubview(CashGuideCollectionView);
            #endregion
            #region BottomView
            

            btnAdd = new UIImageView();
            btnAdd.Image = UIImage.FromBundle("Add");
            btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "cash");
            if (!check)
            {
                btnAdd.Alpha = 0.5f;
            }
            btnAdd.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("ADD:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAdd.AddGestureRecognizer(tapGesture);
            View.AddSubview(btnAdd);
            #endregion
            SetupAutoLayout();

            var refreshControl = new UIRefreshControl();
            refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
            refreshControl.AddTarget(async (obj, sender) => {
                if (await GabanaAPI.CheckNetWork())
                {
                    await notificationManager.CashTemplateChange();
                }
                cashTemplates = await CashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                cashTemplates = cashTemplates.OrderBy(x => x.Amount).ToList();
                ((CashGuideDataSource)CashGuideCollectionView.DataSource).ReloadData(cashTemplates);
                CashGuideCollectionView.ReloadData();
                refreshControl.EndRefreshing();
            }, UIControlEvent.ValueChanged);
            CashGuideCollectionView.AlwaysBounceVertical = true;
            CashGuideCollectionView.AddSubview(refreshControl);
        }

        private void CashGuideDataSource_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                var check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "cash");
                if (check)
                {
               
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodeletecash", "Are you sure you want to delete cash?"), UIAlertControllerStyle.Alert);

                    //Add Actions
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => DeleteCash(indexPath)));
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
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูล Customer ได้");
                return;
            }
        }

        private async void DeleteCash(NSIndexPath indexPath)
        {

            try
            {
                List<Gabana.ORM.Master.CashTemplate> CashTemplateData = new List<Gabana.ORM.Master.CashTemplate>();

                var configlocal = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.CashTemplate, Gabana.ORM.Master.CashTemplate>();
                });

                var Imapperlocal = configlocal.CreateMapper();
                var Branchlocal = Imapperlocal.Map<ORM.MerchantDB.CashTemplate, Gabana.ORM.Master.CashTemplate>(cashTemplates[indexPath.Row]);
                CashTemplateData.Add(Branchlocal);
                //var CashTemplateData = cashTemplates[indexPath.Row];
                if (CashTemplateData != null)
                {
                    var DeleteonCloud = await GabanaAPI.DeleteDataCashTemplate(CashTemplateData);
                    if (DeleteonCloud == null)
                    {
                        DeleteonCloud = await GabanaAPI.DeleteDataCashTemplate(CashTemplateData);
                    }
                    if (!DeleteonCloud.Status)
                    {
                        Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                        return;
                    }
                    if (DeleteonCloud.Status)
                    {
                        CashTemplateManage cashTemplateManage = new CashTemplateManage();
                        var data = await cashTemplateManage.DeleteCashTemplate(DataCashingAll.MerchantId, CashTemplateData[0].CashTemplateNo);
                        if (!data)
                        {
                            Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                            return;
                        }
                        if (data)
                        {
                            Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
                            cashTemplates = await CashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                            cashTemplates = cashTemplates.OrderBy(x => x.Amount).ToList();
                            ((CashGuideDataSource)CashGuideCollectionView.DataSource).ReloadData(cashTemplates);
                            CashGuideCollectionView.ReloadData();
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

        [Export("ADD:")]
        public void ADD(UIGestureRecognizer sender)
        {
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "cash");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("addcash", "Add Cash"));
                var add = new AddCashController();
                this.NavigationController.PushViewController(add, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        void SetupAutoLayout()
        {
            lblCash.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            lblCash.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblCash.HeightAnchor.ConstraintEqualTo(30).Active = true;
            //lblCash.WidthAnchor.ConstraintEqualTo(80).Active = true;

            lblMax.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            lblMax.LeftAnchor.ConstraintEqualTo(lblCash.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            //lblMax.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblMax.HeightAnchor.ConstraintEqualTo(30).Active = true;

            CashGuideCollectionView.TopAnchor.ConstraintEqualTo(lblCash.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CashGuideCollectionView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CashGuideCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CashGuideCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            
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