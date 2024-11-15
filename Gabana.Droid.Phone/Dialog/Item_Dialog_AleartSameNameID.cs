using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Item_Dialog_AleartSameNameID : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static string DetailItem, Detail, Event, ItemCode, ItemName;
        TextView txtDetail;
        ItemOnBranch itemOnBranch;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Item_Dialog_AleartSameNameID NewInstance(string _itemname, string _itemcode, string _detailinsert, string _event)
        {
            DetailItem = _itemname;
            Detail = _detailinsert;
            Event = _event;
            ItemCode = _itemcode;
            ItemName = _itemname;
            var frag = new Item_Dialog_AleartSameNameID { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.item_dialog_same_id_name, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = string.Empty;

                //มีสินค้าใช้ Item Code 123 อยู่แล้ว ต้องการใช้ซ้ำหรือไม่?

                var textItemName = ItemName;
                var textItemCode = ItemCode;
                var text1 = GetText(Resource.String.dialog_additem1);
                var text2 = GetText(Resource.String.dialog_additem_itemname);
                var text3 = GetText(Resource.String.dialog_additem2);
                var text4 = GetText(Resource.String.dialog_additem_itemcode);
                var text5 = GetText(Resource.String.dialog_additem3);

                if (DataCashing.Language == "th")
                {
                    txtDetail.Text = text1 + text2 + " " + textItemName + " " + text4 + " " + textItemCode + " " + text3 + " " + text5;
                }
                else
                {
                    txtDetail.Text = text1 + " " + text2 + " " + textItemName + " " + text4 + " " + ItemCode + " " + text3 + " " + text5;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                var data = JsonConvert.DeserializeObject<InsertRepeatItem>(Detail);
                if (data != null)
                {
                    if (Event == "insert")
                    {
                        DetailInsert(data.checkManageStock, data.DetailITem, data.Stock, data.minimumstock);
                    }
                    else
                    {
                        DetailUpdate(data.checkManageStock, data.DetailITem, data.Stock, data.minimumstock);
                    }
                    MainDialog.CloseDialog();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }


        async void DetailInsert(bool checkManageStock, Item DetailITem, string Stock, string minimumstock)
        {
            try
            {
                if (checkManageStock)
                {
                    if (DetailITem.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(Stock) | string.IsNullOrEmpty(minimumstock))
                        {
                            Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                            return;
                        }

                        DetailITem.FTrackStock = 1;
                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = DetailITem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = DetailITem.SysItemID,
                            BalanceStock = Convert.ToDecimal(Stock),
                            MinimumStock = Convert.ToDecimal(minimumstock),
                        };
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.insertdataitem), ToastLength.Long).Show();
                        return;
                    }
                }

                //AddItemActivity.createItem.SysItemIdInsertRepeat = DetailITem.SysItemID;
                //AddItemActivity.createItem.SysItemId = DetailITem.SysItemID;
                var resultAddSize = await AddItemActivity.createItem.AddItemExSize();
                if (!resultAddSize)
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.repeatnameexsize), ToastLength.Short).Show();
                    return;
                }

                ItemManage itemManage = new ItemManage();
                var result = await itemManage.InsertItem(DetailITem, itemOnBranch, AddItemActivity.createItem.itemExSizes);
                if (!result)
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)DetailITem.MerchantID, (int)DetailITem.SysItemID);
                }
                else
                {
                    DetailITem.FWaitSending = 2;
                    await itemManage.UpdateItem(DetailITem);
                }

                AddItemActivity.checkManageStock = false;
                AddItemActivity.SyscategoryIDfromPOS = 0;
                AddItemActivity.favoritefromPOS = false;
                AddItemActivity.tabSelected = string.Empty;
                ItemActivity.SetFocusNewItem(DetailITem);
                DataCashing.EditItemID = 0;
                AddItemActivity.createItem.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUpdate at item_dialog_insertrepeatitem");
                Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
            }
        }

        async void DetailUpdate(bool checkManageStock, Item DetailITem, string Stock, string minimumstock)
        {
            try
            {
                bool resultstock = false;
                if (checkManageStock)
                {
                    if (DetailITem.FTrackStock == 1)
                    {
                        //Open Stock
                        if (string.IsNullOrEmpty(Stock))
                        {
                            Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                            return;
                        }
                        decimal minimumStock = 0;
                        if (string.IsNullOrEmpty(minimumstock))
                        {
                            minimumStock = 0;
                        }
                        else
                        {
                            minimumStock = Convert.ToDecimal(minimumstock);
                        }

                        //int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock
                        resultstock = await UpdateOpenStock(DataCashingAll.SysBranchId, (int)DetailITem.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(Stock)), minimumStock);
                        if (!resultstock)
                        {
                            Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                            return;
                        }

                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = DetailITem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = DetailITem.SysItemID,
                            BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(Stock)),
                            MinimumStock = minimumStock,
                            LastDateBalanceStock = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        //Close Stock
                        resultstock = await UpdateClosetock((int)DetailITem.SysItemID);
                        if (!resultstock)
                        {
                            Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                            return;
                        }

                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = DetailITem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = DetailITem.SysItemID,
                            BalanceStock = 0,
                            MinimumStock = 0,
                            LastDateBalanceStock = DateTime.UtcNow
                        };
                    }
                    DetailITem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);

                    ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                    var updateStock = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranch);
                }

                ItemManage itemManage = new ItemManage();
                var result = await itemManage.UpdateItem(DetailITem);
                if (result)
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.editsucess), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                var resultUpdateSize = await AddItemActivity.createItem.EditItemExSize();
                if (!resultUpdateSize)
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.repeatnameexsize), ToastLength.Short).Show();
                    return;
                }
                AddItemActivity.createItem.UpdateItemExsize();


                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)DetailITem.MerchantID, (int)DetailITem.SysItemID);
                }
                else
                {
                    DetailITem.FWaitSending = 2;
                    await itemManage.UpdateItem(DetailITem);
                }

                ItemActivity.SetFocusNewItem(DetailITem);
                AddItemActivity.tabSelected = string.Empty;
                AddItemActivity.checkManageStock = false;
                DataCashing.EditItemID = 0;
                AddItemActivity.createItem.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUpdate at item_dialog_insertrepeatitem");
                Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotedit), ToastLength.Short).Show();
            }
        }

        async Task<bool> UpdateOpenStock(int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock)
        {
            //Post/Open การเปิดระบบ Track Stock
            var PostDataTrackStockOpen = await GabanaAPI.PostDataTrackStockOpen(sysItemID, deviceNo);
            if (PostDataTrackStockOpen.Status)
            {
                //Post/Adjust เป็นการ update BalanceStock หรือ MinimumStock
                var PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(sysBranchID, sysItemID, deviceNo, balanceStock, minimumStock);
                if (PostDataTrackStockAdjust.Status)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if ("Item has stock tracking already." == PostDataTrackStockOpen.Message)
            {
                //Post/Adjust เป็นการ update BalanceStock หรือ MinimumStock
                ResultAPI PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(sysBranchID, sysItemID, deviceNo, balanceStock, minimumStock);
                if (PostDataTrackStockAdjust.Status)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Toast.MakeText(Application.Context, PostDataTrackStockOpen.Message, ToastLength.Long).Show();
                return false;
            }
        }

        async Task<bool> UpdateClosetock(int sysitem)
        {
            //Post/Close เป็นการปิดระบบ Track Stock
            var PostDataTrackStockClose = await GabanaAPI.PostDataTrackStockClose(sysitem, (int)DataCashingAll.DeviceNo);
            if (PostDataTrackStockClose.Status)
            {
                return true;
            }
            else
            {
                Toast.MakeText(Application.Context, PostDataTrackStockClose.Message, ToastLength.Long).Show();
                return false;
            }
        }

        decimal ConvertToDecimal(string txt)
        {
            decimal decimalValue = 0;
            decimal.TryParse(txt, out decimalValue);
            return decimalValue;
        }
    }
}
