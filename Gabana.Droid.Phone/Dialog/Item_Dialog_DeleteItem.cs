using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;

namespace Gabana.Droid.Phone
{
    public class Item_Dialog_DeleteItem : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static int SysitemID;
        static string Page;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Item_Dialog_DeleteItem NewInstance(int sysitemID, string _page)
        {
            SysitemID = sysitemID;
            Page = _page;
            var frag = new Item_Dialog_DeleteItem { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_deleteitem, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            //Delete Item ที่เลือก 
            //Update DataStatus ของ Item เป็น D ,FWaitSending = 2, WaitSendingTime = Now
            try
            {
                btn_save.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    btn_save.Enabled = true;
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                ItemManage ItemManage = new ItemManage();
                Item itemDelete = new Item();
                itemDelete = await ItemManage.GetItem(DataCashingAll.MerchantId, Convert.ToInt32(SysitemID));
                itemDelete.DataStatus = 'D';
                itemDelete.FWaitSending = 2;
                itemDelete.LastDateModified = DateTime.UtcNow;
                var update = await ItemManage.UpdateItem(itemDelete);                
                if (!update)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    if (Page == "main")
                    {
                        DataCashingAll.flagItemChange = true;
                        DataCashingAll.flagItemOnBranchChange = true;
                        DataCashingAll.flagCategoryChange = true;
                        ItemActivity.itemActivity.Resume();
                    }
                    btn_save.Enabled = true;
                    return;
                }

                //ลบรูปภาพ
                if (!string.IsNullOrEmpty(itemDelete.ThumbnailLocalPath))
                {
                    Java.IO.File imgFile = new Java.IO.File(itemDelete.ThumbnailLocalPath);
                    if (System.IO.File.Exists(imgFile.AbsolutePath))
                    {
                        System.IO.File.Delete(imgFile.AbsolutePath);
                    }
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem(DataCashingAll.MerchantId, Convert.ToInt32(SysitemID));
                }
                else
                {
                    itemDelete.FWaitSending = 2;
                    await ItemManage.UpdateItem(itemDelete);
                }
                DataCashingAll.flagItemChange = true;
                DataCashingAll.flagItemOnBranchChange = true;
                DataCashingAll.flagCategoryChange = true;
                if (Page != "main")
                {
                    //หน้า add                        
                    this.Activity.Finish();
                }
                else
                {
                    ItemActivity.itemActivity.Resume();
                }
                MainDialog.CloseDialog();
                btn_save.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                if (Page == "main")
                {
                    ItemActivity.itemActivity.Resume();
                }
                btn_save.Enabled = true;
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}
