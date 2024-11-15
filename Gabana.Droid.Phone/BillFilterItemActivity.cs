
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class BillFilterItemActivity : AppCompatActivity
    {
        RecyclerView recyclerview_listitem;
        RelativeLayout lnSearchItem;
        public static List<Item> chooseItem = new List<Item>();
        Button btnAll, btnApply;
        bool CheckNet = false; 

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                // Create your application here
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.billfilter_activity_item);
                recyclerview_listitem = FindViewById<RecyclerView>(Resource.Id.recyclerview_listitem);
                lnSearchItem = FindViewById<RelativeLayout>(Resource.Id.lnSearchItem);
                btnAll = FindViewById<Button>(Resource.Id.btnAll);
                btnAll.Click += BtnAll_Click;
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnApply.Click += BtnApply_Click;

                CheckJwt();

                if (await GabanaAPI.CheckSpeedConnection())
                {
                    await GetOnlineDataitem();
                    CheckNet = true;
                }
                else
                {
                    CheckNet = false;
                }
                await GetItemList();
                if (items != null) chooseItem = items.ToList();
                SetItemData();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : BillFilterItemActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Oncreate at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            BillHistoryFilterActivity.chooseAllItem = chooseAll;
            BillHistoryFilterActivity.chooseItem = chooseItem;

        }

        private void BtnAll_Click(object sender, EventArgs e)
        {
            if (chooseAll)
            {
                chooseItem = new List<Item>();
                chooseAll = false;
            }
            else
            {
                chooseItem = items;
                chooseAll = true;
            }
            SetItemData();
            SetBtnAll(chooseAll);
            SetBtnApply();
        }

        private void SetBtnApply()
        {
            if (chooseItem.Count == 0)
            {
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else if (chooseAll)
            {
                btnApply.Enabled = true;
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else if (chooseItem.Count <= maxitemFilter)
            {
                btnApply.Enabled = true;
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.Enabled = false;
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }

        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        ItemManage itemManage = new ItemManage();
        int maxItemRevision = 0;
        ItemExSizeManage itemExSizeManage = new ItemExSizeManage();

        private async Task GetOnlineDataitem()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 30).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region Item                   
                    List<Item> GetAllitem = new List<Item>();
                    List<Gabana3.JAM.Items.ItemWithItemExSizes> UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                    List<Gabana3.JAM.Items.ItemWithItemExSizes> InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                    try
                    {
                        var allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, 0);

                        if (allItem == null)
                        {
                            return;
                        }
                        else if (allItem?.ItemsWithItemExSizes.Count == 0)
                        {
                            return;
                        }
                        else
                        {
                            int round = 0, addrount = 0;
                            round = allItem.totalItems / 100;
                            addrount = round + 1;
                            double increaseProgress = 0;
                            increaseProgress = 25 / addrount;

                            for (int j = 0; j < addrount; j++)
                            {
                                allItem = await GabanaAPI.GetDataItem((int)revisionNo.LastRevisionNo, j);

                                if (allItem == null)
                                {
                                    break;
                                }

                                if (allItem.totalItems == 0)
                                {
                                    break;
                                }

                                allItem.ItemsWithItemExSizes.ToList().OrderBy(x => x.ItemStatus.item.RevisionNo);
                                var maxItem = allItem.ItemsWithItemExSizes.ToList().Max(x => x.ItemStatus.item.RevisionNo);// OrderByDescending(x => x.item.RevisionNo).First();                            

                                GetAllitem = await itemManage.GetAllItemType();
                                UpdateItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();
                                InsertItem = new List<Gabana3.JAM.Items.ItemWithItemExSizes>();

                                UpdateItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)).ToList());
                                InsertItem.AddRange(allItem.ItemsWithItemExSizes.Where(x => !(GetAllitem.Select(y => (long)y.SysItemID).ToList().Contains(x.ItemStatus.item.SysItemID)) && x.ItemStatus.DataStatus != 'D').ToList());

                                List<Item> Bulkitem = new List<Item>();
                                List<ItemExSize> BulkitemExsize = new List<ItemExSize>();

                                //Insert Item
                                if (InsertItem.Count > 0)
                                {
                                    foreach (var item in InsertItem)
                                    {
                                        string thumnailPath = string.Empty;
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                                            {
                                                string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                                                thumnailPath = pathImage ?? "";
                                            }
                                            else
                                            {
                                                thumnailPath = "";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                            var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                            maxItemRevision = (errorRevison == 0) ? 0 : errorRevison + 1;
                                            Log.Error("connecterror", "Bulkitem - Image : " + ex.Message);
                                            thumnailPath = "";
                                        }

                                        Bulkitem.Add(new Item()
                                        {
                                            MerchantID = item.ItemStatus.item.MerchantID,
                                            SysItemID = item.ItemStatus.item.SysItemID,
                                            ItemName = item.ItemStatus.item.ItemName,
                                            Ordinary = item.ItemStatus.item.Ordinary,
                                            SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                            ItemCode = item.ItemStatus.item.ItemCode,
                                            ShortName = item.ItemStatus.item.ShortName,
                                            PicturePath = item.ItemStatus.item.PicturePath,
                                            ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                            PictureLocalPath = "",
                                            ThumbnailLocalPath = thumnailPath,
                                            Colors = item.ItemStatus.item.Colors,
                                            FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                            UnitName = item.ItemStatus.item.UnitName,
                                            RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                            EstimateCost = item.ItemStatus.item.EstimateCost,
                                            Price = item.ItemStatus.item.Price,
                                            OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                            TaxType = item.ItemStatus.item.TaxType,
                                            SellBy = item.ItemStatus.item.SellBy,
                                            FTrackStock = item.ItemStatus.item.FTrackStock,
                                            TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                            SaleItemType = item.ItemStatus.item.SaleItemType,
                                            Comments = item.ItemStatus.item.Comments,
                                            LastDateModified = item.ItemStatus.item.LastDateModified,
                                            UserLastModified = item.ItemStatus.item.UserLastModified,
                                            DataStatus = item.ItemStatus.DataStatus,
                                            FWaitSending = 0,
                                            WaitSendingTime = DateTime.UtcNow,
                                            LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                            LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                            FDisplayOption = item.ItemStatus.item.FDisplayOption
                                        });

                                        var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                        itemSizes.ForEach(itemSize => BulkitemExsize.Add(new ItemExSize()
                                        {
                                            MerchantID = itemSize.MerchantID,
                                            SysItemID = itemSize.SysItemID,
                                            EstimateCost = itemSize.EstimateCost,
                                            ExSizeName = itemSize.ExSizeName,
                                            ExSizeNo = itemSize.ExSizeNo,
                                            Price = itemSize.Price,
                                            Comments = itemSize.Comments
                                        }));
                                    }

                                    using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                    {
                                        await db.BeginTransactionAsync();
                                        try
                                        {
                                            await db.BulkCopyAsync(Bulkitem);
                                            await db.BulkCopyAsync(BulkitemExsize);
                                            await db.CommitTransactionAsync();
                                        }
                                        catch (Exception ex)
                                        {
                                            await db.RollbackTransactionAsync();
                                            //Update RevisionNo ที่ผิดพลาด เพื่อเรียกข้อมูลใหม่
                                            var errorRevison = InsertItem.Select(x => x.ItemStatus.item.RevisionNo).Min();
                                            maxItemRevision = errorRevison;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                            Utils.DeletePictureItemMaster(Bulkitem.Select(x => x.ThumbnailLocalPath).ToList());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                            Log.Error("connecterror", "Bulkitem,BulkitemExsize : " + ex.Message);
                                            throw ex;
                                        }
                                    }
                                }

                                //Update Item
                                if (UpdateItem.Count > 0)
                                {
                                    UpdateItem.ForEach(async item =>
                                    {
                                        //Gabana3.JAM.Items.ItemStatus itemStatus = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.ItemStatus).FirstOrDefault();
                                        char itemStatus = item.ItemStatus.DataStatus;
                                        List<ORM.Master.ItemOnBranch> itemOnBranch = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemOnBranchs).FirstOrDefault() ?? new List<ORM.Master.ItemOnBranch>();
                                        var data = await itemManage.GetItem((int)item.ItemStatus.item.MerchantID, (int)item.ItemStatus.item.SysItemID);

                                        if (itemStatus == 'D')
                                        {
                                            await Utils.DeleteItem(data, itemOnBranch);
                                        }
                                        else
                                        {
                                            string thumnailLocalPath = string.Empty;
                                            if (!string.IsNullOrEmpty(item.ItemStatus.item.PicturePath))
                                            {
                                                if (item.ItemStatus.item.PicturePath != data.PicturePath)
                                                {
                                                    //delete รูป
                                                    if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                                                    {
                                                        Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                                        if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                                        {
                                                            System.IO.File.Delete(imgTempFile.AbsolutePath);
                                                        }
                                                    }

                                                    string pathImage = await Utils.InsertLocalPictureItemMaster(item.ItemStatus.item.PicturePath);
                                                    thumnailLocalPath = pathImage ?? "";
                                                }
                                                else
                                                {
                                                    thumnailLocalPath = data?.ThumbnailLocalPath;
                                                }
                                            }

                                            Item updateItem = new Item()
                                            {
                                                MerchantID = item.ItemStatus.item.MerchantID,
                                                SysItemID = item.ItemStatus.item.SysItemID,
                                                ItemName = item.ItemStatus.item.ItemName,
                                                Ordinary = item.ItemStatus.item.Ordinary,
                                                SysCategoryID = item.ItemStatus.item.SysCategoryID,
                                                ItemCode = item.ItemStatus.item.ItemCode,
                                                ShortName = item.ItemStatus.item.ShortName,
                                                PicturePath = item.ItemStatus.item.PicturePath,
                                                ThumbnailPath = item.ItemStatus.item.ThumbnailPath,
                                                PictureLocalPath = "",
                                                ThumbnailLocalPath = thumnailLocalPath,
                                                Colors = item.ItemStatus.item.Colors,
                                                FavoriteNo = item.ItemStatus.item.FavoriteNo,
                                                UnitName = item.ItemStatus.item.UnitName,
                                                RegularSizeName = item.ItemStatus.item.RegularSizeName,
                                                EstimateCost = item.ItemStatus.item.EstimateCost,
                                                Price = item.ItemStatus.item.Price,
                                                OptSalePrice = item.ItemStatus.item.OptSalePrice,
                                                TaxType = item.ItemStatus.item.TaxType,
                                                SellBy = item.ItemStatus.item.SellBy,
                                                FTrackStock = item.ItemStatus.item.FTrackStock,
                                                TrackStockDateTime = item.ItemStatus.item.TrackStockDateTime,
                                                SaleItemType = item.ItemStatus.item.SaleItemType,
                                                Comments = item.ItemStatus.item.Comments,
                                                LastDateModified = item.ItemStatus.item.LastDateModified,
                                                UserLastModified = item.ItemStatus.item.UserLastModified,
                                                DataStatus = item.ItemStatus.DataStatus,
                                                FWaitSending = 0,
                                                WaitSendingTime = DateTime.UtcNow,
                                                LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                                LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                                FDisplayOption = item.ItemStatus.item.FDisplayOption
                                            };

                                            var insertOrreplace = await itemManage.UpdateItem(updateItem);

                                            var itemSizes = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemExSizes).FirstOrDefault() ?? new List<ORM.Master.ItemExSize>();

                                            itemSizes.ForEach(async itemSize =>
                                            {
                                                await itemExSizeManage.InsertOrReplaceItemSize(new ItemExSize()
                                                {
                                                    MerchantID = itemSize.MerchantID,
                                                    SysItemID = itemSize.SysItemID,
                                                    EstimateCost = itemSize.EstimateCost,
                                                    ExSizeName = itemSize.ExSizeName,
                                                    ExSizeNo = itemSize.ExSizeNo,
                                                    Price = itemSize.Price,
                                                    Comments = itemSize.Comments
                                                });
                                            });
                                        }
                                        maxItemRevision = item.ItemStatus.item.RevisionNo;
                                    });
                                }

                                await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxItem);
                            }
                            Log.Debug("connectpass", "listRivisionItem");
                            DataCashingAll.flagItemChange = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionItem : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxItemRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetOnlineDataitem at ite m");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        List<Item> items;       
        ListItem listItem;
        internal static bool chooseAll = true;
        private int maxitemFilter = 10;

        async Task GetItemList()
        {
            try
            {
                items = new List<Item>();
                items = await itemManage.GetAllItem();
                if (items == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    items = new List<Item>();
                }
                //await SetItemData();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
            }
        }
        private async void SetItemData()
        {
            try
            {
                listItem = new ListItem(items);
                BillHistory_Adapter_FilterItem item_Adapter_Item = new BillHistory_Adapter_FilterItem(listItem, CheckNet);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerview_listitem.SetLayoutManager(gridLayoutItem);
                recyclerview_listitem.HasFixedSize = true;
                int count = items == null ? 0 : items.Count + 1;
                recyclerview_listitem.SetItemViewCacheSize(count);
                recyclerview_listitem.SetAdapter(item_Adapter_Item);
                item_Adapter_Item.ItemClick += Item_Adapter_Item_ItemClick;

                if (item_Adapter_Item.ItemCount == 0)
                {
                    lnSearchItem.Visibility = ViewStates.Gone;
                    recyclerview_listitem.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnSearchItem.Visibility = ViewStates.Visible;
                    recyclerview_listitem.Visibility = ViewStates.Visible;
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetItemData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Item_Adapter_Item_ItemClick(object sender, int e)
        {
            try
            {
                var id = listItem[e].SysItemID;
                var index = chooseItem.FindIndex(x => x.SysItemID == id);
                if (index == -1)
                {
                    chooseItem.Add(listItem[e]);
                }
                else
                {
                    chooseItem.RemoveAt(index);
                    chooseAll = false;
                }
                SetItemData();
                SetBtnApply();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Item_Adapter_Item_ItemClick at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetBtnAll(bool chooseAll)
        {
            if (chooseAll)
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAll.SetTextColor(Android.Graphics.Color.White);
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
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
    }
}