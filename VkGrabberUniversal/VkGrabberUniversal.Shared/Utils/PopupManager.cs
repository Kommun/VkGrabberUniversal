using Windows.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using VkGrabberUniversal.Controls;
namespace VkGrabberUniversal.Utils
{
    public class PopupManager
    {
        /// <summary>
        /// Текущее уведомление
        /// </summary>
        public Popup CurrentPopup { get; set; }

        /// <summary>
        /// Создает уведомление
        /// </summary>
        /// <returns></returns>
        public async Task ShowNotificationPopup(string message)
        {
            var dialog = new NotificationMessage { Message = message };
            await ShowDialog(dialog);
        }

        /// <summary>
        /// Показать всплывающий список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<int> ShowListDialog(IEnumerable items, string hint)
        {
            var dialog = new ListDialogPopup()
            {
                Items = items,
                Hint = hint
            };
            await ShowDialog(dialog);
            return dialog.SelectedIndex;
        }

        /// <summary>
        /// Показать диалог с выбором ответа
        /// </summary>
        /// <param name="message"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public async Task<bool> ShowMessageDialog(string message, bool defaultValue)
        {
            var dialog = new MessageDialogPopup
            {
                Message = message,
                Result = defaultValue
            };
            await ShowDialog(dialog);
            return dialog.Result;
        }

        /// <summary>
        /// Показать диалог ввода даты
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime?> ShowDateTimeDialog()
        {
            var dialog = new DateTimeDialog();
            await ShowDialog(dialog);
            return dialog.Result;
        }

        /// <summary>
        /// Показать диалог
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public async Task ShowDialog(Control dialog)
        {
            bool closed = false; ;
            var popup = new Popup();
            popup.ChildTransitions = new TransitionCollection { new ContentThemeTransition() };

            popup.Child = dialog;
            popup.Closed += (a, e) =>
            {
                App.NavigationService.SetAppBarVisibility(true);
                CurrentPopup = null;
                closed = true;
            };
            App.NavigationService.SetAppBarVisibility(false);
            CurrentPopup = popup;
            popup.IsOpen = true;
            await Task.Factory.StartNew(() => { while (!closed) { } });
        }

        /// <summary>
        /// Закрыть текущее уведомление
        /// </summary>
        public void CloseCurrentPopup()
        {
            if (CurrentPopup != null)
            {
                CurrentPopup.IsOpen = false;
                CurrentPopup = null;
            }
        }
    }
}
