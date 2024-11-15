using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Edit_Dialog_Back : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static string Page, PassValue;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Edit_Dialog_Back NewInstance(string _page, string _passvalue)
        {
            Page = _page;
            PassValue = _passvalue;
            var frag = new Edit_Dialog_Back { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.edit_dialog_back, container, false);
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
                    AddItemActivity.createItem.btnAdditem.PerformClick();
                }
                if (Page == "topping")
                {
                    AddExtraToppingActivity.addExtra.btnadditem.PerformClick();
                }
                if (Page == "category")
                {
                    AddCategoryActivity.addcategory.btnAdd.PerformClick();
                }
                if (Page == "customer")
                {
                    AddCustomerActivity.addCustomer.btnAdd.PerformClick();
                }
                if (Page == "employee")
                {
                    AddEmployeeActivity.addEmployee.btnAddEmployee.PerformClick();
                }
                if (Page == "branch")
                {
                    AddBranchActivity.addBranch.btnAddBranch.PerformClick();
                }
                if (Page == "note")
                {
                    AddNoteActivity.addNote.btnAdd.PerformClick();
                }
                if (Page == "membertype")
                {
                    AddMembertypeActivity.addMembertype.btnAdd.PerformClick();
                }
                if (Page == "cash")
                {
                    AddCashGuideActivity.addCash.btnApplyCashGuide.PerformClick();
                }
                if (Page == "giftvoucher")
                {
                    AddGiftvoucherActivity.activity.btnAdd.PerformClick();
                }
                if (Page == "myqr")
                {
                    AddmyQRActivity.activity.btnAdd.PerformClick();
                }
                if (Page == "merchant")
                {
                    MerchantActivity.merchant.btnSave.PerformClick();
                }
                if (Page == "device")
                {
                    DeviceActivity.activity.btnSave.PerformClick();
                }
                MainDialog.CloseDialog();
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
                AddItemActivity.createItem.DialogCheckBack();
            }
            if (Page == "topping")
            {
                AddExtraToppingActivity.addExtra.DialogCheckBack();
            }
            if (Page == "category")
            {
                AddCategoryActivity.addcategory.DialogCheckBack();
            }
            if (Page == "customer")
            {
                AddCustomerActivity.addCustomer.DialogCheckBack();
            }
            if (Page == "employee")
            {
                AddEmployeeActivity.addEmployee.DialogCheckBack();
            }
            if (Page == "merchant")
            {
                MerchantActivity.merchant.DialogCheckBack();
            }
            if (Page == "branch")
            {
                AddBranchActivity.addBranch.DialogCheckBack();
            }
            if (Page == "note")
            {
                AddNoteActivity.addNote.DialogCheckBack();
            }
            if (Page == "membertype")
            {
                AddMembertypeActivity.addMembertype.DialogCheckBack();
            }
            if (Page == "cash")
            {
                AddCashGuideActivity.addCash.DialogCheckBack();
            }
            if (Page == "giftvoucher")
            {
                AddGiftvoucherActivity.activity.DialogCheckBack();
            }
            if (Page == "myqr")
            {
                AddmyQRActivity.activity.DialogCheckBack();
            }
            if (Page == "device")
            {
                DeviceActivity.activity.DialogCheckBack();
            }
            if (Page == "merchant")
            {
                MerchantActivity.merchant.DialogCheckBack();
            }
            MainDialog.CloseDialog();
        }

    }
}
