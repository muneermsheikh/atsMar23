export interface ICandidateAssessedDto
{
     id: number;
     checked: boolean;
     orderItemId: number;
     orderItemRef: string;
     categoryName: string;
     applicationNo: number;
     customerName: string;
     requireInternalReview: boolean;
     candidateId: number;
     candidateName: string;
     assessedById: number;
     checklistedByName: string;
     checklistedOn: Date;
     charges: string;
     assessedByName: string;
     assessedResult: string;
     assessedOn: Date;
     cvRefId: number;
     remarks: string;
}

export class CandidateAssessedDto implements ICandidateAssessedDto
{
     id= 0;
     checked= false;
     orderItemId= 0;
     orderItemRef= '';
     categoryName= '';
     applicationNo= 0;
     customerName= '';
     requireInternalReview= false;
     candidateId= 0;
     candidateName= '';
     assessedById= 0;
     checklistedByName= '';
     checklistedOn: Date = new Date('1900-01-01');
     charges= '';
     assessedByName= '';
     assessedResult= '';
     assessedOn: Date = new Date('1900-01-01');
     cvRefId= 0;
     remarks= '';
}