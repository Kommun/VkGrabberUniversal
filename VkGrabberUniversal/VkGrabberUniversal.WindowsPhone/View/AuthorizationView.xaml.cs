using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VkGrabberUniversal.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VkGrabberUniversal.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthorizationView : Page
    {


        /// <summary>
        /// Конструктор
        /// </summary>
        public AuthorizationView()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            OnLoad((bool?)e.Parameter ?? false);
        }

        private async void OnLoad(bool logout)
        {
            wvAuthorization.Visibility = Visibility.Collapsed;
            if (logout)
            {
                wvAuthorization.LoadCompleted += WvAuthorization_LoadCompleted; ;
                wvAuthorization.Navigate(new Uri("https://m.vk.com/"));
                await Task.Run(() =>
                {
                    //while (!logOuted) { }
                });
            }

            wvAuthorization.NavigationCompleted += WvAuthorization_NavigationCompleted;
            wvAuthorization.Navigate(new Uri(string.Format("https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri={2}&display=page&response_type=token",
                  VkSettings.AppId, VkSettings.Scopes, VkSettings.RedirectUri)));
        }

        private void WvAuthorization_LoadCompleted(object sender, NavigationEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Обработчик перехода на страницу авторизации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WvAuthorization_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            wvAuthorization.Visibility = Visibility.Visible;
            if (string.IsNullOrEmpty(args.Uri.Fragment))
                return;

            var decoder = new WwwFormUrlDecoder(args.Uri.Fragment.Substring(1));
            App.VkSettings.AccessToken = decoder.GetFirstValueByName("access_token");
            App.VkSettings.UserId = decoder.GetFirstValueByName("user_id");
            App.NavigationService.Navigate(typeof(SettingsView));

            wvAuthorization.NavigationCompleted -= WvAuthorization_NavigationCompleted;
        }

        /// <summary>
        /// Обработчик загрузки главной страницы мобильной версии ВК
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WbAuthorization_MainPageLoadCompleted(object sender, NavigationEventArgs e)
        {
            //try
            //{
            //    var document = wvAuthorization.Document as IHTMLDocument3;
            //    var logoutLink = document.getElementsByTagName("li")
            //        .Cast<IHTMLElement>()
            //        .FirstOrDefault(el => el.getAttribute("className") == "mmi_logout")
            //        .children[0].getAttribute("href");
            //    if (!string.IsNullOrEmpty(logoutLink))
            //    {
            //        wbAuthorization.Navigated += WbAuthorization_LogoutNavigated;
            //        wbAuthorization.Navigate(logoutLink);
            //    }
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }

            //wbAuthorization.LoadCompleted -= WbAuthorization_MainPageLoadCompleted;
        }

        /// <summary>
        /// Обработчик перехода по ссылке логаута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WbAuthorization_LogoutNavigated(object sender, NavigationEventArgs e)
        {
            // Вышли из учетной записи
            //logOuted = true;
            //wvAuthorization.NavigationCompleted -= WbAuthorization_LogoutNavigated;
        }
    }
}
