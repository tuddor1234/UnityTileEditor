using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class LevelBuilder : MonoBehaviour
{

    [Range(1,100)]
    public int Width = 32;
    [Range(1, 100)]
    public int Height = 32;
  
    float CellSize = 1;


    public Color GridColor = Color.green;
    public bool bGridVisible = false;
    [HideInInspector]
    public List<Sprite> AvaiableTiles = new List<Sprite>();

    Dictionary<GameObject, Vector3> m_LevelTiles = new Dictionary<GameObject, Vector3>();
    int m_SelectedTileIndex = 0;
    [HideInInspector]
    [SerializeField]
    private GameObject m_TilePrefab;


    public bool bIsEditing = true;

    

    void OnDrawGizmos()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        GridColor.a = bGridVisible ? 1f : 0;
        Gizmos.color = GridColor;

        int xpos =(int) transform.position.x;
        int ypos =(int) transform.position.y;

        for(int y =  ypos + (int) -Height / 2; y <= Height / 2 + ypos; y++)
        {
            Gizmos.DrawLine(new Vector3(-Width / 2 * CellSize + CellSize / 2 + xpos, y * CellSize + CellSize / 2, 0.0f),
                            new Vector3(Width / 2 * CellSize + CellSize / 2 + xpos, y * CellSize + CellSize / 2, 0.0f));
        }

        for (float x = xpos + (int)-Width / 2; x <= xpos + Width / 2; x++)
        {
            Gizmos.DrawLine(new Vector3(x * CellSize + CellSize / 2, -Height / 2 * CellSize + CellSize / 2 + ypos, 0.0f),
                           new Vector3(x * CellSize + CellSize / 2, Height / 2 * CellSize + CellSize / 2 + ypos, 0.0f));
        }
    }

    public void AddTile(Vector3 position)
    {
        foreach (var tile in m_LevelTiles)
        {
            if (tile.Value == position)
            {
                return;
            }
        }
         
        GameObject newTile = Instantiate(m_TilePrefab);
        newTile.GetComponent<SpriteRenderer>().sprite = AvaiableTiles[m_SelectedTileIndex];
        newTile.transform.position = position;
        newTile.transform.SetParent(transform);
        m_LevelTiles.Add(newTile, position);   

    }

    public void RemoveTileAt(Vector3 position)
    {
        foreach (var tile in m_LevelTiles)
        {
            if (tile.Key.transform.position == position)
            {
                DestroyImmediate(tile.Key);
                m_LevelTiles.Remove(tile.Key);
                return;
            }
        }
    }

    public void ResetLevel()
    {
        if( PrefabUtility.GetPrefabInstanceStatus(gameObject) != PrefabInstanceStatus.NotAPrefab)
        {
            PrefabUtility.UnpackPrefabInstance(gameObject,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
        }
        // Remove all children
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (!child.gameObject.Equals(gameObject))
            {
                DestroyImmediate(child.gameObject);

            }
        }
        // Clear m_LevelTiles list
        m_LevelTiles.Clear();
    }


    public void SelectTile(int index)
    {
        m_SelectedTileIndex = index;
        // In case of array overflow default to 0
        if (index >= AvaiableTiles.Count)
            m_SelectedTileIndex = 0;
    }
    public void Save()
    {
        var localPath = "Assets/TileEditor/Resources/Levels/" + gameObject.name + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);  
        PrefabUtility.SaveAsPrefabAsset(gameObject, localPath);   
    }

    public void OnValidate()
    {
        m_LevelTiles.Clear();
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
             var child = gameObject.transform.GetChild(i);
             m_LevelTiles.Add(child.gameObject, child.transform.position);
        }
    }

    public void Update()
    {
        if(!bIsEditing) 
            return;

        foreach (var tile in m_LevelTiles)
        {
             tile.Key.transform.position = tile.Value;         
        }
    }

    private void OnDestroy()
    {
        m_LevelTiles.Clear();
    }

    
}
