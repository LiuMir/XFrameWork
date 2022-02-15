/****************************************************
    文件：WrongSpriteChecker.cs
    作者：Chaox
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WrongSpriteChecker
{
    private static List<Texture> assetsList = new List<Texture>();

    public class texturesInfo
    {
        public string file;
        public string CopyPath;
    }

    [MenuItem("Tools/图片尺寸修改")]
    private static void FindWrongSizeSprite()
    {
        assetsList.Clear();

        //List<Object> list = GetTextures("Assets/BundleResources/Atlas").Concat(GetTextures("Assets/Content/Effect/TEX_NEW")).ToList();
        string localpath = Application.dataPath.Replace("Assets", "Localization").Replace('\\', '/');
        string creatpath = localpath.Replace("Localization", "local");
        Directory.CreateDirectory(creatpath);
        List<texturesInfo> textures = new List<texturesInfo>();
        string[] guids = AssetDatabase.FindAssets("t:Texture", new string[] { "Assets/BundleResources/NoPacking" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null)
            {
                continue;
            }
            if (!string.IsNullOrEmpty(ti.spritePackingTag))
            {
                continue;
            }
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
            if (texture == null)
            {
                continue;
            }

            if (texture.width % 4 != 0 || texture.height % 4 != 0)
            {
                if (texture.width < 3 || texture.height < 3)
                {
                    continue;
                }

                int newWidth = texture.width;
                int newHeight = texture.height;
                if (texture.width % 4 != 0)
                {
                    if (texture.width % 4 == 1)
                    {
                        newWidth = texture.width - 1;
                    }
                    else if (texture.width % 4 == 2)
                    {
                        newWidth = texture.width - 2;
                    }
                    else if (texture.width % 4 == 3)
                    {
                        newWidth = texture.width + 1;
                    }
                }
                if (texture.height % 4 != 0)
                {
                    if (texture.height % 4 == 1)
                    {
                        newHeight = texture.height - 1;
                    }
                    else if (texture.height % 4 == 2)
                    {
                        newHeight = texture.height - 2;
                    }
                    else if (texture.height % 4 == 3)
                    {
                        newHeight = texture.height + 1;
                    }
                }


                DirectoryInfo topDir = Directory.GetParent(path);

                string parentName = topDir.Name;
                string pathName = Path.GetFileName(path);
                string fileName = parentName + "/" + pathName;
                string newcreatpath = creatpath + "/" + parentName;
                //string pathName = GetRelativeAssetsName(path);//Path.GetFileName(path);
                if (!Directory.Exists(newcreatpath))
                {
                    Directory.CreateDirectory(newcreatpath);
                }
                texturesInfo info = new texturesInfo();

                string file = Application.dataPath + "/" + path;
                file = file.Replace(Application.dataPath + "/" + "Assets", Application.dataPath);
                info.file = file;
                info.CopyPath = newcreatpath + "/" + pathName;
                if (Path.GetExtension(file) == ".png")
                {
                    #if !UNITY_IPHONE
                    using (System.Drawing.Image pic = System.Drawing.Image.FromFile(@file))
                    {
                        System.Drawing.Bitmap img = new System.Drawing.Bitmap(pic, newWidth, newHeight);
                        img.Save(info.CopyPath);
                        textures.Add(info);
                    }
                    assetsList.Add(texture);
                    Debug.LogError("file = " + file + " PNG格式: 已处理");
                    #endif
                }
                else if (Path.GetExtension(file) != ".meta")
                {
                    Debug.LogError("file = " + file + ", 非PNG格式: 未处理");
                }

                //string[] files = Directory.GetFiles(path);

                //foreach (var file in files)
                //{
                //    info.file = file;
                //    info.CopyPath = newcreatpath + "/" + pathName;
                //    if (Path.GetExtension(file) == ".meta")
                //    {
                //        continue;
                //    }
                //    if (Path.GetExtension(file) == ".png")
                //    {
                //        using (System.Drawing.Image pic = System.Drawing.Image.FromFile(@file))
                //        {
                //            System.Drawing.Bitmap img = new System.Drawing.Bitmap(pic, newWidth, newHeight);
                //            img.Save(info.CopyPath);
                //            textures.Add(info);
                //        }
                //        assetsList.Add(texture);
                //    }
                //    else
                //    {
                //        Debug.LogError("file = " + file + ", 非PNG格式。");
                //    }
                //}
            }
        }
        foreach (var info in textures)
        {
            FileInfo fi = new FileInfo(info.CopyPath);
            fi.CopyTo(info.file, true);
        }
        Directory.Delete(creatpath, true);
        Debug.Log("Image Modify Done ! ");
    }

    [MenuItem("Tools/查找大尺寸非NoPacking图片")]
    private static void FindAbBigImage()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture", new string[] { "Assets/BundleResources/Atlas" });
        float count = 0;
        foreach (string guid in guids)
        {
            ++count;
            EditorUtility.DisplayProgressBar("查找大尺寸非NoPacking图片", string.Format("Finding ({0}/{1})", count, guids.Length), count / guids.Length);

            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null)
            {
                continue;
            }
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
            if (texture == null)
            {
                continue;
            }
            if (texture.width >= 512 || texture.height >= 512)
            {
                if (!ti.assetPath.Contains("WorldMap_"))
                {
                    Debug.LogError(ti.assetPath);
                }
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log(" Done ! ");
    }

    [MenuItem("Assets/改变图片锚点,手动改Pivot为Custom")]
    private static void changePivot()
    {
       Texture[] textures = Selection.GetFiltered<Texture>(SelectionMode.DeepAssets);

        foreach (var texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.spritePivot = new Vector2(0.5f, 0);
            importer.SaveAndReimport();
        }
        AssetDatabase.Refresh();

        return;
    }
}
