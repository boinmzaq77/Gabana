using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid.Adapter
{
    public class Option_Adapter_Extra : RecyclerView.Adapter
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
                tranWithDetails = OptionActivity.tranWithDetails;
                TranDetailItemWithTopping tranDetailItemWithToppings = new TranDetailItemWithTopping();

                //Update Topping
                if (OptionActivity.POSDataItem == null)
                {
                    if (CartActivity.CurrentActivity)
                    {
                        tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == OptionActivity.sysitemId && x.tranDetailItem.DetailNo == CartActivity.DetailNo).FirstOrDefault();
                    }
                    else
                    {
                        tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == OptionActivity.sysitemId && x.tranDetailItem.DetailNo == CartScanActivity.detailNoClickOption).FirstOrDefault();
                    }

                    if (tranDetailItemWithToppings != null)
                    {
                        foreach (var item in tranDetailItemWithToppings.tranDetailItemToppings)
                        {
                            var oldTopping = OptionActivity.lstTranSelectTopping.FindIndex(x => x.SysItemID == item.SysItemID);
                            if (oldTopping == -1)
                            {
                                OptionActivity.lstTranSelectTopping.Add(item);
                                OptionActivity.lstTempTopping.Add(item);
                            }
                        }
                        tranDetailItemWithToppings.tranDetailItemToppings.RemoveRange(0, tranDetailItemWithToppings.tranDetailItemToppings.Count);
                    }
                }
                #endregion

                if (OptionActivity.lstTranSelectTopping != null)
                {
                    foreach (var item in OptionActivity.lstTranSelectTopping)
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
                        TranDetailItemTopping data = OptionActivity.lstTranSelectTopping.Where(x => x.SysItemID == OptionActivity.sysitemIDToppping).FirstOrDefault();
                        if (data != null)
                        {
                            if (OptionActivity.sysitemIDToppping == data.SysItemID)
                            {
                                UnSelectExtra();
                                return;
                            }
                        }
                        else
                        {
                            if (OptionActivity.sysitemIDToppping == lstItemTopping[position].SysItemID)
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
                            OptionActivity.sysitemIDToppping = lstItemTopping[position].SysItemID;
                            Tranindex = position;
                            TranDetailItemTopping data = OptionActivity.lstTranSelectTopping.Where(x => x.SysItemID == OptionActivity.sysitemIDToppping).FirstOrDefault();
                            if (data != null)
                            {
                                if (OptionActivity.sysitemIDToppping == data.SysItemID)
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
                            OptionActivity.sysitemIDToppping = lstItemTopping[position].SysItemID;
                            Tranindex = position;
                            TranDetailItemTopping data = OptionActivity.lstTranSelectTopping.Where(x => x.SysItemID == OptionActivity.sysitemIDToppping).FirstOrDefault();
                            if (data != null)
                            {
                                if (OptionActivity.sysitemIDToppping == data.SysItemID)
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
                if (OptionActivity.lstTranSelectTopping.Count > 0)
                {
                    var row = OptionActivity.lstTranSelectTopping.FindIndex(x => x.SysItemID == OptionActivity.sysitemIDToppping);
                    OptionActivity.lstTranSelectTopping.RemoveAt(row);
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
                var row = OptionActivity.lstTranSelectTopping.FindIndex(x => x.SysItemID == OptionActivity.sysitemIDToppping);
                if (row == -1)
                {
                    //Quantity;
                    if (OptionActivity.lstTranSelectTopping.Count == 0)
                    {
                        ToppingNo = OptionActivity.lstTranSelectTopping.Count + 1;
                    }
                    else
                    {
                        ToppingNo = (int)OptionActivity.lstTranSelectTopping.Max(x => x.ToppingNo) + 1;
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
                    OptionActivity.lstTranSelectTopping.Add(topping);
                }
                else //เลือกท้อปปิ้งแล้ว มีการเพิ่มจำนวนท้อปปิ้งที่เลือก
                {
                    var detail = OptionActivity.lstTranSelectTopping.Where(x => x.SysItemID == OptionActivity.sysitemIDToppping).FirstOrDefault();
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
}