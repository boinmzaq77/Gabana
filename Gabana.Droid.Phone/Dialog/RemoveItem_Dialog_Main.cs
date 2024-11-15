using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Gabana.Droid
{
    public class RemoveItem_Dialog_Main : Android.Support.V4.App.DialogFragment
    {
        Button btn_save;
        TranWithDetailsLocal tranWithDetailsLocal = new TranWithDetailsLocal();
        List<Item> AllItem = new List<Item>();
        static string tranWithDetails;
        static string lstSysitem;



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static RemoveItem_Dialog_Main NewInstance(string _tranWithDetails, string _lstSysitem)
        {
            tranWithDetails = _tranWithDetails;
            lstSysitem = _lstSysitem;
            var frag = new RemoveItem_Dialog_Main { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            AllItem.AddRange(PosActivity.DefaultAllItem);
            var view = inflater.Inflate(Resource.Layout.offline_dialog_main, container, false);
            try
            {
                TextView textUsepoint = view.FindViewById<TextView>(Resource.Id.textUsepoint);
                //var txt1 = GetString(Resource.String.connectinternet1);
                //var txt2 = GetString(Resource.String.connectinternet2);

                var txt1 = "ตะกร้ามีการอัปเดต เนื่องจาก";
                var txt2 = "สินค้าในตะกร้ามีการเปลี่ยนแปลง";
                textUsepoint.Text = txt1 + "\n" + txt2;
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //PosActivity.tranWithDetails = await  PosActivity.RemoveItem();
                //PosActivity.tranWithDetails =  PosActivity.RemoveItem();

                var jsontranWithDetails = JsonConvert.DeserializeObject<TranWithDetailsLocal>(tranWithDetails);
                var jsonlstSysitem = JsonConvert.DeserializeObject<List<long?>>(lstSysitem);
                tranWithDetailsLocal = Utils.RemoveLstItem(jsontranWithDetails, jsonlstSysitem);

                //if (OrderActivity.OrderCurrentActivity)
                //{

                //}
                PosActivity.tranWithDetails = tranWithDetailsLocal;
                PosActivity.pos.Resume();
                MainDialog.CloseDialog();
            }
            catch (Exception)
            {
                MainDialog.CloseDialog();
            }
        }

    }
}