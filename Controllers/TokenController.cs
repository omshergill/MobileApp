using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VCL_Models;
using VolMobile.AppData.Interfaces;
using Xamarin.Forms;
using VolMobile.Constants;
using VolMobile.HttpCalls;

namespace VolMobile.Controllers
{
    public class TokenController
    {
        //private static ILogger logger = DependencyService.Get<ILogManager>().GetLog();

        //The controller initializes with the Application state and begins the timer to refresh the timer
        public TokenController()
        {
        }

        public async Task GetAuthorizationToken(string uName, string pWord) 
        {
            bool tokenReturned = false;
            string tokenGetResponse = string.Empty;
            try
            {
                LoginModel logtest = new LoginModel();
                logtest.Username = uName;
                logtest.Password = pWord;
                logtest.AppType = Constants.AppDetails.APP_CODE;
                logtest.SystemCode = Constants.AppDetails.SYSTEM_CODE;
                logtest.Push.PushSystem = PushAddressModel.PushSystemCode.FireBase;
                logtest.Push.Address = App.AppState.AppSettings.PushNotificationToken;
                logtest.Push.AppCode = Constants.AppDetails.APP_CODE;
                string uri = URI.message_Chat_Endpoint;
                //  Start_Conversation --> sends the new request to start a convo
                (tokenReturned, tokenGetResponse) = await ApiFunctions.Post(logtest, URI.token_Endpoint, false);
            }
            catch (Exception ex)
            {
                //logger.Error(App.LogPrefix() + "Error: " + ex.Message);
                Console.WriteLine("Error: " + ex.Message);
            }

            if (tokenReturned)
            {
                try
                {
                    //deserialize the return object
                    TokenModel token = JsonConvert.DeserializeObject<TokenModel>(tokenGetResponse);
                    App.AppState.AppSettings.token = token;
                }
                catch (Exception ex)
                {
                    //logger.Error(App.LogPrefix() + "Error: " + ex.Message);
                    Console.WriteLine(App.LogPrefix() + "Error: " + ex.Message);
                }
                //logger.Trace(App.LogPrefix() + "Profile Token loaded: " + App.AppState.AppSettings.token.access_token);
                Console.WriteLine(App.LogPrefix() + "Profile Token loaded: " + App.AppState.AppSettings.token.access_token);
            }
            else
            {
                //logger.Error(App.LogPrefix() + "Error loading profile token");
                Console.WriteLine(App.LogPrefix() + "Error loading profile token");
            }
        }


        public async void WaitAndRefreshToken(TokenModel token)
        {
            //logger.Trace("Starting token refresh timer");
            Console.WriteLine("Starting token refresh timer");
            //start a delay that lasts for 50% of the token duration
            await Task.Delay((int)(token.expires_in * 0.5) * 1000);
            //attempt to refresh the token asuming it is still valid - HTTP calls will redirect to login if not
            bool refreshed = false;
            string rtnValue = string.Empty;
            (refreshed, rtnValue) = await HttpCalls.ApiFunctions.Get(null, URI.token_Endpoint);
            //refreshed = await HttpCalls.ApiFunctions.refreshToken(token);

            //if the refresh worked then recreate the token controller to start the timer again
            if (refreshed == false)
            {
                //logger.Error(App.LogPrefix() + "Token Refresh Failed.");
                Console.WriteLine(App.LogPrefix() + "Token Refresh Failed.");
            }
            else
            {
                try
                {
                    TokenModel newToken = JsonConvert.DeserializeObject<TokenModel>(rtnValue);
                    if (newToken != null)
                    {
                        App.AppState.AppSettings.token = newToken;
                    }
                    //logger.Trace("Token Refresh Success.");
                    Console.WriteLine("Token Refresh Success.");
                    //run the wait and refresh on the new token (a delay occurs at the beginning of the function
                    WaitAndRefreshToken(App.AppState.AppSettings.token);
                }
                catch (Exception ex)
                {
                    //logger.Error(App.LogPrefix() + "Token Refresh Failed: " + ex.Message);
                    Console.WriteLine(App.LogPrefix() + "Token Refresh Failed: " + ex.Message);
                }
            }
        }


    }
}
