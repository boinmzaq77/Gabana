using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.DataModel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace Gabana.Droid.Adapter
{
    public class Pos_Adapter_Grid : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        //CardView cardViewGrid;
        //ListViewItemHolder vh;
        public bool CheckNet;

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

        public Pos_Adapter_Grid(ListItem l, bool c)
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
                #region OldCode
                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                ListViewItemHolder vh = holder as ListViewItemHolder;
               
                if (listitem[position].SysItemID == 0)
                {
                    vh.frameAdd.Visibility = PosActivity.favoriteMenu ? ViewStates.Gone : ViewStates.Visible;
                }
                else
                {
                    vh.frameAdd.Visibility = ViewStates.Gone;

                    var cloudpath = listitem[position].PicturePath ?? string.Empty;
                    var localpath = listitem[position].ThumbnailLocalPath ?? string.Empty;

                    SetImageView(vh, cloudpath, localpath, listitem[position].Colors, listitem[position].ItemName);

                    vh.ItemName.Text = listitem[position].ItemName?.ToString();

                    if (PosActivity.tranWithDetails.tranDetailItemWithToppings.Count > 0)
                    {
                        var itemId = listitem[position].SysItemID;
                        var quantity = PosActivity.tranWithDetails.tranDetailItemWithToppings
                            .Where(x => x.tranDetailItem.SysItemID == itemId)
                            .Sum(x => x.tranDetailItem.Quantity);

                        if (quantity > 0)
                        {
                            vh.ItemName.Text = $"{quantity}x {listitem[position].ItemName}";
                        }
                    }

                    vh.ItemPrice.Text = $"{CURRENCYSYMBOLS}{Utils.DisplayDecimal(listitem[position].Price)}";

                    SetStockIndicator(vh, listitem[position].FTrackStock, listitem[position].SysItemID);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void SetStockIndicator(ListViewItemHolder vh, decimal trackStock, long sysItemId)
        {
            #region Aleart Stock
            //กำหนด MinimumStock ระบบจะแสดงเป็น Indicater(วงกลม Notification) สีเหลืองเมื่อต่ำกว่า MinimumStock
            //สีแดง สินค้า =0 หรือติดลบ
            if (trackStock == 1)
            {
                ItemOnBranchManage itemOnBranchManage = new ItemOnBranchManage();
                ORM.MerchantDB.ItemOnBranch stock = new ORM.MerchantDB.ItemOnBranch();
                stock = await itemOnBranchManage.GetItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, (int)sysItemId);
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

        private void SetImageView(ListViewItemHolder vh, string cloudpath, string localpath, long? colors,string itemname)
        {
            #region OldCode
            if (CheckNet)
            {
                if (string.IsNullOrEmpty(localpath))
                {
                    if (string.IsNullOrEmpty(cloudpath))
                    {
                        //defalut
                        vh.PicItemView.SetImageURI(null);
                        string conColor = Utils.SetBackground(Convert.ToInt32(colors));
                        var color = Android.Graphics.Color.ParseColor(conColor);
                        vh.ColorItemView.SetImageURI(null);
                        vh.lnColorBottom.SetBackgroundColor(Android.Graphics.Color.ParseColor("#33000000"));
                        vh.ItemNameView.Visibility = ViewStates.Visible;
                        vh.ColorItemView.SetBackgroundColor(color);
                        vh.ItemNameView.Text = itemname?.ToString();
                    }
                    else
                    {
                        //cloud
                        vh.lnColorBottom.SetBackgroundColor(Android.Graphics.Color.ParseColor("#66000000"));
                        Utils.SetImage(vh.PicItemView, cloudpath);
                        vh.ItemNameView.Visibility = ViewStates.Gone;
                        vh.ItemNameView.Text = string.Empty;
                    }
                }
                else
                {
                    //local  
                    Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                    vh.PicItemView.SetImageURI(uri);
                    vh.lnColorBottom.SetBackgroundColor(Android.Graphics.Color.ParseColor("#66000000"));
                    vh.ItemNameView.Visibility = ViewStates.Gone;
                    vh.ItemNameView.Text = string.Empty;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(localpath))
                {
                    vh.lnColorBottom.SetBackgroundColor(Android.Graphics.Color.ParseColor("#66000000"));
                    Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                    vh.PicItemView.SetImageURI(uri);
                    vh.ItemNameView.Visibility = ViewStates.Gone;
                    vh.ItemNameView.Text = string.Empty;
                }
                else
                {
                    vh.lnColorBottom.SetBackgroundColor(Android.Graphics.Color.ParseColor("#33000000"));
                    vh.PicItemView.SetImageURI(null);
                    string conColor = Utils.SetBackground(Convert.ToInt32(colors));
                    var color = Android.Graphics.Color.ParseColor(conColor);
                    vh.ColorItemView.SetImageURI(null);
                    vh.ItemNameView.Visibility = ViewStates.Visible;
                    vh.ColorItemView.SetBackgroundColor(color);
                    vh.ItemNameView.Text = itemname?.ToString();
                }
            }
            #endregion
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.pos_adapter_menugrid, parent, false);
            var vh = new ListViewItemHolder(itemView, OnClick);
            var cardViewGrid = itemView.FindViewById<CardView>(Resource.Id.cardViewGrid);

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;
            var widthCard = (width - 10) / 3;
            var heightimgCard = ((widthCard / 4) * 3);
            var heighttextCard = heightimgCard / 2;
            var heigtCard = heightimgCard + heighttextCard;
            cardViewGrid.LayoutParameters.Height = Convert.ToInt32(Convert.ToDecimal(heigtCard).ToString("##"));
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