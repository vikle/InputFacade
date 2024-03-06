using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputObserver))]
public sealed class InputObserverEditor : Editor
{
    public new InputObserver target;
    bool[] m_foldouts;
    static InputActionWrapper s_actionWrapper;
    static InputActionData s_actionData;

    void OnEnable()
    {
        if (Application.isPlaying == false) return;
        target = (InputObserver)base.target;
        m_foldouts = new bool[target.asset.Count()];
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying == false) return;

        GUILayout.Space(5f);

        int action_index = 0;
        var maps = target.Maps;
        for (int i = 0, i_max = maps.Length; i < i_max; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();

            var map = maps[i];
            GUI.color = map.Enabled ? Color.white : Color.gray;

            var actions = map.Actions;
            for (int j = 0, j_max = actions.Length; j < j_max; j++)
            {
                var act = actions[j];
                s_actionWrapper = act;
                s_actionData = act.Data;

                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(16f));
                string f_lbl = $"{map.Map.name} / {act.Action.name} {(s_actionData.IsActive ? "•" : "")}";
                bool fld_is_open = m_foldouts[action_index] = EditorGUI.Foldout(rect, m_foldouts[action_index], f_lbl, true);

                if (fld_is_open)
                {
                    DrawSpacedField(() => s_actionWrapper.Enabled = EditorGUILayout.ToggleLeft("Enabled", s_actionWrapper.Enabled));
                    DrawSpacedField(() => EditorGUILayout.LabelField($"Phase={s_actionWrapper.Phase}"));

                    switch (act.ControlType)
                    {
                        case EControlType.Axis:
                            DrawSpacedField(() => EditorGUILayout.LabelField($"Value={s_actionData.AxisValue:f1}"));
                            break;
                        case EControlType.Vector2:
                            DrawSpacedField(() => EditorGUILayout.LabelField($"Value={s_actionData.Vector2Value}"));
                            break;
                        default: break;
                    }
                }
                else
                {
                    DrawSpacedField(delegate
                    {
                    });
                }

                action_index++;
            }

            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        Repaint();
    }

    private static void DrawSpacedField(Action action)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20f);
        action.Invoke();
        EditorGUILayout.EndHorizontal();
    }
};
