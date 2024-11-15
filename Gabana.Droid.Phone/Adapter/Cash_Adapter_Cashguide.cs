using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class Cash_Adapter_Cashguide : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<CashTemplate> listcashTemplates;
        public string positionClick;


        public Cash_Adapter_Cashguide(List<CashTemplate> l)
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
                ListViewMembertypeHolder vh = holder as ListViewMembertypeHolder;
                vh.Name.Text = listcashTemplates[position].Amount.ToString("#,###.##");
                GetItemViewType(position);

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cash_adapter_cashguide, parent, false);
            ListViewMembertypeHolder vh = new ListViewMembertypeHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
        public override int GetItemViewType(int position)
        {
            if (position == 4)
            {
                return 1;
            }
            return base.GetItemViewType(position);
        }
    }
}