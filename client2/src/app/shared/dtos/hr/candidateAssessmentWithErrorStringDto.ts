import { ICandidateAssessment } from "../../models/hr/candidateAssessment";

export interface ICandidateAssessmentWithErrorStringDto
{
     candidateAssessment: ICandidateAssessment;
     errorString: string;
}