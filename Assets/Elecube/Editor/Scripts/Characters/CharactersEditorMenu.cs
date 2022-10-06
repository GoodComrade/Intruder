using UnityEditor;
using UnityEngine;

public class CharactersEditorMenu : EditorWindow
{
    private Editor m_MyScriptableObjectEditor;
    private Characters _characters;

    
    [MenuItem("Intruder/Characters")]
    private static void ShowWindow()
    {
        GetWindow<CharactersEditorMenu>().Show();
    }
    
    private void OnEnable()
    {
        _characters = Characters.GetInstance();
        m_MyScriptableObjectEditor = Editor.CreateEditor(_characters);
    }
 
    private void OnGUI()
    {
        if(m_MyScriptableObjectEditor == null)
            OnEnable();
        m_MyScriptableObjectEditor.OnInspectorGUI();
    }
}
