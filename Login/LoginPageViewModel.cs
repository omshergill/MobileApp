using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using VolMobile.AppData.Interfaces;
using VolMobile.AppData.SQL;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VolMobile.Pages.AuthenticationPages.Login
{

    public class LoginPageViewModel : INotifyPropertyChanged
    {
        //private static ILogger logger = DependencyService.Get<ILogManager>().GetLog();

        public LoginPageViewModel()
        {

        }

        public void Load()
        {
            //RememberMe = DeviceStorage.RememberMe;
            //if (DeviceStorage.RememberMe == true)
            //{
            //UserName = DeviceStorage.UsersName; //+AppData.AppData.Alpha_Beta_Version -->adding extra @beta/alpha to name on log in.
            //}
            IsLoading = false;
        }

        #region Properties


        public string AppVersion
        {
            get
            {
                return "v." + AppInfo.VersionString;
            }
        }


        string _UserName = string.Empty;
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        string _Password = string.Empty;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private bool _RememberMe = false;
        public bool RememberMe
        {
            get
            {
                return _RememberMe;
            }
            set
            {
                _RememberMe = value;
                OnPropertyChanged(nameof(RememberMe));
            }
        }


        private bool _IsLoading = true;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                _IsLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        #endregion


        public async Task Login()
        {

            IsLoading = true;

            var userNameStripped = HttpCalls.ApiFunctions.SetServerURI(UserName);
            try
            {
                await App.AppState.AppControllers.tokenController.GetAuthorizationToken(userNameStripped, Password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

            if (App.AppState.AppSettings.loggedIn)
            {
                //Run the Post login Initialization of Controllers
                try
                {
                    await App.AppState.Initialize_PostLogin();
                }
                catch (Exception ex)
                {
                    //logger.Error(App.LogPrefix() + "Init_PostLogin error: " + ex.Message);
                    Console.WriteLine(App.LogPrefix() + "Init_PostLogin error: " + ex.Message);

                }

                //Proceed to dashboard
               // Device.BeginInvokeOnMainThread(() =>
                //{
                    try
                    {

                        App.Current.MainPage = new Navigation.Navigation(new Dashboard.DashboardPage());
                    }
                    catch (Exception ex)
                    {
                        //logger.Error(App.LogPrefix() + "Error opening Navigation Page: " + ex.Message);
                    Console.WriteLine(App.LogPrefix() + "Error opening Navigation Page: " + ex.Message);
                        this.IsLoading = false;
                    }

                //});

            }
            else
            {
                this.IsLoading = false;
            }

        }




        //Property change event, used to update data bound objects in the UI
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            System.Diagnostics.Debug.WriteLine("Before");
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("Fired");
            handler(this,
                new PropertyChangedEventArgs(propName));
        }

    }
}
