using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cache
{
    public static Dictionary<Collider2D, Tile> characters = new Dictionary<Collider2D, Tile>();

    public static Tile GetTile(Collider2D collider)
    {
        if (!characters.ContainsKey(collider))
        {
            characters.Add(collider, collider.GetComponent<Tile>());
        }

        return characters[collider];
    }

}
