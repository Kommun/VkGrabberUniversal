using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace VkGrabberUniversal.Utils
{
    public class AppSettings : PropertyChangedBase
    {
        public ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public void AddOrUpdateValue(string Key, object value)
        {
            if (settings.Values[Key] != value)
                settings.Values[Key] = value;
            OnPropertyChanged(Key);
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            if (settings.Values.ContainsKey(Key))
                value = (T)settings.Values[Key];
            else
                value = defaultValue;

            return value;
        }

        #region Launch 

        public int Runs
        {
            get { return GetValueOrDefault<int>("Runs", 0); }
            set { AddOrUpdateValue("Runs", value); }
        }

        public bool NotRated
        {
            get { return GetValueOrDefault<bool>("NotRated", true); }
            set { AddOrUpdateValue("NotRated", value); }
        }

        public bool IsFirstLaunch
        {
            get { return GetValueOrDefault<bool>("IsFirstLaunch", true); }
            set { AddOrUpdateValue("IsFirstLaunch", value); }
        }

        #endregion

        /// <summary>
        /// Количество репостов
        /// </summary>
        public int RepostsCount
        {
            get { return GetValueOrDefault<int>("RepostsCount", 0); }
            set { AddOrUpdateValue("RepostsCount", value); }
        }

        /// <summary>
        /// Полная версия
        /// </summary>
        public bool IsFullVersion
        {
            get { return GetValueOrDefault<bool>("IsFullVersion", false); }
            set { AddOrUpdateValue("IsFullVersion", value); }
        }
    }
}