using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Sync
{
    static public class CustomerSync
    {
        static Gabana.ShareSource.Manage.CustomerManage CustomerManage = new Gabana.ShareSource.Manage.CustomerManage();
        static ORM.Master.Customer Mastercustomer = new ORM.Master.Customer();        

        static public async Task SentCustomer(int merchantid, int SysCustomerID, byte[] ImageByte)
        {
            byte[] ImageByteArray = ImageByte;
            Gabana.ORM.MerchantDB.Customer LocalCustomer = new ORM.MerchantDB.Customer();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                LocalCustomer = await CustomerManage.GetCustomer(merchantid, SysCustomerID);               
                LocalCustomer.LastDateModified = LocalCustomer.LastDateModified;
                LocalCustomer.WaitSendingTime = LocalCustomer.WaitSendingTime;
                if (LocalCustomer.BirthDate != null)
                {
                    var checkDate = LocalCustomer.BirthDate;
                    var date = UtilsAll.BirthDayBE(checkDate ?? DateTime.UtcNow);
                    LocalCustomer.BirthDate = date;
                }               

                if (LocalCustomer is null)
                {
                    return;
                }
                if (LocalCustomer.FWaitSending == 0)
                {
                    return;
                }
                if (LocalCustomer.DataStatus == 'N')
                {
                    return;
                }
                switch (LocalCustomer.DataStatus)
                {
                    case 'I':
                        InsertCustomer(LocalCustomer, ImageByteArray);
                        break;
                    case 'M':
                        UpdateCustomer(LocalCustomer, ImageByteArray);
                        break;
                    case 'D':
                        DeleteCustomer(LocalCustomer);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                var Localcustomer = await CustomerManage.GetCustomer(merchantid, SysCustomerID);
                Localcustomer.FWaitSending = 2;
                await CustomerManage.UpdateCustomer(Localcustomer);
            }
        }

        static public async Task SentCustomerAndroid(int merchantid, int SysCustomerID, byte[] ImageByte)
        {
            byte[] ImageByteArray = ImageByte;
            Gabana.ORM.MerchantDB.Customer LocalCustomer = new ORM.MerchantDB.Customer();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                LocalCustomer = await CustomerManage.GetCustomerAndroid(merchantid, SysCustomerID);
                LocalCustomer.LastDateModified = LocalCustomer.LastDateModified;
                LocalCustomer.WaitSendingTime = LocalCustomer.WaitSendingTime;
                if (LocalCustomer.BirthDate != null)
                {
                    var checkDate = LocalCustomer.BirthDate;
                    var date = UtilsAll.BirthDayBE(checkDate ?? DateTime.UtcNow);
                    LocalCustomer.BirthDate = date;
                }

                if (LocalCustomer is null)
                {
                    return;
                }
                if (LocalCustomer.FWaitSending == 0)
                {
                    return;
                }
                if (LocalCustomer.DataStatus == 'N')
                {
                    return;
                }
                switch (LocalCustomer.DataStatus)
                {
                    case 'I':
                        InsertCustomer(LocalCustomer, ImageByteArray);
                        break;
                    case 'M':
                        UpdateCustomer(LocalCustomer, ImageByteArray);
                        break;
                    case 'D':
                        DeleteCustomer(LocalCustomer);
                        break;
                    default:
                        break;
                }
            }
            catch (WebException ex)
            {
                var Localcustomer = await CustomerManage.GetCustomer(merchantid, SysCustomerID);
                Localcustomer.FWaitSending = 2;
                await CustomerManage.UpdateCustomer(Localcustomer);
            }

        }

        private static async void InsertCustomer(Gabana.ORM.MerchantDB.Customer LocalCustomer, byte[] ImageByte)
        {

            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Customer, Gabana.ORM.Master.Customer>();
                });

                var Imapper = config.CreateMapper();
                var JAMCustomer= Imapper.Map<ORM.MerchantDB.Customer, Gabana.ORM.Master.Customer>(LocalCustomer);

                Mastercustomer = JAMCustomer;
                Mastercustomer.LastDateModified = DateTime.UtcNow;
               
                if (Mastercustomer == null)
                {
                    return;
                }

                var result = await GabanaAPI.PostDataCustomer(Mastercustomer, ImageByte);
                if (result.Status)
                {
                    LocalCustomer.FWaitSending = 0;
                    //LocalCustomer.PictureLocalPath = null;
                }
                else
                {
                    LocalCustomer.FWaitSending = 2;
                }
                await CustomerManage.UpdateCustomer(LocalCustomer);
            }
            catch (WebException ex)
            {
                LocalCustomer = await CustomerManage.GetCustomer((int)LocalCustomer.MerchantID, (int)LocalCustomer.SysCustomerID);
                LocalCustomer.FWaitSending = 2;
                await CustomerManage.UpdateCustomer(LocalCustomer);
            }

        }

        private static async void UpdateCustomer(Gabana.ORM.MerchantDB.Customer LocalCustomer, byte[] ImageByte)
        {

            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Customer, Gabana.ORM.Master.Customer>();
                });

                var Imapper = config.CreateMapper();
                var JAMCustomer = Imapper.Map<ORM.MerchantDB.Customer, Gabana.ORM.Master.Customer>(LocalCustomer);  

                Mastercustomer = JAMCustomer;
                Mastercustomer.LastDateModified = DateTime.UtcNow;

                if (Mastercustomer == null)
                {
                    return;
                }

                var result = await GabanaAPI.PutDataCustomer(Mastercustomer, ImageByte);
                if (result.Status)
                {
                    LocalCustomer.FWaitSending = 0;
                    //LocalCustomer.PictureLocalPath = null;
                }
                else
                {
                    LocalCustomer.FWaitSending = 2;
                }
                await CustomerManage.UpdateCustomer(LocalCustomer);
            }
            catch (WebException ex)
            {
                LocalCustomer = await CustomerManage.GetCustomer((int)LocalCustomer.MerchantID, (int)LocalCustomer.SysCustomerID);
                LocalCustomer.FWaitSending = 2;
                await CustomerManage.UpdateCustomer(LocalCustomer);
            }

        }

        private static async void DeleteCustomer(Gabana.ORM.MerchantDB.Customer LocalCustomer)
        {

            try
            {
                var result = await GabanaAPI.DeleteDataCustomer((int)LocalCustomer.SysCustomerID,DataCashingAll.DeviceNo); //return UpdateLastRevisionNo
                if (result.Status)
                {
                    LocalCustomer.FWaitSending = 0;

                    //Delete Item ที่ Local
                    var deleteItem = await CustomerManage.DeleteCustomer((int)LocalCustomer.MerchantID, (int)LocalCustomer.SysCustomerID);
                    if (!deleteItem)
                    {
                        LocalCustomer.FWaitSending = 2;
                        await CustomerManage.UpdateCustomer(LocalCustomer);
                    }
                }
                else
                {
                    LocalCustomer.FWaitSending = 2;
                }
                await CustomerManage.UpdateCustomer(LocalCustomer);
            }
            catch (WebException ex)
            {
                LocalCustomer = await CustomerManage.GetCustomer((int)LocalCustomer.MerchantID, (int)LocalCustomer.SysCustomerID);
                LocalCustomer.FWaitSending = 2;
                await CustomerManage.UpdateCustomer(LocalCustomer);
            }

        }

        
    }
}
