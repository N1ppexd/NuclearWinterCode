using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public static class RevealTile
    {
        public static void RevealHex(CustomTile tile, CustomGrid grid, int revealRange)
        {
            List<CustomTile> tilesToReveal = HexagonOperations.GetTilesInRange(grid, tile, revealRange);


            for (int i = 0; i < tilesToReveal.Count; i++)
            {
                HexTile hexTile = tilesToReveal[i].HexTile;

                if (!hexTile.IsVisible)
                {
                    hexTile.Show();
                }
            }
        }

        public static void RevealPath(List<CustomTile> tiles, CustomGrid grid, int revealRange)
        {
            for(int i = 0; i < tiles.Count; i++)
            {
                RevealHex(tiles[i], grid, revealRange);
            }
        }
    }
}
