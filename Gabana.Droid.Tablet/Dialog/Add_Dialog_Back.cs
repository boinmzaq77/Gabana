using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Add_Dialog_Back : AndroidX.Fragment.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static string Page;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Add_Dialog_Back NewInstance()
        {
            var frag = new Add_Dialog_Back { Arguments = new Bundle() };
            return frag;
        }

        public static void SetPage(string _page)
        {
            Page = _page;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.add_dialog_back, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_ok);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }
        private void Btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page == "item")
                {
                    Item_Fragment_AddItem.fragment_additem.btnAdditem.PerformClick();
                }
                if (Page == "POS_item")
                {
                    POS_Dialog_AddItem.dialog_additem.btnAdditem.PerformClick();
                }
                if (Page == "topping")
                {
                    Item_Fragment_AddTopping.fragment_addtopping.btnAddTopping.PerformClick();
                }
                if (Page == "POS_topping")
                {
                    POS_Dialog_AddTopping.dialog_addtopping.btnAddTopping.PerformClick();
                }
                if (Page == "category")
                {
                    Item_Fragment_AddCategory.fragment_addcategory.btnAdd.PerformClick();
                }
                if (Page == "customer")
                {
                    Customer_Fragment_AddCustomer.fragment_main.btnAdd.PerformClick();
                }
                if (Page == "employee")
                {
                    Employee_Fragment_AddEmployee.fragment_main.btnAddEmployee.PerformClick();
                }
                if (Page == "branch")
                {
                    Setting_Fragment_BranchDetail.fragment_main.btnAddBranch.PerformClick();
                }
                if (Page == "note")
                {
                    Setting_Fragment_AddNote.fragment_addnote.btnAdd.PerformClick();
                }
                if (Page == "membertype")
                {
                    Setting_Fragment_AddMemberType.fragment_addmembertype.btnAdd.PerformClick();
                }
                if (Page == "cash")
                {
                    Setting_Fragment_AddCash.fragment_addcash.btnApplyCash.PerformClick();
                }
                if (Page == "giftvoucher")
                {
                    Setting_Fragment_AddGiftVoucher.fragment_giftvoucher.btnAdd.PerformClick();
                }
                if (Page == "myqr")
                {
                    Setting_Fragment_AddMyQR.fragment_addmyqr.btnAdd.PerformClick();
                }
                Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at addcus");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            if (Page == "item")
            {
                Item_Fragment_AddItem.fragment_additem.SetClearData();
            }
            if (Page == "POS_item")
            {
                POS_Dialog_AddItem.dialog_additem.SetClearData();
            }
            if (Page == "topping")
            {
                Item_Fragment_AddTopping.fragment_addtopping.SetClearData();
            }
            if (Page == "POS_topping")
            {
                POS_Dialog_AddTopping.dialog_addtopping.SetClearData();
            }
            if (Page == "category")
            {
                Item_Fragment_AddCategory.fragment_addcategory.SetClearData();
            }
            if (Page == "customer")
            {
                Customer_Fragment_AddCustomer.fragment_main.SetClearData();
            }
            if (Page == "employee")
            {
                Employee_Fragment_AddEmployee.fragment_main.SetClearData();
            }
            if (Page == "branch")
            {
                Setting_Fragment_BranchDetail.fragment_main.SetClearData();
            }
            if (Page == "note")
            {
                Setting_Fragment_AddNote.fragment_addnote.SetClearData();
            }
            if (Page == "membertype")
            {
                Setting_Fragment_AddMemberType.fragment_addmembertype.SetClearData();
            }
            if (Page == "cash")
            {
                Setting_Fragment_AddCash.fragment_addcash.SetClearData();
            }
            if (Page == "giftvoucher")
            {
                Setting_Fragment_AddGiftVoucher.fragment_giftvoucher.SetClearData();
            }
            if (Page == "myqr")
            {
                Setting_Fragment_AddMyQR.fragment_addmyqr.SetClearData();
            }
            Dismiss();
        }


    }
}