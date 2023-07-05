using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelSignature))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelSignature lvl = (LevelSignature)target;

        string lvlNumber = "0";
        for (int i = 0; i < lvl.levelName.Length - 5; i++)
        {
            if (lvl.levelName.Substring(i, 5).Equals("Level"))
            {
                lvlNumber = lvl.levelName.Substring(i + 6, lvl.levelName.Length - i - 6);
                break;
            }
        }

        try
        {
            lvl.levelNumber = Int32.Parse(lvlNumber);
        }
        catch (Exception) { }

        if (GUILayout.Button("UP"))
        {
            if (lvl.pathsUnlocked.Count == 0)
                lvl.pathsUnlocked.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.up);
        }
        if (GUILayout.Button("DOWN"))
        {
            if (lvl.pathsUnlocked.Count == 0)
                lvl.pathsUnlocked.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.down);
        }
        if (GUILayout.Button("RIGHT"))
        {
            if (lvl.pathsUnlocked.Count == 0)
                lvl.pathsUnlocked.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.right);
        }
        if (GUILayout.Button("LEFT"))
        {
            if (lvl.pathsUnlocked.Count == 0)
                lvl.pathsUnlocked.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.left);
        }
        EditorUtility.SetDirty(target);
    }
}
#endif
