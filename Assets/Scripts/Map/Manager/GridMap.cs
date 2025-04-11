using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    private Tilemap currentTilemap;
    [SerializeField] private MapDataSO mapDataSO;
    [SerializeField] private GridType gridType;

    void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            if (mapDataSO != null)
                mapDataSO.tileProperties.Clear();
        }    
    }

    void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();

            UpdateTileProperties();
            #if UNITY_EDITOR
            if (mapDataSO != null)
                EditorUtility.SetDirty(mapDataSO);
            #endif
        }
    }

    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this))
        {
            if (mapDataSO != null)
            {
                Vector3Int startPos = currentTilemap.cellBounds.min;
                Vector3Int endPos= currentTilemap.cellBounds.max;

                for (int i = startPos.x; i < endPos.x; i++)
                {
                    for (int j = startPos.y; j < endPos.y; j++)
                    {
                        TileBase tileBase = currentTilemap.GetTile(new Vector3Int(i, j, 0));

                        if (tileBase != null)
                        {
                            mapDataSO.tileProperties.Add(new TileProperty
                            {
                                tileCoordinate = new Vector2Int(i, j),
                                gridType = this.gridType,
                                boolTypeValue = true
                            });
                        }
                    }
                }
            }
        }
    }


}
