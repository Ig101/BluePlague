import { Color } from 'src/app/shared/models/color.model';
import { AnimationFrame } from '../../models/animations/animation-frame.model';
import { rangeBetween, angleBetween } from 'src/app/helpers/math.helper';

export function throwIssueDeclaration(issueX: number, issueY: number, targetX: number, targetY: number, color: Color) {
  const frames: AnimationFrame[] = [];
  const range = rangeBetween(issueX, issueY, targetX, targetY);
  const angle = angleBetween(issueX, issueY, targetX, targetY);
  const sin = Math.sin(angle);
  const cos = Math.cos(angle);
  for (let i = 0; i < range; i++) {
    const passed = i;
    const frameColor = {
      r: (Math.random() + 9) / 10 * color.r,
      g: (Math.random() + 9) / 10 * color.g,
      b: (Math.random() + 9) / 10 * color.b,
      a: 1};
    let x;
    let y;
    if (passed > range) {
      x = targetX;
      y = targetY;
    } else {
      x = Math.round(issueX + (passed * cos));
      y = Math.round(issueY + (passed * sin));
    }
    frames.push({
      updateSynchronizer: false,
      animationTiles: [{x, y, char: '*', color: frameColor, unitColorMultiplier: 0,
        unitAlpha: false, ignoreHeight: false, overflowHealth: false, priority: 10, workingOnSpecEffects: true}],
      specificAction: undefined
    });
  }
  frames.push({
    updateSynchronizer: true,
    animationTiles: [{x: targetX, y: targetY, char: undefined, color: {r: 255, g: 255, b: 255, a: 1}, unitAlpha: true,
      unitColorMultiplier: 0, priority: 10, ignoreHeight: true, overflowHealth: false, workingOnSpecEffects: false}],
    specificAction: undefined
  });
  return frames;
}
