using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Item_Adapter_Stock : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        private List<ItemOnBranch> itemOnBranch;
        public string positionClick;
        bool CheckNet;

        public Item_Adapter_Stock(ListItem l, List<ItemOnBranch> o, bool c)
        {
            listitem = l;
            itemOnBranch = o;
            CheckNet = c;
        }

        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewItemHolder vh = holder as ListViewItemHolder;

                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;
                var category = new ORM.MerchantDB.Category();

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

                vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                vh.ItemName.Text = listitem[position].ItemName?.ToString();

                if (listitem[position].SysCategoryID == null)
                {
                    category = null;
                    vh.CategoryItem.Text = string.Empty;
                }
                else
                {
                    var categoryid = listitem[position].SysCategoryID;
                    CategoryManage categoryManage = new CategoryManage();
                    List<Category> lstCategory = await categoryManage.GetAllCategory();
                    category = lstCategory.Where(x => x.SysCategoryID == categoryid).FirstOrDefault();
                    if (category != null)
                    {
                        vh.CategoryItem.Text = category.Name;
                    }
                }

                if (listitem[position].FTrackStock == 1)
                {
                    var stockitem = itemOnBranch.Where(x => x.SysItemID == listitem[position].SysItemID).FirstOrDefault();
                    if (stockitem != null)
                    {
                        vh.ItemPrice.Text = stockitem.BalanceStock.ToString("#,##0");
                        if (stockitem.BalanceStock > stockitem.MinimumStock)
                        {
                            vh.Alert.Visibility = ViewStates.Invisible;
                            vh.ItemPrice.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primary, null));
                        }
                        else if (stockitem.BalanceStock <= stockitem.MinimumStock & stockitem.BalanceStock > 0)
                        {
                            vh.Alert.Visibility = ViewStates.Visible;
                            vh.Alert.SetImageResource(Resource.Mipmap.IndicatorYellow);
                            vh.ItemPrice.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.coloralear, null));
                        }
                        else
                        {
                            //stock.BalanceStock <= 0
                            vh.ItemPrice.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorclearcart, null));
                            vh.Alert.Visibility = ViewStates.Visible;
                            vh.Alert.SetImageResource(Resource.Mipmap.IndicatorRed);
                        }
                    }
                    else
                    {
                        vh.Alert.Visibility = ViewStates.Invisible;

                        vh.ItemPrice.Text = "0";
                    }
                }

                vh.ColorItemView.Click += delegate
                {
                    if (!string.IsNullOrEmpty(cloudpath) && !string.IsNullOrEmpty(localpath))
                    {
                        ItemActivity.OpenDialogImage(listitem[position]);
                    }
                };

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_stock, parent, false);
            ListViewItemHolder vh = new ListViewItemHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}