using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid.Adapter
{
    public class Item_Adapter_Item : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        public bool CheckNet;
        public string positionClick;
        ListViewItemHolder vh;
        public static ListViewItemHolder vhselect;


        public Item_Adapter_Item(ListItem l, bool c)
        {
            listitem = l;
            CheckNet = c;
        }
        public void reloaddata(ListItem l, bool c)
        {
            listitem = l;
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
                vh = holder as ListViewItemHolder;
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;
                vhselect = vh;

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

                vh.ItemNameView.Text = listitem[position].ShortName?.ToString();
                vh.ItemName.Text = listitem[position].ItemName?.ToString();

                if (listitem[position].SysCategoryID != null)
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
                else
                {
                    category = null;
                    vh.CategoryItem.Text = string.Empty;
                }

                vh.ItemPrice.Text = Utils.DisplayDecimal(listitem[position].Price);
                if (listitem[position].FavoriteNo == 1)
                {
                    vh.Favorite.SetBackgroundResource(Resource.Mipmap.Fav);
                }
                else
                {
                    vh.Favorite.SetBackgroundResource(Resource.Mipmap.Unfav);
                }

                #region Aleart Stock
                //กำหนด MinimumStock ระบบจะแสดงเป็น Indicater(วงกลม Notification) สีเหลืองเมื่อต่ำกว่า MinimumStock
                //สีแดง สินค้า =0 หรือติดลบ
                if (listitem[position].FTrackStock == 1)
                {
                    ItemOnBranchManage itemOnBranchManage = new ItemOnBranchManage();
                    var stock = await itemOnBranchManage.GetItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, (int)listitem[position].SysItemID);
                    if (stock != null)
                    {
                        if (stock.BalanceStock > stock.MinimumStock)
                        {
                            vh.Alert.Visibility = ViewStates.Invisible;
                        }
                        else if (stock.BalanceStock <= stock.MinimumStock & stock.BalanceStock > 0)
                        {
                            vh.Alert.Visibility = ViewStates.Visible;
                            vh.Alert.SetImageResource(Resource.Mipmap.IndicatorYellow);
                        }
                        else
                        {
                            //stock.BalanceStock <= 0
                            vh.Alert.Visibility = ViewStates.Visible;
                            vh.Alert.SetImageResource(Resource.Mipmap.IndicatorRed);
                        }
                    }
                    else
                    {
                        vh.Alert.Visibility = ViewStates.Invisible;
                    }
                }
                else
                {
                    vh.Alert.Visibility = ViewStates.Invisible;
                }
                #endregion

                vh.lnFavorite.Click += delegate
                {
                    try
                    {
                        FavClick(position);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("vh.lnFavorite.Click at Item");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };

                vh.Favorite.Click += delegate
                {
                    try
                    {
                        FavClick(position);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("vh.Favorite.Click at Item");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };

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
                return;
            }
        }

        public void FavClick(int Position)
        {
            try
            {
                LnFavoriteClick(listitem[Position].SysItemID);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FavClick at Item");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnFavoriteClick(long? itemId)
        {
            ItemActivity.itemActivity.ShowItemFav(itemId);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_item, parent, false);
            ListViewItemHolder vh = new ListViewItemHolder(itemView, OnClick);

            TextView textSignMoney = itemView.FindViewById<TextView>(Resource.Id.textSignMoney);
            var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            textSignMoney.Text = CURRENCYSYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


        
    }
}