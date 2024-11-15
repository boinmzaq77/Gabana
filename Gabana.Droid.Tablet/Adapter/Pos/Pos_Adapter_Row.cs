using Android.App;
using Android.Content;
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

namespace Gabana.Droid.Tablet.Adapter.POS
{
    public class Pos_Adapter_Row : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItem listitem;
        public ListCategory listCategory;
        public int positionClick;
        View itemView;
        ListViewItemHolder vh;
        CardView cardViewRow;
        public bool CheckNet;

        public Pos_Adapter_Row(ListItem l, bool c)
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

                vh.IsRecyclable = false;

                var category = new ORM.MerchantDB.Category();
                if (listitem[position].SysItemID == 0)
                {
                    if (POS_Fragment_Main.favoriteMenu)
                    {
                        cardViewRow.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        vh.frameAdd.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    vh.frameAdd.Visibility = ViewStates.Gone;

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
                                vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                                vh.ItemName.Text = listitem[position].ItemName?.ToString();
                            }
                            else
                            {
                                //cloud
                                Utils.SetImage(vh.ColorItemView, cloudpath);
                                vh.ItemName.Text = listitem[position].ItemName?.ToString();
                                vh.ItemNameView.Text = string.Empty;
                            }
                        }
                        else
                        {
                            //local  
                            Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                            vh.ColorItemView.SetImageURI(uri);
                            vh.ItemName.Text = listitem[position].ItemName?.ToString();
                            vh.ItemNameView.Text = string.Empty;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(localpath))
                        {
                            Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                            vh.ColorItemView.SetImageURI(uri);
                            vh.ItemName.Text = listitem[position].ItemName?.ToString();
                            vh.ItemNameView.Text = string.Empty;
                        }
                        else
                        {
                            string conColor = Utils.SetBackground(Convert.ToInt32(listitem[position].Colors));
                            var color = Android.Graphics.Color.ParseColor(conColor);
                            vh.ColorItemView.SetImageURI(null);
                            vh.ItemNameView.Visibility = ViewStates.Visible;
                            vh.ColorItemView.SetBackgroundColor(color);
                            vh.ItemNameView.Text = listitem[position].ItemName?.ToString();
                            vh.ItemName.Text = listitem[position].ItemName?.ToString();
                        }
                    }

                    if (POS_Fragment_Main.tranWithDetails.tranDetailItemWithToppings.Count > 0)
                    {
                        var itemId = listitem[position].SysItemID;
                        TranDetailItemWithTopping Item = new TranDetailItemWithTopping();
                        List<TranDetailItemWithTopping> lstItem = new List<TranDetailItemWithTopping>();

                        Item = POS_Fragment_Main.tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == itemId).FirstOrDefault();
                        if (Item != null)
                        {
                            Android.Text.SpannableStringBuilder builder = new Android.Text.SpannableStringBuilder();

                            string Quantity = "0";
                            lstItem = POS_Fragment_Main.tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == itemId).ToList();
                            if (lstItem.Count > 0)
                            {
                                Quantity = (int)lstItem.Sum(x => x.tranDetailItem.Quantity) + "x ";
                            }

                            Android.Text.SpannableString redSpannable = new Android.Text.SpannableString(Quantity);
                            redSpannable.SetSpan(new Android.Text.Style.ForegroundColorSpan(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null)), 0, Quantity.Length, 0);
                            builder.Append(redSpannable);

                            string itemName = listitem[position].ItemName?.ToString();
                            Android.Text.SpannableString Spannable = new Android.Text.SpannableString(itemName);
                            Spannable.SetSpan(new Android.Text.Style.ForegroundColorSpan(Application.Context.Resources.GetColor(Resource.Color.eclipse, null)), 0, itemName.Length, 0);
                            builder.Append(Spannable);

                            vh.ItemName.SetText(builder, TextView.BufferType.Spannable);
                        }
                    }

                    if (listitem[position].SysCategoryID == null)
                    {
                        category = null;
                        vh.CategoryItem.Text = string.Empty;
                    }
                    else
                    {
                        var categoryid = listitem[position].SysCategoryID;
                        category = POS_Fragment_Main.lstCategory.Where(x => x.SysCategoryID == categoryid).FirstOrDefault();
                        vh.CategoryItem.Text = category.Name;
                    }

                    vh.ItemPrice.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(listitem[position].Price);

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
                return;
            }
        }



        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.pos_adapter_menurow, parent, false);
            vh = new ListViewItemHolder(itemView, OnClick);
            cardViewRow = itemView.FindViewById<CardView>(Resource.Id.cardViewrow);
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

        //private void ItemView_LongClick(long itemID)
        //{
        //    try
        //    {
        //        long itemIDclick = itemID;
        //        PosActivity.pos.LongItemClick(itemIDclick);
        //    }
        //    catch (Exception ex)
        //    {
        //        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
        //        return;
        //    }
        //}
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
        public ListViewItemHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ColorItemView = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            PicItemView = itemview.FindViewById<ImageView>(Resource.Id.imagePicItem);
            //PicItemViewRow = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            ItemNameView = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            ItemName = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryItem);
            ItemPrice = itemview.FindViewById<TextView>(Resource.Id.txtItemPrice);
            //PriceTotal = itemview.FindViewById<TextView>(Resource.Id.txtPriceTotal);
            //PriceBeforeDis = itemview.FindViewById<TextView>(Resource.Id.txtPriceBeforeDis);
            Alert = itemview.FindViewById<ImageView>(Resource.Id.imagealert);
            //CountItem = itemview.FindViewById<TextView>(Resource.Id.textCountITem);
            frameAdd = itemview.FindViewById<FrameLayout>(Resource.Id.frameAdd);
            imgShowAnime = itemview.FindViewById<ImageView>(Resource.Id.imgShowAnime);

            //lnEditItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnEditItem);
            //lnQuantity = itemview.FindViewById<LinearLayout>(Resource.Id.lnQuantity);
            //lnOption = itemview.FindViewById<LinearLayout>(Resource.Id.lnOption);
            //lnChangePrice = itemview.FindViewById<LinearLayout>(Resource.Id.lnPrice);
            //lnDiscount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscount);
            //lnDelete = itemview.FindViewById<LinearLayout>(Resource.Id.lnDelete);
            //OptionList = itemview.FindViewById<RelativeLayout>(Resource.Id.lnOptionlist);
            //Discount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscountItem);
            //DiscountItem = itemview.FindViewById<TextView>(Resource.Id.textItemDiscount);
            //lnFavorite = itemview.FindViewById<LinearLayout>(Resource.Id.lnFav);
            //Favorite = itemview.FindViewById<ImageButton>(Resource.Id.btnFavorite);
            //DiscountRowImage = itemview.FindViewById<ImageView>(Resource.Id.imageDiscountRow);

            //lnComment = itemview.FindViewById<RelativeLayout>(Resource.Id.lnCommentItem);
            //Comment = itemview.FindViewById<TextView>(Resource.Id.textCommentItem);

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