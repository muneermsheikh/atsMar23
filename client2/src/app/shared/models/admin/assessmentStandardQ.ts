export interface IAssessmentStandardQ
{    
     id: number;
     subject: string;
     questionNo: number;
     question: string;
     maxPoints: number;
}

export class AssessmentStandardQ implements IAssessmentStandardQ
{    
     id = 0;
     subject = '';
     questionNo = 0;
     question = '';
     maxPoints = 0;
}