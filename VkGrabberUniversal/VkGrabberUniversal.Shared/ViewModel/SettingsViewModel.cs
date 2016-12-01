using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VkGrabberUniversal.Utils;
using VkGrabberUniversal.Model;
using VkGrabberUniversal.Model.Messenger;
using VkGrabberUniversal.Model.Rest;

namespace VkGrabberUniversal.ViewModel
{
    public class SettingsViewModel : PropertyChangedBase
    {
        #region Commands

        /// <summary>
        /// Добавить группу
        /// </summary>
        public ICommand AddGroupCommand { get; set; } = new CustomCommand((object p) => App.NavigationService.Navigate(typeof(View.GroupView)));

        /// <summary>
        /// Изменить группу
        /// </summary>
        public ICommand ChangeGroupCommand { get; set; } = new CustomCommand((object p) => App.NavigationService.Navigate(typeof(View.GroupView), p as Group));

        /// <summary>
        /// Обновить список постов
        /// </summary>
        public ICommand RefreshPostsCommand { get; set; } = new CustomCommand((object p) => Messenger.Default.Send<GrabMessage>(null));

        /// <summary>
        /// Выйти из учетной записи
        /// </summary>
        public ICommand LogoutCommand { get; set; } = new CustomCommand((object p) => { App.NavigationService.Navigate(typeof(View.AuthorizationView), true); });

        #endregion

        private User _currentUser;
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }

        /// <summary>
        /// Параметры
        /// </summary>
        public VkSettings VkSettings
        {
            get { return App.VkSettings; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public SettingsViewModel()
        {
            GetCurrentUser();
        }

        /// <summary>
        /// Получить информацию о текущем пользователе
        /// </summary>
        private async void GetCurrentUser()
        {
            CurrentUser = (await App.VkApi.GetUsersById(VkSettings.UserId)).FirstOrDefault();
        }
    }
}
