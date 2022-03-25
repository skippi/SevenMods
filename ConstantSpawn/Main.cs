namespace ConstantSpawn;

using UnityEngine;
public class Main : IModApi
{
    private string[] spawnables = {
        "animalBoar",
        "animalChicken",
        "animalCoyote",
        "animalDoe",
        "animalRabbit",
        "animalSnake",
        "animalStag",
        "animalWolf",
        "animalZombieVulture",
        "npcBanditLeader",
        "npcBanditMelee",
        "npcBanditRanged",
        "zombieArlene",
        "zombieBiker",
        "zombieBoe",
        "zombieBurnt",
        "zombieBusinessMan",
        "zombieDarlene",
        "zombieDemolition",
        "zombieFatCop",
        "zombieFatHawaiian",
        "zombieFemaleFat",
        "zombieJanitor",
        "zombieJoe",
        "zombieLab",
        "zombieLumberjack",
        "zombieMaleHazmat",
        "zombieMarlene",
        "zombieMoe",
        "zombieMutated",
        "zombieNurse",
        "zombiePartyGirl",
        "zombieSkateboarder",
        "zombieSoldier",
        "zombieSpider",
        "zombieSteve",
        "zombieSteveCrawler",
        "zombieTomClark",
        "zombieUtilityWorker",
        "zombieYo",
    };

    private string[] bossMobs = {
        "animalBear",
        "animalBossGrace",
        "animalDireWolf",
        "animalMountainLion",
        "animalZombieBear",
        "animalZombieDog",
        "animalZombieVultureRadiated",
        "zombieArleneFeral",
        "zombieArleneRadiated",
        "zombieBikerFeral",
        "zombieBikerRadiated",
        "zombieBoeFeral",
        "zombieBoeRadiated",
        "zombieBurntFeral",
        "zombieBurntRadiated",
        "zombieBusinessManFeral",
        "zombieBusinessManRadiated",
        "zombieDarleneFeral",
        "zombieDarleneRadiated",
        "zombieFatCopFeral",
        "zombieFatCopRadiated",
        "zombieFatHawaiianFeral",
        "zombieFatHawaiianRadiated",
        "zombieFemaleFatFeral",
        "zombieFemaleFatRadiated",
        "zombieJanitorFeral",
        "zombieJanitorRadiated",
        "zombieJoeFeral",
        "zombieJoeRadiated",
        "zombieLabFeral",
        "zombieLabRadiated",
        "zombieLumberjackFeral",
        "zombieLumberjackRadiated",
        "zombieMaleHazmatFeral",
        "zombieMaleHazmatRadiated",
        "zombieMarleneFeral",
        "zombieMarleneRadiated",
        "zombieMoeFeral",
        "zombieMoeRadiated",
        "zombieMutatedFeral",
        "zombieMutatedRadiated",
        "zombieNurseFeral",
        "zombieNurseRadiated",
        "zombiePartyGirlFeral",
        "zombiePartyGirlRadiated",
        "zombieScreamer",
        "zombieScreamerFeral",
        "zombieScreamerRadiated",
        "zombieSkateboarderFeral",
        "zombieSkateboarderRadiated",
        "zombieSoldierFeral",
        "zombieSoldierRadiated",
        "zombieSpiderFeral",
        "zombieSpiderRadiated",
        "zombieSteveCrawlerFeral",
        "zombieSteveFeral",
        "zombieSteveRadiated",
        "zombieTomClarkFeral",
        "zombieTomClarkRadiated",
        "zombieUtilityWorkerFeral",
        "zombieUtilityWorkerRadiated",
        "zombieWightFeral",
        "zombieWightRadiated",
        "zombieYoFeral",
        "zombieYoRadiated",
    };

    public void InitMod(Mod mod)
    {
        var prevTriggerTime = 0ul;
        ModEvents.PlayerSpawnedInWorld.RegisterHandler((client, respawnType, pos) =>
        {
            if (!(GameManager.IsDedicatedServer || SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer))
            {
                return;
            }
            var players = GameManager.Instance.World.Players;
            if (!players.dict.ContainsKey(client.entityId))
            {
                return;
            }
            var player = players.dict[client.entityId];
            var world = GameManager.Instance.World;
            var spawnPos = world.Players.list
                .Where(p => p.IsAlive())
                .Select(p => p.GetPosition())
                .FirstOrDefault();
            if (spawnPos.Equals(default))
            {
                return;
            }
            player.Teleport(spawnPos);
        });
        ModEvents.GameUpdate.RegisterHandler(() =>
        {
            if (!(GameManager.IsDedicatedServer || SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer))
            {
                return;
            }
            try
            {
                var world = GameManager.Instance.World;
                if (world.GetWorldTime() < prevTriggerTime)
                {
                    prevTriggerTime = world.GetWorldTime();
                }
                var delayInSecs = Mathf.RoundToInt(Mathf.Lerp(60, 10, GameUtils.WorldTimeToDays(world.GetWorldTime()) / 7.0f));
                var delay = delayInSecs * GameStats.GetInt(EnumGameStats.TimeOfDayIncPerSec);
                if (world.GetWorldTime() - prevTriggerTime < (ulong)delay) return;
                var players = GameManager.Instance.World.Players.list;
                var alivePlayers = players
                    .Where(p => p.IsAlive())
                    .ToArray();
                if (alivePlayers.Length == 0) return;
                var target = players[world.GetGameRandom().RandomRange(0, alivePlayers.Length)];
                for (int i = 0; i < players.Count; ++i)
                {
                    Vector3 spawnPos;
                    if (!GetRandomSpawnPosition(target.GetPosition(), 1, 12, out spawnPos)) continue;
                    var className = spawnables[world.GetGameRandom().RandomRange(0, spawnables.Length)];
                    var entityId = EntityClass.FromString(className);
                    if (!EntityClass.list.ContainsKey(entityId)) continue;
                    var entity = EntityFactory.CreateEntity(entityId, spawnPos, new Vector3(0f, world.GetGameRandom().RandomFloat * 360f, 0f));
                    world.SpawnEntityInWorld(entity);
                    entity.SetSpawnerSource(EnumSpawnerSource.Unknown);
                    entity.bIsChunkObserver = false;
                }
                prevTriggerTime = world.GetWorldTime();
            }
            catch (Exception ex)
            {
                Log.Out(ex.ToString());
            }
        });
    }

    static bool GetRandomSpawnPosition(Vector3 _targetPos, int _minRange, int _maxRange, out Vector3 _position)
    {
        _position = Vector3.zero;
        int num = _maxRange - _minRange;
        if (num <= 0)
        {
            return false;
        }
        var world = GameManager.Instance.World;
        for (int i = 0; i < 10; i++)
        {
            Vector2 vector = Vector2.zero;
            do
            {
                vector = world.GetGameRandom().RandomInsideUnitCircle * (float)num;
            }
            while (vector.sqrMagnitude < 0.01f);
            vector += vector * ((float)_minRange / vector.magnitude);
            _position.x = _targetPos.x + vector.x;
            _position.y = _targetPos.y;
            _position.z = _targetPos.z + vector.y;
            Vector3i vector3i = World.worldToBlockPos(_position);
            Chunk chunk = (Chunk)world.GetChunkFromWorldPos(vector3i);
            if (chunk is null) continue;
            int x = World.toBlockXZ(vector3i.x);
            int z = World.toBlockXZ(vector3i.z);
            vector3i.y = (int)(chunk.GetHeight(x, z) + 1);
            _position.y = (float)vector3i.y;
            _position = vector3i.ToVector3() + new Vector3(0.5f, world.GetTerrainOffset(0, vector3i), 0.5f);
            return true;
        }
        _position = Vector3.zero;
        return false;
    }
}
