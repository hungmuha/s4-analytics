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

import { KeepSilverlightAliveService } from './keep-silverlight-alive.service';
import { OptionsResolveService, Options } from './options-resolve.service';
import { IdentityService } from './identity.service';
import { AuthGuard, AnyAdminGuard, GlobalAdminGuard } from './auth-guard.service';
import { S4IdentityUser } from './s4-identity-user';
import { Html5ConduitResolve } from './html5-conduit-resolve.service';
import { AppStateService } from './app-state.service';

export const PROVIDERS: any[] = [
    KeepSilverlightAliveService,
    OptionsResolveService,
    IdentityService,
    AuthGuard,
    AnyAdminGuard,
    GlobalAdminGuard,
    Html5ConduitResolve,
    AppStateService
];
