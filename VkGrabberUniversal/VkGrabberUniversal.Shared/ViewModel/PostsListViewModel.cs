using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using VkGrabberUniversal.Utils;
using VkGrabberUniversal.Model;
using VkGrabberUniversal.Model.Rest;
using VkGrabberUniversal.Model.Messenger;

namespace VkGrabberUniversal.ViewModel
{
    public class PostsListViewModel : PropertyChangedBase
    {
        private SemaphoreSlim _mutex = new SemaphoreSlim(1);
        private Post _currentZoomedPost;
        private CoreDispatcher _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        #region Commands

        /// <summary>
        /// Получить список постов
        /// </summary>
        public ICommand GrabNextCommand { get; set; }

        /// <summary>
        /// Запостить
        /// </summary>
        public ICommand PostCommand { get; set; }

        /// <summary>
        /// Выложить в определнное время
        /// </summary>
        public ICommand PostAtTimeCommand { get; set; }

        /// <summary>
        /// Запостить с помощью планировщика
        /// </summary>
        public ICommand PostWithSchedulerCommand { get; set; }

        /// <summary>
        /// Открыть оригинал
        /// </summary>
        public ICommand OpenOriginalCommand { get; set; }

        /// <summary>
        /// Меню
        /// </summary>
        public ICommand MenuCommand { get; set; }

        /// <summary>
        /// Увеличить фото
        /// </summary>
        public ICommand ZoomCommand { get; set; }

        /// <summary>
        /// Увеличить следующее фото
        /// </summary>
        public ICommand ZoomNextCommand { get; set; }

        /// <summary>
        /// Скрыть увеличенное фото
        /// </summary>
        public ICommand HideZoomedPhotoCommand { get; set; }

        /// <summary>
        /// Поиск по картинке
        /// </summary>
        public ICommand FindImageCommand { get; set; }

        /// <summary>
        /// Очистить список
        /// </summary>
        public ICommand ClearListCommand { get; set; }

        #endregion

        #region Properties    

        /// <summary>
        /// Ширина поста
        /// </summary>
        public double PostWidth { get; set; } = 550;

        private string _zoomedPhoto;
        /// <summary>
        /// Url увеличенного фото
        /// </summary>
        public string ZoomedPhoto
        {
            get { return _zoomedPhoto; }
            set
            {
                _zoomedPhoto = value;
                OnPropertyChanged("ZoomedPhoto");
            }
        }

        private Visibility _zoomedPhotoVisibility = Visibility.Collapsed;
        /// <summary>
        /// Видимость увеличенного фото
        /// </summary>
        public Visibility ZoomedPhotoVisibility
        {
            get { return _zoomedPhotoVisibility; }
            set
            {
                if (value == Visibility.Visible)
                    App.NavigationService.NavigatedBack += NavigationService_NavigatedBack;

                _zoomedPhotoVisibility = value;
                OnPropertyChanged("ZoomedPhotoVisibility");
            }
        }

        private Visibility _loadingIndicatorVisibility = Visibility.Collapsed;
        /// <summary>
        /// Видимость индикатора загрузки
        /// </summary>
        public Visibility LoadingIndicatorVisibility
        {
            get { return _loadingIndicatorVisibility; }
            set
            {
                _loadingIndicatorVisibility = value;
                OnPropertyChanged("LoadingIndicatorVisibility");
            }
        }

        /// <summary>
        /// Отфильтованные по лайкам и репостам записи
        /// </summary>
        public ObservableCollection<Post> FilteredPosts { get; set; } = new ObservableCollection<Post>();

        /// <summary>
        /// Активные группы
        /// </summary>
        public List<Group> ActiveGroups
        {
            get { return App.VkSettings.Groups.Where(g => g.IsActive).ToList(); }
        }

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public PostsListViewModel()
        {
            GrabNextCommand = new CustomCommand(GrabNext);
            PostCommand = new CustomCommand(Post);
            PostAtTimeCommand = new CustomCommand(PostAtTime);
            PostWithSchedulerCommand = new CustomCommand(PostWithScheduler);
            OpenOriginalCommand = new CustomCommand(OpenOriginal);
            MenuCommand = new CustomCommand(Menu);
            ZoomCommand = new CustomCommand(Zoom);
            ZoomNextCommand = new CustomCommand(ZoomNext);
            HideZoomedPhotoCommand = new CustomCommand(HideZoomedPhoto);
            FindImageCommand = new CustomCommand(FindImage);
            ClearListCommand = new CustomCommand(ClearList);

            // Для телефонов ширина поста устанавливается в зависимости от фактического размера экрана
#if WINDOWS_PHONE_APP
            PostWidth = Window.Current.Bounds.Width - 40;
#endif

            Messenger.Default.Register(this, async (GrabMessage o) =>
             {
                 ClearList();
                 await Grab();
             });

            Grab();
        }

        #region Methods

        /// <summary>
        /// Получить список постов
        /// </summary>
        private async Task Grab()
        {
            foreach (var group in ActiveGroups)
            {
                var groupInfo = (await App.VkApi.GetGroupsById(group.Name))?.FirstOrDefault();
                if (groupInfo == null)
                {
                    var dialogResult = await App.PopupManager.ShowMessageDialog($"Группа '{group.Name}' не найдена. Удалить ее из списка?", false);
                    if (dialogResult)
                        App.VkSettings.Groups.Remove(group);

                    continue;
                }

                var res = await App.VkApi.GetPosts(groupInfo.Id, 100, group.Offset);
                if (res == null)
                    continue;

                // Если с сервера получено 0 постов
                if (res.Items.Count == 0)
                {
                    var dialogResult = await App.PopupManager.ShowListDialog(new string[] { "Деактивировать", "Обнулить сдвиг", "Удалить" }, $"В группе {group.Name} закончились посты");
                    if (dialogResult != -1)
                    {
                        switch (dialogResult)
                        {
                            case 0:
                                group.IsActive = false;
                                break;
                            case 1:
                                group.Offset = 0;
                                break;
                            case 2:
                                App.VkSettings.Groups.Remove(group);
                                break;
                        }
                    }
                }

                // Отбираем посты, к которым ничего не прикреплено или прикреплены только фото, а также фильтруем по количеству лайков и репостов
                var posts = res.Items
                      .Where(p =>
                          p.Attachments?.All(a => a.Type == "photo") != false
                          && p.Likes.Count >= group.LikeCount
                          && p.Reposts.Count >= group.RepostCount)
                      .ToList();

                foreach (var p in posts)
                {
                    p.GroupInfo = groupInfo;

                    if (p.Attachments?.Count > 0)
                        SetPhotoSize(p.Attachments.Select(a => a.Photo).ToList());

                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => FilteredPosts.Add(p));
                }
            }
        }

        /// <summary>
        /// Установить размер фото
        /// </summary>
        /// <param name="photo"></param>
        private void SetPhotoSize(List<Photo> photo)
        {
            if (photo.Count == 1)
            {
                photo.Single().Width = double.NaN;
                photo.Single().Height = double.NaN;
            }

            if (photo.Count >= 2 && photo.Count <= 3)
                ResizePhotos(photo);

            if (photo.Count >= 4)
            {
                ResizePhotos(photo.GetRange(0, 2));
                ResizePhotos(photo.GetRange(2, photo.Count - 2));
            }
        }

        /// <summary>
        /// Подстраивает массив фото под ширину поста
        /// </summary>
        /// <param name="photo"></param>
        private void ResizePhotos(List<Photo> photo)
        {
            double minHeight = photo.Min(i => i.Height);
            foreach (var p in photo)
            {
                p.Width = (int)(p.Width / (p.Height / minHeight));
                p.Height = minHeight;
            };

            double summaryWidth = photo.Sum(i => i.Width);
            double koef = summaryWidth / (PostWidth - 50 - photo.Count * 4);
            foreach (var p in photo)
            {
                p.Width = (int)(p.Width / koef);
                p.Height = (int)(p.Height / koef);
            };
        }

        /// <summary>
        /// Добавить пост
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        private async Task<bool> Post(Post post, DateTimeOffset? date = null, bool fromScheduler = false)
        {
            if (!App.Settings.IsFullVersion && App.Settings.RepostsCount >= 10)
            {
                await App.PopupManager.ShowNotificationPopup("В пробной версии невозможно добавить больше 10 записей");
                return false;
            }

            await _mutex.WaitAsync();

            try
            {
                bool success = false;
                ChangeLoadingIncidacorVisibility(true);

                var groupInfo = (await App.VkApi.GetGroupsById(App.VkSettings.TargetGroup))?.FirstOrDefault();
                if (groupInfo == null)
                {
                    await App.PopupManager.ShowNotificationPopup("Целевая группа задана неверно");
                    ChangeLoadingIncidacorVisibility(false);
                    return false;
                }

                // Для записи, публикуемой через планировщик, берем дату из настроек
                if (fromScheduler)
                    date = App.VkSettings.SchedulerSettings.NextPostDate;

                success = await App.VkApi.Post(groupInfo.Id.ToString(), true, post.Text, post.Attachments, date);

                if (success)
                {
                    // Увеличиваем счетчик репостов
                    App.Settings.RepostsCount++;

                    if (fromScheduler)
                        App.VkSettings.SchedulerSettings.CalculateNextPostDate();
                }

                ChangeLoadingIncidacorVisibility(false);
                return success;
            }
            finally
            {
                _mutex.Release();
            }
        }

        /// <summary>
        /// Изменить видимость индикатора добавления поста
        /// </summary>
        /// <param name="isVisible"></param>
        private async void ChangeLoadingIncidacorVisibility(bool isVisible)
        {
#if WINDOWS_PHONE_APP
            var progressbar = StatusBar.GetForCurrentView();

            if (isVisible)
            {
                progressbar.ForegroundColor = Colors.Black;
                progressbar.ProgressIndicator.Text = "Пост загружается...";
                await progressbar.ProgressIndicator.ShowAsync();
            }
            else
                await progressbar.ProgressIndicator.HideAsync();
#else
            LoadingIndicatorVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
#endif
        }

        /// <summary>
        /// Получить тип группы для ссылки
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string GetGroupTypeUrl(GroupInfo info)
        {
            switch (info.Type)
            {
                case "page":
                    return "public";
                case "group":
                    return "club";
                case "event":
                    return "event";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Обработчик перехода назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationService_NavigatedBack(object sender, EventArgs e)
        {
            ZoomedPhotoVisibility = Visibility.Collapsed;
            App.NavigationService.NavigatedBack -= NavigationService_NavigatedBack;
        }

        #endregion

        #region CommandMethods

        /// <summary>
        /// Получить следующие записи
        /// </summary>
        /// <param name="parameter"></param>
        private async void GrabNext(object parameter)
        {
            foreach (var group in ActiveGroups)
                group.Offset += 100;

            await Grab();
        }

        /// <summary>
        /// Запостить
        /// </summary>
        /// <param name="parameter"></param>
        private async void Post(object parameter = null)
        {
            await Post(parameter as Post);
        }

        /// <summary>
        /// Запостить в определенное время
        /// </summary>
        /// <param name="parameter"></param>
        private async void PostAtTime(object parameter = null)
        {
            var time = await App.PopupManager.ShowDateTimeDialog();

            if (time == null)
                return;

            if (time < DateTime.Now.AddMinutes(1))
            {
                await App.PopupManager.ShowNotificationPopup("Невозможно добавить пост с прошедшей датой");
                return;
            }

            await Post(parameter as Post, time);
        }

        /// <summary>
        /// Запостить с помощью планировщика
        /// </summary>
        /// <param name="parameter"></param>
        private async void PostWithScheduler(object parameter)
        {
            if (App.VkSettings.SchedulerSettings.FromTime == null || App.VkSettings.SchedulerSettings.ToTime == null)
                await App.PopupManager.ShowNotificationPopup("Задайте временные рамки планировщика");
            else if (App.VkSettings.SchedulerSettings.Interval == null)
                await App.PopupManager.ShowNotificationPopup("Задайте интервал планировщика");
            else if (App.VkSettings.SchedulerSettings.NextPostDate == null)
                await App.PopupManager.ShowNotificationPopup("Задайте дату следующего поста");
            else if (App.VkSettings.SchedulerSettings.NextPostDate < DateTime.Now.AddMinutes(1))
                await App.PopupManager.ShowNotificationPopup("Дата следующего поста должна быть больше текущей");
            else
                await Post(parameter as Post, fromScheduler: true);
        }

        /// <summary>
        /// Открыть оригинал
        /// </summary>
        /// <param name="parameter"></param>
        private async void OpenOriginal(object parameter = null)
        {
            var post = parameter as Post;
            var url = new Uri(string.Format("https://vk.com/{0}{1}?w=wall-{1}_{2}", GetGroupTypeUrl(post.GroupInfo), post.GroupInfo.Id, post.Id));
            await Launcher.LaunchUriAsync(url);
        }

        /// <summary>
        /// Меню
        /// </summary>
        /// <param name="parameter"></param>
        private async void Menu(object parameter)
        {
            var res = await App.PopupManager.ShowListDialog(new string[] { "Добавить", "Добавить по дате", "В планировщик", "Открыть оригинал" }, "Меню");
            switch (res)
            {
                case 0:
                    Post(parameter);
                    break;
                case 1:
                    PostAtTime(parameter);
                    break;
                case 2:
                    PostWithScheduler(parameter);
                    break;
                case 3:
                    OpenOriginal(parameter);
                    break;
            }
        }

        /// <summary>
        /// Увеличить фото
        /// </summary>
        /// <param name="parameter"></param>
        private void Zoom(object parameter = null)
        {
            var attachmentInfo = parameter as AttachmentInfo;
            if (attachmentInfo == null)
                return;

            ZoomedPhoto = attachmentInfo.Attachment.Photo.BiggestPhoto;
            _currentZoomedPost = attachmentInfo.Post as Post;
            ZoomedPhotoVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Увеличить следующее фото
        /// </summary>
        /// <param name="parameter"></param>
        private void ZoomNext(object parameter = null)
        {
            int nextIndex = _currentZoomedPost.Attachments.IndexOf(_currentZoomedPost.Attachments.Single(a => a.Photo.BiggestPhoto == ZoomedPhoto)) + 1;
            if (nextIndex < _currentZoomedPost.Attachments.Count)
                ZoomedPhoto = _currentZoomedPost.Attachments[nextIndex].Photo.BiggestPhoto;

            else HideZoomedPhoto();
        }

        /// <summary>
        /// Скрыть увеличенное фото
        /// </summary>
        /// <param name="parameter"></param>
        private void HideZoomedPhoto(object parameter = null)
        {
            ZoomedPhotoVisibility = Visibility.Collapsed;
            _currentZoomedPost = null;
        }

        /// <summary>
        /// Поиск по картинке
        /// </summary>
        /// <param name="parameter"></param>
        private async void FindImage(object parameter)
        {
#if WINDOWS_PHONE_APP
            var url = new Uri(string.Format("https://m.yandex.ru/images/search?rpt=imageview&url={0}", ZoomedPhoto));
#else
            var url = new Uri(string.Format("https://yandex.ru/images/search?rpt=imageview&url={0}", ZoomedPhoto));
#endif

            await Launcher.LaunchUriAsync(url);
        }

        /// <summary>
        /// Очистить список
        /// </summary>
        /// <param name="parameter"></param>
        private void ClearList(object parameter = null)
        {
            FilteredPosts.Clear();
        }

        #endregion CommandMethods
    }
}
