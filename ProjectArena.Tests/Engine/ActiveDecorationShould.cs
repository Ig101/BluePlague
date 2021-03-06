﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Engine.Objects;
using ProjectArena.Tests.Engine.Helpers;

namespace ProjectArena.Tests.Engine
{
    [TestFixture]
    public class ActiveDecorationShould : BasicEngineTester
    {
        private ActiveDecoration _decoration;

        [SetUp]
        public void Prepare()
        {
            SyncMessages = new List<ISyncEventArgs>();
            Scene = SceneSamples.CreateSimpleScene(this.EventHandler, false);
            Scene.Actors.Find(x => SceneHelper.GetOrderByGuid(x.ExternalId) == 1).Kill();
            Scene.EndTurn();
            _decoration = Scene.CreateDecoration(Scene.Players.First(), "test_decoration", Scene.Tiles[4][4], null, null, null, null, null);
            SyncMessages.Clear();
        }

        [Test]
        public void StartState()
        {
            Assert.That(Scene.Tiles[4][4].TempObject, Is.EqualTo(_decoration), "Position of decoration");
            Assert.That(_decoration.TempTile, Is.EqualTo(Scene.Tiles[4][4]), "Position of decoration");
            Assert.That(_decoration.DamageModel.Health, Is.EqualTo(100), "Health");
        }

        [Test]
        public void DecorationCast()
        {
            Assert.That(Scene.ActorWait(), Is.True, "Actor first turn");
            Assert.That(SyncMessages.Count, Is.EqualTo(1), "Amount of syncMessages first turn");
            Assert.That(_decoration.DamageModel.Health, Is.EqualTo(100), "Health first turn");
            Assert.That(Scene.ActorWait(), Is.True, "Actor second turn");
            Assert.That(SyncMessages.Count, Is.EqualTo(4), "Amount of syncMessages second turn");
            Assert.That(_decoration.DamageModel.Health, Is.EqualTo(90), "Health second turn");
            Assert.That(SyncMessages[2].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.Decoration), "Decoration action");
            Assert.That(Scene.TempTileObject, Is.EqualTo(Scene.Actors[0]), "Temp tile object at last");
        }

        [Test]
        public void DecorationDeath()
        {
            int i = 0;
            while (_decoration.DamageModel.Health > 0 && i < 500)
            {
                i++;
                Scene.ActorWait();
            }

            Assert.That(i > 400, Is.False, "Cycle error");
            Assert.That(SyncMessages[^2].Action, Is.EqualTo(ProjectArena.Engine.Helpers.SceneAction.Decoration), "Decoration action");
            Assert.That(SyncMessages[^2].SyncInfo.DeletedDecorations.Count(), Is.EqualTo(1), "Decoration killed");
            Assert.That(SyncMessages[^2].SyncInfo.DeletedDecorations.ToArray()[0], Is.EqualTo(_decoration.Id), "Decoration killed id");
        }
    }
}
