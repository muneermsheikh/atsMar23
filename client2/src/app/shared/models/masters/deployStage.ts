export interface IDeployStage
{
     id: number;
     sequence: number;
     status: string;
     //processName: string;
     estimatedDaysToCompleteThisStage: number;
     nextSequence: number;
}