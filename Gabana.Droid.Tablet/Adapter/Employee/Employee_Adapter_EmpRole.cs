using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Employee
{
    internal class Employee_Adapter_EmpRole : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListEmpRole listitem;
        public string positionClick;
        public Employee_Adapter_EmpRole(ListEmpRole l)
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
            if (Employee_Dialog_EmpRole.SelectPosition == listitem[position].Position)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.employee_adapter_emprole, parent, false);
            ListViewEmpRoleHolder vh = new ListViewEmpRoleHolder(itemView, OnClick);
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewEmpRoleHolder : RecyclerView.ViewHolder
    {
        public ImageView EmpPosition { get; set; }
        public TextView Name { get; set; }
        public TextView Detail { get; set; }
        public ImageButton Check { get; set; }

        public ListViewEmpRoleHolder(View itemview, Action<int> listener) : base(itemview)
        {
            EmpPosition = itemview.FindViewById<ImageView>(Resource.Id.imgPosition);
            Name = itemview.FindViewById<TextView>(Resource.Id.textName);
            Detail = itemview.FindViewById<TextView>(Resource.Id.textDetail);
            Check = itemview.FindViewById<ImageButton>(Resource.Id.btnCheck);



            itemview.Click += (sender, e) => listener(base.Position);


        }
    }
}