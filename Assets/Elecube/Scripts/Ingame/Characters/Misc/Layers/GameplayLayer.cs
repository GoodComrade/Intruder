#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable]
public class GameplayLayer
{
    [SerializeField]
    private int m_LayerIndex = 0;
    public int LayerIndex
    {
        get { return m_LayerIndex; }
    }
  
    public void Set(int _layerIndex)
    {
        if (_layerIndex > 0 && _layerIndex < 32)
        {
            m_LayerIndex = _layerIndex;
        }
    }
  
    public int Mask
    {
        get { return 1 << m_LayerIndex; }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(GameplayLayer))]
public class SingleUnityLayerPropertyDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginProperty(_position, GUIContent.none, _property);
        SerializedProperty layerIndex = _property.FindPropertyRelative("m_LayerIndex");
        _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
        if (layerIndex != null)
        {
            layerIndex.intValue = EditorGUI.LayerField(_position, layerIndex.intValue);
        }
        EditorGUI.EndProperty( );
    }
}
#endif