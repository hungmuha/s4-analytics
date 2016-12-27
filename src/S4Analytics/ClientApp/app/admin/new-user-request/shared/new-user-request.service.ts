import { Injectable } from '@angular/core';
import { NewUserRequest } from './new-user-request';


@Injectable()
export class NewUserRequestService {

    getNewUserRequests(): NewUserRequest[] {
        let nur1 = new NewUserRequest(
            123,
            new Date('12/12/2017'),
            'request desc',
            2,
            0,
            null,
            7,
            1,
            '',
            '',
            'Sara',
            'Yorty',
            '',
            'syorty@alachua.gov',
            null,null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null);

        let nur2 = new NewUserRequest(
            124,
            new Date('12/13/2017'),
            'request desc',
            4,
            5,
            null,
            6,
            9,
            '',
            '',
            'Jim',
            'Jomes',
            '',
            'jones@alachua.gov',
            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null);

        return [nur1, nur2];
    }




}