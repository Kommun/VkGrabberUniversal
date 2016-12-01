using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using VkGrabberUniversal.Model;
using VkGrabberUniversal.Utils;

namespace VkGrabberUniversal.ViewModel
{
    public class GroupViewModel
    {
        private bool _isChanging;

        /// <summary>
        /// Сохранить
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// Удалить
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="group"></param>
        public GroupViewModel(Group group)
        {
            SaveCommand = new CustomCommand(Save);
            DeleteCommand = new CustomCommand(Delete);

            if (group != null)
                _isChanging = true;

            Group = group ?? new Group();
        }

        /// <summary>
        /// Сохранить
        /// </summary>
        /// <param name="parameter"></param>
        private async void Save(object parameter)
        {
            if (string.IsNullOrEmpty(Group.Name))
            {
                await App.PopupManager.ShowNotificationPopup("Введите название группы");
                return;
            }

            if (!_isChanging)
                App.VkSettings.Groups.Add(Group);

            App.NavigationService.GoBack();
        }

        /// <summary>
        /// Удалить
        /// </summary>
        /// <param name="parameter"></param>
        private void Delete(object parameter)
        {
            App.VkSettings.Groups.Remove(Group);
            App.NavigationService.GoBack();
        }
    }
}
