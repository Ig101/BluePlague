using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProjectArena.Content;
using ProjectArena.Domain.BattleService.Helpers;
using ProjectArena.Domain.BattleService.Models;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Domain.Registry;
using ProjectArena.Domain.Registry.Entities;
using ProjectArena.Domain.Registry.Helpers;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.EngineHelper;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Battle.Synchronization;

namespace ProjectArena.Domain.BattleService
{
    public class BattleService : IBattleService
    {
        private const int RandomModifier = 10000;
        private const int MaxExperience = 1000000;

        private readonly object _locker = new object();
        private readonly IServiceProvider _serviceProvider;
        private readonly Random _random;
        private IEnumerable<IScene> _scenes;
        private INativeManager _nativeManager;

        public BattleService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _scenes = new List<IScene>();
            _random = new Random();
        }

        private void SetupNewNativeManager()
        {
            var nativeManager = EngineHelper.CreateNativeManager();
            nativeManager.FillNatives();
            _nativeManager = nativeManager;
        }

        public void Init()
        {
            SetupNewNativeManager();
        }

        private void ApplyTalentAction(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs,
            TalentNode talent,
            IEnumerable<TalentNode> talents,
            ICollection<int> appliedTalents)
        {
            if (talent.Prerequisites.Count() > 0)
            {
                var requiredTalents = talents.Where(x => !appliedTalents.Contains(x.Position) && talent.Prerequisites.Contains(x.Id));
                foreach (var requiredTalent in requiredTalents)
                {
                    ApplyTalentAction(ref attackSkill, ref actionPointsIncome, ref skills, ref startBuffs, requiredTalent, talents, appliedTalents);
                }
            }

            var action = (TalentActionDelegates.Action)Delegate.CreateDelegate(
                typeof(TalentActionDelegates.Action),
                typeof(TalentActionDelegates).GetMethod(talent.UniqueAction, BindingFlags.Public | BindingFlags.Static));
            action(ref attackSkill, ref actionPointsIncome, ref skills, ref startBuffs);
            appliedTalents.Add(talent.Position);
        }

        private IActor GenerateActor(Character character, IEnumerable<TalentNode> talents)
        {
            var strength = BattleHelper.DefaultStrength;
            var willpower = BattleHelper.DefaultWillpower;
            var constitution = BattleHelper.DefaultConstitution;
            var speed = BattleHelper.DefaultSpeed;
            var classPoints = new Dictionary<CharacterClass?, int>();
            foreach (var talent in talents)
            {
                strength += talent.StrengthModifier;
                willpower += talent.WillpowerModifier;
                constitution += talent.ConstitutionModifier;
                speed += talent.SpeedModifier;
                if (talent.Class.HasValue)
                {
                    classPoints.TryAdd(talent.Class, 0);
                    classPoints[talent.Class.Value] += talent.ClassPoints;
                }
            }

            var nativeId = "adventurer";
            if (classPoints.Count != 0)
            {
                var bestClass = classPoints.First();
                var draw = false;
                foreach (var points in classPoints)
                {
                    if (bestClass.Value < points.Value)
                    {
                        bestClass = points;
                        draw = false;
                    }

                    if (bestClass.Value == points.Value && bestClass.Key != points.Key)
                    {
                        draw = true;
                    }
                }

                if (!draw)
                {
                    switch (bestClass.Key)
                    {
                        case CharacterClass.Architect:
                            nativeId = "architect";
                            break;
                        case CharacterClass.Bloodletter:
                            nativeId = "bloodletter";
                            break;
                        case CharacterClass.Enchanter:
                            nativeId = "enchanter";
                            break;
                        case CharacterClass.Fighter:
                            nativeId = "fighter";
                            break;
                        case CharacterClass.Mistcaller:
                            nativeId = "mistcaller";
                            break;
                        case CharacterClass.Ranger:
                            nativeId = "ranger";
                            break;
                    }
                }
            }

            var attackingSkill = "slash";
            ICollection<string> skills = new List<string>() { "magicMissle" };
            var actionPointsIncome = 4;
            ICollection<string> startBuffs = new List<string>();

            var talentsWithUniqueAction = talents.Where(t => t.UniqueAction != null).ToList();
            var talentPositionsWithAppliedActions = new List<int>();
            foreach (var talent in talentsWithUniqueAction)
            {
                if (talentPositionsWithAppliedActions.Contains(talent.Position))
                {
                    continue;
                }

                ApplyTalentAction(ref attackingSkill, ref actionPointsIncome, ref skills, ref startBuffs, talent, talentsWithUniqueAction, talentPositionsWithAppliedActions);
            }

            return EngineHelper.CreateActorForGeneration(
                Guid.Parse(character.Id),
                nativeId,
                attackingSkill,
                strength,
                willpower,
                constitution,
                speed,
                skills,
                actionPointsIncome,
                startBuffs);
        }

        public async Task StartNewBattleAsync(SceneMode mode, IEnumerable<UserInQueue> users)
        {
            var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();
            var gameContext = _serviceProvider.GetRequiredService<GameContext>();
            var registryContext = _serviceProvider.GetRequiredService<RegistryContext>();

            var userIds = users.Select(x => x.UserId).ToList();
            var tempSceneId = Guid.NewGuid();

            var players = new List<IPlayer>(users.Count());

            var rosters = await gameContext.Rosters.GetAsync(x => userIds.Contains(x.UserId));
            var rosterIds = rosters.Select(x => x.Id).ToList();
            var characters = await gameContext.Characters.GetAsync(x => rosterIds.Contains(x.RosterId) && !x.Deleted);
            var allTalentIds = characters.SelectMany(x => x.ChosenTalents).ToList();
            var allTalents = await registryContext.TalentMap.GetAsync(x => allTalentIds.Contains(x.Position));

            foreach (string id in userIds)
            {
                var playerRosters = rosters.Where(x => x.UserId == id).ToList();
                var roster = playerRosters[_random.Next(playerRosters.Count * RandomModifier) % playerRosters.Count];
                var playerCharacters = characters.Where(x => x.RosterId == roster.Id).ToList();
                var playerActors = characters
                .Where(x => x.RosterId == roster.Id)
                .Select(x =>
                {
                    var talents = allTalents.Where(t => x.ChosenTalents.Contains(t.Position)).ToList();
                    return GenerateActor(x, talents);
                })
                .ToList();

                players.Add(EngineHelper.CreatePlayerForGeneration(Guid.NewGuid().ToString(), id, null, playerActors));
            }

            var scene = EngineHelper.CreateNewScene(tempSceneId, players, mode.Generator, _nativeManager, mode.VarManager, _random.Next(), 180, SynchronizationInfoEventHandler);
            lock (_locker)
            {
                _scenes = _scenes.Append(scene).ToHashSet();
            }
        }

        private void SynchronizationInfoEventHandler(object sender, ISyncEventArgs e)
        {
            var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();

            string actionName = BattleHelper.GetBattleActionMethodName(e.Action);

            var uniqueUsers = e.Scene.ShortPlayers
                .GroupBy(key => key.UserId, res => res, (key, res) => new
                {
                    UserId = key,
                    Players = res
                })
                .ToList();

            foreach (var userPlayers in uniqueUsers)
            {
                if (userPlayers.Players.All(x => x.Left))
                {
                    continue;
                }

                var user = battleHub.Clients.User(userPlayers.UserId);
                if (user != null)
                {
                    var synchronizer = BattleHelper.MapSynchronizer(e, userPlayers.UserId);
                    foreach (var player in userPlayers.Players)
                    {
                        if (BattleHelper.CalculateReward(ref synchronizer, e.Scene, player.Id))
                        {
                            Task.Run(async () => await PayRewardAsync(synchronizer.Reward, player.UserId));
                        }
                    }

                    battleHub.Clients.User(userPlayers.UserId).SendAsync(actionName, synchronizer);
                }
            }
        }

        public void EngineTimeProcessing(double seconds)
        {
            lock (_locker)
            {
                var newScenes = _scenes
                .Where(scene =>
                {
                    var sendMessage = scene.UpdateTime((float)seconds);
                    if (sendMessage)
                    {
                        var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();
                        var uniqueUsers = scene.ShortPlayers
                        .Where(x => !x.Left)
                        .Select(x => x.UserId)
                        .Distinct()
                        .ToList();
                        battleHub.Clients.Users(uniqueUsers).SendAsync("BattleIdle");
                    }

                    return scene.IsActive;
                })
                .ToHashSet();
            }
        }

        public bool IsUserInBattle(string userId)
        {
            return _scenes.Any(scene => scene.IsActive && scene.ShortPlayers.FirstOrDefault(x => x.UserId == userId && !x.Left) != null);
        }

        private async Task PayRewardAsync(RewardDto reward, string userId)
        {
            var gameContext = _serviceProvider.GetRequiredService<GameContext>();
            var rosters = await gameContext.Rosters.GetAsync(x => x.UserId == userId);
            if (rosters.Count() == 1)
            {
                var roster = rosters.First();
                gameContext.Rosters.Update(
                    x => x.Id == roster.Id,
                    Builders<Roster>.Update.Set(x => x.Experience, Math.Min(roster.Experience + reward.Experience, MaxExperience)));
                await gameContext.ApplyChangesAsync();
            }
        }

        public SynchronizerDto GetUserSynchronizationInfo(string userId, Guid? sceneId)
        {
            IScene scene;
            scene = _scenes
                .FirstOrDefault(scene => scene.IsActive &&
                    (sceneId == null || sceneId == scene.Id) &&
                    scene.ShortPlayers.FirstOrDefault(x => x.UserId == userId && !x.Left) != null);

            if (scene == null)
            {
                return null;
            }

            var player = scene.ShortPlayers.First(x => x.UserId == userId);
            var synchronizer = BattleHelper.GetFullSynchronizationData(scene, userId);
            BattleHelper.CalculateReward(ref synchronizer, scene, player.Id);
            return synchronizer;
        }

        public IEnumerable<SynchronizerDto> GetAllUserSynchronizationInfos(string userId)
        {
            return _scenes
                .Where(scene => scene.IsActive && scene.ShortPlayers.FirstOrDefault(x => x.UserId == userId && !x.Left) != null)
                .Select(scene =>
                {
                    var player = scene.ShortPlayers.First(x => x.UserId == userId);
                    var synchronizer = BattleHelper.GetFullSynchronizationData(scene, userId);
                    BattleHelper.CalculateReward(ref synchronizer, scene, player.Id);
                    return synchronizer;
                })
                .ToHashSet();
        }

        public IScene GetUserScene(string userId, Guid sceneId)
        {
            return _scenes.FirstOrDefault(scene => scene.IsActive && sceneId == scene.Id && scene.ShortPlayers.FirstOrDefault(x => x.UserId == userId && !x.Left) != null);
        }

        public bool LeaveScene(string userId, Guid sceneId)
        {
            var scene = GetUserScene(userId, sceneId);
            if (scene == null)
            {
                return false;
            }

            return scene.LeaveScene(userId);
        }
    }
}