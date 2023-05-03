import { IMessage } from "../../models/admin/message";

export interface IMessagesDto
{
     emailMessage: IMessage;
     errorMessage: string;
     cvRefIds: number[];
}