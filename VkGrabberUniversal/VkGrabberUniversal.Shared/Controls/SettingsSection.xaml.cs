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
    public sealed partial class SettingsSection : UserControl
    {
        #region HeaderFontSizeProperty        

        /// <summary>
        /// Размер шрифта заголовка
        /// </summary>
        public int HeaderFontSize
        {
            get { return (int)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register("HeaderFontSize", typeof(int), typeof(SettingsSection), new PropertyMetadata(20));

        #endregion

        #region SectionHeaderProperty        

        /// <summary>
        /// Заголовок
        /// </summary>
        public string SectionHeader
        {
            get { return (string)GetValue(SectionHeaderProperty); }
            set { SetValue(SectionHeaderProperty, value); }
        }

        public static readonly DependencyProperty SectionHeaderProperty = DependencyProperty.Register("SectionHeader", typeof(string), typeof(SettingsSection), new PropertyMetadata(null));

        #endregion

        #region AdditionalContentProperty

        /// <summary>
        /// Контент
        /// </summary>
        public object AdditionalContent
        {
            get { return (object)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }

        public static readonly DependencyProperty AdditionalContentProperty = DependencyProperty.Register("AdditionalContent", typeof(object), typeof(SettingsSection), null);

        #endregion

        public SettingsSection()
        {
            this.InitializeComponent();
        }
    }
}
