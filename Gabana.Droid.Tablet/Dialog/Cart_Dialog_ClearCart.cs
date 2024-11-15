using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.PayMent;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Cart_Dialog_ClearCart : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Cart_Dialog_ClearCart NewInstance()
        {
            var frag = new Cart_Dialog_ClearCart { Arguments = new Bundle() };
            return frag;
        }
        View view;
        Button btnCancel, btnSave;
        static TranWithDetailsLocal tranWithDetails;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.cart_dialog_clearcart, container, false);
            try
            {
                tranWithDetails = MainActivity.tranWithDetails;
                btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
                btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
                btnCancel.Click += Btn_cancel_Click;
                btnSave.Click += Btn_save_Click;
                _ = TinyInsights.TrackPageViewAsync("OnCreateView : Cart_Dialog_ClearCart");
                return view;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return view;
            }

        }
        private async void Btn_save_Click(object sender, EventArgs e)
        {
            //clear cart ทั้งหมด tranitem = 0 
            //ห้ามกดปุ่มอื่นนอกจาก back 
            try
            {                
                POS_Dialog_Option.lstTranSelectTopping = new List<TranDetailItemTopping>();
                POS_Dialog_Option.SelectSize = new ItemExSize();
                POS_Dialog_Option.SelectNote = new List<Note>();
                POS_Dialog_Option.ExSizeNo = 0;
                POS_Dialog_Option.sysitemIDToppping = 0;
                POS_Dialog_Option.NoteID = 0;
                POS_Dialog_Option.flagLoadSize = false;
                POS_Dialog_Option.POSDataItem = null;
                DataCashing.flagEditOptionSize = false;
                DataCashing.flagEditOptionExtraTopping = false;
                DataCashing.flagEditOptionNote = false;
                DataCashing.SysCustomerID = 999;
                POS_Fragment_Cart.addDiscount = false;
                POS_Fragment_Cart.addRemark = false;
                POS_Fragment_Cart.DetailNo = 0;
                POS_Dialog_Scan.detailNoClickOption = 0;
                await Utils.CancelTranOrder(tranWithDetails);
                DataCashing.isCurrentOrder = false;

                //23/05/66
                //Initial ค่าใหม่หลังจากเปิดการขายรอบใหม่
                tranWithDetails = null;
                tranWithDetails = await Utils.initialData();
                MainActivity.tranWithDetails = tranWithDetails;
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();                            
                
                if (Cart_Dialog_Option.cart_optiion != null)
                {
                    Cart_Dialog_Option.cart_optiion.Dismiss();
                }

                if (POS_Dialog_Scan.scan != null)
                {
                    POS_Dialog_Scan.scan.OnResume();
                }
                this.Dialog.Dismiss();
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
            this.Dialog.Dismiss();
        }
    }

}