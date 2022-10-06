using UnityEditor;
using UnityEngine;

public class ShopItemEditorMenu : EditorWindow
{
    private Editor m_MyScriptableObjectEditor;
    private ShopItems _items;
    
    
    [MenuItem("Intruder/Shop Items")]
    private static void ShowWindow()
    {
        GetWindow<ShopItemEditorMenu>().Show();
    }
    
    private void OnEnable()
    {
        _items = ShopItems.GetInstance();
        m_MyScriptableObjectEditor = Editor.CreateEditor(_items);
    }
 
    private void OnGUI()
    {
        if(m_MyScriptableObjectEditor == null)
            OnEnable();
        m_MyScriptableObjectEditor.OnInspectorGUI();
    }
}
