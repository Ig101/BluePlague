using ProjectArena.Domain.Registry.EntityModels;

namespace ProjectArena.Domain.Registry.Migrations
{
    public class M0000_AddDefaultTilesAndActors : IContentMigration
    {
        public void Up(RegistryContext context)
        {
            context.ActorNatives.InsertOneAtomicallyAsync(new Entities.ActorNative()
            {
                Id = "apprentice",
                Tags = new[] { "organic", "intelligent" },
                DefaultZ = 0,
                Armor = new TagSynergy[0]
            }).Wait();
            context.TileNatives.InsertOneAtomicallyAsync(new Entities.TileNative()
            {
                Id = "grass",
                Tags = new string[0],
                Flat = false,
                DefaultHeight = 0,
                Unbearable = false,
                DefaultMod = 1,
                Actions = new string[0],
                OnStepActions = new string[0]
            }).Wait();
            context.SkillNatives.InsertOneAtomicallyAsync(new Entities.SkillNative()
            {
                Id = "slash",
                Tags = new[] { "target", "slash", "melee" },
                DefaultRange = 1,
                DefaultCost = 2,
                DefaultCd = 0,
                DefaultMod = 20,
                Actions = new[] { "DoDamageAttack" }
            }).Wait();
            context.SkillNatives.InsertOneAtomicallyAsync(new Entities.SkillNative()
            {
                Id = "explosion",
                Tags = new[] { "area", "magic", "fire", "range" },
                DefaultRange = 3,
                DefaultCost = 4,
                DefaultCd = 2,
                DefaultMod = 25,
                Actions = new[] { "DoSmallAoeDamageSkill", "DoSmallAoeOneTurnStun" }
            }).Wait();
            context.BuffNatives.InsertOneAtomicallyAsync(new Entities.BuffNative()
            {
                Id = "stun",
                Tags = new[] { "negative", "control" },
                Eternal = false,
                Repeatable = 1,
                SummarizeLength = false,
                DefaultDuration = 1,
                DefaultMod = 0,
                Actions = new string[0],
                Appliers = new[] { "Stun" },
                OnPurgeActions = new string[0]
            }).Wait();
        }
    }
}