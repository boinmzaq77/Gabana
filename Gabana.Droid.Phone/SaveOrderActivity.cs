using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class SaveOrderActivity : Activity
    {
        public static TranWithDetailsLocal tranWithDetails;
        EditText txtOrderName, textComment;
        TransManage transManage = new TransManage();
        string usernamelogin;
        Button btnSave;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.saveorder_activity_main);

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                txtOrderName = FindViewById<EditText>(Resource.Id.txtOrderName);
                textComment = FindViewById<EditText>(Resource.Id.textComment);

                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnSave.Click += BtnSave_Click;
                usernamelogin = Preferences.Get("User", "");
                CheckJwt();

                if (tranWithDetails.tran != null)
                {
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    txtOrderName.Text = "Order-" + DateTime.Now.ToString("HH:mm");
                    textComment.Text = tranWithDetails.tran.Comments; //ดึง comment ของบิลขึ้นมาแสดงที่หน้า order ก่อนครับ ถ้าเค้าแก้ก็จะได้แทนที่ไปเลย
                    DataCashing.ModifyTranOrder = true;
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreate : SaveOrderActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Save Order");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

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

                //ไม่ใช่แล้ว เนื่องจาก การกดเข้ามาดูออเดอร์เดิม จะทำให้ order name เป็นเวลาใหม่ นับว่าเป็นการแก้ไขออเดอร์ ทำให้เป็นการสร้างออเดอร์ใหม่
                //else if (DataCashing.ModifyTranOrder == false & tranWithDetails.tran.TranType == 'O') //เลือก order และมาพักบิลอีกรอบ โดยที่ไม่มีการแก้ไข
                //{
                //    tranWithDetails.tran.Comments = textComment.Text;
                //    await Utils.CancelTranOrder(tranWithDetails);
                //    DataCashing.isCurrentOrder = false;
                //}

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

                NewSale();
                btnSave.Enabled = true;

            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at Save Order");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        async void NewSale()
        {
            try
            {
                #region NewSale
                //StartActivity(new Intent(Application.Context, typeof(PosActivity)));
                //tranWithDetails = new TranWithDetailsLocal { tran = new Tran { TaxRate = 0 }, tranDetailItemWithToppings = new List<TranDetailItemWithTopping>(), tranPayments = new List<TranPayment>(), tranTradDiscounts = new List<TranTradDiscount>() };

                tranWithDetails = null;
                tranWithDetails = await Utils.initialData();
                PosActivity.SetTranDetail(tranWithDetails);
                PosActivity.totlaItems = 0;
                DataCashing.setQuantityToCart = 1;
                DataCashing.SysCustomerID = 999;
                DataCashing.isCurrentOrder = false;
                DataCashing.ModifyTranOrder = false;

                if (CartActivity.cart != null)
                {
                    CartActivity.cart.lnRemark.Visibility = Android.Views.ViewStates.Gone;
                }

                if (CartScanActivity.scan != null)
                {
                    CartScanActivity.lnRemark.Visibility = Android.Views.ViewStates.Gone;
                }
                this.Finish();
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NewSale at Save Order");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            try
            {
                StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                PaymentActivity.SetTranDetail(tranWithDetails);
                base.OnBackPressed();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBackPressed at Save Order");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        internal static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'SaveOrderActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'SaveOrderActivity.openPage' is assigned but its value is never used
        public DateTime pauseDate = DateTime.Now;
        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
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

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

        }
    }
}

