using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using LinqToDB.Common;
using SeAuth2.ORM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Gabana.ShareSource
{
    public class UtilsAll
    {
        static CultureInfo cultureUS = new CultureInfo("en-US");
        public static string ShowDate(DateTime d)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            return datetime;
        }

        public static decimal Stringtodecimal(string s)
        {
            var result = decimal.Parse((s == null || s.ToString() == String.Empty) ? "0" : s.ToString());
            return result;
        }

        public static string ChangeDateTime(DateTime d)
        {
            try
            {
                var destinationTimeZone = TimeZoneInfo.Utc ;
                string datetime = TimeZoneInfo.ConvertTimeFromUtc(d, destinationTimeZone).ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-US"));
                return datetime;
            }
            catch (Exception ex)
            {                
                return d.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-US"));
            }
        }

        public static string ChangeDateTimeUS(DateTime d)
        {
            try
            {
                string datetime = d.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-US"));
                return datetime;
            }
            catch (Exception ex)
            {
                return d.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-US"));
            }
        }

        public static DateTime Culture(DateTime dateTime)
        {
            try
            {
                System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("en-US");
                DateTime dt = DateTime.Parse(dateTime.ToString(), cultureinfo);
                return dt;
            }
            catch (Exception ex)
            {
                return dateTime;
            }
        }

        public static DateTime BirthDayBE(DateTime checkDate)
        {
            var destinationTimeZone = TimeZoneInfo.Local;
            string datetime = TimeZoneInfo.ConvertTimeFromUtc(checkDate, destinationTimeZone).ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            var date = DateTime.ParseExact(datetime, "dd/MM/yyyy", new CultureInfo("en-US"));
            return date;
        }

        public static Boolean CheckPincode()
        {
            try
            {
                var getuseractive = Preferences.Get("UserActive", "");
                if (string.IsNullOrEmpty(getuseractive))
                {
                    DataCashingAll.UserActive = DateTime.Now;
                }
                else
                {
                    DateTime dt;
                    if (DateTime.TryParse(getuseractive, out dt))
                    {
                        DataCashingAll.UserActive = dt;
                    }
                    else
                    {
                        DataCashingAll.UserActive = DateTime.Now;
                    }
                }

                if (DataCashingAll.UserActive == null)
                {
                    Preferences.Set("UserActive", DateTime.Now.ToString());
                    return false;
                }
                else
                {
                    DateTime last = DataCashingAll.UserActive;
                    var dif = DateTime.Now.Minute - last.Minute;
                    if (dif > 5)
                    {
                        Preferences.Set("UserActive", DateTime.Now.ToString());
                        return false;
                    }
                    else
                    {
                        Preferences.Set("UserActive", DateTime.Now.ToString());
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public  static DateTime TranDateformat(DateTime checkDate)
        {
            try
            {
                return checkDate;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return checkDate;
            }
        }

        static public async Task ErrorRevisionNo(int SystemID, int LastRevisionNo)
        {
            try
            {
                //Post SystemRevisionNo
                ORM.Master.DeviceSystemRevisionNo revisionNo = new ORM.Master.DeviceSystemRevisionNo()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    SystemID = SystemID,
                    LastRevisionNo = LastRevisionNo,
                    DeviceNo = DataCashingAll.DeviceNo
                };
                var postRevisionNo = await GabanaAPI.PostDataDeviceSystemRevisionNo(revisionNo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public async Task updateRevisionNo(int SystemID,int LastRevisionNo) 
        {
            try
            {                
                SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();

                //Insert to Local DB
                ORM.MerchantDB.SystemRevisionNo SystemRevisionNo = new ORM.MerchantDB.SystemRevisionNo()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    SystemID = SystemID,
                    LastRevisionNo = LastRevisionNo
                };
                var result = await systemRevisionNoManage.UpdateSystemReviosion(SystemRevisionNo);

                //Post SystemRevisionNo
                ORM.Master.DeviceSystemRevisionNo revisionNo = new ORM.Master.DeviceSystemRevisionNo()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    SystemID = SystemID,
                    LastRevisionNo = LastRevisionNo,
                    DeviceNo = DataCashingAll.DeviceNo
                };
                var postRevisionNo = await GabanaAPI.PostDataDeviceSystemRevisionNo(revisionNo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public async Task<int> GetCountOrder()
        {
            try
            {
                int order = 0;
                List<Gabana3.JAM.Trans.Order> lstCloudOrder = new List<Gabana3.JAM.Trans.Order>();
                List<Tran> lstDeviceOrder = new List<Tran>();
                TransManage transManage = new TransManage();

                lstDeviceOrder = await transManage.GetAllTranOrder(DataCashingAll.MerchantId, DataCashingAll.SysBranchId);

                if (await GabanaAPI.CheckNetWork())
                {
                    lstCloudOrder =  await GabanaAPI.GetDataTranOrder(DataCashingAll.SysBranchId);
                    if (lstCloudOrder != null)
                    {
                        int merge = 0;
                        merge = lstCloudOrder.Where(x => lstDeviceOrder.Select(y => y.TranNo).ToList().Contains(x.tranNo)).ToList().Count();
                        order = lstCloudOrder == null ? 0 : lstCloudOrder.Count() - merge ;
                    }
                    else
                    {
                        order = 0;
                    }                    
                }
                else
                {
                    order =  lstDeviceOrder == null ? 0 : lstDeviceOrder.Count() ;
                }
                return order;
            }
            catch (Exception  ex)
            {
                Console.WriteLine("GetCountOrder : " + ex.Message);
                return 0;
            }
        }

        public static async void PostPrintCounter(int sysBranchID, string tranNo, string tranDate, int localprintCounter, Tran tran)
        {
            try
            {
                Tran getTran = new Tran();
                getTran = tran;
                TransManage transManage = new TransManage();

                if (await GabanaAPI.CheckNetWork())
                {
                    var CloudprintCount = await GabanaAPI.PutDataTranPrintCounter(sysBranchID, tranNo, tranDate, localprintCounter);
                    if (CloudprintCount != 0)
                    {
                        //กรณีสำเร็จ
                        getTran.PrintCounterLocal = 0;
                        getTran.PrintCounter = CloudprintCount;
                        var check = await transManage.UpdatePrintCounterTran(getTran);
                    }
                    else
                    {
                        //กรณีไม่สำเร็จ
                        getTran.PrintCounterLocal = localprintCounter;
                        var check = await transManage.UpdatePrintCounterTran(getTran);
                    }
                }
                else
                {
                    if (getTran != null)
                    {
                        getTran.PrintCounterLocal = localprintCounter + 1;
                        var updatePrint = await transManage.UpdatePrintCounterTran(getTran);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public static bool CheckPermissionRoleUser(string RoleName,string CaseEvent, string Page)
        {
            try
            {
                string _RoleName = RoleName.ToLower();
                string _CaseEvent = CaseEvent.ToLower();
                string _Page = Page.ToLower();
                switch (_CaseEvent)
                {
                    case "insert":
                        if (_Page == "item" || _Page == "topping" || _Page == "stock" || _Page == "category" || _Page == "customer")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager" || _RoleName == "officer")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (_Page == "merchant" || _Page == "empmanage" || _Page == "package" || _Page == "note" || _Page == "membertype"
                            || _Page == "vat" || _Page == "currency" || _Page == "decimal"
                            || _Page == "servicecharge" || _Page == "cash" || _Page == "giftvoucher" || _Page == "myqr" || _Page == "employee")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (_Page == "device" || _Page == "branch" )
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (_Page == "cashdrawer")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager" || _RoleName == "cashier")
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
                            //printer
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager" || _RoleName == "cashier")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        break;
                    case "update":
                        if (_Page == "item" || _Page == "topping" || _Page == "stock" || _Page == "category" || _Page == "customer"|| _Page == "branch")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager")
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        break;
                    case "delete":
                        if (_Page == "item" || _Page == "topping" || _Page == "stock" || _Page == "category" || _Page == "customer" || _Page == "cash" || _Page == "employee")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager")
                            {
                                DataCashingAll.CheckRoleSwipe = true;
                                return true;
                            }
                            else
                            {
                                DataCashingAll.CheckRoleSwipe = false;
                                return false;
                            }
                        }
                        else if (_Page == "void")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager" || _RoleName == "invoice" || _RoleName == "editor")
                            {
                                DataCashingAll.CheckRoleSwipe = true;
                                return true;
                            }
                            else
                            {
                                DataCashingAll.CheckRoleSwipe = false;
                                return false;
                            }
                        }
                        else if (_Page == "branch" || _Page == "membertype" || _Page == "giftvoucher"  || _Page == "note")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" )
                            {
                                DataCashingAll.CheckRoleSwipe = true;
                                return true;
                            }
                            else
                            {
                                DataCashingAll.CheckRoleSwipe = false;
                                return false;
                            }
                        }
                        break;
                    case "view":
                        if (_Page == "item" || _Page == "topping" || _Page == "stock" || _Page == "category" || _Page == "customer")
                        {
                            if (_RoleName == "owner" || _RoleName == "admin" || _RoleName == "manager" )		
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }


        public static string GetExceptionPromotion(string strex, string language)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                strex = strex.Replace("SeAuth2API"," ");
                strex = Regex.Replace(strex, @"[^0-9]+", String.Empty);
                if (language == "th")
                {
                    switch (strex)
                    {
                        case ErrorCodePromotion.Promotion_code_is_null:
                            return "Promotion code is null";
                        case ErrorCodePromotion.MyProductLicence_is_not_found:
                            return "MyProductLicence is not found";
                        case ErrorCodePromotion.This_product_has_already_charged_a_commission:
                            return "This product has already charged a commission";
                        case ErrorCodePromotion.You_have_already_used_the_permissions:
                            return "You have already used the permissions";
                        case ErrorCodePromotion.Sellers_has_been_cancelled:
                            return "Sellers has been cancelled";
                        case ErrorCodePromotion.Bonus_code_has_been_cancelled:
                            return "Bonus code has been cancelled";
                        case ErrorCodePromotion.Bonus_code_expires:
                            return "Bonus code expires";
                        case ErrorCodePromotion.Seller_product_has_been_cancelled:
                            return "Seller product has been cancelled";
                        case ErrorCodePromotion.Seller_product_expires:
                            return "Seller product expires";
                        case ErrorCodePromotion.Promotion_code_is_not_found:
                            return "Promotion code is not found";
                        case ErrorCodePromotion.Promotion_code_has_been_used:
                            return "Promotion code has been used";
                        case ErrorCodePromotion.Promotion_code_has_been_cancelled:
                            return "Promotion code has been cancelled";
                        case ErrorCodePromotion.Promotion_code_expires:
                            return "Promotion code expires";
                        case ErrorCodePromotion.Bonus_code_is_not_found:
                            return "Bonus code is not found";
                        case ErrorCodePromotion.Already_have_gabana_product:
                            return "Already have gabana product";
                        case ErrorCodePromotion.ObfuscatedAccountId_not_found:
                            return "ObfuscatedAccountId not found";
                        case ErrorCodePromotion.Merchant_not_found:
                            return "Merchant not found";
                        default:
                            return strex;
                    }
                }
                else
                {
                    switch (strex)
                    {
                        case ErrorCodePromotion.Promotion_code_is_null:
                            return "Promotion code is null";
                        case ErrorCodePromotion.MyProductLicence_is_not_found:
                            return "MyProductLicence is not found";
                        case ErrorCodePromotion.This_product_has_already_charged_a_commission:
                            return "This product has already charged a commission";
                        case ErrorCodePromotion.You_have_already_used_the_permissions:
                            return "You have already used the permissions";
                        case ErrorCodePromotion.Sellers_has_been_cancelled:
                            return "Sellers has been cancelled";
                        case ErrorCodePromotion.Bonus_code_has_been_cancelled:
                            return "Bonus code has been cancelled";
                        case ErrorCodePromotion.Bonus_code_expires:
                            return "Bonus code expires";
                        case ErrorCodePromotion.Seller_product_has_been_cancelled:
                            return "Seller product has been cancelled";
                        case ErrorCodePromotion.Seller_product_expires:
                            return "Seller product expires";
                        case ErrorCodePromotion.Promotion_code_is_not_found:
                            return "Promotion code is not found";
                        case ErrorCodePromotion.Promotion_code_has_been_used:
                            return "Promotion code has been used";
                        case ErrorCodePromotion.Promotion_code_has_been_cancelled:
                            return "Promotion code has been cancelled";
                        case ErrorCodePromotion.Promotion_code_expires:
                            return "Promotion code expires";
                        case ErrorCodePromotion.Bonus_code_is_not_found:
                            return "Bonus code is not found";
                        case ErrorCodePromotion.Already_have_gabana_product:
                            return "Already have gabana product";
                        case ErrorCodePromotion.ObfuscatedAccountId_not_found:
                            return "ObfuscatedAccountId not found";
                        case ErrorCodePromotion.Merchant_not_found:
                            return "Merchant not found";
                        default:
                            return strex;
                    }
                }
            }
            catch (Exception ex)
            {
                return strex;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        public static string CheckErrorGetToken(string strex)
        {
            try
            {
                string TempStr = string.Empty;
                TempStr = strex;
                strex = strex.Trim().Replace("invalid_grant: ","");
                strex = strex.Trim().Substring(0,5);
                switch (strex)
                {
                    case "C0001":
                        return "UserPassIncorrect";
                    case "C0002":
                        return "CloudProductExpired";
                    case "C0003":
                        return "NotCloudProductLicence";
                    case "C0004":
                        return "NotAllowOwnerLoginWithPassword";
                    case "C0005":
                        return "UserCanNotAccessCloudProduct";
                    default: 
                        return TempStr;
                }
            }
            catch (Exception)
            {
                return strex;
            }
        }

        public static async Task<double> CheckInternetSpeed()
        {
            //DateTime Variable To Store Download Start Time.
            double kbsec = 0, secs = 0;           
            double internetSpeed = 0;
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(20);
                double starttime = Environment.TickCount;

                byte[] data = await httpClient.GetByteArrayAsync("https://lab.seniorsoft.com/gabanaapi/help/speedtest");

                // get current tickcount  
                double endtime = Environment.TickCount;

                // how many seconds did it take?  
                // we are calculating this by subtracting starttime from endtime  
                // and dividing by 1000 (since the tickcount is in miliseconds.. 1000 ms = 1 sec)  
                secs = Math.Floor(endtime - starttime) / 1000;

                // calculate download rate in kb per sec.  
                // this is done by dividing 1024 by the number of seconds it  
                // took to download the file (1024 bytes = 1 kilobyte)  

                if (secs == 0)
                {
                    kbsec = 0;
                }
                else
                {
                    kbsec = Math.Round(10240 / secs);
                }

                internetSpeed = kbsec;               
            }
            catch (Exception ex)
            {
                internetSpeed = 0;
            }
            return internetSpeed;
        }

        public static string CheckAPIStatusCode(HttpResponseMessage res,string contents)
        {
            try
            {
                //switch (res.StatusCode)
                //{
                //    case HttpStatusCode.NoContent:
                //    case HttpStatusCode.BadRequest:
                //    case HttpStatusCode.Unauthorized:
                //    case HttpStatusCode.Forbidden:
                //    case HttpStatusCode.NotFound:
                //    case HttpStatusCode.NotAcceptable:
                //    case HttpStatusCode.RequestTimeout:
                //    case HttpStatusCode.Conflict:
                //    case HttpStatusCode.UnsupportedMediaType:
                //    case HttpStatusCode.InternalServerError:
                //    case HttpStatusCode.NotImplemented:
                //    case HttpStatusCode.BadGateway:
                //    case HttpStatusCode.ServiceUnavailable:
                //    case HttpStatusCode.GatewayTimeout: 
                //    default:
                //        return await res.Content.ReadAsStringAsync();
                //}

                //204
                if (res.StatusCode == HttpStatusCode.NoContent)
                {
                    return "NoContent";
                    //return await res.Content.ReadAsStringAsync();
                }

                //4xx
                if (res.StatusCode == HttpStatusCode.BadRequest)
                {
                    return "BadRequest";
                }
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return "Unauthorized";
                }
                if (res.StatusCode == HttpStatusCode.Forbidden)
                {
                    return "Forbidden";
                }
                if (res.StatusCode == HttpStatusCode.NotFound)
                {
                    return "NotFound";
                }
                if (res.StatusCode == HttpStatusCode.Conflict)
                {
                    return contents;
                }

                //5XX
                if (res.StatusCode == HttpStatusCode.InternalServerError)
                {
                    return "InternalServerError";
                }
                if (res.StatusCode == HttpStatusCode.NotImplemented)
                {
                    return "NotImplemented";
                }
                if (res.StatusCode == HttpStatusCode.BadGateway)
                {
                    return "BadGateway";
                }
                if (res.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    return "ServiceUnavailable";
                }
                if (res.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    return "GatewayTimeout";
                }
                return contents;
            }
            catch (Exception)
            {
                return contents;
            }
        }


        public static char GetLoginTypeToRefreshToken()
        {
            try
            {
                char LoginType = '\0';
                string Type = Preferences.Get("LoginType", "");
                if (Type.ToLower() == "owner")
                {
                    LoginType = 'o';
                }
                else
                {
                    LoginType = 'e';
                }
                return LoginType;
            }
            catch (Exception)
            {
                return '\0';
            }
        }
    }
}
