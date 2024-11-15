using CoreFoundation;
using Foundation;
using Gabana.ShareSource;
using Gabana.ShareSource.Sync;
using Gabana3.JAM.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class JobQueue : Gabana.IJobQueue
    {
        DispatchQueue dispatchQueue;
        static public JobQueue Default { get; set; }

        public JobQueue()
        {
            dispatchQueue = new DispatchQueue(label: "gabana.serial.queue");
        }

        private readonly Random _random = new Random();
        public void AddJobSendItem(int merchantid, int SysItemID)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : Itemid {SysItemID}");




                byte[] imageByteArray = null;
                Gabana.ShareSource.Manage.ItemManage itemManage = new Gabana.ShareSource.Manage.ItemManage();
                var item = await itemManage.GetItem(merchantid, SysItemID);
                if (item != null)
                {
                    if (!string.IsNullOrEmpty(item.PictureLocalPath) & item.DataStatus != 'D')
                    {
                        nfloat quality = (nfloat)0.7;
                        var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        UIImage img = UIImage.FromFile(Path.Combine(docFolder, item.PictureLocalPath));
                        imageByteArray = Utils.ReadFully(img.AsJPEG().AsStream());
                    }
                }

                await ItemSync.SentItem(merchantid, SysItemID, imageByteArray); 



                Console.WriteLine($"Background task {taskId} completed.  : Itemid {SysItemID}");

                // mark our background task as complete
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });

        }
        public void AddJobSendCategory(int merchantid, int SysCategoryID)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : Itemid {SysCategoryID}");

                await CategorySync.SentCategory(merchantid, SysCategoryID);



                Console.WriteLine($"Background task {taskId} completed.  : Itemid {SysCategoryID}");

                // mark our background task as complete
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });

        }
        public void AddJobSendCustomer(int merchantid, int SysCustomerID)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : Itemid {SysCustomerID}");


                byte[] imageByteArray = null;
                Gabana.ShareSource.Manage.CustomerManage CustomerManage = new Gabana.ShareSource.Manage.CustomerManage();
                var Customer = await CustomerManage.GetCustomer(merchantid, SysCustomerID);
                if (Customer != null)
                {
                    if (!string.IsNullOrEmpty(Customer.PictureLocalPath) & Customer.DataStatus != 'D')
                    {
                        nfloat quality = (nfloat)0.7;
                        var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        UIImage img = UIImage.FromFile(Path.Combine(docFolder, Customer.PictureLocalPath));
                        imageByteArray = Utils. ReadFully(img.AsJPEG().AsStream());
                    }
                }
                await CustomerSync.SentCustomer(merchantid, SysCustomerID, imageByteArray);



                Console.WriteLine($"Background task {taskId} completed.  : Itemid {SysCustomerID}");

                // mark our background task as complete 
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });

        }
        public void AddJobSendPost(int merchantid, int SysCategoryID)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : Itemid {SysCategoryID}");

                await CategorySync.SentCategory(merchantid, SysCategoryID);



                Console.WriteLine($"Background task {taskId} completed.  : Itemid {SysCategoryID}");

                // mark our background task as complete
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });

        }

        public void AddJobSendCatagory(int merchantid, int SysCategoryID)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : Itemid {SysCategoryID}");

                await CategorySync.SentCategory(merchantid, SysCategoryID);



                Console.WriteLine($"Background task {taskId} completed.  : Itemid {SysCategoryID}");

                // mark our background task as complete
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });
        }

        internal void AddJobSendTrans(int merchantID, int sysBranchID, string tranNo)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : tranNo {tranNo}");

                byte[] imageByteArray = null;
                Gabana.ShareSource.Manage.TransManage transManage = new ShareSource.Manage.TransManage();
                Gabana.ShareSource.Manage.TranPaymentManage tranPaymentManage = new Gabana.ShareSource.Manage.TranPaymentManage();
                var Tran = await transManage.GetTran(merchantID, sysBranchID, tranNo);
                if (Tran != null)
                {
                    var Payment = await tranPaymentManage.GetTranPayment(merchantID, sysBranchID, tranNo);
                    if (Payment != null)
                    {
                        var Paymentpicture = Payment.Where(x => !string.IsNullOrEmpty(x.PicturePath)).FirstOrDefault();

                        if (!string.IsNullOrEmpty(Paymentpicture?.PicturePath))
                        {
                            nfloat quality = (nfloat)0.7;
                            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                            UIImage img = UIImage.FromFile(Path.Combine(docFolder, Paymentpicture?.PicturePath));
                            imageByteArray = Utils.ReadFully(img.AsJPEG().AsStream());
                            //imageByteArray = await Utils.streamImageOffine(Paymentpicture?.PicturePath); //แนบรูป ได้ 1 ใบ
                        }
                    }
                }
                await TransSync.SentTrans(merchantID, sysBranchID, tranNo, imageByteArray);



                Console.WriteLine($"Background task {taskId} completed.  : tranNo {tranNo}");

                // mark our background task as complete
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });
        }

        internal void AddJobSendNote(int merchantID, int sysNoteID)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : note {sysNoteID}");

                await NoteSync.SentNote(merchantID, sysNoteID);



                Console.WriteLine($"Background task {taskId} completed.  : note {sysNoteID}");

                // mark our background task as complete
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });
        }

        internal void AddJobSendNoteCatagory(int merchantID, int sysNoteCategoryID)
        {
            dispatchQueue.DispatchAsync(async () => {

                nint taskId = 0;
                // register our background task
                taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
                {
                    Console.WriteLine("Running out of time to complete you background task!");
                    UIApplication.SharedApplication.EndBackgroundTask(taskId);
                });

                Console.WriteLine($"Starting background task {taskId} : noteCategory {sysNoteCategoryID}");

                await NoteCategorySynce.SentNoteCategory(merchantID, sysNoteCategoryID);



                Console.WriteLine($"Background task {taskId} completed.  : noteCategory {sysNoteCategoryID}");

                // mark our background task as complete
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
            });
        }

        void IJobQueue.AddJobSendTrans(int merchantid, int SysBranchID, string transNo)
        {
            ((IJobQueue)Default).AddJobSendTrans(merchantid, SysBranchID, transNo);
        }

        void IJobQueue.AddJobSendNoteCatagory(int merchantid, int SysNoteCatagoryID)
        {
            ((IJobQueue)Default).AddJobSendNoteCatagory(merchantid, SysNoteCatagoryID);
        }

        void IJobQueue.AddJobSendNote(int merchantid, int SysNoteID)
        {
            ((IJobQueue)Default).AddJobSendNote(merchantid, SysNoteID);
        }
    }
}