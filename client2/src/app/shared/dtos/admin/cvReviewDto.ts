export interface ICVReviewDto {
     candidateId: number;
     orderItemId: number;
     execRemarks: string;
     charges: number;
     assignedToId: number;
}

export class cvReviewDto implements ICVReviewDto {
     candidateId = 0;
     orderItemId = 0;
     execRemarks = '';
     charges = 0;
     assignedToId = 0;
}