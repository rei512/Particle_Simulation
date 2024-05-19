using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
[CustomEditor(typeof(main_script))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var m = target as main_script;
        base.OnInspectorGUI();
        int x = 0;

        EditorGUI.BeginChangeCheck();
        {
            x = EditorGUILayout.IntField("grid", m.grid);
        }
        if (EditorGUI.EndChangeCheck())
            m.grid = x;
    }
}