using System;
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
    public sealed partial class MessageDialogPopup : UserControl
    {
        #region ResultProperty 

        /// <summary>
        /// Результат диалога
        /// </summary>
        public bool Result
        {
            get { return (bool)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(bool), typeof(MessageDialogPopup), null);
        #endregion

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public MessageDialogPopup()
        {
            this.InitializeComponent();
            Height = Window.Current.Bounds.Height;
            Width = Window.Current.Bounds.Width;
            btnYes.Click += BtnYes_Click;
            btnNo.Click += BtnNo_Click;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

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
