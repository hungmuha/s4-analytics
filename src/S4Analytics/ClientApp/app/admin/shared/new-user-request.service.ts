import { Injectable } from '@angular/core';
import { NewUserRequest } from './new-user-request';


@Injectable()
export class NewUserRequestService {

    getNewUserRequests(): NewUserRequest[] {

        let nur1 = {
                requestNbr: 123,
                requestDt: new Date('12/16/2017'),
                requestDesc: 'New Employee',
                requestType: 5,
                requestStatus: 6,
                userCreatedDt: new Date('12/16/2017'),
                agncyId: 7,
                newAgncyTypeCd: 0,
                newAgncyNm: '',
                newAgncyEmailDomain: '',
                requestorFirstNm: 'Sara',
                requestorLastNm: 'Yorty',
                requestorSuffixNm: '',
                requestorEmail: 'syorty@alachua.gov',
                contractorId: 0,
                newContractorNm: '',
                newContractorEmailDomain: '',
                consultantFirstNm: '',
                consultantLastNm: '',
                consultantSuffixNm: '',
                consultantEmail: '',
                accessReasonTx: '',
                contractStartDt: new Date('12/16/2017'),
                contractEndDt: new Date('12/16/2017'),
                userId: '',
                warnRequestorEmail: 'N',
                warnUserEmailCd: 'N',
                warnUserAgncyEmailCd: 'N',
                warnUserVendorEmailCd: 'N',
                adminComment: ''
            };

        let nur2 = {
                requestNbr: 123,
                requestDt: new Date('12/18/2017'),
                requestDesc: 'New Employee',
                requestType: 10,
                requestStatus: 11,
                userCreatedDt: new Date('12/18/2017'),
                agncyId: 12,
                newAgncyTypeCd: 0,
                newAgncyNm: '',
                newAgncyEmailDomain: '',
                requestorFirstNm: 'Jim',
                requestorLastNm: 'Jones',
                requestorSuffixNm: '',
                requestorEmail: 'jjones@alachua.gov',
                contractorId: 0,
                newContractorNm: '',
                newContractorEmailDomain: '',
                consultantFirstNm: '',
                consultantLastNm: '',
                consultantSuffixNm: '',
                consultantEmail: '',
                accessReasonTx: '',
                contractStartDt: new Date('12/18/2017'),
                contractEndDt: new Date('12/18/2017'),
                userId: '',
                warnRequestorEmail: 'N',
                warnUserEmailCd: 'N',
                warnUserAgncyEmailCd: 'N',
                warnUserVendorEmailCd: 'N',
                adminComment: ''
            };

        return [nur1 as NewUserRequest, nur2 as NewUserRequest];
    }




}