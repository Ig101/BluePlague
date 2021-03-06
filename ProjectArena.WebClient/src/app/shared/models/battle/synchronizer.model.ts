import { SyncActor } from './synchronization/sync-actor.model';
import { SyncDecoration } from './synchronization/sync-decoration.model';
import { SyncPlayer } from './synchronization/sync-player.model';
import { SyncSpecEffect } from './synchronization/sync-spec-effect.model';
import { SyncTile } from './synchronization/sync-tile.model';
import { Reward } from './reward.model';

export interface Synchronizer {
  id: string;
  version: number;
  roundsPassed: number;
  idle: boolean;
  actorId?: number;
  skillActionId?: number;
  targetX?: number;
  targetY?: number;
  tilesetWidth: number;
  tilesetHeight: number;
  turnTime: number;
  tempActor?: number;
  tempDecoration?: number;
  reward: Reward;
  players: SyncPlayer[];
  changedActors: SyncActor[];
  changedDecorations: SyncDecoration[];
  changedEffects: SyncSpecEffect[];
  deletedActors: number[];
  deletedDecorations: number[];
  deletedEffects: number[];
  changedTiles: SyncTile[];
}
