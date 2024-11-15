using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Additem_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Additem_Dialog_Delete NewInstance()
        {
            var frag = new Additem_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }
        Button btn_cancel, btn_save;
      
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.additem_dialog_delete, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }


        private async void Btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                ItemManage itemManage = new ItemManage();
                Item itemDelete = new Item();

                if (DataCashing.EditItem != null)
                {
                    itemDelete = DataCashing.EditItem;
                }

                //btn_save.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    //btn_save.Enabled = true;
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                itemDelete.DataStatus = 'D';
                itemDelete.FWaitSending = 2;
                itemDelete.LastDateModified = DateTime.UtcNow;
                var update = await itemManage.UpdateItem(itemDelete);
                if (!update)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    //POS_Fragment_Main.fragment_main.OnResume();
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
                    JobQueue.Default.AddJobSendItem(DataCashingAll.MerchantId, Convert.ToInt32(itemDelete.SysItemID));
                }
                else
                {
                    itemDelete.FWaitSending = 2;
                    await itemManage.UpdateItem(itemDelete);
                }

                DataCashing.EditItem = null;
                Item_Fragment_AddItem.itemEdit = null;
                Item_Fragment_AddItem.flagdatachange = false;
                Item_Fragment_AddItem.keepCropedUri = null;
                Item_Fragment_Main.fragment_main.DeleteItem(itemDelete);
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "default");
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                MainDialog.CloseDialog();
                return;
            }            
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }
    }
}