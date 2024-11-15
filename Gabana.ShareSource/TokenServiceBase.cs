
using Gabana.Model;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace Gabana.ShareSource
{
    public abstract class TokenServiceBase
    {
        public TokenServiceBase()
        {

        }

        public static async Task<TokenResult> GetToken()
        {
            TokenResult tokenResult = new TokenResult();
            String Jwt;
           
            if (!Preferences.ContainsKey("gbnJWT"))
            {
                return new TokenResult() { status = false, gbnJWT = "" };
            }
            if (Preferences.Get("AppState", "") == "logout")
            {
                return new TokenResult() { status = false, gbnJWT = "" };
            }
            Jwt = Preferences.Get("gbnJWT", "");

            if (!await GabanaAPI.CheckNetWork())
            {
                return new TokenResult() { status = true, gbnJWT = Jwt };
            }


            if (await GabanaAPI.isValidToken(Jwt))
            {
                Jwt = Jwt;
                return new TokenResult() { status = true, gbnJWT = Jwt };
            }
            else
            {
                try
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        char LoginType = UtilsAll.GetLoginTypeToRefreshToken();
                        Jwt = await Gabana.ShareSource.GetToken.RefreshToken(Preferences.Get("RefreshToken", ""), LoginType);
                        return new TokenResult() { status = true, gbnJWT = Jwt };
                    }
                    else
                    {
                        //throw new Exception("No Ienternet !");
                        return new TokenResult() { status = false, gbnJWT = "" };
                    }

                }
                catch (Exception ex)
                {
                    //Preferences.Clear();
                    return new TokenResult() { status = false, gbnJWT = "" };
                }
            }
        }

        public abstract string OpenLoginPage();
        public abstract void OpenOTPPage();

        
    }
    
}
