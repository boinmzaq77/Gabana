using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    public class Setting_Adapter_MemberType : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<MemberType> listmemberTypes;
        public string positionClick;


        public Setting_Adapter_MemberType(List<MemberType> l)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_membertype, parent, false);
            ListViewMembertypeHolder vh = new ListViewMembertypeHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewMembertypeHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }

        public ListViewMembertypeHolder(View itemview, Action<int> listener) : base(itemview)
        {

            Name = itemview.FindViewById<TextView>(Resource.Id.txtName);
            itemview.Click += (sender, e) => listener(base.Position);

        }
    }
}