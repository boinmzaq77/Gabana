using Acr.Logging;
using Foundation;
using Gabana.iOS;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana
{
    internal class NotificationManager
    {
        int maxCategoryRevision = 0;
        int maxItemRevision = 0;
        int maxCustomerRevision = 0;
        int maxNoteCategoryRevision = 0;
        int maxNoteRevision = 0;

        List<ORM.Master.MerchantConfig> listmerchantConfig = new List<ORM.Master.MerchantConfig>();
        List<ORM.Master.MemberType> listmembertype = new List<ORM.Master.MemberType>();
        Gabana3.JAM.Merchant.Merchants merchants = new Gabana3.JAM.Merchant.Merchants();
        List<ORM.Master.MyQrCode> myqrcodes = new List<ORM.Master.MyQrCode>();
        List<ORM.Master.GiftVoucher> listgiftVouchers = new List<ORM.Master.GiftVoucher>();
        List<ORM.Master.Branch> getMerchant = new List<ORM.Master.Branch>();
        List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();

        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNo SystemRevisionNo;
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        CategoryManage categoryManage = new CategoryManage();
        Category category = new Category();
        ItemManage itemManage = new ItemManage();
        ItemExSizeManage itemExSizeManage = new ItemExSizeManage();
        Item getItem = new Item();
        ItemExSize getitemSize = new ItemExSize();
        DeviceSystemSeqNo deviceSystemSeqNo = new DeviceSystemSeqNo();
        DeviceTranRunningNoManage deviceTranRunningNoManage = new DeviceTranRunningNoManage();
        DeviceTranRunningNo deviceTranRunning = new DeviceTranRunningNo();
        CustomerManage customerManage = new CustomerManage();
        Customer customer = new Customer();
        DiscountTemplateManage discountTemplateManage = new DiscountTemplateManage();
        DiscountTemplate discount = new DiscountTemplate();
        NoteManage noteManage = new NoteManage();
        Note note = new Note();
        NoteCategoryManage noteCategoryManage = new NoteCategoryManage();
        NoteCategory noteCategory = new NoteCategory();
        ItemOnBranchManage onBranchManage = new ItemOnBranchManage();




        internal async void Noti(string action, string message)
        {
            try
            {


                //Utils.ShowMessage(message);
                //var page = Utils.GetCurrentUIController();

                switch (action)
                {
                    case Gabana3.DataModel.BellNotificationHub.Action.ITEM:
                          ItemChange();
                        Utils.ShowMessage(Utils.TextBundle("item_noti", "Item have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.CATEGORY:
                          CategoryChange(); 
                        Utils.ShowMessage(Utils.TextBundle("category_noti", "Category have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.NOTE:
                          NoteChange(); 
                        Utils.ShowMessage(Utils.TextBundle("note_noti", "Note have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.NOTECATEGORY:
                          NoteCategoryChange();
                        Utils.ShowMessage(Utils.TextBundle("notecategory_noti", "Note Category have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.CUSTOMER:
                          CustomerChange(); 
                        Utils.ShowMessage(Utils.TextBundle("customer_noti", "Customer have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.MERCHANTCONFIG:
                         MerchantConfigChange(); 
                        Utils.ShowMessage(Utils.TextBundle("merchantconfig_noti", "Setting have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.ITEMONBRANCH:
                         ItemOnBranchChange();
                        Utils.ShowMessage(Utils.TextBundle("itemonbranch_noti", "Stock have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.MEMBERTYPE:
                          MemberTypeChange(); 
                        Utils.ShowMessage(Utils.TextBundle("membertype_noti", "Member type have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.MERCHANT:
                         MerchantChange(); 
                        Utils.ShowMessage(Utils.TextBundle("merchant_noti", "Merchant have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.MYQRCODE:
                         MyQrCodeChange();
                        Utils.ShowMessage(Utils.TextBundle("myqrcode_noti", "My QR Code have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.GIFTVOUCHER:
                          GiftVoucherChange();
                        Utils.ShowMessage(Utils.TextBundle("giftvoucher_noti", "Gift Voucher have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.BRANCH:
                          BranchChange(); 
                        Utils.ShowMessage(Utils.TextBundle("branch_noti", "Branch have been changed."));
                        break;
                    case Gabana3.DataModel.BellNotificationHub.Action.CASHTEMPLATE:
                          CashTemplateChange();
                        Utils.ShowMessage(Utils.TextBundle("cashtemplate", "Cash Template have been changed."));
                        break;
                    default:

                        break;

                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }




        public async Task ItemChange()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision == null)
                {
                    return;
                }
                for (int i = 0; i < listRivision.Count; i++)
                {
                    if (listRivision[i].SystemID == 30)
                    {
                        #region Item
                        //------------------------------------------------
                        //Get Item API
                        //offset = index สำหรับเรียกข้อมูล ครั้งละ 100 ตัว เริ่มที่ 0
                        //total >= 100 item = 0 - 99     รอบที่ 1  offset = 0
                        //             item = 100 - 199  รอบที่ 2  offset = 1
                        //total > 100  => totalitem/100 = จำนวนรอบที่เรียก 
                        //------------------------------------------------
                        try
                        {
                            var allItem = await GabanaAPI.GetDataItem((int)listRivision[i].LastRevisionNo, 0);

                            if (allItem == null)
                            {
                                break;
                            }
                            else if (allItem?.ItemsWithItemExSizes.Count == 0)
                            {
                                break;
                            }
                            else
                            {
                                int round = allItem.totalItems / 100;
                                int addrount = round + 1;
                                for (int j = 0; j < addrount; j++)
                                {
                                    allItem = await GabanaAPI.GetDataItem((int)listRivision[i].LastRevisionNo, j);

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

                                    foreach (var item in allItem.ItemsWithItemExSizes)
                                    {
                                        var data = await itemManage.GetItem(item.ItemStatus.item.MerchantID, item.ItemStatus.item.SysItemID);

                                        if (item.ItemStatus.DataStatus == 'D')
                                        {
                                            List<ORM.Master.ItemOnBranch> itemOnBranch = allItem.ItemsWithItemExSizes.Where(x => x.ItemStatus.item.SysItemID == item.ItemStatus.item.SysItemID).Select(x => x.itemOnBranchs).FirstOrDefault() ?? new List<ORM.Master.ItemOnBranch>();
                                            await Utils.DeleteItem(data, itemOnBranch);
                                        }
                                        else
                                        {
                                            //insertorreplace
                                            getItem = new Item()
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
                                                PictureLocalPath = data?.PictureLocalPath,
                                                ThumbnailLocalPath = data?.ThumbnailLocalPath,
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
                                                DataStatus = 'I',
                                                FWaitSending = 1,
                                                WaitSendingTime = DateTime.UtcNow,
                                                LinkProMaxxItemID = item.ItemStatus.item.LinkProMaxxItemID,
                                                LinkProMaxxItemUnit = item.ItemStatus.item.LinkProMaxxItemUnit,
                                                FDisplayOption = item.ItemStatus.item.FDisplayOption
                                            };

                                            var insertOrreplace = await itemManage.InsertOrReplaceItem(getItem);

                                            if (!string.IsNullOrEmpty(getItem.PicturePath))
                                            {
                                                await Utils.InsertLocalPictureItem(getItem);
                                            }

                                            foreach (var itemSize in item.itemExSizes)
                                            {
                                                getitemSize = new ItemExSize()
                                                {
                                                    MerchantID = itemSize.MerchantID,
                                                    SysItemID = itemSize.SysItemID,
                                                    EstimateCost = itemSize.EstimateCost,
                                                    ExSizeName = itemSize.ExSizeName,
                                                    ExSizeNo = itemSize.ExSizeNo,
                                                    Price = itemSize.Price,
                                                    Comments = itemSize.Comments
                                                };
                                                var insertItemSize = await itemExSizeManage.InsertOrReplaceItemSize(getitemSize);
                                            }
                                        }
                                        maxItemRevision = item.ItemStatus.item.RevisionNo;
                                    }
                                    await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxItem);

                                    //Set flag สำหรับ Reload ข้อมูลที่เครื่อง
                                    
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxItemRevision);
                            _ = TinyInsights.TrackErrorAsync(ex);
                        }
                        #endregion
                    }
                }
                if (DataCaching.posPage != null )
                {
                    DataCaching.posPage.SearchBytxt();
                }
                if (DataCaching.itempage != null)
                {
                    DataCaching.itempage.SearchBytxt();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemChange");
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        

        public async Task CategoryChange()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision == null)
                {
                    return;
                }
                for (int i = 0; i < listRivision.Count; i++)
                {
                    if (listRivision[i].SystemID == 20)
                    {
                        #region Category
                        try
                        {
                            //Get Category API
                            var allcategory = await GabanaAPI.GetDataCategory((int)listRivision[i].LastRevisionNo);

                            if (allcategory == null)
                            {
                                return;
                            }

                            int maxCategory = 0;
                            if (allcategory.Categories.Count > 0)
                            {
                                allcategory.Categories.ToList().OrderBy(x => x.RevisionNo);
                                maxCategory = allcategory.Categories.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                                foreach (var item in allcategory.Categories)
                                {
                                    //insertorreplace
                                    category = new Category()
                                    {
                                        MerchantID = item.MerchantID,
                                        SysCategoryID = item.SysCategoryID,
                                        Ordinary = item.Ordinary,
                                        Name = item.Name,
                                        DateCreated = item.DateCreated,
                                        DateModified = item.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 1,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxID = item.LinkProMaxxID
                                    };
                                    var insertOrreplace = await categoryManage.InsertOrReplaceCategory(category);
                                    maxCategoryRevision = item.RevisionNo;
                                }
                            }

                            if (allcategory.CategoryBins.Count > 0)
                            {
                                allcategory.CategoryBins.ToList().OrderBy(x => x.RevisionNo);
                                maxCategory = allcategory.CategoryBins.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();
                                foreach (var item in allcategory.CategoryBins)
                                {
                                    //UpdateItem
                                    var UpdateItem = await itemManage.GetItembyCategory(item.MerchantID, item.SysCategoryID);
                                    if (UpdateItem != null)
                                    {
                                        foreach (var update in UpdateItem)
                                        {
                                            update.SysCategoryID = null;
                                            var resultUpdate = await itemManage.UpdateItem(update);
                                        }
                                    }
                                    //delete
                                    var delete = await categoryManage.DeleteCategory(item.MerchantID, item.SysCategoryID);
                                    maxCategoryRevision = item.RevisionNo;
                                }
                            }
                            await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxCategory);
                            //Set flag สำหรับ Reload ข้อมูลที่เครื่อง
                            DataCashingAll.flagCategoryChange = true;
                            
                        }
                        catch (Exception ex )
                        {
                            await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxCategoryRevision);
                            _ = TinyInsights.TrackErrorAsync(ex);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CategoryChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task NoteChange()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision == null)
                {
                    return;
                }
                for (int i = 0; i < listRivision.Count; i++)
                {
                    if (listRivision[i].SystemID == 70)
                    {
                        #region Note                            
                        try
                        {
                            //Get NoteCategory API
                            var allNote = await GabanaAPI.GetDataNotes((int)listRivision[i].LastRevisionNo, 0);

                            if (allNote == null)
                            {
                                break;
                            }

                            if (allNote.totalNotes == 0)
                            {
                                break;
                            }

                            int round = allNote.totalNotes / 100;
                            int addrount = round + 1;
                            for (int j = 0; j < addrount; j++)
                            {
                                allNote = await GabanaAPI.GetDataNotes((int)listRivision[i].LastRevisionNo, j);

                                if (allNote == null)
                                {
                                    break;
                                }

                                if (allNote.totalNotes == 0)
                                {
                                    break;
                                }

                                allNote.noteWithNoteStatuses.ToList().OrderBy(x => x.note.RevisionNo);
                                var maxNote = allNote.noteWithNoteStatuses.ToList().Max(x => x.note.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();                                                             

                                foreach (var item in allNote.noteWithNoteStatuses)
                                {
                                    if (item.DataStatus == 'D')
                                    {
                                        //delete
                                        var delete = await noteManage.DeleteNote(item.note.MerchantID, item.note.SysNoteID);
                                    }
                                    else
                                    {
                                        //insertorreplace
                                        note = new Note()
                                        {
                                            MerchantID = item.note.MerchantID,
                                            SysNoteID = item.note.SysNoteID,
                                            Ordinary = item.note.Ordinary,
                                            Message = item.note.Message,
                                            SysNoteCategoryID = item.note.SysNoteCategoryID,
                                            LastDateModified = item.note.LastDateModified,
                                            UserLastModified = item.note.UserLastModified,
                                            DataStatus = 'I',
                                            FWaitSending = 1,
                                            WaitSendingTime = DateTime.UtcNow
                                        };
                                        var insertOrreplace = await noteManage.InsertOrReplaceNote(note);
                                    }
                                    maxNoteRevision = item.note.RevisionNo;
                                }
                                await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxNote);
                                //Set flag สำหรับ Reload ข้อมูลที่เครื่อง
                                DataCashingAll.flagNoteChange = true;
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxNoteRevision);
                            _ = TinyInsights.TrackErrorAsync(ex);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task NoteCategoryChange()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision == null)
                {
                    return;
                }
                for (int i = 0; i < listRivision.Count; i++)
                {
                    if (listRivision[i].SystemID == 60)
                    {
                        #region NoteCategory
                        try
                        {
                            var allNoteCategory = await GabanaAPI.GetDataNoteCategory((int)listRivision[i].LastRevisionNo);

                            if (allNoteCategory == null)
                            {
                                break;
                            }

                            int maxNoteCategory = 0;
                            if (allNoteCategory.NoteCategory.Count > 0)
                            {
                                allNoteCategory.NoteCategory.ToList().OrderBy(x => x.RevisionNo);
                                maxNoteCategory = allNoteCategory.NoteCategory.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  

                                //insertorreplace
                                foreach (var item in allNoteCategory.NoteCategory)
                                {
                                    noteCategory = new NoteCategory()
                                    {
                                        MerchantID = item.MerchantID,
                                        SysNoteCategoryID = item.SysNoteCategoryID,
                                        Ordinary = item.Ordinary,
                                        Name = item.Name,
                                        DateCreated = item.DateCreated,
                                        DateModified = item.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 1,
                                        WaitSendingTime = DateTime.UtcNow
                                    };
                                    var insertOrreplace = await noteCategoryManage.InsertOrReplaceCategory(noteCategory);

                                    maxNoteCategoryRevision = item.RevisionNo;
                                }
                            }

                            if (allNoteCategory.NoteCategoryBin.Count > 0)
                            {
                                allNoteCategory.NoteCategoryBin.ToList().OrderBy(x => x.RevisionNo);
                                maxNoteCategory = allNoteCategory.NoteCategoryBin.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  
                                                                                                                  //delete
                                foreach (var itemDelete in allNoteCategory.NoteCategoryBin)
                                {
                                    //UpdateItem
                                    var UpdateNote = await noteManage.GetNoteBYCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                    if (UpdateNote != null)
                                    {
                                        foreach (var update in UpdateNote)
                                        {
                                            update.SysNoteCategoryID = null;
                                            var resultUpdate = await noteManage.UpdateNote(update);
                                        }
                                    }
                                    var delete = await noteCategoryManage.DeleteNoteCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                    maxNoteCategoryRevision = itemDelete.RevisionNo;
                                }
                            }

                            await UtilsAll.updateRevisionNo((int)listRivision[i].SystemID, maxNoteCategory);
                            //Set flag สำหรับ Reload ข้อมูลที่เครื่อง
                            DataCashingAll.flagNoteCategoryChange = true;
                            
                        }
                        catch (Exception)
                        {
                            await UtilsAll.ErrorRevisionNo((int)listRivision[i].SystemID, maxNoteCategoryRevision);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteCategoryChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task CustomerChange()
        {
            try
            {
                int SysCustomerIdFocus = 0;
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision == null)
                {
                    return;
                }
                SystemRevisionNo revisionNo = new SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 50).FirstOrDefault();
                #region Customer
                try
                {
                    //Get Customer API
                    var allcustomer = await GabanaAPI.GetDataCustomer((int)revisionNo.LastRevisionNo, 0);

                    if (allcustomer == null)
                    {
                        return;
                    }

                    if (allcustomer.totalCustomer == 0)
                    {
                        return;
                    }

                    //check ว่ามีไหม
                    List<Gabana3.JAM.Customer.CustomerStatus> UpdateCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                    List<Gabana3.JAM.Customer.CustomerStatus> InsertCustomer = new List<Gabana3.JAM.Customer.CustomerStatus>();
                    List<Customer> lstCustomerImage = new List<Customer>();
                    int round = 0, addrount = 0;
                    round = allcustomer.totalCustomer / 100;
                    addrount = round + 1;
                    for (int j = 0; j < addrount; j++)
                    {
                        allcustomer = await GabanaAPI.GetDataCustomer((int)revisionNo.LastRevisionNo, j);

                        if (allcustomer == null)
                        {
                            break;
                        }

                        if (allcustomer.totalCustomer == 0)
                        {
                            break;
                        }

                        allcustomer.CustomerStatus.ToList().OrderBy(x => x.Customers.RevisionNo);
                        var maxCustomer = allcustomer.CustomerStatus.ToList().Max(x => x.Customers.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();

                        List<Customer> GetallCustomer = new List<Customer>();
                        GetallCustomer = await customerManage.GetAllCustomer();
                        UpdateCustomer.AddRange(allcustomer.CustomerStatus.Where(x => GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)).ToList());
                        InsertCustomer.AddRange(allcustomer.CustomerStatus.Where(x => !(GetallCustomer.Select(y => (long)y.SysCustomerID).ToList().Contains(x.Customers.SysCustomerID)) && x.DataStatus != 'D').ToList());

                        //Insert Customer
                        if (InsertCustomer.Count > 0)
                        {
                            List<Customer> BulkCustomer = new List<Customer>();
                            foreach (var customer in InsertCustomer)
                            {
                                string thumnailPath = string.Empty;

                                BulkCustomer.Add(new Customer()
                                {
                                    MerchantID = customer.Customers.MerchantID,
                                    SysCustomerID = customer.Customers.SysCustomerID,
                                    CustomerName = customer.Customers.CustomerName,
                                    Ordinary = customer.Customers.Ordinary,
                                    ShortName = customer.Customers.ShortName,
                                    PictureLocalPath = "",
                                    ThumbnailLocalPath = thumnailPath,
                                    EMail = customer.Customers.EMail,
                                    Mobile = customer.Customers.Mobile,
                                    Gender = customer.Customers.Gender,
                                    BirthDate = customer.Customers.BirthDate,
                                    Address = customer.Customers.Address,
                                    ProvincesId = customer.Customers.ProvincesId,
                                    AmphuresId = customer.Customers.AmphuresId,
                                    DistrictsId = customer.Customers.DistrictsId,
                                    PicturePath = customer.Customers.PicturePath, //Clound Image
                                    IDCard = customer.Customers.IDCard,
                                    Comments = customer.Customers.Comments,
                                    LastDateModified = customer.Customers.LastDateModified,
                                    UserLastModified = customer.Customers.UserLastModified,
                                    DataStatus = customer.DataStatus,
                                    FWaitSending = 0,
                                    WaitSendingTime = DateTime.UtcNow,
                                    LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                    MemberTypeNo = customer.Customers.MemberTypeNo,
                                    CustomerID = customer.Customers.CustomerID,
                                    LineID = customer.Customers.LineID,
                                    ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image
                                });
                                maxCustomerRevision = customer.Customers.RevisionNo;
                            }

                            using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                            {
                                try
                                {
                                    await db.BulkCopyAsync(BulkCustomer);
                                }
                                catch (Exception ex)
                                {
                                    var errorRevison = InsertCustomer.Select(x => x.Customers.RevisionNo).Min();
                                    maxCustomerRevision = errorRevison;
                                    Log.Error("connecterror", "BulkCustomer :" + ex.Message);
                                    throw ex;
                                }
                            }

                            lstCustomerImage.AddRange(BulkCustomer);
                        }

                        //Update Customer
                        if (UpdateCustomer.Count > 0)
                        {
                            foreach (var customer in UpdateCustomer)
                            {
                                var data = await customerManage.GetCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);

                                if (customer.DataStatus == 'D')
                                {
                                    //delete รูป
                                    if (!string.IsNullOrEmpty(data?.ThumbnailLocalPath))
                                    {
                                        //Java.IO.File imgTempFile = new Java.IO.File(data?.ThumbnailLocalPath);

                                        //if (System.IO.File.Exists(imgTempFile.AbsolutePath))
                                        //{
                                        //    System.IO.File.Delete(imgTempFile.AbsolutePath);
                                        //}
                                    }
                                    //delete
                                    var delete = await customerManage.DeleteCustomer(customer.Customers.MerchantID, customer.Customers.SysCustomerID);
                                    if (!delete)
                                    {
                                        if (data != null)
                                        {
                                            data.DataStatus = 'D';
                                            data.FWaitSending = 0;
                                            await customerManage.UpdateCustomer(data);
                                        }
                                    }
                                }
                                else
                                {
                                    string thumnailLocalPath = string.Empty;

                                    //insertorreplace
                                    Customer _customer = new Customer()
                                    {
                                        MerchantID = customer.Customers.MerchantID,
                                        SysCustomerID = customer.Customers.SysCustomerID,
                                        CustomerName = customer.Customers.CustomerName,
                                        Ordinary = customer.Customers.Ordinary,
                                        ShortName = customer.Customers.ShortName,
                                        PictureLocalPath = "",
                                        ThumbnailLocalPath = thumnailLocalPath,
                                        EMail = customer.Customers.EMail,
                                        Mobile = customer.Customers.Mobile,
                                        Gender = customer.Customers.Gender,
                                        BirthDate = customer.Customers.BirthDate,
                                        Address = customer.Customers.Address,
                                        ProvincesId = customer.Customers.ProvincesId,
                                        AmphuresId = customer.Customers.AmphuresId,
                                        DistrictsId = customer.Customers.DistrictsId,
                                        PicturePath = customer.Customers.PicturePath, //Clound Image
                                        IDCard = customer.Customers.IDCard,
                                        Comments = customer.Customers.Comments,
                                        LastDateModified = customer.Customers.LastDateModified,
                                        UserLastModified = customer.Customers.UserLastModified,
                                        DataStatus = customer.DataStatus,
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow,
                                        LinkProMaxxID = customer.Customers.LinkProMaxxID,
                                        MemberTypeNo = customer.Customers.MemberTypeNo,
                                        CustomerID = customer.Customers.CustomerID,
                                        LineID = customer.Customers.LineID,
                                        ThumbnailPath = customer.Customers.ThumbnailPath, //Clound Image

                                    };
                                    var insertOrreplace = await customerManage.InsertOrReplaceCustomer(_customer);
                                }

                                maxCustomerRevision = customer.Customers.RevisionNo;
                            }
                        }
                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxCustomer);
                    }
                    //insert Image to Local เมื่อเพิ่มข้อมูลทั้งหมดสำเร็จ ทั้งเคสเพิ่มและเคสอัปเดต
                    Log.Debug("connectpass", "InsertPictureLocalCustomer(lstCustomerImage) lstCustomerImage : " + lstCustomerImage.Count);
                    Task.Factory.StartNew(() => Utils.InsertPictureLocalCustomer(lstCustomerImage));
                    Log.Debug("connectpass", "UpdateImageCustomer(UpdateCustomer) UpdateCustomer : " + UpdateCustomer.Count);
                    Task.Factory.StartNew(() => Utils.UpdateImageCustomer(UpdateCustomer));

                    Log.Debug("connectpass", "listRivisionCustomer");
                }
                catch (Exception ex)
                {
                    Log.Debug("connecterror", "listRivisionCustomer : " + ex.Message);
                    await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxCustomerRevision);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CustomerChange");
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public async Task MerchantConfigChange()
        {
            try
            {
                List<MerchantConfig> lstconfig = new List<MerchantConfig>();
                SetMerchantConfig setconfig = new SetMerchantConfig();
                MerchantConfigManage merchantconfigManage = new MerchantConfigManage();

                listmerchantConfig = await GabanaAPI.GetDataMerchantConfig();
                if (listmerchantConfig == null)
                {
                    return;
                }
                if (listmerchantConfig.Count > 0)
                {
                    foreach (var item in listmerchantConfig)
                    {
                        MerchantConfig config = new MerchantConfig()
                        {
                            MerchantID = item.MerchantID,
                            CfgKey = item.CfgKey,
                            CfgInteger = item.CfgInteger,
                            CfgFloat = item.CfgFloat,
                            CfgString = item.CfgString,
                            CfgDate = item.CfgDate
                        };
                        var InsertorReplace = await merchantconfigManage.InsertorReplacrMerchantConfig(config);
                        if (InsertorReplace)
                        {
                            lstconfig.Add(config);
                        }
                    }

                    #region merchantConfig
                    var TAXTYPE = lstconfig.Where(x => x.CfgKey == "TAXTYPE").FirstOrDefault();
                    if (TAXTYPE != null)
                    {
                        setconfig.TAXTYPE = TAXTYPE.CfgString;
                    }

                    var TAXRATE = lstconfig.Where(x => x.CfgKey == "TAXRATE").FirstOrDefault();
                    if (TAXRATE != null)
                    {
                        setconfig.TAXRATE = TAXRATE.CfgFloat.ToString();
                    }

                    var CURRENCY_SYMBOLS = lstconfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                    if (CURRENCY_SYMBOLS != null)
                    {
                        if (CURRENCY_SYMBOLS.CfgString == "")
                        {
                            CURRENCY_SYMBOLS.CfgString = " ";
                        }
                        setconfig.CURRENCY_SYMBOLS = CURRENCY_SYMBOLS.CfgString;
                    }

                    var DECIMAL_POINT_CALC = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_CALC").FirstOrDefault();
                    if (DECIMAL_POINT_CALC != null)
                    {
                        setconfig.DECIMAL_POINT_CALC = DECIMAL_POINT_CALC.CfgInteger.ToString();
                    }

                    var DECIMAL_POINT_DISPLAY = lstconfig.Where(x => x.CfgKey == "DECIMAL_POINT_DISPLAY").FirstOrDefault();
                    if (DECIMAL_POINT_DISPLAY != null)
                    {
                        setconfig.DECIMAL_POINT_DISPLAY = DECIMAL_POINT_DISPLAY.CfgInteger.ToString();
                    }

                    var OPTION_ROUNDING = lstconfig.Where(x => x.CfgKey == "OPTION_ROUNDING").FirstOrDefault();
                    if (OPTION_ROUNDING != null)
                    {
                        setconfig.OPTION_ROUNDING_STRING = OPTION_ROUNDING.CfgString;
                        setconfig.OPTION_ROUNDING_INT = OPTION_ROUNDING.CfgInteger.ToString();
                    }

                    var SERVICECHARGE_TYPE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_TYPE").FirstOrDefault();
                    if (SERVICECHARGE_TYPE != null)
                    {
                        setconfig.SERVICECHARGE_TYPE = SERVICECHARGE_TYPE.CfgString;
                    }

                    var SERVICECHARGE_RATE = lstconfig.Where(x => x.CfgKey == "SERVICECHARGE_RATE").FirstOrDefault();
                    if (SERVICECHARGE_RATE != null)
                    {
                        setconfig.SERVICECHARGE_RATE = SERVICECHARGE_RATE.CfgString;
                    }

                    var PRINTER_DEFAULT = lstconfig.Where(x => x.CfgKey == "PRINTER_DEFAULT").FirstOrDefault();
                    if (PRINTER_DEFAULT != null)
                    {
                        setconfig.PRINTER_DEFAULT = PRINTER_DEFAULT.CfgString;
                    }

                    var SUBSCRIPTION_TYPE = lstconfig.Where(x => x.CfgKey == "SUBSCRIPTION_TYPE").FirstOrDefault();
                    if (SUBSCRIPTION_TYPE != null)
                    {
                        setconfig.SUBSCRIPTION_TYPE = SUBSCRIPTION_TYPE.CfgString;
                    }
                    #endregion

                    var merchantConfig = JsonConvert.SerializeObject(setconfig);
                    Preferences.Set("SetmerchantConfig", merchantConfig);
                    var setmerchantConfig = Preferences.Get("SetmerchantConfig", "");
                    if (setmerchantConfig != "")
                    {
                        var Config = JsonConvert.DeserializeObject<SetMerchantConfig>(setmerchantConfig);
                        DataCashingAll.setmerchantConfig = Config;
                    }
                }
            }
            catch (Exception ex)
            {
                string text = "MerchantConfigChange";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task ItemOnBranchChange()
        {
            int maxItemOnBranchRevision = 0;
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                if (listRivision==null)
                {
                    return;
                }
                var revisionNo = listRivision.Where(x => x.SystemID == 31).FirstOrDefault();
                var allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, 0);
                if (allItemOnBranch == null)
                {

                    return;
                }

                if (allItemOnBranch.totalItemOnBranch == 0)
                {

                    return;
                }

                if (allItemOnBranch.totalItemOnBranch > 0)
                {
                    //RunOnUiThread(async ()  =>
                    //{
                    int percent = 0;
                    double round = 0, addrount = 0;
                    round = allItemOnBranch.totalItemOnBranch / 100;
                    addrount = round + 1;
                    double percentage = 0, temp = 0;
                    percentage = (20 / addrount);
                    temp = percentage;
                    percentage = 0;

                    for (int j = 0; j < addrount; j++)
                    {
                        allItemOnBranch = await GabanaAPI.GetDataItemOnBranchV2((int)revisionNo.LastRevisionNo, j); 

                        if (allItemOnBranch == null)
                        {
                            break;
                        }

                        if (allItemOnBranch.totalItemOnBranch == 0)
                        {
                            break;
                        }

                        allItemOnBranch.ItemOnBranches.OrderBy(x => x.RevisionNo);
                        var maxItemOnBranch = allItemOnBranch.ItemOnBranches.Max(x => x.RevisionNo);

                        //check ว่ามีไหม
                        List<ORM.Master.ItemOnBranch> UpdateItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                        List<ORM.Master.ItemOnBranch> InsertItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                        List<ItemOnBranch> GetallItemonBranch = await onBranchManage.GetAllItemOnBranchOnlyMerchantID(DataCashingAll.MerchantId);
                        UpdateItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID)).ToList());
                        InsertItemOnBranch.AddRange(allItemOnBranch.ItemOnBranches.Where(x => !(GetallItemonBranch.Select(y => (long)y.SysItemID).ToList().Contains(x.SysItemID))).ToList());

                        //Insert ItemonBranch
                        if (InsertItemOnBranch.Count > 0)
                        {
                            List<ItemOnBranch> BulkItemOnBranch = new List<ItemOnBranch>();

                            foreach (var itemOnBranch in InsertItemOnBranch)
                            {
                                BulkItemOnBranch.Add(new ItemOnBranch()
                                {
                                    MerchantID = itemOnBranch.MerchantID,
                                    SysBranchID = itemOnBranch.SysBranchID,
                                    SysItemID = itemOnBranch.SysItemID,
                                    BalanceStock = itemOnBranch.BalanceStock,
                                    MinimumStock = itemOnBranch.MinimumStock,
                                    LastDateBalanceStock = itemOnBranch.LastDateBalanceStock,
                                });
                            }

                            using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                            {
                                try
                                {
                                    await db.BulkCopyAsync(BulkItemOnBranch);
                                }
                                catch (Exception ex)
                                {
                                    var errorRevison = InsertItemOnBranch.Select(x => x.RevisionNo).Min();
                                    maxItemOnBranchRevision = errorRevison;
                                    Log.Error("connecterror", "BulkItemOnBranch :" + ex.Message);
                                }
                            }
                        }

                        //Update ItemonBranch
                        if (UpdateItemOnBranch.Count > 0)
                        {
                            foreach (var item in UpdateItemOnBranch)
                            {
                                ItemOnBranch stock = new ItemOnBranch()
                                {
                                    MerchantID = item.MerchantID,
                                    SysBranchID = item.SysBranchID,
                                    SysItemID = item.SysItemID,
                                    BalanceStock = item.BalanceStock,
                                    MinimumStock = item.MinimumStock,
                                    LastDateBalanceStock = item.LastDateBalanceStock,
                                };
                                var insertStock = await onBranchManage.InsertorReplaceItemOnBranch(stock);
                                maxItemOnBranchRevision = item.RevisionNo;
                            }
                        }

                        await UtilsAll.updateRevisionNo(31, maxItemOnBranch);

                    }
                    Log.Debug("connectpass", "listRivisionItemOnBranch");
                    //});
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemOnVBranchChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task MemberTypeChange()
        {
            try
            {
                MemberTypeManage memberTypeManage = new MemberTypeManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    listmembertype = await GabanaAPI.GetDataMemberType();
                    if (listmembertype.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมดก่อน
                        var Allmember = await memberTypeManage.DeleteAllMemberType(DataCashingAll.MerchantId);

                        var lstmember = new List<MemberType>();
                        foreach (var item in listmembertype)
                        {
                            MemberType memberType = new MemberType()
                            {
                                DateModified = item.DateModified,
                                LinkProMaxxID = item.LinkProMaxxID,
                                MemberTypeName = item.MemberTypeName,
                                MemberTypeNo = item.MemberTypeNo,
                                MerchantID = item.MerchantID,
                                PercentDiscount = item.PercentDiscount
                            };
                            var InsertorReplace = await memberTypeManage.InsertorReplacrMemberType(memberType);
                            lstmember.Add(memberType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MemberTypeChange at MyFirebaseMessagingService");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task MerchantChange()
        {
            try
            {

                merchants = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
                if (merchants != null)
                {
                    //insert to local
                    MerchantManage merchantManage = new MerchantManage();
                    Merchant merchantlocal = new Merchant();
                    merchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                    //merchant
                    Merchant merchant = new Merchant()
                    {
                        MerchantID = merchants.Merchant.MerchantID,
                        Name = merchants.Merchant.Name,
                        FMasterMerchant = merchants.Merchant.FMasterMerchant,
                        RefMasterMerchantID = merchants.Merchant.MerchantID,
                        Status = merchants.Merchant.Status,
                        DateOpenMerchant = merchants.Merchant.DateOpenMerchant,
                        RefPackageID = merchants.Merchant.RefPackageID,
                        DayOfPeriod = merchants.Merchant.DayOfPeriod,
                        DueDate = merchants.Merchant.DueDate,
                        LanguageCountryCode = merchants.Merchant.LanguageCountryCode,
                        TimeZoneName = merchants.Merchant.TimeZoneName,
                        TimeZoneUTCOffset = merchants.Merchant.TimeZoneUTCOffset,
                        LogoPath = merchants.Merchant.LogoPath,
                        DateCreated = Utils.GetTranDate(merchants.Merchant.DateCreated),
                        DateModified = Utils.GetTranDate(merchants.Merchant.DateModified),
                        UserNameModified = merchants.Merchant.UserNameModified,
                        DateCloseMerchant = merchants.Merchant.DateCloseMerchant,
                        FPrivacyPolicy = merchants.Merchant.FPrivacyPolicy,
                        FTermConditions = merchants.Merchant.FTermConditions,
                        LogoLocalPath = merchantlocal.LogoLocalPath == null ? null : merchantlocal.LogoLocalPath,
                        RegMark = merchants.Merchant.RegMark,
                        TaxID = merchants.Merchant.TaxID
                    };
                    var result = await merchantManage.UpdateMerchant(merchant);
                    if (result)
                    {
                        string pathClound = merchants.Merchant.LogoPath;
                        string imagePath = Utils.SplitCloundPath(pathClound);
                        var GETmerchantlocal = await merchantManage.GetMerchant(merchants.Merchant.MerchantID);
                        if (Utils.SplitPath(GETmerchantlocal.LogoLocalPath) != imagePath)
                        {
                            GETmerchantlocal.LogoPath = merchants.Merchant.LogoPath;
                            await merchantManage.UpdateMerchant(GETmerchantlocal);
                            await Utils.InsertLocalPictureMerchant(GETmerchantlocal);
                        }

                        DataCashingAll.Merchant = merchants;
                        DataCashingAll.MerchantLocal = GETmerchantlocal;
                        Preferences.Set("MerchantID", (int)GETmerchantlocal.MerchantID);
                        DataCashingAll.MerchantId = Preferences.Get("MerchantID", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MerchantChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task MyQrCodeChange()
        {
            try
            {
                MyQrCodeManage QrCodeManage = new MyQrCodeManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    myqrcodes = await GabanaAPI.GetDataMyQrCode();
                    if (myqrcodes == null)
                    {
                        return;
                    }
                    if (myqrcodes.Count > 0)
                    {
                        //ลบข้อมูลทังหมดก่อน
                        var AllQR = await QrCodeManage.DeleteAllMyQrCode(DataCashingAll.MerchantId);

                        var lst = myqrcodes.OrderBy(x => x.MyQrCodeNo).ToList();
                        foreach (var item in lst)
                        {
                            ORM.MerchantDB.MyQrCode myQrCode = new ORM.MerchantDB.MyQrCode()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified,
                                Comments = item.Comments,
                                FMyQrAllBranch = item.FMyQrAllBranch,
                                MyQrCodeName = item.MyQrCodeName,
                                MyQrCodeNo = item.MyQrCodeNo,
                                PicturePath = item.PicturePath,
                                PictureLocalPath = item.PicturePath,
                                SysBranchID = item.SysBranchID  // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : DataCashingAll.SysBranchId 
                            };
                            await QrCodeManage.InsertOrReplaceMyQrCode(myQrCode);
                            await Utils.InsertLocalPictureQrcode(myQrCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("MyQrCodeChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task GiftVoucherChange()
        {
            try
            {
                GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    listgiftVouchers = await GabanaAPI.GetDataGiftVoucher();
                    if (listgiftVouchers == null)
                    {
                        return;
                    }
                    if (listgiftVouchers.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมด
                        var Allgifts = await giftVoucherManage.DeleteAllGiftVoucher(DataCashingAll.MerchantId);

                        var lst = listgiftVouchers.OrderBy(x => x.FmlAmount).ToList();
                        foreach (var item in lst)
                        {
                            ORM.MerchantDB.GiftVoucher giftVoucher = new GiftVoucher()
                            {
                                DateCreated = item.DateCreated,
                                DateModified = item.DateModified,
                                FmlAmount = item.FmlAmount,
                                GiftVoucherCode = item.GiftVoucherCode,
                                GiftVoucherName = item.GiftVoucherName,
                                MerchantID = item.MerchantID,
                                Ordinary = item.Ordinary,
                                UserNameModified = item.UserNameModified
                            };
                            await giftVoucherManage.InsertOrReplaceGiftVoucher(giftVoucher);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GiftVoucherChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task BranchChange()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                MerchantManage merchantManage = new MerchantManage();
                ORM.MerchantDB.Branch insertBranch = new ORM.MerchantDB.Branch();
                getMerchant = await GabanaAPI.GetDataBranch();
                if (getMerchant == null)
                {
                    return;
                }
                for (int i = 0; i < getMerchant.Count; i++)
                {
                    insertBranch.MerchantID = getMerchant[i].MerchantID;
                    insertBranch.SysBranchID = Convert.ToInt64(getMerchant[i].BranchID);
                    insertBranch.Ordinary = getMerchant[i].Ordinary;
                    insertBranch.BranchName = getMerchant[i].BranchName;
                    insertBranch.BranchID = getMerchant[i].BranchID;
                    insertBranch.Address = getMerchant[i].Address;
                    insertBranch.ProvincesId = getMerchant[i].ProvincesId;
                    insertBranch.AmphuresId = getMerchant[i].AmphuresId;
                    insertBranch.DistrictsId = getMerchant[i].DistrictsId;
                    insertBranch.Status = getMerchant[i].Status;
                    insertBranch.DisplayLanguage = getMerchant[i].DisplayLanguage;
                    insertBranch.Lat = getMerchant[i].Lat;
                    insertBranch.Lng = getMerchant[i].Lng;
                    insertBranch.Email = getMerchant[i].Email;
                    insertBranch.Tel = getMerchant[i].Tel;
                    insertBranch.Line = getMerchant[i].Line;
                    insertBranch.Facebook = getMerchant[i].Facebook;
                    insertBranch.Instagram = getMerchant[i].Instagram;
                    insertBranch.TaxBranchName = getMerchant[i].TaxBranchName;
                    insertBranch.TaxBranchID = getMerchant[i].TaxBranchID;
                    insertBranch.LinkProMaxxID = getMerchant[i].LinkProMaxxID;
                    insertBranch.Comments = getMerchant[i].Comments;

                    var insert = await branchManage.InsertorReplacrBranch(insertBranch);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BranchChange");
                Utils.ShowMessage(ex.Message);
            }
        }

        public async Task CashTemplateChange()
        {
            try
            {
                CashTemplateManage templateManage = new CashTemplateManage();
                if (await GabanaAPI.CheckNetWork())
                {
                    listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                    if (listcashTemplate == null)
                    {
                        return;
                    }
                    if (listcashTemplate.Count > 0)
                    {
                        //ลบข้อมูลทั้งหมด
                        var delete = await templateManage.DeleteAllCashTemplatee(DataCashingAll.MerchantId);

                        var lst = new List<CashTemplate>();
                        foreach (var item in listcashTemplate)
                        {
                            CashTemplate cashTemplate = new CashTemplate()
                            {
                                Amount = item.Amount,
                                CashTemplateNo = item.CashTemplateNo,
                                DateModified = item.DateModified,
                                MerchantID = item.MerchantID,
                            };
                            var InsertorReplace = await templateManage.InsertorReplaceCashTemplate(cashTemplate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CashTemplateChange at MyFirebaseMessagingService");
                Utils.ShowMessage(ex.Message);
            }
        }





    }

}