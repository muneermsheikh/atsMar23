
export interface ICandidateAssessmentItem
{
     id: number;
     candidateAssessmentId: number;
     questionNo: number;
     isMandatory: boolean;
     assessed: boolean;
     question: string;
     maxPoints: number;
     points: number;
     remarks: string;
}

export class CandidateAssessmentItem
{
     constructor(_questionNo: number, _isMandatory: boolean, _question: string, _maxPoints: number) {
          this.questionNo = _questionNo;    
          this.isMandatory = _isMandatory;
          this.question = _question;
          this.questionNo = _questionNo;
          this.maxPoints = _maxPoints;
          this.id = 0;   //so as to patch values
     }

     id = 0;
     candidateAssessmentId=0;
     questionNo = 0;
     isMandatory = false;
     assessed=false;
     question='';
     maxPoints=0;
     points=0;
     remarks='';
}

