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
    public sealed partial class DateTimePicker : UserControl
    {
        private DateTimeOffset? _date;
        private TimeSpan? _time;

        #region Properties

        #region HeaderProperty        

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SettingsSection), null);


        #endregion

        #region ValueProperty        

        /// <summary>
        /// Дата + время
        /// </summary>
        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as DateTimePicker;

            var date = ((DateTime)e.NewValue).Date;
            if (date != control.Date)
                control.Date = date;

            var time = ((DateTime)e.NewValue).TimeOfDay;
            if (time != control.Time)
                control.Time = time;
        }

        #endregion

        /// <summary>
        /// Дата
        /// </summary>
        public DateTimeOffset? Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RefreshValue();
            }
        }

        /// <summary>
        /// Время
        /// </summary>
        public TimeSpan? Time
        {
            get { return _time; }
            set
            {
                _time = value;
                RefreshValue();
            }
        }

        #endregion

        public DateTimePicker()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Обновить значение
        /// </summary>
        private void RefreshValue()
        {
            Value = Date?.Date.Add(Time ?? new TimeSpan());
        }
    }
}
