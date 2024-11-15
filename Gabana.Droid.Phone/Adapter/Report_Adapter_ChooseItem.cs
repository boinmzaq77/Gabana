using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_ChooseItem : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        public List<Item> listChooseItem;
        public string positionClick;

        public Report_Adapter_ChooseItem(ListItem l, List<Item> c)
        {
            listitem = l;
            listChooseItem = c;
        }

        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewBestSaleHolder vh = holder as ListViewBestSaleHolder;

            var paths = listitem[position].PicturePath;
            if (!string.IsNullOrEmpty(paths))
            {
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    Utils.SetImage(vh.ImageItem, paths);
                }
                else
                {
                    var folder = DataCashingAll.PathThumnailFolderImage;
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
                            vh.ImageItem.SetImageURI(uri);
                        }
                    }
                }
                vh.ShortName.Visibility = ViewStates.Gone;
            }
            else
            {
                string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                var color = Android.Graphics.Color.ParseColor(conColor);
                vh.ImageItem.SetBackgroundColor(color);
                vh.ShortName.Visibility = ViewStates.Visible;
            }


            vh.ShortName.Text = listitem[position].ItemName?.ToString();
            vh.Name.Text = listitem[position].ItemName?.ToString();


            var index = listChooseItem.FindIndex(x => x.SysItemID == listitem[position].SysItemID);
            if (index == -1)
            {
                vh.Check.Visibility = ViewStates.Invisible;
            }
            else
            {
                vh.Check.Visibility = ViewStates.Visible;
            }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_chooseitem, parent, false);
            ListViewBestSaleHolder vh = new ListViewBestSaleHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}