// The pipe class implements the PipeTransform interface's transform method that accepts an input value and an optional array of parameters and returns the transformed value.

import { Pipe, PipeTransform } from '@angular/core';

// We tell Angular that this is a pipe by applying the @Pipe decorator which we import from the core Angular library.

@Pipe({

    // The @Pipe decorator takes an object with a name property whose value is the pipe name that we'll use within a template expression. It must be a valid JavaScript identifier. Our pipe's name is orderby.

    name: 'orderby'
})
// Code from https://github.com/nicolas2bert/angular2-orderby-pipe/blob/master/app/orderby.ts used under the ISC license
// Modified to pass in whether the list is to be reversed - mf
export class OrderByPipe implements PipeTransform {
    transform (array: Array<any>, args: any[]) {

        // Check if array exists
        if (array) {
            // get the first element
            let orderByValue = args[0];
            let reverse = args[1];
            let byVal = 1;

            // check if exclamation point
            if (reverse) {
                // reverse the array
                byVal = -1;
                orderByValue = orderByValue.substring(1);
            }

            array.sort((a: any, b: any) => {
                if (a[orderByValue] < b[orderByValue]) {
                    return -1 * byVal;
                } else if (a[orderByValue] > b[orderByValue]) {
                    return 1 * byVal;
                } else {
                    return 0;
                }
            });
            return array;
        }
    }
}