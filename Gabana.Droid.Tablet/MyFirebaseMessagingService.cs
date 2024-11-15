
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Widget;
using BellNotificationHub.Xamarin.Android;
using Firebase.Messaging;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;

namespace Gabana.Droid.Tablet
{
    [Service (Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";

        static int notificationCount = 0;

        //--------------------------------------------------------------
        public static MyFirebaseMessagingService myFirebaseMessaging;

        int maxCategoryRevision = 0;
        int maxItemRevision = 0;
        int maxItemOnBranchRevision = 0;
        int maxCustomerRevision = 0;
        int maxNoteCategoryRevision = 0;
        int maxNoteRevision = 0;

        List<ORM.Master.MerchantConfig> listmerchantConfig = new List<ORM.Master.MerchantConfig>();
        List<ORM.Master.MemberType> listmembertype = new List<ORM.Master.MemberType>();
        Gabana3.JAM.Merchant.Merchants merchants = new Gabana3.JAM.Merchant.Merchants();
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
        List<ORM.Master.GiftVoucher> listgiftVouchers = new List<ORM.Master.GiftVoucher>();
        List<ORM.Master.Branch> getMerchant = new List<ORM.Master.Branch>();
        List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();

        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        CategoryManage categoryManage = new CategoryManage();
        Category category = new Category();
        ItemManage itemManage = new ItemManage();
        ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
        Item getItem = new Item();
        ItemExSize getitemSize = new ItemExSize();
        DeviceSystemSeqNo deviceSystemSeqNo = new DeviceSystemSeqNo();
        DeviceTranRunningNoManage deviceTranRunningNoManage = new DeviceTranRunningNoManage();
        DeviceTranRunningNo deviceTranRunning = new DeviceTranRunningNo();
        CustomerManage customerManage = new CustomerManage();
        Customer customer = new Customer();
        DiscountTemplateManage discountTemplateManage = new DiscountTemplateManage();
        DiscountTemplate discount = new DiscountTemplate();
        NoteManage noteManage = new NoteManage();
        Note note = new Note();
        NoteCategoryManage noteCategoryManage = new NoteCategoryManage();
        NoteCategory noteCategory = new NoteCategory();
        ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
        //--------------------------------------------------------------

        static Activity activity = BellNotificationHub.Xamarin.Android.Utils.GetCurrentActivity();

        public override void OnMessageReceived(RemoteMessage remoteMessage)
        {
            try
            {
                myFirebaseMessaging = this;
                notificationCount++;

                Log.Debug(TAG, "From: " + remoteMessage.From);

                if (!BellNotification.IsRegisted())
                {
                    return;     // ถ้าไม่มีการ Register ก็ให้ Return ออกไปเลย
                }

                if (activity == null)
                {
                    return;
                }

                string msg = remoteMessage.Data.ContainsKey("message") ? remoteMessage.Data["message"] : "";
                string codeno = remoteMessage.Data.ContainsKey("codeno") ? remoteMessage.Data["codeno"] : "";
                string action = remoteMessage.Data.ContainsKey("action") ? remoteMessage.Data["action"] : "";


                _ = TinyInsights.TrackPageViewAsync("OnMessageReceived : " + msg + ","+ codeno+ "," + action);
                Log.Debug("OnMessageReceived" ,"OnMessageReceived : " + msg + "," + codeno + "," + action);


                if (DataCashingAll.flagNotificationEnble)
                {
                    switch (action)
                    {
                        case Gabana3.DataModel.BellNotificationHub.Action.ITEM:
                            DataCashingAll.flagItemChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.CATEGORY:
                            DataCashingAll.flagCategoryChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.NOTE:
                            DataCashingAll.flagNoteChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.NOTECATEGORY:
                            DataCashingAll.flagNoteCategoryChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.CUSTOMER:
                            DataCashingAll.flagCustomerChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.MERCHANTCONFIG:
                            DataCashingAll.flagMerchantChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.ITEMONBRANCH:
                            DataCashingAll.flagItemOnBranchChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.MEMBERTYPE:
                            DataCashingAll.flagMemberTypeChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.MERCHANT:
                            DataCashingAll.flagMerchantChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.MYQRCODE:
                            DataCashingAll.flagMyQrCodeChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.GIFTVOUCHER:
                            DataCashingAll.flagGiftVoucherChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.BRANCH:
                            DataCashingAll.flagBranchChangeOnline = true;
                            break;
                        case Gabana3.DataModel.BellNotificationHub.Action.CASHTEMPLATE:
                            DataCashingAll.flagCashTemplateChangeOnline = true;
                            break;
                        default:
                            break;
                    }
                    return;
                }

                if (GabanaApplication.IsForground)
                {
                    activity.RunOnUiThread(() =>
                    {
                        switch (action)
                        {
                            case Gabana3.DataModel.BellNotificationHub.Action.ITEM:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.item_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ Item มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                ItemChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.CATEGORY:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.category_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ Category มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                CategoryChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.NOTE:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.note_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ Note มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                NoteChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.NOTECATEGORY:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.notecategory_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ NoteCategory มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                NoteCategoryChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.CUSTOMER:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.customer_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ Customer มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                CustomerChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.MERCHANTCONFIG:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.merchantconfig_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ MerchantConfig มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                MerchantConfigChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.ITEMONBRANCH:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.itemonbranch_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ ItemOnBranch มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                ItemOnBranchChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.MEMBERTYPE:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.membertype_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ MemberType มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                MemberTypeChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.MERCHANT:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.merchant_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ Merchant มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                MerchantChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.MYQRCODE:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.myqrcode_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ MyQrCode มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                MyQrCodeChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.GIFTVOUCHER:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.giftvoucher_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ GiftVoucher มีการเปลี่ยนเปลง
                                //---------------------------------------------------------
                                GiftVoucherChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.BRANCH:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.branch_noti), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ Branch มีการเปลี่ยนเปลง
                                //---------------------------------------------------------     
                                BranchChange();
                                break;
                            case Gabana3.DataModel.BellNotificationHub.Action.CASHTEMPLATE:
                                Toast.MakeText(this, Application.Context.GetString(Resource.String.cashtemplate), ToastLength.Short).Show();
                                //---------------------------------------------------------
                                //Load ข้อมูลเมื่อ Branch มีการเปลี่ยนเปลง
                                //---------------------------------------------------------     
                                CashTemplateChange();
                                break;
                            default:
                                Toast.MakeText(this, msg, ToastLength.Short).Show();
                                break;
                                #region Old Code
                                //case "newsfeedpublic":
                                //    Utils.ModifyNewsfeed = false;
                                //    Utils.ModifyNewsfeedPublic = false;
                                //    Toast.MakeText(this, msg, ToastLength.Short).Show();
                                //    break;
                                //case "acceptmember":
                                //    Utils.ModifyMembercards = false;
                                //    Utils.ModifyMerchants = false;
                                //    Utils.ModifyNewsfeed = false;
                                //    Utils.ModifyNewsfeedPublic = false;
                                //    Toast.MakeText(this, msg, ToastLength.Short).Show();
                                //    break;
                                //case "usedmycoupon":
                                //    Utils.ModifyMycoupon = false;
                                //    Toast.MakeText(this, msg, ToastLength.Short).Show();
                                //    break;
                                //case "voidearning":
                                //    Utils.ModifyMembercards = false;
                                //    Utils.ModifyNewsfeed = false;
                                //    Utils.ModifyNewsfeedPublic = false;
                                //    Toast.MakeText(this, msg, ToastLength.Short).Show();
                                //    break;
                                //case "earning":
                                //    Utils.ModifyMembercards = false;
                                //    Utils.ModifyNewsfeed = false;
                                //    Utils.ModifyNewsfeedPublic = false;
                                //    Toast.MakeText(this, msg, ToastLength.Short).Show();
                                //    break;
                                //default:
                                //Utils.ModifyMembercards = false;
                                //Utils.ModifyMerchants = false;
                                //Utils.ModifyMycoupon = false;
                                //Utils.ModifyNewsfeed = false;
                                //Utils.ModifyNewsfeedPublic = false;
                                //Utils.ModifyProfileGiftory = false;
                                //Utils.ModifyRedeem = false;
                                //Toast.MakeText(this, msg, ToastLength.Short).Show();
                                //break; 
                                #endregion
                        }
                    });
                }
                else
                {
                    _sendNotification(msg, codeno, action);
                }
            }
            catch (Exception ex)
            {
                activity.RunOnUiThread(async () => { Toast.MakeText(this, "OnMessageReceived" + ":" + ex.Message, ToastLength.Short).Show(); });
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnMessageReceived");
            }
        }

        public async static void _ReloadNoticication()
        {
            try
            {
                if (activity == null)
                {
                    return;
                }

                if (GabanaApplication.IsForground)
                {
                    activity.RunOnUiThread(async () =>
                    {
                        if (DataCashingAll.flagItemChangeOnline)
                        {
                            myFirebaseMessaging.ItemChange();
                            DataCashingAll.flagItemChangeOnline = false;
                        }
                        if (DataCashingAll.flagCategoryChangeOnline)
                        {
                            myFirebaseMessaging.CategoryChange();
                            DataCashingAll.flagCategoryChangeOnline = false;
                        }
                        if (DataCashingAll.flagNoteChangeOnline)
                        {
                            myFirebaseMessaging.NoteChange();
                            DataCashingAll.flagNoteChangeOnline = false;
                        }
                        if (DataCashingAll.flagNoteCategoryChangeOnline)
                        {
                            myFirebaseMessaging.NoteCategoryChange();
                            DataCashingAll.flagNoteCategoryChangeOnline = false;
                        }
                        if (DataCashingAll.flagCustomerChangeOnline)
                        {
                            myFirebaseMessaging.CustomerChange();
                            DataCashingAll.flagCustomerChangeOnline = false;
                        }
                        if (DataCashingAll.flagMerchantConfigChangeOnline)
                        {
                            myFirebaseMessaging.MerchantConfigChange();
                            DataCashingAll.flagMerchantConfigChangeOnline = false;
                        }
                        if (DataCashingAll.flagItemOnBranchChangeOnline)
                        {
                            myFirebaseMessaging.ItemOnBranchChange();
                            DataCashingAll.flagItemOnBranchChangeOnline = false;
                        }
                        if (DataCashingAll.flagMemberTypeChangeOnline)
                        {
                            myFirebaseMessaging.MemberTypeChange();
                            DataCashingAll.flagMemberTypeChangeOnline = false;
                        }
                        if (DataCashingAll.flagMerchantChangeOnline)
                        {
                            myFirebaseMessaging.MerchantChange();
                            DataCashingAll.flagMerchantChangeOnline = false;
                        }
                        if (DataCashingAll.flagMyQrCodeChangeOnline)
                        {
                            myFirebaseMessaging.MyQrCodeChange();
                            DataCashingAll.flagMyQrCodeChangeOnline = false;
                        }
                        if (DataCashingAll.flagGiftVoucherChangeOnline)
                        {
                            myFirebaseMessaging.GiftVoucherChange();
                            DataCashingAll.flagGiftVoucherChangeOnline = false;
                        }
                        if (DataCashingAll.flagBranchChangeOnline)
                        {
                            myFirebaseMessaging.BranchChange();
                            DataCashingAll.flagBranchChangeOnline = false;
                        }
                        if (DataCashingAll.flagCashTemplateChangeOnline)
                        {
                            myFirebaseMessaging.CashTemplateChange();
                            DataCashingAll.flagCashTemplateChangeOnline = false;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                activity.RunOnUiThread(async () => { Toast.MakeText(myFirebaseMessaging, "_ReloadNoticication" + ":" + ex.Message, ToastLength.Short).Show(); });
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("_ReloadNoticication");
            }
        }

        private void _sendNotification(string messageBody, string codeno, string action)
        {
            switch (action)
            {
                case Gabana3.DataModel.BellNotificationHub.Action.ITEM:
                    messageBody = Application.Context.GetString(Resource.String.item_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.CATEGORY:
                    messageBody = Application.Context.GetString(Resource.String.category_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.NOTE:
                    messageBody = Application.Context.GetString(Resource.String.note_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.NOTECATEGORY:
                    messageBody = Application.Context.GetString(Resource.String.notecategory_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.CUSTOMER:
                    messageBody = Application.Context.GetString(Resource.String.customer_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.MERCHANTCONFIG:
                    messageBody = Application.Context.GetString(Resource.String.merchantconfig_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.ITEMONBRANCH:
                    messageBody = Application.Context.GetString(Resource.String.itemonbranch_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.MEMBERTYPE:
                    messageBody = Application.Context.GetString(Resource.String.membertype_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.MERCHANT:
                    messageBody = Application.Context.GetString(Resource.String.merchant_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.MYQRCODE:
                    messageBody = Application.Context.GetString(Resource.String.myqrcode_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.GIFTVOUCHER:
                    messageBody = Application.Context.GetString(Resource.String.giftvoucher_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.BRANCH:
                    messageBody = Application.Context.GetString(Resource.String.branch_noti);
                    break;
                case Gabana3.DataModel.BellNotificationHub.Action.CASHTEMPLATE:
                    messageBody = Application.Context.GetString(Resource.String.cashtemplate);
                    break;
                default:
                    break;
            }

            var intent = new Intent(this, typeof(SplashActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            //intent.PutExtra("actioncode", action + ":" + codeno);
            intent.PutExtra("action", action);
            intent.PutExtra("codeno", codeno);
            PendingIntent pendingIntent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                pendingIntent = PendingIntent.GetActivity
                       (this, 0, intent, PendingIntentFlags.Immutable);
            }
            else
            {
                pendingIntent = PendingIntent.GetActivity
                       (this, 0, intent, PendingIntentFlags.OneShot);
            }


            var notificationBuilder = new NotificationCompat.Builder(this, SplashActivity.CHANNEL_ID);
            notificationBuilder.SetContentTitle("Gabana")
                        //.SetSmallIcon(Resource.Drawable.ic_launcher)
                        .SetSmallIcon(Resource.Drawable.abc_ratingbar_small_material)
                        .SetContentText(messageBody)
                        .SetAutoCancel(true)
                        .SetShowWhen(true)
                        .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                        .SetVibrate(new long[] { 0, 500, 1000 })
                        .SetContentIntent(pendingIntent)
                        .SetNumber(notificationCount)  // badge : Note การแสดง budges ถ้าตำว่า 8 ไม่สามารถแสดงได้ ต้องจัดการเอง
                        .SetBadgeIconType(NotificationCompat.BadgeIconSmall)
                        ;

            var notificationManager = NotificationManager.FromContext(this);

            notificationManager.Notify(0, notificationBuilder.Build());

            // การแสดง budges ถ้าตำว่า 8 ไม่สามารถแสดงได้ ต้องจัดการเอง
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                if (XamarinShortcutBadger.ShortcutBadger.IsBadgeCounterSupported(this))
                {
                    XamarinShortcutBadger.ShortcutBadger.ApplyCount(this, notificationCount);
                }
            }
            else
            {
                XamarinShortcutBadger.ShortcutBadger.ApplyCount(this, notificationCount);
            }
        }

        public override void OnNewToken(string deviceToken)
        {
            Log.Debug(TAG, "FCM notificationToken: " + deviceToken);
            Log.Debug("Delay Onload", "2-OnNewToken");
            BellNotification.KeepDeviceToken(deviceToken);
            Log.Debug("TAGSPLASH3", $"GetDeviceToken" + deviceToken);
            _ = TinyInsights.TrackEventAsync($"deviceToken : {deviceToken}", null);
        }

        public async Task CategoryChange()
        {
            try
            {
                int SysCategoryIdFocus = 0;
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 20).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region Category
                    try
                    {
                        //Get Category API
                        var allcategory = await GabanaAPI.GetDataCategory((int)revisionNo.LastRevisionNo);

                        if (allcategory == null)
                        {
                            return;
                        }

                        if (allcategory.Categories.Count == 0 && allcategory.CategoryBins.Count == 0)
                        {
                            return;
                        }

                        int maxCategory = 0;
                        if (allcategory.Categories.Count > 0)
                        {
                            allcategory.Categories.ToList().OrderBy(x => x.RevisionNo);
                            maxCategory = allcategory.Categories.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                            SysCategoryIdFocus = allcategory.Categories.OrderByDescending(x => x.RevisionNo).FirstOrDefault().SysCategoryID;

                            //check ว่ามีไหม
                            List<Category> GetallCate = await categoryManage.GetAllCategory();
                            List<ORM.Master.Category> UpdateCategory = new List<ORM.Master.Category>();
                            List<ORM.Master.Category> InsertCategory = new List<ORM.Master.Category>();
                            UpdateCategory.AddRange(allcategory.Categories.Where(x => GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID)).ToList());
                            InsertCategory.AddRange(allcategory.Categories.Where(x => !(GetallCate.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID))).ToList());

                            //Insert Category
                            if (InsertCategory.Count > 0)
                            {
                                List<Category> BulkCategory = new List<Category>();
                                foreach (var category in InsertCategory)
                                {
                                    BulkCategory.Add(new Category()
                                    {
                                        MerchantID = category.MerchantID,
                                        SysCategoryID = category.SysCategoryID,
                                        Ordinary = category.Ordinary,
                                        Name = category.Name,
                                        DateCreated = category.DateCreated,
                                        DateModified = category.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxID = category.LinkProMaxxID
                                    });
                                    maxCategoryRevision = category.RevisionNo;
                                }

                                using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                {
                                    try
                                    {
                                        await db.BulkCopyAsync(BulkCategory);
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorRevison = InsertCategory.Select(x => x.RevisionNo).Min();
                                        maxCategory = errorRevison;
                                        Log.Error("connecterror", "BulkCategory :" + ex.Message);
                                        throw ex;
                                    }
                                }
                            }

                            //Update Category
                            if (UpdateCategory.Count > 0)
                            {
                                foreach (var item in UpdateCategory)
                                {
                                    //insertorreplace
                                    category = new Category()
                                    {
                                        MerchantID = item.MerchantID,
                                        SysCategoryID = item.SysCategoryID,
                                        Ordinary = item.Ordinary,
                                        Name = item.Name,
                                        DateCreated = item.DateCreated,
                                        DateModified = item.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxID = item.LinkProMaxxID
                                    };
                                    var insertOrreplace = await categoryManage.InsertOrReplaceCategory(category);
                                    maxCategoryRevision = item.RevisionNo;
                                }
                            }
                        }

                        if (allcategory.CategoryBins.Count > 0)
                        {
                            allcategory.CategoryBins.ToList().OrderBy(x => x.RevisionNo);
                            maxCategory = allcategory.CategoryBins.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                            foreach (var item in allcategory.CategoryBins)
                            {
                                //UpdateItem
                                List<Item> UpdateItemCate = new List<Item>();
                                UpdateItemCate = await itemManage.GetItembyCategory(item.MerchantID, item.SysCategoryID);
                                if (UpdateItemCate != null)
                                {
                                    foreach (var update in UpdateItemCate)
                                    {
                                        update.SysCategoryID = null;
                                        var resultUpdate = await itemManage.UpdateItem(update);
                                    }
                                }
                                //delete
                                var delete = await categoryManage.DeleteCategory(item.MerchantID, item.SysCategoryID);
                                if (!delete)
                                {
                                    var data = await categoryManage.GetCategory(item.MerchantID, item.SysCategoryID);
                                    if (data != null)
                                    {
                                        data.DataStatus = 'D';
                                        data.FWaitSending = 0;
                                        await categoryManage.UpdateCategory(category);
                                    }
                                }
                                maxCategoryRevision = item.RevisionNo;
                            }
                        }

                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxCategory);
                        Log.Debug("connectpass", "listRivisionCategory");

                        //Set flag สำหรับ Reload ข้อมูลที่เครื่อง
                        DataCashingAll.flagCategoryChange = true;
                        //if (ItemActivity.itemActivity != null)
                        //{
                        //    ItemActivity.SetFocusItem(SysCategoryIdFocus);
                        //    ItemActivity.itemActivity.Resume();
                        //}
                        //if (PosActivity.pos != null)
                        //{
                        //    PosActivity.pos.Resume();
                        //}
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionCategory : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxCategoryRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CategoryChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task ItemChange()
        {
            try
            {
                int SysItemIdFocus = 0;
                List<Item> lstInsertItemImage = new List<Item>();
                List<Item> lstUpdateItemImage = new List<Item>();
                List<Item> GetAllitem = new List<Item>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                List<Gabana3.JAM.Items.ItemWithItemExSizes> InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();

                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 30).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region Item                    
                    try
                    {
                        var allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, 0);

                        if (allItem == null)
                        {
                            return;
                        }
                        else if (allItem?.ItemsWithItemExSizes.Count == 0)
                        {
                            return;
                        }
                        else
                        {
                            double round = 0, addrount = 0;
                            round = allItem.totalItems / 100;
                            addrount = round + 1;
                            double percentage = 0, temp = 0;
                            percentage = (25 / addrount);
                            temp = percentage;
                            percentage = 0;

                            for (int j = 0; j < addrount; j++)
                            {
                                allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, j);

                                if (allItem == null)
                                {
                                    break;
                                }

                                if (allItem.totalItems == 0)
                                {
                                    break;
                                }

                                allItem.ItemsWithItemExSizes.ToList().OrderBy(x => x.ItemStatus.item.RevisionNo);
                                var maxItem = allItem.ItemsWithItemExSizes.ToList().Max(x => x.ItemStatus.item.RevisionNo);// OrderByDescending(x => x.item.RevisionNo).First();                            

                                GetAllitem = await itemManage.GetAllItemType();
                                UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                                InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();

                                UpdateItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)).ToList());
                                InsertItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => !(GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)) && x.ItemStatus.DataStatus != 'D').ToList());

                                List<Item> Bulkitem = new List<Item>();
                                List<ItemExSize> BulkitemExsize = new List<ItemExSize>();

                                //Insert Item
                                if (InsertItem.Count > 0)
                                {
                                    foreach (var item in InsertItem)
                                    {
                                        string thumnailPath = string.Empty;
                                        Bulkitem.Add(new Item()
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
                                            PictureLocalPath = "",
                                            ThumbnailLocalPath = thumnailPath,
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
                                            DataStatus = item.ItemStatus.DataStatus,
                                            FWaitSending = 0,
                                            WaitSendingTime = DateTime.UtcNow,
                                            LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                            LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                            FDisplayOption = item.ItemStatus.item.FDisplayOption
                                        });

                                        var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                        itemSizes.ForEach(itemSize => BulkitemExsize.Add(new ItemExSize()
                                        {
                                            MerchantID = itemSize.MerchantID,
                                            SysItemID = itemSize.SysItemID,
                                            EstimateCost = itemSize.EstimateCost,
                                            ExSizeName = itemSize.ExSizeName,
                                            ExSizeNo = itemSize.ExSizeNo,
                                            Price = itemSize.Price,
                                            Comments = itemSize.Comments
                                        }));
                                    }

                                    using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                    {
                                        await db.BeginTransactionAsync();
                                        try
                                        {
                                            await db.BulkCopyAsync(Bulkitem);
                                            await db.BulkCopyAsync(BulkitemExsize);
                                            await db.CommitTransactionAsync();
                                        }
                                        catch (Exception ex)
                                        {
                                            await db.RollbackTransactionAsync();
                                            //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                            var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                            maxItemRevision = errorRevison;
                                            Utils.DeletePictureItemMaster(Bulkitem.Select(x => x.ThumbnailLocalPath).ToList());
                                            Log.Error("connecterror", "Bulkitem,BulkitemExsize : " + ex.Message);
                                            throw ex;
                                        }
                                    }
                                    lstInsertItemImage.AddRange(Bulkitem);
                                }

                                //Update Item
                                if (UpdateItem.Count > 0)
                                {
                                    UpdateItem.ForEach(async item =>
                                    {
                                        char itemStatus = item.ItemStatus.DataStatus;
                                        List<ORM.Master.ItemOnBranch> itemOnBranch = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemOnBranchs).FirstOrDefault() ?? new List<ORM.Master.ItemOnBranch>();
                                        var data = await itemManage.GetItem((int)item.ItemStatus.item.MerchantID, (int)item.ItemStatus.item.SysItemID);

                                        if (itemStatus == 'D')
                                        {
                                            await Utils.DeleteItem(data, itemOnBranch);
                                        }
                                        else
                                        {
                                            string thumnailLocalPath = string.Empty;
                                            Item updateItem = new Item()
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
                                                PictureLocalPath = "",
                                                ThumbnailLocalPath = thumnailLocalPath,
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
                                                DataStatus = item.ItemStatus.DataStatus,
                                                FWaitSending = 0,
                                                WaitSendingTime = DateTime.UtcNow,
                                                LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                                LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                                FDisplayOption = item.ItemStatus.item.FDisplayOption
                                            };

                                            var insertOrreplace = await itemManage.UpdateItem(updateItem);

                                            var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                            itemSizes.ForEach(async itemSize =>
                                            {
                                                await itemExSizeManage.InsertOrReplaceItemSize(new ItemExSize()
                                                {
                                                    MerchantID = itemSize.MerchantID,
                                                    SysItemID = itemSize.SysItemID,
                                                    EstimateCost = itemSize.EstimateCost,
                                                    ExSizeName = itemSize.ExSizeName,
                                                    ExSizeNo = itemSize.ExSizeNo,
                                                    Price = itemSize.Price,
                                                    Comments = itemSize.Comments
                                                });
                                            });
                                        }
                                        maxItemRevision = item.ItemStatus.item.RevisionNo;
                                    });
                                }
                                await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxItem);
                            }
                            DataCashingAll.flagItemChange = true;
                            //insert Image to Local เมื่อเพิ่มข้อมูลทั้งหมดสำเร็จ ทั้งเคสเพิ่มและเคสอัปเดต
                            Log.Debug("connectpass", "InsertPictureLocal(lstItemImage)" + "lstInsertItemImage " + lstInsertItemImage.Count);
                            Task.Factory.StartNew(() => Utils.InsertPictureLocalItem(lstInsertItemImage));
                            //Function Check Update Image 
                            Log.Debug("connectpass", "UpdateImageItem(UpdateItem)" + "UpdateItem " + UpdateItem.Count);
                            Task.Factory.StartNew(() => Utils.UpdateImageItem(UpdateItem));
                            Log.Debug("connectpass", "listRivisionItem");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task ItemOnBranchChange()
        {
            try
            {
                int SysItemIdFocus = 0;
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 31).FirstOrDefault();

                if (revisionNo != null)
                {
                    #region ItemOnBranch                            
                    #region OldCode
                    //try
                    //{
                    //    var allItemOnBranch = await GabanaAPI.GetDataItemOnBranch((int)listRivision[i].LastRevisionNo, 0);

                    //    if (allItemOnBranch == null)
                    //    {
                    //        break;
                    //    }

                    //    if (allItemOnBranch.Count > 0)
                    //    {
                    //        int round = allItemOnBranch.Count / 100;
                    //        int addrount = round + 1;
                    //        for (int j = 0; j < addrount; j++)
                    //        {
                    //            allItemOnBranch = await GabanaAPI.GetDataItemOnBranch((int)listRivision[i].LastRevisionNo, j);

                    //            if (allItemOnBranch == null)
                    //            {
                    //                break;
                    //            }

                    //            if (allItemOnBranch.Count == 0)
                    //            {
                    //                break;
                    //            }

                    //            allItemOnBranch.OrderBy(x => x.RevisionNo);
                    //            var maxItemOnBranch = allItemOnBranch.Max(x => x.RevisionNo);

                    //            foreach (var item in allItemOnBranch)
                    //            {
                    //                ItemOnBranch stock = new ItemOnBranch()
                    //                {
                    //                    MerchantID = item.MerchantID,
                    //                    SysBranchID = item.SysBranchID,
                    //                    SysItemID = item.SysItemID,
                    //                    BalanceStock = item.BalanceStock,
                    //                    MinimumStock = item.MinimumStock,
                    //                    LastDateBalanceStock = item.LastDateBalanceStock,
                    //                };
                    //                var insertStock = await onBranchManage.InsertorReplaceItemOnBranch(stock);
                    //                maxItemRevision = item.RevisionNo;
                    //                await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                    //            }
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //    await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                    //} 
                    #endregion

                    try
                    {
                        var allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, 0);

                        if (allItemOnBranch == null)
                        {
                            return;
                        }

                        if (allItemOnBranch.totalItemOnBranch == 0)
                        {
                            return;
                        }

                        if (allItemOnBranch.totalItemOnBranch > 0)
                        {
                            int round = 0, addrount = 0;
                            round = allItemOnBranch.totalItemOnBranch / 100;
                            addrount = round + 1;

                            for (int j = 0; j < addrount; j++)
                            {
                                allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, j);

                                if (allItemOnBranch == null)
                                {
                                    break;
                                }

                                if (allItemOnBranch.totalItemOnBranch == 0)
                                {
                                    break;
                                }

                                allItemOnBranch.ItemOnBranches.OrderBy(x => x.RevisionNo);
                                var maxItemOnBranch = allItemOnBranch.ItemOnBranches.Max(x => x.RevisionNo);
                                SysItemIdFocus = allItemOnBranch.ItemOnBranches.OrderByDescending(x => x.RevisionNo).FirstOrDefault().SysItemID;

                                //check ว่ามีไหม
                                List<ORM.Master.ItemOnBranch> UpdateItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                                List<ORM.Master.ItemOnBranch> InsertItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                                List<ItemOnBranch> GetallItemonBranch = await onBranchManage.GetAllItemOnBranchOnlyMerchantID(DataCashingAll.MerchantId);
                                UpdateItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID)).ToList());
                                InsertItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => !(GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID))).ToList());


                                //Insert ItemonBranch
                                if (InsertItemOnBranch.Count > 0)
                                {
                                    List<ItemOnBranch> BulkItemOnBranch = new List<ItemOnBranch>();

                                    foreach (var itemOnBranch in InsertItemOnBranch)
                                    {
                                        BulkItemOnBranch.Add(new ItemOnBranch()
                                        {
                                            MerchantID = itemOnBranch.MerchantID,
                                            SysBranchID = itemOnBranch.SysBranchID,
                                            SysItemID = itemOnBranch.SysItemID,
                                            BalanceStock = itemOnBranch.BalanceStock,
                                            MinimumStock = itemOnBranch.MinimumStock,
                                            LastDateBalanceStock = itemOnBranch.LastDateBalanceStock,
                                        });
                                    }

                                    using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                    {
                                        try
                                        {
                                            await db.BulkCopyAsync(BulkItemOnBranch);
                                        }
                                        catch (Exception ex)
                                        {
                                            var errorRevison = InsertItemOnBranch.Select(x => x.RevisionNo).Min();
                                            maxItemOnBranchRevision = errorRevison;
                                            Log.Error("connecterror", "BulkItemOnBranch :" + ex.Message);
                                            throw ex;
                                        }
                                    }
                                }

                                //Update ItemonBranch
                                if (UpdateItemOnBranch.Count > 0)
                                {
                                    foreach (var item in UpdateItemOnBranch)
                                    {
                                        ItemOnBranch stock = new ItemOnBranch()
                                        {
                                            MerchantID = item.MerchantID,
                                            SysBranchID = item.SysBranchID,
                                            SysItemID = item.SysItemID,
                                            BalanceStock = item.BalanceStock,
                                            MinimumStock = item.MinimumStock,
                                            LastDateBalanceStock = item.LastDateBalanceStock,
                                        };
                                        var insertStock = await onBranchManage.InsertorReplaceItemOnBranch(stock);
                                        maxItemOnBranchRevision = item.RevisionNo;
                                    }
                                }

                                await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxItemOnBranch);

                                DataCashingAll.flagItemOnBranchChange = true;
                                //if (ItemActivity.itemActivity != null)
                                //{
                                //    ItemActivity.SetFocusItem(SysItemIdFocus);
                                //    ItemActivity.itemActivity.Resume();
                                //}
                            }
                            Log.Debug("connectpass", "listRivisionItemOnBranch");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionItemOnBranch : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemOnBranchRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemOnVBranchChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task CustomerChange()
        {
            try
            {
                int SysCustomerIdFocus = 0;
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 50).FirstOrDefault();
                #region Customer
                try
                {
                    //Get Customer API
                    var allcustomer = await GabanaAPI.GetDataCustomer((int)revisionNo.LastRevisionNo, 0);

                    if (allcustomer == null)
                    {
                        return;
                    }

                    if (allcustomer.totalCustomer == 0)
                    {
                        return;
                    }

                    //check ว่ามีไหม
                    List<Gabana3.JAM.Customer.CustomerStatus> UpdateCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                    List<Gabana3.JAM.Customer.CustomerStatus> InsertCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                    List<Customer> lstCustomerImage = new List<Customer>();
                    int round = 0, addrount = 0;
                    round = allcustomer.totalCustomer / 100;
                    addrount = round + 1;
                    for (int j = 0; j < addrount; j++)
                    {
                        allcustomer = await GabanaAPI.GetDataCustomer((int)revisionNo.LastRevisionNo, j);

                        if (allcustomer == null)
                        {
                            break;
                        }

                        if (allcustomer.totalCustomer == 0)
                        {
                            break;
                        }

                        allcustomer.CustomerStatus.ToList().OrderBy(x => x.Customers.RevisionNo);
                        var maxCustomer = allcustomer.CustomerStatus.ToList().Max(x => x.Customers.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                        List<Customer> GetallCustomer = new List<Customer>();
                        GetallCustomer = await customerManage.GetAllCustomer();
                        UpdateCustomer.AddRange(allcustomer.CustomerStatus.Where(x => GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)).ToList());
                        InsertCustomer.AddRange(allcustomer.CustomerStatus.Where(x => !(GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)) && x.DataStatus != 'D').ToList());

                        //Insert Customer
                        if (InsertCustomer.Count > 0)
                        {
                            List<Customer> BulkCustomer = new List<Customer>();
                            foreach (var customer in InsertCustomer)
                            {
                                string thumnailPath = string.Empty;

                                BulkCustomer.Add(new Customer()
                                {
                                    MerchantID = customer.Customers.MerchantID,
                                    SysCustomerID = customer.Customers.SysCustomerID,
                                    CustomerName = customer.Customers.CustomerName,
                                    Ordinary = customer.Customers.Ordinary,
                                    ShortName = customer.Customers.ShortName,
                                    PictureLocalPath = "",
                                    ThumbnailLocalPath = thumnailPath,
                                    EMail = customer.Customers.EMail,
                                    Mobile = customer.Customers.Mobile,
                                    Gender = customer.Customers.Gender,
                                    BirthDate = customer.Customers.BirthDate,
                                    Address = customer.Customers.Address,
                                    ProvincesId = customer.Customers.ProvincesId,
                                    AmphuresId = customer.Customers.AmphuresId,
                                    DistrictsId = customer.Customers.DistrictsId,
                                    PicturePath = customer.Customers.PicturePath, //Clound Image
                                    IDCard = customer.Customers.IDCard,
                                    Comments = customer.Customers.Comments,
                                    LastDateModified = customer.Customers.LastDateModified,
                                    UserLastModified = customer.Customers.UserLastModified,
                                    DataStatus = customer.DataStatus,
                                    FWaitSending = 0,
                                    WaitSendingTime = DateTime.UtcNow,
                                    LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                    MemberTypeNo = customer.Customers.MemberTypeNo,
                                    CustomerID = customer.Customers.CustomerID,
                                    LineID = customer.Customers.LineID,
                                    ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image
                                });
                                maxCustomerRevision = customer.Customers.RevisionNo;
                            }

                            using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                            {
                                try
                                {
                                    await db.BulkCopyAsync(BulkCustomer);
                                }
                                catch (Exception ex)
                                {
                                    var errorRevison = InsertCustomer.Select(x => x.Customers.RevisionNo).Min();
                                    maxCustomerRevision = errorRevison;
                                    Log.Error("connecterror", "BulkCustomer :" + ex.Message);
                                    throw ex;
                                }
                            }

                            lstCustomerImage.AddRange(BulkCustomer);
                        }

                        //Update Customer
                        if (UpdateCustomer.Count > 0)
                        {
                            foreach (var customer in UpdateCustomer)
                            {
                                var data = await customerManage.GetCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);

                                if (customer.DataStatus == 'D')
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
                                    //delete
                                    var delete = await customerManage.DeleteCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);
                                    if (!delete)
                                    {
                                        if (data != null)
                                        {
                                            data.DataStatus = 'D';
                                            data.FWaitSending = 0;
                                            await customerManage.UpdateCustomer(data);
                                        }
                                    }
                                }
                                else
                                {
                                    string thumnailLocalPath = string.Empty;

                                    //insertorreplace
                                    Customer _customer = new Customer()
                                    {
                                        MerchantID = customer.Customers.MerchantID,
                                        SysCustomerID = customer.Customers.SysCustomerID,
                                        CustomerName = customer.Customers.CustomerName,
                                        Ordinary = customer.Customers.Ordinary,
                                        ShortName = customer.Customers.ShortName,
                                        PictureLocalPath = "",
                                        ThumbnailLocalPath = thumnailLocalPath,
                                        EMail = customer.Customers.EMail,
                                        Mobile = customer.Customers.Mobile,
                                        Gender = customer.Customers.Gender,
                                        BirthDate = customer.Customers.BirthDate,
                                        Address = customer.Customers.Address,
                                        ProvincesId = customer.Customers.ProvincesId,
                                        AmphuresId = customer.Customers.AmphuresId,
                                        DistrictsId = customer.Customers.DistrictsId,
                                        PicturePath = customer.Customers.PicturePath, //Clound Image
                                        IDCard = customer.Customers.IDCard,
                                        Comments = customer.Customers.Comments,
                                        LastDateModified = customer.Customers.LastDateModified,
                                        UserLastModified = customer.Customers.UserLastModified,
                                        DataStatus = customer.DataStatus,
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                        MemberTypeNo = customer.Customers.MemberTypeNo,
                                        CustomerID = customer.Customers.CustomerID,
                                        LineID = customer.Customers.LineID,
                                        ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image

                                    };
                                    var insertOrreplace = await customerManage.InsertOrReplaceCustomer(_customer);
                                }

                                maxCustomerRevision = customer.Customers.RevisionNo;
                            }
                        }
                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxCustomer);
                    }
                    //insert Image to Local เมื่อเพิ่มข้อมูลทั้งหมดสำเร็จ ทั้งเคสเพิ่มและเคสอัปเดต
                    Log.Debug("connectpass", "InsertPictureLocalCustomer(lstCustomerImage) lstCustomerImage : " + lstCustomerImage.Count);
                    Task.Factory.StartNew(() => Utils.InsertPictureLocalCustomer(lstCustomerImage));
                    Log.Debug("connectpass", "UpdateImageCustomer(UpdateCustomer) UpdateCustomer : " + UpdateCustomer.Count);
                    Task.Factory.StartNew(() => Utils.UpdateImageCustomer(UpdateCustomer));

                    Log.Debug("connectpass", "listRivisionCustomer");
                }
                catch (Exception ex)
                {
                    Log.Debug("connecterror", "listRivisionCustomer : " + ex.Message);
                    await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxCustomerRevision);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CustomerChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task NoteCategoryChange()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 60).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region NoteCategory
                    try
                    {
                        var allNoteCategory = await GabanaAPI.GetDataNoteCategory((int)revisionNo.LastRevisionNo);

                        if (allNoteCategory == null)
                        {
                            return;
                        }

                        if (allNoteCategory.NoteCategory.Count == 0 && allNoteCategory.NoteCategoryBin.Count == 0)
                        {
                            return;
                        }

                        int maxNoteCategory = 0;
                        if (allNoteCategory.NoteCategory.Count > 0)
                        {
                            allNoteCategory.NoteCategory.ToList().OrderBy(x => x.RevisionNo);
                            maxNoteCategory = allNoteCategory.NoteCategory.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  

                            //check ว่ามีไหม
                            List<ORM.Master.NoteCategory> UpdateNoteCategory = new List<ORM.Master.NoteCategory>();
                            List<ORM.Master.NoteCategory> InsertNoteCategory = new List<ORM.Master.NoteCategory>();
                            List<NoteCategory> GetallNoteCategory = await noteCategoryManage.GetAllNoteCategory();
                            UpdateNoteCategory.AddRange(allNoteCategory.NoteCategory.Where(x => GetallNoteCategory.Select(y => (long)y.SysNoteCategoryID).ToList().Contains(x.SysNoteCategoryID)).ToList());
                            InsertNoteCategory.AddRange(allNoteCategory.NoteCategory.Where(x => !(GetallNoteCategory.Select(y => (long)y.SysNoteCategoryID).ToList().Contains(x.SysNoteCategoryID))).ToList());

                            //Insert NoteCategory
                            if (InsertNoteCategory.Count > 0)
                            {
                                List<NoteCategory> BulkNoteCategory = new List<NoteCategory>();

                                foreach (var NoteCategory in InsertNoteCategory)
                                {
                                    BulkNoteCategory.Add(new NoteCategory()
                                    {
                                        MerchantID = NoteCategory.MerchantID,
                                        SysNoteCategoryID = NoteCategory.SysNoteCategoryID,
                                        Ordinary = NoteCategory.Ordinary,
                                        Name = NoteCategory.Name,
                                        DateCreated = NoteCategory.DateCreated,
                                        DateModified = NoteCategory.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow
                                    });
                                    maxNoteCategoryRevision = NoteCategory.RevisionNo;
                                }

                                using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                {
                                    try
                                    {
                                        await db.BulkCopyAsync(BulkNoteCategory);
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorRevison = InsertNoteCategory.Select(x => x.RevisionNo).Min();
                                        maxNoteCategoryRevision = errorRevison;
                                        Log.Error("connecterror", "BulkNoteCategory :" + ex.Message);
                                        throw ex;
                                    }
                                }
                            }

                            //Update NoteCategory
                            if (UpdateNoteCategory.Count > 0)
                            {
                                foreach (var item in UpdateNoteCategory)
                                {
                                    noteCategory = new NoteCategory()
                                    {
                                        MerchantID = item.MerchantID,
                                        SysNoteCategoryID = item.SysNoteCategoryID,
                                        Ordinary = item.Ordinary,
                                        Name = item.Name,
                                        DateCreated = item.DateCreated,
                                        DateModified = item.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow
                                    };
                                    var insertOrreplace = await noteCategoryManage.InsertOrReplaceCategory(noteCategory);

                                    maxNoteCategoryRevision = item.RevisionNo;
                                }
                            }
                        }

                        if (allNoteCategory.NoteCategoryBin.Count > 0)
                        {
                            allNoteCategory.NoteCategoryBin.ToList().OrderBy(x => x.RevisionNo);
                            maxNoteCategory = allNoteCategory.NoteCategoryBin.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  
                                                                                                              //delete
                            foreach (var itemDelete in allNoteCategory.NoteCategoryBin)
                            {
                                //UpdateItem
                                var UpdateNote = await noteManage.GetNoteBYCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                if (UpdateNote != null)
                                {
                                    foreach (var update in UpdateNote)
                                    {
                                        update.SysNoteCategoryID = null;
                                        var resultUpdate = await noteManage.UpdateNote(update);
                                    }
                                }
                                var delete = await noteCategoryManage.DeleteNoteCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                if (!delete)
                                {
                                    var data = await noteCategoryManage.GetNoteCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                    if (data != null)
                                    {
                                        data.DataStatus = 'D';
                                        data.FWaitSending = 0;
                                        await noteCategoryManage.UpdateNoteCategory(data);
                                    }
                                }
                                maxNoteCategoryRevision = itemDelete.RevisionNo;
                            }
                        }

                        Log.Debug("connectpass", "listRivisionNoteCategory");
                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxNoteCategory);

                        //Set flag สำหรับ Reload ข้อมูลที่เครื่อง
                        DataCashingAll.flagNoteCategoryChange = true;
                        //if (NoteActivity.noteActivity != null)
                        //{
                        //    NoteActivity.noteActivity.Resume();
                        //}
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionNoteCategory : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxNoteCategoryRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteCategoryChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task NoteChange()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 70).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region Note                            
                    try
                    {
                        //Get NoteCategory API
                        var allNote = await GabanaAPI.GetDataNotes((int)revisionNo.LastRevisionNo, 0);

                        if (allNote == null)
                        {
                            return;
                        }

                        if (allNote.totalNotes == 0)
                        {
                            return;
                        }

                        int round = 0, addrount = 0;
                        round = allNote.totalNotes / 100;
                        addrount = round + 1;
                        for (int j = 0; j < addrount; j++)
                        {
                            allNote = await GabanaAPI.GetDataNotes((int)revisionNo.LastRevisionNo, j);

                            if (allNote == null)
                            {
                                break;
                            }

                            if (allNote.totalNotes == 0)
                            {
                                break;
                            }

                            allNote.noteWithNoteStatuses.ToList().OrderBy(x => x.note.RevisionNo);
                            var maxNote = allNote.noteWithNoteStatuses.ToList().Max(x => x.note.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();                                                             

                            //check ว่ามีไหม
                            List<Gabana3.JAM.Notes.NoteWithNoteStatus> UpdateNote = new List<Gabana3.JAM.Notes.NoteWithNoteStatus>();
                            List<Gabana3.JAM.Notes.NoteWithNoteStatus> InsertNote = new List<Gabana3.JAM.Notes.NoteWithNoteStatus>();
                            List<Note> GetallNote = await noteManage.GetAllNote(DataCashingAll.MerchantId);
                            UpdateNote.AddRange(allNote.noteWithNoteStatuses.Where(x => GetallNote.Select(y => (long)y.SysNoteID).ToList().Contains(x.note.SysNoteID)).ToList());
                            InsertNote.AddRange(allNote.noteWithNoteStatuses.Where(x => !(GetallNote.Select(y => (long)y.SysNoteID).ToList().Contains(x.note.SysNoteID)) && x.DataStatus != 'D').ToList()); ;

                            //Insert Note
                            if (InsertNote.Count > 0)
                            {
                                List<Note> BulkNote = new List<Note>();
                                foreach (var Note in InsertNote)
                                {
                                    BulkNote.Add(new Note()
                                    {
                                        MerchantID = Note.note.MerchantID,
                                        SysNoteID = Note.note.SysNoteID,
                                        Ordinary = Note.note.Ordinary,
                                        Message = Note.note.Message,
                                        SysNoteCategoryID = Note.note.SysNoteCategoryID,
                                        LastDateModified = Note.note.LastDateModified,
                                        UserLastModified = Note.note.UserLastModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow
                                    });
                                    maxNoteCategoryRevision = Note.note.RevisionNo;
                                }

                                using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                {
                                    try
                                    {
                                        await db.BulkCopyAsync(BulkNote);
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorRevison = InsertNote.Select(x => x.note.RevisionNo).Min();
                                        maxNoteRevision = errorRevison;
                                        Log.Error("connecterror", "BulkNote :" + ex.Message);
                                        throw ex;
                                    }
                                }
                            }

                            //Update Note
                            if (UpdateNote.Count > 0)
                            {
                                foreach (var Note in UpdateNote)
                                {
                                    if (Note.DataStatus == 'D')
                                    {
                                        //delete
                                        var delete = await noteManage.DeleteNote(Note.note.MerchantID, Note.note.SysNoteID);
                                        if (!delete)
                                        {
                                            var data = await noteManage.GetNote(Note.note.MerchantID, Note.note.SysNoteID);
                                            if (data != null)
                                            {
                                                data.DataStatus = 'D';
                                                data.FWaitSending = 0;
                                                await noteManage.UpdateNote(data);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        note = new Note()
                                        {
                                            MerchantID = Note.note.MerchantID,
                                            SysNoteID = Note.note.SysNoteID,
                                            Ordinary = Note.note.Ordinary,
                                            Message = Note.note.Message,
                                            SysNoteCategoryID = Note.note.SysNoteCategoryID,
                                            LastDateModified = Note.note.LastDateModified,
                                            UserLastModified = Note.note.UserLastModified,
                                            DataStatus = 'I',
                                            FWaitSending = 0,
                                            WaitSendingTime = DateTime.UtcNow
                                        };
                                        var insertOrreplace = await noteManage.InsertOrReplaceNote(note);
                                    }
                                    maxNoteRevision = Note.note.RevisionNo;
                                }
                            }

                            await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxNote);
                        }

                        Log.Debug("connectpass", "listRivisionNote");

                        //Set flag สำหรับ Reload ข้อมูลที่เครื่อง
                        DataCashingAll.flagNoteChange = true;
                        //if (NoteActivity.noteActivity != null)
                        //{
                        //    NoteActivity.noteActivity.Resume();
                        //}
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionNote : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxNoteRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task MerchantConfigChange()
        {
            try
            {
                List<MerchantConfig> lstconfig = new List<MerchantConfig>();
                SetMerchantConfig setconfig = new SetMerchantConfig();
                MerchantConfigManage merchantconfigManage = new MerchantConfigManage();

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
                        setconfig.TAXRATE = TAXRATE.CfgFloat.ToString();
                    }

                    var CURRENCY_SYMBOLS = lstconfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                    if (CURRENCY_SYMBOLS != null)
                    {
                        setconfig.CURRENCY_SYMBOLS = CURRENCY_SYMBOLS.CfgString;
                    }

                    var DECIMAL_POINT_CALC = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_CALC").FirstOrDefault();
                    if (DECIMAL_POINT_CALC != null)
                    {
                        setconfig.DECIMAL_POINT_CALC = DECIMAL_POINT_CALC.CfgInteger.ToString();
                    }

                    var DECIMAL_POINT_DISPLAY = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_DISPLAY").FirstOrDefault();
                    if (DECIMAL_POINT_DISPLAY != null)
                    {
                        setconfig.DECIMAL_POINT_DISPLAY = DECIMAL_POINT_DISPLAY.CfgInteger.ToString();
                    }

                    var OPTION_ROUNDING = lstconfig.Where(x => x.CfgKey == "OPTION_ROUNDING").FirstOrDefault();
                    if (OPTION_ROUNDING != null)
                    {
                        setconfig.OPTION_ROUNDING_STRING = OPTION_ROUNDING.CfgString;
                        setconfig.OPTION_ROUNDING_INT = OPTION_ROUNDING.CfgInteger.ToString();
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

                    var CASHDRAWER = lstconfig.Where(x => x.CfgKey == "CASHDRAWER").FirstOrDefault();
                    if (CASHDRAWER != null)
                    {
                        setconfig.CASHDRAWER = CASHDRAWER.CfgInteger.ToString();
                    }
                    else
                    {
                        setconfig.CASHDRAWER = "0";
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
                    DataCashingAll.flagMerchantConfigChange = true;
                }
            }
            catch (Exception ex)
            {
                string text = "MerchantConfigChange";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task MemberTypeChange()
        {
            try
            {
                MemberTypeManage memberTypeManage = new MemberTypeManage();
                CustomerManage customerManage = new CustomerManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    listmembertype = await GabanaAPI.GetDataMemberType();
                    if (listmembertype.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมดก่อน                       
                        var Allmember = await memberTypeManage.DeleteAllMemberType(DataCashingAll.MerchantId);

                        var lstmember = new List<MemberType>();
                        foreach (var item in listmembertype)
                        {
                            MemberType memberType = new MemberType()
                            {
                                DateModified = item.DateModified,
                                LinkProMaxxID = item.LinkProMaxxID,
                                MemberTypeName = item.MemberTypeName,
                                MemberTypeNo = item.MemberTypeNo,
                                MerchantID = item.MerchantID,
                                PercentDiscount = item.PercentDiscount
                            };
                            var InsertorReplace = await memberTypeManage.InsertorReplacrMemberType(memberType);
                            lstmember.Add(memberType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MemberTypeChange at MyFirebaseMessagingService");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task MerchantChange()
        {
            try
            {

                merchants = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                if (merchants != null)
                {
                    //insert to local
                    MerchantManage merchantManage = new MerchantManage();
                    Merchant merchantlocal = new Merchant();
                    merchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                    string pathClound = merchants.Merchant.LogoPath;
                    bool result = false;
                    //merchant
                    Merchant merchant = new Merchant()
                    {
                        MerchantID = merchants.Merchant.MerchantID,
                        Name = merchants.Merchant.Name,
                        FMasterMerchant = merchants.Merchant.FMasterMerchant, // ถ้า FMasterMerchant = 1 ค่าของ RefMasterMerchantID จะเป็นเลขเดียวกับ MerchantID 
                        RefMasterMerchantID = merchants.Merchant.MerchantID,//รอแก้ไขเรื่อง ถ้า FMasterMerchant = 0 ค่าของ RefMasterMerchantID จะเป็นค่าของ MerchantID ที่เป็น Master ที่ Merchant นี้เป็น Franchise อยู่
                        Status = merchants.Merchant.Status,
                        DateOpenMerchant = merchants.Merchant.DateOpenMerchant,
                        RefPackageID = merchants.Merchant.RefPackageID, //Reference ID ของ Package ไปยังระบบ MerchantLicence
                        DayOfPeriod = merchants.Merchant.DayOfPeriod,//วันที่ของรอบการคิดเงิน ของ Package เช่นทุกวันที่ 10 ของทุกเดือน
                        DueDate = merchants.Merchant.DueDate,
                        LanguageCountryCode = merchants.Merchant.LanguageCountryCode,//Default 'th-TH'
                        TimeZoneName = merchants.Merchant.TimeZoneName,
                        TimeZoneUTCOffset = merchants.Merchant.TimeZoneUTCOffset,
                        LogoPath = merchants.Merchant.LogoPath,
                        DateCreated = merchants.Merchant.DateCreated,
                        DateModified = merchants.Merchant.DateModified,
                        UserNameModified = merchants.Merchant.UserNameModified,
                        DateCloseMerchant = merchants.Merchant.DateCloseMerchant,
                        FPrivacyPolicy = merchants.Merchant.FPrivacyPolicy,
                        FTermConditions = merchants.Merchant.FTermConditions,
                        RegMark = merchants.Merchant.RegMark,
                        TaxID = merchants.Merchant.TaxID
                    };
                    merchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                    if (merchantlocal == null)
                    {
                        string path = await Utils.InsertLocalPictureMerchantMaster(pathClound);
                        merchant.LogoLocalPath = path;
                        result = await merchantManage.InsertMerchant(merchant);
                    }
                    else
                    {
                        string logoPath = string.Empty;
                        logoPath = Utils.SplitCloundPath(merchantlocal?.LogoPath);

                        if ((logoPath?.ToString() != Utils.SplitCloundPath(merchants?.Merchant.LogoPath)) || (string.IsNullOrEmpty(merchantlocal.LogoLocalPath)))
                        {
                            string path = await Utils.InsertLocalPictureMerchantMaster(pathClound);
                            merchant.LogoLocalPath = path;
                        }
                        else
                        {
                            merchant.LogoLocalPath = merchantlocal.LogoLocalPath;
                        }
                        result = await merchantManage.UpdateMerchant(merchant);
                    }
                    if (result)
                    {
                        var GETmerchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                        if (GETmerchantlocal.LogoPath != pathClound)
                        {
                            await Utils.InsertLocalPictureMerchant(GETmerchantlocal);
                        }

                        DataCashingAll.Merchant = merchants;
                        DataCashingAll.MerchantLocal = GETmerchantlocal;
                        Preferences.Set("MerchantID", (int)GETmerchantlocal.MerchantID);
                        DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MerchantChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task MyQrCodeChange()
        {
            try
            {
                MyQrCodeManage QrCodeManage = new MyQrCodeManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
                    List<MyQrCode> lstmyqr = new List<MyQrCode>();
                    myqrcodes = await GabanaAPI.GetDataMyQrCode();
                    if (myqrcodes == null)
                    {
                        return;
                    }
                    if (myqrcodes.Count > 0)
                    {
                        //ลบข้อมูลทังหมดก่อน                        
                        var data = await QrCodeManage.GetAllMyQrCode(DataCashingAll.MerchantId);
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                if (System.IO.File.Exists(item.PictureLocalPath))
                                {
                                    System.IO.File.Delete(item.PictureLocalPath);
                                }
                            }
                        }
                        var AllQR = await QrCodeManage.DeleteAllMyQrCode(DataCashingAll.MerchantId);

                        var lst = myqrcodes.OrderBy(x => x.MyQrCodeNo).ToList();
                        foreach (var item in lst)
                        {
                            ORM.MerchantDB.MyQrCode myQrCode = new ORM.MerchantDB.MyQrCode()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified,
                                Comments = item.Comments,
                                FMyQrAllBranch = item.FMyQrAllBranch,
                                MyQrCodeName = item.MyQrCodeName,
                                MyQrCodeNo = item.MyQrCodeNo,
                                PicturePath = item.PicturePath,
                                PictureLocalPath = item.PicturePath,
                                SysBranchID = item.SysBranchID  // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : DataCashingAll.SysBranchId 
                            };
                            await QrCodeManage.InsertOrReplaceMyQrCode(myQrCode);
                            lstmyqr.Add(myQrCode);
                        }

                        Task.Factory.StartNew(() => Utils.InsertPictureLocalMyQR(lstmyqr));
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MyQrCodeChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task GiftVoucherChange()
        {
            try
            {
                GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    listgiftVouchers = await GabanaAPI.GetDataGiftVoucher();
                    if (listgiftVouchers == null)
                    {
                        return;
                    }
                    if (listgiftVouchers.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมด
                        var Allgifts = await giftVoucherManage.DeleteAllGiftVoucher(DataCashingAll.MerchantId);

                        var lst = listgiftVouchers.OrderBy(x => x.FmlAmount).ToList();
                        foreach (var item in lst)
                        {
                            ORM.MerchantDB.GiftVoucher giftVoucher = new GiftVoucher()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                FmlAmount = item.FmlAmount,
                                GiftVoucherCode = item.GiftVoucherCode,
                                GiftVoucherName = item.GiftVoucherName,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified
                            };
                            await giftVoucherManage.InsertOrReplaceGiftVoucher(giftVoucher);
                        }
                        DataCashingAll.flagGiftVoucherChange = true;
                        if (Setting_Fragment_GiftVoucher.fragment_giftvoucher != null)
                        {
                            Setting_Fragment_GiftVoucher.fragment_giftvoucher.OnResume();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GiftVoucherChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task BranchChange()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                MerchantManage merchantManage = new MerchantManage();
                ORM.MerchantDB.Branch insertBranch = new ORM.MerchantDB.Branch();
                getMerchant = await GabanaAPI.GetDataBranch();
                if (getMerchant == null)
                {
                    return;
                }
                for (int i = 0; i < getMerchant.Count; i++)
                {
                    insertBranch.MerchantID = getMerchant[i].MerchantID;
                    insertBranch.SysBranchID = Convert.ToInt64(getMerchant[i].BranchID);
                    insertBranch.Ordinary = getMerchant[i].Ordinary;
                    insertBranch.BranchName = getMerchant[i].BranchName;
                    insertBranch.BranchID = getMerchant[i].BranchID;
                    insertBranch.Address = getMerchant[i].Address;
                    insertBranch.ProvincesId = getMerchant[i].ProvincesId;
                    insertBranch.AmphuresId = getMerchant[i].AmphuresId;
                    insertBranch.DistrictsId = getMerchant[i].DistrictsId;
                    insertBranch.Status = getMerchant[i].Status;
                    insertBranch.DisplayLanguage = getMerchant[i].DisplayLanguage;
                    insertBranch.Lat = getMerchant[i].Lat;
                    insertBranch.Lng = getMerchant[i].Lng;
                    insertBranch.Email = getMerchant[i].Email;
                    insertBranch.Tel = getMerchant[i].Tel;
                    insertBranch.Line = getMerchant[i].Line;
                    insertBranch.Facebook = getMerchant[i].Facebook;
                    insertBranch.Instagram = getMerchant[i].Instagram;
                    insertBranch.TaxBranchName = getMerchant[i].TaxBranchName;
                    insertBranch.TaxBranchID = getMerchant[i].TaxBranchID;
                    insertBranch.LinkProMaxxID = getMerchant[i].LinkProMaxxID;
                    insertBranch.Comments = getMerchant[i].Comments;

                    var insert = await branchManage.InsertorReplacrBranch(insertBranch);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BranchChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task CashTemplateChange()
        {
            try
            {
                CashTemplateManage templateManage = new CashTemplateManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                    if (listcashTemplate == null)
                    {
                        return;
                    }
                    if (listcashTemplate.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมด
                        var delete = await templateManage.DeleteAllCashTemplatee(DataCashingAll.MerchantId);

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
                            var InsertorReplace = await templateManage.InsertorReplaceCashTemplate(cashTemplate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CashTemplateChange at MyFirebaseMessagingService");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        public async void ReloadRevision(int systeminfo)
        {
            try
            {
                switch (systeminfo)
                {
                    case 20:
                        CategoryChange();
                        break;
                    case 30:
                        ItemChange();
                        break;
                    case 31:
                        ItemOnBranchChange();
                        break;
                    case 50:
                        CustomerChange();
                        break;
                    case 60:
                        NoteCategoryChange();
                        break;
                    case 70:
                        NoteChange();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {               
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ReloadRevision");
            }
        }


    }
}