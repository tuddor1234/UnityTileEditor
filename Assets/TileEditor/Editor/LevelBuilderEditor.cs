using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
    LevelBuilder grid = null;
     
    public void OnEnable()
    {
        if(target.GetType() == typeof(LevelBuilder))
        {
            grid = (LevelBuilder)target;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Save Level"))
        {
            grid.Save();
        }

        if (GUILayout.Button("Reset Level"))
        {
            grid.ResetLevel();
        }

        GUILayout.EndHorizontal();
    }

    void OnSceneGUI()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (!grid.bIsEditing)
        {
            return; 
        }

        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                GUIUtility.hotControl = controlID;
                //Debug.Log("MOUSE DOWN");
                e.Use();
                break;
            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                //Debug.Log("MOUSE UP");
                OnMouseEventHandler(e.mousePosition, e.button);
                e.Use();
                break;
            case EventType.MouseDrag:
                GUIUtility.hotControl = controlID;
                //Debug.Log("MOUSE DRAG");
                OnMouseEventHandler(e.mousePosition, e.button);
                e.Use();
                break;
        }
    }

    void OnMouseEventHandler(Vector2 mpos, int button)
    {
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(mpos.x, -mpos.y + Camera.current.pixelHeight));
        Vector3 mousePos = ray.origin;
        // The spawn position should be snapped to the grid

        int x = (int)(mousePos.x);
        int y = (int)(mousePos.y);
        Vector3Int snappedPosition = new Vector3Int(x, y, 0);

        // Add element: left click, remove element, right click
        if (x <= -grid.Width / 2 || x > grid.Width / 2 || y <= -grid.Height / 2 || y > grid.Height / 2)
        {
            return;
        }
        // Add element: left click, remove element, right click
        if (button == 0)
        {
            grid.AddTile(snappedPosition);
        }
        else if (button == 1)
        {
            grid.RemoveTileAt(snappedPosition);
        }
    }


}
