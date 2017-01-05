import { Injectable } from '@angular/core';
import { NewUserRequest } from './new-user-request';


@Injectable()
export class NewUserRequestService {

    getNewUserRequests(): NewUserRequest[] {

        let nur1 = {
                requestNbr: 123,
                requestDt: '12/16/2017',
                requestDesc: 'New Employee',
                requestType: 5,
                requestStatus: 6,
                userCreatedDt: '',
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
                contractStartDt: '',
                contractEndDt: '',
                userId: '',
                warnRequestorEmail: 'N',
                warnUserEmailCd: 'N',
                warnUserAgncyEmailCd: 'N',
                warnUserVendorEmailCd: 'N',
                adminComment: ''
            };

        let nur2 = {
                requestNbr: 123,
                requestDt: '12/18/2017',
                requestDesc: 'New Contractor, New Consultant',
                requestType: 10,
                requestStatus: 11,
                userCreatedDt: '',
                agncyId: 12,
                newAgncyTypeCd: 0,
                newAgncyNm: '',
                newAgncyEmailDomain: '',
                requestorFirstNm: 'Jim',
                requestorLastNm: 'Jones',
                requestorSuffixNm: '',
                requestorEmail: 'jjones@alachua.gov',
                contractorId: 0,
                newContractorNm: 'New Contract Company, Inc',
                newContractorEmailDomain: 'ncc.gov',
                consultantFirstNm: 'Mary',
                consultantLastNm: 'Smith',
                consultantSuffixNm: '',
                consultantEmail: 'msmith@ncc.gov',
                accessReasonTx: 'She needs to do some work',
                contractStartDt: '12/18/2017',
                contractEndDt: '12/18/2017',
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