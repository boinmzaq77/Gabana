using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    internal class Setting_Adapter_Currency : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListCurrency listCurrency;
        public string positionClick;

        public Setting_Adapter_Currency(ListCurrency l)
        {
            listCurrency = l;
        }
        public override int ItemCount
        {
            get { return listCurrency == null ? 0 : listCurrency.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewCurrencyHolder vh = holder as ListViewCurrencyHolder;
                vh.CurrencyName.Text = listCurrency[position].CurrencyNameEn;
                int res;
                if (Setting_Fragment_Currency.currencySelec == listCurrency[position].CurrencyType)
                {
                    vh.ImgSelect.Visibility = ViewStates.Visible;
                    res = listCurrency[position].LogoCurrency2;
                }
                else
                {
                    vh.ImgSelect.Visibility = ViewStates.Gone;
                    res = listCurrency[position].LogoCurrency;

                }
                vh.ImgCurrency.SetImageResource(res);
                if (DataCashing.Language == "th")
                {
                    vh.CurrencyName.Text = listCurrency[position].CurrencyNameTh;
                }
                else
                {
                    vh.CurrencyName.Text = listCurrency[position].CurrencyNameEn;
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_currency, parent, false);
            ListViewCurrencyHolder vh = new ListViewCurrencyHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
    public class ListViewCurrencyHolder : RecyclerView.ViewHolder
    {
        public ImageView ImgCurrency { get; set; }
        public TextView CurrencyName { get; set; }
        public ImageView ImgSelect { get; set; }

        public ListViewCurrencyHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImgCurrency = itemview.FindViewById<ImageView>(Resource.Id.imgCurrency);
            CurrencyName = itemview.FindViewById<TextView>(Resource.Id.txtCurrencyName);
            ImgSelect = itemview.FindViewById<ImageView>(Resource.Id.imgSelect);

            itemview.Click += (sender, e) => listener(base.Position);

        }
    }

}