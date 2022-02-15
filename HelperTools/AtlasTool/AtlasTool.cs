using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AtlasTool
{
    public static string ATLAS_PATH = "Assets/BundleResources/Atlas";
    public static List<ImageInfo> imageinfoList;    //当前图集的图片信息列表
    public static string[] folders;     //图集列表

    static TextureImporterFormat[] ANDROID_FORMATS = new TextureImporterFormat[] { TextureImporterFormat.RGBA32, TextureImporterFormat.ETC2_RGBA8, TextureImporterFormat.ETC2_RGB4 };
    static TextureImporterFormat[] IOS_FORMATS = new TextureImporterFormat[] { TextureImporterFormat.RGBA32, TextureImporterFormat.ASTC_RGBA_6x6, TextureImporterFormat.ASTC_RGB_6x6 };
    public static string[] FORMATS = new string[] { "RGBA32", "RGBA", "RGB", "-" };
    public static Color[] FCOLORS = new Color[] { new Color(1.0f, 1.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), Color.yellow, Color.red };
    public static Dictionary<string, List<ImageInfo>> ATLAS_TO_IMAGE_DICT = new Dictionary<string, List<ImageInfo>>();
    public static GUIContent[] GRID_LIST_CONTENTS = new GUIContent[]{};
	
	static AtlasTool()
	{
		imageinfoList	= new List<ImageInfo>();
		folders			= Directory.GetDirectories(ATLAS_PATH).Concat(Directory.GetDirectories("Assets/BundleResources/NoPacking")).ToArray();
	}

    /// <summary>
    /// 获取图片具体信息
    /// </summary>
    /// <param name="index">图集索引</param>
	public static void GenerateImageInfoList(int index)
	{
        imageinfoList.Clear();
		string folder = folders[index];
		string[] guids = AssetDatabase.FindAssets("t:Texture", new string[]{folder});

		foreach(string guid in guids)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			ImageInfo imageInfo = new ImageInfo();
			imageInfo.imageName = Path.GetFileNameWithoutExtension(path);//获取不带后缀的名称
			TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
			imageInfo.atlasName = ti.spritePackingTag;
			imageInfo.imagePath = path;
			imageInfo.formatIndex = GetFormatIndex(ti);
			imageinfoList.Add(imageInfo);
		}
        imageinfoList.Sort((imgInfo1, imgInfo2) => 
            {
                if(imgInfo1.formatIndex > imgInfo2.formatIndex)
                {
                    return -1;
                }else if (imgInfo1.formatIndex < imgInfo2.formatIndex)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
    }

	/// <summary>
	/// 保存
	/// </summary>
	/// <param name="info"></param>
    public static void Save(ImageInfo info)
    {
        TextureImporter ti	= AssetImporter.GetAtPath(info.imagePath) as TextureImporter;
        TextureImporterPlatformSettings androidTIP = ti.GetPlatformTextureSettings("Android");
        TextureImporterPlatformSettings iosTIP = ti.GetPlatformTextureSettings("iPhone");
		androidTIP.overridden = true;
		iosTIP.overridden = true;
		androidTIP.format = ANDROID_FORMATS[info.formatIndex];
		iosTIP.format = IOS_FORMATS[info.formatIndex];
		ti.SetPlatformTextureSettings(androidTIP);
		ti.SetPlatformTextureSettings(iosTIP);
        ti.SaveAndReimport();
    }

	/// <summary>
	/// 获取格式索引，非法格式返回3
	/// </summary>
	/// <param name="ti"></param>
	/// <returns></returns>
	static int GetFormatIndex(TextureImporter ti)
	{
		TextureImporterPlatformSettings androidTIP = ti.GetPlatformTextureSettings("Android");
		TextureImporterPlatformSettings iosTIP = ti.GetPlatformTextureSettings("iPhone");
		if (!androidTIP.overridden || !iosTIP.overridden)
		{
            Debug.LogError("Not overridden " + ti.name);
			return 3;
		}
		int androidFormatIndex = Array.IndexOf(ANDROID_FORMATS, androidTIP.format);
		int iosFormatIndex = Array.IndexOf(IOS_FORMATS, iosTIP.format);
		if (androidFormatIndex == -1
		|| iosFormatIndex == -1
		|| androidFormatIndex != iosFormatIndex)
		{
            Debug.LogError("Format index not correct: " + ti.name + ", androidFormatIndex:" + androidFormatIndex + " , iosFormatIndex:" + iosFormatIndex);
            return 3;
		}
		return androidFormatIndex;
	}

	/// <summary>
	/// 刷新当前页面(本地资源更新)
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public static int Refresh(int index)
	{
		string currFolderName = folders[index];
		folders = Directory.GetDirectories(ATLAS_PATH);
		int newIndex = Array.IndexOf(folders, currFolderName);
		if (newIndex == -1)
		{
			newIndex = 0;
		}
		GenerateImageInfoList(newIndex);
        RefreshAtalasData();
        GRID_LIST_CONTENTS = GetMarkedAtlasList();
        return newIndex;
	}
    public static void RefreshAtalasData()
    {
        for (int index = 0; index < folders.Length; index++)
        {
            string folder = folders[index];
            string[] guids = AssetDatabase.FindAssets("t:Texture", new string[] { folder });
            List<ImageInfo> tImageList = new List<ImageInfo>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ImageInfo imageInfo = new ImageInfo();
                imageInfo.imageName = Path.GetFileNameWithoutExtension(path);//获取不带后缀的名称
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                imageInfo.atlasName = ti.spritePackingTag;
                imageInfo.imagePath = path;
                imageInfo.formatIndex = GetFormatIndex(ti);
                tImageList.Add(imageInfo);
            }
            tImageList.Sort((imgInfo1, imgInfo2) =>
            {
                if (imgInfo1.formatIndex > imgInfo2.formatIndex)
                {
                    return -1;
                }
                else if (imgInfo1.formatIndex < imgInfo2.formatIndex)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
            ATLAS_TO_IMAGE_DICT[Path.GetFileName(folder)] = tImageList;
        }
    }
    public static GUIContent[] GetMarkedAtlasList()
    {
        var icon_error = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/Editor/AtlasTool/icon_error.png");
        //icon_error.Resize(16, 16);
        var icon_warning = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/Editor/AtlasTool/icon_warning.png");
        //icon_warning.Resize(16, 16);
        List<GUIContent> result = new List<GUIContent>();
        foreach(var key in ATLAS_TO_IMAGE_DICT.Keys)
        {
            //temp image info list
            var tiil = ATLAS_TO_IMAGE_DICT[key];
            int countError = (from t in tiil where t.formatIndex == 3 && !t.atlasName.Equals("") select t ).Count();
            int countWarning = (from t in tiil where t.formatIndex == 2 && !t.atlasName.Equals("") select t).Count();
            int countNormal = (from t in tiil where t.formatIndex < 2 select t).Count();
            GUIContent content = new GUIContent(key);
            if(0 < countError)
            {
                content.image = icon_error;
            }else if (0 < countWarning && 0 < countNormal)
            {
                content.image = icon_warning;
            }
            result.Add(content);
        }
        return result.ToArray();
    }

	[MenuItem("Tools/检测图片格式")]
	public static void GetWarningImgages()
	{
		string[] guids = AssetDatabase.FindAssets("t:Texture", new string[]{ATLAS_PATH, "Assets/BundleResources/NoPacking"});
		for(int i = 0; i < guids.Length; i++)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
			EditorUtility.DisplayProgressBar(string.Format("处理中{0}/{1}", i+1, guids.Length), assetPath, (i + 1) / (float)guids.Length);
			TextureImporter ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			if(GetFormatIndex(ti) == 3)
			{
				Debug.LogError("Format Warning -> " + assetPath);
			}
		}
		EditorUtility.ClearProgressBar();
		Debug.Log("Finish");
	}
}
