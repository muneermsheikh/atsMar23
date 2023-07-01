import { ICandidateAssessmentItem } from "./candidateAssessmentItem";

export interface ICandidateAssessment
{
     id: number;
     orderItemId: number;
     candidateId: number;
     assessedById: number;
     assessedByName: string;
     assessedOn: Date;
     assessResult: string;
     requireInternalReview: boolean;
     hrChecklistId: number;
     taskIdDocControllerAdmin: number;
     remarks: string;
     candidateAssessmentItems: ICandidateAssessmentItem[];
}
export class CandidateAssessment implements ICandidateAssessment
{
     constructor(_orderitemid: number, _candidateid: number, _assessedbyid: number,  _assessedByName: string, _assessedon: Date, _requireInternalReview: boolean, 
          _hrChecklistId: number, _items: ICandidateAssessmentItem[]) {
          this.orderItemId = _orderitemid;
          this.candidateId = _candidateid;
          this.assessedById = _assessedbyid;
          this.assessedByName = _assessedByName;
          this.assessedOn = _assessedon;
          this.requireInternalReview = _requireInternalReview;
          this.hrChecklistId = _hrChecklistId
          this.candidateAssessmentItems = _items;
          this.id = 0;   //so as to patch values
     }
     id= 0;
     orderItemId =0;
     candidateId= 0;
     assessedById= 0;
     assessedByName= '';
     assessedOn: Date;
     assessResult= "";
     requireInternalReview: boolean;
     hrChecklistId= 0;
     remarks= '';
     cvRefId= 0;
     taskIdDocControllerAdmin= 0;
     candidateAssessmentItems: ICandidateAssessmentItem[]=[];
}
