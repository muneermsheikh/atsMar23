export interface IDeploymentStatus
{
     id: number;
     stageId: number;
     statusName: string;
     processName: string;
     nextStageId: number;
     workingDaysReqdForNextStage: number;
}