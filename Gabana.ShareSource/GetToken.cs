using Gabana.Model;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Gabana.ShareSource
{
    public static class GetToken
    {
        public class TokenJWT2Model
        {
            public string grant_type { get; set; }
            public string client_secret { get; set; }
            public string client_id { get; set; }
            public string scope { get; set; }
            public string OwnerID { get; set; }
            public string DelegateCode { get; set; }
        }
        public class Token
        {
            public string access_token { get; set; }
            public DateTimeOffset start_token_date { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
        }

        public static async Task<string> Get_ccJWT()
        {
            try
            {
                var apiClientCredentials = new ClientCredentialsTokenRequest
                {
                    Address = Constants.SeAuth2Host,
                    ClientId = "GabanaCC",
                    ClientSecret = "lm9NIJcrApRMaGj18oRSAO8LGI5mwEvL7aa4EuqtOJE49W3HU69FmleXc96HKUbACLak0jRKlUiRjHQCaGlwpw",
                    Scope = "seauth2.accesswithotp gabana.accesswithotp " 

                };


                var client = new HttpClient();

                TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
                if (tokenResponse.IsError)
                {
                    throw new Exception("Get Jwt Error: " + tokenResponse.Error);
                }

                return tokenResponse.AccessToken;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static async Task<string> GetgbnJWTForOwner(VerifyOTP Giftoryotp)
        {
            try
            {
                var httpClient = new HttpClient() { BaseAddress = new Uri(Constants.SeAuth2Host) };
                var httpClientRequestToken = await httpClient.RequestTokenAsync(new TokenRequest
                {
                    GrantType = "delegatecode",
                    ClientId = "GabanaDC",
                    ClientSecret = "4DQhA13WbwdK2gDxUdEdPEY5uiDedaCnMl4kV6brLPN6mBD6XnEE71DmAuwDGtxGV2Oq5Jh7mTkcpJJoaBrtVQ",

                    Parameters = new Dictionary<string, string>
                    {
                        { "scope", "offline_access gabana.standard seauth2.mymerchant gabanabellhub.deviceaccess epaymentapi.mymerchant" } ,
                        { "ownerid", Giftoryotp.OwnerID } ,
                        { "delegatecode", Giftoryotp.OTP } ,
                    }
                });

                if (httpClientRequestToken.IsError)
                {
                    //always getting 'invalid_grant' error
                    //throw InvalidOperationException($"{cReq.Error}: {cReq.ErrorDescription}");
                    throw new Exception($"{httpClientRequestToken.Error}: {httpClientRequestToken.ErrorDescription}");
                }

                Preferences.Set("RefreshToken", httpClientRequestToken.RefreshToken);
                Preferences.Set("gbnJWT", httpClientRequestToken.AccessToken);

                return httpClientRequestToken.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static async Task<string> GetgbnJWTForEmp(LoginEmp login)
        {
            try
            {
                var httpClient = new HttpClient() { BaseAddress = new Uri(Constants.SeAuth2Host) };
                var httpClientRequestToken = await httpClient.RequestTokenAsync(new TokenRequest
                {
                    GrantType = "password",//ResourceOwnerPassword
                    ClientId = "GabanaApp",
                    ClientSecret = "bVofcmtqmxqslf6V0u61dde3uH1GGPO5IFq1mk1skx9c96q6xRUZw9OVTvm9PcpQODM0MHhxUdvZpvWS6sgZzA",

                    Parameters = new Dictionary<string, string>
                    {
                        { "scope", "offline_access gabana.standard gabanabellhub.deviceaccess seauth2.mymerchant epaymentapi.mymerchant" } ,
                        { "merchantid", login.MerchantID } ,
                        { "username", login.Username } ,
                        { "password", login.Password } 
                        
                    }
                });

                if (httpClientRequestToken.IsError)
                {
                    //always getting 'invalid_grant' error
                    //throw InvalidOperationException($"{cReq.Error}: {cReq.ErrorDescription}");
                    throw new Exception($"{httpClientRequestToken.Error}: {httpClientRequestToken.ErrorDescription}");
                }

                Preferences.Set("RefreshToken", httpClientRequestToken.RefreshToken);
                Preferences.Set("gbnJWT", httpClientRequestToken.AccessToken);

                return httpClientRequestToken.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static async Task<string> RefreshToken(string refresh_token_value,char type)//type e , o
        {
            if (string.IsNullOrEmpty(refresh_token_value))
            {
                throw new ArgumentNullException("refresh token is null.");
            }
            try
            {

                var httpClient = new HttpClient() { BaseAddress = new Uri(Constants.SeAuth2Host) };
                TokenResponse httpClientRequestToken = new TokenResponse();
                if (type=='e')
                {
                    httpClientRequestToken = await httpClient.RequestTokenAsync(new TokenRequest
                    {
                        GrantType = "refresh_token",
                        ClientId = "GabanaApp",
                        ClientSecret = "bVofcmtqmxqslf6V0u61dde3uH1GGPO5IFq1mk1skx9c96q6xRUZw9OVTvm9PcpQODM0MHhxUdvZpvWS6sgZzA",

                        Parameters = new Dictionary<string, string>
                    {
                        { "refresh_token", refresh_token_value } ,
                    }
                    });
                    
                }
                else if (type == 'o')
                {
                    httpClientRequestToken = await httpClient.RequestTokenAsync(new TokenRequest
                    {
                        GrantType = "refresh_token",
                        ClientId = "GabanaDC",
                        ClientSecret = "4DQhA13WbwdK2gDxUdEdPEY5uiDedaCnMl4kV6brLPN6mBD6XnEE71DmAuwDGtxGV2Oq5Jh7mTkcpJJoaBrtVQ",

                        Parameters = new Dictionary<string, string>
                    {
                        { "refresh_token", refresh_token_value } ,
                    }
                    });
                    
                }

                if (httpClientRequestToken.IsError)
                {
                    throw new Exception($"{httpClientRequestToken.Error}: {httpClientRequestToken.ErrorDescription}");
                }

                Preferences.Set("RefreshToken", httpClientRequestToken.RefreshToken);
                Preferences.Set("gbnJWT", httpClientRequestToken.AccessToken);

                return httpClientRequestToken.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        

        
    }
}
