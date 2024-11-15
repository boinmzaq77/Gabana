using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_ShowEmployee : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<SalesBySellerResponse> listTime;
        public string positionClick;
        private string CURRENCYSYMBOLS;
        List<ORM.MerchantDB.UserAccountInfo> users;
        public Report_Adapter_ShowEmployee(List<SalesBySellerResponse> l, List<ORM.MerchantDB.UserAccountInfo> listEmployee)
        {
            listTime = l;
            users = listEmployee;
        }

        public override int ItemCount
        {
            get { return listTime == null ? 0 : listTime.Count; }
        }

        public List<ReportHourly> ReportHourlies { get; }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewTimeReporHolder vh = holder as ListViewTimeReporHolder;
                var dataEmpPosition = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == listTime[position].sellerName.ToLower()).FirstOrDefault();

                string Language = Preferences.Get("Language", "");

                if (Language == "th")
                {
                    switch (dataEmpPosition.MainRoles.ToLower())
                    {
                        case "owner":
                            vh.TextPosition.Text = "เจ้าของ";
                            break;
                        case "admin":
                            vh.TextPosition.Text = "แอดมิน";
                            break;
                        case "cashier":
                            vh.TextPosition.Text = "พนักงานเก็บเงิน";
                            break;
                        case "editor":
                            vh.TextPosition.Text = "ผู้แก้ไข";
                            break;
                        case "invoice":
                            vh.TextPosition.Text = "พนักงานออกใบเสร็จ";
                            break;
                        case "manager":
                            vh.TextPosition.Text = "ผู้จัดการ";
                            break;
                        case "officer":
                            vh.TextPosition.Text = "พนักงานทั่วไป";
                            break;
                        default:
                            vh.TextPosition.Text = "";
                            break;
                    }
                }
                else
                {
                    vh.TextPosition.Text = dataEmpPosition.MainRoles?.ToString();
                }

                Utils.SetEmployeeImage(vh.Position, dataEmpPosition.MainRoles, true);

                vh.Time.Text = listTime[position].sellerName;
                vh.Amount.Text = CURRENCYSYMBOLS + " " + listTime[position].sumTotalAmount.ToString("#,##0.00");
                Log.Debug("dataEmpPosition", dataEmpPosition.MainRoles.ToString()) ;
            }
            catch (Exception ex)
            {
                Log.Debug("dataEmpPosition", ex.Message);
            }
           
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_employee, parent, false);
            ListViewTimeReporHolder vh = new ListViewTimeReporHolder(itemView, OnClick);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}