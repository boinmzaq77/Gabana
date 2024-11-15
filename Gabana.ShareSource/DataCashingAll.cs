using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana3.JAM.Merchant;

namespace Gabana.ShareSource
{
    public class DataCashingAll
    {
        public static string PathdbPrototype;
        public static string Pathdb;
        public static string PathdbPrototypepool;
        public static string Pathdbpool;
        public static string sqliteMerchantDB = "MerchantDB.db";
        public static string sqlitePoolDB = "PoolDB.db";
        public static string DevicePlatform;
        public static string DeviceUDID;
        public static Merchants Merchant;
        public static Merchant MerchantLocal;
        public static int MerchantId;
        public static int SysBranchId;
        public static int DeviceNo;
        public static int TaxRate = 7;
        public static byte[] imageByteArray;
        //public static bool CheckConnectInternet;
        public static List<Gabana.ORM.PoolDB.Province> Provinces;
        public static List<Model.UserAccountInfo> UserAccountInfo;
        public static List<Gabana.ORM.Master.BranchPolicy> BranchPolicy;
        public static List<UserAccount> listUserAccount;
        public static SettingPrinter setting = new SettingPrinter();
        public static SetMerchantConfig setmerchantConfig;
        public static Device Device;
        public static string Pathdbpoolnew;
        public static string PathThumnailFolderImage;
        public static string PathFolderImage;
        public static string PathImageBill;
        public static InsertImage InsertImage;
        public static GabanaInfo GetGabanaInfo;
        public static bool UsePinCode;
        public static DateTime UserActive;
        public static string PrintType;
        public static string intUsePinCode;
        public static string printerName;
        public static string addresssame;

        public static bool CheckConnectInternet { get; set; }

        //Set Flag When Data is Upsdate Local
        public static bool flagItemChange { get; set; }
        public static bool flagCategoryChange { get; set; }
        public static bool flagNoteChange { get; set; }
        public static bool flagNoteCategoryChange { get; set; }
        public static bool flagCustomerChange { get; set; }
        public static bool flagItemOnBranchChange { get; set; }
        public static bool flagMemberTypeChange { get; set; }
        public static bool flagMerchantChange { get; set; }
        public static bool flagMerchantConfigChange { get; set; }
        public static bool flagMyQrCodeChange { get; set; }
        public static bool flagGiftVoucherChange { get; set; }
        public static bool flagBranchChange { get; set; }
        public static bool flagCashTemplateChange { get; set; }  
        

        public static bool flagNotificationEnble { get; set; }

        //Set Flag When Data is Upsdate Online

        public static bool flagItemChangeOnline { get; set; }
        public static bool flagCategoryChangeOnline { get; set; }
        public static bool flagNoteChangeOnline { get; set; }
        public static bool flagNoteCategoryChangeOnline { get; set; }
        public static bool flagCustomerChangeOnline { get; set; }
        public static bool flagMerchantChangeOnline { get; set; }
        public static bool flagItemOnBranchChangeOnline { get; set; }
        public static bool flagMemberTypeChangeOnline { get; set; }
        public static bool flagMerchantConfigChangeOnline { get; set; }
        public static bool flagMyQrCodeChangeOnline { get; set; }
        public static bool flagGiftVoucherChangeOnline { get; set; }
        public static bool flagBranchChangeOnline { get; set; }
        public static bool flagCashTemplateChangeOnline { get; set; }
        public static bool CheckRoleSwipe { get; set; }
    }

    public partial class SettingPrinter
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

        public string PRINTTYPE { get; set; } // string
        public string COMMAND { get; set; } // string

    }

    public partial class SetMerchantConfig
    {
        public string TAXTYPE { get; set; } // string
        public string TAXRATE { get; set; } // integer
        public string CURRENCY_SYMBOLS { get; set; } // string
        public string DECIMAL_POINT_CALC { get; set; } // integer
        public string DECIMAL_POINT_DISPLAY { get; set; } // integer
        public string OPTION_ROUNDING_STRING { get; set; } // integer 
        public string OPTION_ROUNDING_INT { get; set; } //  string
        public string SERVICECHARGE_TYPE { get; set; } // string
        public string SERVICECHARGE_RATE { get; set; } // string
        public string PRINTER_DEFAULT { get; set; } // string
        public string SUBSCRIPTION_TYPE { get; set; } // string
        public string CASHDRAWER { get; set; } // string
    }

    public  partial class Device
    {
        public int MerchantID { get; set; } // int
        public int DeviceNo { get; set; } // int
        public string Platform { get; set; } // character varying(4)
        public string UDID { get; set; }
        public string DeviceInfo { get; set; } // string
        public DateTime DateCreated    { get; set; } // timestamp (6) without time zone
        public DateTime DateLastActive { get; set; } // timestamp (6) without time zone
        public string Comments { get; set; } = ""; // string
    }

    public partial class InsertImage
    {
        public Customer customer { get; set; }
        public byte[] byteImage { get; set; }         
    }

    public partial class CreateNewMerchant
    {
        public int MerchantID { get; set; }
        public bool createNew { get; set; }
    }

    public partial class GetGabanaInfo
    {
        public int MerchantID { get; set; }
        public int CloudProductID { get; set; } //รหัสแสดงแอป gabana
        public int TotalBranch { get; set; }
        public int TotalUser { get; set; }
        public char FStatus { get; set; }
        public DateTime? ActiveUntilDate { get; set; }
        public string Comments { get; set; }
    }
}
