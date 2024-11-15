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
using System.Web;
using Gabana3.JAM.Report;
using Xamarin.Essentials;
using LinqToDB.Common;
using static System.Net.Mime.MediaTypeNames;
using Gabana3.JAM.PubSub;

namespace Gabana.ShareSource
{
    static public class GabanaAPI
    {
        public static string ccJWT;
        public static string gbnJWT;
        public static async Task verifyCustomToken()
        {
            try
            {
                if (!await GabanaAPI.isValidToken(GabanaAPI.gbnJWT))
                {
                    char LoginType = UtilsAll.GetLoginTypeToRefreshToken();
                    gbnJWT = await Gabana.ShareSource.GetToken.RefreshToken(Preferences.Get("RefreshToken", ""), LoginType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static public async Task<bool> CheckNetWork()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {  
                    var url = Constants.Gabana3API + Constants.urlCheckNet;
                    httpClient.Timeout = TimeSpan.FromMinutes(0.5);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }
        }
        static public async Task<bool> RenewiOS(RenewiOS renewiOS)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(renewiOS);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Ngrok + Constants.RenewiOS;
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }
        }

        static public async Task<bool> CheckSpeedConnection()
        {
            try
            {
                double speed = await UtilsAll.CheckInternetSpeed();
                if (speed < 5000)
                {
                    return false;
                }

                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + Constants.urlCheckNet;
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }
        }

        static public async Task<ResultAPI> CheckSingUpOrLoginGabana(string OwnerID)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    //http://192.168.200.240:8002/OTP/CheckSingUpOrLoginGabana?OwnerID=0915683140   
                    var url = Constants.SeAuth2APIHost + Constants.urlCheckSingUpOrLoginGabana + "?OwnerID=" + OwnerID;
                    var jwt = ccJWT;
                    httpClient.SetBearerToken(jwt);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    #region old Code
                    //if (res.StatusCode == HttpStatusCode.NoContent)
                    //{
                    //    if (string.IsNullOrEmpty(contents))
                    //    {
                    //        throw new WebException("Content is NULL");
                    //    }
                    //    else
                    //    {
                    //        throw new WebException(contents);
                    //    }
                    //}
                    //return true; 
                    #endregion

                    if (res.StatusCode == HttpStatusCode.NoContent)
                    {
                        string message = ((HttpStatusCode)res.StatusCode).ToString();
                        throw new WebException(message);
                    }

                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> urlInitialCreateGabana(SendOTP sendOTP)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(sendOTP);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt = ccJWT;
                    httpClient.SetBearerToken(jwt);
                    var res = await httpClient.PostAsync(Constants.SeAuth2APIHost + Constants.urlInitialCreateGabana, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> urlInitialLoginGabana(SendOTP sendOTP)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(sendOTP);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt = ccJWT;
                    httpClient.SetBearerToken(jwt);
                    var res = await httpClient.PostAsync(Constants.SeAuth2APIHost + Constants.urlInitialLoginGabana, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> CreateGabana(VerifyOTP GiftoryProfile)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(GiftoryProfile);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt = ccJWT;
                    httpClient.SetBearerToken(jwt);
                    var res = await httpClient.PostAsync(Constants.SeAuth2APIHost + Constants.urlCreateGabana, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> LoginGabana(VerifyOTP GiftoryProfile)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(GiftoryProfile);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt = ccJWT;
                    httpClient.SetBearerToken(jwt);
                    var res = await httpClient.PostAsync(Constants.SeAuth2APIHost + Constants.urlLoginGabana, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }


        static public async Task<bool> isValidToken(string jwt)
        {
            try
            {
                if (jwt != "")
                {
                    // NOTE: For that code to work, you need install System.IdentityModel.Tokens.Jwt package from NuGet (the link includes the latest stable version)
                    // Link: https://www.nuget.org/packages/System.IdentityModel.Tokens.Jwt/4.0.2.206221351


                    var token = new JwtSecurityToken(jwtEncodedString: jwt);
                    string expString = token.Claims.First(c => c.Type == "exp").Value;
                    long expSecond;
                    long.TryParse(expString, out expSecond);
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expSecond);

                    DateTime expTime = dateTimeOffset.LocalDateTime;
                    DateTime nowTime = DateTime.Now;
                    nowTime.AddMinutes(-5);

                    if (expTime < nowTime)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return false;
            }
        }


        /// ///////////////////////////////////////////////// Merchant  ///////////////////////

        static public async Task<ResultAPI> PostMerchant(Gabana3.JAM.Merchant.Merchants value, byte[] imageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = DataCashingAll.DeviceNo.ToString();

                    var formDataContent = new MultipartFormDataContent();
                    formDataContent.Add(new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"), "Value");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");

                    if (imageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(imageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "FileName" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    //formDataContent                  
                    formDataContent.Add(new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"), "Value");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    var url = Constants.Gabana3API + "v1/Merchant/";
                    var res = await httpClient.PostAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);

                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutMerchant(Gabana3.JAM.Merchant.Merchants value, byte[] imageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = DataCashingAll.DeviceNo.ToString();

                    var formDataContent = new MultipartFormDataContent();
                    var json = JsonConvert.SerializeObject(value);
                    formDataContent.Add(new StringContent(json, Encoding.UTF8, "application/json"), "Value");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");

                    if (imageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(imageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "FileName" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    var url = Constants.Gabana3API + "v1/Merchant/";
                    var res = await httpClient.PutAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<Gabana3.JAM.Merchant.Merchants> GetMerchantDetail(string Platform, string UDID)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Merchant" + "?UDID=" + UDID + "&Platform=" + Platform;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync(); 
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Merchant.Merchants merchantDetail = new Gabana3.JAM.Merchant.Merchants();
                        merchantDetail = JsonConvert.DeserializeObject<Gabana3.JAM.Merchant.Merchants>(contents);
                        return merchantDetail;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<List<Gabana.ORM.Master.MerchantConfig>> GetDataMerchantConfig()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Merchant" + "/" + Constants.MerchantConfig;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.MerchantConfig> MerchantConfig = new List<ORM.Master.MerchantConfig>();
                        MerchantConfig = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.MerchantConfig>>(contents);
                        return MerchantConfig;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PutDataMerchantConfig(List<Gabana.ORM.Master.MerchantConfig> configs, int DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(configs);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    //Constants.SeAuth2APIHostUserAccount + Constants.AddUser + "/1"
                    var url = Constants.Gabana3API + "v1/Merchant" + "/" + Constants.MerchantConfig + "/" + DeviceNo;
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Item  ///////////////////////

        //revisionNo =0, offset เริ่มที่ 1 มี limit ที่ 100 
        static public async Task<Gabana3.JAM.Items.Items> GetDataItem(int revisionNo, int offset)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Items" + "?revisionNo=" + revisionNo + "&offset=" + offset;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Items.Items Item = new Gabana3.JAM.Items.Items();
                        Item = JsonConvert.DeserializeObject<Gabana3.JAM.Items.Items>(contents);
                        return Item;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataItem(Gabana3.JAM.Items.JsonOfItemWithItemExSizes Data, byte[] imageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = Data.DeviceNo.ToString();
                    string balanceStock = Data.balanceStock.ToString() ?? "";
                    string minimumStock = Data.minimumStock.ToString() ?? "";
                    string sysBranchID = Data.sysBranchID.ToString();
                    string value = Data.value;

                    var formDataContent = new MultipartFormDataContent();
                    formDataContent.Add(new StringContent(value, Encoding.UTF8, "application/json"), "value");
                    formDataContent.Add(new StringContent(balanceStock, Encoding.UTF8, "application/json"), "balanceStock");
                    formDataContent.Add(new StringContent(minimumStock, Encoding.UTF8, "application/json"), "minimumStock");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");
                    formDataContent.Add(new StringContent(sysBranchID, Encoding.UTF8, "application/json"), "sysBranchID");

                    if (imageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(imageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "FileName" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Items/";
                    var res = await httpClient.PostAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataItem(Gabana3.JAM.Items.JsonOfItemWithItemExSizes Data, byte[] imageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = Data.DeviceNo.ToString();
                    string value = Data.value;

                    var formDataContent = new MultipartFormDataContent();
                    formDataContent.Add(new StringContent(value, Encoding.UTF8, "application/json"), "value");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");

                    if (imageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(imageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "FileName" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Items/";
                    var res = await httpClient.PutAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataItem(int? SysItemID, int? DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Items" + "?SysItemID=" + SysItemID + "&DeviceNo=" + DeviceNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }


        /////////////////////////////////////// Category  ///////////////////////

        static public async Task<Gabana3.JAM.Category.CategoriesWithCategoryBin> GetDataCategory(int revisionNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Category" + "?revisionNo=" + revisionNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Category.CategoriesWithCategoryBin category = new Gabana3.JAM.Category.CategoriesWithCategoryBin();
                        category = JsonConvert.DeserializeObject<Gabana3.JAM.Category.CategoriesWithCategoryBin>(contents);
                        return category;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataCategory(Gabana3.JAM.Category.CategoryWithDeviceNo value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Category/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataCategory(Gabana3.JAM.Category.CategoryWithDeviceNo value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Category/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataCategory(int? SysCategoryID, int? DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Category" + "?SysCategoryID=" + SysCategoryID + "&DeviceNo=" + DeviceNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Tran  ///////////////////////

        static public async Task<List<TransHistory>> GetDataTranHistory(int sysBranchID, int offset, string latestTranDate)
        {
            try
            {
                await verifyCustomToken();
                //http://192.168.200.240:5001/v1/Trans/History?sysBranchID=1&offset=0&latestTranDate=2021-06-22 09:39:22 AM
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Trans/History" + "?sysBranchID=" + sysBranchID + "&offset=" + offset + "&latestTranDate=" + latestTranDate;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<TransHistory> tranConvert = new List<TransHistory>();
                        tranConvert = JsonConvert.DeserializeObject<List<TransHistory>>(contents);
                        return tranConvert;
                    }
                }
            }
            catch (WebException)
            {
                return new List<TransHistory>();
            }
        }

        static public async Task<Gabana3.JAM.Trans.TranWithDetails> GetDataTranDetail(int sysBranchID, string tranNo, string tranDate)
        {
            try
            {
                await verifyCustomToken();
                //ตัวอย่าง url ที่เรียกข้อมูลได้
                //http://192.168.200.240:5001/v1/Trans/Detail?sysBranchID=1&tranNo=%231-000037&tranDate=6/21/2021
                using (var httpClient = new HttpClient())
                {
                    string tranNoConvert = HttpUtility.UrlEncode(tranNo);

                    var url = Constants.Gabana3API + "v1/Trans/Detail" + "?sysBranchID=" + sysBranchID + "&tranNo=" + tranNoConvert + "&tranDate=" + tranDate;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Trans.TranWithDetails tranConvert = new Gabana3.JAM.Trans.TranWithDetails();
                        tranConvert = JsonConvert.DeserializeObject<Gabana3.JAM.Trans.TranWithDetails>(contents);
                        return tranConvert;
                    }

                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<Gabana3.JAM.Trans.TranWithDetails> GetDataTranSearch(int sysBranchID, string tranNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string tranNoConvert = HttpUtility.UrlEncode(tranNo);
                    var url = Constants.Gabana3API + "v1/Trans/Search" + "?sysBranchID=" + sysBranchID + "&tranNo=" + tranNoConvert;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Trans.TranWithDetails tranConvert = new Gabana3.JAM.Trans.TranWithDetails();
                        tranConvert = JsonConvert.DeserializeObject<Gabana3.JAM.Trans.TranWithDetails>(contents);
                        return tranConvert;
                    }
                }
            }
            catch (WebException ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        static public async Task<List<Gabana3.JAM.Trans.Order>> GetDataTranOrder(int sysBranchID)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Trans/Orders" + "?sysBranchID=" + sysBranchID;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);                    
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana3.JAM.Trans.Order> tranConvert = new List<Gabana3.JAM.Trans.Order>();
                        tranConvert = JsonConvert.DeserializeObject<List<Gabana3.JAM.Trans.Order>>(contents);
                        return tranConvert;
                    }
                }
            }
            catch (WebException ex)
            {
                return null;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        static public async Task<Gabana3.JAM.Trans.TranWithDetails> GetDataTranOrderDetail(int sysBranchID, string tranNo, string tranDate)
        {
            try
            {
                await verifyCustomToken();
                string tranNoConvert = HttpUtility.UrlEncode(tranNo);

                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Trans/OrderDetail" + "?sysBranchID=" + sysBranchID + "&tranNo=" + tranNoConvert + "&tranDate=" + tranDate;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Trans.TranWithDetails tranConvert = new Gabana3.JAM.Trans.TranWithDetails();
                        tranConvert = JsonConvert.DeserializeObject<Gabana3.JAM.Trans.TranWithDetails>(contents);
                        return tranConvert;
                    }
                }
            }
            catch (WebException ex)
            {
                Gabana3.JAM.Trans.TranWithDetails tran = new Gabana3.JAM.Trans.TranWithDetails() { tran = new ORM.Period.Tran { Comments = ex.Message } };
                return tran;
            }
        }

        static public async Task<ResultAPI> PostDataTran(Gabana3.JAM.Trans.JsonOfTranwithDetailsWithFilePicture Data, byte[] imageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string value = Data.value;

                    var formDataContent = new MultipartFormDataContent();
                    formDataContent.Add(new StringContent(value, Encoding.UTF8, "application/json"), "value");

                    if (imageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(imageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "FileName" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Trans/";
                    var res = await httpClient.PostAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
                
            }
        }

        static public async Task<ResultAPI> PutDataTran(int sysBranchID, string tranNo, string tranDate)
        {
            try
            {
                await verifyCustomToken();
                string tranNoConvert = HttpUtility.UrlEncode(tranNo);

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject("");
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Trans/ReturnOrder" + "?sysBranchID=" + sysBranchID + "&tranNo=" + tranNoConvert + "&tranDate=" + tranDate;
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        //Put Trans/PrintCounter หาก success  จะ response PrintCounter กลับไปให้ เอาไป update ที่ MerchantDB
        static public async Task<int> PutDataTranPrintCounter(int sysBranchID, string tranNo, string tranDate, int quantity)
        {
            try
            {
                await verifyCustomToken();
                string tranNoConvert = HttpUtility.UrlEncode(tranNo);

                using (var httpClient = new HttpClient())
                {
                    HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Trans/PrintCounter" + "?sysBranchID=" + sysBranchID + "&tranNo=" + tranNoConvert + "&tranDate=" + tranDate + "&quantity=" + quantity;
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        return Convert.ToInt32(contents);
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        static public async Task<ResultAPI> DeleteDataTran(int sysBranchID, string tranNo, string tranDate)
        {
            try
            {
                await verifyCustomToken();
                //http://192.168.200.240:5001/v1/Trans?sysBranchID=1&tranNo=%231-000088&tranDate=2021-06-24 04:46:56
                string tranNoConvert = HttpUtility.UrlEncode(tranNo);
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Trans" + "?sysBranchID=" + sysBranchID + "&tranNo=" + tranNoConvert + "&tranDate=" + tranDate;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (res.StatusCode == HttpStatusCode.NoContent)
                    {
                        string message = ((HttpStatusCode)res.StatusCode).ToString();
                        throw new WebException(message);
                    }

                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Reviosion  ///////////////////////
        static public async Task<List<SystemRevisionNo>> GetDataSystemRevisionNo()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "/SystemRevisionNo";
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<SystemRevisionNo> lstsystemRevision = new List<SystemRevisionNo>();
                        lstsystemRevision = JsonConvert.DeserializeObject<List<SystemRevisionNo>>(contents);
                        return lstsystemRevision;
                    }
                }
            }
            catch (Exception ex)
            {
                return new List<SystemRevisionNo>();
            }
        }

        static public async Task<ResultAPI> PostDataDeviceSystemRevisionNo(Gabana.ORM.Master.DeviceSystemRevisionNo value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/DeviceSystemRevisionNo/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// DeviceSeqNo  ///////////////////////

        static public async Task<List<ORM.Master.DeviceSystemSeqNo>> GetDataDeviceSeqNo(int DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/DeviceSystemSeqNo" + "?DeviceNo=" + DeviceNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<ORM.Master.DeviceSystemSeqNo> DeviceSeqNo = new List<ORM.Master.DeviceSystemSeqNo>();
                        DeviceSeqNo = JsonConvert.DeserializeObject<List<ORM.Master.DeviceSystemSeqNo>>(contents);
                        return DeviceSeqNo;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /////////////////////////////////////// DeviceTranRunningNo  ///////////////////////

        static public async Task<List<ORM.MerchantDB.DeviceTranRunningNo>> GetDataDeviceTranRunningNo(int DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/DeviceTranRunningNo" + "?DeviceNo=" + DeviceNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<ORM.MerchantDB.DeviceTranRunningNo> DeviceTranRunningNo = new List<DeviceTranRunningNo>();
                        DeviceTranRunningNo = JsonConvert.DeserializeObject<List<ORM.MerchantDB.DeviceTranRunningNo>>(contents);
                        return DeviceTranRunningNo;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /////////////////////////////////////// Customer  ///////////////////////

        static public async Task<Gabana3.JAM.Customer.CustomerWithTotal> GetDataCustomer(int revisionNo, int offset)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Customer?revisionNo=" + revisionNo + "&offset=" + offset;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Customer.CustomerWithTotal customer = new Gabana3.JAM.Customer.CustomerWithTotal();
                        customer = JsonConvert.DeserializeObject<Gabana3.JAM.Customer.CustomerWithTotal>(contents);
                        return customer;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataCustomer(Gabana.ORM.Master.Customer value, byte[] ImageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = DataCashingAll.DeviceNo.ToString();
                    var formDataContent = new MultipartFormDataContent();
                    var json = JsonConvert.SerializeObject(value);
                    formDataContent.Add(new StringContent(json, Encoding.UTF8, "application/json"), "Value");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");
                    if (ImageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(ImageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "aaaaa" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Customer/";
                    var res = await httpClient.PostAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataCustomer(Gabana.ORM.Master.Customer value, byte[] ImageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = DataCashingAll.DeviceNo.ToString();
                    var formDataContent = new MultipartFormDataContent();
                    formDataContent.Add(new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"), "Value");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");
                    if (ImageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(ImageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "waaaaa" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Customer/";
                    var res = await httpClient.PutAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataCustomer(int? SysCustomerID, int? DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Customer" + "?SysCustomerID=" + SysCustomerID + "&DeviceNo=" + DeviceNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Discount  /////////////////////////////
        static public async Task<Gabana3.JAM.DiscountTemplate.DiscountTemplateWithTotal> GetDataDiscountTemplate(int revisionNo, int offset)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/DiscountTemplate/?revisionNo=" + revisionNo + "&offset=" + offset;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.DiscountTemplate.DiscountTemplateWithTotal discount = new Gabana3.JAM.DiscountTemplate.DiscountTemplateWithTotal();
                        discount = JsonConvert.DeserializeObject<Gabana3.JAM.DiscountTemplate.DiscountTemplateWithTotal>(contents);
                        return discount;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataDiscountTemplate(Gabana.ORM.Master.DiscountTemplate value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/DiscountTemplate/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataDiscountTemplate(Gabana.ORM.Master.DiscountTemplate value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/DiscountTemplate/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataDiscountTemplate(int? SysDiscountTemplate, int? DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/DiscountTemplate" + "?SysDiscountTemplate=" + SysDiscountTemplate + "&DeviceNo=" + DeviceNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Notes  /////////////////////////////
        static public async Task<Gabana3.JAM.Notes.Notes> GetDataNotes(int revisionNo, int offset)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Note/?revisionNo=" + revisionNo + "&offset=" + offset;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Notes.Notes note = new Gabana3.JAM.Notes.Notes();
                        note = JsonConvert.DeserializeObject<Gabana3.JAM.Notes.Notes>(contents);
                        return note;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataNotes(Gabana3.JAM.Notes.NoteWithNoteStatus value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Note/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataNotes(Gabana3.JAM.Notes.NoteWithNoteStatus value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2); 
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Note/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataNotes(int? SysNoteID, int? DeviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Note/" + "?SysNoteID=" + SysNoteID + "&DeviceNo=" + DeviceNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// NoteCategory  /////////////////////////////
        static public async Task<Gabana3.JAM.NoteCategory.NoteCategoryWithNoteCategoryBin> GetDataNoteCategory(int revisionNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/NoteCategory/?revisionNo=" + revisionNo;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.NoteCategory.NoteCategoryWithNoteCategoryBin note = new Gabana3.JAM.NoteCategory.NoteCategoryWithNoteCategoryBin();
                        note = JsonConvert.DeserializeObject<Gabana3.JAM.NoteCategory.NoteCategoryWithNoteCategoryBin>(contents);
                        return note;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataNoteCategory(Gabana.ORM.Master.NoteCategory value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/NoteCategory/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataNoteCategory(Gabana.ORM.Master.NoteCategory value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/NoteCategory/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataNoteCategory(int? SysNoteCategoryID)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/NoteCategory/" + "?SysNoteCategoryID=" + SysNoteCategoryID;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }

        }

        /////////////////////////////////////// TrackStock  /////////////////////////////

        //BalanceStock กับ MinimumStock

        static public async Task<ORM.MerchantDB.ItemOnBranch> GetDataStock(int sysBranchID, int sysItemID)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/TrackStock?sysBranchID=" + sysBranchID + "&sysItemID=" + sysItemID;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        ORM.MerchantDB.ItemOnBranch Stock = new ItemOnBranch();
                        Stock = JsonConvert.DeserializeObject<ORM.MerchantDB.ItemOnBranch>(contents);
                        return Stock;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        //ItemMovement
        static public async Task<List<Gabana.Model.ItemMovement>> GetDataStockItemMovement(int sysBranchID, int sysItemID, int offset)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/TrackStock/ItemMovement?sysBranchID=" + sysBranchID + "&sysItemID=" + sysItemID + "&offset=" + offset;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.Model.ItemMovement> ItemMovement = new List<ItemMovement>();
                        ItemMovement = JsonConvert.DeserializeObject<List<Gabana.Model.ItemMovement>>(contents);
                        return ItemMovement;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        //Post/Opeการเปิดระบบ Track Stock
        static public async Task<ResultAPI> PostDataTrackStockOpen(int sysItemID, int deviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(sysItemID);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/TrackStock/Open?sysItemID=" + sysItemID + "&deviceNo=" + deviceNo;
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (res.StatusCode == HttpStatusCode.NoContent)
                    {
                        string message = ((HttpStatusCode)res.StatusCode).ToString();
                        throw new WebException(message);
                    }
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        //Post/Close เป็นการปิดระบบ Track Stock
        static public async Task<ResultAPI> PostDataTrackStockClose(int sysItemID, int deviceNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(sysItemID);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/TrackStock/Close?sysItemID=" + sysItemID + "&deviceNo=" + deviceNo;
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        //Post/Adjust เป็นการ update BalanceStock หรือ MinimumStock
        static public async Task<ResultAPI> PostDataTrackStockAdjust(int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(sysItemID);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/TrackStock/Adjust?sysBranchID=" + sysBranchID + "&sysItemID=" + sysItemID + "&deviceNo=" + deviceNo + "&balanceStock=" + balanceStock + "&minimumStock=" + minimumStock;
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// UserAccount  ///////////////////////

        //SeAuth2API

        static public async Task<List<Model.UserAccountInfo>> GetSeAuthDataListUserAccount()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.SeAuth2APIHostUserAccount + Constants.GetUser + "/1";
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Model.UserAccountInfo> lstuseraccount = new List<Model.UserAccountInfo>();
                        lstuseraccount = JsonConvert.DeserializeObject<List<Model.UserAccountInfo>>(contents);
                        return lstuseraccount;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PutSeAuthDataUserAccessProducts(string username, char status)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(username);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = Constants.SeAuth2APIHostUserAccount + Constants.UserAccessProducts + "/1" + "?username=" + username + "&status=" + status;
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<Gabana.Model.UserAccount> GetSeAuthDataUserAccount(string username)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.SeAuth2APIHostUserAccount + Constants.User + "?username=" + username;
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana.Model.UserAccount useraccount = new UserAccount();
                        useraccount = JsonConvert.DeserializeObject<Gabana.Model.UserAccount>(contents);
                        return useraccount;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostSeAuthDataUserAccount(UserAccount useraccount)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(useraccount);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(Constants.SeAuth2APIHostUserAccount + Constants.AddUser + "/1", httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutSeAuthDataUserAccount(UserAccount useraccount)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(useraccount);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PutAsync(Constants.SeAuth2APIHostUserAccount, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutSeAuthEmployeeDataUserAccount(UserAccount useraccount)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(useraccount);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PutAsync(Constants.SeAuth2APIHostUserAccount + Constants.Profile, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutSeAuthDataChangePassword(ChangePassword password)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(password);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PutAsync(Constants.SeAuth2APIHostUserAccount + Constants.ChangePassword, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutSeAuthDataOnwerChangePassword(ChangePassword password)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(password);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PutAsync(Constants.SeAuth2APIHostUserAccount + Constants.OnwerChangePassword, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteSeAuthDataUserAccount(string username)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.SeAuth2APIHostUserAccount + "?username=" + username + "&cloudproductid=1";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        //GabanaAPI

        static public async Task<Gabana3.JAM.UserAccount.UserAccountResult> GetDataUserAccount(string username)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/UserAccountInfo" + "?username=" + username;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.UserAccount.UserAccountResult userAccountInfo = new Gabana3.JAM.UserAccount.UserAccountResult();
                        userAccountInfo = JsonConvert.DeserializeObject<Gabana3.JAM.UserAccount.UserAccountResult>(contents);
                        return userAccountInfo;
                    }
                }
            }
            catch (WebException)
            {
                //throw ex;
                return null;
            }
        }

        static public async Task<ResultAPI> PostDataUserAccount(Gabana3.JAM.UserAccount.UserAccountResult value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/UserAccountInfo/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataUserAccount(Gabana3.JAM.UserAccount.UserAccountResult value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/UserAccountInfo/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataUserAccount(string username)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/UserAccountInfo" + "?username=" + username;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }


        /////////////////////////////////////// Branch  ///////////////////////

        static public async Task<List<Gabana.ORM.Master.Branch>> GetDataBranch()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Branch";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.Branch> branch = new List<Gabana.ORM.Master.Branch>();
                        branch = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.Branch>>(contents);
                        return branch;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataBranch(ORM.Master.Branch value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Branch/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataBranch(ORM.Master.Branch value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Branch/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataBranch(int sysbranchid)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Branch" + "?sysbranchid=" + sysbranchid;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }

        }

        /////////////////////////////////////// Membertype  ///////////////////////

        static public async Task<List<Gabana.ORM.Master.MemberType>> GetDataMemberType()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/MemberType";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.MemberType> memberType = new List<ORM.Master.MemberType>();
                        memberType = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.MemberType>>(contents);
                        return memberType;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<List<Gabana.ORM.Master.MemberType>> PostDataMemberType(List<ORM.Master.MemberType> value)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/MemberType/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<ORM.Master.MemberType> memberType = new List<ORM.Master.MemberType>();
                        memberType = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.MemberType>>(contents);
                        return memberType;
                    }                    
                }
            }
            catch (WebException ex)
            {
                return null;
            }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        }

        static public async Task<ResultAPI> PutDataMemberType(List<ORM.Master.MemberType> value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/MemberType/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataMemberType(List<ORM.Master.MemberType> value)
        {
            try
            {
                await verifyCustomToken();

                var url = Constants.Gabana3API + "v1/MemberType/";
                var json = JsonConvert.SerializeObject(value);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                request.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());

                var client = new HttpClient();
                var jwt2 = GabanaAPI.gbnJWT;
                client.SetBearerToken(jwt2); 
                client.Timeout = TimeSpan.FromMinutes(1);
                var res = await client.SendAsync(request);
                string contents = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(contents))
                    {
                        throw new WebException(contents);
                    }
                    else
                    {
                        string message = ((HttpStatusCode)res.StatusCode).ToString();
                        throw new WebException(message);
                    }
                }
                return new ResultAPI(true, contents);
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Device  ///////////////////////

        static public async Task<Gabana.ORM.Master.Device> GetDataDevice(string UDID, string Platform)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    // var url = Constants.Gabana3API + "v1/NoteCategory/Adjust/?sysBranchID=" + sysBranchID + "sysItemID=" + sysItemID + "balanceStock=" + balanceStock;
                    var url = Constants.Gabana3API + "v1/Device/?UDID=" + UDID + "&Platform=" + Platform;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana.ORM.Master.Device Device = new ORM.Master.Device();
                        Device = JsonConvert.DeserializeObject<Gabana.ORM.Master.Device>(contents);
                        return Device;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PutDataDevice(Gabana.ORM.Master.Device value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/Device";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Branchpolicy  ///////////////////////

        static public async Task<List<Gabana.ORM.Master.BranchPolicy>> GetDataBranchPolicy()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/BranchPolicy";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.BranchPolicy> branchpolicy = new List<ORM.Master.BranchPolicy>();
                        branchpolicy = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.BranchPolicy>>(contents);
                        return branchpolicy;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> DeleteDataBranchPolicy(string username)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/BranchPolicy" + "?username=" + username;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        /////////////////////////////////////// Dashboard  ///////////////////////

        static public async Task<Gabana3.JAM.Dashboard.DashboardDataModel> GetDataDashboard(int sysBranchID)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/Dashboard?sysBranchID=" + sysBranchID;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.JAM.Dashboard.DashboardDataModel DashboardDataModel = new Gabana3.JAM.Dashboard.DashboardDataModel();
                        DashboardDataModel = JsonConvert.DeserializeObject<Gabana3.JAM.Dashboard.DashboardDataModel>(contents);
                        return DashboardDataModel;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /////////////////////////////////////// Report  ///////////////////////

        // รายงานยอดขาย
        static public async Task<List<Gabana.ORM.Period.SummaryHourly>> GetDataReportSummaryHourly(string sysBranchID, string startDate, string endDate)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    //sysBranchID = "1,10";
                    HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/Sales?sysBranchID=" + sysBranchID + "&startDate=" + startDate + "&endDate=" + endDate;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Period.SummaryHourly> SummaryHourly = new List<ORM.Period.SummaryHourly>();
                        SummaryHourly = JsonConvert.DeserializeObject<List<Gabana.ORM.Period.SummaryHourly>>(contents);
                        return SummaryHourly;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        // รายงานยอดขายตามสาขา
        static public async Task<List<Gabana3.JAM.Report.SalesByBranchModel>> GetDataReportSalesByBranchModel(string startDate, string endDate)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/SalesByBranch?startDate=" + startDate + "&endDate=" + endDate;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana3.JAM.Report.SalesByBranchModel> SalesByBranchModel = new List<SalesByBranchModel>();
                        SalesByBranchModel = JsonConvert.DeserializeObject<List<Gabana3.JAM.Report.SalesByBranchModel>>(contents);
                        return SalesByBranchModel;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }


        // รายงานสินค้าขายดี
        static public async Task<List<Gabana3.JAM.Report.SummaryItemModel>> GetDataReportSummaryDailyItem(string sysBranchID, string startDate, string endDate)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/BestSalesItem?sysBranchID=" + sysBranchID + "&startDate=" + startDate + "&endDate=" + endDate;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana3.JAM.Report.SummaryItemModel> SummaryItemModel = new List<SummaryItemModel>();
                        SummaryItemModel = JsonConvert.DeserializeObject<List<Gabana3.JAM.Report.SummaryItemModel>>(contents);
                        return SummaryItemModel;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        // รายงานยอดขายตาม Category
        static public async Task<List<SalesByCategoryResponse>> GetDataReportSummaryDailyCategory(SalesByCategoryRequest value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/SalesByCategory";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<SalesByCategoryResponse> SummaryDailyCategory = new List<SalesByCategoryResponse>();
                        SummaryDailyCategory = JsonConvert.DeserializeObject<List<SalesByCategoryResponse>>(contents);
                        return SummaryDailyCategory;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        //รายงานยอดขายตาม Payment
        static public async Task<List<SalesByPaymentResponse>> GetDataReportSummaryDailyPayment(SalesByPaymentRequest value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/SalesByPayment";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<SalesByPaymentResponse> SummaryDailyPayment = new List<SalesByPaymentResponse>();
                        SummaryDailyPayment = JsonConvert.DeserializeObject<List<SalesByPaymentResponse>>(contents);
                        return SummaryDailyPayment;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        // รายงานยอดขายตาม Customer
        static public async Task<List<SalesByCustomerResponse>> GetDataReportSummaryDailyCustomer(SalesByCustomerRequest value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/SalesByCustomer";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<SalesByCustomerResponse> SummaryDailyCustomer = new List<SalesByCustomerResponse>();
                        SummaryDailyCustomer = JsonConvert.DeserializeObject<List<SalesByCustomerResponse>>(contents);
                        return SummaryDailyCustomer;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        // รายงานยอดขายตาม Seller
        static public async Task<List<SalesBySellerResponse>> GetDataReportSummaryDailySeller(SalesBySellerRequest value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/SalesBySeller";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<SalesBySellerResponse> SummaryDailySeller = new List<SalesBySellerResponse>();
                        SummaryDailySeller = JsonConvert.DeserializeObject<List<SalesBySellerResponse>>(contents);
                        return SummaryDailySeller;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        // รายงานสินค้าคงเหลือ
        static public async Task<List<Gabana.ORM.Master.ItemOnBranch>> GetDataReportBalanceItemsStock(Gabana3.JAM.Report.ItemsBalanceStockRequest value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = Constants.Gabana3API + "v1/Report/ItemsBalanceStock";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.ItemOnBranch> ItemOnBranch = new List<ORM.Master.ItemOnBranch>();
                        ItemOnBranch = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.ItemOnBranch>>(contents);
                        return ItemOnBranch;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /////////////////////////////////////// MyQrCode  ///////////////////////

        static public async Task<List<Gabana.ORM.Master.MyQrCode>> GetDataMyQrCode()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/MyQrCode";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.MyQrCode> MyQrCode = new List<ORM.Master.MyQrCode>();
                        MyQrCode = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.MyQrCode>>(contents);
                        return MyQrCode;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataMyQrCode(Gabana.ORM.Master.MyQrCode value, byte[] imageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = DataCashingAll.DeviceNo.ToString();

                    var formDataContent = new MultipartFormDataContent();
                    var json = JsonConvert.SerializeObject(value);
                    formDataContent.Add(new StringContent(json, Encoding.UTF8, "application/json"), "Value");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");

                    if (imageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(imageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "FileName" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/MyQrCode";
                    var res = await httpClient.PostAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);

                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataMyQrCode(Gabana.ORM.Master.MyQrCode value, byte[] imageByteArray)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string DeviceNo = DataCashingAll.DeviceNo.ToString();

                    var formDataContent = new MultipartFormDataContent();
                    formDataContent.Add(new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json"), "Value");
                    formDataContent.Add(new StringContent(DeviceNo, Encoding.UTF8, "application/json"), "DeviceNo");
                    if (imageByteArray != null)
                    {
                        ByteArrayContent bytecontent = new ByteArrayContent(imageByteArray);
                        bytecontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "FilePicture", FileName = "FileName" };
                        bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        formDataContent.Add(bytecontent);
                    }

                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/MyQrCode";
                    var res = await httpClient.PutAsync(url, formDataContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    DataCashingAll.imageByteArray = null;
                    return new ResultAPI(true, contents);

                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataMyQrCode(int? myqrcodeno)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/MyQrCode" + "?myqrcodeno=" + myqrcodeno;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        ////////////////////////////////////// GiftVoucher  ///////////////////////

        static public async Task<List<Gabana.ORM.Master.GiftVoucher>> GetDataGiftVoucher()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/GiftVoucher";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.GiftVoucher> GiftVoucher = new List<ORM.Master.GiftVoucher>();
                        GiftVoucher = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.GiftVoucher>>(contents);
                        return GiftVoucher;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<ResultAPI> PostDataGiftVoucher(Gabana.ORM.Master.GiftVoucher value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/GiftVoucher";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> PutDataGiftVoucher(Gabana.ORM.Master.GiftVoucher value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/GiftVoucher";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataGiftVoucher(string giftvouchercode)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/GiftVoucher" + "?giftvouchercode=" + giftvouchercode;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.DeleteAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }


        ////////////////////////////////////// DatabaseInfo  ///////////////////////

        //Get DatabaseInfo/Version จะ response List<ScriptAlterMerchantDB> เรียงลำดับตามเลข er revision และ detailno
        //ให้วน alter จนครบและเอา ErVersionDBInfo ของรายการสุดท้ายมาอัพเดตที่ MerchantDB

        static public async Task<List<Gabana.ORM.Master.ScriptAlterMerchantDB>> GetDataVersion(string erVersion)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "DatabaseInfo/Version?erVersion=" + erVersion;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        var version = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.ScriptAlterMerchantDB>>(contents);
                        return version;
                    }                   
                    
                }
            }
            catch (WebException)
            {
                return null;
            }
        }

        ////////////////////////////////////// ItemOnBranch  ///////////////////////

        static public async Task<List<Gabana.ORM.Master.ItemOnBranch>> GetDataItemOnBranch(int revisionNo, int offset)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/ItemOnBranch" + "?revisionNo=" + revisionNo + "&offset=" + offset;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.ItemOnBranch> itemOnBranches = new List<ORM.Master.ItemOnBranch>();
                        itemOnBranches = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.ItemOnBranch>>(contents);
                        return itemOnBranches;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<Gabana3.DataModel.ItemOnBranch.ItemOnBranchModel> GetDataItemOnBranchV2(int revisionNo, int offset)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v2/ItemOnBranch" + "?revisionNo=" + revisionNo + "&offset=" + offset;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2); 
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana3.DataModel.ItemOnBranch.ItemOnBranchModel itemOnBranches = new Gabana3.DataModel.ItemOnBranch.ItemOnBranchModel();
                        itemOnBranches = JsonConvert.DeserializeObject<Gabana3.DataModel.ItemOnBranch.ItemOnBranchModel>(contents);
                        return itemOnBranches;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        ////////////////////////////////////// CashTemplate  ///////////////////////

        static public async Task<List<Gabana.ORM.Master.CashTemplate>> GetDataCashTemplate()
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/CashTemplate";
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.CashTemplate> CashTemplate = new List<Gabana.ORM.Master.CashTemplate>();
                        CashTemplate = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.CashTemplate>>(contents);
                        return CashTemplate;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        static public async Task<List<Gabana.ORM.Master.CashTemplate>> PostDataCashTemplate(List<ORM.Master.CashTemplate> value)
        {
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {

                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/CashTemplate/";
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        List<Gabana.ORM.Master.CashTemplate> CashTemplate = new List<ORM.Master.CashTemplate>();
                        CashTemplate = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.CashTemplate>>(contents);
                        return CashTemplate;
                    }
                }
            }
            catch (WebException)
            {
                return null;
            }
        }

        static public async Task<ResultAPI> PutDataCashTemplate(List<ORM.Master.CashTemplate> value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/CashTemplate/";
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    return new ResultAPI(true, contents);
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        static public async Task<ResultAPI> DeleteDataCashTemplate(List<ORM.Master.CashTemplate> value)
        {
            try
            {
                await verifyCustomToken();

                var url = Constants.Gabana3API + "v1/CashTemplate/";
                var json = JsonConvert.SerializeObject(value);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                request.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());

                var client = new HttpClient();
                var jwt2 = GabanaAPI.gbnJWT;
                client.SetBearerToken(jwt2);
                client.Timeout = TimeSpan.FromMinutes(1);
                var res = await client.SendAsync(request);
                string contents = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(contents))
                    {
                        throw new WebException(contents);
                    }
                    else
                    {
                        string message = ((HttpStatusCode)res.StatusCode).ToString();
                        throw new WebException(message);
                    }
                }
                return new ResultAPI(true, contents);
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        ////////////////////////////////////// AppConfig  ///////////////////////

        static public async Task<Gabana.ORM.Master.AppConfig> GetDataAppConfig(string key)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.Gabana3API + "v1/AppConfigV1" + "?key=" + key;
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        Gabana.ORM.Master.AppConfig AppConfig = new ORM.Master.AppConfig();
                        AppConfig = JsonConvert.DeserializeObject<Gabana.ORM.Master.AppConfig>(contents);
                        return AppConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ////////////////////////////////////// CloudProductLicence  ///////////////////////

        static public async Task<CloudProductLicence> GetDataCloudProductLicence()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.SeAuth2APIHostCloudProductLicences + "info";
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        CloudProductLicence cloudProductLicence = new CloudProductLicence();
                        cloudProductLicence = JsonConvert.DeserializeObject<CloudProductLicence>(contents);
                        return cloudProductLicence;
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }


        ////////////////////////////////////// Package  ///////////////////////
        static public async Task<ResultAPI> PutDataPackage(Gabana.Model.RenewModel value)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.SeAuth2APIHostRenew;
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        return new ResultAPI(true, contents);
                    }
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }


        ////////////////////////////////////// Renew  ///////////////////////
        static public async Task<ResultAPI> PutDataRenewPackage(Gabana.Model.RenewModel value)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    //var jwt2 = GabanaAPI.gbnJWT;
                    var jwt2 = GabanaAPI.ccJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.SeAuth2APIHostRenew;
                    var res = await httpClient.PutAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        return new ResultAPI(true, contents);
                    }
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        ////////////////////////////////////// Promotion  ///////////////////////

        static public async Task<ResultAPI> PostDataPromotion(GabanaLicenceModel value)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(value);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.SeAuth2APIHostPromotion;
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        return new ResultAPI(true, contents);
                    }
                }
            }
            catch (WebException ex)
            {
                return new ResultAPI(false, ex.Message);
            }
        }

        ////////////////////////////////////// GabanaInfo  ///////////////////////

        static public async Task<GabanaInfo> GetDataGabanaInfo()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = Constants.SeAuth2APIHostGabanaInfo;
                    var jwt2 = gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        GabanaInfo gabanaInfo = new GabanaInfo();
                        gabanaInfo = JsonConvert.DeserializeObject<GabanaInfo>(contents);
                        return gabanaInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                return new GabanaInfo() { Comments = "ex execption" };
            }
        }

        ////////////////////////////////////// KBank  ///////////////////////        

        static public async Task<respone_QrKBank> GetDataQRPayment(string tranNo, double amount)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string tranNoConvert = HttpUtility.UrlEncode(tranNo);
                    var url = Constants.Gabana3API + "v1/KBank/QRPayment?tranNo=" + tranNoConvert + "&amount=" + amount;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        respone_QrKBank respone_Qr = new respone_QrKBank();
                        respone_Qr = JsonConvert.DeserializeObject<respone_QrKBank>(contents);
                        return respone_Qr;
                    }
                }
            }
            catch (WebException ex)
            {
                //return null;
                respone_QrKBank respone_Qr = new respone_QrKBank();
                respone_Qr.statusCode = "-1";
                respone_Qr.errorDesc = ex.Message;
                return respone_Qr;
            }
        }

        static public async Task<respone_QrKBank> GetDataStatusQRPayment(string tranNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string tranNoConvert = HttpUtility.UrlEncode(tranNo);
                    var url = Constants.Gabana3API + "v1/KBank/StatusQRPayment?tranNo=" + tranNoConvert;
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    httpClient.DefaultRequestHeaders.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var res = await httpClient.GetAsync(url);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        respone_QrKBank respone_Qr = new respone_QrKBank();
                        respone_Qr = JsonConvert.DeserializeObject<respone_QrKBank>(contents);
                        return respone_Qr;
                    }
                }
            }
            catch (WebException ex)
            {
                respone_QrKBank respone_Qr = new respone_QrKBank();
                respone_Qr.statusCode = "-1";
                respone_Qr.errorDesc = ex.Message;
                return respone_Qr;
            }
        }               

        static public async Task<respone_QrKBank> PostDataCancelQRPayment(string tranNo)
        {
            try
            {
                await verifyCustomToken();
                using (var httpClient = new HttpClient())
                {
                    string tranNoConvert = HttpUtility.UrlEncode(tranNo);
                    var json = JsonConvert.SerializeObject(tranNoConvert);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    httpContent.Headers.Add("DeviceNo", DataCashingAll.DeviceNo.ToString());
                    var jwt2 = GabanaAPI.gbnJWT;
                    httpClient.SetBearerToken(jwt2);
                    httpClient.Timeout = TimeSpan.FromMinutes(1);
                    var url = Constants.Gabana3API + "v1/KBank/CancelQRPayment?tranNo=" + tranNoConvert;
                    var res = await httpClient.PostAsync(url, httpContent);
                    var contents = await res.Content.ReadAsStringAsync();
                    if (!res.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(contents))
                        {
                            throw new WebException(contents);
                        }
                        else
                        {
                            string message = ((HttpStatusCode)res.StatusCode).ToString();
                            throw new WebException(message);
                        }
                    }
                    else
                    {
                        respone_QrKBank respone_Qr = new respone_QrKBank();
                        respone_Qr = JsonConvert.DeserializeObject<respone_QrKBank>(contents);
                        return respone_Qr;
                    }
                }
            }
            catch (WebException ex)
            {
                respone_QrKBank respone_Qr = new respone_QrKBank();
                respone_Qr.statusCode = "-1";
                respone_Qr.errorDesc = ex.Message;
                return respone_Qr;
            }
        }



    }
}
