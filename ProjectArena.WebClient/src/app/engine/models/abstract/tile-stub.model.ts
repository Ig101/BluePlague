import { Color } from 'src/app/shared/models/color.model';
import { Actor } from '../../scene/actor.object';

export interface TileStub {
  endTime: number;
  x?: number;
  y?: number;
  actorLink?: Actor;
  char?: string; // if null not change
  active?: boolean;
  color?: Color; // if null not change
  backgroundColor?: Color; // if null not change
  priority: number;
}
