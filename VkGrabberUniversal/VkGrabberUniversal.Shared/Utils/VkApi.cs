using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Net.Http.Headers;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Networking.BackgroundTransfer;
using VkGrabberUniversal.Model.Rest;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace VkGrabberUniversal.Utils
{
    public class VkApi
    {
        const string BaseUrl = "https://api.vk.com/method";

        private readonly VkSettings _settings;
        private int _queriesCountLastSecond;
        private const int _maxQueriesCountPerSecond = 3;

        #region ErrorDescriptions

        /// <summary>
        /// Описание ошибок по коду
        /// </summary>
        public static Dictionary<int, string> ErrorDescriptions = new Dictionary<int, string> {
            { 1,"Произошла неизвестная ошибка." },
            { 2,"Приложение выключено." },
            { 3,"Передан неизвестный метод." },
            { 4,"Неверная подпись." },
            { 5,"Авторизация пользователя не удалась." },
            { 6,"Слишком много запросов в секунду." },
            { 7,"Нет прав для выполнения этого действия." },
            { 8,"Неверный запрос." },
            { 9,"Слишком много однотипных действий." },
            { 10,"Произошла внутренняя ошибка сервера." },
            { 11,"В тестовом режиме приложение должно быть выключено или пользователь должен быть залогинен." },
            { 14,"Требуется ввод кода с картинки (Captcha)." },
            { 15,"Доступ запрещён." },
            { 16,"Требуется выполнение запросов по протоколу HTTPS, т.к.пользователь включил настройку, требующую работу через безопасное соединение." },
            { 17,"Требуется валидация пользователя." },
            { 18,"Страница удалена или заблокирована." },
            { 20,"Данное действие запрещено для не Standalone приложений." },
            { 21,"Данное действие разрешено только для Standalone и Open API приложений." },
            { 23,"Метод был выключен." },
            { 24,"Требуется подтверждение со стороны пользователя." },
            { 100,"Один из необходимых параметров был не передан или неверен." },
            { 101,"Неверный API ID приложения." },
            { 113,"Неверный идентификатор пользователя." },
            { 150,"Неверный timestamp." },
            { 200,"Доступ к альбому запрещён." },
            { 201,"Доступ к аудио запрещён." },
            { 203,"Доступ к группе запрещён." },
            { 300,"Альбом переполнен." },
            { 500,"Действие запрещено. Вы должны включить переводы голосов в настройках приложения." },
            { 600,"Нет прав на выполнение данных операций с рекламным кабинетом." },
            { 603,"Произошла ошибка при работе с рекламным кабинетом." }
        };

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="settings"></param>
        public VkApi(VkSettings settings)
        {
            _settings = settings;

            // Обнуляем счетчик запросов раз в секунду
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    _queriesCountLastSecond = 0;
                }
            });
        }

        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<T> Execute<T>(RestRequest request, bool showErrors = true) where T : new()
        {
            while (_queriesCountLastSecond >= _maxQueriesCountPerSecond)
            { }

            var client = new RestClient();
            client.BaseUrl = new Uri(BaseUrl);
            request.AddParameter("access_token", _settings.AccessToken);
            request.AddParameter("v", "5.53");

            var response = await client.Execute<ApiResponse<T>>(request);
            _queriesCountLastSecond++;

            if (response.Data.Error != null && showErrors)
            {
                var errorCode = response.Data.Error.Error_Code;
                await new MessageDialog(ErrorDescriptions.ContainsKey(errorCode) ? ErrorDescriptions[errorCode] : response.Data.Error.Error_Msg).ShowAsync();
            }

            return response.Data.Response;
        }

        /// <summary>
        /// Получить информацию о группах
        /// </summary>
        /// <param name="groupIds">Id групп</param>
        /// <returns></returns>
        public async Task<List<GroupInfo>> GetGroupsById(params string[] groupIds)
        {
            var request = new RestRequest("groups.getById", Method.GET);
            request.AddParameter("group_ids", string.Join(",", groupIds));
            return await Execute<List<GroupInfo>>(request, false);
        }

        /// <summary>
        /// Получить информацию о пользователях
        /// </summary>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        public async Task<List<User>> GetUsersById(params string[] userIds)
        {
            var request = new RestRequest("users.get", Method.GET);
            request.AddParameter("user_ids", string.Join(",", userIds));
            request.AddParameter("fields", "photo_50,city");
            return await Execute<List<User>>(request, false);
        }

        /// <summary>
        /// Получить список постов группы
        /// </summary>
        /// <param name="group">Название группы</param>
        /// <param name="count">Количество постов</param>
        /// <returns></returns>
        public async Task<ListResponse<Post>> GetPosts(long groupId, int count, int offset)
        {
            var request = new RestRequest("wall.get", Method.GET);
            request.AddParameter("owner_id", $"-{groupId}");
            request.AddParameter("count", count);
            request.AddParameter("offset", offset);
            return await Execute<ListResponse<Post>>(request);
        }

        /// <summary>
        /// Запостить на стену
        /// </summary>
        /// <param name="groupId">Id группы</param>
        /// <param name="fromGroup">Добавить запись от лица группы</param>
        /// <param name="message">Текст</param>
        /// <param name="attachments">Прикрепленные документы</param>
        /// <returns></returns>
        public async Task<bool> Post(string groupId, bool fromGroup, string message, List<Attachment> attachments, DateTimeOffset? publishDate = null)
        {
            string attachmentsString = "";
            // Получаем сервер для загрузки фото
            var uploadServer = await GetWallUploadServer(groupId);
            var client = new HttpClient();

            foreach (var attach in attachments)
            {
                string fileName = $"PostImages\\{attach.Photo.Id}.png";

                // Скачиваем фото из группы
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(new Uri(attach.Photo.BiggestPhoto), file);
                await download.StartAsync();

                //Загружаем фото на сервер                
                var multipart = new HttpMultipartFormDataContent();
                multipart.Add(new HttpStreamContent(await file.OpenAsync(FileAccessMode.Read)), "file", fileName);
                var response = await client.PostAsync(new Uri(uploadServer.Upload_Url), multipart);
                var uploadResult = Newtonsoft.Json.JsonConvert.DeserializeObject<UploadResult>(await response.Content.ReadAsStringAsync());

                // Удаляем локальный файл
                await file.DeleteAsync();

                // Сохраняем фото на стене
                var photo = await SaveWallPhoto(groupId, uploadResult);
                attachmentsString += $"photo{photo.Owner_Id}_{photo.Id},";
            }

            // Формируем запрос
            var request = new RestRequest("wall.post", Method.POST);
            request.AddParameter("owner_id", $"-{groupId}");
            request.AddParameter("from_group", fromGroup);
            request.AddParameter("message", message);
            request.AddParameter("attachments", attachmentsString);

            if (publishDate != null)
            {
                var unixTimestamp = (publishDate.Value.UtcDateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                request.AddParameter("publish_date", unixTimestamp);
            }

            var result = await Execute<object>(request);
            return result != null;
        }

        /// <summary>
        /// Получить сервер для загрузки фотографий
        /// </summary>
        /// <param name="groupId">Id группы</param>
        /// <returns></returns>
        public async Task<WallUploadServer> GetWallUploadServer(string groupId)
        {
            var request = new RestRequest("photos.getWallUploadServer", Method.GET);
            request.AddParameter("group_id", groupId);
            return await Execute<WallUploadServer>(request);
        }

        /// <summary>
        /// Сохранить фото в альбоме стены
        /// </summary>
        /// <param name="groupId">Id группы</param>
        /// <param name="uploadResult">Результат загрузки фото на сервер</param>
        /// <returns></returns>
        public async Task<Photo> SaveWallPhoto(string groupId, UploadResult uploadResult)
        {
            var request = new RestRequest("photos.saveWallPhoto", Method.POST);
            request.AddParameter("group_id", groupId);
            request.AddParameter("photo", uploadResult.Photo);
            request.AddParameter("server", uploadResult.Server);
            request.AddParameter("hash", uploadResult.Hash);
            return (await Execute<List<Photo>>(request)).SingleOrDefault();
        }
    }
}
