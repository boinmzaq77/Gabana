using Android.App;
using Android.App.Job;
using Android.OS;
using Android.Util;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Sync;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet
{
    [Service(Name = "gabanaJob.JobService", Permission = "android.permission.BIND_JOB_SERVICE")]
    class GabanaJob : JobService
    {
        public static readonly string TAG = "GabanaTest";// typeof(GabanaJob).FullName;
        public static int jobStartedCount = 0;

        GabanaDispatchTask gabanaDispatchTask;
        JobParameters parameters;

        // เมื่อคุณต้องการนับงานที่เสร็จสิ้นหรือเริ่มต้น
        public static int getJobStartedCount()
        {
            return jobStartedCount;
        }

        public override bool OnStartJob(JobParameters @params)
        {
            Log.Debug(TAG, "OnStartJob");
            parameters = @params;

            // Check MethodID & Parameter
            string methodID = @params.Extras.GetString(JobQueue.MethodIDValueKey, "");
            if (methodID == JobQueue.SendItemMethodID)
            {
                int merchantID = @params.Extras.GetInt(JobQueue.MerchantIDValueKey, -1);
                if (merchantID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter merchantID");
                    return false;
                }

                int sysItemID = @params.Extras.GetInt(JobQueue.SysItemIDValueKey, -1);
                if (sysItemID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter sysItemID");
                    return false;
                }

                gabanaDispatchTask = new GabanaDispatchTask(this);
                gabanaDispatchTask.SetParaOfSendItemMethod(merchantID, sysItemID);

                gabanaDispatchTask.Execute();

                // Called by the operating system when starting the service.
                // Start up a thread, do work on the thread.
                return true;
            }
            else if (methodID == JobQueue.SendCatagoryMethodID)
            {
                int merchantID = @params.Extras.GetInt(JobQueue.MerchantIDValueKey, -1);
                if (merchantID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter merchantID");
                    return false;
                }

                int sysCategoryID = @params.Extras.GetInt(JobQueue.SysCategoryIDValueKey, -1);
                if (sysCategoryID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter sysCategoryID");
                    return false;
                }

                gabanaDispatchTask = new GabanaDispatchTask(this);
                gabanaDispatchTask.SetParaOfSendCategoryMethod(merchantID, sysCategoryID);

                gabanaDispatchTask.Execute();

                // Called by the operating system when starting the service.
                // Start up a thread, do work on the thread.
                return true; // No more work to do!
            }
            else if (methodID == JobQueue.SendTransMethodID)
            {
                int merchantID = @params.Extras.GetInt(JobQueue.MerchantIDValueKey, -1);
                if (merchantID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter merchantID");
                    return false;
                }

                int sysBranchID = @params.Extras.GetInt(JobQueue.SysBranchIDValueKey, -1);
                if (sysBranchID == -1)
                {
                    Log.Debug(TAG, $"methodID {sysBranchID} : no parameter sysBranchID");
                    return false;
                }

                string sysTransNo = @params.Extras.GetString(JobQueue.SysTransIDValueKey);
                if (string.IsNullOrEmpty(sysTransNo))
                {
                    Log.Debug(TAG, $"methodID {sysTransNo} : no parameter sysTransNo");
                    return false;
                }

                gabanaDispatchTask = new GabanaDispatchTask(this);
                gabanaDispatchTask.SetParaOfSendTransMethod(merchantID, sysBranchID, sysTransNo);

                gabanaDispatchTask.Execute();

                //Test Count Job
                jobStartedCount++;
                Log.Debug("CountJob", "jobStartedCount = " + jobStartedCount);

                // Called by the operating system when starting the service.
                // Start up a thread, do work on the thread.
                return true; // No more work to do!
            }
            else if (methodID == JobQueue.SendCustomerMethodID)
            {
                int merchantID = @params.Extras.GetInt(JobQueue.MerchantIDValueKey, -1);
                if (merchantID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter merchantID");
                    return false;
                }

                int sysCustomerID = @params.Extras.GetInt(JobQueue.SysCustomerIDValueKey, -1);
                if (sysCustomerID == -1)
                {
                    Log.Debug(TAG, $"methodID {sysCustomerID} : no parameter sysCustomerID");
                    return false;
                }

                gabanaDispatchTask = new GabanaDispatchTask(this);
                gabanaDispatchTask.SetParaOfSendCustomerMethod(merchantID, sysCustomerID);

                gabanaDispatchTask.Execute();

                // Called by the operating system when starting the service.
                // Start up a thread, do work on the thread.
                return true; // No more work to do!
            }
            else if (methodID == JobQueue.SendNoteCatagoryMethodID)
            {
                int merchantID = @params.Extras.GetInt(JobQueue.MerchantIDValueKey, -1);
                if (merchantID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter merchantID");
                    return false;
                }

                int sysNoteCategoryID = @params.Extras.GetInt(JobQueue.SysNoteCatagoryIDValueKey, -1);
                if (sysNoteCategoryID == -1)
                {
                    Log.Debug(TAG, $"methodID {sysNoteCategoryID} : no parameter sysNoteCategoryID");
                    return false;
                }

                gabanaDispatchTask = new GabanaDispatchTask(this);
                gabanaDispatchTask.SetParaOfSendNoteCategoryMethod(merchantID, sysNoteCategoryID);

                gabanaDispatchTask.Execute();

                // Called by the operating system when starting the service.
                // Start up a thread, do work on the thread.
                return true; // No more work to do!
            }
            else if (methodID == JobQueue.SendNoteMethodID)
            {
                int merchantID = @params.Extras.GetInt(JobQueue.MerchantIDValueKey, -1);
                if (merchantID == -1)
                {
                    Log.Debug(TAG, $"methodID {methodID} : no parameter merchantID");
                    return false;
                }

                int sysNoteID = @params.Extras.GetInt(JobQueue.SysNoteIDValueKey, -1);
                if (sysNoteID == -1)
                {
                    Log.Debug(TAG, $"methodID {sysNoteID} : no parameter sysNoteCategoryID");
                    return false;
                }

                gabanaDispatchTask = new GabanaDispatchTask(this);
                gabanaDispatchTask.SetParaOfSendNoteMethod(merchantID, sysNoteID);

                gabanaDispatchTask.Execute();

                // Called by the operating system when starting the service.
                // Start up a thread, do work on the thread.
                return true; // No more work to do!
            }
            else
            {
                Log.Debug(TAG, $"Unknow methodID {methodID}");
                return false;
            }
        }

        public override bool OnStopJob(JobParameters @params)
        {
            Log.Debug(TAG, "OnStopJob");
            Log.Debug(TAG, "---------------------------------------   System halted the job. -------------------------------------");
            if (gabanaDispatchTask != null && !gabanaDispatchTask.IsCancelled)
            {
                gabanaDispatchTask.Cancel(true);
            }
            gabanaDispatchTask = null;

            // Called by Android when it has to terminate a running service.
            return false; // Don't reschedule the job.        
        }

        class GabanaDispatchTask : AsyncTask<long, Java.Lang.Void, long>
        {
            readonly GabanaJob jobService;

            string paraMethodID = "";
            int paraMerchantID;
            int paraSysItemID;
            int paraSysCategoryID;
            int paraSysBranchID;
            int paraSysCustomerID;
            int paraSysNoteCategoryID;
            int paraSysNoteID;
            string paraSysTransNo = "";

            public GabanaDispatchTask(GabanaJob jobService)
            {
                this.jobService = jobService;
            }

            public void SetParaOfSendItemMethod(int merchantID, int sysItemID)
            {
                paraMethodID = JobQueue.SendItemMethodID;
                paraMerchantID = merchantID;
                paraSysItemID = sysItemID;
                Log.Debug(TAG, "SetParaOfSendItemMethod");
            }

            public void SetParaOfSendCategoryMethod(int merchantID, int sysCategoryID)
            {
                paraMethodID = JobQueue.SendCatagoryMethodID;
                paraMerchantID = merchantID;
                paraSysCategoryID = sysCategoryID;
                Log.Debug(TAG, "SetParaOfSendCategoryMethod");
            }

            public void SetParaOfSendTransMethod(int merchantID, int sysBranchID, string sysTranNo)
            {
                paraMethodID = JobQueue.SendTransMethodID;
                paraMerchantID = merchantID;
                paraSysBranchID = sysBranchID;
                paraSysTransNo = sysTranNo;
                Log.Debug(TAG, "SetParaOfSendTransMethod");
            }

            public void SetParaOfSendCustomerMethod(int merchantID, int SysCustomerID)
            {
                paraMethodID = JobQueue.SendCustomerMethodID;
                paraMerchantID = merchantID;
                paraSysCustomerID = SysCustomerID;
                Log.Debug(TAG, "SetParaOfSendCustomerMethod");
            }

            public void SetParaOfSendNoteCategoryMethod(int merchantID, int SysNoteCategoryID)
            {
                paraMethodID = JobQueue.SendNoteCatagoryMethodID;
                paraMerchantID = merchantID;
                paraSysNoteCategoryID = SysNoteCategoryID;
                Log.Debug(TAG, "SendNoteCatagoryMethodID");
            }

            public void SetParaOfSendNoteMethod(int merchantID, int SysNoteID)
            {
                paraMethodID = JobQueue.SendNoteMethodID;
                paraMerchantID = merchantID;
                paraSysNoteID = SysNoteID;
                Log.Debug(TAG, "SendNoteCatagoryMethodID");
            }

            protected override long RunInBackground(params long[] @params)
            {
                Log.Debug(TAG, "RunInBackground");
                if (paraMethodID == JobQueue.SendItemMethodID)
                {
                    Log.Debug(TAG, $"Starting background task {JobQueue.SendItemMethodID} ++++++++++++++: merchantid {paraMerchantID} : Itemid {paraSysItemID}");

                    SentItemStart();

                    Log.Debug(TAG, $"Background task {JobQueue.SendItemMethodID} completed. ------------- : merchantid {paraMerchantID} : Itemid {paraSysItemID}");
                    jobService.JobFinished(jobService.parameters, false);
                    return 1;
                }
                else if (paraMethodID == JobQueue.SendCatagoryMethodID)
                {
                    Log.Debug(TAG, $"Starting background task {JobQueue.SendCatagoryMethodID} : merchantid {paraMerchantID} : SyscategoryID {paraSysCategoryID}");

                    SentCategorystart();

                    Log.Debug(TAG, $"Background task {JobQueue.SendCatagoryMethodID} completed.  : merchantid {paraMerchantID} : SyscategoryID {paraSysCategoryID}");
                    jobService.JobFinished(jobService.parameters, false);
                    return 1;
                }
                else if (paraMethodID == JobQueue.SendTransMethodID)
                {
                    Log.Debug(TAG, $"Starting background task {JobQueue.SendTransMethodID} : merchantid {paraMerchantID} : SysBranchID {paraSysBranchID} : SystransNo {paraSysTransNo}");

                    SentTransstart();

                    Log.Debug(TAG, $"Background task {JobQueue.SendTransMethodID} completed.  : merchantid {paraMerchantID} : SysBranchID {paraSysBranchID}: SystransNo {paraSysTransNo}");
                    jobService.JobFinished(jobService.parameters, false);
                    return 1;
                }
                else if (paraMethodID == JobQueue.SendCustomerMethodID)
                {
                    Log.Debug(TAG, $"Starting background task {JobQueue.SendCustomerMethodID} : merchantid {paraMerchantID} : SysCustomerID {paraSysCustomerID} ");

                    Task.Run(SentCustomerstart);

                    Log.Debug(TAG, $"Background task {JobQueue.SendCustomerMethodID} completed.  : merchantid {paraMerchantID} : SysCustomerID {paraSysCustomerID}");
                    jobService.JobFinished(jobService.parameters, false);
                    return 1;
                }
                else if (paraMethodID == JobQueue.SendNoteCatagoryMethodID)
                {
                    Log.Debug(TAG, $"Starting background task {JobQueue.SendNoteCatagoryMethodID} : merchantid {paraMerchantID} : SysNoteCategoryID {paraSysNoteCategoryID} ");

                    Task.Run(SentNoteCategorystart);

                    Log.Debug(TAG, $"Background task {JobQueue.SendNoteCatagoryMethodID} completed.  : merchantid {paraMerchantID} : SysNoteCategoryID {paraSysNoteCategoryID}");
                    jobService.JobFinished(jobService.parameters, false);
                    return 1;
                }
                else if (paraMethodID == JobQueue.SendNoteMethodID)
                {
                    Log.Debug(TAG, $"Starting background task {JobQueue.SendNoteMethodID} : merchantid {paraMerchantID} : SysNoteID {paraSysNoteID} ");

                    Task.Run(SentNotestart);

                    Log.Debug(TAG, $"Background task {JobQueue.SendNoteMethodID} completed.  : merchantid {paraMerchantID} : SysNoteID {paraSysNoteID}");
                    jobService.JobFinished(jobService.parameters, false);
                    return 1;
                }
                else
                {
                    Log.Debug(TAG, $"Unknow methodID {paraMethodID}");
                    return -1;
                }
            }

            private async void SentItemStart()
            {
                try
                {
                    byte[] imageByteArray = null;
                    Gabana.ShareSource.Manage.ItemManage itemManager = new Gabana.ShareSource.Manage.ItemManage();
                    Item item = new Item();
                    item = await itemManager.GetItemforSync(paraMerchantID, paraSysItemID);
                    if (item != null)
                    {
                        if (!string.IsNullOrEmpty(item.PictureLocalPath) & item.DataStatus != 'D')
                        {
                            imageByteArray = await Utils.streamImageOffine(item.PictureLocalPath);
                        }
                    }
                    await ItemSync.SentItemAndroid(paraMerchantID, paraSysItemID, imageByteArray);
                }
                catch (System.Exception ex)
                {
                    await TinyInsights.TrackErrorAsync(ex);
                    await TinyInsights.TrackPageViewAsync("SentItemStart");
                }
            }

            private async void SentCategorystart()
            {
                await CategorySync.SentCategoryAndroid(paraMerchantID, paraSysCategoryID);
            }

            private async void SentTransstart()
            {                
                try
                {
                    byte[] imageByteArray = null;
                    Gabana.ShareSource.Manage.TransManage transManage = new ShareSource.Manage.TransManage();
                    Gabana.ShareSource.Manage.TranPaymentManage tranPaymentManage = new Gabana.ShareSource.Manage.TranPaymentManage();
                    Tran tran = new Tran();
                    tran = await transManage.GetTranAndroid(paraMerchantID, paraSysBranchID, paraSysTransNo);
                    if (tran != null)
                    {
                        await transManage.UpdateTranWaitSendingTime(tran);
                        var Payment = await tranPaymentManage.GetTranPayment(paraMerchantID, paraSysBranchID, paraSysTransNo);
                        if (Payment != null)
                        {
                            var Paymentpicture = Payment.Where(x => !string.IsNullOrEmpty(x.PicturePath)).FirstOrDefault();

                            if (!string.IsNullOrEmpty(Paymentpicture?.PicturePath))
                            {
                                imageByteArray = await Utils.streamImageOffine(Paymentpicture?.PicturePath); //แนบรูป ได้ 1 ใบ
                            }
                        }
                        await TransSync.SentTransAndroid(paraMerchantID, paraSysBranchID, paraSysTransNo, imageByteArray);
                    }
                }
                catch (System.Exception ex)
                {
                    await TinyInsights.TrackErrorAsync(ex);
                    await TinyInsights.TrackPageViewAsync("SentTransstart");
                }
            }

            private async Task SentCustomerstart()
            {
                try
                {
                    byte[] imageByteArray = null;
                    Gabana.ShareSource.Manage.CustomerManage CustomerManage = new Gabana.ShareSource.Manage.CustomerManage();
                    Customer customer = new Customer();
                    customer = await CustomerManage.GetCustomerAndroid(paraMerchantID, paraSysCustomerID);
                    if (customer != null)
                    {
                        if (!string.IsNullOrEmpty(customer.PictureLocalPath) & customer.DataStatus != 'D')
                        {
                            imageByteArray = await Utils.streamImageOffine(customer.PictureLocalPath);
                        }
                        await CustomerSync.SentCustomerAndroid(paraMerchantID, paraSysCustomerID, imageByteArray);
                    }
                }
                catch (System.Exception ex)
                {
                    await TinyInsights.TrackErrorAsync(ex);
                    await TinyInsights.TrackPageViewAsync("SentItemStart");
                }                
            }

            private async Task SentNoteCategorystart()
            {
                await NoteCategorySynce.SentNoteCategoryAndroid(paraMerchantID, paraSysNoteCategoryID);
            }

            private async Task SentNotestart()
            {
                await NoteSync.SentNoteAndroid(paraMerchantID, paraSysNoteID);
            }

        }
    }
}