import { IInterviewItemCandidate } from "./interviewItemCandidate";

export interface IInterviewItem {  
     id: number;
     interviewId: number;
     orderItemId: number;
     categoryId: number;
     interviewDateFrom: Date;
     interviewDateUpto: Date;
     interviewMode: string;
     interviewerName: string;
     concludingRemarks: string;
     interviewStatus: string;
     interviewItemCandidates: IInterviewItemCandidate[];
}
