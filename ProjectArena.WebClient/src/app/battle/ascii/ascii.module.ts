import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from 'src/app/shared/shared.module';
import { AsciiRoutingModule } from './ascii-routing.module';
import { AsciiBattleComponent } from './ascii-battle/ascii-battle.component';
import { AsciiBattleStorageService } from './services/ascii-battle-storage.service';
import { AsciiBattleSynchronizerService } from './services/ascii-battle-synchronizer.service';
import { AsciiBattlePathCreatorService } from './services/ascii-battle-path-creator.service';
import { InitiativeBlockComponent } from './ascii-battle/initiative-block/initiative-block.component';
import { ActionPointsBlockComponent } from './ascii-battle/action-points-block/action-points-block.component';
import { HotkeyedButtonComponent } from './ascii-battle/hotkeyed-button/hotkeyed-button.component';
import { AsciiBattleAnimationsService } from './services/ascii-battle-animations.service';
import { VictoryModalComponent } from './modals/victory-modal/victory-modal.component';
import { ActorModalComponent } from './modals/actor-modal/actor-modal.component';
import { SkillModalComponent } from './modals/skill-modal/skill-modal.component';
import { SceneObjectModalComponent } from './modals/scene-object-modal/scene-object-modal.component';
import { ModalTabsComponentComponent } from './modals/modal-tabs-component/modal-tabs-component.component';
import { MenuModalComponent } from './modals/menu-modal/menu-modal.component';
import { RewardForModalComponent } from './modals/reward-for-modal/reward-for-modal.component';



@NgModule({
  declarations: [
    AsciiBattleComponent,
    InitiativeBlockComponent,
    ActionPointsBlockComponent,
    HotkeyedButtonComponent,
    VictoryModalComponent,
    ActorModalComponent,
    SkillModalComponent,
    SceneObjectModalComponent,
    ModalTabsComponentComponent,
    MenuModalComponent,
    RewardForModalComponent],
  providers: [
    AsciiBattleStorageService,
    AsciiBattleSynchronizerService,
    AsciiBattlePathCreatorService,
    AsciiBattleAnimationsService
  ],
  imports: [
    SharedModule,
    AsciiRoutingModule
  ]
})
export class AsciiModule { }
