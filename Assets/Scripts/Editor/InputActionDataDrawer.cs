using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InputActionData))]
public sealed class InputActionDataDrawer : PropertyDrawer
{
    string[] m_guids;
    string[] m_labels;

    private void Init()
    {
        if (m_guids != null) return;

        var observer = InputObserver.Get;
        var asset = (observer != null) ? observer.asset : null;
        if (asset == null) return;

        m_guids = asset.Select(a => $"{a.id.ToString()}").ToArray();
        m_labels = asset.Select(a => $"{a.actionMap.name}{'\uFF0F'.ToString()}{a.name}").ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init();
        
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (m_guids != null) DrawStringAsPopup(in position, property);
        else EditorGUI.LabelField(position, "ERROR: Can't find 'InputActionAsset'");

        EditorGUI.EndProperty();
    }

    private void DrawStringAsPopup(in Rect position, SerializedProperty property)
    {
        property = property.FindPropertyRelative("guid");
        string prop_value = property.stringValue;

        int index = -1;
        for (int i = 0, i_max = m_guids.Length; i < i_max; i++)
        {
            if (m_guids[i] != prop_value) continue;
            index = i;
            break;
        }

        index = EditorGUI.Popup(position, index, m_labels);
        property.stringValue = (index > -1) ? m_guids[index] : string.Empty;
    }
};
