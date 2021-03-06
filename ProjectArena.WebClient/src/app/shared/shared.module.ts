import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TextInputComponent } from './components/text-input/text-input.component';
import { ButtonComponent } from './components/button/button.component';
import { LinkComponent } from './components/link/link.component';
import { FocusRemoverDirective } from './components/directives/focus-remover.directive';
import { MenuIconComponent } from './components/menu-icon/menu-icon.component';
import { ModalService } from './services/modal.service';
import { ModalLoadingComponent } from './components/modal-loading/modal-loading.component';

@NgModule({
  declarations: [TextInputComponent, ButtonComponent, LinkComponent, FocusRemoverDirective, MenuIconComponent, ModalLoadingComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  providers: [
    ModalService
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    TextInputComponent,
    ButtonComponent,
    LinkComponent,
    FocusRemoverDirective,
    MenuIconComponent,
    ModalLoadingComponent
  ]
})
export class SharedModule { }
