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
    public class Membertype_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<MemberType> listmemberTypes;
        public string positionClick;


        public Membertype_Adapter_Main(List<MemberType> l)
        {
            listmemberTypes = l;
        }
        public override int ItemCount
        {
            get { return listmemberTypes == null ? 0 : listmemberTypes.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewMembertypeHolder vh = holder as ListViewMembertypeHolder;
                vh.Name.Text = listmemberTypes[position].MemberTypeName;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.membertype_adapter_main, parent, false);
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