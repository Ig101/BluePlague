import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LobbyRoutingModule } from './lobby-routing.module';
import { LobbyResolverService } from './resolvers/lobby-resolver.service';
import { SharedModule } from '../shared/shared.module';
import { QueueService } from './services/queue.service';
import { TalentsService } from './services/talents.service';



@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    LobbyRoutingModule
  ],
  providers: [
    LobbyResolverService,
    QueueService,
    TalentsService
  ]
})
export class LobbyModule { }
