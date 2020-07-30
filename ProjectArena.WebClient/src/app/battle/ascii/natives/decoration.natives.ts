import { DecorationNative } from '../models/natives/decoration-native.model';


export const decorationNatives: { [id: string]: DecorationNative } = {
  barrier: {
    name: 'Barrier',
    description: 'Unpassable barrier.',
    active: false,
    visualization: {
      char: '#',
      color: {r: 174, g: 92, b: 0, a: 1}
    },
    action: undefined,
    onDeathAction: undefined
  },
  tree: {
    name: 'Tree',
    description: 'Unpassable barrier.',
    active: false,
    visualization: {
      char: 'tree',
      color: { r: 160, g: 0, b: 200, a: 1 }
    },
    action: undefined,
    onDeathAction: undefined
  }
};
