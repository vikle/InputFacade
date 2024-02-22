using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InputActionData))]
public sealed class InputActionDataDrawer : PropertyDrawer
{
    string[] m_guids;
    string[] m_labels;
    const char k_Separator = '\uFF0F';
    const float k_ESpace = 18f;
    
    private void Init()
    {
        var observer = InputObserver.Get;
        var asset = (observer != null) ? observer.asset : null;
        if (asset == null) return;

        m_guids = asset.Select(a => $"{a.id}").ToArray();
        m_labels = asset.Select(a => $"{a.actionMap.name}{k_Separator}{a.name}").ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (m_guids == null) Init();

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        position.x -= k_ESpace;
        position.width += k_ESpace;

        if (m_guids == null)
        {
            EditorGUI.LabelField(position, label.text, "ERROR: Can't find 'InputActionAsset'.");
            EditorGUI.EndProperty();
            return;
        }

        DrawStringAsPopup(in position, property);

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
