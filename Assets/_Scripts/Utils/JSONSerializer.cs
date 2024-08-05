using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace Teston.Utils
{
    public static class JSONSerializer<T>
    {
        public static string GetJson(T data)
        {
            var json = JsonUtility.ToJson(data, true);
            return json;
        }

        public static void Save(T data, string fileName)
        {
            var json = JsonUtility.ToJson(data, true);
            var filePath = Path.Combine(Application.persistentDataPath, string.Format("{0}.json", fileName));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var bytes = Encoding.ASCII.GetBytes(json);
            File.WriteAllBytes(filePath, bytes);
        }

        public static void Load(ref T data, string fileName)
        {
            var filePath = Path.Combine(Application.persistentDataPath, string.Format("{0}.json", fileName));

            if(!File.Exists(filePath)) 
            {
                return;
            }

            var bytes = File.ReadAllBytes(filePath);
            var json = Encoding.UTF8.GetString(bytes);
            data = JsonUtility.FromJson<T>(json);
        }

        public static void Delete(string fileName)
        {
            var filePath = Path.Combine(Application.persistentDataPath, string.Format("{0}.json", fileName));
            File.Delete(filePath);
        }
    }
}