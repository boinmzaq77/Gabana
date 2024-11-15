using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    internal class Setting_Adapter_Cash : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<CashTemplate> listcashTemplates;
        public string positionClick;


        public Setting_Adapter_Cash(List<CashTemplate> l)
        {
            listcashTemplates = l;
        }

        public override int ItemCount
        {
            get { return listcashTemplates == null ? 0 : listcashTemplates.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                ListViewMembertypeHolder vh = holder as ListViewMembertypeHolder;
                vh.Name.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(listcashTemplates[position].Amount).ToString();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_cash, parent, false);
            ListViewMembertypeHolder vh = new ListViewMembertypeHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }

}