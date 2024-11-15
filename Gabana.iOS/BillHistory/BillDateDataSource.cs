using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;
using System.Threading.Tasks;
using static Gabana.iOS.CustomerController;
using static Gabana.iOS.BillHistoryController;
using System.Globalization;
using Gabana.ShareSource;
using TinyInsightsLib;

namespace Gabana.iOS
{
    public class BillDateDataSource : UICollectionViewDataSource
    {
        //  List<tra>
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        ListBillHistory bill ;
        UIViewController uIView;
        public BillDateDataSource(ListBillHistory list,UIViewController uIView)
        {
            this.bill = list;
            this.uIView = uIView;
        }
        public void ReloadData(ListBillHistory cus)
        {
            try
            {
                this.bill = cus;
                //if (DataCashingAll.CheckConnectInternet)
                //{
                //    NotifyDataSetChanged();
                //}
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
                return;
            }
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell;
            if (this.bill.Trans[(int)indexPath.Row].fhead)
            {

                cell = collectionView.DequeueReusableCell("BillhistoryCollectionViewCell", indexPath) as BillhistoryCollectionViewCell;

                CultureInfo cultureInfo = new CultureInfo("en-US");
                var timezoneslocal = TimeZoneInfo.Local;
                var TranDate = this.bill.Trans[(int)indexPath.Row].tranDate;
                string date = Utils.ShowDate(TranDate);
                DateTime DateTranDate = DateTime.Parse(TranDate.ToString());

                string timer = TimeZoneInfo.ConvertTimeFromUtc(TranDate, timezoneslocal).ToString("HH:mm tt", new CultureInfo("en-US"));
                //string timer = TranDate.ToString("HH:mm tt", cultureUS);

                string datenow = DateTime.Today.ToString("dd/MM/yyyy", cultureInfo);
                DateTime Now = DateTime.ParseExact(datenow, "dd/MM/yyyy", null);



                if (date == Now.Date.ToShortDateString())
                {
                    (cell as BillhistoryCollectionViewCell).Date = "Today  , " + DateTranDate.ToString("dd/MM/yyyy", cultureInfo);
                }
                else
                {
                    string str = TranDate.DayOfWeek.ToString();
                    (cell as BillhistoryCollectionViewCell).Date = str.Substring(0, 3) + "  , " + DateTranDate.ToString("dd/MM/yyyy", cultureInfo);
                }
                //   cell.
                (cell as BillhistoryCollectionViewCell).sizeWidth = (int)this.uIView.View.Frame.Width;
                (cell as BillhistoryCollectionViewCell).countRow = this.bill.Trans.Count;

            }
            else
            {
                cell = collectionView.DequeueReusableCell("BillListViewCell", indexPath) as BillListViewCell;
                //if (this.bill.Trans[(int)indexPath.Row].paymentType == "EP" && this.bill.Trans[(int)indexPath.Row].ePaymentType == "QR")
                //{
                //    (cell as BillListViewCell).Image =  Utils.SetPaymentImage(this.bill.Trans[(int)indexPath.Row].ePaymentType);
                //}
                //else
                //{
                //    (cell as BillListViewCell).Image = Utils.SetPaymentImage(this.bill.Trans[(int)indexPath.Row].paymentType);
                //}
               (cell as BillListViewCell).Image = Utils.SetPaymentImage(this.bill.Trans[(int)indexPath.Row].paymentType);
                (cell as BillListViewCell).CustomerName = this.bill.Trans[(int)indexPath.Row].customerName;
                (cell as BillListViewCell).BillNo = this.bill.Trans[(int)indexPath.Row].tranNo;
                (cell as BillListViewCell).Total = Utils.DisplayDecimal(this.bill.Trans[(int)indexPath.Row].grandTotal)+ CURRENCYSYMBOLS;
                var timezoneslocal = TimeZoneInfo.Local;
                var date = this.bill.Trans[(int)indexPath.Row].tranDate;
                

                (cell as BillListViewCell).Time = TimeZoneInfo.ConvertTimeFromUtc(date, timezoneslocal).ToString("HH:mm tt", new CultureInfo("en-US"));  //date.ToString("HH:mm tt", new CultureInfo("en-US"));
                (cell as BillListViewCell).sizeWidth = (int)this.uIView.View.Frame.Width;
                if (this.bill.Trans[(int)indexPath.Row].fCancel == 1 )
                {
                    (cell as BillListViewCell).Voidbill = 1; 
                }
                else if (this.bill.Trans[(int)indexPath.Row].FWaiting != 0)
                {
                    (cell as BillListViewCell).Voidbill = 2;
                }
                else
                {
                    (cell as BillListViewCell).Voidbill = 3;
                }
                
                //(cell as BillListViewCell).Voidbill = this.bill.Trans[(int)indexPath.Row].fCancel;
            }



            //cell.TranHisRow =  this.bill.Trans.Where(x=>x.tranDate == TranDate);
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if(this.bill != null || this.bill.Count>0)
                return this.bill.Count;
            else
                return 0;
        }
    }
}