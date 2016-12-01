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
        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.Today, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = sender as DateTimePicker;

            var date = ((DateTime)e.NewValue).Date;
            if (date != control.Date)
                control.Date = date;

            var time = ((DateTime)e.NewValue).TimeOfDay;
            if (time != control.Time)
                control.Time = time;
        }

        #endregion

        #region DateProperty        

        /// <summary>
        /// Дата
        /// </summary>
        public DateTimeOffset? Date
        {
            get { return (DateTimeOffset?)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(DateTimeOffset), typeof(DateTimePicker), new PropertyMetadata(DateTimeOffset.Now, new PropertyChangedCallback(OnDateChanged)));

        private static void OnDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = sender as DateTimePicker;

            if (control.Value.Date != ((DateTimeOffset?)e.NewValue)?.Date)
                control.RefreshValue();
        }

        #endregion

        #region TimeProperty        

        /// <summary>
        /// Время
        /// </summary>
        public TimeSpan? Time
        {
            get { return (TimeSpan?)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(TimeSpan), typeof(DateTimePicker), new PropertyMetadata(null, new PropertyChangedCallback(OnTimeChanged)));

        private static void OnTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var control = sender as DateTimePicker;

            if (control.Value.TimeOfDay != (TimeSpan?)e.NewValue)
                control.RefreshValue();
        }

        #endregion

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public DateTimePicker()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Обновить значение
        /// </summary>
        public void RefreshValue()
        {
            Value = Date?.Date.Add(Time ?? new TimeSpan()) ?? DateTime.Now;
        }
    }
}
