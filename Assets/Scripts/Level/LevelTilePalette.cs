using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A named set of level prefabs the authoring tools place from — the single source of truth shared
/// by the palette placer (<c>LevelPlacerWindow</c>) and the Tiled importer (<c>TiledLevelImporter</c>),
/// so both stay in sync. It is plain data (a <see cref="ScriptableObject"/> asset): each
/// <see cref="Entry"/> pairs an optional numeric id (for Tiled maps, where 0 means "empty") with the
/// prefab and a display label. Referencing prefabs directly means no Resources folder and no string
/// lookups at author time.
/// </summary>
[CreateAssetMenu(menuName = "Level/Tile Palette", fileName = "LevelTilePalette")]
public class LevelTilePalette : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        [Tooltip("Optional id used by the Tiled importer (the tile's gid). Ignore it for hand placing.")]
        public int id;
        public GameObject prefab;
        [Tooltip("Optional label shown in the palette; falls back to the prefab name.")]
        public string label;

        public string DisplayName => !string.IsNullOrEmpty(label) ? label
            : (prefab != null ? prefab.name : "(missing prefab)");
    }

    [SerializeField] private List<Entry> tiles = new List<Entry>();

    public IReadOnlyList<Entry> Tiles => tiles;

    /// <summary>The prefab for a Tiled tile id, or null if this palette has no entry for it.</summary>
    public GameObject GetPrefab(int id)
    {
        foreach (var entry in tiles)
            if (entry.id == id)
                return entry.prefab;
        return null;
    }
}
