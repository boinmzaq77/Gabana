using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Phone
{
    public class ScanCart_Dialog_Item : Android.Support.V4.App.DialogFragment
    {
        private static List<Item> listItem;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static ScanCart_Dialog_Item NewInstance()
        {
            var frag = new ScanCart_Dialog_Item { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.cartscan_dialog_items, container, false);
            LinearLayout lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;
            try
            {
                RecyclerView recyclerItem = view.FindViewById<RecyclerView>(Resource.Id.recyclerItem);
                ListItem l = new ListItem(listItem);
                CartScan_Adapter_Item adapter_Item = new CartScan_Adapter_Item(l);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Context, 2, 1, false);
                recyclerItem.SetLayoutManager(gridLayoutManager);
                recyclerItem.SetItemViewCacheSize(20);
                recyclerItem.SetAdapter(adapter_Item);
                recyclerItem.HasFixedSize = true;
                adapter_Item.ItemClick += Adapter_Item_ItemClick;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            CartScanActivity.scan.Onresume();
            MainDialog.CloseDialog();
        }

        private void Adapter_Item_ItemClick(object sender, int e)
        {
            var item = listItem[e];
            CartScanActivity.scan.ScanItemtoCart(item);
            CartScanActivity.scan.Onresume();
            MainDialog.CloseDialog();
        }

        internal static void SetListItem(List<Item> i)
        {
            listItem = i;
        }

    }
}
