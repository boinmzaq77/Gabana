using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Gabana.Model;
using Gabana.ORM.PoolDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using UIKit;

namespace Gabana.iOS
{
    static class DataCaching
    {
        public static SettingPrinter setting = new SettingPrinter();
        public static UIViewController SplashLoadingController;
        public static UINavigationController MainNavigation;
        public static UINavigationController LoginNavigation;
        public static UINavigationController TermsNavigation;

        public static UIBarButtonItem TitlePage;

        public static UINavigationController UpdateProfileNavigation;
        public static UINavigationController CheckOutNavigation;
        public static UINavigationController DetailItemNavigation;
        public static UINavigationController BillHistoryNavigation;
        internal static ItemsController itempage;
        public static EmployeeController employeeController;  
        public static string deviceNo;
        public static string PathFolderImage;
        public static List<Gabana.ORM.PoolDB.Province> Provinces;
        public static string UserNameEmp;
        public static string PositionEmp;
        public static Gabana.ORM.MerchantDB.Branch branchDeatail;
        public static List<string> Membertype;
        public static POSController posPage ;
        public static PaymentController paymentpage; 
        public static UIViewController mainpage;

        public static long NewItem { get; internal set; }
    }
}