using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource
{
    class Constants
    {
        internal const string urlInitialCreateGabana = "InitialCreateGabana";
        internal const string urlInitialLoginGabana = "InitialLoginGabana";
        internal const string urlCreateGabana = "CreateGabana";
        internal const string urlLoginGabana = "LoginGabana";
        internal const string urlCheckNet = "help";
        internal const string urlCheckSingUpOrLoginGabana = "CheckSingUpOrLoginGabana";
        internal const string AddUser = "AddUser";
        internal const string GetUser = "Users";
        internal const string User = "User";
        internal const string UserAccessProducts = "UserAccessProducts";
        internal const string ChangePassword = "ChangePassword"; 
        internal const string OnwerChangePassword = "OnwerChangePassword";
        internal const string MerchantConfig = "MerchantConfig";
        internal const string Profile = "Profile";
        internal const string RenewiOS = "AppStore/InitialPurchase";

#if (DEBUGs)
        internal const string SeAuth2Host = "https://lab.seniorsoft.com/blueid/connect/token"; //Request JWT
        internal const string SeAuth2APIHost = "https://lab.seniorsoft.com/seauth2api/OTP/";
        internal const string Gabana3API = "https://lab.seniorsoft.com/Gabanaapi/";
        internal const string SeAuth2APIHostUserAccount = "https://lab.seniorsoft.com/seauth2api/UserAccount/";
        internal const string SeAuth2APIHostCloudProductLicences = "https://lab.seniorsoft.com/seauth2api/CloudProductLicences/";
        internal const string SeAuth2APIHostRenew = "https://lab.seniorsoft.com/seauth2api/Gabana/renew/";
        internal const string SeAuth2APIHostPromotion = "https://lab.seniorsoft.com/seauth2api/Gabana/Promotion/";
        internal const string SeAuth2APIHostGabanaInfo = "https://lab.seniorsoft.com/seauth2api/Gabana/info/";
        internal const string Ngrok = "https://ab0a-125-24-163-116.ap.ngrok.io/";

#else
        internal const string SeAuth2Host = "https://blueid.seniorsoft.com/blueid/connect/token"; //Request JWT 
        internal const string SeAuth2APIHost = "https://blueid.seniorsoft.com/blueidapi/OTP/";
        internal const string Gabana3API = "https://gabana.seniorsoft.com/Gabanaapi/";
        internal const string SeAuth2APIHostUserAccount = "https://blueid.seniorsoft.com/blueidapi/UserAccount/";
        internal const string SeAuth2APIHostCloudProductLicences = "https://blueid.seniorsoft.com/blueidapi/CloudProductLicences/";
        internal const string SeAuth2APIHostRenew = "https://blueid.seniorsoft.com/blueidapi/Gabana/renew/";
        internal const string SeAuth2APIHostPromotion = "https://blueid.seniorsoft.com/blueidapi/Gabana/Promotion/"; 
        internal const string SeAuth2APIHostGabanaInfo = "https://blueid.seniorsoft.com/blueidapi/Gabana/info/";
        internal const string Ngrok = "https://ab0a-125-24-163-116.ap.ngrok.io/";
#endif


    }

}
