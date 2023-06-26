using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FenceTile : Tile
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

    public Sprite[] m_SpritesURDL;
    public Sprite GetSprite(Vector3Int position, ITilemap tilemap)
    {
        bool fenceUp = tilemap.GetTile(position + Vector3Int.up) != null;
        bool fenceRight = tilemap.GetTile(position + Vector3Int.right) != null;
        bool fenceLeft = tilemap.GetTile(position + Vector3Int.left) != null;
        bool fenceDown = tilemap.GetTile(position + Vector3Int.down) != null;

        int index = 0;
        if (fenceLeft)
            index += 1;
        if (fenceDown)
            index += 2;
        if (fenceRight)
            index += 4;
        if (fenceUp)
            index += 8;

        return m_SpritesURDL[index];
    }

    [MenuItem("Assets/Create/2D/Custom Tiles/Variable Tile")]
    public static void CreateVariableTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Variable Tile", "New Variable Tile", "Asset", "Save Variable Tile", "Assets");
        if (path == "") return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FenceTile>(), path);
    }
}
