import { CreateSelDecision } from "../../models/admin/createSelDecision";

export class selDecisionToAddDto
{
	cVRefId: number=0;
	decisionDate: Date=new Date();
	selectionStatusId: number=0;
	remarks: string='';
}

export class selDecisionsToAddParams
{
	selDecisionsToAddDto: CreateSelDecision[]=[];
	advisesToClients: boolean=false;
	selectionEmailToCandidates: boolean = false;
	rejectionEmaiLToCandidates: boolean=false;
	rejectionSMSToCandidates: boolean = false;
	selectionSMSToCandidates: boolean = false;
  
}