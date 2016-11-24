using System;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace VkGrabberUniversal.Utils
{
    public static class SettingsManager
    {
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public static async Task SerializeObject(object serializableObject, string fileName)
        {
            if (serializableObject == null)
                return;

            try
            {
                DataContractSerializer serializer = new DataContractSerializer(serializableObject.GetType());
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                Stream stream = await file.OpenStreamForWriteAsync();

                using (stream)
                {
                    serializer.WriteObject(stream, serializableObject);
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }


        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<T> DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return default(T);

            T objectOut = default(T);

            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);

                var instream = await file.OpenStreamForReadAsync();
                var serializer = new DataContractSerializer(typeof(T));
                objectOut = (T)serializer.ReadObject(instream);
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }
    }
}
