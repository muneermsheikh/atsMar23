export interface ICandidateShortlisted
{
     id: number;
     orderItemId: number;
     candidateId: number;
     assessedById: number;
     assessedOn: Date;
     requireInternalReview: boolean;
     remarks: string;
}