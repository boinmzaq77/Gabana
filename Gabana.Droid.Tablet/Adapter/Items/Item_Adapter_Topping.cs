using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabana.Model;
using Gabana.Droid.Tablet.Fragments.Items;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Adapter.Items
{
    internal class Item_Adapter_Topping : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        public string positionClick;
        ListViewItemHolder vh;
        public static ListViewItemHolder vhselect;
        public bool CheckNet;

        public Item_Adapter_Topping(ListItem l, bool c)
        {
            int index = -1;
            index = l.items.FindIndex(x => x.SysItemID == 0);
            if (index > -1)
            {
                l.items.RemoveAt(index);
            }
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
                                
                if (!string.IsNullOrEmpty(listitem[position].ItemName))
                {
                    vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                    vh.ItemName.Text = listitem[position].ItemName?.ToString();

                    if (listitem[position].SysCategoryID != null)
                    {
                        var categoryid = listitem[position].SysCategoryID?.ToString();
                        category = Item_Fragment_Main.lstCategory.Where(x => x.SysCategoryID.ToString() == categoryid).FirstOrDefault();
                        if (category != null)
                        {
                            vh.CategoryItem.Text = category.Name;
                        }
                        else
                        {
                            category = null;
                            vh.CategoryItem.Text = string.Empty;
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
                }
                else
                {
                    vh.CardAdapter.Visibility = ViewStates.Gone;
                }

                vh.lnFavorite.Click += delegate
                {
                    try
                    {
                        FavClick(position);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("lnFavorite at Item");
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
                        _ = TinyInsights.TrackPageViewAsync("Favorite at Item");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };

                vh.ColorItemView.Click += delegate
                {
                    if (!string.IsNullOrEmpty(cloudpath) && !string.IsNullOrEmpty(localpath))
                    {
                        MainActivity.OpenDialogImage(listitem[position]);
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                _ = TinyInsights.TrackPageViewAsync("vh.Favorite.Click at Item");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_topping, parent, false);
            ListViewItemHolder vh = new ListViewItemHolder(itemView, OnClick);

            TextView textSignMoney = itemView.FindViewById<TextView>(Resource.Id.textSignMoney);
            textSignMoney.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

        private void LnFavoriteClick(long? itemId)
        {
            Item_Fragment_Main.fragment_main.ShowToppingFav(itemId);
        }

    }

    internal class Item_Adapter_ToppingViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}