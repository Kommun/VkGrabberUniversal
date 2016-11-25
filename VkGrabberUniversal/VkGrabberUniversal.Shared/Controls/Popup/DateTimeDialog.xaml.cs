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
    public sealed partial class DateTimeDialog : UserControl
    {
        #region ResultProperty 

        /// <summary>
        /// Результат диалога
        /// </summary>
        public DateTime? Result
        {
            get { return (DateTime?)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(DateTime?), typeof(DateTimeDialog), null);
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public DateTimeDialog()
        {
            this.InitializeComponent();
            Height = Window.Current.Bounds.Height;
            Width = Window.Current.Bounds.Width;
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = dateTimePicker.Value;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
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
