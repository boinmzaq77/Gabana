using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    public class EmployeeRole_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListEmpRole listitem;
        public string positionClick;
        public EmployeeRole_Adapter_Main(ListEmpRole l)
        {
            listitem = l;
        }
        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewEmpRoleHolder vh = holder as ListViewEmpRoleHolder;
            vh.EmpPosition.SetImageResource(listitem[position].ImagePosition);
            vh.Name.Text = listitem[position].Position;
            vh.Detail.Text = listitem[position].Details;
            if (EmployeeRoleActivity.SelectPosition == listitem[position].Position)
            {
                vh.Check.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.Check.Visibility = ViewStates.Gone;
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.employeerole_adapter_main, parent, false);
            ListViewEmpRoleHolder vh = new ListViewEmpRoleHolder(itemView, OnClick);
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}