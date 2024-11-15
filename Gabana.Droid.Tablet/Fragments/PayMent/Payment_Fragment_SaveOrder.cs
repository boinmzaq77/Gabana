using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    internal class Payment_Fragment_SaveOrder : AndroidX.Fragment.App.Fragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Payment_Fragment_SaveOrder NewInstance()
        {
            Payment_Fragment_SaveOrder frag = new Payment_Fragment_SaveOrder();
            return frag;
        }

        View view;
        public static TranWithDetailsLocal tranWithDetails;
        public static Payment_Fragment_SaveOrder fragment_saveorder;
        string CURRENCYSYMBOLS, DECIMALPOINTDISPLAY, usernamelogin;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_saveorder, container, false);
            try
            {
                fragment_saveorder = this;
                tranWithDetails = PaymentActivity.tranWithDetails;
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                usernamelogin = Preferences.Get("User", "");
                ComBineUI();
                btnSave.Click += BtnSave_Click;

                if (tranWithDetails.tran != null)
                {
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    txtOrderName.Text = "Order-" + DateTime.Now.ToString("HH:mm");
                    textComment.Text = tranWithDetails.tran.Comments; //ดึง comment ของบิลขึ้นมาแสดงที่หน้า order ก่อนครับ ถ้าเค้าแก้ก็จะได้แทนที่ไปเลย
                    DataCashing.ModifyTranOrder = true;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }

            return view;
        }

        TransManage transManage = new TransManage();
        private async void BtnSave_Click(object sender, EventArgs e)
        {            
            //บันทึกออเดอร์ ถ้ามีการแก้ไข ไม่ว่าจะเป็นสินค้า ชื่อออเดอร์ หรือหมายเหตุ ให้นับว่ามีการแก้ไข //17/06/2565 ถามพี่บัสแล้ว
            //Save Order
            try
            {
                btnSave.Enabled = false;
                if (tranWithDetails.tran == null)
                {
                    btnSave.Enabled = true;
                    return;
                }

                //แก้ไข order เดิมและพักบิลอีกรอบ
                if (DataCashing.ModifyTranOrder == true & tranWithDetails.tran.TranType == 'O')
                {
                    //await Utils.ModifyTranOrder(tranWithDetails);

                    //ทำการสร้าง Order ใหม่ขึ้นมาแทน
                    //Old Tran
                    tranWithDetails.tran.Status = 120;
                    tranWithDetails.tran.FWaitSending = 0;
                    var updatetTran = await transManage.UpdateTran(tranWithDetails.tran);

                    //New Tran 
                    Model.TranWithDetailsLocal TranWithDetailsnewTran = await Utils.initialData();
                    string NewTranOrder = "#" + DataCashingAll.DeviceNo.ToString() + "-" + Utils.ChangeDateTimeTranOrder(DateTime.UtcNow);

                    if (TranWithDetailsnewTran != null)
                    {
                        TranWithDetailsnewTran.tran = tranWithDetails.tran;
                        TranWithDetailsnewTran.tran.TranNo = NewTranOrder;
                        TranWithDetailsnewTran.tran.TranType = 'O';
                        TranWithDetailsnewTran.tran.Status = 100;
                        TranWithDetailsnewTran.tran.LocalDataStatus = 'I';
                        TranWithDetailsnewTran.tran.FWaitSending = 2;
                        TranWithDetailsnewTran.tran.WaitSendingTime = DateTime.UtcNow;
                        TranWithDetailsnewTran.tran.OrderName = txtOrderName.Text;
                        TranWithDetailsnewTran.tran.Comments = textComment.Text;
                        TranWithDetailsnewTran.tran.TranDate = DateTime.UtcNow;
                        TranWithDetailsnewTran.tran.SellerName = usernamelogin;
                        TranWithDetailsnewTran.tran.DeviceNo = DataCashingAll.DeviceNo;
                        TranWithDetailsnewTran.tran.LastDateModified = DateTime.UtcNow;
                        TranWithDetailsnewTran.tran.LastUserModified = usernamelogin;
                        TranWithDetailsnewTran.tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings;
                        TranWithDetailsnewTran.tranPayments = tranWithDetails.tranPayments;
                        TranWithDetailsnewTran.tranTradDiscounts = tranWithDetails.tranTradDiscounts;

                        //แก้ไข tranNo ให้เป็นตัวใหม่
                        foreach (var item in TranWithDetailsnewTran.tranDetailItemWithToppings)
                        {
                            //TranDetail
                            item.tranDetailItem.TranNo = NewTranOrder;

                            //TranDetailTopping
                            foreach (var itemTopping in item.tranDetailItemToppings)
                            {
                                itemTopping.TranNo = NewTranOrder;
                            }
                        }

                        //TranDiscount
                        foreach (var item in TranWithDetailsnewTran.tranTradDiscounts)
                        {
                            //TranDetail
                            item.TranNo = NewTranOrder;
                        }

                        //TranPayment
                        foreach (var item in TranWithDetailsnewTran.tranPayments)
                        {
                            //TranDetail
                            item.TranNo = NewTranOrder;
                        }

                        var insertTran = await transManage.InsertTran(TranWithDetailsnewTran);
                        if (await GabanaAPI.CheckNetWork())
                        {
                            JobQueue.Default.AddJobSendTrans((int)DataCashingAll.MerchantId, DataCashingAll.SysBranchId, TranWithDetailsnewTran.tran.TranNo);
                        }
                        else
                        {
                            TranWithDetailsnewTran.tran.Status = 100;
                            TranWithDetailsnewTran.tran.FWaitSending = 2;
                            var insertTranoffline = await transManage.UpdateTran(TranWithDetailsnewTran.tran);
                        }
                    }
                }

                if (tranWithDetails.tran.TranType == 'B')
                {
                    //TranType จาก 'B' -> 'O'
                    //Save order click
                    string NewTranOrder = "#" + DataCashingAll.DeviceNo.ToString() + "-" + Utils.ChangeDateTimeTranOrder(DateTime.UtcNow);
                    tranWithDetails.tran.TranNo = NewTranOrder;
                    tranWithDetails.tran.TranType = 'O';
                    tranWithDetails.tran.Status = 100;
                    tranWithDetails.tran.LocalDataStatus = 'I';
                    tranWithDetails.tran.OrderName = txtOrderName.Text;
                    tranWithDetails.tran.Comments = textComment.Text;
                    tranWithDetails.tran.FWaitSending = 2;
                    tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                    tranWithDetails.tran.TranDate = DateTime.UtcNow;

                    //แก้ไข tranNo ให้เป็นตัวใหม่
                    foreach (var item in tranWithDetails.tranDetailItemWithToppings)
                    {
                        //TranDetail
                        item.tranDetailItem.TranNo = NewTranOrder;

                        //TranDetailTopping
                        foreach (var itemTopping in item.tranDetailItemToppings)
                        {
                            itemTopping.TranNo = NewTranOrder;
                        }
                    }

                    //TranDiscount
                    foreach (var item in tranWithDetails.tranTradDiscounts)
                    {
                        //TranDetail
                        item.TranNo = NewTranOrder;
                    }

                    //TranPayment
                    foreach (var item in tranWithDetails.tranPayments)
                    {
                        //TranDetail
                        item.TranNo = NewTranOrder;
                    }

                    var insertToTran = await transManage.InsertTran(tranWithDetails);

                    //Cloud JobQueue
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendTrans((int)DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                    }
                    else
                    {
                        tranWithDetails.tran.FWaitSending = 2;
                        await transManage.UpdateTran(tranWithDetails.tran);
                    }
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.textbillsuccess), ToastLength.Short).Show();
                POS_Fragment_Main.totlaItems = 0;
                DataCashing.setQuantityToCart = 1;
                DataCashing.SysCustomerID = 999;
                DataCashing.isCurrentOrder = false;
                DataCashing.ModifyTranOrder = false;
                //DataCashing.PaymentNo = 0;


                //Initial ค่าใหม่หลังจากเปิดการขายรอบใหม่
                tranWithDetails = null;
                tranWithDetails = await Utils.initialData();
                MainActivity.tranWithDetails = tranWithDetails;
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();
                PaymentActivity.tranWithDetails = tranWithDetails;
                PaymentActivity.payment_main.OnBackPressed();
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at Save Order");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        EditText txtOrderName, textComment;
        Button btnSave;

        private void ComBineUI()
        {
            txtOrderName = view.FindViewById<EditText>(Resource.Id.txtOrderName);
            textComment = view.FindViewById<EditText>(Resource.Id.textComment);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                if (!IsAdded)
                {
                    return;
                }

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    return;
                }
                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }
    }
}