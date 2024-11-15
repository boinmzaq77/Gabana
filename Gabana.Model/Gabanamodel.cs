using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gabana.Model
{
    public static class GabanaModel
    {
        public static GabanaMain gabanaMain = new GabanaMain();
        public static JWT jwt = new JWT();

        //public static List<Customer> customers = new List<Customer>();
    }
    public class MenuitemHeader
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }

        public MenuitemHeader(int _id, string _description)
        {
            this.MenuId = _id;
            this.MenuName = _description;
        }
    }
    public class ListMyQRCodeIOS
    {
        public List<MyQrCode> myQrCodes;
        static List<MyQrCode> builitem;
        public ListMyQRCodeIOS(List<MyQrCode> myQrCodes)
        {
            builitem = myQrCodes;
            this.myQrCodes = builitem;
        }
        public int Count
        {
            get
            {
                return myQrCodes == null ? 0 : myQrCodes.Count;
            }
        }
        public MyQrCode this[int i]
        {
            get { return myQrCodes == null ? null : myQrCodes[i]; }
        }
    }
    public class MenuitemHeaderIOS
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public bool select { get; set; }

        public MenuitemHeaderIOS(int _id, string _description, bool _select)
        {
            this.MenuId = _id;
            this.MenuName = _description;
            this.select = _select;
        }
    }
    public class LstItemGiftVoucher
    {
        public List<GiftVoucher> vouchers;
        static List<GiftVoucher> builitem;
        public LstItemGiftVoucher(List<GiftVoucher> vouchers)
        {
            builitem = vouchers;
            this.vouchers = builitem;
        }
        public int Count
        {
            get
            {
                return vouchers == null ? 0 : vouchers.Count;
            }
        }
        public GiftVoucher this[int i]
        {
            get { return vouchers == null ? null : vouchers[i]; }
        }
    }
    public class ListItem
    {
        public List<Gabana.ORM.MerchantDB.Item> items;
        static List<Gabana.ORM.MerchantDB.Item> builitem;
        public ListItem(List<Gabana.ORM.MerchantDB.Item> item)
        {
            builitem = new List<Item>();
            items = new List<Item>();
            builitem = item;
            items = builitem;
        }
        public ListItem(List<Gabana.ORM.MerchantDB.Item> item, char type)
        {
            if (type == 'N')
            {
                builitem = item.Where(x => x.SaleItemType == type).ToList();
                this.items = builitem;
            }
            else if (type == 'A')
            {
                builitem = item.Where(x => x.SaleItemType == type).ToList();
                this.items = builitem;
            }
            else
            {
                builitem = item;
                this.items = builitem;
            }
        }
        public int Count
        {
            get
            {
                return items == null ? 0 : items.Count;
            }
        }
        public Gabana.ORM.MerchantDB.Item this[int i]
        {
            get { return items.Count == 0 ? null : items[i]; }
        }
    }
    public class ListPaymentType
    {
        public List<PaymentType> payments;
        static List<PaymentType> builitem;
        public ListPaymentType(List<PaymentType> lstPayment)
        {
            builitem = lstPayment;
            this.payments = builitem;
        }
        public int Count
        {
            get
            {
                return payments == null ? 0 : payments.Count;
            }
        }
        public PaymentType this[int i]
        {
            get { return payments == null ? null : payments[i]; }
        }

    }
    public class ListEmployee
    {
        public List<ORM.MerchantDB.UserAccountInfo> userAccountInfos;
        static List<ORM.MerchantDB.UserAccountInfo> builitem;
        public ListEmployee(List<ORM.MerchantDB.UserAccountInfo> userAccountInfos)
        {
            builitem = userAccountInfos;
            this.userAccountInfos = builitem;

        }
        public int Count
        {
            get
            {
                return userAccountInfos == null ? 0 : userAccountInfos.Count;
            }
        }
        public ORM.MerchantDB.UserAccountInfo this[int i]
        {
            get { return userAccountInfos == null ? null : userAccountInfos[i]; }
        }
    }
    public class ListCustomer
    {
        public List<Gabana.ORM.MerchantDB.Customer> customers;
        static List<Gabana.ORM.MerchantDB.Customer> builitem;
        public ListCustomer(List<Gabana.ORM.MerchantDB.Customer> lstcustomer)
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
        public Gabana.ORM.MerchantDB.Customer this[int i]
        {
            get { return customers == null ? null : customers[i]; }
        }
    }
    public class ListCategory
    {
        public List<ORM.MerchantDB.Category> categories;
        static List<ORM.MerchantDB.Category> builitem;
        public ListCategory(List<ORM.MerchantDB.Category> category)
        {
            builitem = category;
            this.categories = builitem;

        }
        public int Count
        {
            get
            {
                return categories == null ? 0 : categories.Count;
            }
        }
        public ORM.MerchantDB.Category this[int i]
        {
            get { return categories == null ? null : categories[i]; }
        }
    }
    public class MenuitemBody
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuPrice { get; set; }

        public MenuitemBody(int _id, string _description, string _Price)
        {
            this.MenuId = _id;
            this.MenuName = _description;
            this.MenuPrice = _Price;
        }
    }
    public partial class SettingPrinternew
    {
        public string TYPEPAGE { get; set; } // integer
        public string TYPE { get; set; } // integer
        public string USE { get; set; } // integer
        public string BLUETOOTH1 { get; set; } // integer
        public string BLUETOOTH2 { get; set; } // integer
        public string BLUETOOTH3 { get; set; } // integer
        public string BLUETOOTH4 { get; set; } // integer
        public string BLUETOOTH5 { get; set; } // integer
        public string IPADDRESS { get; set; } // integer
        public string PORTNUMBER { get; set; } // integer
        public string TYPESPEED { get; set; } // integer
    }
    public class showNoteDetail
    {
        public string Name { get; set; }
        public int totalItem { get; set; }
        public double SysCategoryID { get; set; }
    }

    public class showNoteData
    {
        public string NoteName { get; set; }
        public string NoteCategoryName { get; set; }
    }

    public class PaymentDetail
    {
        public int Id { get; set; }
        public double sumAmount { get; set; }
        public string optionSelect { get; set; }
        public double moneyCurrent { get; set; }
        public PaymentDetail(int _id, double _sumAmount, string _optionSelect, double _moneyCurrent)
        {
            this.Id = _id;
            this.sumAmount = _sumAmount;
            this.optionSelect = _optionSelect;
            this.moneyCurrent = _moneyCurrent;
        }
    }
    public class GabanaMain
    {
        public List<Profile> profile { get; set; }
        public List<Menuitem> menu { get; set; }
        public List<ItemSize> listsize { get; set; }

        public List<Currency> currency { get; set; }
        public List<Bluetooth> bluetooth { get; set; }

        public List<EmployeeRole> empRole { get; set; }
        public List<PaymentType> payments { get; set; }
        public List<PackageProduce> packages { get; set; }
    }
    public class JWT
    {
        public string Jwt1 { get; set; }
        public string Jwt2 { get; set; }

    }
    public class ResultAPI
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ResultAPI(bool s, string m)
        {
            Status = s;
            Message = m;
        }
    }

    public class TokenResult
    {
        public bool status { get; set; }
        public string gbnJWT { get; set; }
    }

    //public class Branch
    //{
    //    public string BranchName { get; set; }
    //}

    public class Profile
    {
        public int MerchantId { get; set; }
        public string Company { get; set; }
        public string Branch { get; set; }
        public string ProfileImage { get; set; }
    }

    public class MenuitemProfile
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }

        public MenuitemProfile(int _id, string _icon, string _description)
        {
            this.MenuId = _id;
            this.MenuIcon = _icon;
            this.MenuName = _description;
        }
    }

    public class VerifyOTP
    {
        public string OwnerID { get; set; }
        public string OTP { get; set; }
        public string RefOTP { get; set; }

    }
    public class RenewiOS
    {
        public string MerchantID { get; set; }
        public string OriginalTransactionID { get; set; }

    }
    public class LoginEmp
    {
        public string MerchantID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
    public class SendOTP
    {
        public string OwnerID { get; set; }
        public string OTP { get; set; }
        public string UDID { get; set; }

    }

    public class POSMenuitem
    {
        public string POSMenuName { get; set; }

        public POSMenuitem(string _description)
        {
            this.POSMenuName = _description;
        }
    }

    public class itemPOS
    {
        public string itemName { get; set; }
        public decimal itemCost { get; set; }
        public string itemImage { get; set; }
        public string itemType { get; set; }
        public itemPOS(string _description, decimal _cost, string _type, string _image)
        {
            this.itemName = _description;
            this.itemImage = _image;
            this.itemCost = _cost;
            this.itemType = _type;
        }
    }
    public class Menuitem
    {
        public string MenuName { get; set; }
        public string MenuIcon { get; set; }
        public Menuitem(string _description, string _image)
        {
            this.MenuName = _description;
            this.MenuIcon = _image;
        }
    }
    public class Bluetooth
    {
        public string id { get; set; }
        public string BluetoothName { get; set; }
        public string BluetoothStatus { get; set; }
        public string Address { get; set; }

    }
    public class Bluetooth2
    {
        public Guid id { get; set; }
        public string BluetoothName { get; set; }
        public string BluetoothStatus { get; set; }

    }

    public class ItemSize
    {
        public string SizeName { get; set; }
        public decimal Price { get; set; }
        public decimal Estimate { get; set; }

        public ItemSize(string _description, decimal _price, decimal _cost)
        {
            this.SizeName = _description;
            this.Price = _price;
            this.Estimate = _cost;

        }
    }
    public class MenuPayment
    {
        public string itemName { get; set; }
        public string itemImage { get; set; }
        public MenuPayment(string _description, string _image)
        {
            this.itemName = _description;
            this.itemImage = _image;
        }
    }
    public class Member
    {
        public int memberID { get; set; }
        public string MerchantName { get; set; }
        public string LogoPath { get; set; }
    }
    public class CartItem
    {
        public int type { get; set; }
        public Item detail { get; set; }
        public ItemFootBill footBills { get; set; }
        public string total { get; set; }
        public int amount { get; set; }
    }
    public class ItemFootBill
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class TranWithDetailsLocal
    {
        public Tran tran { get; set; }
        public List<TranDetailItemWithTopping> tranDetailItemWithToppings { get; set; }
        public List<TranTradDiscount> tranTradDiscounts { get; set; }
        public List<TranPayment> tranPayments { get; set; }

        public TranWithDetailsLocal()
        {
            tranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
            tranTradDiscounts = new List<TranTradDiscount>();
            tranPayments = new List<TranPayment>();
        }

    }
    public class TranDetailItemWithTopping
    {
        public TranDetailItemNew tranDetailItem { get; set; }
        public List<TranDetailItemTopping> tranDetailItemToppings { get; set; }

    }
    public class TranDetailItemNew : TranDetailItem
    {
        public bool choose { get; set; }
    }
    public class TranWithDetailsLocalResult
    {
        public Tran tran { get; set; }
        public List<TranDetailItem> tranDetailItems { get; set; }
        public List<TranPayment> tranPayments { get; set; }

        public TranWithDetailsLocalResult()
        {
            tranDetailItems = new List<TranDetailItem>();
            tranPayments = new List<TranPayment>();
        }
    }

    public class MenuitemName
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public int MenuIcon { get; set; }
        public MenuitemName(int _id, int _image, string _description)
        {
            this.MenuID = _id;
            this.MenuName = _description;
            this.MenuIcon = _image;
        }
    }

    public class MenuTab
    {
        public string NameMenuEn { get; set; } // varchar(1)
        public string NameMenuTh { get; set; } // varchar(1)
    }

    public class MenuTabwithSysCategory
    {
        public string NameMenuEn { get; set; } // varchar(1)
        public string NameMenuTh { get; set; } // varchar(1)
        public long SysCategory { get; set; }
    }

    public class TransHistory
    {
        public string tranNo { get; set; }
        public DateTime tranDate { get; set; } // utc
        public string customerName { get; set; }
        public decimal grandTotal { get; set; }
        public string paymentType { get; set; }  // แสดงรูปแบบการชำระเงินรายการแรกของบิล 2021-07-27 แก้ไขตาม UI
        public decimal fCancel { get; set; }
        //public string ePaymentType { get; set; }
        public bool fhead { get; set; }
    }

    public class TransHistoryNew : TransHistory
    {
        public char TypeOfflineOrOnline { get; set; } // 'O' online - 'F' offine
        public int FWaiting { get; set; } // รอส่ง 1  หรือ ส่งไม่สำเร็จ 2
    }
    

    public class Currency
    {
        public string CurrencyType { get; set; }
        public int LogoCurrency { get; set; } // int resource id
        public int LogoCurrency2 { get; set; } // int resource id
        public string CurrencyNameEn { get; set; } // varchar(1)
        public string CurrencyNameTh { get; set; } // varchar(1)
    }

    public class UserAccount
    {
        public string UserName { get; set; } //id
        public string FullName { get; set; }
        public string MainRoles { get; set; }
        public string PasswordHash { get; set; } // password
        public string Code { get; set; } // Code 31/08/65 -- เพิ่มจากพี่โน๊ต
        public string MerchantName { get; set; } // MerchantName 01/09/65 -- เพิ่มจากพี่โน๊ต

    }

    public class UserAccountInfo : SeAuth2.ORM.UserAccount
    {
        public string Mobile { get; set; }
        public string ListUserAccessProduct { get; set; }
        public string ListSeniorStaff { get; set; }
        public bool UserAccessProduct { get; set; }

    }

    public class ChangePassword
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class EmployeeRole
    {
        public int ImagePosition { get; set; }
        public string Position { get; set; }
        public string Details { get; set; }
    }

    public class ItemMovement
    {
        public int MerchantID { get; set; } // integer
        public int SysBranchID { get; set; } // integer
        public int SysItemID { get; set; } // integer
        public int MovementNo { get; set; } // integer
        public DateTime MovementDate { get; set; } // timestamp (6) without time zone
        public char MovementType { get; set; } // character varying(1)
        public string RefTranNo { get; set; } // character varying(25)
        public decimal Quantity { get; set; } // numeric(18,4)
        public string UserName { get; set; } // character varying(50)
    }
    public class TimeReport
    {
        public string time { get; set; }
        public string hour { get; set; }
    }
    public class PaymentType
    {
        public string Type { get; set; }
        public string Detail { get; set; }
        public int Logo { get; set; }
        public string color { get; set; }
    }

    public class InsertRepeatItem
    {
        public bool checkManageStock { get; set; }
        public Item DetailITem { get; set; }
        public string Stock { get; set; }
        public string minimumstock { get; set; }
    }

    public class PaymentTypeAmount
    {
        public string TypePayment { get; set; }
        public string DetailType { get; set; }
        public decimal amount { get; set; }

    }

    public class OrderNew : Gabana3.JAM.Trans.Order
    {
        public char TypeOfflineOrOnline { get; set; } // 'O' online - 'F' offine
        public decimal FWaiting { get; set; } // รอส่ง 1  หรือ ส่งไม่สำเร็จ 2

        public bool Fhead { get; set; }
    }

    public class SaleReportBranch
    {
        public int BranchID { get; set; }
        public decimal sumGrandTotal { get; set; }
        public string BranchName { get; set; }
    }

    public class ReportProfit
    {
        public DateTime DateTime { get; set; }
        public string dateTime { get; set; }
        public decimal sumGrandTotal { get; set; }
        public decimal sumProfitTotal { get; set; }
    }

    public class EmployeeReport
    {
        public string sellerName { get; set; }
        public string MainRoles { get; set; }
        public decimal sumTotalAmount { get; set; }

    }
    public class PackageProduce
    {
        public int id { get; set; }
        public string ProductId { get; set; }
        public string PackageName { get; set; }
        public string MaxBranch { get; set; }
        public string MaxUser { get; set; }
        public string Price { get; set; }
    }


    public class CloudProductLicence : SeAuth2.ORM.CloudProductLicence
    {
        //public int MerchantID { get; set; } // int
        //public int CloudProductID { get; set; } // int
        //public int TotalBranch { get; set; } // int
        //public char FStatus { get; set; } // nvarchar(1)
        //public DateTime? ActiveUntilDate { get; set; } // date
        //public string Comments { get; set; } // nvarchar(250)
    }

    public class RenewModel
    {
        public string Id { get; set; }
        public DateTime TransactionDateUtc { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public List<string> ProductIds { get; set; }
        public bool AutoRenewing { get; set; }
        public string PurchaseToken { get; set; }
        public int State { get; set; }
        public int ConsumptionState { get; set; }
        public bool IsAcknowledged { get; set; }
        public string ObfuscatedAccountId { get; set; }
        public string ObfuscatedProfileId { get; set; }
        public string Payload { get; set; }
        public string OriginalJson { get; set; }
        public string Signature { get; set; }
    }

    public class GabanaLicenceModel
    {
        public int MerchantID { get; set; }
        public string PromotionCode { get; set; }
    }

    public class GabanaInfo
    {
        public int MerchantID { get; set; }
        public int CloudProductID { get; set; } //รหัสแสดงแอป gabana
        public int TotalBranch { get; set; }
        public int TotalUser { get; set; }
        public char FStatus { get; set; }
        public DateTime? ActiveUntilDate { get; set; }
        public string Comments { get; set; }
    }


    public class GabanaLicenceDetail
    {
        public int TotalDayRecieved { get; set; } //mylicence
        public DateTime ExpiryDate { get; set; }

    }

    public class ErrorCodePromotion
    {
        public const string Promotion_code_is_null = "101";
        public const string MyProductLicence_is_not_found = "102";
        public const string This_product_has_already_charged_a_commission = "103";
        public const string You_have_already_used_the_permissions = "104";
        public const string Sellers_has_been_cancelled = "105";
        public const string Bonus_code_has_been_cancelled = "106";
        public const string Bonus_code_expires = "107";
        public const string Seller_product_has_been_cancelled = "108";
        public const string Seller_product_expires = "109";
        public const string Promotion_code_is_not_found = "111";
        public const string Promotion_code_has_been_used = "112";
        public const string Promotion_code_has_been_cancelled = "113";
        public const string Promotion_code_expires = "114";
        public const string Bonus_code_is_not_found = "115";
        public const string Already_have_gabana_product = "116";
        public const string ObfuscatedAccountId_not_found = "117"; //ObfuscatedAccountId = Merchantid form_GooglePay
        public const string Merchant_not_found = "118";

        //Seauth2api
        public const string CloudProductLicence_not_found = "201";
        public const string ObfuscatedAccountId_not_found_Seauth2Api = "202"; //ObfuscatedAccountId = Merchantid form_GooglePay
        public const string Merchant_not_found_Seauth2Api = "203";
    }

    public class HasChangeinCart
    {
        public TranWithDetailsLocal tranWithDetailsLocal { get; set; }
        public bool FlagChange { get; set; }

        public HasChangeinCart()
        {
            tranWithDetailsLocal = new TranWithDetailsLocal();
            FlagChange = false;
        }
    }

    public class respone_QrKBank
    {
        public string partnerTxnUid { get; set; }
        public string partnerId { get; set; }
        public string statusCode { get; set; }
        public string errorCode { get; set; }
        public string errorDesc { get; set; }
        public string accountName { get; set; }
        public string qrCode { get; set; }
        public List<string> sof { get; set; }
        public string txnStatus { get; set; }
    }

    public class Status_QrKBank
    {
        public string statusCode { get; set; }
        public string txnStatus { get; set; }
        public string errorDesc { get; set; }
    }

    public class DefaultAllData
    {
        public List<Item> DefaultDataItem { get; set; }
        public List<Item> DefaultDataTopping { get; set; }
        public List<Item> DefaultDataItemonBranch { get; set; }
        public List<Item> DefaultAllItem { get; set; }
        public List<Category> DefaultDataCategory { get; set; }
        public List<Item> AllItemStatusD { get; set; }

        public DefaultAllData()
        {
            this.DefaultDataItem = new List<Item>();
            this.DefaultDataTopping = new List<Item>();
            this.DefaultDataItemonBranch = new List<Item>();
            this.DefaultDataCategory = new List<Category>();
            this.AllItemStatusD = new List<Item>();
            this.DefaultAllItem = new List<Item>();
        }
    }
}
