using Gabana.Model;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using Gabana.ShareSource;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gabana.ORM.MerchantDB;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Gabana3.JAM.Items;
using AutoMapper;
using System.Globalization;

namespace Gabana.ShareSource
{

    static public class ItemSync
    {
        static Gabana.ShareSource.Manage.ItemManage itemManager = new Gabana.ShareSource.Manage.ItemManage();
        static Gabana.ShareSource.Manage.ItemExSizeManage ItemExSizeManage = new Manage.ItemExSizeManage();
        static Gabana.ShareSource.Manage.ItemOnBranchManage ItemOnBranchManage = new Manage.ItemOnBranchManage();

        public static ItemStatus ItemStatus { get; private set; }
        static ItemWithItemExSizes itemWithItemEx = new ItemWithItemExSizes();
        static JsonOfItemWithItemExSizes jsonOfItem = new JsonOfItemWithItemExSizes();

        static public async Task SentItem(int merchantid, int SysItemID, byte[] ImageByte)
        {
            byte[] ImageByteArray = ImageByte;
            Item item = new Item();
            List<ItemExSize> itemEXSize = new List<ItemExSize>();
            ItemOnBranch itemBranch = new ItemOnBranch();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                item = await itemManager.GetItemforSync(merchantid, SysItemID);
                itemEXSize = await ItemExSizeManage.GetItemSize(merchantid, SysItemID);
                itemBranch = await ItemOnBranchManage.GetItemOnBranch(merchantid, DataCashingAll.SysBranchId, SysItemID);

                if (item is null)
                {
                    return;
                }
                if (item.FWaitSending == 0)
                {
                    return;
                }

                if (item.DataStatus == 'N')
                {
                    return;
                }
                switch (item.DataStatus)
                {
                    case 'I':
                        InsertItem(item, itemEXSize, itemBranch, ImageByteArray);
                        break;
                    case 'M':
                        UpdateItem(item, itemEXSize, ImageByteArray);
                        break;
                    case 'D':
                        DeleteItem(item);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                item = await itemManager.GetItem(merchantid, SysItemID);
                item.FWaitSending = 2;
                await itemManager.UpdateItem(item);
            }
        }

        static public async Task SentItemAndroid(int merchantid, int SysItemID, byte[] ImageByte)
        {
            byte[] ImageByteArray = ImageByte;
            Item item = new Item();
            List<ItemExSize> itemEXSize = new List<ItemExSize>();
            ItemOnBranch itemBranch = new ItemOnBranch();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                item = await itemManager.GetItemforSyncAndroid(merchantid, SysItemID);
                itemEXSize = await ItemExSizeManage.GetItemSize(merchantid, SysItemID);
                itemBranch = await ItemOnBranchManage.GetItemOnBranch(merchantid, DataCashingAll.SysBranchId, SysItemID);

                if (item is null)
                {
                    return;
                }
                if (item.FWaitSending == 0)
                {
                    return;
                }

                if (item.DataStatus == 'N')
                {
                    return;
                }
                switch (item.DataStatus)
                {
                    case 'I':
                        InsertItem(item, itemEXSize, itemBranch, ImageByteArray);
                        break;
                    case 'M':
                        UpdateItem(item, itemEXSize, ImageByteArray);
                        break;
                    case 'D':
                        DeleteItem(item);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                item = await itemManager.GetItem(merchantid, SysItemID);
                item.FWaitSending = 2;
                await itemManager.UpdateItem(item);
            }
        }

        private async static void DeleteItem(Item item)
        {
            try
            {
                var result = await GabanaAPI.DeleteDataItem((int)item.SysItemID,DataCashingAll.DeviceNo); //return UpdateLastRevisionNo
                if (result.Status)
                {
                    item.FWaitSending = 0;

                    //Delete ItemOnBranch                   
                    var deleteItemOnBranch = await ItemOnBranchManage.DeleteItemOnBranch((int)item.MerchantID, DataCashingAll.SysBranchId,(int)item.SysItemID);

                    //Delete ItemSize                     
                    var deleteItemSize = await ItemExSizeManage.DeleteItemsize((int)item.MerchantID, (int)item.SysItemID);

                    //Delete Item ที่ Local
                    var deleteItem = await itemManager.DeleteItem((int)item.MerchantID, (int)item.SysItemID);
                    if (!deleteItem)
                    {
                        await itemManager.UpdateItem(item);
                    }
                }
                else
                {
                    item.FWaitSending = 2;
                }
                await itemManager.UpdateItem(item);                 
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                item = await itemManager.GetItem((int)item.MerchantID, (int)item.SysItemID);
                item.FWaitSending = 2;
                await itemManager.UpdateItem(item);
            }
        }

        private async static void UpdateItem(Item item, List<ItemExSize> itemEXSize, byte[] ImageByte)
        {
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.ItemExSize, ORM.Master.ItemExSize>();
                });

                var Imapper = config.CreateMapper();
                var JAMitemExSizes = Imapper.Map<List<ORM.MerchantDB.ItemExSize>, List<ORM.Master.ItemExSize>>(itemEXSize);

                int? category;
                if (item.SysCategoryID == null)
                {
                    category = null;
                }
                else
                {
                    category = (int)item.SysCategoryID;
                }

                int? Ordinary;
                if (item.Ordinary == null)
                {
                    Ordinary = null;
                }
                else
                {
                    Ordinary = (int)item.Ordinary;
                }

                itemWithItemEx = new Gabana3.JAM.Items.ItemWithItemExSizes()
                {
                    item = new ORM.Master.Item()
                    {
                        MerchantID = (int)item.MerchantID,
                        SysItemID = (int)item.SysItemID,
                        ItemName = item.ItemName,
                        Ordinary = (int)item.Ordinary,
                        SysCategoryID = category,
                        ItemCode = item.ItemCode,
                        ShortName = item.ShortName,
                        PicturePath = item.PicturePath,
                        Colors = (int)item.Colors,
                        FavoriteNo = (int)item.FavoriteNo,
                        UnitName = item.UnitName,
                        RegularSizeName = item.RegularSizeName,
                        EstimateCost = item.EstimateCost,
                        Price = Convert.ToDecimal(item.Price),
                        OptSalePrice = item.OptSalePrice,
                        TaxType = item.TaxType,
                        SellBy = item.SellBy,
                        FTrackStock = item.FTrackStock,
                        TrackStockDateTime = item.TrackStockDateTime,
                        SaleItemType = item.SaleItemType,
                        Comments = item.Comments,
                        LastDateModified = item.LastDateModified,
                        UserLastModified = item.UserLastModified,
                        LinkProMaxxItemID = item.LinkProMaxxItemID,
                        LinkProMaxxItemUnit = item.LinkProMaxxItemUnit,
                        ThumbnailPath = item.ThumbnailPath,
                        FDisplayOption = item.FDisplayOption
                    },
                    itemExSizes = JAMitemExSizes,
                    ItemStatus = null
                };

                jsonOfItem = new JsonOfItemWithItemExSizes();
                jsonOfItem.value = JsonConvert.SerializeObject(itemWithItemEx);
                jsonOfItem.DeviceNo = DataCashingAll.DeviceNo;

                var result = await GabanaAPI.PutDataItem(jsonOfItem, ImageByte);
                if (result.Status)
                {
                    item.FWaitSending = 0;
                    //item.PictureLocalPath = null;
                }
                else
                {
                    item.FWaitSending = 2;
                }
                await itemManager.UpdateItem(item);
            }
            catch (WebException ex)
            {
                item = await itemManager.GetItem((int)item.MerchantID, (int)item.SysItemID);
                item.FWaitSending = 2;
                await itemManager.UpdateItem(item);
                Console.WriteLine(ex.Message);
            }
        }

        private static async void InsertItem(Item item, List<ItemExSize> itemEXSize,ItemOnBranch itemOnBranch, byte[] ImageByte)
        {
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.ItemExSize, ORM.Master.ItemExSize>();
                });

                var Imapper = config.CreateMapper();
                var JAMitemExSizes = Imapper.Map<List<ORM.MerchantDB.ItemExSize>, List<ORM.Master.ItemExSize>>(itemEXSize);

                if (JAMitemExSizes == null)
                {
                    JAMitemExSizes = new List<ORM.Master.ItemExSize>();
                }

                int? category;
                if (item.SysCategoryID == null)
                {
                    category = null;
                }
                else
                {
                    category = (int)item.SysCategoryID;
                }

                int? Ordinary;
                if (item.Ordinary == null)
                {
                    Ordinary = null;
                }
                else
                {
                    Ordinary = (int)item.Ordinary;
                }

                ORM.Master.Item insertitem = new ORM.Master.Item()
                {
                    MerchantID = (int)item.MerchantID,
                    SysItemID = (int)item.SysItemID,
                    ItemName = item.ItemName,
                    Ordinary = Ordinary,
                    SysCategoryID = category,
                    ItemCode = item.ItemCode,
                    ShortName = item.ShortName,
                    PicturePath = item.PicturePath,
                    Colors = (int)item.Colors,
                    FavoriteNo = (int)item.FavoriteNo,
                    UnitName = item.UnitName,
                    RegularSizeName = item.RegularSizeName,
                    EstimateCost = item.EstimateCost,
                    Price = Convert.ToDecimal(item.Price),
                    OptSalePrice = item.OptSalePrice,
                    TaxType = item.TaxType,
                    SellBy = item.SellBy,
                    FTrackStock = item.FTrackStock,
                    TrackStockDateTime = item.TrackStockDateTime,
                    SaleItemType = item.SaleItemType,
                    Comments = item.Comments,
                    LastDateModified = item.LastDateModified,
                    UserLastModified = item.UserLastModified,
                    LinkProMaxxItemID = item.LinkProMaxxItemID,
                    LinkProMaxxItemUnit = item.LinkProMaxxItemUnit,
                    ThumbnailPath = item.ThumbnailPath,
                    FDisplayOption = item.FDisplayOption,               
                };                

                itemWithItemEx = new Gabana3.JAM.Items.ItemWithItemExSizes()
                {                    
                    item = insertitem,
                    itemExSizes = JAMitemExSizes,
                    ItemStatus = null,
                };

                jsonOfItem = new JsonOfItemWithItemExSizes();
                jsonOfItem.value = JsonConvert.SerializeObject(itemWithItemEx);
                jsonOfItem.DeviceNo = DataCashingAll.DeviceNo;
                if (itemOnBranch != null)
                {
                    jsonOfItem.balanceStock = itemOnBranch.BalanceStock;
                    jsonOfItem.minimumStock = itemOnBranch.MinimumStock;
                }                
                jsonOfItem.sysBranchID = DataCashingAll.SysBranchId;
                                
                var result = await GabanaAPI.PostDataItem(jsonOfItem, ImageByte);
                if (result.Status)
                {
                    item.FWaitSending = 0;
                    //item.PictureLocalPath = null;
                }
                else
                {
                    item.FWaitSending = 2;
                }
                await itemManager.UpdateItem(item);
            }
            catch (WebException ex)
            {
                item = await itemManager.GetItem((int)item.MerchantID, (int)item.SysItemID);
                item.FWaitSending = 2;
                await itemManager.UpdateItem(item);
                Console.WriteLine(ex.Message);
            }
        }

    }
}
