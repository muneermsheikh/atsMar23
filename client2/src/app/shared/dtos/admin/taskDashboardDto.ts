export interface ITaskDashboardDto
{
	applicationTaskId: number;
	taskTypeId: number;
	taskDate: Date;
	taskOwnerId: number;
	taskOwnerName: string;
	assignedToId: number;
	assignedToName: string;
	taskDescription: string;
	completeBy: Date;
	taskStatus: string;
}