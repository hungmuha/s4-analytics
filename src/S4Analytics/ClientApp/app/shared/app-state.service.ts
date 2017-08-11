// Service to manage app state

import { Injectable } from '@angular/core';
import { Options } from './options-resolve.service';

@Injectable()
export class AppStateService {
    options: Options;
}
