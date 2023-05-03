export interface IUserExp {
     id: number;
     candidateId: number;
     srNo: number;
     positionId?: number;
     employer: string;
     position: string;
     currentJob?: boolean;
     salaryCurrency: string;
     monthlySalaryDrawn?: number;
     workedFrom: Date;
     workedUpto?: Date;
}