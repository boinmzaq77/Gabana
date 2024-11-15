using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.ShareSource;
using System;
using System.Globalization;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    class Bill_Adapter_Main : RecyclerView.Adapter
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
                //lstBill = newBillHistoryList;

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
                #region oldcode
                //ListViewBillHistoryHolder vh = holder as ListViewBillHistoryHolder;

                ////CultureInfo.CurrentCulture = new CultureInfo("en-US");
                //CultureInfo cultureInfo = new CultureInfo("en-US");
                //var timezoneslocal = TimeZoneInfo.Local;
                //DateTime dateTime = lstBill.Trans[position].tranDate;
                //var TranDate = lstBill.Trans[position].tranDate;
                //string date = Utils.ShowDate(TranDate);
                ////DateTime DateTranDate = DateTime.Parse(TranDate.ToString());

                //DateTime DateTranDate = TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal);

                //string timer = TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal).ToString("HH:mm tt", new CultureInfo("en-US"));
                ////string timer = TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal).ToString("dd/MM/yyy", new CultureInfo("en-US")) + " " + lstBill.Trans[position].tranDate.ToString("HH:mm tt");

                //string datenow = DateTime.Today.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                //DateTime Now = DateTime.ParseExact(datenow, "dd/MM/yyyy", new CultureInfo("en-US"));

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
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.bill_adapter_main, parent, false);
            ListViewBillHistoryHolder vh = new ListViewBillHistoryHolder(itemView, OnClick);
            TextView txtCurrency = itemView.FindViewById<TextView>(Resource.Id.txtCurrency);
            txtCurrency.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
        public delegate void CardCellQRCodeIndexDelegate5();
        public event CardCellQRCodeIndexDelegate5 OnCardCellbtnIndex5;
    }
}