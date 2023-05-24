using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Feudality.RuntimeSerialization {
    public static class Serialization {

        public static byte[] SerializeObject<T>(this T serializableObject) {
            using (var stream = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, serializableObject);
                var result = stream.ToArray();
                return result;
            }
        }
        public static T DeserializeObject<T>(this byte[] serilizedBytes) {
            using (var stream = new MemoryStream(serilizedBytes)) {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }

        public static void SaveFile(string path, string fileName, byte[] bytes) {
            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);
            var fullName = Path.Combine(path, fileName);
            //Debug.LogErrorFormat("SaveFile {0}, {1}, {2}", path, fileName, fullName);
            File.WriteAllBytes(fullName, bytes);
        }        
        public static byte[] LoadFile(string path) {
            if (!File.Exists(path)) {
                Debug.LogErrorFormat("Can't load file. Path {0} is incorrect!", path);
                return null;
            }

            var bytes = File.ReadAllBytes(path);
            return bytes;
        }
    }
}