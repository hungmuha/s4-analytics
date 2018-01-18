export * from './keep-silverlight-alive.service';
export * from './options-resolve.service';
export * from './identity.service';
export * from './auth-guard.service';
export * from './s4-identity-user';
export * from './html5-conduit-resolve.service';
export * from './app-state.service';
export * from './checkbox-group.component';
export * from './radio-button-group.component';
export * from './button-group.component';
export * from './card.component';
export * from './server-time-resolve.service';
export * from './server-date-resolve.service';

import { KeepSilverlightAliveService } from './keep-silverlight-alive.service';
import { OptionsResolveService } from './options-resolve.service';
import { IdentityService } from './identity.service';
import { AuthGuard, AnyAdminGuard, GlobalAdminGuard } from './auth-guard.service';
import { Html5ConduitResolve } from './html5-conduit-resolve.service';
import { AppStateService } from './app-state.service';
import { ServerTimeResolveService } from './server-time-resolve.service';
import { ServerDateResolveService } from './server-date-resolve.service';
import { CheckboxGroupComponent } from './checkbox-group.component';
import { RadioButtonGroupComponent } from './radio-button-group.component';
import { ButtonGroupComponent } from './button-group.component';
import { CardComponent } from './card.component';

export const PROVIDERS: any[] = [
    KeepSilverlightAliveService,
    OptionsResolveService,
    IdentityService,
    AuthGuard,
    AnyAdminGuard,
    GlobalAdminGuard,
    Html5ConduitResolve,
    AppStateService,
    ServerTimeResolveService,
    ServerDateResolveService
];

export const COMPONENTS: any[] = [
    CheckboxGroupComponent,
    RadioButtonGroupComponent,
    ButtonGroupComponent,
    CardComponent
];
