using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    internal class Package_Adapter_Package : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public List<PackageProduce> packages;
        CardView cardViewGrid;
        ListViewPackageHolder vh;
        View itemView;
        string CURRENCYSYMBOLS;

        public Package_Adapter_Package(List<PackageProduce> l)
        {
            packages = l;
        }
        public override int ItemCount
        {
            get { return packages == null ? 0 : packages.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
                ListViewPackageHolder vh = holder as ListViewPackageHolder;
                vh.PackName.Text = packages[position].PackageName;
                vh.PackDetail1.Text = packages[position].MaxBranch;
                vh.PackDetail2.Text = packages[position].MaxUser;
                vh.Price.Text = packages[position].Price;
                vh.Poppular.Visibility = ViewStates.Gone;
                if (packages[position].id == 1)
                {
                    vh.Poppular.Visibility = ViewStates.Visible;
                }

                vh.btnSelectPackage.Click += delegate
                {
                    try
                    {
                        if (RenewPackageActivity.activity != null)
                        {
                            RenewPackageActivity.activity.SelectPackage(packages[position]);
                        }
                        else if (PackageActivity.activity != null)
                        {
                            PackageActivity.activity.SelectPackage(packages[position]);

                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return;
                    }
                };
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_package, parent, false);
            vh = new ListViewPackageHolder(itemView, OnClick);
            cardViewGrid = itemView.FindViewById<CardView>(Resource.Id.cardViewGrid);
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
           
            return vh;
        }
        private void OnClick(int obj)
        {
            try
            {
                if (ItemClick != null)
                    ItemClick(this, obj);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

    }

    public class ListViewPackageHolder : RecyclerView.ViewHolder
    {
        public TextView PackName { get; set; }
        public TextView PackDetail1 { get; set; }
        public TextView PackDetail2 { get; set; }
        public TextView Price { get; set; }
        public ImageView Poppular { get; set; }
        public RelativeLayout lnPrice { get; set; }
        public Button btnSelectPackage { get; set; }

        public ListViewPackageHolder(View itemview, Action<int> listener) : base(itemview)
        {
            PackName = itemview.FindViewById<TextView>(Resource.Id.textPackName);
            PackDetail1 = itemview.FindViewById<TextView>(Resource.Id.textPackDetail1);
            PackDetail2 = itemview.FindViewById<TextView>(Resource.Id.textPackDetail2);
            Price = itemview.FindViewById<TextView>(Resource.Id.txtPrice);
            Poppular = itemview.FindViewById<ImageView>(Resource.Id.imagePoppular);
            lnPrice = itemview.FindViewById<RelativeLayout>(Resource.Id.lnPrice);
            btnSelectPackage = itemview.FindViewById<Button>(Resource.Id.btnSelectPackage);
            itemview.Click += (sender, e) => listener(base.Position);

        }
    }

}