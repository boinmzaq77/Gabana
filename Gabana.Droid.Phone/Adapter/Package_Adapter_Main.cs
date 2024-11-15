using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace Gabana.Droid.Adapter
{
    public class Package_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<PackageProduce> packages;
        CardView cardViewGrid;
        ListViewPackageHolder vh;
        View itemView;
        string CURRENCYSYMBOLS;

        public Package_Adapter_Main(List<PackageProduce> l)
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
                        if (PackageActivity.activity != null)
                        {
                            PackageActivity.activity.SelectPackage(packages[position]);
                        }

                        if (RenewPackageActivity.activity != null)
                        {
                            RenewPackageActivity.activity.SelectPackage(packages[position]);
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
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.package_adapter_main, parent, false);
            vh = new ListViewPackageHolder(itemView, OnClick);
            cardViewGrid = itemView.FindViewById<CardView>(Resource.Id.cardViewGrid);
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            //var height = mainDisplayInfo.Height;
            //var widthCard = (width - 10) / 2;
            //var heightimgCard = ((widthCard / 4) * 3);
            //var heighttextCard = heightimgCard / 2;
            //var heigtCard = heightimgCard + heighttextCard;
            //cardViewGrid.LayoutParameters.Height = Convert.ToInt32(Convert.ToDecimal(widthCard).ToString("##"));
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

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete

        }
    }
}
