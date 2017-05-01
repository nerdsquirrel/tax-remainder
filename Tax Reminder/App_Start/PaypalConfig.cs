using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Tax_Reminder.App_Start
{
    public static class PaypalConfiguration
    {
        //these variables will store the clientID and clientSecret
        //by reading them from the web.config
        public readonly static string ClientId;
        public readonly static string ClientSecret;

        static PaypalConfiguration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }

        // getting properties from the web.config
        public static Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>
            {
                { "clientId", ConfigurationManager.AppSettings["clientId"] },
                { "clientSecret", ConfigurationManager.AppSettings["clientSecret"] },
                { "mode", ConfigurationManager.AppSettings["mode"] },
                { "connectionTimeout", ConfigurationManager.AppSettings["connectionTimeout"] },
                { "requestRetries", ConfigurationManager.AppSettings["requestRetries"] },
            };

        }

        private static string GetAccessToken()
        {
            // getting accesstocken from paypal                
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();

            return accessToken;
        }

        public static APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}