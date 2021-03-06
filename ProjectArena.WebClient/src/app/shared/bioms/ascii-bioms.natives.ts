import { Color } from '../models/color.model';

export const asciiBioms: {[biom: number]: {char: string, color: Color, backgroundColor: Color, probability: number}[] } = {
  0: [
    {
      char: '-',
      color: { r: 0, g: 180, b: 0, a: 1 },
      backgroundColor: { r: 0, g: 35, b: 0 },
      probability: 6
    },
    {
      char: '*',
      color: { r: 0, g: 160, b: 0, a: 1 },
      backgroundColor: { r: 0, g: 35, b: 0 },
      probability: 1
    },
    {
      char: 'Y',
      color: { r: 139, g: 69, b: 19, a: 1 },
      backgroundColor: { r: 0, g: 35, b: 0 },
      probability: 1
    },
    {
      char: 'Y',
      color: { r: 255, g: 165, b: 79, a: 1 },
      backgroundColor: { r: 0, g: 35, b: 0 },
      probability: 1
    }
  ]
};
