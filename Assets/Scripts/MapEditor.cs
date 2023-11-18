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
        LevelAnimator lvlAnimator = lvl.gameObject.GetComponent<LevelAnimator>();

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
            lvlAnimator.SetLevelNumber(lvl.levelNumber);
        }
        catch (Exception) { }

        foreach (KeyValuePair<string, List<Vector2Int>> pair in lvl.exitToPathsUnlocked)
        {
            if (GUILayout.Button("UP " + pair.Key))
            {
                if (pair.Value.Count == 0)
                    pair.Value.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
                pair.Value.Add(pair.Value[pair.Value.Count - 1] + Vector2Int.up);
            }
            if (GUILayout.Button("DOWN " + pair.Key))
            {
                if (pair.Value.Count == 0)
                    pair.Value.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
                pair.Value.Add(pair.Value[pair.Value.Count - 1] + Vector2Int.down);
            }
            if (GUILayout.Button("RIGHT " + pair.Key))
            {
                if (pair.Value.Count == 0)
                    pair.Value.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
                pair.Value.Add(pair.Value[pair.Value.Count - 1] + Vector2Int.right);
            }
            if (GUILayout.Button("LEFT " + pair.Key))
            {
                if (pair.Value.Count == 0)
                    pair.Value.Add(new Vector2Int((int)lvl.transform.position.x, (int)lvl.transform.position.y));
                pair.Value.Add(pair.Value[pair.Value.Count - 1] + Vector2Int.left);
            }
        }
        EditorUtility.SetDirty(target);
    }
}
#endif
