using Foundation;
using Gabana.AppSetting;
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
    public partial class SettingController : UIViewController
    {
        UIBarButtonItem backButton;
        UIView MerchantSettingView, MerSet_EmployeeView, ItemSettingView, BillSettingView, DeviceSettingView, _contentView;
        UILabel lblHeadMerchantSetting, lblHeadItemSetting, lblHeadBillSetting, lblHeadDeviceSetting;
        UIScrollView _scrollView;

        UIView MerSet_merView, MerSet_BranchView, MerSet_LinkProView, MemberSettingView;
        UIImageView MerSet_merImg, MerSet_BranchImg, MerSet_LinkproImg, MerSet_EmpImg;
        UILabel lblMerSet_mer, lblMerSet_Branch, lblMerSet_Linkpro, lblMerSet_Emp;
        UIView lineMer1, lineMer2, lineMer21, lineMer3;

        UIView ItemSet_NoteView;
        UIImageView ItemSet_NoteImg;
        UILabel lblItemSet_Note;
        UIView lineItem2;

        UIView BillSet_VatView, BillSet_CurrencyView, BillSet_DecimalView, BillSet_ServiceChargeView, BillSet_GiftVoucherView, BillSet_CashView;
        UIImageView BillSet_VatImg, BillSet_CurrencyImg, BillSet_DecimalImg, BillSet_ServiceChageImg, BillSet_GiftVoucherImg , BillSet_CashImg;
        UILabel lblBillSet_Vat, lblBillSet_Currency, lblBill_CurrencyName, lblBillSet_Decimal, lblBillSet_ServiceChage, lblBillSet_GiftVoucher, lblVatValue, lblServiceValue, lblBillSet_Cash;
        UIView lineBill2, lineBill3, lineBill4, lineBill5, lineBill6, lineBill7;

        UIView DevSet_DeviceView, DevSet_PrinterView;
        UIImageView DevSet_DeviceImg, DevSet_PrinterImg, MemSet_MemTypeImg;
        UILabel lblDevSet_Device, lblDevSet_Printer, lblHeadMemberSetting, lblMemSet_MemType;
        UIView lineDev1, lineDev2, MemSet_MemTypeView, lineMem;

        UIView BillSet_SettingMyQRView;
        UIImageView BillSet_SettingMyQRImg;
        UILabel lblBillSet_SettingMyQr;

        public static List<Currency> currencies = new List<Currency>();

        #region Page
        MerchantSettingController MerchantPage = null;
        BranchSettingController BranchPage = null;
        itemDiscountController itemDiscountPage = null;
        itemNoteController itemNotetPage = null;
        EmployeeManagementController empPage = null;
        VatSettingController vatPage = null;
        CurrencyController curPage = null;
        ServiceChargeController ServiceChargePage = null;
        MemberTypeSettingController MemberTypePage = null;
        DeviceSettingController DevicePage = null;
        PrinterSettingController printerPage = null;
        GiftVoucherSettingController GiftVoucherPAge = null;
        CashGuideController cashGuide = null;
        DecimalController DecimalPage = null;
        MyQrSettingController MyQRPage = null;
        private string LoginType;
        private UIView MerSet_PackageView;
        private UIImageView MerSet_PackageImg;
        private UILabel lblMerSet_Package;
        private UIView lineMer4;
        #endregion

        public SettingController()
        {
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {

            
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("setting", "Setting"));
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                this.NavigationController.SetNavigationBarHidden(false, false);
                
                if (await GabanaAPI.CheckNetWork())
                {
                    await GetmerchantConfig();
                }
                long taxrate = 0;
                Int64.TryParse(DataCashingAll.setmerchantConfig.TAXRATE, out taxrate);

                if (string.IsNullOrEmpty( DataCashingAll.setmerchantConfig.TAXRATE))
                {

                    lblVatValue.Text = Utils.TextBundle("nonvat", "Non VAT") ;
                }
                else
                {
                    var text = Utils.DisplayDecimal(Convert.ToDecimal(DataCashingAll.setmerchantConfig.TAXRATE)) + " % , ";
                    if (DataCashingAll.setmerchantConfig.TAXTYPE == "I")
                    {
                        text += Utils.TextBundle("includevat", "Include Vat") ;
                    }
                    else
                    {
                        text += Utils.TextBundle("excludevat", "Exclude Vat");
                    }
                    lblVatValue.Text = text; 
                }
                switch (DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS) 
                {
                    case "$":
                        lblBill_CurrencyName.Text = currencies[0].Name;
                        break;
                    case "฿":
                        lblBill_CurrencyName.Text = currencies[1].Name;
                        break;
                    case "€":
                        lblBill_CurrencyName.Text = currencies[2].Name;
                        break;
                    case "¥":
                        lblBill_CurrencyName.Text = currencies[3].Name;
                        break;
                    default:
                        lblBill_CurrencyName.Text = currencies[4].Name;
                        break;

                }
                if (DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE == "0" || DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE == "")
                {

                    lblServiceValue.Text = Utils.TextBundle("freecharge", "Free of Charge");
                }
                else
                {
                    string text2 = "";
                    if (!DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE.Contains("%"))
                    {
                        text2 +=  DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                    }
                    text2 += DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE + " , ";
                    if (DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE == "A")
                    {
                        text2 += Utils.TextBundle("afterdis", "After Discount") ;
                    }
                    else
                    {
                        text2 += Utils.TextBundle("beforedis", "Before Discount") ;
                    }

                    lblServiceValue.Text = text2;
                }
                if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
                {
                    var pinCodePage = new PinCodeController("Pincode");
                    pinCodePage.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    await this.PresentViewControllerAsync(pinCodePage, false);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }


        public async override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;

            base.ViewDidLoad();
            LoginType = Preferences.Get("LoginType", "");
            initAttribute();
            SetupAutoLayout();
        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.White;

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.White;

            #region MerchantSettingView
            MerchantSettingView = new UIView();
            MerchantSettingView.TranslatesAutoresizingMaskIntoConstraints = false;
            MerchantSettingView.BackgroundColor = UIColor.White;

            lblHeadMerchantSetting = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHeadMerchantSetting.Font = lblHeadMerchantSetting.Font.WithSize(15);
            lblHeadMerchantSetting.Text = Utils.TextBundle("merchantsetting", "Merchant Setting") ;
            MerchantSettingView.AddSubview(lblHeadMerchantSetting);

            #region MerSet_merView
            MerSet_merView = new UIView();
            MerSet_merView.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_merView.BackgroundColor = UIColor.White;
            MerchantSettingView.AddSubview(MerSet_merView);

            MerSet_merImg = new UIImageView();
            MerSet_merImg.Image = UIImage.FromBundle("SettingMerchant");
            MerSet_merImg.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_merView.AddSubview(MerSet_merImg);

            lblMerSet_mer = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerSet_mer.Font = lblMerSet_mer.Font.WithSize(15);
            lblMerSet_mer.Text = Utils.TextBundle("merchant", "Merchant");
            MerSet_merView.AddSubview(lblMerSet_mer);

            MerSet_merView.UserInteractionEnabled = true;
            var tapGestureMerchant = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Merchant:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            MerSet_merView.AddGestureRecognizer(tapGestureMerchant);

            #endregion

            lineMer1 = new UIView();
            lineMer1.TranslatesAutoresizingMaskIntoConstraints = false;
            lineMer1.BackgroundColor = UIColor.FromRGB(226,226,226);
            MerchantSettingView.AddSubview(lineMer1);

            #region MerSet_BranchView
            MerSet_BranchView = new UIView();
            MerSet_BranchView.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_BranchView.BackgroundColor = UIColor.White;
            MerchantSettingView.AddSubview(MerSet_BranchView);

            MerSet_BranchImg = new UIImageView();
            MerSet_BranchImg.Image = UIImage.FromBundle("SettingBranch");
            MerSet_BranchImg.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_BranchView.AddSubview(MerSet_BranchImg);

            lblMerSet_Branch = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerSet_Branch.Font = lblMerSet_Branch.Font.WithSize(15);
            lblMerSet_Branch.Text = Utils.TextBundle("branch", "Branch");
            MerSet_BranchView.AddSubview(lblMerSet_Branch);

            MerSet_BranchView.UserInteractionEnabled = true;
            var tapGestureBranch = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Branch:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            MerSet_BranchView.AddGestureRecognizer(tapGestureBranch);
            #endregion

            lineMer2 = new UIView();
            lineMer2.TranslatesAutoresizingMaskIntoConstraints = false;
            lineMer2.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            MerchantSettingView.AddSubview(lineMer2);

            #region MerSet_EmployeeView
            MerSet_EmployeeView = new UIView();
            MerSet_EmployeeView.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_EmployeeView.BackgroundColor = UIColor.White;
            MerchantSettingView.AddSubview(MerSet_EmployeeView);

            MerSet_EmpImg = new UIImageView();
            MerSet_EmpImg.Image = UIImage.FromBundle("SettingEmployee");
            MerSet_EmpImg.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_EmployeeView.AddSubview(MerSet_EmpImg);

            lblMerSet_Emp = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerSet_Emp.Font = lblMerSet_Emp.Font.WithSize(15);
            lblMerSet_Emp.Text = Utils.TextBundle("employeemanage", "Employee Management");
            MerSet_EmployeeView.AddSubview(lblMerSet_Emp);

            //EmpManage
            MerSet_EmployeeView.UserInteractionEnabled = true;
            var tapGestureEmployee = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("EmpManage:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            MerSet_EmployeeView.AddGestureRecognizer(tapGestureEmployee);
            #endregion

            lineMer21 = new UIView();
            lineMer21.TranslatesAutoresizingMaskIntoConstraints = false;
            lineMer21.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            MerchantSettingView.AddSubview(lineMer21);

            #region MerSet_LinkProView
            MerSet_LinkProView = new UIView();
            MerSet_LinkProView.Alpha = 0.5f;
            MerSet_LinkProView.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_LinkProView.BackgroundColor = UIColor.White;
            MerchantSettingView.AddSubview(MerSet_LinkProView);

            MerSet_LinkproImg = new UIImageView();
            MerSet_LinkproImg.Image = UIImage.FromBundle("SettingLinkProMaxx");
            MerSet_LinkproImg.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_LinkProView.AddSubview(MerSet_LinkproImg);

            lblMerSet_Linkpro = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerSet_Linkpro.Font = lblMerSet_Linkpro.Font.WithSize(15);
            lblMerSet_Linkpro.Text = Utils.TextBundle("linkpromaxx", "Link ProMaxx");
            MerSet_LinkProView.AddSubview(lblMerSet_Linkpro);
            #endregion

            lineMer3 = new UIView();
            lineMer3.TranslatesAutoresizingMaskIntoConstraints = false;
            lineMer3.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            MerchantSettingView.AddSubview(lineMer3);

            //

            #region MerSet_PackageView
            MerSet_PackageView = new UIView();
            MerSet_PackageView.Alpha = 0.5f;
            MerSet_PackageView.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_PackageView.BackgroundColor = UIColor.White;
            MerchantSettingView.AddSubview(MerSet_PackageView);

            MerSet_PackageImg = new UIImageView();
            MerSet_PackageImg.Image = UIImage.FromBundle("SettingPackage");
            MerSet_PackageImg.TranslatesAutoresizingMaskIntoConstraints = false;
            MerSet_PackageView.AddSubview(MerSet_PackageImg);

            lblMerSet_Package = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerSet_Package.Font = lblMerSet_Package.Font.WithSize(15);
            lblMerSet_Package.Text = Utils.TextBundle("setpackage", "setpackage");
            MerSet_PackageView.AddSubview(lblMerSet_Package);
            #endregion

            lineMer4 = new UIView();
            lineMer4.TranslatesAutoresizingMaskIntoConstraints = false;
            lineMer4.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            MerchantSettingView.AddSubview(lineMer4);

            MerSet_PackageView.UserInteractionEnabled = true;
            var packkkk = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("packagesetting:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            MerSet_PackageView.AddGestureRecognizer(packkkk);

            #endregion

            #region ItemSettingView
            ItemSettingView = new UIView();
            ItemSettingView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemSettingView.BackgroundColor = UIColor.White;

            lblHeadItemSetting = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHeadItemSetting.Font = lblHeadItemSetting.Font.WithSize(15);
            lblHeadItemSetting.Text = Utils.TextBundle("itemsetting", "Item Setting") ;
            ItemSettingView.AddSubview(lblHeadItemSetting);

            #region ItemSet_NoteView
            ItemSet_NoteView = new UIView();
            ItemSet_NoteView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemSet_NoteView.BackgroundColor = UIColor.White;
            ItemSettingView.AddSubview(ItemSet_NoteView);

            ItemSet_NoteImg = new UIImageView();
            ItemSet_NoteImg.Image = UIImage.FromBundle("SettingNote");
            ItemSet_NoteImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemSet_NoteView.AddSubview(ItemSet_NoteImg);

            lblItemSet_Note = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemSet_Note.Font = lblItemSet_Note.Font.WithSize(15);
            lblItemSet_Note.Text = Utils.TextBundle("note", "Note");
            ItemSet_NoteView.AddSubview(lblItemSet_Note);

            ItemSet_NoteView.UserInteractionEnabled = true;
            var ItemSet_Note_Tap = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("ItemSet_Note:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ItemSet_NoteView.AddGestureRecognizer(ItemSet_Note_Tap);
            #endregion

            lineItem2 = new UIView();
            lineItem2.TranslatesAutoresizingMaskIntoConstraints = false;
            lineItem2.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ItemSettingView.AddSubview(lineItem2);

            #endregion

            #region MemberSettingView
            MemberSettingView = new UIView();
            MemberSettingView.TranslatesAutoresizingMaskIntoConstraints = false;
            MemberSettingView.BackgroundColor = UIColor.White;

            lblHeadMemberSetting = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHeadMemberSetting.Font = lblHeadMemberSetting.Font.WithSize(15);
            lblHeadMemberSetting.Text =  Utils.TextBundle("membersetting", "Member Setting");
            MemberSettingView.AddSubview(lblHeadMemberSetting);

            #region MemSet_MemTypeView
            MemSet_MemTypeView = new UIView();
            MemSet_MemTypeView.TranslatesAutoresizingMaskIntoConstraints = false;
            MemSet_MemTypeView.BackgroundColor = UIColor.White;
            MemberSettingView.AddSubview(MemSet_MemTypeView);

            MemSet_MemTypeImg = new UIImageView();
            MemSet_MemTypeImg.Image = UIImage.FromBundle("SettingMemberType");
            MemSet_MemTypeImg.TranslatesAutoresizingMaskIntoConstraints = false;
            MemSet_MemTypeView.AddSubview(MemSet_MemTypeImg);

            lblMemSet_MemType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMemSet_MemType.Font = lblMemSet_MemType.Font.WithSize(15);
            lblMemSet_MemType.Text = Utils.TextBundle("membertype", "Member Type");
            MemSet_MemTypeView.AddSubview(lblMemSet_MemType);

            MemSet_MemTypeView.UserInteractionEnabled = true;
            var tapGestureMemberType = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("MemberType:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            MemSet_MemTypeView.AddGestureRecognizer(tapGestureMemberType);
            #endregion

            lineMem = new UIView();
            lineMem.TranslatesAutoresizingMaskIntoConstraints = false;
            lineMem.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            MemberSettingView.AddSubview(lineMem);
            #endregion

            #region BillSettingView
            BillSettingView = new UIView();
            BillSettingView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSettingView.BackgroundColor = UIColor.White;

            lblHeadBillSetting = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHeadBillSetting.Font = lblHeadBillSetting.Font.WithSize(15);
            lblHeadBillSetting.Text = Utils.TextBundle("billsetting", "Bill Setting"); 
            BillSettingView.AddSubview(lblHeadBillSetting);

            #region BillSet_VatView
            BillSet_VatView = new UIView();
            BillSet_VatView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_VatView.BackgroundColor = UIColor.White;
            BillSettingView.AddSubview(BillSet_VatView);

            BillSet_VatImg = new UIImageView();
            BillSet_VatImg.Image = UIImage.FromBundle("SettingVat");
            BillSet_VatImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_VatView.AddSubview(BillSet_VatImg);

            lblBillSet_Vat = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBillSet_Vat.Font = lblBillSet_Vat.Font.WithSize(15);
            lblBillSet_Vat.Text =  Utils.TextBundle("vat", "Vat");
            BillSet_VatView.AddSubview(lblBillSet_Vat);

            lblVatValue = new UILabel
            {
                TextColor = UIColor.FromRGB(172,172,172),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblVatValue.Font = lblVatValue.Font.WithSize(15);
            lblVatValue.Text = Utils.TextBundle("vat", "Vat");
            BillSet_VatView.AddSubview(lblVatValue);

            BillSet_VatView.UserInteractionEnabled = true;
            var tapGestureVat = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("VatSetting:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BillSet_VatView.AddGestureRecognizer(tapGestureVat);
            #endregion

            lineBill2 = new UIView();
            lineBill2.TranslatesAutoresizingMaskIntoConstraints = false;
            lineBill2.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            BillSettingView.AddSubview(lineBill2);

            #region BillSet_CurrencyView

            if (currencies == null || currencies.Count == 0)
            {
                currencies.Add(new Currency { Name = Utils.TextBundle("usdollar", "US Dollar"), Image = "Currency-USDg", Imagechoose = "Currency-USD", Choose = false, CurrencyType = "$" });
                currencies.Add(new Currency { Name = Utils.TextBundle("thaibaht", "Thai Baht"), Imagechoose = "Currency-THB", Image = "Currency-THBg", Choose = false, CurrencyType = "฿" });
                currencies.Add(new Currency { Name = Utils.TextBundle("euro", "Euro"), Imagechoose = "Currency-EUR", Image = "Currency-EURg", Choose = false, CurrencyType = "€" });
                currencies.Add(new Currency { Name = Utils.TextBundle("japaneseyen", "Japanese Yen"), Image = "Currency-JPYg", Imagechoose = "Currency-JPY", Choose = false, CurrencyType = "¥" });
                currencies.Add(new Currency { Name = Utils.TextBundle("notdisplayed", "Not Displayed"), Image = "Currency-NoG", Imagechoose = "Currency-No", Choose = false, CurrencyType = "" });
            }

            BillSet_CurrencyView = new UIView();
            BillSet_CurrencyView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_CurrencyView.BackgroundColor = UIColor.White;
            BillSettingView.AddSubview(BillSet_CurrencyView);

            BillSet_CurrencyImg = new UIImageView();
            BillSet_CurrencyImg.Image = UIImage.FromBundle("SettingCurrency");
            BillSet_CurrencyImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_CurrencyView.AddSubview(BillSet_CurrencyImg);

            lblBillSet_Currency = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBillSet_Currency.Font = lblBillSet_Currency.Font.WithSize(15);
            lblBillSet_Currency.Text = Utils.TextBundle("currency", "Currency");
            BillSet_CurrencyView.AddSubview(lblBillSet_Currency);

            lblBill_CurrencyName = new UILabel
            {
                TextColor = UIColor.FromRGB(172, 172, 172),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBill_CurrencyName.Font = lblBill_CurrencyName.Font.WithSize(15);
            BillSet_CurrencyView.AddSubview(lblBill_CurrencyName);


            BillSet_CurrencyView.UserInteractionEnabled = true;
            var tapGestureCurrency = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("CurrencySetting:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BillSet_CurrencyView.AddGestureRecognizer(tapGestureCurrency);
            #endregion

            lineBill3 = new UIView();
            lineBill3.TranslatesAutoresizingMaskIntoConstraints = false;
            lineBill3.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            BillSettingView.AddSubview(lineBill3);
            //Decimal
            #region BillSet_DecimalView
            BillSet_DecimalView = new UIView();
            BillSet_DecimalView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_DecimalView.BackgroundColor = UIColor.White;
            BillSettingView.AddSubview(BillSet_DecimalView);

            BillSet_DecimalImg = new UIImageView();
            BillSet_DecimalImg.Image = UIImage.FromBundle("SettingDecimal");
            BillSet_DecimalImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_DecimalView.AddSubview(BillSet_DecimalImg);

            lblBillSet_Decimal = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBillSet_Decimal.Font = lblBillSet_Decimal.Font.WithSize(15);
            lblBillSet_Decimal.Text = Utils.TextBundle("decimal", "Decimal");
            BillSet_DecimalView.AddSubview(lblBillSet_Decimal);

            BillSet_DecimalView.UserInteractionEnabled = true;
            var tapGestureDecimal = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Decimal:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BillSet_DecimalView.AddGestureRecognizer(tapGestureDecimal);
            #endregion

            lineBill4 = new UIView();
            lineBill4.TranslatesAutoresizingMaskIntoConstraints = false;
            lineBill4.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            BillSettingView.AddSubview(lineBill4);

            #region BillSet_ServiceChargeView
            BillSet_ServiceChargeView = new UIView();
            BillSet_ServiceChargeView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_ServiceChargeView.BackgroundColor = UIColor.White;
            BillSettingView.AddSubview(BillSet_ServiceChargeView);

            BillSet_ServiceChageImg = new UIImageView();
            BillSet_ServiceChageImg.Image = UIImage.FromBundle("SettingServiceCharge");
            BillSet_ServiceChageImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_ServiceChargeView.AddSubview(BillSet_ServiceChageImg);

            lblBillSet_ServiceChage = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBillSet_ServiceChage.Font = lblBillSet_ServiceChage.Font.WithSize(15);
            lblBillSet_ServiceChage.Text = Utils.TextBundle("servicecharge", "Service Charge"); 
            BillSet_ServiceChargeView.AddSubview(lblBillSet_ServiceChage);

            lblServiceValue = new UILabel
            {
                TextColor = UIColor.FromRGB(172, 172, 172),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblServiceValue.Font = lblServiceValue.Font.WithSize(15);
            lblServiceValue.Text = Utils.TextBundle("vat", "Vat");
            BillSet_ServiceChargeView.AddSubview(lblServiceValue);

            BillSet_ServiceChargeView.UserInteractionEnabled = true;
            var tapGestureService = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("ServiceCharge:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BillSet_ServiceChargeView.AddGestureRecognizer(tapGestureService);
            #endregion

            lineBill5 = new UIView();
            lineBill5.TranslatesAutoresizingMaskIntoConstraints = false;
            lineBill5.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            BillSettingView.AddSubview(lineBill5);

            #region BillSet_GiftVoucherView
            BillSet_CashView = new UIView();
            BillSet_CashView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_CashView.BackgroundColor = UIColor.White;
            BillSettingView.AddSubview(BillSet_CashView);

            BillSet_CashImg = new UIImageView();
            BillSet_CashImg.Image = UIImage.FromBundle("SettingCash");
            BillSet_CashImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_CashView.AddSubview(BillSet_CashImg);

            lblBillSet_Cash = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBillSet_Cash.Font = lblBillSet_Cash.Font.WithSize(15);
            lblBillSet_Cash.Text = Utils.TextBundle("cash", "Cash");
            BillSet_CashView.AddSubview(lblBillSet_Cash);

            BillSet_CashView.UserInteractionEnabled = true;
            var tapGestureCash = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Cash:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BillSet_CashView.AddGestureRecognizer(tapGestureCash);
            #endregion

            lineBill7 = new UIView();
            lineBill7.TranslatesAutoresizingMaskIntoConstraints = false;
            lineBill7.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            BillSettingView.AddSubview(lineBill7);

            #region BillSet_GiftVoucherView
            BillSet_GiftVoucherView = new UIView();
            BillSet_GiftVoucherView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_GiftVoucherView.BackgroundColor = UIColor.White;
            BillSettingView.AddSubview(BillSet_GiftVoucherView);

            BillSet_GiftVoucherImg = new UIImageView();
            BillSet_GiftVoucherImg.Image = UIImage.FromBundle("SettingGiftVoucher");
            BillSet_GiftVoucherImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_GiftVoucherView.AddSubview(BillSet_GiftVoucherImg);

            lblBillSet_GiftVoucher = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBillSet_GiftVoucher.Font = lblBillSet_GiftVoucher.Font.WithSize(15);
            lblBillSet_GiftVoucher.Text = Utils.TextBundle("giftvoucher", "Gift Voucher"); 
            BillSet_GiftVoucherView.AddSubview(lblBillSet_GiftVoucher);

            BillSet_GiftVoucherView.UserInteractionEnabled = true;
            var tapGestureGift = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("GiftVoucher:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BillSet_GiftVoucherView.AddGestureRecognizer(tapGestureGift);
            #endregion

            lineBill6 = new UIView();
            lineBill6.TranslatesAutoresizingMaskIntoConstraints = false;
            lineBill6.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            BillSettingView.AddSubview(lineBill6);

            #region BillSet_SettingMyQRView
            BillSet_SettingMyQRView = new UIView();
            BillSet_SettingMyQRView.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_SettingMyQRView.BackgroundColor = UIColor.White;
            BillSettingView.AddSubview(BillSet_SettingMyQRView);

            BillSet_SettingMyQRImg = new UIImageView();
            BillSet_SettingMyQRImg.Image = UIImage.FromFile("SettingmyQR.png");
            BillSet_SettingMyQRImg.TranslatesAutoresizingMaskIntoConstraints = false;
            BillSet_SettingMyQRView.AddSubview(BillSet_SettingMyQRImg);

            lblBillSet_SettingMyQr = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBillSet_SettingMyQr.Font = lblBillSet_SettingMyQr.Font.WithSize(15);
            lblBillSet_SettingMyQr.Text = Utils.TextBundle("settingmyqr", "Setting myQR"); 
            BillSet_SettingMyQRView.AddSubview(lblBillSet_SettingMyQr);

            BillSet_SettingMyQRView.UserInteractionEnabled = true;
            var tapGestureMYQR= new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("MYQR:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BillSet_SettingMyQRView.AddGestureRecognizer(tapGestureMYQR);
            #endregion

            #endregion

            #region DeviceSettingView
            DeviceSettingView = new UIView();
            DeviceSettingView.TranslatesAutoresizingMaskIntoConstraints = false;
            DeviceSettingView.BackgroundColor = UIColor.White;

            lblHeadDeviceSetting = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblHeadDeviceSetting.Font = lblHeadDeviceSetting.Font.WithSize(15);
            lblHeadDeviceSetting.Text = Utils.TextBundle("devicesetting", "Device Setting");
            DeviceSettingView.AddSubview(lblHeadDeviceSetting);

            #region DevSet_DeviceView
            DevSet_DeviceView = new UIView();
            DevSet_DeviceView.TranslatesAutoresizingMaskIntoConstraints = false;
            DevSet_DeviceView.BackgroundColor = UIColor.White;
            DeviceSettingView.AddSubview(DevSet_DeviceView);

            DevSet_DeviceImg = new UIImageView();
            DevSet_DeviceImg.Image = UIImage.FromBundle("SettingDevice");
            DevSet_DeviceImg.TranslatesAutoresizingMaskIntoConstraints = false;
            DevSet_DeviceView.AddSubview(DevSet_DeviceImg);

            lblDevSet_Device = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDevSet_Device.Font = lblDevSet_Device.Font.WithSize(15);
            lblDevSet_Device.Text = Utils.TextBundle("device", "Device"); 
            DevSet_DeviceView.AddSubview(lblDevSet_Device);

            DevSet_DeviceView.UserInteractionEnabled = true;
            var tapGestureDeviceSet = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("DeviceSetting:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DevSet_DeviceView.AddGestureRecognizer(tapGestureDeviceSet);
            #endregion

            lineDev1 = new UIView();
            lineDev1.TranslatesAutoresizingMaskIntoConstraints = false;
            lineDev1.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            DeviceSettingView.AddSubview(lineDev1);

            #region DevSet_PrinterView
            DevSet_PrinterView = new UIView();
            DevSet_PrinterView.TranslatesAutoresizingMaskIntoConstraints = false;
            DevSet_PrinterView.BackgroundColor = UIColor.White;
            DeviceSettingView.AddSubview(DevSet_PrinterView);

            DevSet_PrinterImg = new UIImageView();
            DevSet_PrinterImg.Image = UIImage.FromBundle("SettingPrinter");
            DevSet_PrinterImg.TranslatesAutoresizingMaskIntoConstraints = false;
            DevSet_PrinterView.AddSubview(DevSet_PrinterImg);

            lblDevSet_Printer = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDevSet_Printer.Font = lblDevSet_Printer.Font.WithSize(15);
            lblDevSet_Printer.Text = Utils.TextBundle("printer", "Printer"); 
            DevSet_PrinterView.AddSubview(lblDevSet_Printer);

            DevSet_PrinterView.UserInteractionEnabled = true;
            var tapGesturePrinterSet = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("PrinterSetting:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DevSet_PrinterView.AddGestureRecognizer(tapGesturePrinterSet);
            #endregion

            lineDev2 = new UIView();
            lineDev2.TranslatesAutoresizingMaskIntoConstraints = false;
            lineDev2.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            DeviceSettingView.AddSubview(lineDev2);

            #endregion

            _contentView.AddSubview(MerchantSettingView);
            _contentView.AddSubview(ItemSettingView);
            _contentView.AddSubview(MemberSettingView);
            _contentView.AddSubview(BillSettingView);
            _contentView.AddSubview(DeviceSettingView);
            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
        }
        #region toggle field
            #region itemSetting
        [Export("itemSet_Discount:")]
        public void itemSet_Discount(UIGestureRecognizer sender)
        {
            
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("discount", "Discount")); 
            if (itemDiscountPage==null)
            {
                itemDiscountPage = new itemDiscountController();
            }
            this.NavigationController.PushViewController(itemDiscountPage, false);
        }
        [Export("ItemSet_Note:")]
        public void ItemSet_Note(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "note");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("note", "Note"));
                //if (itemNotetPage == null)
                //{
                itemNotetPage = new itemNoteController();
                //}
                this.NavigationController.PushViewController(itemNotetPage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        async Task GetmerchantConfig()
        {
            try
            {
                List<MerchantConfig> lstconfig = new List<MerchantConfig>();
                SetMerchantConfig setconfig = new SetMerchantConfig();
                MerchantConfigManage merchantconfigManage = new MerchantConfigManage();


                List<ORM.Master.MerchantConfig> listmerchantConfig = new List<ORM.Master.MerchantConfig>();
                listmerchantConfig = await GabanaAPI.GetDataMerchantConfig();
                if (listmerchantConfig == null)
                {
                    return;
                }
                if (listmerchantConfig.Count > 0)
                {
                    foreach (var item in listmerchantConfig)
                    {
                        MerchantConfig config = new MerchantConfig()
                        {
                            MerchantID = item.MerchantID,
                            CfgKey = item.CfgKey,
                            CfgInteger = item.CfgInteger,
                            CfgFloat = item.CfgFloat,
                            CfgString = item.CfgString,
                            CfgDate = item.CfgDate
                        };
                        var InsertorReplace = await merchantconfigManage.InsertorReplacrMerchantConfig(config);
                        if (InsertorReplace)
                        {
                            lstconfig.Add(config);
                        }
                    }

                    #region merchantConfig
                    var TAXTYPE = lstconfig.Where(x => x.CfgKey == "TAXTYPE").FirstOrDefault();
                    if (TAXTYPE != null)
                    {
                        setconfig.TAXTYPE = TAXTYPE.CfgString;
                    }

                    var TAXRATE = lstconfig.Where(x => x.CfgKey == "TAXRATE").FirstOrDefault();
                    if (TAXRATE != null)
                    {
                        if (TAXRATE.CfgFloat != null)
                        {
                            setconfig.TAXRATE = TAXRATE.CfgFloat.ToString();
                        }
                    }

                    var CURRENCY_SYMBOLS = lstconfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                    if (CURRENCY_SYMBOLS != null)
                    {
                        setconfig.CURRENCY_SYMBOLS = CURRENCY_SYMBOLS.CfgString;
                    }

                    var DECIMAL_POINT_CALC = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_CALC").FirstOrDefault();
                    if (DECIMAL_POINT_CALC != null)
                    {
                        if (DECIMAL_POINT_CALC.CfgInteger != null)
                        {
                            setconfig.DECIMAL_POINT_CALC = DECIMAL_POINT_CALC.CfgInteger.ToString();
                        }
                    }

                    var DECIMAL_POINT_DISPLAY = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_DISPLAY").FirstOrDefault();
                    if (DECIMAL_POINT_DISPLAY != null)
                    {
                        if (DECIMAL_POINT_DISPLAY.CfgInteger != null)
                        {
                            setconfig.DECIMAL_POINT_DISPLAY = DECIMAL_POINT_DISPLAY.CfgInteger.ToString();
                        }
                    }

                    var OPTION_ROUNDING = lstconfig.Where(x => x.CfgKey == "OPTION_ROUNDING").FirstOrDefault();
                    if (OPTION_ROUNDING != null)
                    {
                        setconfig.OPTION_ROUNDING_STRING = OPTION_ROUNDING.CfgString;
                        if (OPTION_ROUNDING.CfgInteger != null)
                        {
                            setconfig.OPTION_ROUNDING_INT = OPTION_ROUNDING.CfgInteger.ToString();
                        }
                    }

                    var SERVICECHARGE_TYPE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_TYPE").FirstOrDefault();
                    if (SERVICECHARGE_TYPE != null)
                    {
                        setconfig.SERVICECHARGE_TYPE = SERVICECHARGE_TYPE.CfgString;
                    }

                    var SERVICECHARGE_RATE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_RATE").FirstOrDefault();
                    if (SERVICECHARGE_RATE != null)
                    {
                        setconfig.SERVICECHARGE_RATE = SERVICECHARGE_RATE.CfgString;
                    }

                    var PRINTER_DEFAULT = lstconfig.Where(x => x.CfgKey == "PRINTER_DEFAULT").FirstOrDefault();
                    if (PRINTER_DEFAULT != null)
                    {
                        setconfig.PRINTER_DEFAULT = PRINTER_DEFAULT.CfgString;
                    }

                    var SUBSCRIPTION_TYPE = lstconfig.Where(x => x.CfgKey == "SUBSCRIPTION_TYPE").FirstOrDefault();
                    if (SUBSCRIPTION_TYPE != null)
                    {
                        setconfig.SUBSCRIPTION_TYPE = SUBSCRIPTION_TYPE.CfgString;
                    }

                    #endregion

                    var merchantConfig = JsonConvert.SerializeObject(setconfig);
                    Preferences.Set("SetmerchantConfig", merchantConfig);
                    var setmerchantConfig = Preferences.Get("SetmerchantConfig", "");
                    if (setmerchantConfig != "")
                    {
                        var Config = JsonConvert.DeserializeObject<SetMerchantConfig>(setmerchantConfig);
                        DataCashingAll.setmerchantConfig = Config;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetmerchantConfig");
                //Log.Error("connecterror", "GetmerchantConfig : " + ex.Message);
                throw;
            }
        }

        [Export("packagesetting:")]
        public void ItemSet_Pack(UIGestureRecognizer sender) 
        {
            var packpage = new PackageDetailController(); 
            DataCaching.MainNavigation.PushViewController(packpage, false);
        }
        #endregion
        #region MerchantSetting
        [Export("Merchant:")]
        public void Merchant(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("merchant", "Merchant"));
            
            MerchantPage = new MerchantSettingController();
            this.NavigationController.PushViewController(MerchantPage, false);
        }
        [Export("Branch:")]
        public void Branch(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("branch", "Branch"));
            
            BranchPage = new BranchSettingController();
            this.NavigationController.PushViewController(BranchPage, false);
        }

        [Export("EmpManage:")]
        public void EmpManage(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("employeemanage", "Employee Management"));
            
            empPage = new EmployeeManagementController();
            this.NavigationController.PushViewController(empPage, false);
        }
        #endregion
            #region BillSetting
        [Export("VatSetting:")]
        public void VatSetting(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "vat");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("vat", "VAT"));

                vatPage = new VatSettingController();
                this.NavigationController.PushViewController(vatPage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        [Export("CurrencySetting:")]
        public void CurrencySetting(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "currency");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("currency", "Currency"));

                curPage = new CurrencyController();
                this.NavigationController.PushViewController(curPage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }

        }
        [Export("ServiceCharge:")]
        public void ServiceCharge(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "servicecharge");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("servicecharge", "Service Charge"));

                //if (ServiceChargePage == null)
                //{
                ServiceChargePage = new ServiceChargeController();
                //}
                this.NavigationController.PushViewController(ServiceChargePage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        [Export("GiftVoucher:")]
        public void GiftVoucher(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "giftvoucher");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("giftvoucher", "Gift Voucher"));

                GiftVoucherPAge = new GiftVoucherSettingController();
                this.NavigationController.PushViewController(GiftVoucherPAge, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        [Export("Cash:")]
        public void Cash(UIGestureRecognizer sender)
        {

            Utils.SetTitle(this.NavigationController, Utils.TextBundle("cash", "Cash"));
            cashGuide = new CashGuideController();
            this.NavigationController.PushViewController(cashGuide, false);
        }

        [Export("Decimal:")]
        public void Decimal(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "decimal");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("decimal", "Decimal"));
                
                DecimalPage = new DecimalController();
                
                this.NavigationController.PushViewController(DecimalPage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        [Export("MYQR:")]
        public void MYQR(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "myqr");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("settingmyqr", "Setting myQR"));
                MyQRPage = new MyQrSettingController();
                this.NavigationController.PushViewController(MyQRPage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        //GiftVoucher
        #endregion
        #region MemberSetting
        [Export("MemberType:")]
        public void MemberType(UIGestureRecognizer sender)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "membertype");
            if (check)
            {
                Utils.SetTitle(this.NavigationController, Utils.TextBundle("membertype", "Member Type")); 

                MemberTypePage = new MemberTypeSettingController();
                this.NavigationController.PushViewController(MemberTypePage, false);
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("notperm", "Not accessible"));
            }
        }
        #endregion
            #region DeviceSetting
            [Export("DeviceSetting:")]
            public void DeviceSetting(UIGestureRecognizer sender)
            {

            //if (DevicePage == null)
            //{
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("device", "Device")); 
            
            if (DevicePage == null)
            {

                DevicePage = new DeviceSettingController();
            
            }
                this.NavigationController.PushViewController(DevicePage, false);
            }
            [Export("PrinterSetting:")]
            public void PrinterSetting(UIGestureRecognizer sender)
            {

            Utils.SetTitle(this.NavigationController, Utils.TextBundle("printer", "Printer")); 
            
                //if (printerPage == null)
                //{

                printerPage = new PrinterSettingController();
                //}
                this.NavigationController.PushViewController(printerPage, false);
            }
        #endregion
        #endregion
      
        void SetupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region MerchantSettingView
            MerchantSettingView.TopAnchor.ConstraintEqualTo(MerchantSettingView.Superview.TopAnchor, 11).Active = true;
            MerchantSettingView.LeftAnchor.ConstraintEqualTo(MerchantSettingView.Superview.LeftAnchor, 0).Active = true;
            MerchantSettingView.RightAnchor.ConstraintEqualTo(MerchantSettingView.Superview.RightAnchor, 0).Active = true;
            MerchantSettingView.HeightAnchor.ConstraintEqualTo(270).Active = true;

            lblHeadMerchantSetting.TopAnchor.ConstraintEqualTo(lblHeadMerchantSetting.Superview.TopAnchor, 10).Active = true;
            lblHeadMerchantSetting.LeftAnchor.ConstraintEqualTo(lblHeadMerchantSetting.Superview.LeftAnchor, 15).Active = true;
            lblHeadMerchantSetting.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblHeadMerchantSetting.RightAnchor.ConstraintEqualTo(lblHeadMerchantSetting.Superview.RightAnchor, -30).Active = true;

            #region MerSet_merView
            MerSet_merView.TopAnchor.ConstraintEqualTo(lblHeadMerchantSetting.BottomAnchor, 0).Active = true;
            MerSet_merView.LeftAnchor.ConstraintEqualTo(MerSet_merView.Superview.LeftAnchor, 0).Active = true;
            MerSet_merView.RightAnchor.ConstraintEqualTo(MerSet_merView.Superview.RightAnchor, 0).Active = true;
            MerSet_merView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            MerSet_merImg.CenterYAnchor.ConstraintEqualTo(MerSet_merImg.Superview.CenterYAnchor).Active = true;
            MerSet_merImg.LeftAnchor.ConstraintEqualTo(MerSet_merImg.Superview.LeftAnchor, 25).Active = true;
            MerSet_merImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            MerSet_merImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblMerSet_mer.CenterYAnchor.ConstraintEqualTo(lblMerSet_mer.Superview.CenterYAnchor).Active = true;
            lblMerSet_mer.LeftAnchor.ConstraintEqualTo(MerSet_merImg.RightAnchor, 25).Active = true;
            lblMerSet_mer.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMerSet_mer.RightAnchor.ConstraintEqualTo(lblMerSet_mer.Superview.RightAnchor,-30).Active = true;
            #endregion

            lineMer1.BottomAnchor.ConstraintEqualTo(MerSet_merView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineMer1.LeftAnchor.ConstraintEqualTo(lineMer1.Superview.LeftAnchor, 25).Active = true;
            lineMer1.RightAnchor.ConstraintEqualTo(lineMer1.Superview.RightAnchor, 0).Active = true;
            lineMer1.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region MerSet_BranchView
            MerSet_BranchView.TopAnchor.ConstraintEqualTo(MerSet_merView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            MerSet_BranchView.LeftAnchor.ConstraintEqualTo(MerSet_BranchView.Superview.LeftAnchor, 0).Active = true;
            MerSet_BranchView.RightAnchor.ConstraintEqualTo(MerSet_BranchView.Superview.RightAnchor, 0).Active = true;
            MerSet_BranchView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            MerSet_BranchImg.CenterYAnchor.ConstraintEqualTo(MerSet_BranchImg.Superview.CenterYAnchor).Active = true;
            MerSet_BranchImg.LeftAnchor.ConstraintEqualTo(MerSet_BranchImg.Superview.LeftAnchor, 25).Active = true;
            MerSet_BranchImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            MerSet_BranchImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblMerSet_Branch.CenterYAnchor.ConstraintEqualTo(lblMerSet_Branch.Superview.CenterYAnchor).Active = true;
            lblMerSet_Branch.LeftAnchor.ConstraintEqualTo(MerSet_BranchImg.RightAnchor, 25).Active = true;
            lblMerSet_Branch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMerSet_Branch.RightAnchor.ConstraintEqualTo(lblMerSet_Branch.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineMer2.BottomAnchor.ConstraintEqualTo(MerSet_BranchView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineMer2.LeftAnchor.ConstraintEqualTo(lineMer2.Superview.LeftAnchor, 25).Active = true;
            lineMer2.RightAnchor.ConstraintEqualTo(lineMer2.Superview.RightAnchor, 0).Active = true;
            lineMer2.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region MerSet_EmployeeView
            MerSet_EmployeeView.TopAnchor.ConstraintEqualTo(MerSet_BranchView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            MerSet_EmployeeView.LeftAnchor.ConstraintEqualTo(MerSet_EmployeeView.Superview.LeftAnchor, 0).Active = true;
            MerSet_EmployeeView.RightAnchor.ConstraintEqualTo(MerSet_EmployeeView.Superview.RightAnchor, 0).Active = true;
            MerSet_EmployeeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            MerSet_EmpImg.CenterYAnchor.ConstraintEqualTo(MerSet_EmpImg.Superview.CenterYAnchor).Active = true;
            MerSet_EmpImg.LeftAnchor.ConstraintEqualTo(MerSet_EmpImg.Superview.LeftAnchor, 25).Active = true;
            MerSet_EmpImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            MerSet_EmpImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblMerSet_Emp.CenterYAnchor.ConstraintEqualTo(lblMerSet_Emp.Superview.CenterYAnchor).Active = true;
            lblMerSet_Emp.LeftAnchor.ConstraintEqualTo(MerSet_EmpImg.RightAnchor, 25).Active = true;
            lblMerSet_Emp.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMerSet_Emp.RightAnchor.ConstraintEqualTo(lblMerSet_Emp.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineMer21.BottomAnchor.ConstraintEqualTo(MerSet_EmployeeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineMer21.LeftAnchor.ConstraintEqualTo(lineMer21.Superview.LeftAnchor, 25).Active = true;
            lineMer21.RightAnchor.ConstraintEqualTo(lineMer21.Superview.RightAnchor, 0).Active = true;
            lineMer21.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region MerSet_LinkProView
            MerSet_LinkProView.TopAnchor.ConstraintEqualTo(MerSet_EmployeeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            MerSet_LinkProView.LeftAnchor.ConstraintEqualTo(MerSet_LinkProView.Superview.LeftAnchor, 0).Active = true;
            MerSet_LinkProView.RightAnchor.ConstraintEqualTo(MerSet_LinkProView.Superview.RightAnchor, 0).Active = true;
            MerSet_LinkProView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            MerSet_LinkproImg.CenterYAnchor.ConstraintEqualTo(MerSet_LinkproImg.Superview.CenterYAnchor).Active = true;
            MerSet_LinkproImg.LeftAnchor.ConstraintEqualTo(MerSet_LinkproImg.Superview.LeftAnchor, 25).Active = true;
            MerSet_LinkproImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            MerSet_LinkproImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblMerSet_Linkpro.CenterYAnchor.ConstraintEqualTo(lblMerSet_Linkpro.Superview.CenterYAnchor).Active = true;
            lblMerSet_Linkpro.LeftAnchor.ConstraintEqualTo(MerSet_LinkproImg.RightAnchor, 25).Active = true;
            lblMerSet_Linkpro.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMerSet_Linkpro.RightAnchor.ConstraintEqualTo(lblMerSet_Linkpro.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineMer3.BottomAnchor.ConstraintEqualTo(MerSet_LinkProView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineMer3.LeftAnchor.ConstraintEqualTo(lineMer3.Superview.LeftAnchor, 25).Active = true;
            lineMer3.RightAnchor.ConstraintEqualTo(lineMer3.Superview.RightAnchor, 0).Active = true;
            lineMer3.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region MerSet_LinkProView
            MerSet_PackageView.TopAnchor.ConstraintEqualTo(MerSet_LinkProView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            MerSet_PackageView.LeftAnchor.ConstraintEqualTo(MerSet_PackageView.Superview.LeftAnchor, 0).Active = true;
            MerSet_PackageView.RightAnchor.ConstraintEqualTo(MerSet_PackageView.Superview.RightAnchor, 0).Active = true;
            MerSet_PackageView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            MerSet_PackageImg.CenterYAnchor.ConstraintEqualTo(MerSet_PackageImg.Superview.CenterYAnchor).Active = true;
            MerSet_PackageImg.LeftAnchor.ConstraintEqualTo(MerSet_PackageImg.Superview.LeftAnchor, 25).Active = true;
            MerSet_PackageImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            MerSet_PackageImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblMerSet_Package.CenterYAnchor.ConstraintEqualTo(lblMerSet_Package.Superview.CenterYAnchor).Active = true;
            lblMerSet_Package.LeftAnchor.ConstraintEqualTo(MerSet_PackageImg.RightAnchor, 25).Active = true;
            lblMerSet_Package.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMerSet_Package.RightAnchor.ConstraintEqualTo(lblMerSet_Package.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineMer4.BottomAnchor.ConstraintEqualTo(MerSet_PackageView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineMer4.LeftAnchor.ConstraintEqualTo(lineMer4.Superview.LeftAnchor, 25).Active = true;
            lineMer4.RightAnchor.ConstraintEqualTo(lineMer4.Superview.RightAnchor, 0).Active = true;
            lineMer4.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            #endregion

            #region ItemSettingView
            ItemSettingView.TopAnchor.ConstraintEqualTo(MerSet_PackageView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            ItemSettingView.LeftAnchor.ConstraintEqualTo(ItemSettingView.Superview.LeftAnchor, 0).Active = true;
            ItemSettingView.RightAnchor.ConstraintEqualTo(ItemSettingView.Superview.RightAnchor, 0).Active = true;
            ItemSettingView.HeightAnchor.ConstraintEqualTo(80).Active = true;

            lblHeadItemSetting.TopAnchor.ConstraintEqualTo(lblHeadItemSetting.Superview.TopAnchor, 0).Active = true;
            lblHeadItemSetting.LeftAnchor.ConstraintEqualTo(lblHeadItemSetting.Superview.LeftAnchor, 15).Active = true;
            lblHeadItemSetting.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblHeadItemSetting.RightAnchor.ConstraintEqualTo(lblHeadItemSetting.Superview.RightAnchor, -30).Active = true;

            #region ItemSet_NoteView
            ItemSet_NoteView.TopAnchor.ConstraintEqualTo(lblHeadItemSetting.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ItemSet_NoteView.LeftAnchor.ConstraintEqualTo(ItemSet_NoteView.Superview.LeftAnchor, 0).Active = true;
            ItemSet_NoteView.RightAnchor.ConstraintEqualTo(ItemSet_NoteView.Superview.RightAnchor, 0).Active = true;
            ItemSet_NoteView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            ItemSet_NoteImg.CenterYAnchor.ConstraintEqualTo(ItemSet_NoteImg.Superview.CenterYAnchor).Active = true;
            ItemSet_NoteImg.LeftAnchor.ConstraintEqualTo(ItemSet_NoteImg.Superview.LeftAnchor, 25).Active = true;
            ItemSet_NoteImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            ItemSet_NoteImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblItemSet_Note.CenterYAnchor.ConstraintEqualTo(lblItemSet_Note.Superview.CenterYAnchor).Active = true;
            lblItemSet_Note.LeftAnchor.ConstraintEqualTo(ItemSet_NoteImg.RightAnchor, 25).Active = true;
            lblItemSet_Note.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblItemSet_Note.RightAnchor.ConstraintEqualTo(lblItemSet_Note.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineItem2.BottomAnchor.ConstraintEqualTo(ItemSet_NoteView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineItem2.LeftAnchor.ConstraintEqualTo(lineItem2.Superview.LeftAnchor, 25).Active = true;
            lineItem2.RightAnchor.ConstraintEqualTo(lineItem2.Superview.RightAnchor, 0).Active = true;
            lineItem2.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            #endregion

            #region MemberSettingView
            MemberSettingView.TopAnchor.ConstraintEqualTo(ItemSet_NoteView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            MemberSettingView.LeftAnchor.ConstraintEqualTo(MemberSettingView.Superview.LeftAnchor, 0).Active = true;
            MemberSettingView.RightAnchor.ConstraintEqualTo(MemberSettingView.Superview.RightAnchor, 0).Active = true;
            MemberSettingView.HeightAnchor.ConstraintEqualTo(80).Active = true;

            lblHeadMemberSetting.TopAnchor.ConstraintEqualTo(lblHeadMemberSetting.Superview.TopAnchor, 0).Active = true;
            lblHeadMemberSetting.LeftAnchor.ConstraintEqualTo(lblHeadMemberSetting.Superview.LeftAnchor, 15).Active = true;
            lblHeadMemberSetting.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblHeadMemberSetting.RightAnchor.ConstraintEqualTo(lblHeadMemberSetting.Superview.RightAnchor, -30).Active = true;

            #region MemSet_MemTypeView
            MemSet_MemTypeView.TopAnchor.ConstraintEqualTo(lblHeadMemberSetting.BottomAnchor, 0).Active = true;
            MemSet_MemTypeView.LeftAnchor.ConstraintEqualTo(MemSet_MemTypeView.Superview.LeftAnchor, 0).Active = true;
            MemSet_MemTypeView.RightAnchor.ConstraintEqualTo(MemSet_MemTypeView.Superview.RightAnchor, 0).Active = true;
            MemSet_MemTypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            MemSet_MemTypeImg.CenterYAnchor.ConstraintEqualTo(MemSet_MemTypeImg.Superview.CenterYAnchor).Active = true;
            MemSet_MemTypeImg.LeftAnchor.ConstraintEqualTo(MemSet_MemTypeImg.Superview.LeftAnchor, 25).Active = true;
            MemSet_MemTypeImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            MemSet_MemTypeImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblMemSet_MemType.CenterYAnchor.ConstraintEqualTo(lblMemSet_MemType.Superview.CenterYAnchor).Active = true;
            lblMemSet_MemType.LeftAnchor.ConstraintEqualTo(MemSet_MemTypeImg.RightAnchor, 25).Active = true;
            lblMemSet_MemType.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblMemSet_MemType.RightAnchor.ConstraintEqualTo(lblMemSet_MemType.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineMem.BottomAnchor.ConstraintEqualTo(MemSet_MemTypeView.BottomAnchor, 0).Active = true;
            lineMem.LeftAnchor.ConstraintEqualTo(lineMem.Superview.LeftAnchor, 25).Active = true;
            lineMem.RightAnchor.ConstraintEqualTo(lineMem.Superview.RightAnchor, 0).Active = true;
            lineMem.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            #endregion

            #region BillSettingView
            BillSettingView.TopAnchor.ConstraintEqualTo(MemSet_MemTypeView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            BillSettingView.LeftAnchor.ConstraintEqualTo(BillSettingView.Superview.LeftAnchor, 0).Active = true;
            BillSettingView.RightAnchor.ConstraintEqualTo(BillSettingView.Superview.RightAnchor, 0).Active = true;
            BillSettingView.HeightAnchor.ConstraintEqualTo(440).Active = true;

            lblHeadBillSetting.TopAnchor.ConstraintEqualTo(lblHeadBillSetting.Superview.TopAnchor, 0).Active = true;
            lblHeadBillSetting.LeftAnchor.ConstraintEqualTo(lblHeadBillSetting.Superview.LeftAnchor, 15).Active = true;
            lblHeadBillSetting.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblHeadBillSetting.RightAnchor.ConstraintEqualTo(lblHeadBillSetting.Superview.RightAnchor, -30).Active = true;

            #region BillSet_VatView
            BillSet_VatView.TopAnchor.ConstraintEqualTo(lblHeadBillSetting.BottomAnchor, 0).Active = true;
            BillSet_VatView.LeftAnchor.ConstraintEqualTo(BillSet_VatView.Superview.LeftAnchor, 0).Active = true;
            BillSet_VatView.RightAnchor.ConstraintEqualTo(BillSet_VatView.Superview.RightAnchor, 0).Active = true;
            BillSet_VatView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            BillSet_VatImg.CenterYAnchor.ConstraintEqualTo(BillSet_VatImg.Superview.CenterYAnchor).Active = true;
            BillSet_VatImg.LeftAnchor.ConstraintEqualTo(BillSet_VatImg.Superview.LeftAnchor, 25).Active = true;
            BillSet_VatImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            BillSet_VatImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillSet_Vat.CenterYAnchor.ConstraintEqualTo(lblBillSet_Vat.Superview.CenterYAnchor,-12).Active = true;
            lblBillSet_Vat.LeftAnchor.ConstraintEqualTo(BillSet_VatImg.RightAnchor, 25).Active = true;
            lblBillSet_Vat.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillSet_Vat.RightAnchor.ConstraintEqualTo(lblBillSet_Vat.Superview.RightAnchor, -30).Active = true;

            lblVatValue.TopAnchor.ConstraintEqualTo(lblBillSet_Vat.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            lblVatValue.LeftAnchor.ConstraintEqualTo(BillSet_VatImg.RightAnchor, 25).Active = true;
            lblVatValue.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblVatValue.RightAnchor.ConstraintEqualTo(lblVatValue.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineBill2.BottomAnchor.ConstraintEqualTo(BillSet_VatView.BottomAnchor, 0).Active = true;
            lineBill2.LeftAnchor.ConstraintEqualTo(lineBill2.Superview.LeftAnchor, 25).Active = true;
            lineBill2.RightAnchor.ConstraintEqualTo(lineBill2.Superview.RightAnchor, 0).Active = true;
            lineBill2.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region BillSet_CurrencyView
            BillSet_CurrencyView.TopAnchor.ConstraintEqualTo(BillSet_VatView.BottomAnchor, 0).Active = true;
            BillSet_CurrencyView.LeftAnchor.ConstraintEqualTo(BillSet_CurrencyView.Superview.LeftAnchor, 0).Active = true;
            BillSet_CurrencyView.RightAnchor.ConstraintEqualTo(BillSet_CurrencyView.Superview.RightAnchor, 0).Active = true;
            BillSet_CurrencyView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            BillSet_CurrencyImg.CenterYAnchor.ConstraintEqualTo(BillSet_CurrencyImg.Superview.CenterYAnchor).Active = true;
            BillSet_CurrencyImg.LeftAnchor.ConstraintEqualTo(BillSet_CurrencyImg.Superview.LeftAnchor, 25).Active = true;
            BillSet_CurrencyImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            BillSet_CurrencyImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillSet_Currency.BottomAnchor.ConstraintEqualTo(lblBillSet_Currency.Superview.CenterYAnchor, -1).Active = true;
            lblBillSet_Currency.LeftAnchor.ConstraintEqualTo(BillSet_CurrencyImg.RightAnchor, 25).Active = true;
            lblBillSet_Currency.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillSet_Currency.RightAnchor.ConstraintEqualTo(lblBillSet_Currency.Superview.RightAnchor, -30).Active = true;

            lblBill_CurrencyName.TopAnchor.ConstraintEqualTo(BillSet_CurrencyImg.SafeAreaLayoutGuide.CenterYAnchor, 1).Active = true;
            lblBill_CurrencyName.LeftAnchor.ConstraintEqualTo(BillSet_CurrencyImg.RightAnchor, 25).Active = true;
            lblBill_CurrencyName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBill_CurrencyName.RightAnchor.ConstraintEqualTo(lblBill_CurrencyName.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineBill3.BottomAnchor.ConstraintEqualTo(BillSet_CurrencyView.BottomAnchor, 0).Active = true;
            lineBill3.LeftAnchor.ConstraintEqualTo(lineBill3.Superview.LeftAnchor, 25).Active = true;
            lineBill3.RightAnchor.ConstraintEqualTo(lineBill3.Superview.RightAnchor, 0).Active = true;
            lineBill3.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region BillSet_DecimalView
            BillSet_DecimalView.TopAnchor.ConstraintEqualTo(BillSet_CurrencyView.BottomAnchor, 0).Active = true;
            BillSet_DecimalView.LeftAnchor.ConstraintEqualTo(BillSet_DecimalView.Superview.LeftAnchor, 0).Active = true;
            BillSet_DecimalView.RightAnchor.ConstraintEqualTo(BillSet_DecimalView.Superview.RightAnchor, 0).Active = true;
            BillSet_DecimalView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            BillSet_DecimalImg.CenterYAnchor.ConstraintEqualTo(BillSet_DecimalImg.Superview.CenterYAnchor).Active = true;
            BillSet_DecimalImg.LeftAnchor.ConstraintEqualTo(BillSet_DecimalImg.Superview.LeftAnchor, 25).Active = true;
            BillSet_DecimalImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            BillSet_DecimalImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillSet_Decimal.CenterYAnchor.ConstraintEqualTo(lblBillSet_Decimal.Superview.CenterYAnchor).Active = true;
            lblBillSet_Decimal.LeftAnchor.ConstraintEqualTo(BillSet_DecimalImg.RightAnchor, 25).Active = true;
            lblBillSet_Decimal.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillSet_Decimal.RightAnchor.ConstraintEqualTo(lblBillSet_Decimal.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineBill4.BottomAnchor.ConstraintEqualTo(BillSet_DecimalView.BottomAnchor, 0).Active = true;
            lineBill4.LeftAnchor.ConstraintEqualTo(lineBill4.Superview.LeftAnchor, 25).Active = true;
            lineBill4.RightAnchor.ConstraintEqualTo(lineBill4.Superview.RightAnchor, 0).Active = true;
            lineBill4.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region BillSet_ServiceChargeView
            BillSet_ServiceChargeView.TopAnchor.ConstraintEqualTo(BillSet_DecimalView.BottomAnchor, 0).Active = true;
            BillSet_ServiceChargeView.LeftAnchor.ConstraintEqualTo(BillSet_ServiceChargeView.Superview.LeftAnchor, 0).Active = true;
            BillSet_ServiceChargeView.RightAnchor.ConstraintEqualTo(BillSet_ServiceChargeView.Superview.RightAnchor, 0).Active = true;
            BillSet_ServiceChargeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            BillSet_ServiceChageImg.CenterYAnchor.ConstraintEqualTo(BillSet_ServiceChageImg.Superview.CenterYAnchor).Active = true;
            BillSet_ServiceChageImg.LeftAnchor.ConstraintEqualTo(BillSet_ServiceChageImg.Superview.LeftAnchor, 25).Active = true;
            BillSet_ServiceChageImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            BillSet_ServiceChageImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillSet_ServiceChage.CenterYAnchor.ConstraintEqualTo(lblBillSet_ServiceChage.Superview.CenterYAnchor, -12).Active = true;
            lblBillSet_ServiceChage.LeftAnchor.ConstraintEqualTo(BillSet_ServiceChageImg.RightAnchor, 25).Active = true;
            lblBillSet_ServiceChage.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillSet_ServiceChage.RightAnchor.ConstraintEqualTo(lblBillSet_ServiceChage.Superview.RightAnchor, -30).Active = true;

            lblServiceValue.TopAnchor.ConstraintEqualTo(lblBillSet_ServiceChage.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblServiceValue.LeftAnchor.ConstraintEqualTo(BillSet_ServiceChageImg.RightAnchor, 25).Active = true;
            lblServiceValue.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblServiceValue.RightAnchor.ConstraintEqualTo(lblServiceValue.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineBill5.BottomAnchor.ConstraintEqualTo(BillSet_ServiceChargeView.BottomAnchor, 0).Active = true;
            lineBill5.LeftAnchor.ConstraintEqualTo(lineBill5.Superview.LeftAnchor, 25).Active = true;
            lineBill5.RightAnchor.ConstraintEqualTo(lineBill5.Superview.RightAnchor, 0).Active = true;
            lineBill5.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region BillSet_CashView
            BillSet_CashView.TopAnchor.ConstraintEqualTo(BillSet_ServiceChargeView.BottomAnchor, 0).Active = true;
            BillSet_CashView.LeftAnchor.ConstraintEqualTo(BillSet_CashView.Superview.LeftAnchor, 0).Active = true;
            BillSet_CashView.RightAnchor.ConstraintEqualTo(BillSet_CashView.Superview.RightAnchor, 0).Active = true;
            BillSet_CashView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            BillSet_CashImg.CenterYAnchor.ConstraintEqualTo(BillSet_CashImg.Superview.CenterYAnchor).Active = true;
            BillSet_CashImg.LeftAnchor.ConstraintEqualTo(BillSet_CashImg.Superview.LeftAnchor, 25).Active = true;
            BillSet_CashImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            BillSet_CashImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillSet_Cash.CenterYAnchor.ConstraintEqualTo(lblBillSet_Cash.Superview.CenterYAnchor).Active = true;
            lblBillSet_Cash.LeftAnchor.ConstraintEqualTo(BillSet_CashImg.RightAnchor, 25).Active = true;
            lblBillSet_Cash.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillSet_Cash.RightAnchor.ConstraintEqualTo(lblBillSet_Cash.Superview.RightAnchor, -30).Active = true;
            #endregion.

            lineBill7.BottomAnchor.ConstraintEqualTo(BillSet_CashView.BottomAnchor, 0).Active = true;
            lineBill7.LeftAnchor.ConstraintEqualTo(lineBill7.Superview.LeftAnchor, 25).Active = true;
            lineBill7.RightAnchor.ConstraintEqualTo(lineBill7.Superview.RightAnchor, 0).Active = true;
            lineBill7.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region BillSet_GiftVoucherView
            BillSet_GiftVoucherView.TopAnchor.ConstraintEqualTo(BillSet_CashView.BottomAnchor, 0).Active = true;
            BillSet_GiftVoucherView.LeftAnchor.ConstraintEqualTo(BillSet_GiftVoucherView.Superview.LeftAnchor, 0).Active = true;
            BillSet_GiftVoucherView.RightAnchor.ConstraintEqualTo(BillSet_GiftVoucherView.Superview.RightAnchor, 0).Active = true;
            BillSet_GiftVoucherView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            BillSet_GiftVoucherImg.CenterYAnchor.ConstraintEqualTo(BillSet_GiftVoucherImg.Superview.CenterYAnchor).Active = true;
            BillSet_GiftVoucherImg.LeftAnchor.ConstraintEqualTo(BillSet_GiftVoucherImg.Superview.LeftAnchor, 25).Active = true;
            BillSet_GiftVoucherImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            BillSet_GiftVoucherImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillSet_GiftVoucher.CenterYAnchor.ConstraintEqualTo(lblBillSet_GiftVoucher.Superview.CenterYAnchor).Active = true;
            lblBillSet_GiftVoucher.LeftAnchor.ConstraintEqualTo(BillSet_GiftVoucherImg.RightAnchor, 25).Active = true;
            lblBillSet_GiftVoucher.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillSet_GiftVoucher.RightAnchor.ConstraintEqualTo(lblBillSet_GiftVoucher.Superview.RightAnchor, -30).Active = true;
            #endregion.

            lineBill6.BottomAnchor.ConstraintEqualTo(BillSet_GiftVoucherView.BottomAnchor, 0).Active = true;
            lineBill6.LeftAnchor.ConstraintEqualTo(lineBill6.Superview.LeftAnchor, 25).Active = true;
            lineBill6.RightAnchor.ConstraintEqualTo(lineBill6.Superview.RightAnchor, 0).Active = true;
            lineBill6.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region BillSet_SettingMyQRView
            BillSet_SettingMyQRView.TopAnchor.ConstraintEqualTo(BillSet_GiftVoucherView.BottomAnchor, 0).Active = true;
            BillSet_SettingMyQRView.LeftAnchor.ConstraintEqualTo(BillSet_GiftVoucherView.Superview.LeftAnchor, 0).Active = true;
            BillSet_SettingMyQRView.RightAnchor.ConstraintEqualTo(BillSet_GiftVoucherView.Superview.RightAnchor, 0).Active = true;
            BillSet_SettingMyQRView.BottomAnchor.ConstraintEqualTo(BillSet_SettingMyQRView.Superview.BottomAnchor).Active = true;

            BillSet_SettingMyQRImg.CenterYAnchor.ConstraintEqualTo(BillSet_SettingMyQRImg.Superview.CenterYAnchor).Active = true;
            BillSet_SettingMyQRImg.LeftAnchor.ConstraintEqualTo(BillSet_SettingMyQRImg.Superview.LeftAnchor, 25).Active = true;
            BillSet_SettingMyQRImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            BillSet_SettingMyQRImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillSet_SettingMyQr.CenterYAnchor.ConstraintEqualTo(lblBillSet_SettingMyQr.Superview.CenterYAnchor).Active = true;
            lblBillSet_SettingMyQr.LeftAnchor.ConstraintEqualTo(BillSet_SettingMyQRImg.RightAnchor, 25).Active = true;
            lblBillSet_SettingMyQr.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillSet_SettingMyQr.RightAnchor.ConstraintEqualTo(lblBillSet_SettingMyQr.Superview.RightAnchor, -30).Active = true;
            #endregion

            #endregion

            #region DeviceSettingView
            DeviceSettingView.TopAnchor.ConstraintEqualTo(BillSet_SettingMyQRView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            DeviceSettingView.LeftAnchor.ConstraintEqualTo(DeviceSettingView.Superview.LeftAnchor, 0).Active = true;
            DeviceSettingView.RightAnchor.ConstraintEqualTo(DeviceSettingView.Superview.RightAnchor, 0).Active = true;
            DeviceSettingView.HeightAnchor.ConstraintEqualTo(140).Active = true;
            DeviceSettingView.BottomAnchor.ConstraintEqualTo(DeviceSettingView.Superview.BottomAnchor, 0).Active = true;

            lblHeadDeviceSetting.TopAnchor.ConstraintEqualTo(lblHeadDeviceSetting.Superview.TopAnchor, 0).Active = true;
            lblHeadDeviceSetting.LeftAnchor.ConstraintEqualTo(lblHeadDeviceSetting.Superview.LeftAnchor, 15).Active = true;
            lblHeadDeviceSetting.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblHeadDeviceSetting.RightAnchor.ConstraintEqualTo(lblHeadDeviceSetting.Superview.RightAnchor, -30).Active = true;

            #region DevSet_DeviceView
            DevSet_DeviceView.TopAnchor.ConstraintEqualTo(lblHeadDeviceSetting.BottomAnchor, 0).Active = true;
            DevSet_DeviceView.LeftAnchor.ConstraintEqualTo(DevSet_DeviceView.Superview.LeftAnchor, 0).Active = true;
            DevSet_DeviceView.RightAnchor.ConstraintEqualTo(DevSet_DeviceView.Superview.RightAnchor, 0).Active = true;
            DevSet_DeviceView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            DevSet_DeviceImg.CenterYAnchor.ConstraintEqualTo(DevSet_DeviceImg.Superview.CenterYAnchor).Active = true;
            DevSet_DeviceImg.LeftAnchor.ConstraintEqualTo(DevSet_DeviceImg.Superview.LeftAnchor, 25).Active = true;
            DevSet_DeviceImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            DevSet_DeviceImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblDevSet_Device.CenterYAnchor.ConstraintEqualTo(lblDevSet_Device.Superview.CenterYAnchor).Active = true;
            lblDevSet_Device.LeftAnchor.ConstraintEqualTo(DevSet_DeviceImg.RightAnchor, 25).Active = true;
            lblDevSet_Device.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDevSet_Device.RightAnchor.ConstraintEqualTo(lblDevSet_Device.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineDev1.BottomAnchor.ConstraintEqualTo(DevSet_DeviceView.BottomAnchor, 0).Active = true;
            lineDev1.LeftAnchor.ConstraintEqualTo(lineDev1.Superview.LeftAnchor, 25).Active = true;
            lineDev1.RightAnchor.ConstraintEqualTo(lineDev1.Superview.RightAnchor, 0).Active = true;
            lineDev1.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region DevSet_PrinterView
            DevSet_PrinterView.TopAnchor.ConstraintEqualTo(DevSet_DeviceView.BottomAnchor, 0).Active = true;
            DevSet_PrinterView.LeftAnchor.ConstraintEqualTo(DevSet_PrinterView.Superview.LeftAnchor, 0).Active = true;
            DevSet_PrinterView.RightAnchor.ConstraintEqualTo(DevSet_PrinterView.Superview.RightAnchor, 0).Active = true;
            DevSet_PrinterView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            DevSet_PrinterImg.CenterYAnchor.ConstraintEqualTo(DevSet_PrinterImg.Superview.CenterYAnchor).Active = true;
            DevSet_PrinterImg.LeftAnchor.ConstraintEqualTo(DevSet_PrinterImg.Superview.LeftAnchor, 25).Active = true;
            DevSet_PrinterImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            DevSet_PrinterImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblDevSet_Printer.CenterYAnchor.ConstraintEqualTo(lblDevSet_Printer.Superview.CenterYAnchor).Active = true;
            lblDevSet_Printer.LeftAnchor.ConstraintEqualTo(DevSet_PrinterImg.RightAnchor, 25).Active = true;
            lblDevSet_Printer.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblDevSet_Printer.RightAnchor.ConstraintEqualTo(lblDevSet_Printer.Superview.RightAnchor, -30).Active = true;
            #endregion

            lineDev2.BottomAnchor.ConstraintEqualTo(DevSet_PrinterView.BottomAnchor, 0).Active = true;
            lineDev2.LeftAnchor.ConstraintEqualTo(lineDev2.Superview.LeftAnchor, 25).Active = true;
            lineDev2.RightAnchor.ConstraintEqualTo(lineDev2.Superview.RightAnchor, 0).Active = true;
            lineDev2.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            #endregion
        }
    }
}