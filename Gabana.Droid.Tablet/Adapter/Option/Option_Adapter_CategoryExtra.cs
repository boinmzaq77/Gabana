using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Option
{
    internal class Option_Adapter_CategoryExtra : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCategory lstcategory;
        public string positionClick;

        public Option_Adapter_CategoryExtra(ListCategory l)
        {
            lstcategory = l;
        }

        public override int ItemCount
        {
            get { return lstcategory == null ? 0 : lstcategory.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewCategoryExtraDataHolder vh = holder as ListViewCategoryExtraDataHolder;
                vh.txtNameCategory.Text = lstcategory[position].Name?.ToString();
                vh.lineblue.Visibility = ViewStates.Gone;
                if (POS_Dialog_Option.nameCategory == lstcategory[position].Name && POS_Dialog_Option.sysCategory == lstcategory[position].SysCategoryID)
                {
                    vh.txtNameCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    vh.lineblue.Visibility = ViewStates.Visible;
                    vh.ItemView.RequestFocus();
                }
                else
                {
                    vh.txtNameCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
                    vh.lineblue.Visibility = ViewStates.Gone;
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.option_adapter_categoryextra, parent, false);
            ListViewCategoryExtraDataHolder vh = new ListViewCategoryExtraDataHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
    public class ListViewCategoryExtraDataHolder : RecyclerView.ViewHolder
    {
        public TextView txtNameCategory { get; set; }
        public View lineblue { get; set; }
        public ListViewCategoryExtraDataHolder(View itemview, Action<int> listener) : base(itemview)
        {
            txtNameCategory = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);
            lineblue = itemview.FindViewById<View>(Resource.Id.lineblue);

            itemview.Click += (sender, e) => listener(base.Position);

        }
    }

}