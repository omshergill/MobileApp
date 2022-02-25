using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
//using VCL_Models;
using VolMobile.AppData.SQL;
using VolMobile.HttpCalls;
using VolMobile.Models;

using VolMobile.Production_Logic;
using VolMobile.Pages.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VolMobile.Constants;
using VolMobile.AppData.Interfaces;
using Xamarin.Essentials;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Collections.Generic;
using VolMobile.Controllers;

namespace VolMobile.Pages.AuthenticationPages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        //private static ILogger logger = DependencyService.Get<ILogManager>().GetLog();

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginPageViewModel();
        }

        protected async override void OnAppearing()
        {
            if (App.AppState.AppSettings.loggedIn)
            {
                //User has an active session, run login check then proceed
                try
                {
                    //Check if the user is already logged in
                    if (App.AppState.AppSettings.loggedIn)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            try
                            {
                                App.Current.MainPage = new Navigation.Navigation(new Dashboard.DashboardPage());
                            }
                            catch (Exception ex)
                            {
                                //logger.Error(App.LogPrefix() + "Error opening Navigation Page: " + ex.Message);
                                Console.WriteLine(App.LogPrefix() + "Error opening Navigation Page: " + ex.Message);
                            }
                        });
                    }
                    //--success will direct to dashboard
                    //--failure requires user interaction
                }
                catch (Exception e)
                {
                    //logger.Error(App.LogPrefix() + "Error opening Navigation Page: " + e.Message);
                    Console.WriteLine(App.LogPrefix() + "Error opening Navigation Page: " + e.Message);
                }
            }
            else
            {
                //load the login page
                ((LoginPageViewModel)BindingContext).Load();
            }
        }

        protected override void OnDisappearing()
        {
        }

        /// <summary>
        /// handles what happens when each entry is focused or unfocused
        /// </summary>
        #region input entry code
        public void userNameInput(object sender, EventArgs e) //when entering credentials and user hits done button on keyboard
        {
            //UserName.Unfocus();
            //PassWord.Focus();
        }
        public async void passwordInput(object sender, EventArgs e) //when entering credentials and user hits done button on keyboard
        {
            //Attempt to log into the API - The token will be set if this succeeds
            await ((LoginPageViewModel)BindingContext).Login();
        }

        private async void LoginButtonClicked(object sender, EventArgs e)
        {
            string userValue = User_Name.Text;
            string passValue = _Password.Text;
            //Console.WriteLine(userValue);
            //Console.WriteLine(passValue);
            //Device.BeginInvokeOnMainThread(() =>
            //{
                //((LoginPageViewModel)BindingContext).UserName = userValue;
                //((LoginPageViewModel)BindingContext).Password = passValue;
            //});

            //Attempt to log into the API - The token will be set if this succeeds
            await ((LoginPageViewModel)BindingContext).Login();
        }

        #endregion

        /// <summary>
        /// handles button input, and switcher
        /// </summary>
        #region button_Code
        private void infoButtonClicked(object sender, EventArgs e) //when info button is clicked open nesda.ca
        {
            Device.OpenUri(new Uri(URI.ReleaseAddress));
        }
        private async void SignUpSelected(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new Registration.RegistrationPage());
        }
        private async void ForgotPassword(object sender, EventArgs e) //when forgot password is clicked go to the forgot password page
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new ForgotPassword.ForgotPassword());
        }

        private void Toggle_Remember(object sender, EventArgs e)
        {
            ((LoginPageViewModel)BindingContext).RememberMe = !((LoginPageViewModel)BindingContext).RememberMe;
            if (!DeviceStorage.RememberMe) { DeviceStorage.UsersName = null; }//clear the username cache
        }
       
        #endregion

    }
}