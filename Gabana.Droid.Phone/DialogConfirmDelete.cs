using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Phone;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Globalization;

namespace Gabana.Droid
{
    public class DialogConfirmDelete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static DialogConfirmDelete NewInstance()
        {
            var frag = new DialogConfirmDelete { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.dialog_delete_item, container, false);
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
            //Update DataStatus ของ Item เป็น D ,FWaitSending = 1, WaitSendingTime = Now
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                long SysItemId = DataCashing.EditItemID;
                ItemManage itemManage = new ItemManage();
                Item DeleteItem = new Item();
                DeleteItem = await itemManage.GetItem(DataCashingAll.MerchantId, (int)SysItemId);
                DeleteItem.DataStatus = 'D';
                DeleteItem.FWaitSending = 1;
                DeleteItem.WaitSendingTime = DateTime.UtcNow;
                DeleteItem.TrackStockDateTime = DateTime.UtcNow;
                DeleteItem.LastDateModified = DateTime.UtcNow;


                var result = await itemManage.UpdateItem(DeleteItem);
                if (result)
                {
                    Toast.MakeText(this.Activity, "ลบข้อมูลสำเร็จ", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this.Activity, "ลบข้อมูลไม่สำเร็จ", ToastLength.Short).Show();
                    return;
                }


                //Test get Item
                var getItem = await itemManage.GetItem(DataCashingAll.MerchantId, (int)SysItemId);

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)DataCashingAll.MerchantId, (int)DataCashing.EditItemID);
                }

                //เรียก Dialog Confirm Delete มาแสดง
                Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                Item_Dialog_Edit dialogEdit = new Item_Dialog_Edit();
                //dialogEdit.SetStyle(Resource.Style.PauseDialog, Theme);
                var transactionId = dialogEdit.Show(ft, "DialogEditItem");
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            //เรียก Dialog Confirm Delete มาแสดง
            Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
            Item_Dialog_Edit dialogEdit = new Item_Dialog_Edit();
            //dialogEdit.SetStyle(Resource.Style.PauseDialog, Theme);
            var transactionId = dialogEdit.Show(ft, "DialogEditItem");
            Dismiss();
        }


    }
}
