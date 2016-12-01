﻿import { Pipe } from '@angular/core';

@Pipe({
    name: 'truncate'
})
export class TruncatePipe {
    transform(value: string, limit: string = '10', trail: string = '...'): string {
        let limitNum = parseInt(limit);
        return value.length > limitNum ? value.substring(0, limitNum) + trail : value;
    }
}
