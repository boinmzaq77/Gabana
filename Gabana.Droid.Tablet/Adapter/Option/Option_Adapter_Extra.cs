using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Adapter.Option
{
    internal class Option_Adapter_Extra : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListItem lstItemTopping;
        int Tranindex = 0, ToppingNo = 0;
        public TranWithDetailsLocal tranWithDetails;
        ListViewExtraDataHolder vh;
        TranDetailItemTopping topping;
        List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
        public static Option_Adapter_Extra _Option_Adapter_Extra;
        public Option_Adapter_Extra(ListItem l)
        {
            lstItemTopping = l;
        }
        public override int ItemCount
        {
            get { return lstItemTopping == null ? 0 : lstItemTopping.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                _Option_Adapter_Extra = this;
                vh = holder as ListViewExtraDataHolder;
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;

                vh.textToppingName.Text = lstItemTopping[position].ItemName?.ToString();
                vh.btnReExtra.Alpha = (float)0.2;
                vh.txtUnitExtra.Text = "1";
                vh.txtUnitExtra.Alpha = (float)0.2;
                vh.btnAddExtra.Alpha = (float)0.2;
                vh.textPriceExtra.Text = "+" + Utils.DisplayDecimal(lstItemTopping[position].Price);

                vh.txtUnitExtra.Clickable = false;
                #region tranWithDetails
                ////////// tranWithDetails /////////////
                tranWithDetails = POS_Dialog_Option.tranWithDetails;
                TranDetailItemWithTopping tranDetailItemWithToppings = new TranDetailItemWithTopping();

                //Update Topping
                if (POS_Dialog_Option.POSDataItem == null)
                {
                    if(POS_Dialog_Scan.scan != null)
                    {
                        tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == POS_Dialog_Option.sysitemId && x.tranDetailItem.DetailNo == POS_Dialog_Scan.detailNoClickOption).FirstOrDefault();
                    }
                    else
                    {
                        tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == POS_Dialog_Option.sysitemId && x.tranDetailItem.DetailNo == POS_Fragment_Cart.DetailNo).FirstOrDefault();
                    }

                    if (tranDetailItemWithToppings != null)
                    {
                        foreach (var item in tranDetailItemWithToppings.tranDetailItemToppings)
                        {
                            var oldTopping = POS_Dialog_Option.lstTranSelectTopping.FindIndex(x => x.SysItemID == item.SysItemID);
                            if (oldTopping == -1)
                            {
                                POS_Dialog_Option.lstTranSelectTopping.Add(item);
                                POS_Dialog_Option.lstTempTopping.Add(item);
                            }
                        }
                        tranDetailItemWithToppings.tranDetailItemToppings.RemoveRange(0, tranDetailItemWithToppings.tranDetailItemToppings.Count);
                    }
                }
                #endregion

                if (POS_Dialog_Option.lstTranSelectTopping != null)
                {
                    foreach (var item in POS_Dialog_Option.lstTranSelectTopping)
                    {
                        if (item.SysItemID == lstItemTopping[position].SysItemID)
                        {
                            vh.btnSelectExtra.SetBackgroundResource(Resource.Mipmap.OptionCheck);
                            vh.btnReExtra.Alpha = (float)1;
                            vh.txtUnitExtra.Text = item.Quantity.ToString();
                            vh.textPriceExtra.Text = "+" + Utils.DisplayDecimal(lstItemTopping[position].Price);
                            vh.txtUnitExtra.Alpha = (float)1;
                            vh.btnAddExtra.Alpha = (float)1;
                        }
                    }
                }

                vh.ItemView.Click += (s, e) =>
                {
                    try
                    {
                        TranDetailItemTopping data = POS_Dialog_Option.lstTranSelectTopping.Where(x => x.SysItemID == POS_Dialog_Option.sysitemIDToppping).FirstOrDefault();
                        if (data != null)
                        {
                            if (POS_Dialog_Option.sysitemIDToppping == data.SysItemID)
                            {
                                UnSelectExtra();
                                return;
                            }
                        }
                        else
                        {
                            if (POS_Dialog_Option.sysitemIDToppping == lstItemTopping[position].SysItemID)
                            {
                                Tranindex = position;
                                SelectExtra();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("ItemView at employee)adapter");
                    }
                };

                if (!vh.btnAddExtra.HasOnClickListeners)
                {
                    vh.btnReExtra.Click += (s, e) =>
                    {
                        try
                        {
                            POS_Dialog_Option.sysitemIDToppping = lstItemTopping[position].SysItemID;
                            Tranindex = position;
                            TranDetailItemTopping data = POS_Dialog_Option.lstTranSelectTopping.Where(x => x.SysItemID == POS_Dialog_Option.sysitemIDToppping).FirstOrDefault();
                            if (data != null)
                            {
                                if (POS_Dialog_Option.sysitemIDToppping == data.SysItemID)
                                {
                                    if (data.Quantity > 1)
                                    {
                                        int MinusQuantity = (int)data.Quantity - 1;
                                        AddToTran(MinusQuantity);
                                        Option_Adapter_Extra._Option_Adapter_Extra.OnBindViewHolder(holder, position);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("btnReExtra at opteion_extra");
                            Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        }
                    };
                }

                if (!vh.btnAddExtra.HasOnClickListeners)
                {
                    vh.btnAddExtra.Click += (o, e) =>
                    {
                        try
                        {
                            POS_Dialog_Option.sysitemIDToppping = lstItemTopping[position].SysItemID;
                            Tranindex = position;
                            TranDetailItemTopping data = POS_Dialog_Option.lstTranSelectTopping.Where(x => x.SysItemID == POS_Dialog_Option.sysitemIDToppping).FirstOrDefault();
                            if (data != null)
                            {
                                if (POS_Dialog_Option.sysitemIDToppping == data.SysItemID)
                                {
                                    int PlusQuantity = (int)data.Quantity + 1;
                                    AddToTran(PlusQuantity);
                                    Option_Adapter_Extra._Option_Adapter_Extra.OnBindViewHolder(holder, position);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("btnAddExtra at opteion_extra");
                            Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBindViewHolder at opteion_extra");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        void SelectExtra()
        {
            vh.btnSelectExtra.SetBackgroundResource(Resource.Mipmap.OptionCheck);
            vh.btnReExtra.Alpha = (float)1;
            vh.txtUnitExtra.Alpha = (float)1;
            vh.btnAddExtra.Alpha = (float)1;
            AddToTran(1); //เลือกแค่ที่เซต Default มาให้ จำนวน = 1
        }
        void UnSelectExtra()
        {
            vh.btnSelectExtra.SetBackgroundResource(Resource.Mipmap.OptionBlank);
            vh.btnReExtra.Alpha = (float)0.2;
            vh.txtUnitExtra.Text = "1";
            vh.txtUnitExtra.Alpha = (float)0.2;
            vh.btnAddExtra.Alpha = (float)0.2;
            DropFromTran();
        }
        void DropFromTran()
        {
            try
            {
                if (POS_Dialog_Option.lstTranSelectTopping.Count > 0)
                {
                    var row = POS_Dialog_Option.lstTranSelectTopping.FindIndex(x => x.SysItemID == POS_Dialog_Option.sysitemIDToppping);
                    POS_Dialog_Option.lstTranSelectTopping.RemoveAt(row);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DropFromTran at opteion_extra");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        void AddToTran(int Quantity)
        {
            try
            {
                //เลือกท้อปปิ้งครั้งแรก จำนวน เท่ากับ 1
                var row = POS_Dialog_Option.lstTranSelectTopping.FindIndex(x => x.SysItemID == POS_Dialog_Option.sysitemIDToppping);
                if (row == -1)
                {
                    //Quantity;
                    if (POS_Dialog_Option.lstTranSelectTopping.Count == 0)
                    {
                        ToppingNo = POS_Dialog_Option.lstTranSelectTopping.Count + 1;
                    }
                    else
                    {
                        ToppingNo = (int)POS_Dialog_Option.lstTranSelectTopping.Max(x => x.ToppingNo) + 1;
                    }

                    topping = new TranDetailItemTopping()
                    {
                        ItemName = lstItemTopping[Tranindex].ItemName,//toppping
                        SysItemID = lstItemTopping[Tranindex].SysItemID,
                        UnitName = lstItemTopping[Tranindex].UnitName,
                        RegularSizeName = lstItemTopping[Tranindex].RegularSizeName,
                        Quantity = Quantity,
                        ToppingPrice = lstItemTopping[Tranindex].Price,
                        EstimateCost = lstItemTopping[Tranindex].EstimateCost,
                        Comments = lstItemTopping[Tranindex].Comments,
                        ToppingNo = ToppingNo,
                    };
                    POS_Dialog_Option.lstTranSelectTopping.Add(topping);
                }
                else //เลือกท้อปปิ้งแล้ว มีการเพิ่มจำนวนท้อปปิ้งที่เลือก
                {
                    var detail = POS_Dialog_Option.lstTranSelectTopping.Where(x => x.SysItemID == POS_Dialog_Option.sysitemIDToppping).FirstOrDefault();
                    if (detail != null)
                    {
                        detail.Quantity = Quantity;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddToTran at opteion_extra");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.option_adapter_extra, parent, false);
            ListViewExtraDataHolder vh = new ListViewExtraDataHolder(itemView, OnClick);
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
    public class ListViewExtraDataHolder : RecyclerView.ViewHolder
    {
        public TextView textToppingName { get; set; }
        public TextView txtUnitExtra { get; set; }
        public TextView textPriceExtra { get; set; }
        public ImageButton btnSelectExtra { get; set; }
        public FrameLayout btnReExtra { get; set; }
        public FrameLayout btnAddExtra { get; set; }


        public ListViewExtraDataHolder(View itemview, Action<int> listener) : base(itemview)
        {
            textToppingName = itemview.FindViewById<TextView>(Resource.Id.textToppingName);
            txtUnitExtra = itemview.FindViewById<TextView>(Resource.Id.txtUnitExtra);
            textPriceExtra = itemview.FindViewById<TextView>(Resource.Id.textPriceExtra);
            btnSelectExtra = itemview.FindViewById<ImageButton>(Resource.Id.btnSelectExtra);
            btnReExtra = itemview.FindViewById<FrameLayout>(Resource.Id.btnReExtra);
            btnAddExtra = itemview.FindViewById<FrameLayout>(Resource.Id.btnAddExtra);
            itemview.Click += (sender, e) => listener(base.Position);
        }
    }

}