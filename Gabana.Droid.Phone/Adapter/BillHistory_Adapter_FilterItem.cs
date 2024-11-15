using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;

namespace Gabana.Droid.Adapter
{
    public class BillHistory_Adapter_FilterItem : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        public string positionClick;
        private string CURRENCYSYMBOLS;
        public bool CheckNet;

        public BillHistory_Adapter_FilterItem(ListItem l, bool c)
        {
            listitem = l;
            CheckNet = c;
        }

        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewFilterItemHolder vh = holder as ListViewFilterItemHolder;
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;

                var cloudpath = listitem[position].PicturePath == null ? string.Empty : listitem[position].PicturePath;
                var localpath = listitem[position].ThumbnailLocalPath == null ? string.Empty : listitem[position].ThumbnailLocalPath;

                if (CheckNet)
                {
                    if (string.IsNullOrEmpty(localpath))
                    {
                        if (string.IsNullOrEmpty(cloudpath))
                        {
                            //defalut
                            string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                            var color = Android.Graphics.Color.ParseColor(conColor);
                            vh.ColorItemView.SetImageURI(null);
                            vh.ItemNameView.Visibility = ViewStates.Visible;
                            vh.ColorItemView.SetBackgroundColor(color);
                        }
                        else
                        {
                            //cloud
                            Utils.SetImage(vh.ColorItemView, cloudpath);
                            vh.ItemNameView.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        //local
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        vh.ColorItemView.SetImageURI(uri);
                        vh.ItemNameView.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        vh.ColorItemView.SetImageURI(uri);
                        vh.ItemNameView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                        var color = Android.Graphics.Color.ParseColor(conColor);
                        vh.ColorItemView.SetImageURI(null);
                        vh.ItemNameView.Visibility = ViewStates.Visible;
                        vh.ColorItemView.SetBackgroundColor(color);
                    }
                }

                vh.ItemNameView.Text = listitem[position].ShortName?.ToString();
                vh.ItemName.Text = listitem[position].ItemName?.ToString();

                var index = BillFilterItemActivity.chooseItem.FindIndex(x => x.SysItemID == listitem[position].SysItemID);
                if (index == -1)
                {
                    vh.Check.Visibility = ViewStates.Invisible;
                }
                else
                {
                    vh.Check.Visibility = ViewStates.Visible;
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.bilhistory_adapter_filteritem, parent, false);
            ListViewFilterItemHolder vh = new ListViewFilterItemHolder(itemView, OnClick);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
    public class ListViewFilterItemHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageItem { get; set; }
        public TextView ShortName { get; set; }
        public TextView Name { get; set; }
        public ImageView Check { get; set; }
        public ImageView ColorItemView { get; set; }
        public TextView ItemNameView { get; set; }
        public TextView ItemName { get; set; }

        public ListViewFilterItemHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImageItem = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            ShortName = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            Name = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            Check = itemview.FindViewById<ImageView>(Resource.Id.imgCheck);
            ColorItemView = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            ItemNameView = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            ItemName = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);

#pragma warning disable CS0618 // Type or member is obsolete
            itemview.Click += (sender, e) => listener(base.Position);
#pragma warning restore CS0618 // Type or member is obsolete

        }
    }

}