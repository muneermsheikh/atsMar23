import { IProspectiveCandidate } from "./prospectiveCandidate";

export interface IProspectiveHeader
{
	categoryRef: string;
	orderIemId: number;
	source: string;
	date?: Date;
	userName: string;
	prospectiveCandidates: IProspectiveCandidate[];
}