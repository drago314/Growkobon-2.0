using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelSignature))]
public class MapEditor : Editor
{
    bool instantiated = false;

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

        if (!instantiated)
        {
            instantiated = true;
            lvl.pathsUnlocked = new List<Vector2Int>();
            lvl.pathsUnlocked.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
        }

        if (GUILayout.Button("UP"))
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.up);
        if (GUILayout.Button("DOWN"))
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.down);
        if (GUILayout.Button("RIGHT"))
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.right);
        if (GUILayout.Button("LEFT"))
            lvl.pathsUnlocked.Add(lvl.pathsUnlocked[lvl.pathsUnlocked.Count - 1] + Vector2Int.left);
    }
}
#endif
