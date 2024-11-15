using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;

namespace Gabana.Droid.Adapter
{
    public class Option_Adapter_CategoryExtra : RecyclerView.Adapter
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
                if (OptionActivity.nameCategory == lstcategory[position].Name && OptionActivity.sysCategory == lstcategory[position].SysCategoryID)
                {
                    vh.txtNameCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    vh.lineblue.Visibility = ViewStates.Visible;
                    vh.ItemView.RequestFocus();
                }
                else
                {
                    vh.txtNameCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
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
}