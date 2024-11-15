using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Addtopping_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Addtopping_Dialog_Delete NewInstance()
        {
            var frag = new Addtopping_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }
        Button btn_cancel, btn_save;
        Addtopping_Dialog_Delete dialog_Delete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.addtopping_dialog_delete, container, false);
            try
            {
                dialog_Delete = this;
                TextView textconfirm1 = view.FindViewById<TextView>(Resource.Id.textconfirm1);
                TextView textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);

                textconfirm1.Text = GetString(Resource.String.dialog_delete_topping_1);
                textconfirm2.Text = GetString(Resource.String.dialog_delete_topping_2);
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
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    MainDialog.CloseDialog();
                    return;
                }

                Item itemDelete = new Item();
                if (DataCashing.EditTopping != null)
                {
                    itemDelete = DataCashing.EditTopping;
                }
                itemDelete.DataStatus = 'D';
                itemDelete.FWaitSending = 2;
                itemDelete.LastDateModified = DateTime.UtcNow;
                var update = await itemManage.UpdateItem(itemDelete);
                if (!update)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    return;
                }

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
                    JobQueue.Default.AddJobSendItem(DataCashingAll.MerchantId, Convert.ToInt32(DataCashing.EditTopping.SysItemID));
                }
                else
                {
                    itemDelete.FWaitSending = 2;
                    await itemManage.UpdateItem(itemDelete);
                }

                DataCashing.EditTopping = null;
                Item_Fragment_AddTopping.ExtraToppping = null;
                Item_Fragment_AddTopping.flagdatachange = false;
                Item_Fragment_AddTopping.keepCropedUri = null;
                Item_Fragment_Main.fragment_main.DeleteTopping(itemDelete);
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "default");
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteExtraTopping at add Extra");
                Toast.MakeText(this.Activity, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }
    }
}