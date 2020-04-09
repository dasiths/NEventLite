using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace NEventLite.StorageProviders.InMemory
{
    public static class SerializerHelper
    {
        private static void SaveToJson(string strFile, object item)
        {
            var serializerSetting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var content = JsonConvert.SerializeObject(item, serializerSetting);

            File.WriteAllText(strFile, content);
        }

        private static object LoadFromJson(string strFile)
        {
            var serializerSetting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var content = File.ReadAllText(strFile);
            var obj = JsonConvert.DeserializeObject<object>(content, serializerSetting);

            return obj;
        }

        public static void SaveToFile(string file, object items)
        {
            SaveToJson(file, items);
        }

        public static object LoadFromFile(string file)
        {
            var results = LoadFromJson(file);
            return results;
        }
    }
}
