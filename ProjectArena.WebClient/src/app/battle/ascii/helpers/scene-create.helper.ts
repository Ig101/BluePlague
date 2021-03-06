import { SyncTile } from 'src/app/shared/models/battle/synchronization/sync-tile.model';
import { tileNatives, skillNatives, actorNatives, buffNatives, decorationNatives, effectNatives } from '../natives';
import { SyncSkill } from 'src/app/shared/models/battle/synchronization/sync-skill.model';
import { SyncBuff } from 'src/app/shared/models/battle/synchronization/sync-buff.model';
import { SyncActor } from 'src/app/shared/models/battle/synchronization/sync-actor.model';
import { Skill } from '../models/scene/skill.model';
import { Tile } from '../models/scene/tile.model';
import { Buff } from '../models/scene/buff.model';
import { Actor } from '../models/scene/actor.model';
import { ActionAnimation } from '../models/action-animation.model';
import { TwoPhaseActionAnimation } from '../models/two-phase-action-animation.model';
import { BuffAnimation } from '../models/buff-animation.model';
import { Player } from '../models/player.model';
import { SyncDecoration } from 'src/app/shared/models/battle/synchronization/sync-decoration.model';
import { ActiveDecoration } from '../models/scene/active-decoration.model';
import { SyncSpecEffect } from 'src/app/shared/models/battle/synchronization/sync-spec-effect.model';
import { SpecEffect } from '../models/scene/spec-effect.model';

export function cloneActionAnimation(animation: ActionAnimation): ActionAnimation {
  return animation ? {
    generateDeclarations: animation.generateDeclarations
  } : undefined;
}

export function cloneTwoPhaseActionAnimation(animation: TwoPhaseActionAnimation): TwoPhaseActionAnimation {
  return animation ? {
    generateIssueDeclarations: animation.generateIssueDeclarations,
    generateSyncDeclarations: animation.generateSyncDeclarations
  } : undefined;
}

export function cloneBuffAnimation(animation: BuffAnimation): BuffAnimation {
  return animation ? {
    doSomethingWithBearer: animation.doSomethingWithBearer,
    resetEffect: animation.resetEffect,
    colorDifference: { r: 0, g: 0, b: 0, a: 0 },
    changedChar: false
  } : undefined;
}

export function convertTile(tile: SyncTile, owner: Player): Tile {
  let native = tileNatives[tile.nativeId];
  if (!native) {
    native = {
      name: 'Undefined',
      description: undefined,
      visualization: {
        char: '!',
        color: { r: 200, g: 200, b: 0, a: 1 }
      },
      backgroundColor: { r: 15, g: 15, b: 0 },
      bright: false,
      action: undefined,
      onActionEffectAnimation: undefined,
      onStepAction: undefined
    };
    console.error(`Tile native ${tile.nativeId} is not found.`);
  }
  return {
    x: tile.x,
    y: tile.y,
    name: native.name,
    description: native.description,
    visualization: native.visualization,
    backgroundColor: native.backgroundColor,
    bright: native.bright,
    action: cloneActionAnimation(native.action),
    onStepAction: cloneActionAnimation(native.onStepAction),
    onActionEffectAnimation: cloneActionAnimation(native.onActionEffectAnimation),
    height: tile.height,
    nativeId: tile.nativeId,
    owner,
    actor: undefined,
    decoration: undefined,
    specEffects: [],
    unbearable: tile.unbearable
  };
}

export function convertSkill(skill: SyncSkill): Skill {
  if (!skill) {
    return undefined;
  }

  let skillNative = skillNatives[skill.visualization];
  if (!skillNative) {
    skillNative = {
      name: 'Undefined',
      description: undefined,
      action: undefined
    };
    console.error(`Skill native ${skill.visualization} is not found.`);
  }
  return {
    id: skill.id,
    name: skillNative.name,
    description: skillNative.description,
    action: cloneTwoPhaseActionAnimation(skillNative.action),
    range: skill.range,
    cd: skill.cd,
    cost: skill.cost,
    preparationTime: Math.ceil(skill.preparationTime),
    onlyVisibleTargets: skill.onlyVisibleTargets,
    availableTargets: skill.availableTargets
  };
}

export function convertBuff(buff: SyncBuff): Buff {
  let buffNative = buffNatives[buff.nativeId];
  if (!buffNative) {
    buffNative = {
      name: 'Undefined',
      description: undefined,
      char: '!',
      color: {r: 255, g: 255, b: 0, a: 1},
      onActionEffectAnimation: undefined,
      onApplyAnimation: undefined,
      onPurgeAnimation: undefined,
      passiveAnimation: undefined,
      effectAnimation: undefined
    };
    console.error(`Buff native ${buff.nativeId} is not found.`);
  }
  return {
    id: buff.id,
    char: buffNative.char,
    color: buffNative.color,
    name: buffNative.name,
    description: buffNative.description,
    onApplyAnimation: cloneActionAnimation(buffNative.onApplyAnimation),
    effectAnimation: cloneActionAnimation(buffNative.effectAnimation),
    onPurgeAnimation: cloneActionAnimation(buffNative.onPurgeAnimation),
    passiveAnimation: cloneBuffAnimation(buffNative.passiveAnimation),
    onActionEffectAnimation: cloneActionAnimation(buffNative.onActionEffectAnimation),
    duration: buff.duration
  };
}

export function convertActor(actor: SyncActor, owner: Player, name: string): Actor {
  let native = actorNatives[actor.visualization];
  if (!native) {
    native = {
      name: 'Undefined',
      description: undefined,
      visualization: {
        char: '!',
        color: { r: 255, g: 255, b: 0, a: 1 }
      }
    };
    console.error(`Actor native ${actor.visualization} is not found.`);
  }
  const visualization = native.visualization;
  return {
    id: actor.id,
    name: name ? name : native.name,
    description: name ? native.name : native.description,
    defaultVisualization: visualization,
    visualization: {
      char: visualization.char,
      color: { r: visualization.color.r, g: visualization.color.g, b: visualization.color. b, a: visualization.color.a }
    },
    externalId: actor.externalId,
    attackingSkill: convertSkill(actor.attackingSkill),
    skills: actor.skills.map(skill => convertSkill(skill) ),
    buffs: actor.buffs.map(buff => convertBuff(buff)),
    initiativePosition: actor.initiativePosition,
    health: actor.health,
    owner,
    x: actor.x,
    y: actor.y,
    z: actor.z,
    maxHealth: actor.maxHealth,
    actionPoints: actor.actionPoints,
    initiative: actor.initiative,
    canAct: actor.canAct,
    canMove: actor.canMove
  };
}

export function convertDecoration(decoration: SyncDecoration, owner: Player): ActiveDecoration {
  let native = decorationNatives[decoration.visualization];
  if (!native) {
    native = {
      name: 'Undefined',
      description: undefined,
      visualization: {
        char: '!',
        color: { r: 255, g: 0, b: 0, a: 1 }
      },
      active: false,
      action: undefined,
      onDeathAction: undefined
    };
    console.error(`Decoration native ${decoration.visualization} is not found.`);
  }
  return {
    id: decoration.id,
    name: native.name,
    description: native.description,
    visualization: native.visualization,
    action: cloneActionAnimation(native.action),
    onDeathAction: cloneActionAnimation(native.onDeathAction),
    active: native.active,
    initiativePosition: decoration.initiativePosition,
    health: decoration.health,
    owner,
    isAlive: decoration.isAlive,
    x: decoration.x,
    y: decoration.y,
    z: decoration.z,
    maxHealth: decoration.maxHealth
  };
}

export function convertEffect(effect: SyncSpecEffect, owner: Player): SpecEffect {
  let native = effectNatives[effect.visualization];
  if (!native) {
    native = {
      name: 'Undefined',
      description: undefined,
      visualization: {
        char: '!',
        color: { r: 0, g: 255, b: 255, a: 1 }
      },
      action: undefined,
      onActionEffectAnimation: undefined,
      onDeathAction: undefined
    };
    console.error(`Effect native ${effect.visualization} is not found.`);
  }
  return {
    id: effect.id,
    name: native.name,
    description: native.description,
    visualization: native.visualization,
    action: cloneActionAnimation(native.action),
    onDeathAction: cloneActionAnimation(native.onDeathAction),
    onActionEffectAnimation: cloneActionAnimation(native.onActionEffectAnimation),
    owner,
    isAlive: effect.isAlive,
    x: effect.x,
    y: effect.y,
    z: effect.z,
    duration: effect.duration
  };
}
