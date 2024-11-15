using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Cart_Dialog_ClearCart : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static TranWithDetailsLocal tranWithDetails;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Cart_Dialog_ClearCart NewInstance()
        {
            var frag = new Cart_Dialog_ClearCart { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_deleteitem, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

                textconfirm1 = view.FindViewById<TextView>(Resource.Id.textconfirm1);
                textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);
                textconfirm1.Text = string.Empty;
                textconfirm2.Text = string.Empty;
                textconfirm1.Text = GetString(Resource.String.dialog_confirm_clearcart1);
                textconfirm2.Text = GetString(Resource.String.dialog_confirm_clearcart2);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            //clear cart ทั้งหมด tranitem = 0 
            //ห้ามกดปุ่มอื่นนอกจาก back 
            try
            {
                if (CartActivity.CurrentActivity)
                {
                    OptionActivity.lstTranSelectTopping = new List<TranDetailItemTopping>();
                    OptionActivity.SelectSize = new ItemExSize();
                    OptionActivity.SelectNote = new List<Note>();
                    OptionActivity.ExSizeNo = 0;
                    OptionActivity.sysitemIDToppping = 0;
                    OptionActivity.NoteID = 0;
                    OptionActivity.flagLoadSize = false;
                    OptionActivity.POSDataItem = null;
                    DataCashing.flagEditOptionSize = false;
                    DataCashing.flagEditOptionExtraTopping = false;
                    DataCashing.flagEditOptionNote = false;
                    DataCashing.SysCustomerID = 999;
                    CartActivity.addDiscount = false;
                    //CartActivity.addRemark = false;
                    CartActivity.DetailNo = 0;


                    await Utils.CancelTranOrder(tranWithDetails);
                    DataCashing.isCurrentOrder = false;
                    //CartActivity.tranWithDetails = new TranWithDetailsLocal();
                    //CartActivity.tranWithDetails = new TranWithDetailsLocal { tran = new Tran { TaxRate = 0 }, tranDetailItemWithToppings = new List<TranDetailItemWithTopping>(), tranPayments = new List<TranPayment>(), tranTradDiscounts = new List<TranTradDiscount>() };
                    
                    tranWithDetails = null;
                    CartActivity.tranWithDetails = await Utils.initialData();
                    StartActivity(new Intent(Application.Context, typeof(CartActivity)));
                    MainDialog.CloseDialog();
                }
                else
                {
                    OptionActivity.flagLoadSize = false;
                    OptionActivity.POSDataItem = null;
                    DataCashing.SysCustomerID = 999;
                    CartScanActivity.addDiscount = false;
                    //CartScanActivity.addRemark = false;
                    CartScanActivity.detailNoClickOption = 0;

                    await Utils.CancelTranOrder(tranWithDetails);
                    DataCashing.isCurrentOrder = false;
                    //CartScanActivity.tranWithDetails.tranTradDiscounts = new List<TranTradDiscount>();
                    //CartScanActivity.tranWithDetails.tranDetailItemWithToppings = new List<TranDetailItemWithTopping>();
                    //CartScanActivity.tranWithDetails = new TranWithDetailsLocal { tran = new Tran { TaxRate = 0 }, tranDetailItemWithToppings = new List<TranDetailItemWithTopping>(), tranPayments = new List<TranPayment>(), tranTradDiscounts = new List<TranTradDiscount>() };

                    tranWithDetails = null;
                    CartScanActivity.tranWithDetails = await Utils.initialData();
                    StartActivity(new Intent(Application.Context, typeof(CartScanActivity)));
                    MainDialog.CloseDialog();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at cartdialog");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

    }
}
