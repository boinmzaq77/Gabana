using CoreFoundation;
using Foundation;
using Gabana.iOS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using GlobalToast;
using Newtonsoft.Json;
using Plugin.InAppBilling;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.AppSetting
{
    public class PackageController : UIViewController
    {
        private UICollectionViewFlowLayout PackageflowLayout;
        private UICollectionView PackageCollectionview;
        private PackageDataSource PackageData;
        private PackageCollectionDelegate packageCollectionDelegate;
        private UILabel lblpackage;
        private UILabel lblcomment;
        private UILabel lblpackageanother;
        private UIView pack1View;
        private UIView pack2View;
        private UILabel lblpackname1;
        private UIImageView logo1;
        private UILabel lblpackdetail1;
        private UIView packdetail1View;
        private UILabel lblpayat1;
        private UIButton btnchoosepack1;
        private UILabel lblpackname2;
        private UIImageView logo2;
        private UILabel lblpackdetail2;
        private UIView packdetail2View;
        private UILabel lblpayat2;
        private UIButton btnchoosepack2;
        private UILabel lblpromotion;
        private UIView promotionView;
        private UILabel lblpromotioncode;
        private UIButton btnSelectpromotion;
        BranchManage branchManage = new BranchManage();
        string[] ProductId = new string[]
                {
                "01",
                "02",
                "03",
                "04"
                };

        List<PackageProduce> packages = new List<PackageProduce>();
        private UIScrollView _scrollView;
        private UIView _contentView;
        MerchantConfigManage configManage = new MerchantConfigManage();
        private string SubscripttionType;
        private List<ORM.MerchantDB.Branch> lstBranch;
        private PackageProduce Produce;
        private InAppBillingPurchase inAppBillingPurchase;
        public static int PackageIdSelected;
        private string HistoryPurchase;
        private InAppBillingPurchase purchases;

        public PackageController()
        {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            //this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
        }
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
            this.inAppBillingPurchase = MainController.inAppBillingPurchase;
            if (inAppBillingPurchase == null)
            {
                PackageIdSelected = 1;
                //lnSubScription.Visibility = Android.Views.ViewStates.Visible;
                //lnChangePackage.Visibility = Android.Views.ViewStates.Gone;
                //lnContact.Visibility = Android.Views.ViewStates.Gone;
            }
            else
            {
                HistoryPurchase = inAppBillingPurchase.ProductId;
                //lnSubScription.Visibility = Android.Views.ViewStates.Gone;
                //lnChangePackage.Visibility = Android.Views.ViewStates.Gone;
                //lnContact.Visibility = Android.Views.ViewStates.Visible;

            }

            packages = await GetListPackage();
            
            if (packages.Count > 0)
            {
                Produce = packages[0];
            }
            initAttribute();
            SetupAutoLayout();
            SubscripttionType = DataCashingAll.setmerchantConfig.SUBSCRIPTION_TYPE;
            lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
            // Perform any additional setup after loading the view
        }

        private async Task<List<PackageProduce>> GetListPackage()
        {
            try
            {
                var connect = await CrossInAppBilling.Current.ConnectAsync();
                if (!connect)
                {
                    return new List<PackageProduce>();
                }
                var product = await CrossInAppBilling.Current.GetProductInfoAsync(itemType: ItemType.Subscription, ProductId);
                packages = new List<PackageProduce>();
                foreach (var item in product)
                {
                    var detail = Utils.SetDetailPackage(Int32.Parse(item.ProductId).ToString());
                    PackageProduce package = new PackageProduce();
                    package.id = Int32.Parse(item.ProductId);
                    package.ProductId = item.ProductId;
                    package.PackageName = item.ProductId;
                    package.MaxBranch = detail[1] + " " + "สาขา";
                    package.MaxUser = detail[0] + " " + "User"; 
                    package.Price = item.LocalizedPrice;
                    if (HistoryPurchase == item.ProductId)
                    {
                        PackageIdSelected = Int32.Parse(item.ProductId);
                    }

                    packages.Add(package);
                }
                packages = packages.OrderBy(x => x.id).ToList();

                return packages;
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                return new List<PackageProduce>();
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync();
                
            }
        }
        private void initAttribute()
        {
            try
            {
                

                _scrollView = new UIScrollView();
                _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
                _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                _contentView = new UIView();
                _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
                _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                //lblpackage = new UILabel();
            
                //lblpackage.Font = lblpackage.Font.WithSize(15);
                //lblpackage.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                //lblpackage.Lines = 1;
                //lblpackage.Text = "แพ็กเกจรายเดือน";
                //lblpackage.TextAlignment = UITextAlignment.Left;
                //lblpackage.TranslatesAutoresizingMaskIntoConstraints = false;
                //_contentView.AddSubview(lblpackage);

                //#region ItemPOSGridCollectionview

                //PackageflowLayout = new UICollectionViewFlowLayout();
                //PackageflowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                //PackageflowLayout.SectionInset = new UIEdgeInsets(top: 5, left: 5, bottom: 5, right: 5);
                //PackageflowLayout.ItemSize = new CoreGraphics.CGSize(width: ((View.Frame.Width - 20) / 2) - 20, height: (((View.Frame.Width - 20) / 2) - 20));

                //PackageCollectionview = new UICollectionView(frame: View.Frame, layout: PackageflowLayout);
                //PackageCollectionview.Hidden = false;
                //PackageCollectionview.BackgroundColor = UIColor.White;
                //PackageCollectionview.ShowsVerticalScrollIndicator = false;
                //PackageCollectionview.TranslatesAutoresizingMaskIntoConstraints = false;

                //PackageCollectionview.RegisterClassForCell(cellType: typeof(PackageCollectionViewCell), reuseIdentifier: "PackageCollectionViewCell");

                //PackageData = new PackageDataSource(packages);
                //PackageCollectionview.DataSource = PackageData;
                //PackageCollectionview.BackgroundColor = UIColor.Clear;
                //packageCollectionDelegate = new PackageCollectionDelegate();
                //packageCollectionDelegate.OnItemSelected += async (indexPath) => 
                //{
                //    SelectPackage(packages[indexPath.Row]);
                //};
                //PackageCollectionview.Delegate = packageCollectionDelegate;
                //_contentView.AddSubview(PackageCollectionview);

                //#endregion

                //lblcomment = new UILabel();
                //lblcomment.Font = lblcomment.Font.WithSize(12);
                //lblcomment.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                //lblcomment.Lines = 2;
                //lblcomment.Text = "การต่ออายุรายเดือนผ่านแอปพลิเคชันจะเป็นการต่ออัตโนมัติ และสามารถยกเลิกได้ตลอดเวลา";
                //lblcomment.TextAlignment = UITextAlignment.Left;
                //lblcomment.TranslatesAutoresizingMaskIntoConstraints = false;
                //_contentView.AddSubview(lblcomment);

                //lblpackageanother = new UILabel();
                //lblpackageanother.Font = lblpackageanother.Font.WithSize(15);
                //lblpackageanother.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                //lblpackageanother.Lines = 1;
                //lblpackageanother.Text = "แพ็กเกจอื่นๆ";
                //lblpackageanother.TextAlignment = UITextAlignment.Left;
                //lblpackageanother.TranslatesAutoresizingMaskIntoConstraints = false;
                //_contentView.AddSubview(lblpackageanother);

                //#region pack1View
                //pack1View = new UIView();
                //pack1View.BackgroundColor = UIColor.White;
                //pack1View.TranslatesAutoresizingMaskIntoConstraints = false;
                //_contentView.AddSubview(pack1View);

                //lblpackname1 = new UILabel();
                //lblpackname1.Font = lblpackname1.Font.WithSize(15);
                //lblpackname1.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                //lblpackname1.Lines = 1;
                //lblpackname1.Text = "Package 5";
                //lblpackname1.TextAlignment = UITextAlignment.Left;
                //lblpackname1.TranslatesAutoresizingMaskIntoConstraints = false;
                //pack1View.AddSubview(lblpackname1);

                //logo1 = new UIImageView();
                //logo1.TranslatesAutoresizingMaskIntoConstraints = false;
                //logo1.Image = UIImage.FromBundle("GabanaMain.png");

                //pack1View.AddSubview(logo1);

                //lblpackdetail1 = new UILabel();
                //lblpackdetail1.Font = lblpackdetail1.Font.WithSize(12);
                //lblpackdetail1.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                //lblpackdetail1.Lines = 1;
                //lblpackdetail1.Text = "ราย 6 เดือน";
                //lblpackdetail1.TextAlignment = UITextAlignment.Left;
                //lblpackdetail1.TranslatesAutoresizingMaskIntoConstraints = false;
                //pack1View.AddSubview(lblpackdetail1);

                //packdetail1View = new UIView();
                //packdetail1View.BackgroundColor = UIColor.FromRGB(232, 232, 232);

                //packdetail1View.TranslatesAutoresizingMaskIntoConstraints = false;
                //pack1View.AddSubview(packdetail1View);

                //lblpayat1 = new UILabel();
                //lblpayat1.Font = lblpayat1.Font.WithSize(12);
                //lblpayat1.TextColor = UIColor.FromRGB(162, 162, 162);
                //lblpayat1.Lines = 2;
                //lblpayat1.Text = "Pay at SeniorSoft";
                //lblpayat1.TextAlignment = UITextAlignment.Left;
                //lblpayat1.TranslatesAutoresizingMaskIntoConstraints = false;
                //packdetail1View.AddSubview(lblpayat1);

                //btnchoosepack1 = new UIButton();
                //btnchoosepack1.Layer.CornerRadius = 10f;
                //btnchoosepack1.Layer.BorderWidth = 0.5f;
                //btnchoosepack1.SetTitle("เลือก", UIControlState.Normal);
                //btnchoosepack1.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                //btnchoosepack1.BackgroundColor =  UIColor.FromRGB(0, 149, 218);
                //btnchoosepack1.TranslatesAutoresizingMaskIntoConstraints = false;
                //btnchoosepack1.TouchUpInside += (sender, e) => {

                
                //};
                //packdetail1View.AddSubview(btnchoosepack1);

                //#endregion

                //#region pack2View
                //pack2View = new UIView();
                //pack2View.BackgroundColor = UIColor.White;
                //pack2View.TranslatesAutoresizingMaskIntoConstraints = false;
                //_contentView.AddSubview(pack2View);

                //lblpackname2 = new UILabel();
                //lblpackname2.Font = lblpackname2.Font.WithSize(15);
                //lblpackname2.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                //lblpackname2.Lines = 1;
                //lblpackname2.Text = "Package 6";
                //lblpackname2.TextAlignment = UITextAlignment.Left;
                //lblpackname2.TranslatesAutoresizingMaskIntoConstraints = false;
                //pack2View.AddSubview(lblpackname2);

                //logo2 = new UIImageView();
                //logo2.TranslatesAutoresizingMaskIntoConstraints = false;

                //logo2.Image = UIImage.FromBundle("GabanaMain.png");

                //pack2View.AddSubview(logo2);

                //lblpackdetail2 = new UILabel();
                
                //lblpackdetail2.Font = lblpackdetail2.Font.WithSize(12);
                //lblpackdetail2.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                //lblpackdetail2.Lines = 1;
                //lblpackdetail2.Text = "ราย 1 ปี ";
                //lblpackdetail2.TextAlignment = UITextAlignment.Left;
                //lblpackdetail2.TranslatesAutoresizingMaskIntoConstraints = false;
                //pack2View.AddSubview(lblpackdetail2);

                //packdetail2View = new UIView();
                //packdetail2View.BackgroundColor = UIColor.FromRGB(232, 232, 232);
                //packdetail2View.TranslatesAutoresizingMaskIntoConstraints = false;
                //pack2View.AddSubview(packdetail2View);

                //lblpayat2 = new UILabel();
                //lblpayat2.Font = lblpayat2.Font.WithSize(12);
                //lblpayat2.TextColor = UIColor.FromRGB(162, 162, 162);
                //lblpayat2.Lines = 1;
                //lblpayat2.Text = "Pay at SeniorSoft";
                //lblpayat2.TextAlignment = UITextAlignment.Left;
                //lblpayat2.TranslatesAutoresizingMaskIntoConstraints = false;
                //packdetail2View.AddSubview(lblpayat2);

                //btnchoosepack2 = new UIButton();
                //btnchoosepack2.Layer.CornerRadius = 10f;
                //btnchoosepack2.Layer.BorderWidth = 0.5f;
                //btnchoosepack2.SetTitle("เลือก", UIControlState.Normal);
                //btnchoosepack2.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                //btnchoosepack2.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                //btnchoosepack2.TranslatesAutoresizingMaskIntoConstraints = false;
                //btnchoosepack2.TouchUpInside += (sender, e) => {


                //};
                //packdetail2View.AddSubview(btnchoosepack2);

                //#endregion

                #region promotionView

                lblpromotion = new UILabel();
                lblpromotion.Font = lblpromotion.Font.WithSize(15);
                lblpromotion.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                lblpromotion.Lines = 1;
                lblpromotion.TextAlignment = UITextAlignment.Center;
                lblpromotion.TranslatesAutoresizingMaskIntoConstraints = false;
                _contentView.AddSubview(lblpromotion);


                promotionView = new UIView();
                promotionView.BackgroundColor = UIColor.White;
                promotionView.TranslatesAutoresizingMaskIntoConstraints = false;
                _contentView.AddSubview(promotionView);

                lblpromotioncode = new UILabel();
                lblpromotioncode.Font = lblpromotioncode.Font.WithSize(15);
                lblpromotioncode.TextColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1f);
                lblpromotioncode.Lines = 1;
                lblpromotioncode.TextAlignment = UITextAlignment.Center;
                lblpromotioncode.TranslatesAutoresizingMaskIntoConstraints = false;
                promotionView.AddSubview(lblpromotioncode);

                btnSelectpromotion = new UIButton();
                btnSelectpromotion.SetBackgroundImage(UIImage.FromBundle("Next"), UIControlState.Normal);
                btnSelectpromotion.TranslatesAutoresizingMaskIntoConstraints = false;
                promotionView.AddSubview(btnSelectpromotion);
                #endregion

                
                
                _scrollView.AddSubview(_contentView);

                View.AddSubview(_scrollView);
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
            }
        }
        public async void SelectPackage(PackageProduce packages)
        {
            try
            {
                //GabanaLoading.SharedInstance.Show(this);
                #region PresentCode
                //switch (SubscripttionType)
                //{
                //    case "P":
                //    case "F":
                //    case "U":
                //        break;
                //    case "A":
                //        //case sub_type คนละตัวกับระบบปฏิบัติการ แจ้งเตือน dialog ให้ unsub ก่อน return; 

                //        Utils.ShowAlert(this, "Error !", "ไม่สามารถสมัครรับการใช้งานได้ กรุณา Unsub ก่อน");
                //        return;
                //    case "B":
                //        //เปิด webview สำหรับต่อที่หลังบ้าน

                //        return;
                //    default:
                //        break;
                //}

                //if (Produce != null) PackageIdSelected = packages.id;
                //Produce = packages;
                //{
                //    var connect = await CrossInAppBilling.Current.ConnectAsync();

                //    var purchaseHistory = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);

                //    if (SubscripttionType == "P")
                //    {
                //        //เช็คว่ามีประวัติการซื้อแพ็กเกจที่เครื่องไหม
                //        if (purchaseHistory == null || purchaseHistory.FirstOrDefault() == null)
                //        {
                //            //มีข้อมูลการซื้อที่หลังบ้าน
                //            //purchaseHistory = null
                //            //กรณีต่ออายุผ่าน store แล้วต้องการเปลี่ยน account ในการ subscription
                //            //มีที่หลังบ้าน SUBSCRIPTION_TYPE = P แต่ไม่มี purchaseHistory = null
                //            //เกิดจาก ใช้คนละอีเมลในการ subscript ที่ play store  .... 

                //            //TextError = GetString(Resource.String.package_activity_changemail);
                //            //MainDialog dialog = new MainDialog();
                //            //Bundle bundle = new Bundle();
                //            //String myMessage = Resource.Layout.package_dialog_error.ToString();
                //            //bundle.PutString("message", myMessage);
                //            //dialog.Arguments = bundle;
                //            //dialog.Show(SupportFragmentManager, myMessage);
                //            return;
                //        }
                //        else
                //        {
                //            //กรณีมีการซื้อจากเครื่องเดิม
                //            //google account เดียวกัน ต่างร้านค้า
                //            var GetPurchase = purchaseHistory.Where(x => x.State == Plugin.InAppBilling.PurchaseState.Purchased && !string.IsNullOrEmpty(x.ObfuscatedAccountId) && x.ObfuscatedAccountId != DataCashingAll.MerchantId.ToString()).OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault();
                //            if (GetPurchase != null)
                //            {
                //                //TextError = GetString(Resource.String.package_activity_sub1) + " " + GetPurchase.ObfuscatedAccountId + " " +
                //                //   GetString(Resource.String.package_activity_sub2) + " " + DataCashingAll.MerchantId
                //                //   + " " + GetString(Resource.String.package_activity_sub3);
                //                //MainDialog dialog = new MainDialog();
                //                //Bundle bundle = new Bundle();
                //                //String myMessage = Resource.Layout.package_dialog_error.ToString();
                //                //bundle.PutString("message", myMessage);
                //                //dialog.Arguments = bundle;
                //                //dialog.Show(SupportFragmentManager, myMessage);
                //                return;
                //            }

                //            //GetInfo ปัจจุบัน
                //            var CurrentPackagePurchase = purchaseHistory.Where(x => x.State == Plugin.InAppBilling.PurchaseState.Purchased && !string.IsNullOrEmpty(x.ObfuscatedAccountId) && x.ObfuscatedAccountId == DataCashingAll.MerchantId.ToString()).OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault();
                //            bool hasCurrentPackage = await CheckCurrentPackage(CurrentPackagePurchase, Produce);
                //            if (hasCurrentPackage)
                //            {
                //                return;
                //            }
                //        }
                //    }


                //    //------- ทำการซื้อแพ็กเกจแล้วส่งขึ้นไปบันทึกที่ Cloud --------
                //    //purchaseHistory == null 
                //    //SubscripttionType == "F" , SubscripttionType == "U" 
                //    //SubscripttionType == "P" && ข้อมูลหลังบ้านไม่มี 

                //    purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                //    //var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                //    if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                //    {


                //        //var state = ConvertPurchaseState(purchases.State);
                //        //if (state == 0)
                //        //{
                //        //    var ConsumptionState = ConvertPurchaseConsumptionState(purchases.ConsumptionState);
                //        //    RenewModel renewModel = new RenewModel
                //        //    {
                //        //        Id = purchases.Id,
                //        //        TransactionDateUtc = purchases.TransactionDateUtc,
                //        //        ProductId = purchases.ProductId,
                //        //        Quantity = purchases.Quantity,
                //        //        ProductIds = purchases.ProductIds.ToList(),
                //        //        AutoRenewing = purchases.AutoRenewing,
                //        //        PurchaseToken = purchases.PurchaseToken,
                //        //        State = state,
                //        //        ConsumptionState = ConsumptionState,
                //        //        IsAcknowledged = purchases.IsAcknowledged,
                //        //        ObfuscatedAccountId = DataCashingAll.MerchantId.ToString(),
                //        //        ObfuscatedProfileId = purchases.ObfuscatedProfileId,
                //        //        Payload = purchases.Payload,
                //        //        OriginalJson = purchases.OriginalJson,
                //        //        Signature = purchases.Signature,
                //        //    };

                //        //    var PurchasePackage = await GabanaAPI.PutDataPackage(renewModel);
                //        //    if (PurchasePackage.Status)
                //        //    {
                //        //        PutData();
                //        //    }
                //        //    else
                //        //    {
                //        //        //กรณีซื้อไม่สำเร็จ
                //        //        //ส่งใหม่ dialog refresh เพื่อส่งข้อมูลใหม่
                //        //        //MainDialog dialog = new MainDialog();
                //        //        //Bundle bundle = new Bundle();
                //        //        //String myMessage = Resource.Layout.package_dialog_refresh.ToString();
                //        //        //bundle.PutString("message", myMessage);
                //        //        //dialog.Arguments = bundle;
                //        //        //dialog.Show(SupportFragmentManager, myMessage);
                //        //    }
                //        //}
                //    }
                //    else
                //    {
                //        //var x = await CrossInAppBilling.Current.FinalizePurchaseAsync(purchases.TransactionIdentifier);
                //        Utils.ShowMessage(purchases.State.ToString());
                //    }
                //}
                #endregion
                if (SubscripttionType == "P")
                {
                    //case sub_type คนละตัวกับระบบปฏิบัติการ แจ้งเตือน dialog ให้ unsub ก่อน return; 
                    //TextError = "";
                    //MainDialog dialog = new MainDialog();
                    //Bundle bundle = new Bundle();
                    //String myMessage = Resource.Layout.package_dialog_error.ToString();
                    //bundle.PutString("message", myMessage);
                    //dialog.Arguments = bundle;
                    //dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
                else if (SubscripttionType == "B")
                {
                    //เปิด webview สำหรับต่อที่หลังบ้าน
                    return;
                }
                else //เคส P,U,F
                {
                    if (Produce != null) PackageIdSelected = packages.id;
                    Produce = packages;
                    {
                        var connect = await CrossInAppBilling.Current.ConnectAsync();
                        var purchaseHistory = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.Subscription);

                        //สมัครแพ็กเกจครั้งแรก ไม่มีประวัติการสมัครโดยใช้อีเมลที่เครื่องไหน หรือร้านค้าไหน
                        if (purchaseHistory == null || purchaseHistory.FirstOrDefault() == null)
                        {
                            //เคส A
                            if (SubscripttionType == "A")
                            {
                                //TextError = GetString(Resource.String.package_activity_changemail);
                                //MainDialog dialog = new MainDialog();
                                //Bundle bundle = new Bundle();
                                //String myMessage = Resource.Layout.package_dialog_error.ToString();
                                //bundle.PutString("message", myMessage);
                                //dialog.Arguments = bundle;
                                //dialog.Show(SupportFragmentManager, myMessage);
                                return;
                            }
                            else //เคส F,U
                            {
                                purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                                //var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                                if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                                {
                                    var renew = new RenewiOS()
                                    {
                                        MerchantID = DataCashingAll.MerchantId.ToString(),
                                        OriginalTransactionID = purchases.TransactionIdentifier.ToString()
                                    };
                                    var status = await GabanaAPI.RenewiOS(renew);

                                    //
                                }
                                return;
                            }
                        }
                        else //มีประวัติการสมัครสมาชิก
                        {
                            // merchantID ตรงกับร้านค้าปัจจุบัน
                            if (purchaseHistory.FirstOrDefault().ObfuscatedAccountId == DataCashingAll.MerchantId.ToString())
                            {
                                //มีการ Subscription อยู่ Auto Renew == true
                                if (purchaseHistory.FirstOrDefault().AutoRenewing == true)
                                {
                                    bool hasCurrentPackage = await CheckCurrentPackage(purchaseHistory.OrderByDescending(x => x.TransactionDateUtc).FirstOrDefault(), Produce);
                                    if (hasCurrentPackage)
                                    {
                                        return;
                                    }
                                }
                                else  //ไม่มีการ SubscriptionAuto Renew == false
                                {
                                    purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                                    //var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                                    if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                                    {
                                        var renew = new RenewiOS()
                                        {
                                            MerchantID = DataCashingAll.MerchantId.ToString(),
                                            OriginalTransactionID = purchases.TransactionIdentifier.ToString()
                                        };
                                        var status = await GabanaAPI.RenewiOS(renew);
                                        //var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);
                                        //DialogLoading dialogLoading = new DialogLoading();
                                        //if (dialogLoading.Cancelable != false)
                                        //{
                                        //    dialogLoading.Cancelable = false;
                                        //    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                                        //}

                                        ////Dialog
                                        ////เปลี่ยนแพ็กเกจสำเร็จ
                                        //MainDialog dialog = new MainDialog();
                                        //Bundle bundle = new Bundle();
                                        //String myMessage = Resource.Layout.offline_dialog_main.ToString();
                                        //bundle.PutString("message", myMessage);
                                        //bundle.PutString("SubscriptSuccess", "SubscriptSuccess");
                                        //dialog.Arguments = bundle;
                                        //dialog.Show(SupportFragmentManager, myMessage);

                                        //if (dialogLoading != null)
                                        //{
                                        //    dialogLoading.DismissAllowingStateLoss();
                                        //    dialogLoading.Dismiss();
                                        //    dialogLoading = new DialogLoading();
                                        //}
                                    }
                                    return;
                                }
                            }
                            else // merchantID ไม่ตรงกับร้านค้าปัจจุบัน
                            {
                                //มีการ Subscription อยู่ Auto Renew == true
                                if (purchaseHistory.FirstOrDefault().AutoRenewing == true)
                                {
                                   // TextError = GetString(Resource.String.package_activity_sub1) + " " + purchaseHistory.FirstOrDefault().ObfuscatedAccountId + " " +
                                   //GetString(Resource.String.package_activity_sub2) + " " + DataCashingAll.MerchantId
                                   //+ " " + GetString(Resource.String.package_activity_sub3);
                                   // MainDialog dialog = new MainDialog();
                                   // Bundle bundle = new Bundle();
                                   // String myMessage = Resource.Layout.package_dialog_error.ToString();
                                   // bundle.PutString("message", myMessage);
                                   // dialog.Arguments = bundle;
                                   // dialog.Show(SupportFragmentManager, myMessage);
                                    return;
                                }
                                else //ไม่มีการ SubscriptionAuto Renew == false
                                {
                                    //เคส P
                                    if (SubscripttionType == "A")
                                    {
                                        //TextError = GetString(Resource.String.package_activity_changemail);
                                        //MainDialog dialog = new MainDialog();
                                        //Bundle bundle = new Bundle();
                                        //String myMessage = Resource.Layout.package_dialog_error.ToString();
                                        //bundle.PutString("message", myMessage);
                                        //dialog.Arguments = bundle;
                                        //dialog.Show(SupportFragmentManager, myMessage);
                                        return;
                                    }
                                    else //เคส F,U
                                    {
                                        purchases = await CrossInAppBilling.Current.PurchaseAsync(Produce.ProductId, ItemType.Subscription, DataCashingAll.MerchantId.ToString());
                                        //var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(purchases.PurchaseToken);
                                        if (purchases.State == Plugin.InAppBilling.PurchaseState.Purchased)
                                        {

                                            var renew = new RenewiOS()
                                            {
                                                MerchantID = DataCashingAll.MerchantId.ToString(),
                                                OriginalTransactionID = purchases.TransactionIdentifier.ToString()
                                            };
                                            var status = await GabanaAPI.RenewiOS(renew);

                                            //var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);
                                            //DialogLoading dialogLoading = new DialogLoading();
                                            //if (dialogLoading.Cancelable != false)
                                            //{
                                            //    dialogLoading.Cancelable = false;
                                            //    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                                            //}

                                            ////Dialog
                                            ////เปลี่ยนแพ็กเกจสำเร็จ
                                            //MainDialog dialog = new MainDialog();
                                            //Bundle bundle = new Bundle();
                                            //String myMessage = Resource.Layout.offline_dialog_main.ToString();
                                            //bundle.PutString("message", myMessage);
                                            //bundle.PutString("SubscriptSuccess", "SubscriptSuccess");
                                            //dialog.Arguments = bundle;
                                            //dialog.Show(SupportFragmentManager, myMessage);

                                            //if (dialogLoading != null)
                                            //{
                                            //    dialogLoading.DismissAllowingStateLoss();
                                            //    dialogLoading.Dismiss();
                                            //    dialogLoading = new DialogLoading();
                                            //}
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (InAppBillingPurchaseException purchaseEx)
            {
                Utils.ShowMessage(purchaseEx.Message);
                //Toast.MakeText(this, purchaseEx.Message, ToastLength.Short).Show();
                return;
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
            finally
            {
                GabanaLoading.SharedInstance.Hide();
            }
        }
        private async void PutData()
        {
            try
            {
                List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                var merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SUBSCRIPTION_TYPE",
                    CfgString = "P"
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (update.Status)
                {
                    //Insert to Local DB
                    MerchantConfig localConfig = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "SUBSCRIPTION_TYPE",
                        CfgString = "P"
                    };

                    var localVAT = await configManage.InsertorReplacrMerchantConfig(localConfig);
                    if (localVAT)
                    {
                        DataCashingAll.setmerchantConfig.SUBSCRIPTION_TYPE = "P";
                    }
                }
                else
                {
                    Utils.ShowMessage(update.Message);
                    //Toast.MakeText(this, update.Message, ToastLength.Short).Show();
                }

                //var verify = await CrossInAppBilling.Current.FinishTransaction(purchases);
                //Dialog
                //เปลี่ยนแพ็กเกจสำเร็จ

                //MainDialog dialog = new MainDialog();
                //Bundle bundle = new Bundle();
                //String myMessage = Resource.Layout.offline_dialog_main.ToString();
                //bundle.PutString("message", myMessage);
                //bundle.PutString("SubscriptSuccess", "SubscriptSuccess");
                //dialog.Arguments = bundle;
                //dialog.Show(SupportFragmentManager, myMessage);
                Utils.ShowMessage("SubscriptSuccess");
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private int ConvertPurchaseConsumptionState(ConsumptionState consumptionState)
        {
            try
            {
                switch (consumptionState)
                {
                    case ConsumptionState.NoYetConsumed:
                        return 0;
                    case ConsumptionState.Consumed:
                        return 1;
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return 0;
            }
        }
        private int ConvertPurchaseState(Plugin.InAppBilling.PurchaseState state)
        {
            try
            {
                switch (state)
                {
                    case Plugin.InAppBilling.PurchaseState.Purchased:
                        return 0;
                    case Plugin.InAppBilling.PurchaseState.Canceled:
                        return 1;
                    case Plugin.InAppBilling.PurchaseState.Purchasing:
                        return 3;
                    case Plugin.InAppBilling.PurchaseState.Failed:
                        return 4;
                    case Plugin.InAppBilling.PurchaseState.Restored:
                        return 5;
                    case Plugin.InAppBilling.PurchaseState.Deferred:
                        return 6;
                    case Plugin.InAppBilling.PurchaseState.PaymentPending:
                        return 8;
                    case Plugin.InAppBilling.PurchaseState.Unknown:
                        return 10;
                    default:
                        return 0;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return 0;
            }
        }
        private async Task<bool> CheckCurrentPackage(InAppBillingPurchase purchaseCurrent, PackageProduce packageProduce)
        {
            try
            {
                var _purchaseCurrent = purchaseCurrent;
                if (_purchaseCurrent != null)
                {
                    InAppBillingPurchase UpgradePurchasedSub = new InAppBillingPurchase();

                    //เช็คจากสาขา ถ้าสาขาปัจจุบันมีสาขามากกว่า                     
                    GabanaInfo gabanaInfo = new GabanaInfo();
                    gabanaInfo = await GabanaAPI.GetDataGabanaInfo();
                    DataCashingAll.GetGabanaInfo = gabanaInfo;
                    var GabanaInfo = JsonConvert.SerializeObject(gabanaInfo);
                    Preferences.Set("GabanaInfo", GabanaInfo);

                    //get package ID
                    //id ปัจจุบัน , id ตัวที่เลือก
                    //เงื่อนไข มากว่า น้อยกว่า

                    int PackageIDCurrent = Utils.SetPackageID(DataCashingAll.GetGabanaInfo.TotalBranch, DataCashingAll.GetGabanaInfo.TotalUser);
                    int PackageSelect = Convert.ToInt32(packageProduce.ProductId);

                    if (PackageIDCurrent == -1)
                    {
                        //กรณี add on ที่มีการเพิ่มจากหลังบ้าน เนื่องจาก ไม่มี packageid = -1
                        return true;
                    }

                    if (PackageSelect < PackageIDCurrent)
                    {
                        //การ downgrade จะตรวจสอบก่อนว่า user และ branch มีมากกว่า package ที่ต้องการ downgrade หรือไม่ 
                        //หากมีจะไม่สามารถทำการ downgrade ได้ และมีข้อความแจ้งให้ไปทำการลบข้อมูลก่อน

                        //เช็คจำนวนสาขาปัจจุบัน ว่าน้อยกว่าหรือเท่ากับสาขาที่เลือกหรือไม่
                        List<string> detail = Utils.SetDetailPackage(packageProduce.ProductId);

                        if ((lstBranch.Count > Convert.ToInt32(detail[1])) && (DataCashingAll.UserAccountInfo.Count > Convert.ToInt32(detail[0])))
                        {

                            //TextError = Resources.GetString(Resource.String.package_activity_downgrade);
                            //MainDialog dialog = new MainDialog();
                            //Bundle bundle = new Bundle();
                            //String myMessage = Resource.Layout.package_dialog_error.ToString();
                            //bundle.PutString("message", myMessage);
                            //dialog.Arguments = bundle;
                            //dialog.Show(SupportFragmentManager, myMessage);
                            return true;
                        }

                        UpgradePurchasedSub = await CrossInAppBilling.Current.UpgradePurchasedSubscriptionAsync(packageProduce.ProductId, _purchaseCurrent.PurchaseToken, SubscriptionProrationMode.ImmediateWithoutProration);//Downgrade
                    }
                    else if (PackageIDCurrent == PackageSelect)
                    {
                        return true;
                    }
                    else
                    {
                        UpgradePurchasedSub = await CrossInAppBilling.Current.UpgradePurchasedSubscriptionAsync(packageProduce.ProductId, _purchaseCurrent.PurchaseToken, SubscriptionProrationMode.ImmediateAndChargeProratedPrice); //Upgrade
                    }


                    //var AcknowledgePurchase = await CrossInAppBilling.Current.AcknowledgePurchaseAsync(UpgradePurchasedSub.PurchaseToken);
                    if (UpgradePurchasedSub.State == Plugin.InAppBilling.PurchaseState.Purchased)
                    {
                        var state = ConvertPurchaseState(UpgradePurchasedSub.State);
                        if (state == 0)
                        {
                            var ConsumptionState = ConvertPurchaseConsumptionState(UpgradePurchasedSub.ConsumptionState);
                            RenewModel renewModel = new RenewModel
                            {
                                Id = UpgradePurchasedSub.Id,
                                TransactionDateUtc = UpgradePurchasedSub.TransactionDateUtc,
                                ProductId = UpgradePurchasedSub.ProductId,
                                Quantity = UpgradePurchasedSub.Quantity,
                                ProductIds = UpgradePurchasedSub.ProductIds.ToList(),
                                AutoRenewing = UpgradePurchasedSub.AutoRenewing,
                                PurchaseToken = UpgradePurchasedSub.PurchaseToken,
                                State = state,
                                ConsumptionState = ConsumptionState,
                                IsAcknowledged = UpgradePurchasedSub.IsAcknowledged,
                                ObfuscatedAccountId = UpgradePurchasedSub.ObfuscatedAccountId,
                                ObfuscatedProfileId = UpgradePurchasedSub.ObfuscatedProfileId,
                                Payload = UpgradePurchasedSub.Payload,
                                OriginalJson = UpgradePurchasedSub.OriginalJson,
                                Signature = UpgradePurchasedSub.Signature,
                            };

                            var PurchasePackage = await GabanaAPI.PutDataPackage(renewModel);
                            if (PurchasePackage.Status)
                            {
                                PutData();
                            }
                            else
                            {
                                //กรณีซื้อไม่สำเร็จ
                                //ส่งใหม่ dialog refresh เพื่อส่งข้อมูลใหม่
                                //MainDialog dialog = new MainDialog();
                                //Bundle bundle = new Bundle();
                                //String myMessage = Resource.Layout.package_dialog_refresh.ToString();
                                //bundle.PutString("message", myMessage);
                                //dialog.Arguments = bundle;
                                //dialog.Show(SupportFragmentManager, myMessage);
                                Utils.ShowMessage("ไม่สำเร็จ");
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return true;
            }
        }
        private void SetupAutoLayout()
        {

            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            //lblpackage.TopAnchor.ConstraintEqualTo(lblpackage.Superview.TopAnchor,10).Active = true;
            //lblpackage.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblpackage.LeftAnchor.ConstraintEqualTo(lblpackage.Superview.LeftAnchor, 10).Active = true;
            //lblpackage.RightAnchor.ConstraintEqualTo(lblpackage.Superview.RightAnchor, -10).Active = true;

            //PackageCollectionview.TopAnchor.ConstraintEqualTo(lblpackage.BottomAnchor, 10).Active = true;
            //PackageCollectionview.HeightAnchor.ConstraintEqualTo(350).Active = true;
            //PackageCollectionview.LeftAnchor.ConstraintEqualTo(PackageCollectionview.Superview.LeftAnchor, 10).Active = true;
            //PackageCollectionview.RightAnchor.ConstraintEqualTo(PackageCollectionview.Superview.RightAnchor, -10).Active = true;

            //lblcomment.TopAnchor.ConstraintEqualTo(PackageCollectionview.BottomAnchor, 10).Active = true;
            //lblcomment.HeightAnchor.ConstraintEqualTo(32).Active = true;
            //lblcomment.LeftAnchor.ConstraintEqualTo(lblcomment.Superview.LeftAnchor, 10).Active = true;
            //lblcomment.RightAnchor.ConstraintEqualTo(lblcomment.Superview.RightAnchor, -10).Active = true;

            //lblpackageanother.TopAnchor.ConstraintEqualTo(lblcomment.BottomAnchor, 10).Active = true;
            //lblpackageanother.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblpackageanother.LeftAnchor.ConstraintEqualTo(lblpackageanother.Superview.LeftAnchor, 10).Active = true;
            //lblpackageanother.RightAnchor.ConstraintEqualTo(lblpackageanother.Superview.RightAnchor, -10).Active = true;

            //pack1View.TopAnchor.ConstraintEqualTo(lblpackageanother.BottomAnchor, 10).Active = true;
            //pack1View.HeightAnchor.ConstraintEqualTo(120).Active = true;
            //pack1View.LeftAnchor.ConstraintEqualTo(PackageCollectionview.Superview.LeftAnchor, 10).Active = true;
            //pack1View.WidthAnchor.ConstraintEqualTo((View.Frame.Width-30)/2).Active = true;

            //lblpackname1.TopAnchor.ConstraintEqualTo(pack1View.TopAnchor, 10).Active = true;
            //lblpackname1.HeightAnchor.ConstraintEqualTo(23).Active = true;
            //lblpackname1.LeftAnchor.ConstraintEqualTo(pack1View.LeftAnchor, 10).Active = true;
            //lblpackname1.RightAnchor.ConstraintEqualTo(pack1View.RightAnchor, -10).Active = true;

            //logo1.TopAnchor.ConstraintEqualTo(lblpackname1.BottomAnchor, 10).Active = true;
            //logo1.HeightAnchor.ConstraintEqualTo(10).Active = true;
            //logo1.LeftAnchor.ConstraintEqualTo(pack1View.LeftAnchor, 10).Active = true;
            //logo1.WidthAnchor.ConstraintEqualTo(10).Active = true;

            //lblpackdetail1.CenterYAnchor.ConstraintEqualTo(logo1.CenterYAnchor).Active = true;
            //lblpackdetail1.HeightAnchor.ConstraintEqualTo(23).Active = true;
            //lblpackdetail1.LeftAnchor.ConstraintEqualTo(logo1.RightAnchor, 10).Active = true;
            //lblpackdetail1.RightAnchor.ConstraintEqualTo(pack1View.RightAnchor, -10).Active = true;

            //packdetail1View.BottomAnchor.ConstraintEqualTo(pack1View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            //packdetail1View.HeightAnchor.ConstraintEqualTo(50).Active = true;
            //packdetail1View.LeftAnchor.ConstraintEqualTo(pack1View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            //packdetail1View.RightAnchor.ConstraintEqualTo(pack1View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            //btnchoosepack1.CenterYAnchor.ConstraintEqualTo(packdetail1View.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            //btnchoosepack1.HeightAnchor.ConstraintEqualTo(30).Active = true;
            //btnchoosepack1.WidthAnchor.ConstraintEqualTo(50).Active = true;
            //btnchoosepack1.RightAnchor.ConstraintEqualTo(packdetail1View.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            //lblpayat1.TopAnchor.ConstraintEqualTo(packdetail1View.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            //lblpayat1.BottomAnchor.ConstraintEqualTo(packdetail1View.SafeAreaLayoutGuide.BottomAnchor, -2).Active = true;
            //lblpayat1.LeftAnchor.ConstraintEqualTo(packdetail1View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            //lblpayat1.RightAnchor.ConstraintEqualTo(btnchoosepack1.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;

            //pack2View.TopAnchor.ConstraintEqualTo(lblpackageanother.BottomAnchor, 10).Active = true;
            //pack2View.HeightAnchor.ConstraintEqualTo(120).Active = true;
            //pack2View.LeftAnchor.ConstraintEqualTo(pack1View.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            //pack2View.WidthAnchor.ConstraintEqualTo((View.Frame.Width - 30) / 2).Active = true;

            //lblpackname2.TopAnchor.ConstraintEqualTo(pack2View.TopAnchor, 10).Active = true;
            //lblpackname2.HeightAnchor.ConstraintEqualTo(23).Active = true;
            //lblpackname2.LeftAnchor.ConstraintEqualTo(pack2View.LeftAnchor, 10).Active = true;
            //lblpackname2.RightAnchor.ConstraintEqualTo(pack2View.RightAnchor, -10).Active = true;

            //logo2.TopAnchor.ConstraintEqualTo(lblpackname2.BottomAnchor, 10).Active = true;
            //logo2.HeightAnchor.ConstraintEqualTo(10).Active = true;
            //logo2.LeftAnchor.ConstraintEqualTo(pack2View.LeftAnchor, 10).Active = true;
            //logo2.WidthAnchor.ConstraintEqualTo(10).Active = true;

            //lblpackdetail2.CenterYAnchor.ConstraintEqualTo(logo2.CenterYAnchor).Active = true;
            //lblpackdetail2.HeightAnchor.ConstraintEqualTo(23).Active = true;
            //lblpackdetail2.LeftAnchor.ConstraintEqualTo(logo2.RightAnchor, 10).Active = true;
            //lblpackdetail2.RightAnchor.ConstraintEqualTo(pack2View.RightAnchor, -10).Active = true;

            //packdetail2View.BottomAnchor.ConstraintEqualTo(pack2View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            //packdetail2View.HeightAnchor.ConstraintEqualTo(50).Active = true;
            //packdetail2View.LeftAnchor.ConstraintEqualTo(pack2View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            //packdetail2View.RightAnchor.ConstraintEqualTo(pack2View.SafeAreaLayoutGuide.RightAnchor).Active = true;

            //btnchoosepack2.CenterYAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            //btnchoosepack2.HeightAnchor.ConstraintEqualTo(30).Active = true;
            //btnchoosepack2.WidthAnchor.ConstraintEqualTo(50).Active = true;
            //btnchoosepack2.RightAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            //lblpayat2.TopAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            //lblpayat2.BottomAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.BottomAnchor, -2).Active = true;
            //lblpayat2.LeftAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            //lblpayat2.RightAnchor.ConstraintEqualTo(btnchoosepack2.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;


            lblpromotion.TopAnchor.ConstraintEqualTo(lblpromotion.Superview.TopAnchor, 10).Active = true;
            lblpromotion.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblpromotion.LeftAnchor.ConstraintEqualTo(lblpromotion.Superview.LeftAnchor, 10).Active = true;
            lblpromotion.RightAnchor.ConstraintEqualTo(lblpromotion.Superview.RightAnchor, -10).Active = true;

            promotionView.TopAnchor.ConstraintEqualTo(lblpromotion.BottomAnchor, 2).Active = true;
            promotionView.BottomAnchor.ConstraintEqualTo(promotionView.Superview.BottomAnchor).Active = true;
            promotionView.LeftAnchor.ConstraintEqualTo(promotionView.Superview.LeftAnchor, 10).Active = true;
            promotionView.RightAnchor.ConstraintEqualTo(promotionView.Superview.RightAnchor, -10).Active = true;
            promotionView.HeightAnchor.ConstraintEqualTo(50).Active = true;

            lblpromotioncode.TopAnchor.ConstraintEqualTo(promotionView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblpromotioncode.BottomAnchor.ConstraintEqualTo(promotionView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            lblpromotioncode.LeftAnchor.ConstraintEqualTo(promotionView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            lblpromotioncode.RightAnchor.ConstraintEqualTo(promotionView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            btnSelectpromotion.CenterYAnchor.ConstraintEqualTo(promotionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnSelectpromotion.HeightAnchor.ConstraintEqualTo(30).Active = true;
            btnSelectpromotion.WidthAnchor.ConstraintEqualTo(30).Active = true;
            btnSelectpromotion.RightAnchor.ConstraintEqualTo(promotionView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            //btnSelectpromotion.TopAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            //btnSelectpromotion.BottomAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.BottomAnchor, -2).Active = true;
            //btnSelectpromotion.LeftAnchor.ConstraintEqualTo(packdetail2View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            //btnSelectpromotion.RightAnchor.ConstraintEqualTo(btnchoosepack2.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;


        }

        
    }
}