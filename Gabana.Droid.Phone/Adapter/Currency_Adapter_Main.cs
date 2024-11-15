using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    public class Currency_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCurrency listCurrency;
        public string positionClick;


        public Currency_Adapter_Main(ListCurrency l)
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
                if (CurrencyActivity.currencySelec == listCurrency[position].CurrencyType)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.currency_adapter_main, parent, false);
            ListViewCurrencyHolder vh = new ListViewCurrencyHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}