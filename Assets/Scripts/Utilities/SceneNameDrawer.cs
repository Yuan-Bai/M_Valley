#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 仅处理字符串类型的字段
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        // 获取所有场景名称
        string[] sceneNames = GetSceneNames();
        
        // 生成下拉菜单选项
        int selectedIndex = GetSelectedIndex(sceneNames, property.stringValue);
        
        // 绘制下拉菜单
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, sceneNames);
        
        // 更新属性值
        if (selectedIndex >= 0 && selectedIndex < sceneNames.Length)
        {
            property.stringValue = sceneNames[selectedIndex];
        }
    }

    // 获取场景名称数组
    private string[] GetSceneNames()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        string[] names = new string[scenes.Length];
        
        for (int i = 0; i < scenes.Length; i++)
        {
            string path = scenes[i].path;
            names[i] = System.IO.Path.GetFileNameWithoutExtension(path);
        }
        
        return names;
    }

    // 获取当前选中项的索引
    private int GetSelectedIndex(string[] sceneNames, string currentValue)
    {
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i] == currentValue)
                return i;
        }
        return 0; // 默认选中第一个
    }
}
#endif
