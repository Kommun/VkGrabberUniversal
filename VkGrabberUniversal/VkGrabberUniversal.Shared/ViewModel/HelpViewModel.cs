using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Windows.ApplicationModel.Store;
using VkGrabberUniversal.Utils;

namespace VkGrabberUniversal.ViewModel
{
    public class HelpViewModel
    {
        /// <summary>
        /// Купить подписку
        /// </summary>
        public ICommand BuyCommand { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public HelpViewModel()
        {
            BuyCommand = new CustomCommand(Buy);
        }

        /// <summary>
        /// Купить подписку
        /// </summary>
        /// <param name="parameter"></param>
        private async void Buy(object parameter)
        {
            try
            {
                var result = await CurrentApp.RequestProductPurchaseAsync("FullVersion");
                if (result.Status != ProductPurchaseStatus.Succeeded)
                    return;

                App.Settings.IsFullVersion = true;
                await App.PopupManager.ShowNotificationPopup("Подписка успешно активирована");
            }
            catch { }
        }
    }
}
