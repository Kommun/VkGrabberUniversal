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
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VkGrabberUniversal.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostsListView : Page
    {
        private double? _lastVerticalOffset;
        private ScrollViewer _sw;

        /// <summary>
        /// Находится ли курсор над левой панелью
        /// </summary>
        private bool? _mouseOver;

        /// <summary>
        /// Конструктор
        /// </summary>
        public PostsListView()
        {
            this.InitializeComponent();
            DataContext = new ViewModel.PostsListViewModel();
            Loaded += PostsListView_Loaded;
        }

        /// <summary>
        /// Обработчик полной загрузки страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PostsListView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _sw = ((VisualTreeHelper.GetChild(lwPosts, 0) as Border).Child) as ScrollViewer;
                _sw.ViewChanged += _sw_ViewChanged; ;
            }
            catch { }
        }

        /// <summary>
        /// Обработчик прокрутки списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _sw_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_sw.VerticalOffset != 0)
                tbScroll.Text = "˄ Наверх";
            else if (_lastVerticalOffset != null)
                tbScroll.Text = "˅";
            else tbScroll.Text = "";
        }

        /// <summary>
        /// Обработчик нажатия на список
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lwPosts_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_sw == null || e.GetPosition(lwPosts).X > grdToTop.ActualWidth)
                return;

            if (_sw.VerticalOffset == 0 && _lastVerticalOffset != null)
                _sw.ChangeView(null, _lastVerticalOffset.Value, null, true);
            else
            {
                _lastVerticalOffset = _sw.VerticalOffset;
                _sw.ChangeView(null, 0, null, true);
            }
        }

        /// <summary>
        /// Обработчик движения мыши над списком
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lwPosts_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                bool mouseOver = e.GetCurrentPoint(lwPosts).Position.X <= grdToTop.ActualWidth;

                if (mouseOver == _mouseOver)
                    return;

                _mouseOver = mouseOver;
                (Resources[mouseOver ? "mouseEnter" : "mouseLeave"] as Storyboard).Begin();
            }
            catch { }
        }
    }
}
