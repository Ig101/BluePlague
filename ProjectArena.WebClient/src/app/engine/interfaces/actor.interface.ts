import { Player } from '../scene/abstract/player.object';
import { Effect } from '../content/effects';
import { Buff } from '../models/abstract/buff.model';
import { Actor } from '../scene/actor.object';

export interface IActor {
  readonly id: number;

  readonly tags: string[];

  readonly x: number;
  readonly y: number;
  readonly z: number;

  isAlive: boolean;
  owner: Player;

  processEffect(effect: Effect, power: number, order: number);

  applyBuff(buff: Buff);

  purgeBuffs();

  changeDurability(durability: number);

  removeActor(actor: Actor);

  addActor(actor: Actor);

  getActorZ(actor: Actor);

  move(target: IActor);
}