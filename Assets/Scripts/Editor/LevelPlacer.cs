using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The <b>placement operations</b> behind the level authoring window, kept separate from the window's
/// UI (Single Responsibility): snap-and-place a prefab, and erase the object under the cursor. Both
/// register Undo so every author action is a single Ctrl+Z, and both go through
/// <see cref="PrefabUtility"/> so placed objects stay connected prefab instances (edits to the prefab
/// still propagate).
/// </summary>
public static class LevelPlacer
{
    /// <summary>Snap a world point to a grid of <paramref name="grid"/> units (0 or less = no snap).</summary>
    public static Vector3 Snap(Vector3 world, float grid)
    {
        if (grid <= 0f)
            return new Vector3(world.x, world.y, 0f);
        return new Vector3(Mathf.Round(world.x / grid) * grid, Mathf.Round(world.y / grid) * grid, 0f);
    }

    /// <summary>Instantiate <paramref name="prefab"/> at the snapped point under <paramref name="parent"/>.</summary>
    public static GameObject Place(GameObject prefab, Vector3 worldPoint, float grid, Transform parent)
    {
        if (prefab == null)
            return null;

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        instance.transform.position = Snap(worldPoint, grid);
        Undo.RegisterCreatedObjectUndo(instance, "Place " + prefab.name);
        return instance;
    }

    /// <summary>Delete the whole prefab instance the clicked object belongs to.</summary>
    public static void Erase(GameObject picked)
    {
        if (picked == null)
            return;

        GameObject target = PrefabUtility.GetOutermostPrefabInstanceRoot(picked);
        Undo.DestroyObjectImmediate(target != null ? target : picked);
    }

    /// <summary>
    /// The front-most world (non-UI) object under the cursor. UI lives on a Canvas that usually sits
    /// in front of the scene, so any Canvas hit is skipped and the search continues behind it, leaving
    /// only real level objects erasable.
    /// </summary>
    public static GameObject PickWorldObject(Vector2 mousePosition)
    {
        var ignored = new List<GameObject>();
        for (int guard = 0; guard < 10; guard++) // cap in case of many stacked UI layers
        {
            GameObject picked = HandleUtility.PickGameObject(mousePosition, false, ignored.ToArray());
            if (picked == null)
                return null;

            Canvas canvas = picked.GetComponentInParent<Canvas>();
            if (canvas == null)
                return picked; // a world object — safe to erase

            foreach (Transform child in canvas.rootCanvas.GetComponentsInChildren<Transform>(true))
                ignored.Add(child.gameObject);
        }

        return null;
    }
}
