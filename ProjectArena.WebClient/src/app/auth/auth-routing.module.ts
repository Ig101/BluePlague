import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { AuthComponent } from './auth/auth.component';
import { EmailConfirmationResolverService } from './resolvers/email-confirmation-resolver.service';
import { SignInComponent } from './user-management/sign-in/sign-in.component';
import { SignUpComponent } from './user-management/sign-up/sign-up.component';
import { EmailConfirmationComponent } from './user-management/email-confirmation/email-confirmation.component';
import { ForgotPasswordComponent } from './user-management/forgot-password/forgot-password.component';
import { NewPasswordComponent } from './user-management/new-password/new-password.component';
import { UserMenuResolverService } from './resolvers/user-menu-resolver.service';
import { AuthorizationMenuResolverService } from './resolvers/authorization-menu-resolver.service';
import { EmailMenuResolverService } from './resolvers/email-menu-resolver.service';
import { AuthResolverService } from './resolvers/auth-resolver.service';

const loginRoutes: Routes = [
  {
    path: 'signin',
    component: SignInComponent,
    resolve: { AuthorizationMenuResolverService }
  },
  {
    path: 'signup',
    component: SignUpComponent,
    resolve: { AuthorizationMenuResolverService }
  },
  {
    path: 'signup/confirmation',
    component: EmailConfirmationComponent,
    resolve: { EmailMenuResolverService }
  },
  {
    path: 'signin/forgot-password',
    component: ForgotPasswordComponent,
    resolve: { AuthorizationMenuResolverService }
  },
  {
    path: 'signin/new-password/:id/:token',
    component: NewPasswordComponent,
    resolve: { AuthorizationMenuResolverService }
  },
  {
    path: 'signup/confirmation/:id/:token',
    component: SignInComponent,
    resolve: { EmailConfirmationResolverService }
  },
  {
    path: '**',
    redirectTo: 'signin'
  }
];

const routes: Routes = [
  {
    path: '',
    component: AuthComponent,
    resolve: { auth: AuthResolverService },
    children: loginRoutes
  },
  {
    path: '**',
    redirectTo: ''
  }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AuthRoutingModule { }
