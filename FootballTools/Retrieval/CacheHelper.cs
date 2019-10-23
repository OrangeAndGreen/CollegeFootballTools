using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace FootballTools.Retrieval
{
    public static class CacheHelper
    {
        private static readonly string CacheDirectory = "Cache";

        public static T RetrieveItemFromCache<T>(string objectIdentifier)
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    return default(T);
                }

                string filepath = Path.Combine(CacheDirectory, objectIdentifier + ".json");
                if (!File.Exists(filepath))
                {
                    return default(T);
                }

                using (Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while trying to retrieve cache file {objectIdentifier}: {e.Message}");
                return default(T);
            }
        }

        public static bool IsItemInCache(string objectIdentifier)
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    return false;
                }

                string filepath = Path.Combine(CacheDirectory, objectIdentifier + ".json");
                if (!File.Exists(filepath))
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while checking cache for file {objectIdentifier}: {e.Message}");
                return false;
            }
        }

        public static void Cache<T>(string objectIdentifier, T itemToCache)
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                {
                    Directory.CreateDirectory(CacheDirectory);
                }

                string filepath = Path.Combine(CacheDirectory, objectIdentifier + ".json");
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }

                using (Stream stream = new FileStream(filepath, FileMode.CreateNew))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    serializer.WriteObject(stream, itemToCache);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while trying to write cache file {objectIdentifier}: {e.Message}");
            }

        }
    }
}
