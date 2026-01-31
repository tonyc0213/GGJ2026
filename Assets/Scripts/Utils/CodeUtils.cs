using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utils
{
    public static class CodeUtils
    {
        public static void SaveTexture2D(Texture2D texture2D, string name)
        {
            var path = GetTextureFullPath(name);
            var bytes = texture2D.EncodeToPNG();
            
            if(!Directory.Exists(GetTextureFolderPath())) {
                Directory.CreateDirectory(GetTextureFolderPath());
            }
            File.WriteAllBytes(path, bytes);
        }

        public static Texture2D LoadTexture2D(string name)
        {
            var path = GetTextureFullPath(name);
            if(!File.Exists(path))
            {
                return null;
            }
            
            var tex = new Texture2D(2, 2);
            var fileData = File.ReadAllBytes(path);
            tex.LoadImage(fileData);
            
            return tex;
        }

        static string GetTextureFolderPath()
        {
            return Path.Combine(Application.persistentDataPath,"textures");
        }
        
        static string GetTextureFullPath(string name)
        {
            return Path.Combine(GetTextureFolderPath(), name) + ".png";
        }

        public static void ShuffleList<T>(this List<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var rnd = UnityEngine.Random.Range(0, list.Count);
                (list[i], list[rnd]) = (list[rnd], list[i]);
            }
        }
    }
}