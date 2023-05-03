
export interface IInterviewItemCandidateFollowup {
     id: number;
     interviewItemCandidateId: number;
     contactedOn: Date;
     contactedById: number;
     mobileNoCalled: number;
     attendanceStatusId: number;
     followupConcluded: boolean;
}
