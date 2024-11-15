using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.IO;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class CartScan_Adapter_Item : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        CardView cardViewGrid;
        ListViewItemHolder vh;
        View itemView;
        Color color;
        public override int GetItemViewType(int position)
        {
            if (listitem.Count == 1)
            {
                return 0;
            }
            else
            {
                if (listitem.Count % 3 == 0)
                {
                    return 1;
                }
                else
                {
                    return (position > 1 && position == listitem.Count - 1) ? 0 : 1;
                }
            }
        }

        public CartScan_Adapter_Item(ListItem l)
        {
            listitem = l;
        }
        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                ListViewItemHolder vh = holder as ListViewItemHolder;

                var paths = listitem[position].PicturePath;
                if (!string.IsNullOrEmpty(paths))
                {
                    if (await GabanaAPI.CheckSpeedConnection())
                    {
                        Utils.SetImage(vh.PicItemView, paths);
                    }
                    else
                    {
                        Bitmap bitmap = null;
                        var folder = DataCashingAll.PathFolderImage;
                        var filesList = Directory.GetFiles(folder);

                        string pathPicture = listitem[position].ThumbnailLocalPath;
                        string path = Utils.SplitPath(pathPicture);
                        string pathImage = folder + path;

                        foreach (var file in filesList)
                        {
                            var filename = System.IO.Path.GetFileName(file);
                            if (filename == path)
                            {
                                Android.Net.Uri uri = Android.Net.Uri.Parse(pathImage);
                                vh.PicItemView.SetImageURI(uri);
                            }
                        }
                    }
                    vh.ItemNameView.Text = string.Empty;
                }
                else
                {
                    vh.PicItemView.SetImageURI(null);
                    string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                    color = Android.Graphics.Color.ParseColor(conColor);
                    vh.ColorItemView.SetBackgroundColor(color);
                    vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                }
                vh.ItemName.Text = listitem[position].ItemName?.ToString();
                var categoryid = listitem[position].SysCategoryID?.ToString();
                var category = PosActivity.lstCategory.Where(x => x.SysCategoryID.ToString() == categoryid).FirstOrDefault();
                vh.ItemPrice.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(listitem[position].Price);

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cartscan_adapter_item, parent, false);
            vh = new ListViewItemHolder(itemView, OnClick);
            cardViewGrid = itemView.FindViewById<CardView>(Resource.Id.cardViewGrid);
            return vh;
        }

        private void OnClick(int obj)
        {
            try
            {
                if (ItemClick != null)
                    ItemClick(this, obj);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

    }
}