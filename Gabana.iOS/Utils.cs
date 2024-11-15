using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acr.Logging;
using AVFoundation;
using CoreGraphics;
using FFImageLoading;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.Period;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Merchant;
using GlobalToast;
using LinqToDB.Common;
using Newtonsoft.Json;
using Plugin.BLE;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;

namespace Gabana.iOS
{
    public class Utils
    {
        public static string nameTH, nameEn;
        private static byte[] imageItemByteArray;
        internal static void ShowAlert(UIViewController uIViewController, string title, string detail)
        {
            var alert = UIAlertController.Create(title, detail, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default, null));
            uIViewController.PresentViewController(alert, animated: false, completionHandler: null);
        }
        public static DateTime ChangeDateTimeStringReport(string checkDate)
        {
            DateTime convertDate = DateTime.ParseExact(checkDate, "yyyyMMdd", new CultureInfo("en-US"));
            return convertDate;
        }
        public static string ChangeDateTime(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
            return datetime;
        }
        public static async Task CheckJWT()
        {
            try
            {
                if (!await GabanaAPI.CheckNetWork()) { return; }
                if (!await GabanaAPI.isValidToken(GabanaAPI.gbnJWT))
                {

                    string LoginType = Preferences.Get("LoginType", "");
                    if (LoginType.ToLower() == "owner")
                    {
                         await Gabana.ShareSource.GetToken.RefreshToken(Preferences.Get("RefreshToken", ""), 'o');
                    }
                    else
                    {
                         await Gabana.ShareSource.GetToken.RefreshToken(Preferences.Get("RefreshToken", ""), 'e');
                    }
                }
                
            }
            catch (Exception ex)
            {
                Preferences.Set("AppState", "logout");
                Preferences.Set("Branch", "");
                POSController.tranWithDetails = null;
                //await BellNotificationHelper.UnRegisterBellNotification(GabanaAPI.gbnJWT);
                SplashLoadingController SplashLoading = new SplashLoadingController();
                UIWindow uIWindowRoot = UIApplication.SharedApplication.Windows.First();
                uIWindowRoot.RootViewController = SplashLoading;
            }
        }
        public static void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        public static string TextBundle(string id, string defaulttext)
        {
            //var xx = NSBundle._AllBundles;
            //var text2 = NSBundle.FromIdentifier("th").GetLocalizedString(id, defaulttext);
            //NSBundle.MainBundle.PreferredLocalizations = new string[] { "fr" };
            //var x = NSBundle.LocalizedString(id, defaulttext);
            var text = NSBundle.MainBundle.GetLocalizedString(id, defaulttext);
            

            return text;
        }
        public static async Task ReloadInitialData()
        {
            var systemSeqNoManage = new DeviceSystemSeqNoManage();
            var deviceSystemSeq = new DeviceSystemSeqNo();
            var systemRevisionNoManage = new SystemRevisionNoManage();
            var SystemInfoManage = new SystemInfoManage();
            var SystemRevisionNo = new SystemRevisionNo();
            var lstSystemInfo = await SystemInfoManage.GetSystemInfo();
            if (lstSystemInfo.Count == 0)
            {
                List<SystemInfo> systemInfo = new List<SystemInfo>()
                    {
                        new SystemInfo(){ Name = "Branch" , SystemID = 10},
                        new SystemInfo(){ Name = "Category" , SystemID = 20},
                        new SystemInfo(){ Name = "Item" , SystemID = 30},
                        new SystemInfo(){ Name = "ItemOnBranch" , SystemID = 31},
                        new SystemInfo(){ Name = "Discount" , SystemID = 40},
                        new SystemInfo(){ Name = "Customer" , SystemID = 50},
                        new SystemInfo(){ Name = "NoteCategory" , SystemID = 60},
                        new SystemInfo(){ Name = "Note" , SystemID = 70}
                    };
                await SystemInfoManage.InsertSystemInfo(systemInfo);
                lstSystemInfo = await SystemInfoManage.GetSystemInfo();
            }

            var listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
            if (listRivision.Count == 0)
            {
                for (int i = 0; i < lstSystemInfo.Count; i++)
                {
                    SystemRevisionNo = new SystemRevisionNo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        SystemID = lstSystemInfo[i].SystemID,
                        LastRevisionNo = 0
                    };
                    var result = await systemRevisionNoManage.InsertSystemRevisionno(SystemRevisionNo);

                    deviceSystemSeq = new DeviceSystemSeqNo
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        DeviceNo = DataCashingAll.Merchant.Device.DeviceNo,
                        SystemID = lstSystemInfo[i].SystemID,
                        LastSysSeqNo = 0
                    };
                    var resultSeqNo = await systemSeqNoManage.InsertDeviceSystemSeqNo(deviceSystemSeq);

                }
            }
            DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);
            var setDevice = Preferences.Get("Device", "");
            var Config = JsonConvert.DeserializeObject<Device>(setDevice);
            DataCashingAll.Device = Config;
            DataCashingAll.DeviceNo = Config.DeviceNo;
            if (DataCashingAll.Merchant == null)
            {
                var jsonmerchants = Preferences.Get("Merchant", "");
                var Merchant = JsonConvert.DeserializeObject<Gabana3.JAM.Merchant.Merchants>(jsonmerchants);
                DataCashingAll.Merchant = Merchant;
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
                    return 1;

            }
        }
        internal static List<string> SetDetailPackage(string id)
        {
            //return maxuser , maxbranch
            List<string> list = new List<string>();
            var idnew = int.Parse(id);
            switch (idnew.ToString())
            {
                case "1":
                    list = new List<string>() { "5", "1" };
                    return list;
                case "2":
                    list = new List<string>() { "10", "1" };
                    return list;
                case "3":
                    list = new List<string>() { "15", "3" };
                    return list;
                case "4":
                    list = new List<string>() { "30", "3" };
                    return list;
                default:
                    list = new List<string>() { "5", "1" };
                    return list;

            }
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

        public static async Task<bool> Print(TranWithDetailsLocal tranWithDetails , UIViewController thisview) 
        {
            try
            {

                var BranchDetail = new ORM.MerchantDB.Branch();
                GabanaLoading.SharedInstance.Show(thisview);
                bool print = true;
                if (await GabanaAPI.CheckNetWork())
                {
                    List<ORM.Master.Branch> cloudbranch = new List<ORM.Master.Branch>();
                    cloudbranch = await GabanaAPI.GetDataBranch();
                    var local = cloudbranch.Where(x => x.SysBranchID == tranWithDetails.tran.SysBranchID).FirstOrDefault();
                    BranchDetail = new ORM.MerchantDB.Branch()
                    {
                        Address = local.Address,
                        ProvincesId = local.ProvincesId,
                        AmphuresId = local.AmphuresId,
                        DistrictsId = local.DistrictsId,
                        Tel = local.Tel,
                    };
                }
                else
                {
                    BranchManage branchManage = new BranchManage();
                    BranchDetail = await branchManage.GetBranch(DataCashingAll.MerchantId, (int)tranWithDetails.tran.SysBranchID);
                }
                string branchaddress = "";
                if (BranchDetail != null)
                {
                    
                    branchaddress = await Utils.SetTextAddressAsync(BranchDetail);
                }
                var address = Utils.SplitAddress(branchaddress);
                var imageforprint = CreateImageIOS.DrawImage(tranWithDetails,address);

                PrintController c = new PrintController();
                if (DataCashingAll.setting.TYPE == "Wifi")
                {
                    await c.PrintWifi(imageforprint);
                    Utils.ShowMessage("Print Success!");
                    GabanaLoading.SharedInstance.Hide();
                }
                else
                {
                    //DumpView dumpView = new DumpView();


                    Plugin.BLE.Abstractions.ConnectParameters connectParameters = new Plugin.BLE.Abstractions.ConnectParameters(false, false);
                    var adapter = CrossBluetoothLE.Current.Adapter;
                    adapter.ScanTimeout = 1000;
                    adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
                    if (DataCashingAll.setting.BLUETOOTH1 is null)
                    {
                        Utils.ShowMessage("กรุณาตั้งค่าเครื่องปริ้น");
                        GabanaLoading.SharedInstance.Hide();
                        return false;
                    }

                    //IAdapter device = await adapter.ConnectToKnownDeviceAsync(Guid.Parse(DataCashingAll.setting.BLUETOOTH1), connectParameters);
                    int timeout = 10000; // Longer than you'd expect!
                    bool status = false;
                    var task = adapter.ConnectToKnownDeviceAsync(Guid.Parse(DataCashingAll.setting.BLUETOOTH1), connectParameters);
                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        // task completed within timeout
                        var dvc = task.Result;

                        //Utils.ShowMessage("connect");
                        GabanaLoading.SharedInstance.Hide();
                        status = true;
                    }
                    else
                    {
                        Utils.ShowMessage("ไม่สามารถเชื่อมต่อเครื่องปริ้นได้");
                        GabanaLoading.SharedInstance.Hide();
                        return false;
                        // timeout logic
                    }
                    if (task.IsCompleted)
                    {
                        task.Dispose();
                    }
                    
                    if (status)
                    {
                        var device = await adapter.ConnectToKnownDeviceAsync(Guid.Parse(DataCashingAll.setting.BLUETOOTH1), connectParameters);

                        var t = await device.RequestMtuAsync(500);
                        device.UpdateConnectionInterval(Plugin.BLE.Abstractions.ConnectionInterval.High);
                        //await device.UpdateRssiAsync();
                        //var t2 = await device.RequestMtuAsync(244);

                        var Services = await device.GetServicesAsync();

                        foreach (var ser in Services)
                        {
                            //if (ser.Name == "Device Information")
                            //{
                            var Servicethis = await device.GetServiceAsync(ser.Id);
                            var characteristics = await Servicethis.GetCharacteristicsAsync();

                            foreach (var item in characteristics)
                            {
                                if (item.CanWrite && print)
                                {
                                    try
                                    {


                                        item.WriteType = Plugin.BLE.Abstractions.CharacteristicWriteType.WithoutResponse;
                                        
                                        var typemain = DataCashingAll.PrintType;

                                        if (typemain == "Text")
                                        {
                                            var typesub = "Window-874";
                                            //var typesub = DataCashingAll.setting.TYPESPEED.Split(" ")[1];
                                            var imageforprinttext = await CreateImageIOS.DrawString(tranWithDetails);
                                            List<byte> bytelist = new List<byte>();
                                            bytelist.Add((byte)27);
                                            bytelist.Add((byte)97);
                                            bytelist.Add((byte)49);

                                            bytelist.Add((byte)29);
                                            bytelist.Add((byte)33);
                                            bytelist.Add((byte)0);
                                            await item.WriteAsync(bytelist.ToArray());

                                            int size = 0;
                                            int[] itemLength = { 33, 48 };
                                            if (DataCashingAll.setting.TYPEPAGE != "58mm")
                                            {
                                                size = 1;
                                            }

                                            foreach (string txt in imageforprinttext)
                                            {
                                                var txt1 = txt;
                                                var x2 = ThaiLength(txt);

                                                switch (typesub)
                                                {

                                                    case "Window-874":
                                                        var enc = Encoding.GetEncoding("windows-874");
                                                        byte[] bytes = enc.GetBytes(txt1);
                                                        //var x = txt1.Length;
                                                        await item.WriteAsync(bytes);
                                                        break;
                                                    default:

                                                        await item.WriteAsync(System.Text.Encoding.UTF8.GetBytes(txt1));
                                                        var x1 = txt1.Length;
                                                        break;
                                                }
                                                Thread.Sleep(200);
                                                await item.WriteAsync(new byte[] { (byte)10 });
                                            }
                                        }
                                        else
                                        {
                                            await item.WriteAsync(imageforprint);
                                        }

                                        await item.WriteAsync(new byte[] { (byte)10 });
                                        await item.WriteAsync(new byte[] { (byte)10 });
                                        print = false;
                                        Utils.ShowMessage("Print Success !");
                                    }
                                    catch (Exception ex)
                                    {
                                        GabanaLoading.SharedInstance.Hide();
                                        //throw new Exception("ไม่สามารถเชื่อมต่อกับเครื่องปริ้นได้");
                                    }
                                }
                            }
                        }
                        GabanaLoading.SharedInstance.Hide();
                        //var imageforprinttext = CreateImageIOS.DrawString(tran);
                        //c.PrintBluetooth(imageforprint);
                        device.Dispose();
                    }
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                GabanaLoading.SharedInstance.Hide();
                throw ex;
                return false;
                //Utils.ShowMessage(ex.Message);

            }
        }
        public static List<string> SplitItemName(int length, string itemName)
        {
            List<string> names = new List<string>();
            string line1 = "", line2 = "", line3 = "";

            names.Add(line1);
            names.Add(line2);
            names.Add(line3);

            if (DataCashingAll.PrintType != "Image")
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
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return price.ToString();
            }
        }
        public static string PrintDateTime(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string time = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd/MM/yyyy HH:mm tt", new CultureInfo("en-US"));
            return time;
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
        public static bool Checkpermisstion()
        {
            AVAuthorizationStatus authStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
            
            if (authStatus == AVAuthorizationStatus.Authorized)
            {
                return true;
                
            }
            else
            {
                Utils.ShowMessage("ไม่มีสิทธิ์การเข้าถึงกล้อง กรุณาเข้าไปที่ตั้งค่า");
                bool per = false;
                
                AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, (granted) =>
                {
                    per = granted;

                });
                return per;
            }
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
                    //Java.IO.File imgTempFile = new Java.IO.File(item?.ThumbnailLocalPath);

                    //if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                    //{
                    //    System.IO.File.Delete(imgTempFile.AbsolutePath);
                    //}
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
            catch (Exception ex )
            {
                item.FWaitSending = 0;
                item.DataStatus = 'D';
                await itemManage.UpdateItem(item);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public async static Task CancelTranOrder(Model.TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                TransManage transManage = new TransManage();

                //กด Back โดยไม่ได้แก้ไขหรือเปิดบิล จะทำการ Set  FWaitSending เป็น 1 หรือ 2 ขึ้นอยู่กับสถานะการเชื่อมต่อ internet
                if (await GabanaAPI.CheckNetWork())
                {
                    tranWithDetails.tran.Status = 110;
                    tranWithDetails.tran.FWaitSending = 1;
                    tranWithDetails.tran.TranDate = Utils.GetTranDate(tranWithDetails.tran.TranDate);
                    tranWithDetails.tran.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                    tranWithDetails.tran.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);

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
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public static string ShowDateReport(DateTime d)
        {
            try
            {
                var destinationTimeZone = TimeZoneInfo.Local;
                string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                return datetime;
            }
            catch (Exception ex)
            {
                //throw;
                var destinationTimeZone = TimeZoneInfo.Local;
                DateTime now = DateTime.SpecifyKind(d, DateTimeKind.Local);
                string datetime = TimeZoneInfo.ConvertTimeToUtc(now, destinationTimeZone).ToString("dd-MM-yyyy", new CultureInfo("en-US"));
                return datetime;
            }
        }
        public static string SetPaymentImage(string PaymentType)
        {
            string name;
            if (PaymentType == null)
            {
                return "Cash";
            }
            switch (PaymentType.ToUpper())
            {
                case "CH":
                    name = "Cash";
                    break;
                case "DR":
                    name = "Credit";
                    break;
                case "CR":
                    name = "Credit";
                    break;
                case "GV":
                    name = "Giftvoucher";
                    break;
                case "EP":
                    name = "ePayment";
                    break;
                case "MYQR":
                    name = "HistoryQR";
                    break;
                default:
                    name = "Cash";
                    break;
            }
            return name;
        }

        internal static void SetTitle(UINavigationController view ,  string v)
        {
            if (view==null)
            {
                return;
            }
            
            if (view.NavigationBar.TopItem.BackBarButtonItem == null)
            {
                var backButton = new Btnback();
                backButton.TintColor = UIColor.FromRGB(64, 64, 64);
                view.NavigationBar.TopItem.BackBarButtonItem = backButton;
                view.NavigationBar.TopItem.BackBarButtonItem.Title = v;
            }
            else
            {
                view.NavigationBar.TopItem.BackBarButtonItem.Title = v;
                
            }
            //DataCaching.TitlePage. = v;
        }

        internal static void SetTitle2(UINavigationController navigationController, string v)
        {
            var x = navigationController.NavigationBar.Items.ToList();
            x.RemoveAt(navigationController.NavigationBar.Items.Length - 2);
            foreach (var item in x)
            {
                if (item.BackBarButtonItem != null)
                {
                    item.BackBarButtonItem.Title = v;
                }
            }
        }
        internal static void SetTitle3(UINavigationController view, string v)
        {
            if (view == null)
            {
                return;
            }

            if (view.NavigationBar.TopItem.BackBarButtonItem == null)
            {
                var backButton = new UIBarButtonItem();
                backButton.TintColor = UIColor.FromRGB(255, 255, 255);
                view.NavigationBar.TopItem.BackBarButtonItem = backButton;
                view.NavigationBar.TopItem.BackBarButtonItem.Title = v;
            }
            else
            {
                view.NavigationBar.TopItem.BackBarButtonItem.Title = v;

            }
        }

        public static string TextCURRENCYSYMBOLS(string CURRENCY_SYMBOLS)
        {
            string name;
            if (CURRENCY_SYMBOLS == null)
            {
                return "Cash";
            }
            switch (CURRENCY_SYMBOLS)
            {
                case "$":
                    name = "Dollar";
                    break;
                case "฿":
                    name = "Baht";
                    break;
                case "€":
                    name = "Euro";
                    break;
                case "¥":
                    name = "Yen";
                    break;
                //    break;
                case " ":
                    name = "";
                    break;
                default:
                    name = "";
                    break;
            }
            return name;
        }
        public static List<string> Cuttext(string message, int length)
        {
            try
            {
                var val = new List<string>();
                while (message?.Length > length)
                {
                    if (message.IndexOf(" ", length - 15) != -1)
                    {
                        var text = message.Substring(0, message.IndexOf(" ", length - 15));
                        //var text2 = message.Substring(0, message.IndexOf(" ", length - 15));
                        val.Add(text);
                        var xx = message.IndexOf(" ", length - 15);
                        message = message.Remove(0, message.IndexOf(" ", length - 15));
                    }
                    else
                    {
                        var text = message.Substring(0, length);
                        val.Add(text);
                        message = message.Remove(0, length);
                    }
                }


                if (message?.Length > 0)
                {
                    val.Add(message);
                }
                return val;
            }
            catch (Exception ex)
            {
                ShowMessage("sssssssss");
                return new List<string>();
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

                //if (DataCashing.Language == "th")
                //{
                //    address = addressTH;
                //}
                //else
                //{
                //    address = addressEN;
                //}
                address = addressTH;
                address = branch.Address + " " + address.Trim() + " " + ZipCode;

                return address;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public static List<string> SplitAddress(string merchantAddress)
        {
            List<string> address = new List<string>();
            var rest = merchantAddress.Split(' ');
            int lengPage = 40;
            if (true)
            {

            }

            string line1 = "", line2 = "", line3 = "";
            if (merchantAddress.Length <= lengPage)
            {
                line1 = merchantAddress;
            }
            else
            {
                string text = "";
                for (int i = 0; i < rest.Count(); i++)
                {
                    text += rest[i] + " ";
                    line2 = merchantAddress.Substring(text.Length);
                    if (text.Length < line2.Length && text.Length < lengPage)
                    {
                        line1 = text;
                    }
                    else
                    {
                        break;
                    }
                }

                line2 = merchantAddress.Substring(line1.Length);

                if (line2.Length > 40)
                {
                    string textLine2 = line2;
                    text = "";
                    var splitLin2 = textLine2.Split(' ');
                    for (int i = 0; i < splitLin2.Count(); i++)
                    {
                        text += splitLin2[i] + " ";
                        line3 = textLine2.Substring(text.Length);
                        if (text.Length < line3.Length && text.Length < lengPage)
                        {
                            line2 = text;
                        }
                        else
                        {
                            break;
                        }
                    }

                    line3 = textLine2.Substring(line2.Length);
                    if (line3.Length > lengPage)
                    {
                        var splitLin3 = line3.Split(' ');

                        for (int i = 0; i < splitLin3.Count(); i++)
                        {
                            text += splitLin3[i] + " ";
                            if (text.Length < lengPage)
                            {
                                line3 = text;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }


                }

            }
            address.Add(line1);
            address.Add(line2);
            address.Add(line3);

            return address;

        }
        public static UIViewController GetCurrentUIController()
        {
            UIViewController viewController;
            var window = UIApplication.SharedApplication.KeyWindow;
            if (window == null)
            {
                throw new InvalidOperationException("There's no current active window");
            }

            if (window.RootViewController.PresentedViewController == null)
            {
                window = UIApplication.SharedApplication.Windows
                         .First(i => i.RootViewController != null &&
                                     i.RootViewController.GetType().FullName
                                     .Contains(typeof(Xamarin.Forms.Platform.iOS.Platform).FullName));
            }

            viewController = window.RootViewController;

            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            return viewController;
        }
        internal static void ShowMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Toast.MakeToast(message)
                     .SetPosition(ToastPosition.Top) // Default is Bottom
                     .SetAnimator(new GlobalToast.Animation.ScaleAnimator())
                     .SetDuration(ToastDuration.Long) // Default is Regular
                     .Show();

            }
        }

        internal static void ShowMessageDown(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Toast.MakeToast(message)
                     .SetPosition(ToastPosition.Bottom) // Default is Bottom
                     .SetAnimator(new GlobalToast.Animation.ScaleAnimator())
                     .SetDuration(ToastDuration.Regular) // Default is Regular
                     .Show();

            }
        }
        static UIImage FromUrl(string uri)
        {
            try
            {
                using (var url = new NSUrl(uri))
                using (var data = NSData.FromUrl(url))
                    return UIImage.LoadFromData(data);
            }
            catch (Exception ex)
            {
                return null;
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
            catch (Exception ex)
            {
                return strValue;
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

                string[] fullpart = pathImage.Split("https://myseniorsoftresourcegrou.blob.core.windows.net/gabana/");
                path = fullpart[1].Remove(0, 9);
                return path;
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return string.Empty;
            }
        }
        

        public static void SetColor(UIImageView imageView, long value)
        {
            int R, G, B;
            B = Convert.ToInt32(value / 65536);
            G = Convert.ToInt32((value - B * 65536) / 256);
            R = B;
            B = Convert.ToInt32(value - B * 65536 - G * 256);
            if (value != null && (R != 0 || G != 0 || B != 0))
            {
                imageView.BackgroundColor = UIColor.FromRGB(R, G, B);
            }
            else
            {
                imageView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
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
                _ = TinyInsights.TrackErrorAsync(ex);
                return d.ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
            }
        }
        public static string SetPaymentName(string PaymentType)
        {
            string name;

            switch (PaymentType.ToUpper())
            {
                case "CH":
                    nameEn = "Cash";
                    nameTH = "ชำระด้วยเงินสด";
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
                case "MYQR":
                    nameEn = "myQR";
                    nameTH = "myQR";
                    break;
                default:
                    break;
            }
            //if (DataCaching.Language == "th")
            //{
            //    name = nameTH;
            //}
            //else
            //{
                name = nameEn;
           // }

            return name;
        }
        internal static string DisplayDecimal(decimal price)
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
        internal static void OnKeyboardChanged(UIView view, bool visible, nfloat nfloat)
        {
            if (!visible)
                view.Frame = new CGRect(0, 0, view.Frame.Width, view.Frame.Height);
            else

                view.Frame = new CGRect(0, 0 - nfloat, view.Frame.Width, view.Frame.Height);
        }
        public static void SetColor(UIView ContentView, long value)
        {
            int R, G, B;
            if(value == -1)
            {
                ContentView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            }
            else
            {
                B = Convert.ToInt32(value / 65536);
                G = Convert.ToInt32((value - B * 65536) / 256);
                R = B;
                B = Convert.ToInt32(value - B * 65536 - G * 256);
                if (value != null && (R != 0 || G != 0 || B != 0))
                {
                    ContentView.BackgroundColor = UIColor.FromRGB(R, G, B);
                }
                else
                {
                    ContentView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
            }

        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static string DisplayDecimal2(decimal price)
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
        internal static void SetImageURL(UIImageView imageView, string PicturePath)
        {
            if (!string.IsNullOrEmpty(PicturePath))
            {
               // var pathPlaceholder = "NoMenu.png";
                ImageService.Instance.LoadUrl(PicturePath)
                    .LoadingPlaceholder("defaultcust.png", FFImageLoading.Work.ImageSource.CompiledResource)
                    .WithCache(FFImageLoading.Cache.CacheType.Disk)
                    .Into(imageView);
                //if(imageView.Image == null)
                //{
                //    SetImage(imageView, PicturePath);
                //}
            }
            else
            {
                imageView.Image = null;
            }
        }
        internal static void SetImage(UIImageView ImageView, string value)
        {
            if (value != null && value != "")
            {
                if(".png" == value.Substring(value.Length - 4).ToLower())
                {
                    ImageService.Instance.LoadCompiledResource(value)
                        .LoadingPlaceholder("ContactUsTel.png", FFImageLoading.Work.ImageSource.CompiledResource)
                     .WithCache(FFImageLoading.Cache.CacheType.Memory)
                    .Into(ImageView);
                    //ImageView.Image = UIImage.FromFile(value);
                }
                else
                {
                    ImageService.Instance.LoadCompiledResource(value)
                     .WithCache(FFImageLoading.Cache.CacheType.Memory)
                    .Into(ImageView);
                    //ImageView.Image = UIImage.FromBundle(value);
                }
            }
            else
            {
                ImageView.Image = null;
            }
        }
        internal static void SetImageItem(UIImageView ImageView, Item value)
        {
            if (value != null)
            {

                ImageView.Image = null;
                ImageService.Instance.LoadUrl(value.PicturePath)
                    .LoadingPlaceholder("", FFImageLoading.Work.ImageSource.CompiledResource)
                    .WithCache(FFImageLoading.Cache.CacheType.Disk)
                    .Into(ImageView); 
                
                
                    
                    //ImageView.Image = UIImage.FromBundle(value);
            }
            else
            {
                ImageView.Image = null;
            }
        }

        internal static void SetImageItemlocal(UIImageView ImageView, Item value)
        {
            if (value != null)
            {
                ImageView.Image = null;
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                 Path.Combine(docFolder, value.ThumbnailLocalPath);
                ImageService.Instance.LoadCompiledResource(Path.Combine(docFolder, value.ThumbnailLocalPath))
                    .LoadingPlaceholder("ContactUsTel.png", FFImageLoading.Work.ImageSource.CompiledResource)
                 .WithCache(FFImageLoading.Cache.CacheType.Memory)
                .Into(ImageView);
                
            }
            else
            {
                ImageView.Image = null;
            }
        }

        internal static void SetImageCus(UIImageView ImageView, Customer value)
        {
            if (value != null)
            {


                ImageService.Instance.LoadUrl(value.PicturePath)
                    .LoadingPlaceholder("defaultcust.png", FFImageLoading.Work.ImageSource.CompiledResource)
                    .WithCache(FFImageLoading.Cache.CacheType.Disk)
                    .Into(ImageView); 
                
                
                    
                    //ImageView.Image = UIImage.FromBundle(value);
            }
            else
            {
                ImageView.Image = null;
            }
        }

        internal static void SetImageCuslocal(UIImageView ImageView, Customer value)
        {
            if (value != null)
            {
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                 Path.Combine(docFolder, value.ThumbnailLocalPath);
                ImageService.Instance.LoadCompiledResource(Path.Combine(docFolder, value.ThumbnailLocalPath))
                    .LoadingPlaceholder("ContactUsTel.png", FFImageLoading.Work.ImageSource.CompiledResource)
                 .WithCache(FFImageLoading.Cache.CacheType.Memory)
                .Into(ImageView);
                
            }
            else
            {
                ImageView.Image = null;
            }
        }
        public static async Task ResentItem()
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                var lstitem = await itemManage.GetItemFwaiting();
                if (lstitem != null) { 
                    foreach (var item in lstitem)
                    {
                        JobQueue.Default.AddJobSendItem((int)item.MerchantID, (int)item.SysItemID);
                    }
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                ShowMessage(ex.Message);
                //Android.Util.Log.Debug("error", "ResentItem " + ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static async Task ResentCategory()
        {
            try
            {
                CategoryManage categoryManage = new CategoryManage();
                var lstcategory = await categoryManage.GetCategoryFwaiting();
                if (lstcategory != null)
                {
                    foreach (var item in lstcategory)
                    {
                        JobQueue.Default.AddJobSendCatagory((int)item.MerchantID, (int)item.SysCategoryID);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                ShowMessage(ex.Message);
                //Android.Util.Log.Debug("error", "ResentCategory " + ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public static async Task ResentTran()
        {
            try
            {
                TransManage transManage = new TransManage();
                var lsttrans = await transManage.GetTranFwaiting();
                if (lsttrans != null)
                {
                    foreach (var item in lsttrans)
                    {
                        JobQueue.Default.AddJobSendTrans((int)item.MerchantID, (int)item.SysBranchID, item.TranNo.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                ShowMessage(ex.Message);
                //Android.Util.Log.Debug("error", "ResentTran " + ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public static async Task ResentCustomer()
        {
            try
            {
                CustomerManage CustomerManage = new CustomerManage();
                var lstcustomer = await CustomerManage.GetCustomerFwaiting();
                if (lstcustomer != null)
                {
                    foreach (var item in lstcustomer)
                    {
                        System.Threading.Thread.Sleep(500);
                        JobQueue.Default.AddJobSendCustomer((int)item.MerchantID, (int)item.SysCustomerID);
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ResentCustomer at Utils");
                ShowMessage(ex.Message);
                //Android.Util.Log.Debug("error", "ResentCustomer " + ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static async Task ResentNoteCategory()
        {
            try
            {
                NoteCategoryManage NoteCategoryManage = new NoteCategoryManage();
                var lstnotecategory = await NoteCategoryManage.GetNoteCategoryFwaiting();
                if (lstnotecategory != null)
                {
                    foreach (var item in lstnotecategory)
                    {
                        System.Threading.Thread.Sleep(500);
                        JobQueue.Default.AddJobSendNoteCatagory((int)item.MerchantID, (int)item.SysNoteCategoryID);
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ResentNoteCategory at Utils");
                ShowMessage(ex.Message);
                //Android.Util.Log.Debug("error", "ResentNoteCategory " + ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static async Task ResentNote()
        {
            try
            {
                NoteManage NoteManage = new NoteManage();
                var lstnote = await NoteManage.GetNoteFwaiting();
                if (lstnote != null)
                {
                    foreach (var item in lstnote)
                    {
                        System.Threading.Thread.Sleep(500);
                        JobQueue.Default.AddJobSendNote((int)item.MerchantID, (int)item.SysNoteID);
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ResentNote at Utils");
                ShowMessage(ex.Message);
                //Android.Util.Log.Debug("error", "ResentNote " + ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static async Task ResentPrintCounter()
        {
            try
            {
                TransManage transManage = new TransManage();

                var lsttransPrinCounter = await transManage.GetTranFwaitingAndPrintCounter();
                if (lsttransPrinCounter != null)
                {
                    foreach (var tran in lsttransPrinCounter)
                    {
                        System.Threading.Thread.Sleep(500);
                        //ส่งค่าจาก offline -> online
                        UtilsAll.PostPrintCounter(DataCashingAll.SysBranchId, tran.TranNo, UtilsAll.ChangeDateTimeUS(tran.TranDate), (int)tran.PrintCounterLocal, tran);
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ResentNote at Utils");
                ShowMessage(ex.Message);
                //Android.Util.Log.Debug("error", "ResentNote " + ex.Message);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public static async void ItemChange()
        {
            try
            {
                Item getItem = new Item();
                ItemManage itemManage = new ItemManage();
                var systemRevisionNoManage = new SystemRevisionNoManage();
                var listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();

                for (int i = 0; i < listRivision.Count; i++)
                {
                    if (listRivision[i].SystemID == 30)
                    {
                        //Get Item API
                        //offset = index สำหรับเรียกข้อมูล ครั้งละ 100 ตัว เริ่มที่ 0
                        //total >= 100 item = 0 - 99     รอบที่ 1  offset = 0
                        //             item = 100 - 199  รอบที่ 2  offset = 1
                        //total > 100 totalitem/100 = จำนวนรอบที่เรียก 

                        var ItemResult = await GabanaAPI.GetDataItem((int)listRivision[i].LastRevisionNo, 0);
                        if (ItemResult==null)
                        {
                            return;
                        }
                        var allItem = ItemResult;

                        if (allItem.ItemsWithItemExSizes.Count == 0)
                        {
                            return;
                        }

                        //await GetandInsertItem(allItem);
                        var maxItem = allItem.ItemsWithItemExSizes.ToList().Max(x => x.ItemStatus.item.RevisionNo);// OrderByDescending(x => x.item.RevisionNo).First();                            
                        int maxItemRevision = maxItem;

                        //insert to Local DB
                        var SystemRevisionNo = new SystemRevisionNo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            SystemID = 30,
                            LastRevisionNo = maxItemRevision
                        };

                        var resultItem = await systemRevisionNoManage.UpdateSystemReviosion(SystemRevisionNo);

                        foreach (var item in allItem.ItemsWithItemExSizes)
                        {
                            if (item.ItemStatus.DataStatus == 'D')
                            {
                                //delete
                                var delete = await itemManage.DeleteItem(item.ItemStatus.item.MerchantID, item.ItemStatus.item.SysItemID);
                            }
                            else
                            {
                                //insertorreplace
                                getItem = new Item()
                                {
                                    MerchantID = item.ItemStatus.item.MerchantID,
                                    SysItemID = item.ItemStatus.item.SysItemID,
                                    ItemName = item.ItemStatus.item.ItemName,
                                    Ordinary = item.ItemStatus.item.Ordinary,
                                    SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                    ItemCode = item.ItemStatus.item.ItemCode,
                                    ShortName = item.ItemStatus.item.ShortName,
                                    PicturePath = item.ItemStatus.item.PicturePath,
                                    ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                    PictureLocalPath = item.ItemStatus.item.PicturePath,
                                    ThumbnailLocalPath = item.ItemStatus.item.ThumbnailPath,
                                    Colors = item.ItemStatus.item.Colors,
                                    FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                    UnitName = item.ItemStatus.item.UnitName,
                                    RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                    EstimateCost = item.ItemStatus.item.EstimateCost,
                                    Price = item.ItemStatus.item.Price,
                                    OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                    TaxType = item.ItemStatus.item.TaxType,
                                    SellBy = item.ItemStatus.item.SellBy,
                                    FTrackStock = item.ItemStatus.item.FTrackStock,
                                    TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                    SaleItemType = item.ItemStatus.item.SaleItemType,
                                    Comments = item.ItemStatus.item.Comments,
                                    LastDateModified = item.ItemStatus.item.LastDateModified,
                                    UserLastModified = item.ItemStatus.item.UserLastModified,
                                    DataStatus = 'I',
                                    FWaitSending = 1,
                                    WaitSendingTime = DateTime.UtcNow,
                                    LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                    LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,

                                };
                                var insertOrreplace = await itemManage.InsertOrReplaceItem(getItem);
                            }
                        }

                        //if (allItem.totalItems > 100)
                        //{
                        //    int roundgetItem = allItem.totalItems / 100;

                        //    ItemResult = await GabanaAPI.GetDataItem((int)listRiviion[i].LastRevisionNo, roundgetItem);
                        //    allItem = JsonConvert.DeserializeObject<Gabana3.JAM.Items.Items>(ItemResult.Message);

                        //    if (allItem.ItemsWithItemExSizes.Count == 0)
                        //    {
                        //        return;
                        //    }

                        //}     
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }

        }
        public static DateTime f_String_To_Date_AD(string stringDate)
        {
            if (string.IsNullOrEmpty(stringDate))
            {
                return DateTime.MinValue;
            }
            System.Globalization.CultureInfo _cultureEngInfo = new System.Globalization.CultureInfo("en-US");
            return DateTime.ParseExact(stringDate, "dd-MM-yyyy", _cultureEngInfo);
        }
        public static DateTime f_String_To_Date_AD_report(string stringDate)
        {
            if (string.IsNullOrEmpty(stringDate))
            {
                return DateTime.MinValue;
            }
            System.Globalization.CultureInfo _cultureEngInfo = new System.Globalization.CultureInfo("en-US");
            return DateTime.ParseExact(stringDate, "yyyyMMdd", _cultureEngInfo);
        }
        public static string f_Date_To_String_AD(DateTime? dateTime)
        {
            if (dateTime == DateTime.MinValue || dateTime == null)
            {
                return null;
            }
            else
            {
                System.Globalization.CultureInfo _cultureEngInfo = new System.Globalization.CultureInfo("en-US");
                return dateTime?.ToString("dd-MM-yyyy", _cultureEngInfo);
            }
        }
        public static string ShowDate(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            return datetime;
        }
        public static string ShowDateTime(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd/MM/yyyy HH:mm tt", new CultureInfo("en-US"));
            return datetime;
        }
        public static void CreateThumbnailLocalImage(int merchantID)
        {
            try
            {
                string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string libFolder = Path.Combine(docFolder, "..", "Library", merchantID.ToString(),"Thumbnail");
                string pathdb = Path.Combine(libFolder, DataCashingAll.sqliteMerchantDB);

                if (!Directory.Exists(pathdb))
                {
                    System.IO.Directory.CreateDirectory(pathdb);
                }
                DataCaching.PathFolderImage = pathdb;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
            }
        }



        public static bool InsertImageToThumbnail(string PathImage)
        {
            try
            {
                string pathFolder = DataCaching.PathFolderImage;
                var dirPath = pathFolder + PathImage;
                var exists = Directory.Exists(dirPath);
                if (!exists)
                {
                    if (!System.IO.File.Exists(dirPath))
                    {
                        using (var os = new System.IO.FileStream(dirPath, System.IO.FileMode.CreateNew))
                        {
                         //   bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 95, os);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //public static void streamImage(Android.Graphics.Bitmap bitmap)
        //{
        //    try
        //    {
        //        // เอา Bitmap มากเก็บเป็น BytArray เพื่อเตรียมส่งให้ Server
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, memoryStream);
        //            imageItemByteArray = memoryStream.ToArray();
        //            DataCashingAll.imageByteArray = imageItemByteArray;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}


        //Create Thumbnail Image
        public void GenerateThumbImage(string path)
        {
            try
            {
                // Load image.
                //System.Drawing.Image image = System.Drawing.Image.FromFile(path);

                string pathFolder = DataCaching.PathFolderImage;
                var image = pathFolder + path;


                byte[] imageBytes = System.Text.Encoding.Unicode.GetBytes(image);
                MemoryStream ms = new MemoryStream(imageBytes);

                System.Drawing.Image imagee = System.Drawing.Image.FromStream(ms, true, true);

                // Compute thumbnail size.
                System.Drawing.Size thumbnailSize = GetThumbnailSize(imagee);

                // Get thumbnail.
                System.Drawing.Image thumbnail = imagee.GetThumbnailImage(thumbnailSize.Width,
                    thumbnailSize.Height, null, IntPtr.Zero);

                // Save thumbnail.
                //var dcimFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
                //thumbnail.Save(dcimFolder.ToString());
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
            }
        }

        static System.Drawing.Size GetThumbnailSize(System.Drawing.Image original)
        {
            // Maximum size of any dimension.
            const int maxPixels = 40;

            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }

            // Return thumbnail size.
            return new System.Drawing.Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        public static int CheckImageinFolderThumbnail()
        {
            var folder = DataCaching.PathFolderImage;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filesList = Directory.GetFiles(folder);
            foreach (var file in filesList)
            {
                var filename = System.IO.Path.GetFileName(file);
            }
            return filesList.Count();
        }

        internal static void SetConstant(NSLayoutConstraint[] constraints , NSLayoutAttribute width, int v)
        {
             //(UILayoutConstraintAxis.Vertical);
            foreach (var x1 in constraints)
            {
                if (x1.FirstAttribute == width)
                {
                    x1.Constant = v;
                }
            }
        }

        internal static bool IsEmail(string input)
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
        public static async Task<Model.TranWithDetailsLocal> initialData()
        {
            string usernamelogin = Preferences.Get("User", "");
            Model.TranWithDetailsLocal tranWithDetails = new Model.TranWithDetailsLocal();
            var Vat = DataCashingAll.setmerchantConfig.TAXRATE;
            var VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
            DeviceTranRunningNoManage DeviceTranRunningNo = new DeviceTranRunningNoManage();
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var maxtranno = await DeviceTranRunningNo.GetLastDeviceTranRunningNo(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, DataCashingAll.DeviceNo);
            maxtranno++;

            tranWithDetails.tran = new ORM.MerchantDB.Tran();
            tranWithDetails.tran.MerchantID = DataCashingAll.MerchantId;
            tranWithDetails.tran.SysBranchID = DataCashingAll.SysBranchId;
            tranWithDetails.tran.TranNo = "#" + DataCashingAll.DeviceNo.ToString() + "-" + maxtranno.ToString("D6");
            tranWithDetails.tran.TranDate = DateTime.UtcNow;
            tranWithDetails.tran.Status = 10;
            tranWithDetails.tran.DeviceNo = DataCashingAll.DeviceNo;
            tranWithDetails.tran.SysCustomerID = 999;
            tranWithDetails.tran.CustomerName = "ลูกค้าทั่วไป";//
            tranWithDetails.tran.SellerName = usernamelogin;
            tranWithDetails.tran.LastDateModified = DateTime.UtcNow;
            tranWithDetails.tran.LastUserModified = usernamelogin;
            tranWithDetails.tran.FCancel = 0;
            tranWithDetails.tran.TranTaxType = 'I';
            tranWithDetails.tran.CountTradDisc = 0;
            tranWithDetails.tran.SubTotalNoneVat = 0;
            tranWithDetails.tran.TotalTradDiscNoneVat = 0;
            tranWithDetails.tran.TotalNoneVat = 0;
            tranWithDetails.tran.SubTotalHaveVat = 0;
            tranWithDetails.tran.TotalTradDiscHaveVat = 0;
            tranWithDetails.tran.TotalHaveVat = 0;
            tranWithDetails.tran.Total = 0;
            tranWithDetails.tran.ServiceCharge = 0;
            tranWithDetails.tran.TotalVat = 0;
            tranWithDetails.tran.GrandTotal = 0;
            tranWithDetails.tran.PaymentFractional = 0;
            tranWithDetails.tran.GrandPayment = 0;
            tranWithDetails.tran.SummaryPayment = 0;
            tranWithDetails.tran.Change = 0;
            tranWithDetails.tran.Tips = 0;
            tranWithDetails.tran.TotalPointEarning = 0;
            tranWithDetails.tran.PrintCounter = 0;
            tranWithDetails.tran.TaxRate = Convert.ToDecimal(Vat);
            tranWithDetails.tran.TranTaxType = char.Parse(VatType);

            //merchantDB
            tranWithDetails.tran.FWaitSending = 1;
            tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
            tranWithDetails.tran.Comments = null;
            tranWithDetails.tran.LocalDataStatus = 'I';


            //Order or Bill
            tranWithDetails.tran.TranType = 'B';
            tranWithDetails.tran.OrderName = null;
            tranWithDetails.tran.Status = 10;

            return tranWithDetails;
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
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return d.ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
            }
        }
        public static DateTime GetTranDate(DateTime checkDate)
        {
            try
            {
                var destinationTimeZone = TimeZoneInfo.Local;
                string datetime = TimeZoneInfo.ConvertTimeFromUtc(checkDate, destinationTimeZone).ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                var date = DateTime.ParseExact(datetime, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));

                if (checkDate.Year.ToString().Contains("14"))
                {
                    int year = checkDate.Year + 543;
                    string datetrans = checkDate.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                    string deleteyear = datetrans.Remove(6, 4);
                    string addyear = deleteyear.Insert(6, year.ToString());
                    var date1 = DateTime.ParseExact(addyear, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                    return date1;
                }

                if (checkDate.Year.ToString().Length == 3 & checkDate.Year.ToString().Contains("9"))
                {
                    int year = checkDate.Year + 1086;
                    string datetrans = checkDate.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                    string deleteyear = datetrans.Remove(6, 4);
                    string addyear = deleteyear.Insert(6, year.ToString());
                    var date2 = DateTime.ParseExact(addyear, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                    return date2;
                }

                return date;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
                return checkDate;
            }
        }
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
                return pathImage;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SplitPath at Utils");
                Console.WriteLine(ex.Message);
            }
        }
        internal static async Task InsertLocalPictureMerchant(Merchant merchant)
        {
            try
            {
                string Id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                var merchants = await GabanaAPI.GetMerchantDetail("APNS", Id);
                //insert to PictureLocalPath,ThumbnailLocalPath
                if (!string.IsNullOrEmpty(merchants.Merchant.LogoPath))
                {
                    string[] fullpartclound = ((merchants.Merchant.LogoPath == null || merchants.Merchant.LogoPath.ToString() == String.Empty) ? "" : merchants.Merchant.LogoPath.ToString()).Split("/");
                    var namepic1 = fullpartclound[fullpartclound.Length - 1];

                    string[] fullpartlocal = ((merchant.LogoLocalPath == null || merchant.LogoLocalPath.ToString() == String.Empty) ? "" : merchant.LogoLocalPath.ToString()).Split("/");
                    var namepic2 = fullpartlocal[fullpartlocal.Length - 1];

                    if (namepic1!= namepic2)
                    {
                        MerchantManage merchantManage = new MerchantManage();
                        var webClient = new System.Net.WebClient();
                        var image = FromUrl(merchants.Merchant.LogoPath);
                        if (image != null)
                        {
                            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                            if (merchant.LogoLocalPath != null)
                            {
                                if (Directory.Exists(Path.Combine(docFolder, merchant.LogoLocalPath)))
                                {
                                    Directory.Delete(Path.Combine(docFolder, merchant.LogoLocalPath));
                                }
                                merchant.LogoLocalPath = null;
                            }
                            

                                                      // save image 

                            var thumbnail = image.Scale(new CoreGraphics.CGSize(200, 200));
                            
                            var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture");
                            var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", namepic1);
                            var fullfilePath =  Path.Combine(docFolder, filePath);


                            if (!Directory.Exists(libFolder))
                            {
                                Directory.CreateDirectory(libFolder);
                            }
                            NSData data = image.AsPNG();
                            var _picture = new byte[data.Length];
                            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                            File.WriteAllBytes(fullfilePath, _picture);



                            merchant.LogoLocalPath = filePath;
                            merchant.LogoPath = merchants.Merchant.LogoPath;
                            var resultupdate = await merchantManage.UpdateMerchant(merchant);
                        }

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
        public static async Task InsertLocalPictureCustomer(Gabana.ORM.MerchantDB.Customer LocalCustomer)
        {
            try
            {
                //insert to PictureLocalPath,ThumbnailLocalPath
                if (!string.IsNullOrEmpty(LocalCustomer.PicturePath))
                {
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    CustomerManage customerManage = new CustomerManage();
                    var webClient = new System.Net.WebClient();
                    var image = FromUrl(LocalCustomer.PicturePath);
                    if (image != null)
                    {
                        if (!string.IsNullOrEmpty(LocalCustomer.PictureLocalPath))
                        {
                            if (Directory.Exists(Path.Combine(docFolder, LocalCustomer.PictureLocalPath)))
                            {
                                Directory.Delete(Path.Combine(docFolder, LocalCustomer.PictureLocalPath));
                            }
                            LocalCustomer.PictureLocalPath = null;
                        }

                        if (!string.IsNullOrEmpty(LocalCustomer.ThumbnailLocalPath))
                        {
                            if (Directory.Exists(Path.Combine(docFolder, LocalCustomer.ThumbnailLocalPath)))
                            {
                                Directory.Delete(Path.Combine(docFolder, LocalCustomer.ThumbnailLocalPath));
                            }
                            LocalCustomer.ThumbnailLocalPath = null;
                        }

                        // save image 

                        var thumbnail = image.Scale(new CoreGraphics.CGSize(200, 200));
                        //var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                        var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail");

                        string[] fullpart = LocalCustomer.PicturePath.Split("/");
                        var namepic = fullpart[fullpart.Length - 1];


                        var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail", namepic);
                        var fullfilePath = Path.Combine(docFolder, filePath);
                        if (!Directory.Exists(libFolder))
                        {
                            Directory.CreateDirectory(libFolder);
                        }
                        NSData data = thumbnail.AsPNG();
                        var _picture = new byte[data.Length];
                        System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                        File.WriteAllBytes(fullfilePath, _picture);

                        LocalCustomer.ThumbnailLocalPath = filePath;

                        var resultupdate = await customerManage.UpdateCustomer(LocalCustomer);
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

        public static async Task InsertLocalPictureItem(Gabana.ORM.MerchantDB.Item LocalItem)
        {
            try
            {
                //insert to PictureLocalPath,ThumbnailLocalPath
                if (!string.IsNullOrEmpty(LocalItem.PicturePath))
                {
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    ItemManage itemManage = new ItemManage();
                    var webClient = new System.Net.WebClient();
                    var image = FromUrl(LocalItem.PicturePath);
                    if (image != null)
                    {
                        if (!string.IsNullOrEmpty(LocalItem.PictureLocalPath))
                        {
                            if (Directory.Exists(Path.Combine(docFolder, LocalItem.PictureLocalPath)))
                            {
                                Directory.Delete(Path.Combine(docFolder, LocalItem.PictureLocalPath));
                            }
                            LocalItem.PictureLocalPath = null;
                        }

                        if (!string.IsNullOrEmpty(LocalItem.ThumbnailLocalPath))
                        {
                            if (Directory.Exists(Path.Combine(docFolder, LocalItem.ThumbnailLocalPath)))
                            {
                                Directory.Delete(Path.Combine(docFolder, LocalItem.ThumbnailLocalPath));
                            }
                            LocalItem.ThumbnailLocalPath = null;
                        }

                        // save image 

                        var thumbnail = image.Scale(new CoreGraphics.CGSize(200, 200));
                        //var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                        var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail");

                        string[] fullpart = LocalItem.PicturePath.Split("/");
                        var namepic = fullpart[fullpart.Length - 1];


                        var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail", namepic);
                        var fullfilePath = Path.Combine(docFolder, filePath);
                        if (!Directory.Exists(libFolder))
                        {
                            Directory.CreateDirectory(libFolder);
                        }
                        NSData data = thumbnail.AsPNG();
                        var _picture = new byte[data.Length];
                        System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                        File.WriteAllBytes(fullfilePath, _picture);

                        LocalItem.ThumbnailLocalPath = filePath;

                        var resultupdate = await itemManage.UpdateItem(LocalItem);
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

        

        internal static async Task InsertLocalPictureQrcode(ORM.MerchantDB.MyQrCode myQrCode)
        {
            try
            {
                //insert to PictureLocalPath,ThumbnailLocalPath
                if (!string.IsNullOrEmpty(myQrCode.PicturePath))
                {
                    string[] fullpartclound = ((myQrCode.PicturePath == null || myQrCode.PicturePath.ToString() == String.Empty) ? "" : myQrCode.PicturePath.ToString()).Split("/");
                    var namepic1 = fullpartclound[fullpartclound.Length - 1];

                    string[] fullpartlocal = ((myQrCode.PictureLocalPath == null || myQrCode.PictureLocalPath.ToString() == String.Empty) ? "" : myQrCode.PictureLocalPath.ToString()).Split("/");
                    var namepic2 = fullpartlocal[fullpartlocal.Length - 1];

                    if (namepic1 != namepic2)
                    {
                        MyQrCodeManage myQrCodeManage = new MyQrCodeManage();
                        var webClient = new System.Net.WebClient();
                        var image = FromUrl(myQrCode.PicturePath);
                        if (image != null)
                        {
                            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                            if (myQrCode.PictureLocalPath != null)
                            {
                                if (Directory.Exists(Path.Combine(docFolder, myQrCode.PictureLocalPath)))
                                {
                                    Directory.Delete(Path.Combine(docFolder, myQrCode.PictureLocalPath));
                                }
                                myQrCode.PictureLocalPath = null;
                            }


                            // save image 

                            var thumbnail = image.Scale(new CoreGraphics.CGSize(200, 200));

                            var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture");
                            var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", namepic1);
                            var fullfilePath = Path.Combine(docFolder, filePath);


                            if (!Directory.Exists(libFolder))
                            {
                                Directory.CreateDirectory(libFolder);
                            }
                            NSData data = image.AsPNG();
                            var _picture = new byte[data.Length];
                            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                            File.WriteAllBytes(fullfilePath, _picture);



                            myQrCode.PictureLocalPath = filePath;
                            //merchant.LogoPath = merchants.Merchant.LogoPath;
                            var resultupdate = await myQrCodeManage.UpdateMyQrCode(myQrCode);
                        }

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
        public static async void CheckImageLoaditemnotComplete()
        {
            try
            {
                //เช็ค ThumbnailPath กับ PicturePath ที่ข้างล่าง
                Console.WriteLine("CheckImageLoadnotComplete /time" + DateTime.Now);
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
                _ = TinyInsights.TrackPageViewAsync("CheckImageLoadnotComplete at Splash");
                Log.Error("connecterror", "CheckImageLoadnotComplete : " + ex.Message);
                throw;
            }
        }
        public static async void CheckImageLoadcustomernotComplete()
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
        public static async Task InsertPictureLocalItem(List<Item> lstItem)
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                int i = 0;
                foreach (var item in lstItem)
                {
                    
                    string thumnailPath = "";
                    if (!string.IsNullOrEmpty(item.PicturePath))
                    {
                        i++;
                        await Utils.InsertLocalPictureItemMaster(item);
                        Console.WriteLine("insertpic i= " + i + "/time" + DateTime.Now);
                    }
                    //Console.WriteLine("insertpic i= "+i+ "/time"+DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                throw;
            }
        }
        public static List<string> CheckStatusIteminCart(TranWithDetailsLocal tranWithDetails)
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
                List<Item> AllItem = new List<Item>();
                List<Item> AllItemStatusD = new List<Item>();
                List<string> lstSysItemId = new List<string>(); //tranDetailItem
                List<string> lstSysItemIdTopping = new List<string>(); //tranDetailToppings
                List<string> lstRemoveDummy = new List<string>();

                tranWithDetailsLocal = tranWithDetails;
                AllItem = POSController.AllItem;
                AllItemStatusD = POSController.AllItemStatusD;

                //เคสหาสินค้าที่ไม่มีที่เครื่อง

                lstRemoveDummy = tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => !AllItem.Select(y => y.SysItemID).ToList().Contains(x.tranDetailItem?.SysItemID == null ? 0 : (long)x.tranDetailItem?.SysItemID) && x.tranDetailItem?.SysItemID != 0 && !string.IsNullOrEmpty(x.tranDetailItem?.SysItemID?.ToString())).Select(m => m.tranDetailItem?.SysItemID.ToString()).ToList();

                lstSysItemId.AddRange(lstRemoveDummy);
                foreach (var item in tranWithDetailsLocal.tranDetailItemWithToppings)
                {
                    lstSysItemIdTopping.AddRange(item.tranDetailItemToppings.Where(x => !AllItem.Select(y => y.SysItemID).ToList().Contains(x.SysItemID == null ? 0 : (long)x.SysItemID)).Select(m => m.SysItemID.ToString()).ToList());
                }
                lstSysItemId.AddRange(lstSysItemIdTopping);

                //เคสหาสินค้าที่มีสถานะเป็น 'D' และ Tran.Payment == 0
                if (tranWithDetailsLocal.tranPayments.Count == 0)
                {
                    lstSysItemId.AddRange(tranWithDetailsLocal.tranDetailItemWithToppings.Where(x => AllItemStatusD.Select(y => y.SysItemID).ToList().Contains(x.tranDetailItem?.SysItemID == null ? 0 : (long)x.tranDetailItem?.SysItemID) && x.tranDetailItem?.SysItemID != 0).Select(m => m.tranDetailItem?.SysItemID.ToString()).ToList());
                    foreach (var item in tranWithDetailsLocal.tranDetailItemWithToppings)
                    {
                        lstSysItemIdTopping.AddRange(item.tranDetailItemToppings.Where(x => AllItemStatusD.Select(y => y.SysItemID).ToList().Contains(x.SysItemID == null ? 0 : (long)x.SysItemID)).Select(m => m.SysItemID.ToString()).ToList());
                    }
                    lstSysItemId.AddRange(lstSysItemIdTopping);
                }
                return lstSysItemId;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine("CheckStatusIteminCart : " + ex.Message);
                return new List<string>();
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
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine("CheckStatusIteminCart : " + ex.Message);
                HasChangeinCart hasChange = new HasChangeinCart() { tranWithDetailsLocal = tranWithDetails, FlagChange = false };
                return hasChange;
            }
        }
        public static async Task InsertPictureLocalCustomer(List<Customer> lstCustomer)
        {
            try
            {
                CustomerManage customerManage = new CustomerManage();
                foreach (var customer in lstCustomer)
                {
                    string thumnailPath = "";
                    if (!string.IsNullOrEmpty(customer.PicturePath))
                    {
                        await Utils.InsertLocalPictureCustomer(customer);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
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
                        await Utils.InsertLocalPictureQrcode(myQrCode);
                        
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task UpdateImageItem(List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem)
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                string thumnailLocalPath = string.Empty;
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
                                //var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                                //ava.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);
                                var image = FromUrl(data?.PicturePath);
                                if (Directory.Exists(Path.Combine(docFolder, data?.PictureLocalPath)))
                                {
                                    Directory.Delete(Path.Combine(docFolder, data?.PictureLocalPath));
                                }
                            }

                             await Utils.InsertLocalPictureItemMaster(data);
                            
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
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public static async Task UpdateImageCustomer(List<Gabana3.JAM.Customer.CustomerStatus> UpdateCustomer)
        {
            try
            {
                CustomerManage customerManage = new CustomerManage();
                string thumnailLocalPath = string.Empty;
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
                                //var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                                //ava.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);
                                var image = FromUrl(data?.PicturePath);
                                if (Directory.Exists(Path.Combine(docFolder, data?.PictureLocalPath)))
                                {
                                    Directory.Delete(Path.Combine(docFolder, data?.PictureLocalPath));
                                }
                            }

                            await Utils.InsertLocalPictureCustomer(data);

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
            catch (Exception ex)
            {
                Console.WriteLine("UpdateImagecustomer : " + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        public static async Task InsertLocalPictureItemMaster(Item LocalItem)
        {
            try
            {
                //insert to LogoLocalPath
                if (!string.IsNullOrEmpty(LocalItem.PicturePath))
                {
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    ItemManage itemManage = new ItemManage();
                    var webClient = new System.Net.WebClient();
                    var image = FromUrl(LocalItem.PicturePath);
                    if (image != null)
                    {
                        if (LocalItem.PictureLocalPath != null&& LocalItem.PictureLocalPath !="")
                        {
                            if (Directory.Exists(Path.Combine(docFolder, LocalItem.PictureLocalPath)))
                            {
                                Directory.Delete(Path.Combine(docFolder, LocalItem.PictureLocalPath));
                            }
                            LocalItem.PictureLocalPath = null;
                        }

                        if (LocalItem.ThumbnailLocalPath != null && LocalItem.ThumbnailLocalPath != "")
                        {
                            if (Directory.Exists(Path.Combine(docFolder, LocalItem.ThumbnailLocalPath)))
                            {
                                Directory.Delete(Path.Combine(docFolder, LocalItem.ThumbnailLocalPath));
                            }
                            LocalItem.ThumbnailLocalPath = null;
                        }

                        // save image 

                        var thumbnail = image.Scale(new CoreGraphics.CGSize(200, 200));
                        //var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                        var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail");

                        string[] fullpart = LocalItem.PicturePath.Split("/");
                        var namepic = fullpart[fullpart.Length - 1];


                        var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail", namepic);
                        var fullfilePath = Path.Combine(docFolder, filePath);
                        if (!Directory.Exists(libFolder))
                        {
                            Directory.CreateDirectory(libFolder);
                        }
                        NSData data = thumbnail.AsPNG();
                        var _picture = new byte[data.Length];
                        System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                        File.WriteAllBytes(fullfilePath, _picture);

                        LocalItem.ThumbnailLocalPath = filePath;

                        var resultupdate = await itemManage.UpdateItem(LocalItem);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertLocalPictureItemMaster at Utils");
                Console.WriteLine(ex.Message);
                
            }
        }


        public static async Task<bool> TestPrint(UIViewController thisview)
        {
            try
            {
                GabanaLoading.SharedInstance.Show(thisview);
                bool print = true;

                //new Tran Test Print
                #region tranWithDetails.tran
                TranWithDetailsLocal tranWithDetails = new TranWithDetailsLocal();
                tranWithDetails.tran.TranNo = "XXXXXXXXXXXXXXXX";
                tranWithDetails.tran.TranDate = Utils.GetTranDate(DateTime.UtcNow);
                tranWithDetails.tran.CustomerName = "Customer";
                tranWithDetails.tran.SellerName = "Cashier : Employee";
                tranWithDetails.tran.FCancel = 0;
                tranWithDetails.tran.CountTradDisc = 0;
                tranWithDetails.tran.SubTotalNoneVat = 0;
                tranWithDetails.tran.TotalTradDiscNoneVat = 0;
                tranWithDetails.tran.TotalNoneVat = 0;
                tranWithDetails.tran.SubTotalHaveVat = 0;
                tranWithDetails.tran.TotalTradDiscHaveVat = 0;
                tranWithDetails.tran.TotalHaveVat = 0;
                tranWithDetails.tran.Total = 300;
                tranWithDetails.tran.ServiceCharge = 0;
                tranWithDetails.tran.FmlServiceCharge = "5%";
                tranWithDetails.tran.TotalVat = 0;
                tranWithDetails.tran.GrandTotal = 300;
                tranWithDetails.tran.PaymentFractional = 0;
                tranWithDetails.tran.GrandPayment = 300;
                tranWithDetails.tran.SummaryPayment = 0;
                tranWithDetails.tran.Change = 0;
                tranWithDetails.tran.Tips = 0;
                tranWithDetails.tran.TotalPointEarning = 0;
                tranWithDetails.tran.PrintCounter = 0;
                tranWithDetails.tran.TaxRate = 7;
                tranWithDetails.tran.TranTaxType = 'I';
                #endregion

                #region TranTradDiscount
                ORM.MerchantDB.TranTradDiscount discount = new ORM.MerchantDB.TranTradDiscount()
                {
                    MerchantID = tranWithDetails.tran.MerchantID,
                    SysBranchID = tranWithDetails.tran.SysBranchID,
                    TranNo = tranWithDetails.tran.TranNo,
                    PriorityNo = 0,
                    FOnTop = 0,
                    DiscountType = "MD",
                    FmlDiscount = "5%",
                };
                tranWithDetails.tranTradDiscounts.Add(discount);
                ORM.MerchantDB.TranTradDiscount discountPS = new ORM.MerchantDB.TranTradDiscount()
                {
                    MerchantID = tranWithDetails.tran.MerchantID,
                    SysBranchID = tranWithDetails.tran.SysBranchID,
                    TranNo = tranWithDetails.tran.TranNo,
                    PriorityNo = 0,
                    FOnTop = 0,
                    DiscountType = "PS",
                    FmlDiscount = "5%",
                };
                tranWithDetails.tranTradDiscounts.Add(discountPS);
                #endregion

                #region TranPayment
                var tranPayment = new ORM.MerchantDB.TranPayment()
                {
                    PaymentNo = 1,
                    PaymentType = "CH",
                    PaymentAmount = (decimal)300, //เงินที่ต้องจ่าย
                };
                tranWithDetails.tranPayments.Add(tranPayment);
                #endregion

                #region TranDetailItemWithTopping
                List<TranDetailItemWithTopping> lsttranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
                TranDetailItemNew DetailItem = new TranDetailItemNew()
                {
                    TranNo = tranWithDetails.tran.TranNo,
                    ItemName = "Test",
                    SaleItemType = 'D',
                    Quantity = (decimal)1,
                    Price = 100,
                    ItemPrice = 0,
                    Discount = 0,
                    EstimateCost = 0,
                    SizeName = null,
                    Comments = null,
                };

                List<ORM.MerchantDB.TranDetailItemTopping> tranDetailItem = new List<ORM.MerchantDB.TranDetailItemTopping>();
                var tranDetailItemWithTopping = new TranDetailItemWithTopping()
                {
                    tranDetailItem = DetailItem,
                    tranDetailItemToppings = tranDetailItem,
                };

                lsttranDetailItemWithToppings.Add(tranDetailItemWithTopping);

                TranDetailItemNew DetailItem2 = new TranDetailItemNew()
                {
                    TranNo = tranWithDetails.tran.TranNo,
                    ItemName = "Test2",
                    SaleItemType = 'D',
                    Quantity = (decimal)1,
                    Price = 200,
                    ItemPrice = 0,
                    Discount = 0,
                    EstimateCost = 0,
                    SizeName = null,
                    Comments = null,
                };
                List<ORM.MerchantDB.TranDetailItemTopping> tranDetailItem2 = new List<ORM.MerchantDB.TranDetailItemTopping>();
                var tranDetailItemWithTopping2 = new TranDetailItemWithTopping()
                {
                    tranDetailItem = DetailItem2,
                    tranDetailItemToppings = tranDetailItem2,
                };
                lsttranDetailItemWithToppings.Add(tranDetailItemWithTopping2);

                tranWithDetails.tranDetailItemWithToppings.AddRange(lsttranDetailItemWithToppings);
                #endregion

                #region address
                var BranchDetail = new ORM.MerchantDB.Branch()
                {
                    Address = "2991/23-24 อาคารซีเนียร์ซอฟท์ โครงการวิสุทธานี ถนนลาดพร้าว แขวงคลองจั่น เขตบางกะปิ กรุงเทพมหานคร 10240",
                    ProvincesId = 1,
                    AmphuresId = 6,
                    DistrictsId = 100601,
                    Tel = "02-692-5899",
                };
                string branchaddress = "";
                if (BranchDetail != null)
                {
                    branchaddress = await Utils.SetTextAddressAsync(BranchDetail);
                }
                var address = Utils.SplitAddress(branchaddress); 
                #endregion

                var imageforprint = CreateImageIOS.DrawImageTestPrint(tranWithDetails, address);

                PrintController c = new PrintController();
                if (DataCashingAll.setting.TYPE == "Wifi")
                {
                    await c.PrintWifi(imageforprint);
                    Utils.ShowMessage("Print Success!");
                    GabanaLoading.SharedInstance.Hide();
                }
                else
                {
                    Plugin.BLE.Abstractions.ConnectParameters connectParameters = new Plugin.BLE.Abstractions.ConnectParameters(false, false);
                    var adapter = CrossBluetoothLE.Current.Adapter;
                    adapter.ScanTimeout = 1000;
                    adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.Balanced;
                    if (DataCashingAll.setting.BLUETOOTH1 is null)
                    {
                        Utils.ShowMessage("กรุณาตั้งค่าเครื่องปริ้น");
                        GabanaLoading.SharedInstance.Hide();
                        return false;
                    }

                    int timeout = 10000; // Longer than you'd expect!
                    bool status = false;
                    var task = adapter.ConnectToKnownDeviceAsync(Guid.Parse(DataCashingAll.setting.BLUETOOTH1), connectParameters);
                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        var dvc = task.Result;
                        GabanaLoading.SharedInstance.Hide();
                        status = true;
                    }
                    else
                    {
                        Utils.ShowMessage("ไม่สามารถเชื่อมต่อเครื่องปริ้นได้");
                        GabanaLoading.SharedInstance.Hide();
                        return false;
                    }
                    if (task.IsCompleted)
                    {
                        task.Dispose();
                    }

                    if (status)
                    {
                        var device = await adapter.ConnectToKnownDeviceAsync(Guid.Parse(DataCashingAll.setting.BLUETOOTH1), connectParameters);

                        var t = await device.RequestMtuAsync(500);
                        device.UpdateConnectionInterval(Plugin.BLE.Abstractions.ConnectionInterval.High);

                        var Services = await device.GetServicesAsync();

                        foreach (var ser in Services)
                        {
                            var Servicethis = await device.GetServiceAsync(ser.Id);
                            var characteristics = await Servicethis.GetCharacteristicsAsync();

                            foreach (var item in characteristics)
                            {
                                if (item.CanWrite && print)
                                {
                                    try
                                    {
                                        item.WriteType = Plugin.BLE.Abstractions.CharacteristicWriteType.WithoutResponse;

                                        var typemain = DataCashingAll.PrintType;

                                        if (typemain == "Text")
                                        {
                                            var typesub = "Window-874";
                                            var imageforprinttext = await CreateImageIOS.DrawStringTestPrint(tranWithDetails);
                                            List<byte> bytelist = new List<byte>();
                                            bytelist.Add((byte)27);
                                            bytelist.Add((byte)97);
                                            bytelist.Add((byte)49);

                                            bytelist.Add((byte)29);
                                            bytelist.Add((byte)33);
                                            bytelist.Add((byte)0);
                                            await item.WriteAsync(bytelist.ToArray());

                                            int size = 0;
                                            int[] itemLength = { 33, 48 };
                                            if (DataCashingAll.setting.TYPEPAGE != "58mm")
                                            {
                                                size = 1;
                                            }

                                            foreach (string txt in imageforprinttext)
                                            {
                                                var txt1 = txt;
                                                var x2 = ThaiLength(txt);

                                                switch (typesub)
                                                {
                                                    case "Window-874":
                                                        var enc = Encoding.GetEncoding("windows-874");
                                                        byte[] bytes = enc.GetBytes(txt1);
                                                        await item.WriteAsync(bytes);
                                                        break;
                                                    default:

                                                        await item.WriteAsync(System.Text.Encoding.UTF8.GetBytes(txt1));
                                                        var x1 = txt1.Length;
                                                        break;
                                                }
                                                Thread.Sleep(200);
                                                await item.WriteAsync(new byte[] { (byte)10 });
                                            }
                                        }
                                        else
                                        {
                                            await item.WriteAsync(imageforprint);
                                        }

                                        await item.WriteAsync(new byte[] { (byte)10 });
                                        await item.WriteAsync(new byte[] { (byte)10 });
                                        print = false;
                                        Utils.ShowMessage("Print Success !");
                                    }
                                    catch (Exception ex)
                                    {
                                        GabanaLoading.SharedInstance.Hide();
                                        //throw new Exception("ไม่สามารถเชื่อมต่อกับเครื่องปริ้นได้");
                                    }
                                }
                            }
                        }
                        GabanaLoading.SharedInstance.Hide();
                        //var imageforprinttext = CreateImageIOS.DrawString(tran);
                        //c.PrintBluetooth(imageforprint);
                        device.Dispose();
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                GabanaLoading.SharedInstance.Hide();
                throw ex;
                return false;
            }
        }

    }
}