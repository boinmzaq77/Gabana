using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System.Collections.Generic;


namespace Gabana.Droid.Tablet
{
    public class DataCashing
    {
        public static string sqliteMerchantDB = "MerchantDB140.db";
        public static long EditItemID;
        public static Note EditNote;
        public static MemberType EditMemberType;
        public static GiftVoucher EditGiftVoucher;
        public static MyQrCode EditMyQR;
        public static Category EditCategory;
        public static Customer EditCus;
        public static Item EditItem;
        public static Branch EditBranch;
        public static CashTemplate EditCashGuide;
        public static Item EditTopping;
        public static TransHistoryNew billHistory;
        public static List<ORM.Master.MemberType> DeleteMemberType;
        public static ORM.MerchantDB.UserAccountInfo EditEmployee;
        public static string EditSysCategory;
        public static long EditSysCategoryID;
        public static string EditDiscount;
        public static long EditSysDiscountTemplate;
        public static string Namecustomer;
        public static int setQuantityToCart;
        public static bool ChangePayment = false;
        public static bool NewSale = false;
        public static bool flagScan = false;
        public static bool flagCart = false;
        public static bool flagDummy = false;
        public static Item DialogShowItem;
        public static bool EditCustomer;
        public static string iSysCustomerID;
        public static long? SysCustomerID;
        public static List<string> Provinces;
        public static List<string> Amphures;
        public static List<string> Districts;
        public static List<string> Branch;
        public static List<string> Membertype;
        public static TranWithDetailsLocal tranWithDetails;
        public static string PaymentType;
        //public static int PaymentNo;
        public static bool flagEditQuantity;
        public static int EditQuantity;
        public static int ListBillCount;
        public static bool AddDiscount;
        public static string Language;

        public static bool flagEditOptionSize = false;
        public static bool flagEditOptionExtraTopping = false;
        public static bool flagEditOptionNote = false;

        public static Gabana.ORM.MerchantDB.Branch branchDeatail;
        public static bool ModifyTranOrder = false;

        public static List<int> StatusSwipe;
        public static bool addEmployeefromSeauth = false;
        public static bool isModifyBranch = false;
        public static bool isModifyRole = false;
        public static bool isCurrentOrder = false;

        public static bool flagProgress = false;
        public static int reconnect = 0;

        public static bool flagChooseMedia = false;
        internal static bool CheckNet;
        public static bool flagAmountGiftVoucher = false;
        public static bool isModifyBillhistory = false;
        public static CountGenQr countGen = new CountGenQr();
        public static bool saveqrReceipt = false;
    }

    public partial class CountGenQr
    {
        public int countgenQr { get; set; }
        public string Tranno { get; set; }
    }

}