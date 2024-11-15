using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.Res;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Telephony;
using Android.Util;
using Android.Widget;
using FFImageLoading;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.Period;
using Gabana.ShareSource;
using Gabana.ShareSource.ClassStructure;
using Gabana.ShareSource.Controller;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using Java.Util;
using LinqToDB.Common;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using static Android.BillingClient.Api.BillingClient;
using static ZXing.QrCode.Internal.Mode;

namespace Gabana.Droid
{
    public class Utils : AppCompatActivity
    {
        public static string pathdb;
        public static string filepath;
        public static GeneratedTable table;
        public static string nameTH, nameEn;
        //List<Branch> lstbranch = new List<Branch>();
        List<Item> lstitem = new List<Item>();
        List<Item> lstitem1 = new List<Item>();
        List<Category> lstcategory = new List<Category>();
        List<ORM.MerchantDB.Tran> lsttrans = new List<ORM.MerchantDB.Tran>();
        List<ORM.MerchantDB.Tran> lsttransPrinCounter = new List<ORM.MerchantDB.Tran>();
        List<Customer> lstcustomer = new List<Customer>();
        List<NoteCategory> lstnotecategory = new List<NoteCategory>();
        List<Note> lstnote = new List<Note>();

        BranchManage branchManage = new BranchManage();
        ItemManage itemManage = new ItemManage();
        CategoryManage categoryManage = new CategoryManage();
        TransManage transManage = new TransManage();
        CustomerManage CustomerManage = new CustomerManage();
        NoteCategoryManage NoteCategoryManage = new NoteCategoryManage();
        NoteManage NoteManage = new NoteManage();
        private static byte[] imageItemByteArray;
        private static byte[] imageMaster;

        public async static Task<bool> CreateMerchnatDB(int merchantID, Java.IO.File file)
        {
            var sqliteFilename = DataCashing.sqliteMerchantDB;
            try
            {
                string documentsDirectoryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                pathdb = Path.Combine(documentsDirectoryPath, sqliteFilename);
                // Copy database from assets if it doesn't exist
                if (!File.Exists(pathdb))
                {
                    using (var assetStream = Android.App.Application.Context.Assets.Open(sqliteFilename))
                    using (var fileStream = new FileStream(pathdb, FileMode.Create))
                    {
                        await assetStream.CopyToAsync(fileStream);
                    }
                }

                // Create directory and copy database file to it
                var dirPath = Path.Combine(file.Path, merchantID.ToString());
                Directory.CreateDirectory(dirPath);
                filepath = Path.Combine(dirPath, sqliteFilename);
                if (!File.Exists(filepath))
                {
                    File.Copy(pathdb, filepath, true);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CreateMerchnatDB");
                Console.WriteLine(ex.Message);
                Log.Error("connecterror", "CreateMerchnatDB" + ex.Message);
                return false;
            }
            DataCashingAll.PathdbPrototype = pathdb;
            DataCashingAll.Pathdb = filepath;

            Preferences.Set("PathMerchantDB", filepath);
            return true;
        }

        public async static Task<bool> CreatePoolDB(int merchantID, Java.IO.File file)
        {
            var sqliteFilename = DataCashingAll.sqlitePoolDB;
            try
            {
                string documentsDirectoryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                pathdb = Path.Combine(documentsDirectoryPath, sqliteFilename);

                // Copy database from assets if it doesn't exist
                if (!File.Exists(pathdb))
                {
                    using (var assetStream = Android.App.Application.Context.Assets.Open(sqliteFilename))
                    using (var fileStream = new FileStream(pathdb, FileMode.Create))
                    {
                        await assetStream.CopyToAsync(fileStream);
                    }
                }

                // Create directory and copy database file to it
                var dirPath = Path.Combine(file.Path, merchantID.ToString());
                Directory.CreateDirectory(dirPath);
                filepath = Path.Combine(dirPath, sqliteFilename);
                if (!File.Exists(filepath))
                {
                    File.Copy(pathdb, filepath, true);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CreatePoolDB");
                Log.Error("connecterror", "CreatePoolDB" + ex.Message);
                return false;
            }
            DataCashingAll.PathdbPrototypepool = pathdb;
            DataCashingAll.Pathdbpool = filepath;

            Preferences.Set("PathPoolDB", filepath);
            return true;

            //table = new GeneratedTable();
            //var result = table.CreateTable(sqliteFilename);
        }

        public async static Task<bool> DeleteMerchnatDB(int merchantID, Java.IO.File file)
        {
            var sqliteFilename = DataCashing.sqliteMerchantDB;
            try
            {
                string documentsDirectoryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                pathdb = Path.Combine(documentsDirectoryPath, sqliteFilename);

                var dirPath = file + "/" + merchantID;
                var exists = Directory.Exists(dirPath);
                filepath = dirPath + "/" + sqliteFilename;
                if (exists)
                {
                    Directory.Delete(dirPath, true);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("DeleteMerchnatDB");
                return false;
            }
            return true;
        }

        public async static Task<bool> DeletePoolDB(int merchantID, Java.IO.File file)
        {
            var sqliteFilename = DataCashing.sqliteMerchantDB;
            try
            {
                string documentsDirectoryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                pathdb = Path.Combine(documentsDirectoryPath, sqliteFilename);

                var dirPath = file + "/" + merchantID;
                var exists = Directory.Exists(dirPath);
                filepath = dirPath + "/" + sqliteFilename;
                if (exists)
                {
                    Directory.Delete(dirPath, true);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("DeleteMerchnatDB");
                return false;
            }
            return true;
        }

        internal static List<string> SetDetailPackage(string id)
        {
            //return maxuser , maxbranch
            List<string> list = new List<string>();
            switch (id)
            {
                case "1":
                    list = new List<string>() { "5", "1", "759.00" };
                    return list;
                case "2":
                    list = new List<string>() { "10", "1", "1,250.00" };
                    return list;
                case "3":
                    list = new List<string>() { "15", "3", "2,300.00" };
                    return list;
                case "4":
                    list = new List<string>() { "30", "3", "3,800.00" };
                    return list;
                default:
                    list = new List<string>() { DataCashingAll.GetGabanaInfo.TotalUser.ToString(), DataCashingAll.GetGabanaInfo.TotalBranch.ToString(), "" };
                    return list;

            }
        }

        internal static int SetPackageID(int maxbranch, int maxuser)
        {
            //return maxuser , maxbranch
            switch (maxuser)
            {
                case 5:
                    if (maxbranch == 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                case 10:
                    if (maxbranch == 1)
                    {
                        return 2;
                    }
                    else
                    {
                        return -1;
                    }
                case 15:
                    if (maxbranch == 3)
                    {
                        return 3;
                    }
                    else
                    {
                        return -1;
                    }
                case 30:
                    if (maxbranch == 3)
                    {
                        return 4;
                    }
                    else
                    {
                        return -1;
                    }
                default:
                    return -1;

            }
        }

        public static void SetImage(ImageView imageView, string PicturePath)
        {
            string pathPlaceholder = "@mipmap/placeholder_card";
            if (!string.IsNullOrEmpty(PicturePath))
            {
                ImageService.Instance.LoadUrl(PicturePath)
                    .LoadingPlaceholder(pathPlaceholder, FFImageLoading.Work.ImageSource.CompiledResource)
                    .WithCache(FFImageLoading.Cache.CacheType.Disk)
                    .Into(imageView);
            }
        }

        public static void SetPaymentImage(ImageView imageView, string PaymentType)
        {
            int path;
            if (PaymentType == null)
            {
                return;
            }
            switch (PaymentType.ToUpper())
            {
                case "CH":
                    path = Resource.Mipmap.HistoryCash;
                    break;
                case "DR":
                    path = Resource.Mipmap.HistoryDebit;
                    break;
                case "CR":
                    path = Resource.Mipmap.HistoryDebit;
                    break;
                case "GV":
                    path = Resource.Mipmap.HistoryGiftvoucher;
                    break;
                case "MYQR":
                    path = Resource.Mipmap.HistoryQR;
                    break;
                case "QRCH":
                    path = Resource.Mipmap.HistoryQRCash;
                    break;
                case "QRCR":
                    path = Resource.Mipmap.HistoryQRCredit;
                    break;
                case "WECHAT":
                    path = Resource.Mipmap.HistoryWechat;
                    break;
                default:
                    path = Resource.Mipmap.Cash;
                    break;
            }
            imageView.SetBackgroundResource(path);
        }

        public static string SetPaymentName(string PaymentType)
        {
            string name;

            switch (PaymentType.ToUpper())
            {
                case "CH":
                    nameEn = "Cash";
                    nameTH = "ชำระด้วย เงินสด";
                    break;
                case "DR":
                    nameEn = "Debit Card";
                    nameTH = "ชำระด้วย บัตรเดบิต";
                    break;
                case "CR":
                    nameEn = "Credit Card";
                    nameTH = "ชำระด้วย บัตรเครดิต";
                    break;
                case "GV":
                    nameEn = "Gift Voucher";
                    nameTH = "ชำระด้วย บัตรของขวัญ";
                    break;
                case "RP":
                    nameEn = "Redeem Point";
                    nameTH = "ชำระด้วยการ Redeem";
                    break;
                case "EP":
                    nameEn = "ePayment";
                    nameTH = "ePayment";
                    break;
                case "QR":
                    nameEn = "Cash by QR";
                    nameTH = "ชำระด้วย QR";
                    break;
                case "QRCH":
                    nameEn = "QR Cash";
                    nameTH = "ชำระด้วย QR Cash";
                    break;
                case "QRCR":
                    nameEn = "QR Credit";
                    nameTH = "ชำระด้วย QR Credit";
                    break;
                case "MYQR":
                    nameEn = "My QR";
                    nameTH = "ชำระด้วย My QR";
                    break;
                case "WECHAT":
                    nameEn = "Wechat";
                    nameTH = "ชำระด้วย Wechat";
                    break;
                default:
                    break;
            }
            if (DataCashing.Language == "th")
            {
                name = nameTH;
            }
            else
            {
                name = nameEn;
            }

            return name;
        }

        public static string SetPaymentNameRP(string PaymentType)
        {
            string name;

            switch (PaymentType.ToUpper())
            {
                case "CH":
                    nameEn = "Cash";
                    nameTH = "เงินสด";
                    break;
                case "DR":
                    nameEn = "Debit Card";
                    nameTH = "บัตรเดบิต";
                    break;
                case "CR":
                    nameEn = "Credit Card";
                    nameTH = "บัตรเครดิต";
                    break;
                case "GV":
                    nameEn = "Gift Voucher";
                    nameTH = "บัตรของขวัญ";
                    break;
                case "QRCH":
                    nameEn = "QR Cash";
                    nameTH = "คิวอาร์เงินสด";
                    break;
                case "QRCR":
                    nameEn = "QR Credit";
                    nameTH = "คิวอาร์เครดิต";
                    break;
                case "MYQR":
                    nameEn = "My QR";
                    nameTH = "คิวอาร์ของฉัน";
                    break;
                case "WECHAT":
                    nameEn = "Wechat";
                    nameTH = "วีแชท";
                    break;
                default:
                    break;
            }
            if (DataCashing.Language == "th")
            {
                name = nameTH;
            }
            else
            {
                name = nameEn;
            }

            return name;
        }

        public static string SetPaymentNameChart(string PaymentType)
        {
            string name;

            switch (PaymentType.ToUpper())
            {
                case "CH":
                    name = "Cash";
                    break;
                case "DR":
                    name = "Debit Card";
                    break;
                case "CR":
                    name = "Credit Card";
                    break;
                case "GV":
                    name = "Gift Voucher";
                    break;
                case "RP":
                    name = "Redeem Point";
                    break;
                case "EP":
                    name = "ePayment";
                    break;
                case "QR":
                    name = "Cash by QR";
                    break;
                case "QRCH":
                    name = "QR Cash";
                    break;
                case "QRCR":
                    name = "QR Credit";
                    break;
                case "MYQR":
                    name = "My QR";
                    break;
                case "WECHAT":
                    name = "Wechat";
                    break;
                default:
                    name = "";
                    break;
            }
            return name;
        }

        public static void SetButton(Button button)
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;
            button.LayoutParameters.Height = Convert.ToInt32(Convert.ToDecimal(height * 0.059).ToString("##"));
        }


        //--------------------------------
        //Resent 
        //ทุก Table ที่มี Fwaitsending  = 2
        //--------------------------------

        private void HandleException(Exception ex, string source)
        {
            //_ = TinyInsights.TrackErrorAsync(ex);
            //_ = TinyInsights.TrackPageViewAsync(source);
        }

        private static void HandlestaticException(Exception ex, string source)
        {
            //_ = TinyInsights.TrackErrorAsync(ex);
            //_ = TinyInsights.TrackPageViewAsync(source);
        }

        public async Task ResentItem()
        {
            try
            {
                #region old code
                //lstitem = await itemManage.GetItemFwaiting();
                //if (lstitem != null)
                //{
                //    foreach (var item in lstitem)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                //    }
                //}

                //lstitem1 = await itemManage.GetItemFwaiting1();
                //if (lstitem1 != null)
                //{
                //    foreach (var item in lstitem1)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                //    }
                //} 
                #endregion

                await Task.WhenAll(
                    ResendItemsFromList(await itemManage.GetItemFwaiting()),
                    ResendItemsFromList(await itemManage.GetItemFwaiting1())
                );
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentItem at Utils");
            }
        }

        private async Task ResendItemsFromList(List<Item> itemList)
        {
            try
            {
                if (itemList != null)
                {
                    foreach (var item in itemList)
                    {
                        await Task.Delay(500);
                        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResendItemsFromList at Utils");
            }
        }

        public async Task ResentCategory()
        {
            try
            {
                #region Old Code
                //lstcategory = await categoryManage.GetCategoryFwaiting();
                //if (lstcategory != null)
                //{
                //    foreach (var item in lstcategory)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendCatagory((int)item.MerchantID, (int)item.SysCategoryID);
                //    }
                //} 
                #endregion

                await Task.WhenAll(
                   ResendCategorysFromList(await categoryManage.GetCategoryFwaiting())
               );
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentCategory at Utils");
            }
        }

        private async Task ResendCategorysFromList(List<Category> itemList)
        {
            try
            {
                if (itemList != null)
                {
                    foreach (var item in itemList)
                    {
                        await Task.Delay(500);
                        JobQueue.Default.AddJobSendCatagory((int)item.MerchantID, (int)item.SysCategoryID);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResendCategorysFromList at Utils");
            }
        }

        public async Task ResentTran()
        {
            try
            {
                #region old code
                //lsttrans = await transManage.GetTranFwaiting();
                //if (lsttrans != null)
                //{
                //    foreach (var item in lsttrans)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendTrans((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString());
                //    }
                //} 
                #endregion

                await Task.WhenAll(
                    ResentTransFromList(await transManage.GetTranFwaiting())
                );
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentTran at Utils");
            }
        }

        private async Task ResentTransFromList(List<ORM.MerchantDB.Tran> transList)
        {
            try
            {
                if (transList != null)
                {
                    foreach (var item in transList)
                    {
                        await Task.Delay(500);
                        JobQueue.Default.AddJobSendTrans((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentTransFromList at Utils");
            }
        }

        public async Task ResentCustomer()
        {
            try
            {
                #region old Code
                //lstcustomer = await CustomerManage.GetCustomerFwaiting();
                //if (lstcustomer != null)
                //{
                //    foreach (var item in lstcustomer)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendCustomer((int)item.MerchantID, (int)item.SysCustomerID);
                //    }
                //} 
                #endregion

                await Task.WhenAll(
                   ResentCustomersFromList(await CustomerManage.GetCustomerFwaiting())
               );
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentCustomer at Utils");
            }
        }

        private async Task ResentCustomersFromList(List<Customer> transList)
        {
            try
            {
                if (transList != null)
                {
                    foreach (var item in transList)
                    {
                        await Task.Delay(500);
                        JobQueue.Default.AddJobSendCustomer((int)item.MerchantID, (int)item.SysCustomerID);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentTransFromList at Utils");
            }
        }

        public async Task ResentNoteCategory()
        {
            try
            {
                #region Old Code
                //lstnotecategory = await NoteCategoryManage.GetNoteCategoryFwaiting();
                //if (lstnotecategory != null)
                //{
                //    foreach (var item in lstnotecategory)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendNoteCatagory((int)item.MerchantID, (int)item.SysNoteCategoryID);
                //    }
                //} 
                #endregion

                await Task.WhenAll(
                   ResentNoteCategorysFromList(await NoteCategoryManage.GetNoteCategoryFwaiting())
               );
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentNoteCategory at Utils");
            }
        }

        private async Task ResentNoteCategorysFromList(List<NoteCategory> transList)
        {
            try
            {
                if (transList != null)
                {
                    foreach (var item in transList)
                    {
                        await Task.Delay(500);
                        JobQueue.Default.AddJobSendNoteCatagory((int)item.MerchantID, (int)item.SysNoteCategoryID);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentTransFromList at Utils");
            }
        }

        public async Task ResentNote()
        {
            try
            {
                #region Old Code
                //lstnote = await NoteManage.GetNoteFwaiting();
                //if (lstnote != null)
                //{
                //    foreach (var item in lstnote)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendNote((int)item.MerchantID, (int)item.SysNoteID);
                //    }
                //}

                #endregion
                await Task.WhenAll(
                   ResentNoteFromList(await NoteManage.GetNoteFwaiting())
               );
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentNote at Utils");
            }
        }

        private async Task ResentNoteFromList(List<Note> notesList)
        {
            try
            {
                if (notesList != null)
                {
                    foreach (var item in notesList)
                    {
                        await Task.Delay(500);
                        JobQueue.Default.AddJobSendNote((int)item.MerchantID, (int)item.SysNoteID);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentNoteFromList at Utils");
            }
        }

        public async Task ResentPrintCounter()
        {
            try
            {
                #region old Code
                //lsttransPrinCounter = await transManage.GetTranFwaitingAndPrintCounter();
                //if (lsttransPrinCounter != null)
                //{
                //    foreach (var tran in lsttransPrinCounter)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        //ส่งค่าจาก offline -> online
                //        UtilsAll.PostPrintCounter(DataCashingAll.SysBranchId, tran.TranNo, UtilsAll.ChangeDateTimeUS(tran.TranDate), (int)tran.PrintCounterLocal, tran);
                //    }
                //} 
                #endregion

                await Task.WhenAll(
                  ResentPrincountersFromList(await transManage.GetTranFwaitingAndPrintCounter())
              );
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentPrintCounter at Utils");
            }
        }

        private async Task ResentPrincountersFromList(List<ORM.MerchantDB.Tran> transList)
        {
            try
            {
                if (transList != null)
                {
                    foreach (var tran in transList)
                    {
                        await Task.Delay(500);
                        UtilsAll.PostPrintCounter(DataCashingAll.SysBranchId, tran.TranNo, UtilsAll.ChangeDateTimeUS(tran.TranDate), (int)tran.PrintCounterLocal, tran);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "ResentPrincountersFromList at Utils");
            }
        }

        public static async Task ResentTranFwaitingOnetwo()
        {
            try
            {
                TransManage transManage = new TransManage();
                #region old code
                //List<Tran> lsttrans = await transManage.GetTranFwaitingOneTwo();
                //if (lsttrans != null)
                //{
                //    foreach (var item in lsttrans)
                //    {
                //        System.Threading.Thread.Sleep(500);
                //        JobQueue.Default.AddJobSendTrans((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString());
                //    }
                //} 
                #endregion

                await Task.WhenAll(
                   ResentTranFwaitingOnetwo(await transManage.GetTranFwaitingOneTwo())
               );
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ResentTranFwaitingOnetwo at Utils");
                Android.Util.Log.Debug("error", "ResentTranFwaitingOnetwo " + ex.Message);
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private async static Task ResentTranFwaitingOnetwo(List<ORM.MerchantDB.Tran> transList)
        {
            try
            {
                if (transList != null)
                {
                    foreach (var item in transList)
                    {
                        await Task.Delay(500);
                        JobQueue.Default.AddJobSendTrans((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                HandlestaticException(ex, "ResentTranFwaitingOnetwo at Utils");
            }
        }

        //--------------------------------------

        //ThumbnailLocalImage
        public async static Task<bool> CreateThumbnailLocalImage(int merchantID)
        {
            try
            {
                string image_PATH = "";
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    image_PATH = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath + "/" + merchantID + "/picture/" + "Thumbnail/";

                    if (!Directory.Exists(image_PATH))
                    {
                        System.IO.Directory.CreateDirectory(image_PATH);
                    }
                    DataCashingAll.PathThumnailFolderImage = image_PATH;
                }
                else
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    image_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DataDirectory.Path).ToString()
                    + "/" + merchantID + "/picture/" + "Thumbnail/";
#pragma warning restore CS0618 // Type or member is obsolete
                    if (!Directory.Exists(image_PATH))
                    {
                        System.IO.Directory.CreateDirectory(image_PATH);
                    }
                    DataCashingAll.PathThumnailFolderImage = image_PATH;
                }
                Preferences.Set("PathThumnailFolderImage", DataCashingAll.PathThumnailFolderImage);
                return true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CreateThumbnailLocalImage");
                Log.Error("connecterror", "CreateThumbnailLocalImage : " + ex.Message);
                return false;
            }
        }

        //Picture        
        public async static Task<bool> CreateLocalImage(int merchantID)
        {
            try
            {
                string image_PATH = "";
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    image_PATH = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath + "/" + merchantID + "/picture/";

                    if (!Directory.Exists(image_PATH))
                    {
                        System.IO.Directory.CreateDirectory(image_PATH);
                    }
                    //else
                    //{
                    //    Directory.Delete(image_PATH, true);
                    //    System.IO.Directory.CreateDirectory(image_PATH);
                    //}
                    DataCashingAll.PathFolderImage = image_PATH;
                }
                else
                {
                    image_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DataDirectory.Path).ToString()
                    + "/" + merchantID + "/picture/";
                    if (!Directory.Exists(image_PATH))
                    {
                        System.IO.Directory.CreateDirectory(image_PATH);
                    }
                    //else
                    //{
                    //    Directory.Delete(image_PATH, true);
                    //    System.IO.Directory.CreateDirectory(image_PATH);
                    //}
                    DataCashingAll.PathFolderImage = image_PATH;
                }

                DataCashingAll.PathFolderImage = image_PATH;

                Preferences.Set("PathFolderImage", DataCashingAll.PathFolderImage);
                return true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CreateLocalImage");
                Log.Error("connecterror", "CreateLocalImage : " + ex.Message);
                return false;
            }
        }

        //DownloadBill
        public async static Task<bool> CreateDownloadBill(int merchantID)
        {
            try
            {
                string image_PATH = "";
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    image_PATH = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath + "/" + merchantID + "/downloadbill/";
                    if (!Directory.Exists(image_PATH))
                    {
                        System.IO.Directory.CreateDirectory(image_PATH);
                    }
                    //else
                    //{
                    //    Directory.Delete(image_PATH, true);
                    //    System.IO.Directory.CreateDirectory(image_PATH);
                    //}
                    DataCashingAll.PathImageBill = image_PATH;
                }
                else
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    image_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DataDirectory.Path).ToString()
                    + "/" + merchantID + "/downloadbill/";
#pragma warning restore CS0618 // Type or member is obsolete
                    if (!Directory.Exists(image_PATH))
                    {
                        System.IO.Directory.CreateDirectory(image_PATH);
                    }
                    //else
                    //{
                    //    Directory.Delete(image_PATH, true);
                    //    System.IO.Directory.CreateDirectory(image_PATH);
                    //}
                    DataCashingAll.PathImageBill = image_PATH;
                }
                Preferences.Set("PathImageBill", DataCashingAll.PathImageBill);
                return true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CreateLocalImage");
                Log.Error("connecterror", "CreateDownloadBill : " + ex.Message);
                return false;
            }
        }

        // เรียกใช้ CreateThumbnailLocalImage เพื่อเพิ่มรูปไว้ใน Folder

        public static async Task<bool> InsertImageToThumbnail(string PathImage, Android.Graphics.Bitmap bitmap, string Page)
        {
            try
            {
                string pathFolder = DataCashingAll.PathThumnailFolderImage;
                string dirPath = Path.Combine(pathFolder, PathImage);

                var exists = Directory.Exists(dirPath);
                if (!exists)
                {
                    if (System.IO.File.Exists(dirPath))
                    {
                        System.IO.File.Delete(dirPath);
                    }
                }

                // Resize bitmap based on page type
                int targetWidth = (Page == "item") ? 224 : 200;
                int targetHeight = (Page == "item") ? 168 : 200;
                bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, targetWidth, targetHeight, true);

                // Compress and save bitmap to file
                using (var os = new System.IO.FileStream(dirPath, System.IO.FileMode.Create))
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 70, os);
                }
                bitmap.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("connecterror", "InsertImageToThumbnail : " + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertImageToThumbnail");
                return false;
            }
        }

        public static async Task<bool> InsertImageToPicture(string PathImage, Android.Graphics.Bitmap bitmap)
        {
            try
            {
                string pathFolder = DataCashingAll.PathFolderImage;
                string dirPath = Path.Combine(pathFolder, PathImage);

                var exists = Directory.Exists(dirPath);
                if (!exists)
                {
                    if (System.IO.File.Exists(dirPath))
                    {
                        System.IO.File.Delete(dirPath);
                    }
                }

                using (var os = new System.IO.FileStream(dirPath, System.IO.FileMode.Create))
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, os);
                }
                bitmap.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("connecterror", "InsertImageToPicture :" + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertImageToPicture");
                return false;
            }
        }

        public static Task<byte[]> streamImage(Android.Graphics.Bitmap bitmap)
        {
            byte[] imageByteArray = null;
            try
            {
                //new bitmap
                if (bitmap == null)
                {
                    bitmap = Android.Graphics.Bitmap.CreateBitmap(600, 600, Android.Graphics.Bitmap.Config.Argb8888);
                }

                // เอา Bitmap มาเก็บเป็น BytArray เพื่อเตรียมส่งให้ Server
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, memoryStream);
                    imageByteArray = memoryStream.ToArray();
                }
                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("streamImage at Utils");
                DataCashingAll.imageByteArray = null;
                Log.Error("connecterror", "streamImage :" + ex.Message);
            }

            return Task.FromResult(imageByteArray);
        }

        public async static Task<byte[]> streamImageOffine(string PicturePath)
        {
            try
            {
                // เอา Picturepath มาเก็บเป็น BytArray เพื่อเตรียมส่งให้ Server
                Java.IO.File imgFile = new Java.IO.File(PicturePath);

                if (imgFile.Exists())
                {
                    Android.Graphics.Bitmap myBitmap = Android.Graphics.BitmapFactory.DecodeFile(imgFile.AbsolutePath);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        myBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, memoryStream);
                        return  memoryStream.ToArray();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("streamImageOffine at Utils");
                Log.Error("connecterror", "streamImageOffine :" + ex.Message);
                return null;
            }
        }

        //Insert Picture
        public static async Task InsertLocalPictureCustomer(Gabana.ORM.MerchantDB.Customer LocalCustomer)
        {
            try
            {
                if (!string.IsNullOrEmpty(LocalCustomer.PicturePath))
                {
                    CustomerManage customerManage = new CustomerManage();
                    System.Uri uri = new System.Uri(LocalCustomer.PicturePath);
                    System.Net.WebClient webClient = new System.Net.WebClient()
                    {
                        Encoding = Encoding.UTF8,
                    };

                    byte[] imageBytes = await webClient.DownloadDataTaskAsync(uri.AbsoluteUri);
                    if (imageBytes == null)
                    {
                        return;
                    }
                    Android.Graphics.Bitmap bitmap = null;
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        string path = Guid.NewGuid().ToString("N") + ".png";

                        string pathFolder = DataCashingAll.PathFolderImage;
                        string pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                        string pathPicture = LocalCustomer.PictureLocalPath;
                        string pathThumnailPicture = LocalCustomer.ThumbnailLocalPath;

                        //ลบรูป PictureLocalPath ออก
                        if (!string.IsNullOrEmpty(pathPicture))
                        {
                            string PictureName = Utils.SplitPath(pathPicture);
                            var dirPath = pathFolder + PictureName;
                            if (System.IO.File.Exists(dirPath))
                            {
                                System.IO.File.Delete(dirPath);
                                LocalCustomer.PictureLocalPath = null;
                            }
                        }

                        //ลบรูป thumnailLocal ออก
                        string ThumnailPictureName = Utils.SplitPath(pathThumnailPicture);
                        var dirThumnailPath = pathThumnailFolder + ThumnailPictureName;
                        if (System.IO.File.Exists(dirThumnailPath))
                        {
                            System.IO.File.Delete(dirThumnailPath);
                        }

                        //บันทึกรูปไป thumnailLocal ใหม่
                        var result2 = await Utils.InsertImageToThumbnail(path, bitmap, "customer");
                        if (result2)
                        {
                            LocalCustomer.ThumbnailLocalPath = pathThumnailFolder + path;
                        }

                        var resultupdate = await customerManage.UpdateCustomer(LocalCustomer);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertLocalPictureCustomer");
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task<bool> InsertLocalPictureMerchant(Gabana.ORM.MerchantDB.Merchant LocalMerchant)
        {
            try
            {
                //insert to LogoLocalPath
                if (!string.IsNullOrEmpty(LocalMerchant.LogoPath))
                {
                    MerchantManage merchantManage = new MerchantManage();
                    var webClient = new System.Net.WebClient();
                    System.Uri uri = new System.Uri(LocalMerchant.LogoPath);
                    byte[] imageBytes = await webClient.DownloadDataTaskAsync(uri.AbsoluteUri);
                    if (imageBytes == null)
                    {
                        return false;
                    }
                    Android.Graphics.Bitmap bitmap = null;
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        string path = Guid.NewGuid().ToString("N") + ".png";

                        var result = await Utils.InsertImageToPicture(path, bitmap);
                        if (result)
                        {
                            string pathFolder = DataCashingAll.PathFolderImage;
                            LocalMerchant.LogoLocalPath = pathFolder + path;
                        }
                        var resultupdate = await merchantManage.UpdateMerchant(LocalMerchant);
                        DataCashingAll.MerchantLocal = LocalMerchant;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertLocalPictureMerchant");
                return false;
            }
        }

        public static async Task InsertLocalPictureMyQRCode(Gabana.ORM.MerchantDB.MyQrCode LocalMyQrCode)
        {
            try
            {
                //insert to LogoLocalPath
                if (!string.IsNullOrEmpty(LocalMyQrCode.PicturePath))
                {
                    MyQrCodeManage myQrCodeManage = new MyQrCodeManage();
                    var webClient = new System.Net.WebClient();
                    System.Uri uri = new System.Uri(LocalMyQrCode.PicturePath);
                    byte[] imageBytes = await webClient.DownloadDataTaskAsync(uri.AbsoluteUri);
                    if (imageBytes == null)
                    {
                        return;
                    }
                    Android.Graphics.Bitmap bitmap = null;
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        //string path = SplitCloundPath(LocalMyQrCode.PicturePath);
                        string path = Guid.NewGuid().ToString("N") + ".png";

                        var result = await Utils.InsertImageToPicture(path, bitmap);
                        if (result)
                        {
                            string pathFolder = DataCashingAll.PathFolderImage;
                            LocalMyQrCode.PictureLocalPath = pathFolder + path;
                        }
                        var resultupdate = await myQrCodeManage.UpdateMyQrCode(LocalMyQrCode);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertLocalPictureMyQRCode");
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task<bool> InsertLocalPictureItem(Gabana.ORM.MerchantDB.Item LocalItem)
        {
            try
            {
                //insert to LogoLocalPath   
                if (!string.IsNullOrEmpty(LocalItem.PicturePath))
                {
                    ItemManage itemManage = new ItemManage();
                    Android.Graphics.Bitmap bitmap = null;
                    string path;
                    var webClient = new System.Net.WebClient();
                    System.Uri uri = new System.Uri(LocalItem.PicturePath);
                    byte[] imageBytes = await webClient.DownloadDataTaskAsync(uri.AbsoluteUri);

                    if (imageBytes == null)
                    {
                        return false;
                    }
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        //path = SplitCloundPath(LocalItem.PicturePath);
                        path = Guid.NewGuid().ToString("N") + ".png";

                        string pathFolder = DataCashingAll.PathFolderImage;
                        string pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                        string pathPicture = LocalItem.PictureLocalPath;
                        string pathThumnailPicture = LocalItem.ThumbnailLocalPath;

                        //ลบรูป PictureLocalPath ออก
                        if (!string.IsNullOrEmpty(pathPicture))
                        {
                            string PictureName = Utils.SplitPath(pathPicture);
                            var dirPath = pathFolder + PictureName;
                            if (System.IO.File.Exists(dirPath))
                            {
                                System.IO.File.Delete(dirPath);
                                LocalItem.PictureLocalPath = null;
                            }
                        }

                        //ลบรูป thumnailLocal ออก
                        string ThumnailPictureName = Utils.SplitPath(pathThumnailPicture);
                        var dirThumnailPath = pathThumnailFolder + ThumnailPictureName;
                        if (System.IO.File.Exists(dirThumnailPath))
                        {
                            System.IO.File.Delete(dirThumnailPath);
                        }

                        //บันทึกรูปไป thumnailLocal ใหม่
                        var result2 = await Utils.InsertImageToThumbnail(path, bitmap, "item");
                        if (result2)
                        {
                            LocalItem.ThumbnailLocalPath = pathThumnailFolder + path;
                        }

                        var resultupdate = await itemManage.UpdateItem(LocalItem);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertLocalPictureItem at Utils");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static async Task<string> InsertLocalPictureMerchantMaster(string LogoPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(LogoPath))
                {
                    byte[] imageBytes = null;
                    MerchantManage merchantManage = new MerchantManage();
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(1);
                        using (HttpResponseMessage response = await client.GetAsync(LogoPath))
                        {
                            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                imageBytes = await response.Content.ReadAsByteArrayAsync();
                            }
                        }

                        if (imageBytes == null)
                        {
                            return string.Empty;
                        }
                        Android.Graphics.Bitmap bitmap = null;
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                            string path = Guid.NewGuid().ToString("N") + ".png";

                            string pathFolder = DataCashingAll.PathFolderImage;
                            string NewPath = string.Empty;
                            //บันทึกรูปไป thumnailLocal ใหม่
                            var result2 = await Utils.InsertImageToPicture(path, bitmap);
                            if (result2)
                            {
                                NewPath = pathFolder + path;
                                return NewPath;
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertLocalPictureMerchant");
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public static async Task<string> InsertLocalPictureItemMaster(string pathMaster)
        {
            try
            {
                if (!string.IsNullOrEmpty(pathMaster))
                {
                    byte[] imageBytes = null;
                    Android.Graphics.Bitmap bitmap = null;
                    string path;
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(1);
                        using (HttpResponseMessage response = await client.GetAsync(pathMaster))
                        {
                            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                imageBytes = await response.Content.ReadAsByteArrayAsync();
                            }
                        }

                        if (imageBytes == null)
                        {
                            return string.Empty;
                        }

                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                            path = Guid.NewGuid().ToString("N") + ".png";

                            string pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                            string NewPath = string.Empty;
                            //บันทึกรูปไป thumnailLocal ใหม่
                            var result2 = await Utils.InsertImageToThumbnail(path, bitmap, "item");
                            if (result2)
                            {
                                NewPath = pathThumnailFolder + path;
                                return NewPath;
                            }
                        }
                        return string.Empty;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertLocalPictureItemMaster at Utils");
                Console.WriteLine(ex.Message);
                return String.Empty;
            }
        }

        public static async Task<string> InsertLocalPictureCustomerMaster(string pathMaster)
        {
            try
            {
                //insert to PictureLocalPath,ThumbnailLocalPath
                if (!string.IsNullOrEmpty(pathMaster))
                {
                    byte[] imageBytes = null;
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(1);
                        using (HttpResponseMessage response = await client.GetAsync(pathMaster))
                        {
                            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                imageBytes = await response.Content.ReadAsByteArrayAsync();
                            }
                        }

                        if (imageBytes == null)
                        {
                            return string.Empty;
                        }

                        Android.Graphics.Bitmap bitmap = null;
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                            string path = path = Guid.NewGuid().ToString("N") + ".png";

                            string pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                            string NewPath = string.Empty;
                            //บันทึกรูปไป thumnailLocal ใหม่
                            var result2 = await Utils.InsertImageToThumbnail(path, bitmap, "customer");
                            if (result2)
                            {
                                NewPath = pathThumnailFolder + path;
                                return NewPath;
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertLocalPictureCustomerMaster");
                Console.WriteLine(ex.Message);
                return String.Empty;
            }
        }

        public static async Task<string> InsertLocalPictureMyQRMaster(string pathMaster)
        {
            try
            {
                //insert to PictureLocalPath
                if (!string.IsNullOrEmpty(pathMaster))
                {
                    byte[] imageBytes = null;
                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromMinutes(1);
                        using (HttpResponseMessage response = await client.GetAsync(pathMaster))
                        {
                            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                imageBytes = await response.Content.ReadAsByteArrayAsync();
                            }
                        }

                        if (imageBytes == null)
                        {
                            return string.Empty;
                        }

                        Android.Graphics.Bitmap bitmap = null;
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            bitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                            string path = Guid.NewGuid().ToString("N") + ".png";
                            string pathFolder = DataCashingAll.PathFolderImage;
                            string NewPath = string.Empty;
                            var result = await Utils.InsertImageToPicture(path, bitmap);
                            if (result)
                            {
                                NewPath = pathFolder + path;
                                return NewPath;
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertLocalPictureMyQRMaster");
                Console.WriteLine(ex.Message);
                return String.Empty;
            }
        }

        public static async Task DeletePictureItemMaster(List<string> PathMaster)
        {
            try
            {
                foreach (var item in PathMaster)
                {
                    if (System.IO.File.Exists(item))
                    {
                        System.IO.File.Delete(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //path Clound
        public static string SplitPath(string pathImage)
        {
            try
            {
                string path = string.Empty;
                if (string.IsNullOrEmpty(pathImage))
                {
                    return string.Empty;
                }

                string[] fullpart = pathImage.Split("/");
                path = fullpart[fullpart.Length - 1];
                return path;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SplitPath at Utils"); return pathImage;
            }
        }

        public static string SplitCloundPath(string pathImage)
        {
            try
            {
                string path;
                if (string.IsNullOrEmpty(pathImage))
                {
                    return string.Empty;
                }

                string[] fullpart = pathImage.Split("/");
                path = fullpart[fullpart.Length - 1];
                path = path.Remove(0, 9);
                return path;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string ShowDate(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            return datetime;
        }

        public static string ShowTime(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("HH:mm", new CultureInfo("en-US"));
            return datetime;
        }

        public static string PrintDateTime(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string time = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd/MM/yyyy HH:mm tt", new CultureInfo("en-US"));
            return time;
        }

        public static string ShowDateTime(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd/MM/yyyy HH:mm tt", new CultureInfo("en-US"));
            return datetime;
        }

        public static string ChangeDateTime(DateTime d)
        {
            try
            {
                var destinationTimeZone = TimeZoneInfo.Local;
                string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
                return datetime;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return d.ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
            }
        }

        public static string ChangeDateTimeTranOrder(DateTime d)
        {
            try
            {
                var destinationTimeZone = TimeZoneInfo.Local;
                string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("yyyyMMddhhmmssfff", new CultureInfo("en-US"));
                return datetime;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return d.ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
            }
        }

        public static string ChangeDateTimeReport(DateTime d)
        {
            try
            {
                //yyyyMMddHHmmss
                var destinationTimeZone = TimeZoneInfo.Local;
                string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("yyyyMMdd", new CultureInfo("en-US"));
                return datetime;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return d.ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
            }
        }

        public static DateTime BirthDayBE(DateTime? checkDate)
        {
            return checkDate ?? DateTime.MinValue;
        }

        public static DateTime ChangeDateTimeStringReport(string checkDate)
        {
            DateTime convertDate = DateTime.ParseExact(checkDate, "yyyyMMdd", new CultureInfo("en-US"));
            return convertDate;
        }

        public static string ShowDateReport(DateTime d)
        {
            try
            {
                var destinationTimeZone = TimeZoneInfo.Local;
                string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                return datetime;
            }
            catch (Exception)
            {
                //throw;
                var destinationTimeZone = TimeZoneInfo.Local;
                DateTime now = DateTime.SpecifyKind(d, DateTimeKind.Local);
                string datetime = TimeZoneInfo.ConvertTimeToUtc(now, destinationTimeZone).ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                return datetime;
            }
        }

        public static DateTime GetTranDate(DateTime checkDate)
        {
            try
            {
                return checkDate;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return checkDate;
            }
        }

        public static string SetBackground(int color)
        {
            string hexValue = color.ToString("X");
            string hexcolor;
            if (hexValue.Length == 4)
            {
                hexcolor = "#00" + hexValue;
            }
            else if (hexValue.Length == 5)
            {
                hexcolor = "#0" + hexValue;
            }
            else if (hexValue.Length == 6)
            {
                hexcolor = "#" + hexValue;
            }
            else
            {
                hexcolor = "#0095DA";
            }
            return hexcolor;
        }

        public static void SetEmployeeImage(ImageView imageView, string PositionName, bool status)
        {
            int path;
            if (status)
            {
                switch (PositionName)
                {
                    case "owner":
                        path = Resource.Mipmap.EmpOwnerB;
                        break;
                    case "admin":
                        path = Resource.Mipmap.EmpRoleAdminB;
                        break;
                    case "cashier":
                        path = Resource.Mipmap.EmpCashierB;
                        break;
                    case "editor":
                        path = Resource.Mipmap.EmpEditorB;
                        break;
                    case "invoice":
                        path = Resource.Mipmap.EmpInvoiceOfficerB;
                        break;
                    case "manager":
                        path = Resource.Mipmap.EmpManagerB;
                        break;
                    case "officer":
                        path = Resource.Mipmap.EmpOfficerB;
                        break;
                    default:
                        path = Resource.Mipmap.EmpOfficerB;
                        break;
                }
            }
            else
            {
                switch (PositionName)
                {
                    case "owner":
                        path = Resource.Mipmap.EmpOwnerG;
                        break;
                    case "admin":
                        path = Resource.Mipmap.EmpAdminG;
                        break;
                    case "cashier":
                        path = Resource.Mipmap.EmpCashierG;
                        break;
                    case "editor":
                        path = Resource.Mipmap.EmpEditorG;
                        break;
                    case "invoice":
                        path = Resource.Mipmap.EmpInvoiceOfficerG;
                        break;
                    case "manager":
                        path = Resource.Mipmap.EmpManagerG;
                        break;
                    case "officer":
                        path = Resource.Mipmap.EmpOfficerG;
                        break;
                    default:
                        path = Resource.Mipmap.EmpOfficerG;
                        break;
                }
            }

            //imageView.SetImageResource(path);
            imageView.SetBackgroundResource(path);
        }

        public static string DisplayDecimal(decimal price)
        {
            try
            {
                string textDecimal = "";
                var dec = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;
                if (dec != null)
                {
                    if (dec == "4")
                    {
                        textDecimal = price.ToString("#,##0.0000");
                    }
                    else if (dec == "2")
                    {
                        textDecimal = price.ToString("#,##0.00");
                    }
                    else
                    {
                        textDecimal = price.ToString("#,##0");
                    }
                }
                else
                {
                    textDecimal = price.ToString("#,##0");
                }

                return textDecimal;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DisplayDecimal");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return price.ToString();
            }
        }

        public static string DisplayComma(string price)
        {
            try
            {
                string textDecimal = "";
                if (price.ToString().Contains('.'))
                {
                    var split = price.ToString().Split('.');
                    textDecimal = Convert.ToDecimal(split[0]).ToString("#,##0") + "." + split[1];
                }
                else
                {
                    textDecimal = Convert.ToDecimal(price).ToString("#,##0.####");
                }
                return textDecimal;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DisplayDecimal");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return price.ToString();
            }
        }

        public static string DisplayDouble(decimal price)
        {
            try
            {
                string textDouble = price.ToString("#,###.####");
                var havepointer = textDouble.Contains('.');
                if (havepointer)
                {
                    var index = textDouble.IndexOf('.');
                    if (index == 0)
                    {
                        textDouble = "0" + textDouble;
                    }
                }
                return textDouble;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DisplayDecimal");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return price.ToString();
            }
        }

        public static void SetEmployeeActive(ImageView imageView, bool status)
        {
            int path;
            switch (status)
            {
                case true:
                    path = Resource.Mipmap.UserActive;
                    break;
                default:
                    path = Resource.Mipmap.UserInactive;
                    break;
            }
            //imageView.SetImageResource(path);
            imageView.SetBackgroundResource(path);
        }

        public static List<string> SplitAddress(string merchantAddress)
        {
            var rest = merchantAddress.Split(' ');
            int lengPage = 35;
            if (DataCashingAll.setting.TYPEPAGE.Substring(0, 2) == "80")
            {
                lengPage = 40;
            }
            if (DataCashingAll.setting.PRINTTYPE != "Image")
            {
                lengPage = 30;
                if (DataCashingAll.setting.TYPEPAGE.Substring(0, 2) == "80")
                {
                    lengPage = 35;
                }
            }
            List<string> address = new List<string>();
            string line1 = "", line2 = "", line3 = "";
            address.Add(line1);
            address.Add(line2);
            address.Add(line3);

            if (merchantAddress.Length <= lengPage)
            {
                address[0] = merchantAddress;
            }
            else
            {
                string text = "";
                int j = 0;
                for (int i = 0; i < rest.Count(); i++)
                {
                    text = address[j] + rest[i] + " ";
                    if (text.Length < lengPage)
                    {
                        address[j] = text;
                    }
                    else
                    {

                        j = j + 1;
                        if (j < address.Count)
                        {
                            address[j] = rest[i] + " ";
                        }
                    }
                    if (j >= address.Count)
                    {
                        break;
                    }
                }
            }
            return address;
        }

        public static List<string> SplitItemName(int length, string itemName)
        {
            List<string> names = new List<string>();
            string line1 = "", line2 = "", line3 = "";

            names.Add(line1);
            names.Add(line2);
            names.Add(line3);

            if (DataCashingAll.setting.PRINTTYPE != "Image")
            {
                length = 20;
                if (DataCashingAll.setting.TYPEPAGE.Substring(0, 2) == "80")
                {
                    length = 25;
                }
            }
            if (itemName.Length < length) { names[0] = itemName; }
            else
            {
                var rest = itemName.Split(' ');
                int j = 0;
                string text = "";
                for (int i = 0; i < rest.Count(); i++)
                {
                    text = names[j] + rest[i] + " ";
                    if (text.Length < length)
                    {
                        names[j] = text;
                    }
                    else
                    {
                        j = j + 1;
                        if (j < names.Count)
                        {
                            names[j] = rest[i] + " ";
                        }
                    }
                    if (j >= names.Count)
                    {
                        break;
                    }
                }

            }
            return names;
        }

        public static async Task<Model.TranWithDetailsLocal> initialData()
        {
            try
            {
                string usernamelogin = Preferences.Get("User", "");
                var VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
                var getVat = DataCashingAll.setmerchantConfig.TAXRATE;
                decimal vat = 0;
                if (string.IsNullOrEmpty(getVat))
                {
                    vat = 0;
                }
                else
                {
                    vat = Convert.ToDecimal(getVat);
                }

                var ServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                string FmlServiceCharge = "";
                if (string.IsNullOrEmpty(ServiceCharge))
                {
                    FmlServiceCharge = "0";
                }
                else
                {
                    FmlServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                }

                DeviceTranRunningNoManage DeviceTranRunningNo = new DeviceTranRunningNoManage();
                var maxtranno = await DeviceTranRunningNo.GetLastDeviceTranRunningNo(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, DataCashingAll.DeviceNo);
                maxtranno++;

                Model.TranWithDetailsLocal tranWithDetails = new Model.TranWithDetailsLocal();
                tranWithDetails.tran = new ORM.MerchantDB.Tran();
                tranWithDetails.tran.MerchantID = DataCashingAll.MerchantId;
                tranWithDetails.tran.SysBranchID = DataCashingAll.SysBranchId;
                tranWithDetails.tran.TranNo = "#" + DataCashingAll.DeviceNo.ToString() + "-" + maxtranno.ToString("D6");
                tranWithDetails.tran.TranDate = Utils.GetTranDate(DateTime.UtcNow);
                tranWithDetails.tran.Status = 10;
                tranWithDetails.tran.DeviceNo = DataCashingAll.DeviceNo;
                tranWithDetails.tran.SysCustomerID = 999;
                tranWithDetails.tran.CustomerName = "ลูกค้าทั่วไป";//
                tranWithDetails.tran.SellerName = usernamelogin;
                tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                tranWithDetails.tran.LastUserModified = usernamelogin;
                tranWithDetails.tran.FCancel = 0;
                tranWithDetails.tran.CountTradDisc = 0;
                tranWithDetails.tran.SubTotalNoneVat = 0;
                tranWithDetails.tran.TotalTradDiscNoneVat = 0;
                tranWithDetails.tran.TotalNoneVat = 0;
                tranWithDetails.tran.SubTotalHaveVat = 0;
                tranWithDetails.tran.TotalTradDiscHaveVat = 0;
                tranWithDetails.tran.TotalHaveVat = 0;
                tranWithDetails.tran.Total = 0;
                tranWithDetails.tran.ServiceCharge = 0;
                tranWithDetails.tran.FmlServiceCharge = FmlServiceCharge;
                tranWithDetails.tran.TotalVat = 0;
                tranWithDetails.tran.GrandTotal = 0;
                tranWithDetails.tran.PaymentFractional = 0;
                tranWithDetails.tran.GrandPayment = 0;
                tranWithDetails.tran.SummaryPayment = 0;
                tranWithDetails.tran.Change = 0;
                tranWithDetails.tran.Tips = 0;
                tranWithDetails.tran.TotalPointEarning = 0;
                tranWithDetails.tran.PrintCounter = 0;
                tranWithDetails.tran.TaxRate = vat;
                tranWithDetails.tran.TranTaxType = char.Parse(VatType);

                //merchantDB
                tranWithDetails.tran.FWaitSending = 2;
                tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                tranWithDetails.tran.Comments = null;
                tranWithDetails.tran.LocalDataStatus = 'I';

                //Order or Bill
                tranWithDetails.tran.TranType = 'B';
                tranWithDetails.tran.OrderName = null;
                tranWithDetails.tran.Status = 10;

                return tranWithDetails;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("initialData at Util");
                return new TranWithDetailsLocal();
            }
        }

        public static string CheckLenghtValue(string strValue)
        {
            try
            {
                string pattern = "[,.%]";
                string replacement = "";

                System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern);
                var check = System.Text.RegularExpressions.Regex.Replace(regEx.Replace(strValue, replacement), @"\s+", "");
                return check;
            }
            catch (Exception)
            {
                return strValue;
            }
        }

        public static async Task<string> SetTextAddressAsync(ORM.MerchantDB.Branch branch)
        {
            try
            {
                string addressTH = "";
                string addressEN = "";
                string address = "";
                string ZipCode = "";

                List<Gabana.ORM.PoolDB.Province> GetProvinces = new List<Gabana.ORM.PoolDB.Province>();
                List<Gabana.ORM.PoolDB.Amphure> GetAmphures = new List<Gabana.ORM.PoolDB.Amphure>();
                List<Gabana.ORM.PoolDB.District> GetDistricts = new List<Gabana.ORM.PoolDB.District>();
                PoolManage poolManage = new PoolManage();

                if (branch.ProvincesId != null)
                {
                    GetProvinces = await poolManage.GetProvinces();
                    GetAmphures = await poolManage.GetAmphures((int)branch.ProvincesId);
                    GetDistricts = await poolManage.GetDistricts((int)branch.AmphuresId);

                    if (branch.DistrictsId != null && branch.DistrictsId != 0)
                    {
                        addressTH += " " + GetDistricts.Where(x => x.AmphuresId == branch.DistrictsId).Select(x => x.DistrictsNameTH).FirstOrDefault();
                        addressEN += " " + GetDistricts.Where(x => x.AmphuresId == branch.DistrictsId).Select(x => x.DistrictsNameEN).FirstOrDefault();
                    }

                    if (branch.AmphuresId != null && branch.AmphuresId != 0)
                    {
                        addressEN += " " + GetAmphures.Where(x => x.AmphuresId == branch.AmphuresId).Select(x => x.AmphuresNameEN).FirstOrDefault();
                        addressTH += " " + GetAmphures.Where(x => x.AmphuresId == branch.AmphuresId).Select(x => x.AmphuresNameTH).FirstOrDefault();
                    }
                    if (branch.ProvincesId != null && branch.ProvincesId != 0)
                    {
                        addressEN += " " + GetProvinces.Where(x => x.ProvincesId == branch.ProvincesId).Select(x => x.ProvincesNameEN).FirstOrDefault();
                        addressTH += " " + GetProvinces.Where(x => x.ProvincesId == branch.ProvincesId).Select(x => x.ProvincesNameTH).FirstOrDefault();
                    }

                    ZipCode = GetDistricts.Where(x => x.DistrictsId == branch.DistrictsId).Select(x => x.ZipCode).FirstOrDefault();
                }

                if (DataCashing.Language == "th")
                {
                    address = addressTH;
                }
                else
                {
                    address = addressEN;
                }

                address = branch.Address + " " + address.Trim() + " " + ZipCode;

                return address;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //Cancel Order ไม่แก้ไข, ละทิ้ง
        public async static Task CancelTranOrder(Model.TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                TransManage transManage = new TransManage();

                //กด Back โดยไม่ได้แก้ไขหรือเปิดบิล จะทำการ Set  FWaitSending เป็น 1 หรือ 2 ขึ้นอยู่กับสถานะการเชื่อมต่อ internet
                if (await GabanaAPI.CheckNetWork())
                {
                    if (tranWithDetails.tran.LocalDataStatus == 'U')
                    {
                        tranWithDetails.tran.Status = 110;
                        tranWithDetails.tran.FWaitSending = 2;
                        tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                        tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                        tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                    }
                    else
                    {
                        //Insert order ใหม่ เนื่องจากเป็นออเดอร์ที่สร้างจากเครื่องเราแล้วมีการแก้ไขเลย ทำให้ไม่ทัน JobQueue
                        tranWithDetails.tran.Status = 100;
                        tranWithDetails.tran.FWaitSending = 2;
                        tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                        tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                        tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                    }

                    var updatetTran = await transManage.UpdateTran(tranWithDetails.tran);
                    JobQueue.Default.AddJobSendTrans((int)DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                }
                else
                {
                    tranWithDetails.tran.Status = 100;
                    tranWithDetails.tran.FWaitSending = 2;
                    tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                    tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                    tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);

                    var updatetTran = await transManage.UpdateTran(tranWithDetails.tran);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CancelTranOrder");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // MUST CONTAIN ONE AND ONLY ONE @
            var atCount = input.Count(c => c == '@');
            if (atCount != 1) return false;

            // MUST CONTAIN PERIOD
            if (!input.Contains(".")) return false;

            // @ MUST OCCUR BEFORE LAST PERIOD
            var indexOfAt = input.IndexOf("@", StringComparison.Ordinal);
            var lastIndexOfPeriod = input.LastIndexOf(".", StringComparison.Ordinal);
            var atBeforeLastPeriod = lastIndexOfPeriod > indexOfAt;
            if (!atBeforeLastPeriod) return false;

            //หลังจาก . ตัวสุดท้าย ห้ามว่าง
            var checklastIndexOfPeriod = input.LastIndexOf(".", StringComparison.Ordinal);
            if (input.Length == checklastIndexOfPeriod + 1) return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }

        public static void CopyDatabase()
        {
            try
            {
                var bytes = System.IO.File.ReadAllBytes(DataCashingAll.Pathdb);
                //String image_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();
                string _PATH = "";

                if (Build.VERSION.SdkInt > BuildVersionCodes.Q) //30,31,32,33
                {
                    _PATH = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                             Android.OS.Environment.DirectoryDownloads);
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.N) //24-29
                {
                    _PATH = Android.App.Application.Context.GetExternalFilesDir("DirectoryDownloads").AbsolutePath ;
                }
                else
                {
                    _PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();
                }

                Java.IO.File file = new Java.IO.File(_PATH + "/gabana.db");
                System.IO.File.WriteAllBytes(file.AbsolutePath, bytes);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static void RestartApplication(Android.Content.Context context, string ActivityName, int reconnect)
        {
            Intent serviceStart;

            if (reconnect == 1)
            {
                if (ActivityName == "main")
                {
                    serviceStart = new Intent(context, typeof(MainActivity));
                }
                else
                {
                    serviceStart = new Intent(context, typeof(SplashActivity));
                }
            }
            else
            {
                serviceStart = new Intent(Application.Context, typeof(LoginActivity));
            }

            serviceStart.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(serviceStart);
        }

        public static async Task DeleteItem(Item item, List<ORM.Master.ItemOnBranch> itemOnBranch)
        {
            ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
            ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
            ItemManage itemManage = new ItemManage();

            try
            {
                //delete รูป
                if (!string.IsNullOrEmpty(item?.ThumbnailLocalPath))
                {
                    Java.IO.File imgTempFile = new Java.IO.File(item?.ThumbnailLocalPath);

                    if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                    {
                        System.IO.File.Delete(imgTempFile.AbsolutePath);
                    }
                }

                //delete itemOnBranchs
                if (itemOnBranch != null)
                {
                    foreach (var onBranch in itemOnBranch)
                    {
                        var deleteItemonBranch = await onBranchManage.DeleteItemOnBranch(onBranch.MerchantID, onBranch.SysBranchID, onBranch.SysItemID);
                    }
                }
                var deleteItemSize = await itemExSizeManage.DeleteItemsize((int)item.MerchantID, (int)item.SysItemID);
                var delete = await itemManage.DeleteItem((int)item.MerchantID, (int)item.SysItemID);
                if (!delete)
                {
                    item.FWaitSending = 0;
                    item.DataStatus = 'D';
                    await itemManage.UpdateItem(item);
                }
            }
            catch (Exception)
            {
                item.FWaitSending = 0;
                item.DataStatus = 'D';
                await itemManage.UpdateItem(item);
            }
        }

        public static void AddNullValue()
        {
            try
            {
                #region OldCode

                if (string.IsNullOrEmpty(GabanaAPI.gbnJWT))
                {
                    GabanaAPI.gbnJWT = Preferences.Get("gbnJWT", "");
                }

                if (DataCashingAll.MerchantId == 0)
                {
                    var merchantid = Preferences.Get("MerchantID", 0);
                    DataCashingAll.MerchantId = merchantid;
                }

                if (string.IsNullOrEmpty(DataCashingAll.Pathdb))
                {
                    DataCashingAll.Pathdb = Preferences.Get("PathMerchantDB", "");
                }

                if (string.IsNullOrEmpty(DataCashingAll.Pathdbpool))
                {
                    DataCashingAll.Pathdbpool = Preferences.Get("PathPoolDB", "");
                }

                if (string.IsNullOrEmpty(DataCashingAll.PathFolderImage))
                {
                    DataCashingAll.PathFolderImage = Preferences.Get("PathFolderImage", "");
                }

                if (string.IsNullOrEmpty(DataCashingAll.PathThumnailFolderImage))
                {
                    DataCashingAll.PathThumnailFolderImage = Preferences.Get("PathThumnailFolderImage", "");
                }

                if (string.IsNullOrEmpty(DataCashingAll.PathImageBill))
                {
                    DataCashingAll.PathImageBill = Preferences.Get("PathImageBill", "");
                }

                if (DataCashingAll.setmerchantConfig == null)
                {
                    var setmerchantConfig = Preferences.Get("SetmerchantConfig", "");
                    var Config = JsonConvert.DeserializeObject<SetMerchantConfig>(setmerchantConfig);
                    DataCashingAll.setmerchantConfig = Config;
                }

                //Useraccount                
                if (DataCashingAll.UserAccountInfo == null)
                {
                    var Employee = Preferences.Get("Employeeoffline", "");
                    var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                    DataCashingAll.UserAccountInfo = lstEmployee;
                }

                if (DataCashingAll.Device == null)
                {
                    var setDevice = Preferences.Get("Device", "");
                    var Config = JsonConvert.DeserializeObject<Device>(setDevice);
                    DataCashingAll.Device = Config;
                    DataCashingAll.DeviceNo = Config.DeviceNo;
                }

                if (string.IsNullOrEmpty(DataCashingAll.setting?.TYPE))
                {
                    var settingPrinter = Preferences.Get("Setting", "");
                    var Config = JsonConvert.DeserializeObject<SettingPrinter>(settingPrinter);
                    if (Config != null)
                    {
                        DataCashingAll.setting = Config;
                    }
                    else
                    {
                        DataCashingAll.setting = new SettingPrinter()
                        {
                            TYPE = "Wifi",
                            PORTNUMBER = "9100",
                            IPADDRESS = "192.168.200.182",
                            USE = "Wifi",
                            TYPEPAGE = "58mm",
                            PRINTTYPE = "Image",
                            COMMAND = "Epson Command",
                        };
                    }
                }

                if (DataCashingAll.SysBranchId == 0)
                {
                    string branchID = Preferences.Get("Branch", "");
                    int.TryParse(branchID, out int result);
                    if (result == 0)
                    {
                        DataCashingAll.SysBranchId = 1;
                    }
                    else
                    {
                        DataCashingAll.SysBranchId = result;
                    }
                }

                if (string.IsNullOrEmpty(DataCashingAll.DevicePlatform))
                {
                    DataCashingAll.DevicePlatform = "FCM";
                }

                if (string.IsNullOrEmpty(DataCashingAll.DeviceUDID))
                {
                    DataCashingAll.DeviceUDID = Preferences.Get("DeviceUDID", "");
                }

                if (string.IsNullOrEmpty(DataCashingAll.intUsePinCode)) //1 = use , 0 =not use
                {
                    var use = Preferences.Get("UsePincode", 1);
                    if (use == 1)
                    {
                        DataCashingAll.UsePinCode = true;
                        DataCashingAll.intUsePinCode = "1";
                    }
                    else
                    {
                        DataCashingAll.UsePinCode = false;
                        DataCashingAll.intUsePinCode = string.Empty;
                    }
                }

                if (string.IsNullOrEmpty(DataCashingAll.GetGabanaInfo?.MerchantID.ToString()))
                {
                    string gabanaInfo = Preferences.Get("GabanaInfo", "");
                    GabanaInfo GabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(gabanaInfo);
                    DataCashingAll.GetGabanaInfo = GabanaInfo;
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckNullValue");
            }
        }

        public static List<string> CheckStatusIteminCart(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                //ItemManage itemManage = new ItemManage();
                //TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                //List<Item> AllItem = new List<Item>();
                //List<Item> AllItemStatusD = new List<Item>();
                //List<string> lstSysItemId = new List<string>(); //tranDetailItem
                //List<string> lstSysItemIdTopping = new List<string>(); //tranDetailToppings
                //List<string> lstRemoveDummy = new List<string>();

                //tranWithDetailsLocal = tranWithDetails;
                //AllItem = PosActivity.AllItem;
                //AllItemStatusD = PosActivity.AllItemStatusD;

                ////เคสหาสินค้าที่ไม่มีที่เครื่อง

                //lstRemoveDummy = tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => !AllItem.Select(y => y.SysItemID).ToList().Contains(x.tranDetailItem?.SysItemID == null ? 0 : (long)x.tranDetailItem?.SysItemID) && x.tranDetailItem?.SysItemID != 0 && !string.IsNullOrEmpty(x.tranDetailItem?.SysItemID?.ToString())).Select(m => m.tranDetailItem?.SysItemID.ToString()).ToList();

                //lstSysItemId.AddRange(lstRemoveDummy);
                //foreach (var item in tranWithDetailsLocal.tranDetailItemWithToppings)
                //{
                //    lstSysItemIdTopping.AddRange(item.tranDetailItemToppings.Where(x => !AllItem.Select(y => y.SysItemID).ToList().Contains(x.SysItemID == null ? 0 : (long)x.SysItemID)).Select(m => m.SysItemID.ToString()).ToList());
                //}
                //lstSysItemId.AddRange(lstSysItemIdTopping);

                ////เคสหาสินค้าที่มีสถานะเป็น 'D' และ Tran.Payment == 0
                //if (tranWithDetailsLocal.tranPayments.Count == 0)
                //{
                //    lstSysItemId.AddRange(tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => AllItemStatusD.Select(y => y.SysItemID).ToList().Contains(x.tranDetailItem?.SysItemID == null ? 0 : (long)x.tranDetailItem?.SysItemID) && x.tranDetailItem?.SysItemID != 0).Select(m => m.tranDetailItem?.SysItemID.ToString()).ToList());
                //    foreach (var item in tranWithDetailsLocal.tranDetailItemWithToppings)
                //    {
                //        lstSysItemIdTopping.AddRange(item.tranDetailItemToppings.Where(x => AllItemStatusD.Select(y => y.SysItemID).ToList().Contains(x.SysItemID == null ? 0 : (long)x.SysItemID)).Select(m => m.SysItemID.ToString()).ToList());
                //    }
                //    lstSysItemId.AddRange(lstSysItemIdTopping);
                //}
                //return lstSysItemId;

                var lstSysItemId = new List<string>();
                var lstSysItemIdTopping = new List<string>();

                // Extract tranDetailItemWithToppings for easier access
                var tranDetails = tranWithDetails.tranDetailItemWithToppings;

                // Get all items and items with status D
                var allItems = PosActivity.AllItem ?? new List<Item>();
                var statusDItems = PosActivity.AllItemStatusD ?? new List<Item>();

                // Check for items not in the system
                var itemsNotInSystem = tranDetails
                    .Where(x => x.tranDetailItem != null &&
                           !allItems.Any(y => y.SysItemID == (x.tranDetailItem.SysItemID ?? 0)) &&
                           x.tranDetailItem.SysItemID != 0)
                    .Select(m => m.tranDetailItem.SysItemID.ToString())
                    .ToList();

                lstSysItemId.AddRange(itemsNotInSystem);

                // Check for items with status D if there are no payments
                if (tranWithDetails.tranPayments.Count == 0)
                {
                    var statusDItemsIds = tranDetails
                        .Where(x => x.tranDetailItem != null &&
                               statusDItems.Any(y => y.SysItemID == (x.tranDetailItem.SysItemID ?? 0)) &&
                               x.tranDetailItem.SysItemID != 0)
                        .Select(m => m.tranDetailItem.SysItemID.ToString())
                        .ToList();

                    lstSysItemId.AddRange(statusDItemsIds);
                }

                // Get item toppings not in the system
                foreach (var item in tranDetails)
                {
                    var toppingsNotInSystem = item.tranDetailItemToppings
                        .Where(x => x.SysItemID != null &&
                               !allItems.Any(y => y.SysItemID == (x.SysItemID ?? 0)))
                        .Select(m => m.SysItemID.ToString())
                        .ToList();

                    lstSysItemIdTopping.AddRange(toppingsNotInSystem);
                }

                lstSysItemId.AddRange(lstSysItemIdTopping);
                return lstSysItemId;

            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckStatusIteminCart : " + ex.Message);
                return new List<string>();
            }
        }

        public static TranWithDetailsLocal RemoveLstItem(TranWithDetailsLocal tranWithDetails, List<long?> lstRemoveItem)
        {
            try
            {
                //if (lstRemoveItem.Count > 0)
                //{
                //    TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                //    tranWithDetailsLocal = tranWithDetails;
                //    List<Model.TranDetailItemWithTopping> TranDetailItem = new List<Model.TranDetailItemWithTopping>();

                //    foreach (var item in lstRemoveItem)
                //    {
                //        TranDetailItem = tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == item).ToList();
                //        foreach (var Item in TranDetailItem)
                //        {
                //            //ลบออกจาก item
                //            //tranWithDetailsLocal.tranDetailItemWithToppings.Remove(Item);
                //            tranWithDetails = BLTrans.RemoveDetailItem(tranWithDetails, Item);
                //            tranWithDetails = BLTrans.Caltran(tranWithDetailsLocal);
                //        }

                //        foreach (var ItemTopping in tranWithDetailsLocal.tranDetailItemWithToppings)
                //        {
                //            //ลบออกจาก topping
                //            var topping = ItemTopping.tranDetailItemToppings.Where(x => x.SysItemID == item).ToList();
                //            foreach (var RemoveTopping in topping)
                //            {
                //                ItemTopping.tranDetailItemToppings.Remove(RemoveTopping);
                //                tranWithDetails = BLTrans.EditToppingRow(tranWithDetails, ItemTopping);
                //                tranWithDetails = BLTrans.Caltran(tranWithDetailsLocal);
                //            }

                //            //tranWithDetails = BLTrans.EditToppingRow(tranWithDetails, ItemTopping);
                //            //tranWithDetails = BLTrans.Caltran(tranWithDetailsLocal);
                //        }
                //    }
                //}
                //return tranWithDetails; //tranWithDetails = tranWithDetailsLocal;       

                if (lstRemoveItem.Count > 0)
                {
                    foreach (var item in lstRemoveItem)
                    {
                        // Remove items
                        RemoveItems(tranWithDetails, item);
                    }
                }
                return tranWithDetails;

            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckStatusIteminCart : " + ex.Message);
                TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                return tranWithDetailsLocal;
            }
        }

        private static void RemoveItems(TranWithDetailsLocal tranWithDetails, long? item)
        {
            var itemsToRemove = tranWithDetails.tranDetailItemWithToppings
                .Where(x => x.tranDetailItem != null && x.tranDetailItem.SysItemID == item)
                .ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                // Remove item
                tranWithDetails = BLTrans.RemoveDetailItem(tranWithDetails, itemToRemove);
                tranWithDetails = BLTrans.Caltran(tranWithDetails);

                // Remove toppings
                foreach (var topping in itemToRemove.tranDetailItemToppings.Where(x => x.SysItemID == item).ToList())
                {
                    itemToRemove.tranDetailItemToppings.Remove(topping);
                    tranWithDetails = BLTrans.EditToppingRow(tranWithDetails, itemToRemove);
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                }
            }
        }

        public static TranWithDetailsLocal RemoveItem(TranWithDetailsLocal tranWithDetails, long? sysItemID)
        {
            try
            {
                if (sysItemID != 0)
                {
                    TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                    tranWithDetailsLocal = tranWithDetails;
                    List<Model.TranDetailItemWithTopping> TranDetailItem = new List<Model.TranDetailItemWithTopping>();

                    TranDetailItem = tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysItemID).ToList();
                    foreach (var Item in TranDetailItem)
                    {
                        //ลบออกจาก item
                        //tranWithDetailsLocal.tranDetailItemWithToppings.Remove(Item);
                        tranWithDetails = BLTrans.RemoveDetailItem(tranWithDetails, Item);
                        tranWithDetails = BLTrans.Caltran(tranWithDetailsLocal);
                    }

                    foreach (var ItemTopping in tranWithDetailsLocal.tranDetailItemWithToppings)
                    {
                        //ลบออกจาก topping
                        var topping = ItemTopping.tranDetailItemToppings.Where(x => x.SysItemID == sysItemID).ToList();
                        foreach (var RemoveTopping in topping)
                        {
                            ItemTopping.tranDetailItemToppings.Remove(RemoveTopping);
                            tranWithDetails = BLTrans.EditToppingRow(tranWithDetails, ItemTopping);
                            tranWithDetails = BLTrans.Caltran(tranWithDetailsLocal);
                        }
                    }
                }
                return tranWithDetails; //tranWithDetails = tranWithDetailsLocal;       
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckStatusIteminCart : " + ex.Message);
                TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                return tranWithDetailsLocal;
            }
        }

        public static async Task<HasChangeinCart> CheckSettingConfiginCart(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                //เคสแก้ไขรายละเอียดของ merchantconfig
                TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                tranWithDetailsLocal = tranWithDetails;

                bool flagTaxRate = false;
                bool flagTranType = false;
                bool flagServiceCharge = false;
                bool flagtranTradDiscounts = false;

                //Vat
                if (tranWithDetailsLocal.tran.TaxRate != 0)
                {
                    var Vat = DataCashingAll.setmerchantConfig.TAXRATE;
                    var TaxRate = tranWithDetailsLocal.tran.TaxRate;
                    decimal vat = Convert.ToDecimal(string.IsNullOrEmpty(Vat) ? "0" : Vat);
                    decimal vatTran = Convert.ToDecimal(TaxRate == null ? 0 : TaxRate);
                    //vatTran = 12;
                    if (vat != vatTran)
                    {
                        tranWithDetailsLocal.tran.TaxRate = vat;
                        flagTaxRate = true;
                    }
                }

                //VatRate
                if (tranWithDetailsLocal.tran.TranTaxType != '\0')
                {
                    string VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
                    string TranTaxType = tranWithDetailsLocal.tran.TranTaxType.ToString();
                    if (VatType != TranTaxType)
                    {
                        string str = VatType;
                        tranWithDetailsLocal.tran.TranTaxType = char.Parse(str);
                        flagTranType = true;
                    }
                }

                //SERVICECHARGE_RATE
                if (!string.IsNullOrEmpty(tranWithDetailsLocal.tran.FmlServiceCharge))
                {
                    string ServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                    string SERVICECHARGE_RATE = tranWithDetailsLocal.tran.FmlServiceCharge;

                    decimal deciServiceCharge = 0;
                    var check = ServiceCharge.IndexOf('%');
                    if (check == -1)
                    {
                        deciServiceCharge = Convert.ToDecimal(ServiceCharge);
                    }
                    else
                    {
                        deciServiceCharge = Convert.ToDecimal(ServiceCharge.Remove(check));
                    }

                    decimal deciSERVICECHARGE_RATE = 0;
                    var checkRATE = SERVICECHARGE_RATE.IndexOf('%');
                    if (checkRATE == -1)
                    {
                        deciSERVICECHARGE_RATE = Convert.ToDecimal(SERVICECHARGE_RATE);
                    }
                    else
                    {
                        deciSERVICECHARGE_RATE = Convert.ToDecimal(SERVICECHARGE_RATE.Remove(checkRATE));
                    }

                    if (deciServiceCharge != deciSERVICECHARGE_RATE)
                    {
                        tranWithDetailsLocal.tran.FmlServiceCharge = ServiceCharge;
                        flagServiceCharge = true;
                    }
                }

                //Customer Membertype
                long SysCustomerID = tranWithDetails.tran.SysCustomerID;
                if ((SysCustomerID != 0 && SysCustomerID != 999) && (tranWithDetailsLocal.tranTradDiscounts != null && tranWithDetailsLocal.tranTradDiscounts.Count > 0))
                {
                    decimal fmltranTradDiscounts = 0;
                    decimal fmlDiscount = 0;

                    var tranTradDiscounts = tranWithDetailsLocal.tranTradDiscounts.Find(x => x.DiscountType.ToUpper() == "PS");
                    if (tranTradDiscounts != null)
                    {
                        fmltranTradDiscounts = decimal.Parse(tranTradDiscounts.FmlDiscount);


                        CustomerManage customerManage = new CustomerManage();
                        MemberTypeManage memberTypeManage = new MemberTypeManage();
                        var CustomerDetail = await customerManage.GetCustomer(DataCashingAll.MerchantId, (int)SysCustomerID);
                        if (CustomerDetail.MemberTypeNo is null)
                        {
                            var getcustomerType = await memberTypeManage.GetMemberType(DataCashingAll.MerchantId, Convert.ToInt32(CustomerDetail.MemberTypeNo));
                            if (getcustomerType != null && getcustomerType.PercentDiscount != 0)
                            {
                                fmlDiscount = getcustomerType.PercentDiscount;
                            }
                        }
                    }

                    if (fmltranTradDiscounts != fmlDiscount)
                    {
                        tranTradDiscounts.FmlDiscount = fmlDiscount.ToString();
                        flagtranTradDiscounts = true;
                    }
                }

                tranWithDetailsLocal = BLTrans.Caltran(tranWithDetailsLocal);
                bool flagChange = false;
                if (flagTaxRate || flagTranType || flagServiceCharge || flagtranTradDiscounts)
                {
                    flagChange = true;
                }
                HasChangeinCart hasChange = new HasChangeinCart() { tranWithDetailsLocal = tranWithDetailsLocal, FlagChange = flagChange };
                return hasChange;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CheckStatusIteminCart : " + ex.Message);
                HasChangeinCart hasChange = new HasChangeinCart() { tranWithDetailsLocal = tranWithDetails, FlagChange = false };
                return hasChange;
            }
        }

        public static async Task UpdateImageCustomer(List<Gabana3.JAM.Customer.CustomerStatus> UpdateCustomer)
        {
            try
            {
                CustomerManage customerManage = new CustomerManage();
                string thumnailLocalPath = string.Empty;
                foreach (var Customer in UpdateCustomer)
                {
                    Customer data = new Customer();
                    data = await customerManage.GetCustomer(Customer.Customers.MerchantID, Customer.Customers.SysCustomerID);
                    if (!string.IsNullOrEmpty(Customer.Customers.PicturePath))
                    {
                        if (Customer.Customers.PicturePath != data.PicturePath)
                        {
                            //delete รูป
                            if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                            {
                                Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                {
                                    System.IO.File.Delete(imgTempFile.AbsolutePath);
                                }
                                string pathImage = await Utils.InsertLocalPictureCustomerMaster(Customer.Customers.PicturePath);
                                thumnailLocalPath = pathImage ?? "";
                            }
                            else
                            {
                                thumnailLocalPath = data?.ThumbnailLocalPath;
                            }
                            data.ThumbnailLocalPath = thumnailLocalPath;
                            await customerManage.UpdateCustomer(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateImageCustomer : " + ex.Message);
            }
        }


        public static async Task UpdateImageItem(List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem)
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                string thumnailLocalPath = string.Empty;
                foreach (var item in UpdateItem)
                {
                    Item data = new Item();
                    data = await itemManage.GetItem((int)item.ItemStatus.item.MerchantID, (int)item.ItemStatus.item.SysItemID);
                    if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                    {
                        if (item.ItemStatus.item.PicturePath != data.PicturePath)
                        {
                            //delete รูป
                            if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                            {
                                Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                {
                                    System.IO.File.Delete(imgTempFile.AbsolutePath);
                                }
                            }

                            string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                            thumnailLocalPath = pathImage ?? "";
                        }
                        else
                        {
                            thumnailLocalPath = data?.ThumbnailLocalPath;
                        }
                        data.ThumbnailLocalPath = thumnailLocalPath;
                        await itemManage.UpdateItem(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateImageItem : " + ex.Message);
            }
        }

        public static async void InsertPictureLocalItem(List<Item> lstItem)
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                int i = 0;
                foreach (var item in lstItem)
                {
                    i++;
                    string thumnailPath = "";
                    if (!string.IsNullOrEmpty(item.PicturePath))
                    {
                        string pathImage = await Utils.InsertLocalPictureItemMaster(item.PicturePath);
                        thumnailPath = pathImage ?? "";
                        item.ThumbnailLocalPath = thumnailPath;
                        await itemManage.UpdateItem(item);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async void InsertPictureLocalCustomer(List<Customer> lstCustomer)
        {
            try
            {
                CustomerManage customerManage = new CustomerManage();
                foreach (var customer in lstCustomer)
                {
                    string thumnailPath = "";
                    if (!string.IsNullOrEmpty(customer.PicturePath))
                    {
                        string pathImage = await Utils.InsertLocalPictureCustomerMaster(customer.PicturePath);
                        thumnailPath = pathImage ?? "";
                        customer.ThumbnailLocalPath = thumnailPath;
                        await customerManage.UpdateCustomer(customer);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async void InsertPictureLocalMyQR(List<MyQrCode> lstMyQrCode)
        {
            try
            {
                MyQrCodeManage myQrCodeManage = new MyQrCodeManage();
                foreach (var myQrCode in lstMyQrCode)
                {
                    string path = "";
                    if (!string.IsNullOrEmpty(myQrCode.PicturePath))
                    {
                        string pathImage = await Utils.InsertLocalPictureMyQRMaster(myQrCode.PicturePath);
                        path = pathImage ?? "";
                        myQrCode.PictureLocalPath = path;
                        await myQrCodeManage.UpdateMyQrCode(myQrCode);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async void CheckImageLoadnotCompleteCustomer()
        {
            try
            {
                #region Customer
                CustomerManage customerManage = new CustomerManage();
                List<Customer> lstCustomer = new List<Customer>();
                lstCustomer = await customerManage.GetAllCustomerImageLoadnotComplete();
                lstCustomer?.Where(x => !string.IsNullOrEmpty(x.PicturePath)).ToList();
                Utils.InsertPictureLocalCustomer(lstCustomer);
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckImageLoadnotComplete at Splash");
                Log.Error("connecterror", "CheckImageLoadnotComplete : " + ex.Message);
                throw;
            }
        }

        public static async void CheckImageLoadnotCompleteItem()
        {
            try
            {
                //เช็ค ThumbnailPath กับ PicturePath ที่ข้างล่าง
                #region Item
                ItemManage itemManage = new ItemManage();
                List<Item> lstUpdateImageItem = new List<Item>();
                List<Item> lstitem = new List<Item>();
                lstitem = await itemManage.GetAllItemImageLoadnotComplete();
                Utils.InsertPictureLocalItem(lstitem);
                #endregion               
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckImageLoadnotCompleteItem at Splash");
                Log.Error("connecterror", "CheckImageLoadnotCompleteItem : " + ex.Message);
                throw;
            }
        }

        //ตรวจสอบสัญญาณอินเตอร์เน็ต

        //CheckInternetSpeed()
        public static async Task<double> CheckInternetSpeed()
        {
            //DateTime Variable To Store Download Start Time.
            DateTime dt1 = DateTime.Now;
            double internetSpeed = 0;
            try
            {
                // Create Object Of WebClient
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                //Number Of Bytes Downloaded Are Stored In ‘data’
                byte[] data = await client.GetByteArrayAsync("https://www.google.com/");
                //DateTime Variable To Store Download End Time.
                DateTime dt2 = DateTime.Now;
                //To Calculate Speed in Kb Divide Value Of data by 1024 And Then by End Time Subtract Start Time To Know Download Per Second.
                Console.WriteLine("ConnectionSpeed: DataSize (kb) " + data.Length / 1024);
                Console.WriteLine("ConnectionSpeed: ElapsedTime (secs) " + (dt2 - dt1).TotalSeconds);
                internetSpeed = Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2);
            }
            catch (Exception ex)
            {
                internetSpeed = 0;
            }
            return internetSpeed;
        }

        public static void ConvertTranwithDetailtoJson(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                var json = JsonConvert.SerializeObject(tranWithDetails);
                Preferences.Set("tranWithDetails", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public static TranWithDetailsLocal ConvertJsontoTranwithDetail(string _strtranWithDetails)
        {
            try
            {
                var tran = JsonConvert.DeserializeObject<TranWithDetailsLocal>(_strtranWithDetails);
                return tran;
            }
            catch (Exception)
            {
                return new TranWithDetailsLocal();
            }
        }

        public static IList<int> AllIndexOf(string text, string str, StringComparison comparisonType)
        {
            IList<int> allIndexOf = new List<int>();
            int index = text.IndexOf(str, comparisonType);
            while (index != -1)
            {
                allIndexOf.Add(index);
                index = text.IndexOf(str, index + 1, comparisonType);
            }
            return allIndexOf;
        }


        public static double GetUsedRAM(ActivityManager activityManager, ActivityManager.MemoryInfo memoryInfo)
        {
            try
            {
                activityManager.GetMemoryInfo(memoryInfo);
                double totalMemory = memoryInfo.TotalMem / (1024 * 1024); // ขนาดทั้งหมดของ RAM ในเครื่อง (หน่วยเป็น MB)
                double usedMemory = (memoryInfo.TotalMem - memoryInfo.AvailMem) / (1024 * 1024); // จำนวน RAM ที่ใช้งานอยู่ (หน่วยเป็น MB)
                return usedMemory;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting used RAM: " + ex.Message);
                return 0;
            }
        }


        public static void ClearRam()
        {
            try
            {
                ClearMemory();
                ClearCaches();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling memory issues: " + ex.Message);
            }
        }

        public static void ClearMemory()
        {
            try
            {
                // ปลดปล่อยทรัพยากรที่ไม่ได้ใช้งานเพื่อลดการใช้งานหน่วยความจำ
                GC.Collect();

                Java.Lang.JavaSystem.RunFinalization();
                Java.Lang.Runtime.GetRuntime().Gc();
                Java.Lang.JavaSystem.Gc();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ClearMemory caches: " + ex.Message);
            }
        }

        private static void ClearCaches()
        {
            try
            {
                // ตำแหน่งของแคชในแอปพลิเคชัน Android
                string cacheDir = Android.App.Application.Context.CacheDir.AbsolutePath;

                // ลบแคชทั้งหมดในตำแหน่งที่ระบุ
                if (Directory.Exists(cacheDir))
                {
                    Directory.Delete(cacheDir, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing caches: " + ex.Message);
            }
        }

        public static async void TestPrint(AssetManager assets, Activity activity)
        {
            try
            {
                byte[] bytesData = null;
                int lengThName = 28;
                if (DataCashingAll.setting.PRINTTYPE == "Image")
                {
                    if (DataCashingAll.setting?.TYPEPAGE.Substring(0, 2) == "80")
                    {
                        lengThName = 35;
                    }

                    List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
                    items.Add(new List<KeyValuePair<string, string>>()
                    {
                    new KeyValuePair<string, string>("@QuantityItem","1"),
                    new KeyValuePair<string, string>("@ItemName","Test"),
                    new KeyValuePair<string, string>("@ItemPrice","100.00")
                    });
                    items.Add(new List<KeyValuePair<string, string>>()
                    {
                    new KeyValuePair<string, string>("@QuantityItem","2"),
                    new KeyValuePair<string, string>("@ItemName","Test2"),
                    new KeyValuePair<string, string>("@ItemPrice","200.00")
                    });


                    string vat1 = "";
                    string vat2 = "";
                    string vat3 = "";
                    string service1 = "ค่าบริการ";
                    string service2 = "Service";
                    string service3 = "0.00";
                    string member1 = "สมาชิก";
                    string member2 = "Member";
                    string member3 = "0.00";
                    string discountBill1 = "ส่วนลด";
                    string discountBill2 = "Discount";
                    string discountBill3 = "0.00";
                    string merchantAddress1 = "";
                    string merchantAddress2 = "";
                    string merchantAddress3 = "";

                    var address = SplitAddress("2991/23-24 อาคารซีเนียร์ซอฟท์ โครงการวิสุทธานี ถนนลาดพร้าว แขวงคลองจั่น เขตบางกะปิ กรุงเทพมหานคร 10240");
                    merchantAddress1 = address[0];
                    merchantAddress2 = address[1];
                    merchantAddress3 = address[2];

                    //Service charge
                    service1 = "ค่าบริการ " + "XX.XX " + "%";
                    service2 = "Service ";
                    service3 = "XX.XX";

                    //Discount From Member 
                    member1 = "สมาชิก " + "XX.XX" + "%";
                    member2 = "Member ";
                    member3 = "-" + "XX.XX";

                    discountBill1 = "ส่วนลด";
                    discountBill2 = "Discount";
                    discountBill3 = "-" + "XX.XX ";

                    //Vat
                    vat1 = "ภาษีมูลค่าเพิ่ม " + "7 %";
                    vat2 = "Vat " + "7 %";
                    vat3 = "XX.XX";

                    ParamSlip paramSlip = new ParamSlip()
                    {
                        Header = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("@VoucherName", "พิมพ์ทดสอบ"),
                            new KeyValuePair<string, string>("@merchantName", "บริษัท ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด(สำนักงานใหญ่)"),
                            new KeyValuePair<string, string>("@merchantAddress1", merchantAddress1),
                            new KeyValuePair<string, string>("@merchantAddress2", merchantAddress2),
                            new KeyValuePair<string, string>("@merchantAddress3", merchantAddress3),
                            new KeyValuePair<string, string>("@merchantTel", "02-692-5899"),
                            new KeyValuePair<string, string>("@merchantTaxid", "1234567891123"),
                            new KeyValuePair<string, string>("@branchTaxId", "00000"),
                            new KeyValuePair<string, string>("@Billno", "XXXXXXXXXXXXXXXX"),
                            new KeyValuePair<string, string>("@Date", "DD/MM/YYYY"),
                            new KeyValuePair<string, string>("@Time","HH:mm"),

                            new KeyValuePair<string, string>("@Person", "Customer"),
                            new KeyValuePair<string, string>("@Address1", "Address"),
                            new KeyValuePair<string, string>("@Address2", "Address"),

                            new KeyValuePair<string, string>("@CountDetail", "X"),
                            new KeyValuePair<string, string>("@Quantity", "X"),

                            new KeyValuePair<string, string>("@Cashier", "Cashier : Employee"),
                            new KeyValuePair<string, string>("@Vat1", vat1),
                            new KeyValuePair<string, string>("@Vat2", vat2),
                            new KeyValuePair<string, string>("@Vat3", vat3),
                            new KeyValuePair<string, string>("@Service1", service1),
                            new KeyValuePair<string, string>("@Service2", service2),
                            new KeyValuePair<string, string>("@Service3", service3),
                            new KeyValuePair<string, string>("@Member1", member1),
                            new KeyValuePair<string, string>("@Member2", member2),
                            new KeyValuePair<string, string>("@Member3", member3),
                            new KeyValuePair<string, string>("@Discount1", discountBill1),
                            new KeyValuePair<string, string>("@Discount2", discountBill2),
                            new KeyValuePair<string, string>("@Discount3", discountBill3),
                            new KeyValuePair<string, string>("@Total", "XX.XX"),
                            new KeyValuePair<string, string>("@GrandTotal", "XX.XX"),

                        },
                        Details = items,
                        Footer = items
                    };

                    Graphic graphic = new Graphic();
                    if (DataCashingAll.setting.TYPEPAGE == "80mm" || DataCashingAll.setting.TYPEPAGE == "80 มม.")
                    {
                        bytesData = graphic.DrawImageFromXml("SlipTrans80mm.xml", paramSlip, assets);
                    }
                    else
                    {
                        bytesData = graphic.DrawImageFromXml("SlipTrans58mm.xml", paramSlip, assets);
                    }
                }
                
                PrintController c = new PrintController();
                if (DataCashingAll.setting.USE == "Wifi")
                {
                    try
                    {
                        await c.PrintWifi(bytesData);
                        Thread.Sleep(10000);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
                else
                {
                    BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                    ICollection<BluetoothDevice> bondedDevices = bluetoothAdapter.BondedDevices;
                    var printer = bondedDevices.Where(x => x.Name == DataCashingAll.setting.BLUETOOTH1 && x.Address == DataCashingAll.setting.IPADDRESS).FirstOrDefault();
                    if (printer == null)
                    {
                        Toast.MakeText(activity, "Bluetooth DisConnected", ToastLength.Short).Show();
                    }
                    else
                    {
                        BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(printer.Address);
                        Java.Util.UUID UDID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                        BluetoothSocket socket = device.CreateRfcommSocketToServiceRecord(UDID);
                        socket.Connect();
                        try
                        {
                            Java.Lang.Thread.Sleep(3000);
                        }
                        catch (Exception)
                        {
                        }

                        if (DataCashingAll.setting.COMMAND != "Epson Command")
                        {
                            try
                            {
                                if (!socket.IsConnected)
                                {
                                    socket.Connect();
                                    DataCashingAll.addresssame = DataCashingAll.setting.IPADDRESS;
                                }
                            }
                            catch (Exception)
                            {
                                socket.Close();

                                socket = printer.CreateInsecureRfcommSocketToServiceRecord(UDID);
                                socket.Connect();

                            }
                        }

                        try
                        {
                            if (!socket.IsConnected)
                            {
                                socket.Connect();
                                DataCashingAll.addresssame = DataCashingAll.setting.IPADDRESS;
                            }

                            if (socket.IsConnected)
                            {
                                Stream outputStream = socket.OutputStream;
                                outputStream.Flush();

                                var typeprint = DataCashingAll.setting.PRINTTYPE;
                                if (typeprint != "Image")
                                {
                                    List<byte> bytelist = new List<byte>();
                                    bytelist.Add((byte)27);
                                    bytelist.Add((byte)97);
                                    bytelist.Add((byte)49);

                                    bytelist.Add((byte)29);
                                    bytelist.Add((byte)33);
                                    bytelist.Add((byte)0);

                                    await outputStream.WriteAsync(bytelist.ToArray());
                                    var imageforprinttext = await DrawString();
                                    foreach (string txt in imageforprinttext)
                                    {
                                        var txt1 = txt;
                                        var x2 = ThaiLength(txt);
                                        var enc = System.Text.Encoding.GetEncoding("Windows-874");
                                        byte[] bytes = enc.GetBytes(txt1);
                                        var x = txt1.Length;
                                        await outputStream.WriteAsync(bytes);
                                        await outputStream.WriteAsync(new byte[] { (byte)10 });
                                    }
                                    byte[] byteArray = Encoding.ASCII.GetBytes("\n\n");
                                    await outputStream.WriteAsync(byteArray, 0, byteArray.Length);
                                }
                                else
                                {
                                    await outputStream.WriteAsync(bytesData, 0, bytesData.Length);
                                    byte[] byteArray = Encoding.ASCII.GetBytes("\n\n");
                                    await outputStream.WriteAsync(byteArray, 0, byteArray.Length);
                                    if (printer.Address != "00:11:22:33:44:55")
                                    {
                                        Thread.Sleep(4000);

                                        if (DataCashingAll.setting.TYPEPAGE == "80mm" || DataCashingAll.setting.TYPEPAGE == "80 มม.")
                                        {
                                            Thread.Sleep(4000);
                                        }
                                    }
                                }
                                if (DataCashingAll.setting.COMMAND == "Epson Command")
                                {
                                    socket.Close();
                                }
                                await outputStream.FlushAsync();
                            }
                            else
                            {
                                Toast.MakeText(activity, "Bluetooth DisConnected", ToastLength.Short).Show();
                            }
                        }
                        catch (IOException ee)
                        {
                            Toast.MakeText(activity, ee.Message, ToastLength.Short).Show();
                        }
                        catch (Exception ex)
                        {
                            if (socket != null)
                            {
                                try
                                {
                                    socket.Close();
                                }
                                catch (IOException)
                                {

                                }
                                socket = null;
                            }
                        }
                        finally
                        {
                            if (socket != null)
                            {
                                socket.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static async Task<List<string>> DrawString()
        {
            int size;
            if (DataCashingAll.setting?.TYPEPAGE.Substring(0, 2) == "58")
            {
                size = 1;
            }
            else
            {
                size = 2;
            }
            var list = await Draw4(size);
            return list;
        }

        public static int ThaiLength(string stringthai)
        {
            int len = 0;
            int l = stringthai.Length;
            for (int i = 0; i < l; ++i)
            {
                if (char.GetUnicodeCategory(stringthai[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    ++len;
            }
            return len;
        }

        public static async Task<List<string>> Draw4(int size)
        {
            List<string> data = new List<string>();
            int[] itemLength = { 28, 40 };
            int[] itemLength2 = { 33, 48 };

            EscPosCommand escPos = new EscPosCommand(size);
            data.Add("พิมพ์ทดสอบ");

            string mername = "";
            string MerchantName = "บริษัท ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์ จำกัด(สำนักงานใหญ่)";
            if (ThaiLength("") > itemLength[size - 1])
            {
                mername = MerchantName.Substring(0, itemLength[size - 1]);
            }
            else
            {
                mername = MerchantName;
            }
            data.Add(mername);

            data.Add(escPos.ReplaceSpacebar2("MerchantName", "DD/MM/YYYY HH:mm"));
            data.Add(escPos.ReplaceSpacebar2("Customer", ""));
            data.Add(escPos.ReplaceSpacebar2("จำนวนรายการ : " + "X", "รวมจำนวนชิ้น : " + "X"));
            data.Add(escPos.ReplaceSpacebar2("รวมเงิน", "XX.XX"));
            data.Add(escPos.ReplaceSpacebar2("ภาษีมูลค่าเพิ่ม " + "X" + " %", "XX.XX"));
            data.Add(escPos.ReplaceSpacebar2("ค่าบริการ " + "X%", "XX.XX"));
            data.Add(escPos.ReplaceSpacebar2("สมาชิก " + "X%", "-" + "XX.XX"));
            data.Add(escPos.ReplaceSpacebar2("ส่วนลด", "-" + "XX.XX"));
            data.Add(escPos.ReplaceSpacebar2("รวมทั้งสิ้น", "XX.XX"));

            data.Add("...THANK YOU...");
            data.Add("Cashier: " + "Cashier : Employee");
            data.Add("Powered By Seniorsoft");
            return data;
        }

    }
}