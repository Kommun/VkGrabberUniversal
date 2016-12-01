using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using VkGrabberUniversal.Utils;

namespace VkGrabberUniversal.Controls
{
    public class CustomListView : ListView
    {
        #region ItemClickCommandProperty 

        /// <summary>
        /// Действие при нажатии на элемент списка
        /// </summary>
        public CustomCommand ItemClickCommand
        {
            get { return (CustomCommand)GetValue(ItemClickCommandProperty); }
            set { SetValue(ItemClickCommandProperty, value); }
        }

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register("ItemClickCommand", typeof(CustomCommand), typeof(CustomListView), null);
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public CustomListView()
        {
            IsItemClickEnabled = true;
            ItemClick += CustomListView_ItemClick;
        }

        /// <summary>
        /// Обработчик нажатия на элемент списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClickCommand?.Execute(e.ClickedItem);
        }
    }
}
