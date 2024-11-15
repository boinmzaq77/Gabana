using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Pos;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class POS_Dialog_ScanItem : AndroidX.Fragment.App.DialogFragment
    {
        private static List<Item> listItem;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static POS_Dialog_ScanItem NewInstance()
        {
            var frag = new POS_Dialog_ScanItem { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_scanitems, container, false);
            LinearLayout lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;
            try
            {
                RecyclerView recyclerItem = view.FindViewById<RecyclerView>(Resource.Id.recyclerItem);
                ListItem l = new ListItem(listItem);
                POS_Adapter_ScanItem adapter_Item = new POS_Adapter_ScanItem(l);
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
            _ = TinyInsights.TrackPageViewAsync("OnCreateView : POS_Dialog_ScanItem");
            return view;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            if (POS_Dialog_Scan.scan != null)
            {
                POS_Dialog_Scan.scan.OnResume();
            }
            MainDialog.CloseDialog();
        }

        private void Adapter_Item_ItemClick(object sender, int e)
        {
            var item = listItem[e];            
            if (POS_Dialog_Scan.scan != null)
            {
                POS_Dialog_Scan.scan.ScanItemtoCart(item);
                POS_Dialog_Scan.scan.OnResume();
            }
            MainDialog.CloseDialog();
        }

        internal static void SetListItem(List<Item> i)
        {
            listItem = i;
        }
    }
   

}