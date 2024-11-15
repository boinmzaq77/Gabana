using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Util;
using Java.Lang;
using System;

namespace Gabana.Droid.Tablet
{
    public class JobQueue : Gabana.IJobQueue
    {
        public static readonly string TAG = "GabanaTest";// typeof(JobQueue).FullName;

        public static readonly int GabanaJobId = 110;

        public static readonly string MethodIDValueKey = "methodID_value";
        public static readonly string SendItemMethodID = "SendItem";
        public static readonly string SendCatagoryMethodID = "SendCatagory";
        public static readonly string SendTransMethodID = "SendTrans";
        public static readonly string SendCustomerMethodID = "SendCustomer";
        public static readonly string SendNoteCatagoryMethodID = "SendNoteCatagory";
        public static readonly string SendNoteMethodID = "SendNote";


        public static readonly string MerchantIDValueKey = "merchantid_value";
        public static readonly string SysItemIDValueKey = "sysitemid_value";
        public static readonly string SysCategoryIDValueKey = "syscategoryid_value";
        public static readonly string SysTransIDValueKey = "systransid_value";
        public static readonly string SysBranchIDValueKey = "sysbranchid_value";
        public static readonly string SysCustomerIDValueKey = "syscustomerid_value";
        public static readonly string SysNoteCatagoryIDValueKey = "sysnotecategoryid_value";
        public static readonly string SysNoteIDValueKey = "sysnoteid_value";

        Context mainContext;
        JobScheduler jobScheduler;
        static public JobScheduler jobScheduler2;

        static public JobQueue Default { get; set; }
        static int JobID;

        public JobQueue(Context context)
        {
            mainContext = context;
            jobScheduler = (JobScheduler)mainContext.GetSystemService(Context.JobSchedulerService);
            jobScheduler2 = (JobScheduler)mainContext.GetSystemService(Context.JobSchedulerService);
        }

        private JobInfo.Builder CreateJobInfoBuilder()
        {
            Log.Debug(TAG, "CreateJobInfoBuilder");
            JobID++;    // Define Next JobId

            var component = mainContext.GetComponentNameForJob<GabanaJob>();
            JobInfo.Builder builder = new JobInfo.Builder(JobID, component)
                                                 .SetPersisted(false)
                                                 .SetMinimumLatency(5000)    // Wait at least 1 second
                                                 .SetOverrideDeadline(10000)  // But no longer than 5 seconds
                                                 .SetRequiredNetworkType(NetworkType.Any);
            return builder;
        }

        public void AddJobSendItem(int merchantid, int sysItemID)
        {
            Log.Debug(TAG, "AddJobSendItem");
            var extras = new PersistableBundle();
            extras.PutString(MethodIDValueKey, SendItemMethodID);
            extras.PutInt(MerchantIDValueKey, merchantid);
            extras.PutInt(SysItemIDValueKey, sysItemID);

            JobInfo.Builder builder = CreateJobInfoBuilder().SetExtras(extras);
            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job SendItem started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job SendItem : " + result);
            }
        }

        public void AddJobSendCatagory(int merchantid, int SysCategoryID)
        {
            Log.Debug(TAG, "AddJobSendCatagory");
            var extras = new PersistableBundle();
            extras.PutString(MethodIDValueKey, SendCatagoryMethodID);
            extras.PutInt(MerchantIDValueKey, merchantid);
            extras.PutInt(SysCategoryIDValueKey, SysCategoryID);

            JobInfo.Builder builder = CreateJobInfoBuilder().SetExtras(extras);
            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job SendCatagory started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job SendCatagory : " + result);
            }
        }

        public void AddJobSendTrans(int merchantid, int SysBranchID, string transNo)
        {
            Log.Debug(TAG, "AddJobSendTrans");
            var extras = new PersistableBundle();
            extras.PutString(MethodIDValueKey, SendTransMethodID);
            extras.PutInt(MerchantIDValueKey, merchantid);
            extras.PutInt(SysBranchIDValueKey, SysBranchID);
            extras.PutString(SysTransIDValueKey, transNo);

            JobInfo.Builder builder = CreateJobInfoBuilder().SetExtras(extras);
            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job SendTrans started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job SendTrans : " + result);
            }
        }

        public void AddJobSendCustomer(int merchantid, int SysCustomerID)
        {
            Log.Debug(TAG, "AddJobSendTrans");
            var extras = new PersistableBundle();
            extras.PutString(MethodIDValueKey, SendCustomerMethodID);
            extras.PutInt(MerchantIDValueKey, merchantid);
            extras.PutInt(SysCustomerIDValueKey, SysCustomerID);

            JobInfo.Builder builder = CreateJobInfoBuilder().SetExtras(extras);
            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job SendCustomer started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job SendCustomer : " + result);
            }
        }

        public void AddJobSendNoteCatagory(int merchantid, int SysNoteCatagoryID)
        {
            Log.Debug(TAG, "AddJobSendNoteCatagory");
            var extras = new PersistableBundle();
            extras.PutString(MethodIDValueKey, SendNoteCatagoryMethodID);
            extras.PutInt(MerchantIDValueKey, merchantid);
            extras.PutInt(SysNoteCatagoryIDValueKey, SysNoteCatagoryID);



            JobInfo.Builder builder = CreateJobInfoBuilder().SetExtras(extras);
            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job SendNoteCatagory started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job SendNoteCatagory : " + result);
            }
        }

        public void AddJobSendNote(int merchantid, int SysNoteID)
        {
            Log.Debug(TAG, "AddJobSendNote");
            var extras = new PersistableBundle();
            extras.PutString(MethodIDValueKey, SendNoteMethodID);
            extras.PutInt(MerchantIDValueKey, merchantid);
            extras.PutInt(SysNoteIDValueKey, SysNoteID);

            JobInfo.Builder builder = CreateJobInfoBuilder().SetExtras(extras);
            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job SendNote started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job SendNote : " + result);
            }
        }
    }


    public static class JobQueueHelpers
    {
        public static ComponentName GetComponentNameForJob<T>(this Context context) where T : JobService
        {
            Type t = typeof(T);
            Class javaClass = Class.FromType(t);
            return new ComponentName(context, javaClass);
        }

    }

}

