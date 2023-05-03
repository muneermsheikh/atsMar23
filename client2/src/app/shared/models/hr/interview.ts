import {IInterviewItem} from "./interviewItem";

export interface IInterview{
     id: number;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerId: number;
     customerName: string;
     interviewMode: string;
     interviewerName: string;
     interviewVenue: string;
     interviewDateFrom: Date;
     interviewDateUpto: Date;
     interviewLeaderId: number;
     customerRepresentative: string;
     interviewStatus: string;
     concludingRemarks: string;
     interviewItems: IInterviewItem[];
}



