using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Input;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using VkGrabberUniversal.Utils;

namespace VkGrabberUniversal.ViewModel
{
    public class HelpViewModel : PropertyChangedBase
    {
        /// <summary>
        /// Купить подписку
        /// </summary>
        public ICommand BuyCommand { get; set; }

        private string _help;
        /// <summary>
        /// Справка
        /// </summary>
        public string Help
        {
            get { return _help; }
            set
            {
                _help = value;
                OnPropertyChanged("Help");
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public HelpViewModel()
        {
            BuyCommand = new CustomCommand(Buy);

            LoadHelp();
        }

        /// <summary>
        /// Загрузить справку
        /// </summary>
        private async void LoadHelp()
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Resources");
            var file = (await folder.GetFilesAsync()).FirstOrDefault();

            if (file != null)
                Help = await FileIO.ReadTextAsync(file);
            else
            {
                await App.PopupManager.ShowNotificationPopup("Не удалось загрузить файл справки");
                App.NavigationService.GoBack();
            }
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
