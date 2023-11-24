using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathTile : Tile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position + Vector3Int.up, tilemap);
        base.RefreshTile(position + Vector3Int.right, tilemap);
        base.RefreshTile(position + Vector3Int.left, tilemap);
        base.RefreshTile(position + Vector3Int.down, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = GetSprite(position, tilemap);
    }

    public Sprite[] lockedSpritesURDL;
    public Sprite[] unlockedSpritesURDL;
    public Sprite[] specialCaseUnlockedTs;
    public bool unlocked;
    public Sprite GetSprite(Vector3Int position, ITilemap tilemap)
    {
        bool pathUp = tilemap.GetTile(position + Vector3Int.up) != null;
        bool pathRight = tilemap.GetTile(position + Vector3Int.right) != null;
        bool pathLeft = tilemap.GetTile(position + Vector3Int.left) != null;
        bool pathDown = tilemap.GetTile(position + Vector3Int.down) != null;

        int index = 0;
        if (pathLeft)
            index += 1;
        if (pathDown)
            index += 2;
        if (pathRight)
            index += 4;
        if (pathUp)
            index += 8;

        // T-Sprite Special Cases
        if (unlocked && index == 14)
        {
            bool unlockedPathUp = true, unlockedPathDown = true;

            try
            {
                unlockedPathUp = false;
                // Check if sprite above is unlocked
                Sprite spriteUp = tilemap.GetSprite(position + Vector3Int.up);
                int i = 0;
                foreach (var sprite in unlockedSpritesURDL)
                {
                    if (sprite.Equals(spriteUp))
                    {
                        unlockedPathUp = true;
                    }
                    i++;
                }

                // Check if sprite below is unlocked
                Sprite spriteDown = tilemap.GetSprite(position + Vector3Int.down);
                unlockedPathDown = false;
                i = 0;
                foreach (var sprite in unlockedSpritesURDL)
                {
                    if (sprite.Equals(spriteDown))
                    {
                        unlockedPathDown = true;
                    }
                    i++;
                }
            }
            catch (System.Exception e)
            {
                unlockedPathUp = true;
                unlockedPathDown = true;
                Debug.LogWarning("Special T Path Loaded Incorrectly: " + e);
            }

            if (unlockedPathDown && unlockedPathUp)
                return unlockedSpritesURDL[index];
            else if (unlockedPathDown)
                return specialCaseUnlockedTs[0];
            else if (unlockedPathUp)
                return specialCaseUnlockedTs[1];
        }

        // Regular Cases
        if (unlocked)
            return unlockedSpritesURDL[index];
        else
            return lockedSpritesURDL[index];
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Custom Tiles/Path Tile")]
    public static void CreateVariableTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Variable Tile", "New Variable Tile", "Asset", "Save Variable Tile", "Assets");
        if (path == "") return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PathTile>(), path);
    }
#endif
}

