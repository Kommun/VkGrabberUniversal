using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VkGrabberUniversal.Controls
{
    public sealed partial class ListDialogPopup : UserControl
    {
        private int _selectedIndex;

        /// <summary>
        /// Выбранный элемент списка
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                Close();
            }
        }

        /// <summary>
        /// Список элементов списка выбора
        /// </summary>
        public IEnumerable Items { get; set; }

        #region HintProperty 

        /// <summary>
        /// Подсказка
        /// </summary>
        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string), typeof(ListDialogPopup), null);
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public ListDialogPopup()
        {
            this.InitializeComponent();
            Height = Window.Current.Bounds.Height;
            Width = Window.Current.Bounds.Width;
        }

        /// <summary>
        /// Закрыть 
        /// </summary>
        private void Close()
        {
            try
            {
                (this.Parent as Popup).IsOpen = false;
            }
            catch { }
        }
    }
}
