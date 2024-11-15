using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using TinyInsightsLib;
using Gabana.Droid.Tablet.Fragments.Items;
using AndroidX.RecyclerView.Widget;
using AndroidX.CardView.Widget;

namespace Gabana.Droid.Tablet.Adapter.Items
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
                        MainActivity.OpenDialogImage(listitem[position]);
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
            Item_Fragment_Main.fragment_main.ShowItemFav(itemId);
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

    public class ListViewItemHolder : RecyclerView.ViewHolder
    {
        public ImageView ColorItemView { get; set; }
        public ImageView PicItemView { get; set; }
        public ImageView PicItemViewRow { get; set; }
        public TextView ItemNameView { get; set; }
        public TextView ItemName { get; set; }
        public TextView CategoryItem { get; set; }
        public TextView ItemPrice { get; set; }
        public TextView PriceTotal { get; set; }
        public TextView PriceBeforeDis { get; set; }
        public ImageView Alert { get; set; }
        public TextView CountItem { get; set; }
        public LinearLayout lnEditItem { get; set; }
        public FrameLayout frameAdd { get; set; }
        public ImageView imgShowAnime { get; set; }
        public LinearLayout lnQuantity { get; set; }
        public LinearLayout lnOption { get; set; }
        public LinearLayout lnChangePrice { get; set; }
        public LinearLayout lnDiscount { get; set; }
        public LinearLayout lnDelete { get; set; }
        public RelativeLayout OptionList { get; set; }
        public LinearLayout lnFavorite { get; set; }
        public ImageButton Favorite { get; set; }
        public ImageView DiscountRowImage { get; set; }
        public RelativeLayout lnComment { get; set; }
        public LinearLayout Discount { get; set; }
        public TextView Comment { get; set; }
        public TextView DiscountItem { get; set; }
        public LinearLayout lnColorBottom { get; set; }
        public LinearLayout lnStatusD { get; set; }
        public Button btnDeleteItemD { get; set; }

        public CardView CardAdapter { get; set; }

        public ListViewItemHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ColorItemView = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            PicItemView = itemview.FindViewById<ImageView>(Resource.Id.imagePicItem);
            ItemNameView = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            ItemName = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryItem);
            ItemPrice = itemview.FindViewById<TextView>(Resource.Id.txtItemPrice);
            PriceTotal = itemview.FindViewById<TextView>(Resource.Id.txtPriceTotal);
            PriceBeforeDis = itemview.FindViewById<TextView>(Resource.Id.txtPriceBeforeDis);
            Alert = itemview.FindViewById<ImageView>(Resource.Id.imagealert);
            CountItem = itemview.FindViewById<TextView>(Resource.Id.textCountITem);
            frameAdd = itemview.FindViewById<FrameLayout>(Resource.Id.frameAdd);
            imgShowAnime = itemview.FindViewById<ImageView>(Resource.Id.imgShowAnime);
            lnEditItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnEditItem);
            lnQuantity = itemview.FindViewById<LinearLayout>(Resource.Id.lnQuantity);
            lnOption = itemview.FindViewById<LinearLayout>(Resource.Id.lnOption);
            lnChangePrice = itemview.FindViewById<LinearLayout>(Resource.Id.lnPrice);
            lnDiscount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscount);
            lnDelete = itemview.FindViewById<LinearLayout>(Resource.Id.lnDelete);
            OptionList = itemview.FindViewById<RelativeLayout>(Resource.Id.lnOptionlist);
            Discount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscountItem);
            DiscountItem = itemview.FindViewById<TextView>(Resource.Id.textItemDiscount);
            lnFavorite = itemview.FindViewById<LinearLayout>(Resource.Id.lnFav);
            Favorite = itemview.FindViewById<ImageButton>(Resource.Id.btnFavorite);
            DiscountRowImage = itemview.FindViewById<ImageView>(Resource.Id.imageDiscountRow);

            lnComment = itemview.FindViewById<RelativeLayout>(Resource.Id.lnCommentItem);
            Comment = itemview.FindViewById<TextView>(Resource.Id.textCommentItem);

            CardAdapter = itemview.FindViewById<CardView>(Resource.Id.CardAdapter);
            itemview.Click += (sender, e) => listener(base.Position);
        }

        public void Select()
        {
            Favorite.SetBackgroundResource(Resource.Mipmap.Fav);
        }

        public void NotSelect()
        {
            Favorite.SetBackgroundResource(Resource.Mipmap.Unfav);
        }
    }

}