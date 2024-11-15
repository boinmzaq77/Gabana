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
using Gabana.ShareSource.Manage;

namespace  Gabana.ShareSource
{    
    static public class CategorySync
    {
        static Gabana.ShareSource.Manage.CategoryManage categoryManage = new Gabana.ShareSource.Manage.CategoryManage();

        static public async Task SentCategory(int merchantid, int SentCategory)
        {            
            Category category = new ORM.MerchantDB.Category();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                category = await categoryManage.GetCategorySync(merchantid, SentCategory);

                if (category is null)
                {
                    return;
                }
                if (category.FWaitSending == 0)
                {
                    return;
                }
                if (category.DataStatus == 'N')
                {
                    return;
                }
                switch (category.DataStatus)
                {
                    case 'I':
                        InsertCategory(category);
                        break;
                    case 'M':
                        UpdateCategory(category);
                        break;
                    case 'D':
                        DeleteCategory(category);
                        break;
                    default:
                        break;
                }                
            }
            catch (WebException ex)
            {
                category = await categoryManage.GetCategory(merchantid, SentCategory);
                category.FWaitSending = 2;
                await categoryManage.UpdateCategory(category);
            }
        }

        static public async Task SentCategoryAndroid(int merchantid, int SentCategory)
        {
            Category category = new ORM.MerchantDB.Category();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                category = await categoryManage.GetCategorySyncAndroid(merchantid, SentCategory);

                if (category is null)
                {
                    return;
                }
                if (category.FWaitSending == 0)
                {
                    return;
                }
                if (category.DataStatus == 'N')
                {
                    return;
                }
                switch (category.DataStatus)
                {
                    case 'I':
                        InsertCategory(category);
                        break;
                    case 'M':
                        UpdateCategory(category);
                        break;
                    case 'D':
                        DeleteCategory(category);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                category = await categoryManage.GetCategory(merchantid, SentCategory);
                category.FWaitSending = 2;
                await categoryManage.UpdateCategory(category);
            }
        }

        private static async void InsertCategory(ORM.MerchantDB.Category category)
        {
            try
            {
                Gabana.ORM.Master.Category MCategory = new ORM.Master.Category();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Category, Gabana.ORM.Master.Category>();
                });

                var Imapper = config.CreateMapper();
                var JAMcategory = Imapper.Map<ORM.MerchantDB.Category, Gabana.ORM.Master.Category>(category);
                
               // CultureInfo.CurrentCulture = new CultureInfo("en-US");
                MCategory.MerchantID = JAMcategory.MerchantID;
                MCategory.SysCategoryID = JAMcategory.SysCategoryID;
                MCategory.Ordinary = JAMcategory.Ordinary;
                MCategory.Name = JAMcategory.Name;
                MCategory.DateCreated = JAMcategory.DateCreated;
                MCategory.DateModified = JAMcategory.DateModified;
                MCategory.LinkProMaxxID = JAMcategory.LinkProMaxxID;
                MCategory.RevisionNo = 0;

                if (MCategory == null)
                {
                    return;
                }

                Gabana3.JAM.Category.CategoryWithDeviceNo categoryWithDeviceNo = new Gabana3.JAM.Category.CategoryWithDeviceNo()
                {
                    Category = MCategory,
                    DeviceNo = DataCashingAll.DeviceNo
                };
                var result = await GabanaAPI.PostDataCategory(categoryWithDeviceNo);
                if (result.Status)
                {
                    category.FWaitSending = 0;
                }
                else
                {
                    category.FWaitSending = 2;
                }
                await categoryManage.UpdateCategory(category);
            }
            catch (Exception ex)
            {
                category = await categoryManage.GetCategory((int)category.MerchantID, (int)category.SysCategoryID);
                category.FWaitSending = 2;
                await categoryManage.UpdateCategory(category);
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        private static async void UpdateCategory(ORM.MerchantDB.Category category)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                Gabana.ORM.Master.Category MCategory = new ORM.Master.Category();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Category, Gabana.ORM.Master.Category>();
                });

                var Imapper = config.CreateMapper();
                var JAMcategory = Imapper.Map<ORM.MerchantDB.Category, Gabana.ORM.Master.Category>(category);

                MCategory.MerchantID = JAMcategory.MerchantID;
                MCategory.SysCategoryID = JAMcategory.SysCategoryID;
                MCategory.Ordinary = JAMcategory.Ordinary;
                MCategory.Name = JAMcategory.Name;
                MCategory.DateCreated = JAMcategory.DateCreated;
                MCategory.DateModified = JAMcategory.DateModified;
                MCategory.LinkProMaxxID = JAMcategory.LinkProMaxxID;
                MCategory.RevisionNo = 0;

                if (MCategory == null)
                {
                    return;
                }

                Gabana3.JAM.Category.CategoryWithDeviceNo categoryWithDeviceNo = new Gabana3.JAM.Category.CategoryWithDeviceNo()
                {
                    Category = MCategory,
                    DeviceNo = DataCashingAll.DeviceNo
                };
                var result = await GabanaAPI.PutDataCategory(categoryWithDeviceNo);
                if (result.Status)
                {
                    category.FWaitSending = 0;
                }
                else
                {
                    category.FWaitSending = 2;
                }
                await categoryManage.UpdateCategory(category);
            }
            catch (Exception ex)
            {
                category = await categoryManage.GetCategory((int)category.MerchantID, (int)category.SysCategoryID);
                category.FWaitSending = 2;
                await categoryManage.UpdateCategory(category);
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        private static async void DeleteCategory(ORM.MerchantDB.Category category)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                var result = await GabanaAPI.DeleteDataCategory((int)category.SysCategoryID,DataCashingAll.DeviceNo); //return UpdateLastRevisionNo
                if (result.Status)
                {
                    category.FWaitSending = 0;

                    ItemManage itemManage = new ItemManage();
                    //UpdateItem
                    var UpdateItem = await itemManage.GetItembyCategory((int)category.MerchantID, (int)category.SysCategoryID);
                    if (UpdateItem != null)
                    {
                        foreach (var update in UpdateItem)
                        {
                            update.SysCategoryID = null;
                            var resultUpdate = await itemManage.UpdateItem(update);
                        }
                    }

                    //delete ที่ local
                    var delete = await categoryManage.DeleteCategory((int)category.MerchantID, (int)category.SysCategoryID);
                    if (!delete)
                    {
                        category.DataStatus = 'D';
                        category.FWaitSending = 2;
                        await categoryManage.UpdateCategory(category); 
                    }
                }
                else
                {
                    category.FWaitSending = 2;
                }
                await categoryManage.UpdateCategory(category);                
            }
            catch (WebException ex)
            {
                category = await categoryManage.GetCategory((int)category.MerchantID, (int)category.SysCategoryID);
                category.FWaitSending = 2;
                await categoryManage.UpdateCategory(category);
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }
    }
}
