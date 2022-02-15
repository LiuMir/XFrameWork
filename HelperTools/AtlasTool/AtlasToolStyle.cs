using UnityEngine;

public sealed class AtlasToolStyle
{
	static GUIStyle m_bgStyle;
	static GUIStyle m_buttonStyle;
	static GUIStyle m_buttonStyle2;
	static GUIStyle m_labelStyle;
	static GUIStyle m_inputStyle;
	static GUIStyle m_gridStyle;
	static GUIStyle m_popupStyle;

	public static GUIStyle buttonStyle
	{
		get
		{
			if (m_buttonStyle == null)
			{
				m_buttonStyle = new GUIStyle(GUI.skin.button);
				m_buttonStyle.fontSize = 14;
			}
			return m_buttonStyle;
		}
	}

	public static GUIStyle buttonStyle2
	{
		get
		{
			if (m_buttonStyle2 == null)
			{
				m_buttonStyle2 = new GUIStyle(GUI.skin.textField);
				m_buttonStyle2.active = GUI.skin.button.active;
				m_buttonStyle2.fontSize = 14;
				m_buttonStyle2.alignment = TextAnchor.MiddleLeft;
			}
			return m_buttonStyle2;
		}
	}

	public static GUIStyle labelStyle
	{
		get
		{
			if (m_labelStyle == null)
			{
				m_labelStyle = new GUIStyle(GUI.skin.label);
				m_labelStyle.fontSize = 14;
				m_labelStyle.alignment = TextAnchor.MiddleLeft;
			}
			return m_labelStyle;
		}
	}

	public static GUIStyle bgStyle
	{
		get
		{
			if (m_bgStyle == null)
			{
				m_bgStyle = new GUIStyle(GUI.skin.textField);
			}
			return m_bgStyle;
		}
	}

	public static GUIStyle inputStyle
	{
		get
		{
			if (m_inputStyle == null)
			{
				m_inputStyle = new GUIStyle(GUI.skin.textField);
				m_inputStyle.fontSize = 14;
				m_inputStyle.alignment = TextAnchor.MiddleLeft;
			}
			return m_inputStyle;
		}
	}

	public static GUIStyle popupStyle
	{
		get
		{
			if (m_popupStyle == null)
			{
				m_popupStyle = new GUIStyle(GUI.skin.button);
				m_popupStyle.fontSize = 12;

			}
			return m_popupStyle;
		}
	}

	public static GUIStyle gridStyle
	{
		get
		{
			if (m_gridStyle == null)
			{
				m_gridStyle = new GUIStyle(GUI.skin.textField);
				m_gridStyle.fontSize = 14;
				m_gridStyle.active = GUI.skin.button.active;
				m_gridStyle.onHover = GUI.skin.button.active;
				m_gridStyle.onNormal = GUI.skin.button.active;
				m_gridStyle.alignment = TextAnchor.MiddleLeft;
			}
			return m_gridStyle;
		}
	}
}
