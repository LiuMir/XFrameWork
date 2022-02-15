using System.IO;
using UnityEditor;
using UnityEngine;

public class AtlasWindow : EditorWindow
{
    static AtlasWindow	m_window;					//窗口
    static Vector2		m_leftScrollPosition;		//左侧滚动定位
	static Vector2		m_rightScrollPosition;		//右侧列表定位
    static int			m_index = 0;				//选中索引

    [MenuItem("Tools/图集管理 &p")]
    public static void ShowWindow()
    {
        m_window = GetWindow<AtlasWindow>("图集管理");
        m_window.Show();
        m_window.maxSize = new Vector2(960, 800);

		AtlasTool.GenerateImageInfoList(0);
    }

    void OnGUI()
    {
		if(AtlasTool.folders == null)
		{
			return;
		}
		string[] folderNames = new string[AtlasTool.folders.Length];
        GUIContent[] atlasItems = new GUIContent[AtlasTool.folders.Length];
        for (int i = 0; i < folderNames.Length; i++)
        {
            folderNames[i] = Path.GetFileName(AtlasTool.folders[i]);
        }

        EditorGUILayout.BeginHorizontal();

        Space();    //左边间隙
        GUILayout.BeginVertical(AtlasToolStyle.bgStyle, GUILayout.Width(200), GUILayout.Height(780));
        Space(5);
		GUILayout.BeginHorizontal();
        GUILayout.Label("图集列表", AtlasToolStyle.labelStyle);
		if(GUILayout.Button("刷新", AtlasToolStyle.buttonStyle))
		{
			//刷新页面
			AtlasTool.Refresh(m_index);
		}
		GUILayout.EndHorizontal();
		Space(5);
        m_leftScrollPosition = GUILayout.BeginScrollView(m_leftScrollPosition);

        int index = 0;
        if( 0 < AtlasTool.GRID_LIST_CONTENTS.Length)
        {
            index = GUILayout.SelectionGrid(m_index, AtlasTool.GRID_LIST_CONTENTS, 1);
        }
        else
        {
            index = GUILayout.SelectionGrid(m_index, folderNames, 1, AtlasToolStyle.gridStyle);
        }
        
        if (index != m_index)
		{
			m_index = index;
			AtlasTool.GenerateImageInfoList(m_index);
		}

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        Space();
        GUILayout.BeginVertical(AtlasToolStyle.bgStyle, GUILayout.Height(780));
        Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("图片名", AtlasToolStyle.labelStyle, GUILayout.Width(200));
        GUILayout.Label("图集名", AtlasToolStyle.labelStyle, GUILayout.Width(150));
        Space(30);
        GUILayout.Label("格式", AtlasToolStyle.labelStyle, GUILayout.Width(100));

        GUILayout.EndHorizontal();

        m_rightScrollPosition = GUILayout.BeginScrollView(m_rightScrollPosition);
        // normal background color
        Color nbc = GUI.backgroundColor;
        Debug.Log(nbc);
        for (int i = 0; i < AtlasTool.imageinfoList.Count; i++)
        {
            ImageInfo info = AtlasTool.imageinfoList[i];
            if (!info.atlasName.Equals(""))
            { 
                GUI.backgroundColor = AtlasTool.FCOLORS[info.formatIndex];
            }
            GUILayout.BeginHorizontal(AtlasToolStyle.bgStyle);
            EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(info.imagePath), typeof(UnityEngine.Object), true, GUILayout.Width(200), GUILayout.Height(20));
            GUILayout.Label(info.atlasName, AtlasToolStyle.labelStyle, GUILayout.Width(150));
			int formatIndex = EditorGUILayout.Popup(info.formatIndex, AtlasTool.FORMATS, AtlasToolStyle.popupStyle, GUILayout.Width(90), GUILayout.Height(20));
            
            if (formatIndex != info.formatIndex)
			{
				if(formatIndex != 3)
				{
					info.formatIndex = formatIndex;
					AtlasTool.Save(info);
                }
			}
            GUILayout.EndHorizontal();
            GUI.backgroundColor = nbc;
        }
        GUILayout.BeginVertical();
        Texture2D[] pts = UnityEditor.Sprites.Packer.GetTexturesForAtlas(folderNames[m_index]);
        foreach (Texture2D t2d in pts)
        {
            GUILayout.Box(t2d, GUILayout.Width(300), GUILayout.Height(300));
            Space();
        }        
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        Space();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        Space();
        GUILayout.EndHorizontal();
        Space();
    }

    static void Space(int space = 10)
    {
        GUILayout.Space(space);
    }
}
