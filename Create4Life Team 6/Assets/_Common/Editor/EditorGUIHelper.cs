using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

public static class EditorGUIHelper 
{
	private static GUIStyle s_TempStyle = new GUIStyle();

	public static void DrawTexture(Rect position, Texture2D texture) 
	{
		if (Event.current.type != EventType.Repaint)
			return;

		s_TempStyle.normal.background = texture;

		s_TempStyle.Draw(position, GUIContent.none, false, false, false, false);
	}

	public static void LineSeparator()
	{
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
	}

	public static void DrawSpaces(int numSpaces)
	{
		for(int i = 0 ; i < numSpaces; i++)
		{
			EditorGUILayout.Space();
		}
	}
		
	public static object GetParent(SerializedProperty prop)
	{
		if(prop != null)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');

			foreach(var element in elements.Take(elements.Length-1))
			{
				if(element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("["));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
					obj = GetValue(obj, elementName, index);
				}
				else
				{
					obj = GetValue(obj, element);
				}
			}
			return obj;
		}
		return null;
	}

	public static SerializedObject GetParentAsSerializedObject(SerializedProperty prop)
	{
		if(prop != null)
		{
			object obj = GetParent(prop);
			if(obj != null)
			{
				SerializedObject property = (SerializedObject)obj;
				return property;
			}
			return null;
		}
		return null;
	}

	public static object GetValue(object source, string name)
	{
		if(source == null)
			return null;
		var type = source.GetType();
		var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		if(f == null)
		{
			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if(p == null)
				return null;
			return p.GetValue(source, null);
		}
		return f.GetValue(source);
	}

	public static object GetValue(object source, string name, int index)
	{
		var enumerable = GetValue(source, name) as IEnumerable;
		var enm = enumerable.GetEnumerator();
		while(index-- >= 0)
			enm.MoveNext();
		return enm.Current;
	}

	public static void GetTexturePropertiesAndURLS(ref Material material,out string[] properties,out string[] URLs)
	{
		if(material != null)
		{
			Shader shader = material.shader;
			List<string> validProperties = new List<string>();
			List<string> validURLs = new List<string>();

			for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
			{
				if(ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
				{
					string propertyName = ShaderUtil.GetPropertyName(shader, i);
					Texture img = material.GetTexture(propertyName);
					if(img != null)
					{
						validProperties.Add(propertyName);

						string fullDataPath = AssetDatabase.GetAssetPath(img);
						int resourcesIndex = fullDataPath.LastIndexOf("Resources/");
						string finalPathWithExtension = string.Empty;
						string finalPath = string.Empty;
						if(resourcesIndex >= 0)
						{
							int endResourcesIndex = resourcesIndex+10;
							finalPathWithExtension = fullDataPath.Substring(endResourcesIndex);

							int extensionIndex = finalPathWithExtension.LastIndexOf(".");
							if(extensionIndex >= 0)
							{
								finalPath = finalPathWithExtension.Substring(0,extensionIndex);
							}
							validURLs.Add(finalPath);
						}
					}
				}
			}

			properties = validProperties.ToArray();
			URLs = validURLs.ToArray();
		}
		else
		{
			properties = null;
			URLs = null;
		}

	}

	public static void GetAudioClipURL(ref AudioClip audioclip,out string URL)
	{
		string fullDataPath = AssetDatabase.GetAssetPath(audioclip);
		int resourcesIndex = fullDataPath.LastIndexOf("Resources/");
		string finalPathWithExtension = string.Empty;
		if(resourcesIndex >= 0)
		{
			int endResourcesIndex = resourcesIndex+10;
			finalPathWithExtension = fullDataPath.Substring(endResourcesIndex);
			int extensionIndex = finalPathWithExtension.LastIndexOf(".");
			if(extensionIndex >= 0)
			{
				URL = finalPathWithExtension.Substring(0,extensionIndex);
			}
			else
			{
				URL = finalPathWithExtension;
			}
		}
		else
		{
			URL = string.Empty;
		}
	}
		
	public static void ShowFloat( float f, string label )
	{
		EditorGUILayout.LabelField( label, f.ToString() );
	}

	public static void ShowString( string text, string label )
	{
		EditorGUILayout.LabelField( label, text );
	}

	public static float GetFloat( float f, string label, string unit, string tooltip = null )
	{
		GUIStyle unitStyle = new GUIStyle( EditorStyles.label );
		unitStyle.fixedWidth = 65;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( new GUIContent( label, tooltip ), GUILayout.Width(180) );

		float f_ret = EditorGUILayout.FloatField( f,  new GUIStyle( EditorStyles.numberField ));

		if ( !string.IsNullOrEmpty( unit ) )
		{
			GUILayout.Label( unit, unitStyle);
		}
		else
		{
			GUILayout.Label( " ", unitStyle);
		}

		EditorGUILayout.EndHorizontal();

		return f_ret;
	}



	public static int GetInt( int f, string label, string unit, string tooltip = null )
	{
		EditorGUILayout.BeginHorizontal();
		GUIStyle labelStyle = new GUIStyle( EditorStyles.label );
		labelStyle.fixedWidth = 180;
		GUIStyle unitStyle = new GUIStyle( EditorStyles.label );
		unitStyle.fixedWidth = 65;
		GUILayout.Label( new GUIContent( label, tooltip ), labelStyle );
		//EditorGUILayout.Space();
		int f_ret = EditorGUILayout.IntField( f, new GUIStyle( EditorStyles.numberField ) );
		if ( !string.IsNullOrEmpty( unit ) )
		{
			GUILayout.Label( unit, unitStyle );
		}
		else
		{
			GUILayout.Label( " ", unitStyle );
		}
		EditorGUILayout.EndHorizontal();
		return f_ret;
	}

	public static float GetFloat( float f, string label, float sliderMin, float sliderMax, string unit )
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label( label, new GUIStyle( EditorStyles.label ) );
		//EditorGUILayout.Space();
		float f_ret = f;
		f_ret = EditorGUILayout.FloatField( f_ret, new GUIStyle( EditorStyles.numberField ), GUILayout.Width( 50 ) );
		f_ret = GUILayout.HorizontalSlider( f_ret, sliderMin, sliderMax );

		if ( !string.IsNullOrEmpty( unit ) )
		{
			GUILayout.Label( unit, new GUIStyle( EditorStyles.label ) );
		}
		else
		{
			GUILayout.Label( " ", new GUIStyle( EditorStyles.label ) );
		}

		EditorGUILayout.EndHorizontal();
		return f_ret;
	}

	public static float GetFloatPercent( float f, string label, string unit, string tooltip = null )
	{
		EditorGUILayout.BeginHorizontal();
		//GUILayout.Label( label, styleLabel );
		EditorGUILayout.LabelField( new GUIContent( label, tooltip ), GUILayout.Width( 180 ) );
		//EditorGUILayout.Space();
		float f_ret = f;
		f_ret = (float)EditorGUILayout.IntField( Mathf.RoundToInt( f_ret * 100 ), new GUIStyle( EditorStyles.numberField ), GUILayout.Width( 50 ) ) / 100;
		f_ret = GUILayout.HorizontalSlider( f_ret, 0, 1 );

		if ( !string.IsNullOrEmpty( unit ) )
		{
			GUILayout.Label( unit, new GUIStyle( EditorStyles.label ) );
		}
		else
		{
			GUILayout.Label( " ", new GUIStyle( EditorStyles.label ) );
		}

		EditorGUILayout.EndHorizontal();
		return f_ret;
	}

	public static float GetFloatPlusMinusPercent( float f, string label, string unit )
	{
		GUIStyle styleLabel = new GUIStyle( EditorStyles.label );
		styleLabel.fixedWidth = 180;
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label( label, styleLabel );
		//EditorGUILayout.Space();
		float f_ret = f;
		f_ret = (float) EditorGUILayout.IntField( Mathf.RoundToInt( f_ret * 100 ), new GUIStyle( EditorStyles.numberField ), GUILayout.Width( 50 ) ) / 100;
		f_ret = GUILayout.HorizontalSlider( f_ret, -1, 1 );
		if ( !string.IsNullOrEmpty( unit ) )
		{
			GUILayout.Label( unit, new GUIStyle( EditorStyles.label ) );
		}
		else
		{
			GUILayout.Label( " ", new GUIStyle( EditorStyles.label ) );
		}
		EditorGUILayout.EndHorizontal();
		return f_ret;
	}

	public static bool EditFloat(UnityEngine.Object obj, string responsableName, ref float f, string label )
	{
		float new_f = GetFloat( f, label, null );

		if ( new_f != f )
		{
			LogUndo(obj, responsableName, label );
			f = new_f;
			return true;
		}

		return false;
	}

	public static void LogUndo(UnityEngine.Object obj, string responsableName,string label)
	{
		Undo.RecordObject( obj, responsableName + ": " + label );
	}

	public static bool EditFloat(UnityEngine.Object obj, string responsableName, ref float f, string label, string unit, string tooltip = null )
	{
		float new_f = GetFloat( f, label, unit, tooltip );

		if ( new_f != f )
		{
			LogUndo(obj,responsableName, label );
			f = new_f;
			return true;
		}

		return false;
	}

	public static float GetFloat01( float f, string label )
	{
		return Mathf.Clamp01( GetFloatPercent( f, label, null ) );
	}

	public static float GetFloat01( float f, string label, string unit, string tooltip = null )
	{
		return Mathf.Clamp01( GetFloatPercent( f, label, unit, tooltip ) );
	}

	public static float GetFloatPlusMinus1( float f, string label, string unit )
	{
		return Mathf.Clamp( GetFloatPlusMinusPercent( f, label, unit ), -1, 1 );
	}

	public static float GetFloatWithinRange( float f, string label, float minValue, float maxValue )
	{
		return Mathf.Clamp( GetFloat( f, label, minValue, maxValue, null ), minValue, maxValue );
	}

	public static bool EditFloatWithinRange(UnityEngine.Object obj, string responsableName, ref float f, string label, float minValue, float maxValue )
	{
		float new_f = GetFloatWithinRange( f, label, minValue, maxValue );

		if ( new_f != f )
		{
			LogUndo(obj,responsableName, label );
			f = new_f;
			return true;
		}

		return false;
	}

	public static bool EditInt(UnityEngine.Object obj, string responsableName, ref int f, string label )
	{
		int new_f = GetInt( f, label, null );

		if ( new_f != f )
		{
			LogUndo(obj,responsableName, label );
			f = new_f;
			return true;
		}
		return false;
	}

	public static bool EditInt(UnityEngine.Object obj, string responsableName, ref int f, string label, string unit, string tooltip = null )
	{
		int new_f = GetInt( f, label, unit, tooltip );

		if ( new_f != f )
		{
			LogUndo(obj,responsableName, label );
			f = new_f;
			return true;
		}
		return false;
	}

	public static bool EditFloat01(UnityEngine.Object obj, string responsableName, ref float f, string label )
	{
		float new_f = GetFloat01( f, label);

		if ( new_f != f )
		{
			LogUndo(obj,responsableName, label );
			f = new_f;
			return true;
		}

		return false;
	}

	public static bool EditFloat01(UnityEngine.Object obj, string responsableName, ref float f, string label, string unit, string tooltip = null )
	{
		float new_f = GetFloat01( f, label, unit, tooltip );

		if ( new_f != f )
		{
			LogUndo(obj,responsableName, label );
			f = new_f;
			return true;
		}

		return false;
	}

	public static bool EditFloatPlusMinus1(UnityEngine.Object obj, string responsableName, ref float f, string label, string unit )
	{
		float new_f = GetFloatPlusMinus1( f, label, unit );

		if ( new_f != f )
		{
			LogUndo(obj,responsableName, label );
			f = new_f;
			return true;
		}

		return false;

	}

	public static bool GetBool( bool b, string label, string tooltip = null )
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( new GUIContent( label, tooltip ), GUILayout.Width( 180 ) );

		bool b_ret = EditorGUILayout.Toggle( b, GUILayout.Width( 20 ) );
		EditorGUILayout.EndHorizontal();
		return b_ret;
	}

	public static bool EditBool(UnityEngine.Object obj, string responsableName, ref bool b, string label, string tooltip = null ) // returns was changed state
	{
		bool new_b = GetBool( b, label, tooltip );

		if ( new_b != b )
		{
			LogUndo(obj,responsableName, label );
			b = new_b;
			return true;
		}

		return false;
	}

	public static bool EditPrefab<T>(UnityEngine.Object obj, string responsableName, ref T prefab, string label, string tooltip = null ) where T : UnityEngine.Object
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( new GUIContent( label, tooltip ), GUILayout.Width( 180 ) );
		//GUILayout.Label( label, styleLabel );
		T new_f = (T) EditorGUILayout.ObjectField( prefab, typeof( T ), false );
		EditorGUILayout.EndHorizontal();

		if ( new_f != prefab )
		{
			LogUndo(obj,responsableName, label );
			prefab = new_f;
			return true;
		}
		return false;
	}

	public static bool EditString(UnityEngine.Object obj, string responsableName, ref string txt, string label, GUIStyle styleText = null, string tooltip = null )
	{
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField( new GUIContent( label, tooltip ), GUILayout.Width( 180 ) );

		//BeginEditableField();

		string newTxt;
		if ( styleText != null )
		{
			newTxt = EditorGUILayout.TextField( txt, styleText );
		}
		else
		{
			newTxt = EditorGUILayout.TextField( txt );
		}
		//EndEditableField();
		EditorGUILayout.EndHorizontal();

		if ( newTxt != txt )
		{
			LogUndo(obj,responsableName, label );
			txt = newTxt;
			return true;
		}
		return false;
	}

	public static bool EditString(Rect fullRect,UnityEngine.Object obj, string responsableName, ref string txt, string label, string tooltip = null )
	{

		Rect labelRect = fullRect;
		labelRect.width = 0.4f*fullRect.width;

		EditorGUI.LabelField(labelRect, new GUIContent( label, tooltip ));

		Rect fieldRect = fullRect;
		fieldRect.width = 0.4f*fullRect.width;
		fieldRect.x = labelRect.width + 0.1f*fullRect.width;

		string newTxt;
		newTxt = EditorGUI.TextField(fieldRect, txt);

		if ( newTxt != txt )
		{
			LogUndo(obj,responsableName, label );
			txt = newTxt;
			return true;
		}
		return false;
	}




	public static int Popup(UnityEngine.Object obj, string responsableName, string label, int selectedIndex, string[] content, string tooltip = null, bool sortAlphabetically = true )
	{
		return PopupWithStyle(obj,responsableName, label, selectedIndex, content, new GUIStyle( EditorStyles.popup ), tooltip, sortAlphabetically );
	}

	public class ContentWithIndex
	{
		public string content;
		public int index;

		public ContentWithIndex( string content, int index )
		{
			this.content = content;
			this.index = index;
		}
	}

	public static int PopupWithStyle(UnityEngine.Object obj, string responsableName,  string label, int selectedIndex, string[] content, GUIStyle style, string tooltip = null, bool sortAlphabetically = true )
	{
		string[ ] contentSorted;

		List<ContentWithIndex> list = null;

		if ( content.Length == 0 )
		{
			sortAlphabetically = false;
		}

		if ( sortAlphabetically )
		{
			list = _CreateContentWithIndexList( content );
			contentSorted = new string[ content.Length ];
			int index = 0;
			foreach ( var el in list )
			{
				contentSorted[ index++ ] = el.content;
			}
		}
		else
			contentSorted = content;

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField( new GUIContent( label, tooltip ), GUILayout.Width( 180 ) );
		int newIndex;
		if ( sortAlphabetically )
		{
			newIndex = EditorGUILayout.Popup( list.FindIndex( x => x.index == selectedIndex ) , contentSorted, style );

			newIndex = list[ newIndex ].index;
		}
		else
		{
			newIndex = EditorGUILayout.Popup( selectedIndex, contentSorted, style );
		}
		EditorGUILayout.EndHorizontal();
		if ( newIndex != selectedIndex )
		{
			LogUndo(obj,responsableName, label );
		}
		return newIndex;
	}

	private static List<ContentWithIndex> _CreateContentWithIndexList( string[ ] content )
	{
		var list = new List<ContentWithIndex>();
		for ( int i = 0; i < content.Length;i++ )
		{
			list.Add( new ContentWithIndex( content[i], i ) );
		}
		return list.OrderBy( x => x.content ).ToList();
	}

	public static Enum EnumPopup(UnityEngine.Object obj, string responsableName, string label, Enum selectedEnum, string tooltip = null )
	{
		EditorGUILayout.BeginHorizontal();
		//GUILayout.Label( label, styleLabel );
		EditorGUILayout.LabelField( new GUIContent( label, tooltip ), GUILayout.Width( 180 ) );
		Enum newEnum = EditorGUILayout.EnumPopup( selectedEnum, new GUIStyle( EditorStyles.popup ) );
		EditorGUILayout.EndHorizontal();

		if ( !object.Equals( newEnum, selectedEnum ) )
		{
			LogUndo(obj,responsableName, label );
		}
		return newEnum;
	}
		
	public static void DrawTextFieldWithLabel(Rect fullRect, string label,ref SerializedProperty property, string tooltip = null )
	{
		Rect labelRect = fullRect;
		labelRect.width = 0.5f*labelRect.width;

		Rect fieldRect = fullRect;
		fieldRect.width = 0.5f*fieldRect.width;
		fieldRect.x = labelRect.width;

		EditorGUI.LabelField(labelRect,new GUIContent(label,tooltip));
		property.stringValue = EditorGUI.TextField(fieldRect,property.stringValue);

	}
		
	public static int GetIndexFromPath(string path)
	{
		int lastOpeningIndex = path.LastIndexOf("[");
		if(lastOpeningIndex >= 0)
		{
			int index;
			int length = path.Length - lastOpeningIndex - 2;
			string possibleIndex = path.Substring(lastOpeningIndex+1,length);
			if(Int32.TryParse(possibleIndex,out index))
			{
				return index;
			}
			else
			{
				return 0;
			}
		}
		return 0;
	}

	public static UnityEngine.Object InstantiatePrefabAs(GameObject prefab)
	{
		return PrefabUtility.InstantiatePrefab(prefab);
	}
}
