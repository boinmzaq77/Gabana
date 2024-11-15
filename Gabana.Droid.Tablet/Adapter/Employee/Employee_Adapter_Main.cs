using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana.Model;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Adapter.Employee
{
    internal class Employee_Adapter_Main : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListEmployee listemployee;
        public string positionClick;
        string UserLogin;
        string TypeLogin;
        bool EmpActive;
        Model.UserAccountInfo dataEmpPosition;

        public Employee_Adapter_Main(ListEmployee l)
        {
            listemployee = l;
        }
        public override int ItemCount
        {
            get { return listemployee == null ? 0 : listemployee.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewEmployeeHolder vh = holder as ListViewEmployeeHolder;
                UserLogin = Preferences.Get("User", "");
                TypeLogin = Preferences.Get("LoginType", "");
                string Language = Preferences.Get("Language", "");

                if (DataCashingAll.UserAccountInfo == null)
                {
                    var Employee = Preferences.Get("Employeeoffline", "");
                    if (Employee != "")
                    {
                        var lstEmployee = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(Employee);
                        DataCashingAll.UserAccountInfo = lstEmployee;
                    }
                }

                dataEmpPosition = DataCashingAll.UserAccountInfo
                        .Where(x => x.UserName.ToLower() == listemployee[position].UserName.ToLower()).FirstOrDefault();
                if (dataEmpPosition != null)
                {
                    vh.EmpName.Text = listemployee[position].UserName?.ToString();
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

                    EmpActive = dataEmpPosition.UserAccessProduct;

                    Utils.SetEmployeeImage(vh.EmpImage, dataEmpPosition.MainRoles.ToLower(), EmpActive);
                    if (DataCashing.CheckNet)
                    {
                        Utils.SetEmployeeActive(vh.ActiveImage, EmpActive);
                    }
                    else
                    {
                        vh.ActiveImage.Visibility = ViewStates.Gone;
                    }

                    //Active Unactive 
                    if (EmpActive)
                    {
                        vh.EmpPosition.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    }
                    else
                    {
                        vh.EmpPosition.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                    }

                    #region Check LoginType
                    //admin not edit owner
                    if (TypeLogin.ToLower() == "admin" && dataEmpPosition.MainRoles.ToLower() == "owner")
                    {
                        vh.EmpPosition.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                    }

                    //user cannot manage other account
                    if (TypeLogin.ToLower() != "owner" && TypeLogin.ToLower() != "admin")
                    {
                        if (UserLogin.ToLower() == dataEmpPosition.UserName.ToLower())
                        {
                            vh.EmpPosition.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                        }
                        else
                        {
                            vh.EmpPosition.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                        }
                    }
                    #endregion
                }
                else
                {
                    vh.EmpName.Text = listemployee[position].UserName?.ToString();
                    vh.EmpPosition.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBindViewHolder at employee)adapter");
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.employee_adapter_main, parent, false);
            ListViewEmployeeHolder vh = new ListViewEmployeeHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }

    public class ListViewEmployeeHolder : RecyclerView.ViewHolder
    {
        public ImageView EmpImage { get; set; }
        public TextView EmpName { get; set; }
        public TextView EmpPosition { get; set; }
        public ImageView ActiveImage { get; set; }

        public ListViewEmployeeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            EmpImage = itemview.FindViewById<ImageView>(Resource.Id.imgProfile);
            EmpName = itemview.FindViewById<TextView>(Resource.Id.textNameEmp);
            EmpPosition = itemview.FindViewById<TextView>(Resource.Id.textPositionEmp);
            ActiveImage = itemview.FindViewById<ImageView>(Resource.Id.imgShowActiveEmp);

            itemview.Click += (sender, e) => listener(base.Position);
        }
    }

}