using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Bill;
using Gabana.ShareSource;
using Java.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Adapter.Bill
{
    internal class Bill_Adapter_Main : RecyclerView.Adapter
    {
        ListBillHistoryNew lstBill;
        public event EventHandler<int> ItemClick;
        string temp;

        public Bill_Adapter_Main(ListBillHistoryNew l)
        {
            lstBill = l;
        }

        public async void refresh(ListBillHistoryNew newBillHistoryList)
        {
            try
            {
                //lstBill = l;

                lstBill = new ListBillHistoryNew(
                   newBillHistoryList.Trans.OrderByDescending(x => x.tranDate).ThenByDescending(x => x.tranNo).ToList()
                );

                if (await GabanaAPI.CheckNetWork())
                {
                    NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public override int ItemCount
        {
            get { return lstBill == null ? 0 : lstBill.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                #region OldCode
                //ListViewBillHistoryHolder vh = holder as ListViewBillHistoryHolder;

                //CultureInfo cultureInfo = new CultureInfo("en-US");
                //var timezoneslocal = TimeZoneInfo.Local;
                //DateTime dateTime = lstBill.Trans[position].tranDate;
                //var TranDate = lstBill.Trans[position].tranDate;
                //string date = Utils.ShowDate(TranDate);

                //DateTime DateTranDate = TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal);

                //string timer = TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal).ToString("HH:mm tt", new CultureInfo("en-US"));

                //string datenow = DateTime.Today.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                //DateTime Now = DateTime.ParseExact(datenow, "dd/MM/yyyy", new CultureInfo("en-US"));

                ////2024-04-10 23:59:45.01791 UTC จะเป็น 2024-04-11 06:59:45.01791


                //if (DateTranDate.Date == Now.Date)
                //{
                //    vh.Day.Text = "Today  ,  " + TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal).ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                //}
                //else
                //{
                //    vh.Day.Text = TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal).ToString("ddd  ,  dd/MM/yyyy", new CultureInfo("en-US"));
                //}

                //vh.NumberDate.Text = date;
                //vh.BillNo.Text = lstBill.Trans[position].tranNo.ToString();
                //var price = lstBill.Trans[position].grandTotal;
                //vh.Price.Text = Utils.DisplayDecimal(price);
                //vh.Time.Text = timer;
                //vh.CustomerName.Text = lstBill.Trans[position].customerName;
                //Utils.SetPaymentImage(vh.PaymentIcon, lstBill.Trans[position].paymentType);

                //vh.StatusVoid.Text = Application.Context.GetString(Resource.String.billhistory_activity_void);
                //if (lstBill.Trans[position].fCancel == 1)
                //{
                //    vh.Price.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
                //    vh.StatusVoid.Visibility = ViewStates.Visible;
                //}
                //else
                //{
                //    vh.StatusVoid.Visibility = ViewStates.Gone;
                //}

                ////รอ fwait 
                //vh.StatusPending.Visibility = ViewStates.Gone;
                //if (lstBill.Trans[position].FWaiting == 1 || lstBill.Trans[position].FWaiting == 2)
                //{
                //    vh.StatusPending.Visibility = ViewStates.Visible;
                //}

                //if (temp == null)
                //{
                //    vh.Headerbar.Visibility = ViewStates.Visible;

                //    temp = date;
                //}
                //else if (date != temp)
                //{
                //    vh.Headerbar.Visibility = ViewStates.Visible;
                //    temp = date;
                //}
                //else if (datenow == temp)
                //{
                //    vh.Headerbar.Visibility = ViewStates.Gone;
                //    temp = date;
                //}
                //else
                //{
                //    vh.Headerbar.Visibility = ViewStates.Gone;
                //}

                //int countItem = lstBill.Trans.Count - 1;
                //if (countItem == position)
                //{
                //    OnCardCellbtnIndex5?.Invoke();
                //}
                #endregion


                #region newcode
                var vh = holder as ListViewBillHistoryHolder;

                // ตั้งค่าข้อมูลทางวัฒนธรรม
                var cultureInfo = new CultureInfo("en-US");
                var timezoneslocal = TimeZoneInfo.Local;

                // ดึงข้อมูลการทำธุรกรรม
                var tranDate = lstBill.Trans[position].tranDate;
                string date = Utils.ShowDate(tranDate);
                DateTime dateTranDate = TimeZoneInfo.ConvertTimeFromUtc(tranDate, timezoneslocal);
                string timer = dateTranDate.ToString("HH:mm tt", cultureInfo);

                string datenow = DateTime.Today.ToString("dd/MM/yyyy", cultureInfo);
                DateTime now = DateTime.ParseExact(datenow, "dd/MM/yyyy", cultureInfo);

                // ตั้งค่า View ต่างๆ
                vh.Day.Text = dateTranDate.Date == now.Date
                    ? "Today  ,  " + dateTranDate.ToString("dd/MM/yyyy", cultureInfo)
                    : dateTranDate.ToString("ddd  ,  dd/MM/yyyy", cultureInfo);

                vh.NumberDate.Text = date;
                vh.BillNo.Text = lstBill.Trans[position].tranNo.ToString();
                var price = lstBill.Trans[position].grandTotal;
                vh.Price.Text = Utils.DisplayDecimal(price);
                vh.Time.Text = timer;
                vh.CustomerName.Text = lstBill.Trans[position].customerName;
                Utils.SetPaymentImage(vh.PaymentIcon, lstBill.Trans[position].paymentType);

                vh.StatusVoid.Text = Application.Context.GetString(Resource.String.billhistory_activity_void);
                if (lstBill.Trans[position].fCancel == 1)
                {
                    vh.Price.PaintFlags = PaintFlags.StrikeThruText;
                    vh.StatusVoid.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.StatusVoid.Visibility = ViewStates.Gone;
                }

                // รอ fwait 
                vh.StatusPending.Visibility = ViewStates.Gone;
                if (lstBill.Trans[position].FWaiting == 1 || lstBill.Trans[position].FWaiting == 2)
                {
                    vh.StatusPending.Visibility = ViewStates.Visible;
                }

                // จัดการ Header bar
                if (position == 0 || !Utils.ShowDate(lstBill.Trans[position - 1].tranDate).Equals(date))
                {
                    vh.Headerbar.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.Headerbar.Visibility = ViewStates.Gone;
                }


                int countItem = lstBill.Trans.Count - 1;
                if (countItem == position)
                {
                    // เรียก event หรือ method ที่เกี่ยวข้องที่นี่
                    OnCardCellbtnIndex5?.Invoke();
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetBillHistory at Order");
                Toast.MakeText(BillHistory_Fragment_Main.fragment_main.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.bill_adapter_main, parent, false);
                ListViewBillHistoryHolder vh = new ListViewBillHistoryHolder(itemView, OnClick);
                TextView txtCurrency = itemView.FindViewById<TextView>(Resource.Id.txtCurrency);
                txtCurrency.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                return vh;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetBillHistory at Order");
                Toast.MakeText(BillHistory_Fragment_Main.fragment_main.Activity, ex.Message, ToastLength.Short).Show();
                return null;
            }
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
        public delegate void CardCellQRCodeIndexDelegate5();
        public event CardCellQRCodeIndexDelegate5 OnCardCellbtnIndex5;


    }
    public class ListViewBillHistoryHolder : RecyclerView.ViewHolder
    {
        public TextView Day { get; set; }
        public TextView NumberDate { get; set; }
        public ImageView PaymentIcon { get; set; }
        public TextView BillNo { get; set; }
        public TextView Price { get; set; }
        public TextView Time { get; set; }
        public TextView CustomerName { get; set; }
        public LinearLayout Headerbar { get; set; }
        public TextView StatusVoid { get; set; }
        public TextView StatusPending { get; set; }


        public ListViewBillHistoryHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Day = itemview.FindViewById<TextView>(Resource.Id.txtDay);
            NumberDate = itemview.FindViewById<TextView>(Resource.Id.txtnumberdate);
            PaymentIcon = itemview.FindViewById<ImageView>(Resource.Id.imageViewPaymentIcon);
            BillNo = itemview.FindViewById<TextView>(Resource.Id.txtBillNo);
            Price = itemview.FindViewById<TextView>(Resource.Id.txtPrice);
            Time = itemview.FindViewById<TextView>(Resource.Id.txtshowtime);
            CustomerName = itemview.FindViewById<TextView>(Resource.Id.txtCustomerName);
            Headerbar = itemview.FindViewById<LinearLayout>(Resource.Id.lnheaderbar);
            StatusVoid = itemview.FindViewById<TextView>(Resource.Id.txtVoid);
            StatusPending = itemview.FindViewById<TextView>(Resource.Id.txtPending);

            itemview.Click += (sender, e) => listener(base.Position);
        }
    }


}