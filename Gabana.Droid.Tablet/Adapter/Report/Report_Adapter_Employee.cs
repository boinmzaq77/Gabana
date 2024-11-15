using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_Employee : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListEmployee listEmployee;
        public string positionClick;
        Model.UserAccountInfo dataEmpPosition;

        public Report_Adapter_Employee(ListEmployee l)
        {
            listEmployee = l;
        }
        public override int ItemCount
        {
            get { return listEmployee == null ? 0 : listEmployee.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewEmployeeHolder vh = holder as ListViewEmployeeHolder;
                vh.EmpName.Text = listEmployee[position].UserName?.ToString();
                dataEmpPosition = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower().Replace("\u200b", "") == listEmployee[position].UserName.ToLower().Replace("\u200b", "")).FirstOrDefault();

                string Language = Preferences.Get("Language", "");

                if (Language == "th")
                {
                    switch (dataEmpPosition.MainRoles)
                    {
                        case "Admin":
                            vh.EmpPosition.Text = "แอดมิน";
                            break;
                        case "Cashier":
                            vh.EmpPosition.Text = "พนักงานเก็บเงิน";
                            break;
                        case "Editor":
                            vh.EmpPosition.Text = "ผู้แก้ไข";
                            break;
                        case "Invoice":
                            vh.EmpPosition.Text = "พนักงานออกใบเสร็จ";
                            break;
                        case "Manager":
                            vh.EmpPosition.Text = "ผู้จัดการ";
                            break;
                        case "Officer":
                            vh.EmpPosition.Text = "พนักงานทั่วไป";
                            break;
                        default:
                            vh.EmpPosition.Text = "เจ้าของ";
                            break;
                    }
                }
                else
                {
                    vh.EmpPosition.Text = dataEmpPosition.MainRoles?.ToString();
                }

                Utils.SetEmployeeImage(vh.EmpImage, dataEmpPosition.MainRoles, true);

                var index = Report_Dialog_Employee.listChooseEmployee.FindIndex(x => x.UserName == listEmployee[position].UserName);
                if (index == -1)
                {
                    vh.Check.Visibility = ViewStates.Invisible;
                }
                else
                {
                    vh.Check.Visibility = ViewStates.Visible;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_chooseemployee, parent, false);
            ListViewEmployeeHolder vh = new ListViewEmployeeHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }

}