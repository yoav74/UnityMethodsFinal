using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor tool (<b>Tools ▸ Level Placer</b>) for building the levels by hand: pick a prefab from a
/// <see cref="LevelTilePalette"/>, then <b>right-click in the Scene</b> to drop it on the grid, or
/// switch to Erase and right-click to remove the object under the cursor. Placed tiles nest under a
/// shared parent so the hierarchy stays tidy, a wire outline previews where the next tile lands, and
/// every action is a single Undo. It only handles the UI + Scene input; the actual place/erase work
/// lives in <see cref="LevelPlacer"/> and the prefab set in the palette asset (separation of concerns).
/// </summary>
public class LevelPlacerWindow : EditorWindow
{
    private enum Mode { Place, Erase }

    private const string DefaultParentName = "Level";

    private LevelTilePalette _palette;
    private Mode _mode = Mode.Place;
    private int _selected;
    private float _grid = 1f;
    private Transform _parent;
    private Vector2 _scroll;
    private Vector3 _cursorWorld;

    [MenuItem("Tools/Level Placer")]
    public static void ShowWindow()
    {
        GetWindow<LevelPlacerWindow>("Level Placer");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        if (_palette == null)
            _palette = FindPalette();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _palette = (LevelTilePalette)EditorGUILayout.ObjectField("Tile Palette", _palette, typeof(LevelTilePalette), false);
        _mode = (Mode)GUILayout.Toolbar((int)_mode, new[] { "Place", "Erase" });
        _grid = EditorGUILayout.FloatField("Grid Size", _grid);
        _parent = (Transform)EditorGUILayout.ObjectField("Parent", _parent, typeof(Transform), true);

        EditorGUILayout.Space();
        if (_mode == Mode.Erase)
        {
            EditorGUILayout.HelpBox("Right-click an object in the Scene to delete it (whole prefab instance).", MessageType.Info);
            return;
        }

        if (_palette == null || _palette.Tiles.Count == 0)
        {
            EditorGUILayout.HelpBox("Assign a Tile Palette with at least one prefab (Create ▸ Level ▸ Tile Palette).", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);
        DrawPaletteGrid();
        EditorGUILayout.HelpBox($"Right-click in the Scene to place \"{SelectedName()}\". Tiles nest under \"{ParentName()}\".", MessageType.Info);
    }

    // A thumbnail grid of the palette prefabs; the selected one is highlighted.
    private void DrawPaletteGrid()
    {
        const int cols = 4;
        const int cell = 78;   // cell width
        const int icon = 60;   // preview square

        var caption = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.UpperCenter,
            wordWrap = true,
            fixedWidth = cell
        };

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(210));
        for (int i = 0; i < _palette.Tiles.Count; i++)
        {
            if (i % cols == 0)
                EditorGUILayout.BeginHorizontal();

            var entry = _palette.Tiles[i];
            Texture preview = entry.prefab != null ? AssetPreview.GetAssetPreview(entry.prefab) : null;

            // Each cell stacks the icon over a wrapped label, so neither is squished.
            EditorGUILayout.BeginVertical(GUILayout.Width(cell));
            bool on = _selected == i;
            var iconContent = preview != null ? new GUIContent(preview) : new GUIContent(entry.DisplayName);
            bool now = GUILayout.Toggle(on, iconContent, GUI.skin.button, GUILayout.Width(cell), GUILayout.Height(icon));
            if (now && !on)
                _selected = i;
            GUILayout.Label(entry.DisplayName, caption);
            EditorGUILayout.EndVertical();

            if (i % cols == cols - 1 || i == _palette.Tiles.Count - 1)
                EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        _cursorWorld = LevelPlacer.Snap(HandleUtility.GUIPointToWorldRay(e.mousePosition).origin, _grid);

        // Preview outline of where the next tile lands (place mode only).
        if (_mode == Mode.Place && _palette != null && _palette.Tiles.Count > 0)
        {
            Handles.color = new Color(0.3f, 0.9f, 1f, 0.9f);
            float s = _grid > 0f ? _grid : 1f;
            Handles.DrawWireCube(_cursorWorld, new Vector3(s, s, 0.01f));
            if (e.type == EventType.MouseMove)
                sceneView.Repaint();
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            if (_mode == Mode.Place)
                PlaceUnderCursor(e.mousePosition);
            else
                LevelPlacer.Erase(LevelPlacer.PickWorldObject(e.mousePosition));

            e.Use(); // consume so it doesn't also open the context menu / change selection
        }
    }

    private void PlaceUnderCursor(Vector2 mousePosition)
    {
        GameObject prefab = SelectedPrefab();
        if (prefab == null)
            return;

        Vector3 world = HandleUtility.GUIPointToWorldRay(mousePosition).origin;
        Selection.activeGameObject = LevelPlacer.Place(prefab, world, _grid, ResolveParent());
    }

    // The user-assigned parent, or a shared "Level" container created once so tiles never sit loose.
    private Transform ResolveParent()
    {
        if (_parent != null)
            return _parent;

        GameObject container = GameObject.Find(DefaultParentName);
        if (container == null)
        {
            container = new GameObject(DefaultParentName);
            Undo.RegisterCreatedObjectUndo(container, "Create " + DefaultParentName);
        }

        _parent = container.transform;
        return _parent;
    }

    private string ParentName() => _parent != null ? _parent.name : DefaultParentName;

    private string SelectedName()
    {
        var prefab = SelectedPrefab();
        return prefab != null ? prefab.name : "(none)";
    }

    private GameObject SelectedPrefab()
    {
        if (_palette == null || _selected < 0 || _selected >= _palette.Tiles.Count)
            return null;
        return _palette.Tiles[_selected].prefab;
    }

    private static LevelTilePalette FindPalette()
    {
        string guid = AssetDatabase.FindAssets("t:LevelTilePalette").FirstOrDefault();
        return string.IsNullOrEmpty(guid) ? null
            : AssetDatabase.LoadAssetAtPath<LevelTilePalette>(AssetDatabase.GUIDToAssetPath(guid));
    }
}
