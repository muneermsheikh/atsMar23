import { IInterviewItemCandidateFollowup } from "./interviewItemCandidateFollowup";

export interface IInterviewItemCandidate {
     id: number;
     interviewItemId: number;
     candidateId: number;
     applicationNo: number;
     passportNo: string;
     scheduledFrom: Date;
     scheduledUpto: Date;
     interviewMode: string;
     reportedDateTime: Date;
     interviewedDateTime: Date;
     attendanceStatusId: number;
     concludingRemarks: string;
     interviewFollowups: IInterviewItemCandidateFollowup[];
}
