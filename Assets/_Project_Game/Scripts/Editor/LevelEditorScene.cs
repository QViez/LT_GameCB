using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class LevelEditorScene
{
    static LevelEditorScene()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        LevelEditorWindow[] windows = Resources.FindObjectsOfTypeAll<LevelEditorWindow>();
        if (windows == null || windows.Length == 0)
            return;

        LevelEditorWindow window = windows[0];
        if (window == null)
            return;

        LevelEditorWindow.EditMode mode = window.CurrentMode;
        GameObject prefab = window.GetSelectedPrefab();

        if (mode == LevelEditorWindow.EditMode.Place && prefab == null)
            return;

        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controlID);
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Vector3 pos = Vector3.zero;
        bool hasHit = false;
        GameObject hitObject = null;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitObject = hit.collider.gameObject;
            if (mode == LevelEditorWindow.EditMode.Place)
            {
                pos = hit.point + hit.normal * 0.25f;
            }
            else
            {
                pos = hitObject.transform.position;
            }
            hasHit = true;
        }
        else if (mode == LevelEditorWindow.EditMode.Place)
        {
               Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float enter))
            {
                pos = ray.GetPoint(enter);
                hasHit = true;
            }
        }

        if (!hasHit) return;

        float snapSize = 0.5f; 

        if (mode == LevelEditorWindow.EditMode.Place)
        {
            float stepXZ = snapSize / 2f; 

            Vector3 rawPos = hit.point + new Vector3(
                hit.normal.x * (stepXZ / 2f),
                hit.normal.y * (snapSize / 2f),
                hit.normal.z * (stepXZ / 2f)
            );

            pos.x = Mathf.Round(rawPos.x / stepXZ) * stepXZ;
            pos.z = Mathf.Round(rawPos.z / stepXZ) * stepXZ;

            pos.y = Mathf.Round(rawPos.y / snapSize) * snapSize;
        }

        if (mode == LevelEditorWindow.EditMode.Place)
        {
            Handles.color = Color.green;
            Handles.DrawWireCube(pos, Vector3.one * snapSize);
        }
        else if (mode == LevelEditorWindow.EditMode.Erase)
        {
            Handles.color = Color.red;
            Handles.DrawWireCube(pos, Vector3.one * snapSize);
        }
        else if (mode == LevelEditorWindow.EditMode.Rotate)
        {
            Handles.color = Color.cyan;
            Handles.DrawWireCube(pos, Vector3.one * snapSize);
        }
        else if (mode == LevelEditorWindow.EditMode.RotateVertical)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireCube(pos, Vector3.one * snapSize);
        }

        if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
        {
            sceneView.Repaint();
        }

        if (e.type == EventType.MouseDown &&
            e.button == 0 &&
            !e.alt)
        {
            if (mode == LevelEditorWindow.EditMode.Place)
            {
                GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (go != null)
                {
                    go.transform.position = pos;
                    Undo.RegisterCreatedObjectUndo(go, "Place Block");
                }
            }
            else if (mode == LevelEditorWindow.EditMode.Erase && hitObject != null)
            {
                Undo.DestroyObjectImmediate(hitObject);
            }
            else if (mode == LevelEditorWindow.EditMode.Rotate && hitObject != null)
            {
                Undo.RecordObject(hitObject.transform, "Rotate Block");
                hitObject.transform.root.Rotate(0, 45, 0);
            }
            else if (mode == LevelEditorWindow.EditMode.RotateVertical && hitObject != null)
            {
                Undo.RecordObject(hitObject.transform, "Rotate Block Vertically");
                hitObject.transform.root.Rotate(45, 0, 0);
            }
            e.Use();
        }
    }
}