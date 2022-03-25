namespace Snek;

using UnityEngine;
public class Main : IModApi
{
    private string[] spawnables = {
        "animalBoar",
        "animalChicken",
        "animalCoyote",
        "animalDoe",
        "animalRabbit",
        // "animalSnake",
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
        var mobClasses = new HashSet<int>(spawnables.Select(EntityClass.FromString));
        ModEvents.EntityKilled.RegisterHandler((entity, killer) =>
        {
            if (!(killer is EntityAlive)) return;
            if (!(GameManager.IsDedicatedServer || SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)) return;
            if (!mobClasses.Contains((entity.entityClass))) return;
            var world = GameManager.Instance.World;
            var random = world.GetGameRandom();
            var days = GameUtils.WorldTimeToDays(world.GetWorldTime());
            var spawnCount = (int)Math.Round(Mathf.Lerp(0, 2, days / 7) + random.RandomRange(2, 4));
            for (int i = 0; i < spawnCount; ++i)
            {
                var snake = (EntityAlive)EntityFactory.CreateEntity(EntityClass.FromString("animalSnake"), entity.GetPosition(), new Vector3(0f, world.GetGameRandom().RandomFloat * 360f, 0f));
                world.SpawnEntityInWorld(snake);
                snake.SetAttackTarget((EntityAlive)killer, 60000);
                snake.SetSpawnerSource(EnumSpawnerSource.Unknown);
                snake.bIsChunkObserver = false;
            }
        });
    }
}
