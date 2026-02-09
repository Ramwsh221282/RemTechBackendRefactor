import {animate, AnimationTriggerMetadata, state, style, transition, trigger,} from '@angular/animations';

export class AnimationsFactory {
  public static expandCollapseAnimation(
    styleToModifyName: string,
    styleValueAfter: string,
    styleValueBefore: string
  ): AnimationTriggerMetadata {
    const animation = trigger('expandCollapse', [
      state('expand', style({ [styleToModifyName]: styleValueAfter })),
      state('collapse', style({ [styleToModifyName]: styleValueBefore })),
      transition('expand => collapse', [animate('0.1s ease-out')]),
      transition('collapse => expand', [animate('0.1s ease-out')]),
    ]);
    return animation;
  }

  public static fadeInOutAnimation(
    styleValueAfter: number,
    styleValueBefore: number
  ) {
    const animation = trigger('fadeInOut', [
      state('fadeIn', style({ opacity: String(styleValueAfter) })),
      state('fadeOut', style({ opacity: String(styleValueBefore) })),
      transition('fadeIn => fadeOut', [animate('1s ease-out')]),
      transition('fadeOut => fadeIn', [animate('1s ease-out')]),
    ]);
    return animation;
  }

  public static fadeInAnimation(timingsSeconds: string) {
    const enterTransition = transition(':enter', [
      style({opacity: '0'}),
      animate(timingsSeconds, style({opacity: '1'})),
    ])
    return trigger('fadeIn', [enterTransition]);
  }

  public static fadeOutAnimation(timingsSeconds: string) {
    const enterTransition = transition(':leave', [
      style({opacity: '1'}),
      animate(timingsSeconds, style({opacity: '0'})),
    ])
    return trigger('fadeOut', [enterTransition]);
  }
}
