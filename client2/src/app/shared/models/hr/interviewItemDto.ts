
export interface IInterviewItemDto {  
     id: number;
     interviewId: number;
     orderItemId: number;
     categoryId: number;
     categoryName: string;
     interviewDate: Date;
     applicationNo: number;
     candidateName: string;
     interviewMode: string;
     attendanceStatus: string;
     remarks: string;
     //interviewItemCandidates: IInterviewItemCandidate[];
}
