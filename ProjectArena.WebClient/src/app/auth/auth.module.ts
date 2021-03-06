import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { AuthRoutingModule } from './auth-routing.module';
import { EmailConfirmationComponent } from './user-management/email-confirmation/email-confirmation.component';
import { AuthComponent } from './auth/auth.component';
import { EmailConfirmationResolverService } from './resolvers/email-confirmation-resolver.service';
import { SignInComponent } from './user-management/sign-in/sign-in.component';
import { SignUpComponent } from './user-management/sign-up/sign-up.component';
import { UserManagementComponent } from './user-management/user-management.component';
import { UserManagementLoadingComponent } from './user-management/user-management-loading/user-management-loading.component';
import { ForgotPasswordComponent } from './user-management/forgot-password/forgot-password.component';
import { NewPasswordComponent } from './user-management/new-password/new-password.component';
import { UserManagementService } from './services/user-management.service';
import { UserMenuResolverService } from './resolvers/user-menu-resolver.service';
import { EmailMenuResolverService } from './resolvers/email-menu-resolver.service';
import { AuthorizationMenuResolverService } from './resolvers/authorization-menu-resolver.service';
import { AuthResolverService } from './resolvers/auth-resolver.service';


@NgModule({
  declarations: [
    EmailConfirmationComponent,
    AuthComponent,
    SignInComponent,
    SignUpComponent,
    UserManagementComponent,
    UserManagementLoadingComponent,
    ForgotPasswordComponent,
    NewPasswordComponent
  ],
  imports: [
    SharedModule,
    AuthRoutingModule
  ],
  providers: [
    EmailConfirmationResolverService,
    UserManagementService,
    AuthorizationMenuResolverService,
    EmailMenuResolverService,
    UserMenuResolverService,
    AuthResolverService
  ]
})
export class AuthModule { }
