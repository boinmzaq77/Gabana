using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Adapter.POS
{
    public class Pos_Adapter_Grid : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        CardView cardViewGrid;
        ListViewItemHolder vh;
        View itemView;
        Color color;
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

        public async override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                ListViewItemHolder vh = holder as ListViewItemHolder;
                if (listitem[position].SysItemID == 0)
                {
                    if (POS_Fragment_Main.favoriteMenu)
                    {
                        cardViewGrid.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        vh.frameAdd.Visibility = ViewStates.Visible;
                    }
                }
                else
                {

                    vh.frameAdd.Visibility = ViewStates.Gone;
                    //var paths = listitem[position].ThumbnailLocalPath;
                    //if (!string.IsNullOrEmpty(paths))
                    //{
                    //    Android.Net.Uri uri = Android.Net.Uri.Parse(paths);
                    //    vh.PicItemView.SetImageURI(uri);
                    //    vh.ItemNameView.Visibility = ViewStates.Gone;
                    //    vh.ItemNameView.Text = string.Empty;
                    //}
                    //else
                    //{
                    //    vh.PicItemView.SetImageURI(null);
                    //    string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                    //    color = Android.Graphics.Color.ParseColor(conColor);
                    //    vh.ColorItemView.SetBackgroundColor(color);
                    //    vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                    //}

                    var cloudpath = listitem[position].PicturePath == null ? string.Empty : listitem[position].PicturePath;
                    var localpath = listitem[position].ThumbnailLocalPath == null ? string.Empty : listitem[position].ThumbnailLocalPath;

                    if (CheckNet)
                    {
                        if (string.IsNullOrEmpty(localpath))
                        {
                            if (string.IsNullOrEmpty(cloudpath))
                            {
                                //defalut
                                vh.PicItemView.SetImageURI(null);
                                string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                                var color = Android.Graphics.Color.ParseColor(conColor);
                                vh.ColorItemView.SetImageURI(null);
                                vh.ItemNameView.Visibility = ViewStates.Visible;
                                vh.ColorItemView.SetBackgroundColor(color);
                                vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                            }
                            else
                            {
                                //cloud
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
                            vh.ItemNameView.Visibility = ViewStates.Gone;
                            vh.ItemNameView.Text = string.Empty;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(localpath))
                        {
                            Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                            vh.PicItemView.SetImageURI(uri);
                            vh.ItemNameView.Visibility = ViewStates.Gone;
                            vh.ItemNameView.Text = string.Empty;
                        }
                        else
                        {
                            vh.PicItemView.SetImageURI(null);
                            string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                            var color = Android.Graphics.Color.ParseColor(conColor);
                            vh.ColorItemView.SetImageURI(null);
                            vh.ItemNameView.Visibility = ViewStates.Visible;
                            vh.ColorItemView.SetBackgroundColor(color);
                            vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                        }
                    }

                    vh.ItemName.Text = listitem[position].ItemName?.ToString();

                    if (MainActivity.tranWithDetails.tranDetailItemWithToppings.Count > 0)
                    {
                        var itemId = listitem[position].SysItemID;
                        TranDetailItemWithTopping Item = new TranDetailItemWithTopping();
                        Item = MainActivity.tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == itemId).FirstOrDefault();
                        if (Item != null)
                        {
                            vh.ItemName.Text = (int)Item.tranDetailItem.Quantity + "x " + listitem[position].ItemName?.ToString();
                        }
                    }

                    var categoryid = listitem[position].SysCategoryID?.ToString();
                    var category = POS_Fragment_Main.lstCategory.Where(x => x.SysCategoryID.ToString() == categoryid).FirstOrDefault();

                    vh.ItemPrice.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(listitem[position].Price);


                    #region Aleart Stock
                    //กำหนด MinimumStock ระบบจะแสดงเป็น Indicater(วงกลม Notification) สีเหลืองเมื่อต่ำกว่า MinimumStock
                    //สีแดง สินค้า =0 หรือติดลบ
                    if (listitem[position].FTrackStock == 1)
                    {
                        ItemOnBranchManage itemOnBranchManage = new ItemOnBranchManage();
                        ORM.MerchantDB.ItemOnBranch stock = new ORM.MerchantDB.ItemOnBranch();
                        stock = await itemOnBranchManage.GetItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, (int)listitem[position].SysItemID);
                        if (stock != null)
                        {
                            if (stock.BalanceStock > stock.MinimumStock)
                            {
                                vh.Alert.Visibility = ViewStates.Invisible;
                            }
                            else if (stock.BalanceStock <= stock.MinimumStock & stock.BalanceStock > 0)
                            {
                                vh.Alert.Visibility = ViewStates.Visible;
                                //vh.Alert.SetImageResource(Resource.Mipmap.IndicatorYellow);
                            }
                            else
                            {
                                //stock.BalanceStock <= 0
                                vh.Alert.Visibility = ViewStates.Visible;
                                //vh.Alert.SetImageResource(Resource.Mipmap.IndicatorRed);
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

                #region LongClick
                //vh.ItemView.LongClick += (s, e) =>
                //{
                //    if (listitem[position].SysItemID != 0)
                //    {
                //        var itemId = listitem[position].SysItemID;
                //        DataCashing.DialogShowItem = listitem[position];
                //        ItemView_LongClick(itemId);
                //    }
                //}; 
                #endregion
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.pos_adapter_menugrid, parent, false);
            vh = new ListViewItemHolder(itemView, OnClick);
            cardViewGrid = itemView.FindViewById<CardView>(Resource.Id.cardViewGrid);

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var width = mainDisplayInfo.Width;
            var height = mainDisplayInfo.Height;
            var widthCard = (width - 20) / 6;
            var heightimgCard = ((widthCard / 4) * 3);
            var heighttextCard = heightimgCard / 3;
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