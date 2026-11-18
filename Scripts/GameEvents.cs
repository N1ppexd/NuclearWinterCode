using System;
using UnityEngine;

namespace NuclearWinter
{
    public static class GameEvents
    {

        public static Action<CustomTile> onBaseTilesSet;
        public static Action<CustomTile[]> onBaseTileAdded;

        public static Action<int> onDiceThrown;

        public static Action onBaseUpdated;

        public static Action<bool> onGenerated;
        public static Action<CustomGrid> onGeneratedGrid;
        public static Action<Vector3> onFocused;
        public static Action onRoundRestarted;
        public static Action<Player> onPlayerSpawned;
        public static Action onPlayerMoved;

        public static Action<PlayerAttributes> onStatisticsInstantiated;
        public static Action<StatsSO, int> onResourcesUsed;

        public static Action onPlayerGoToTerrain;

        /// <summary>
        /// This event is used to initialize the base...
        /// </summary>
        /// <param name="tiles"></param>
        public static void SetBaseTiles(CustomTile tile) => onBaseTilesSet?.Invoke(tile);

        /// <summary>
        /// This event is used to enlarge the base...
        /// </summary>
        /// <param name="tiles"></param>
        public static void AddBaseTiles(CustomTile[] tiles) => onBaseTileAdded?.Invoke(tiles);

        
        public static void GoToTerrain() => onPlayerGoToTerrain?.Invoke();

        public static void PlayerMove() => onPlayerMoved?.Invoke();


        public static void UpdateBase() => onBaseUpdated?.Invoke();


        //event when the dice is thrown...
        public static void DiceResult(int result) => onDiceThrown?.Invoke(result);


        public static void Generated(bool generated) => onGenerated?.Invoke(generated);

        public static void GeneratedGrid(CustomGrid currentGrid) => onGeneratedGrid?.Invoke(currentGrid);

        public static void FocusOnPoint(Vector3 point) => onFocused?.Invoke(point);

        public static void RestartRound() => onRoundRestarted?.Invoke();

        public static void SpawnPlayer(Player player) => onPlayerSpawned?.Invoke(player);

        //happens when the player stats get loaded to the game.
        public static void InstantiateStatistics(PlayerAttributes attributes) => onStatisticsInstantiated?.Invoke(attributes);


        //when the player uses resources
        public static void UseResources(StatsSO resource, int amount) => onResourcesUsed?.Invoke(resource, amount);

    }
}
