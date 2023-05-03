import { IChecklistHRDto } from "../../dtos/hr/checklistHRDto";
import { ICandidateAssessment } from "./candidateAssessment";

export interface ICandidateAssessmentAndChecklist 
{
     assessed: ICandidateAssessment,
     checklistHRDto: IChecklistHRDto
}