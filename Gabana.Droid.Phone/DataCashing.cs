using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System.Collections.Generic;

namespace Gabana.Droid
{
    public class DataCashing
    {
        public static string sqliteMerchantDB = "MerchantDB140.db";
        public static long EditItemID = 0;
        public static Note EditNoteItem;
        public static bool EditCustomer;
        public static long? SysCustomerID;
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
        public static bool CheckRoleSwipe;
        public static bool addEmployeefromSeauth = false;
        public static bool isModifyBranch = false;
        public static bool isModifyRole = false;
        public static bool isCurrentOrder = false;

        public static bool flagProgress = false;
        public static int reconnect = 0;

        //Path Image When TakePhoto because Uri path is null
        public static Android.Net.Uri PathTakePhoto;
        public static bool isModifyBillhistory = false;
        public static CountGenQr countGen = new CountGenQr(); 


    }

    public partial class CountGenQr
    {
        public int countgenQr { get; set; }
        public string Tranno { get; set; }
    }
}