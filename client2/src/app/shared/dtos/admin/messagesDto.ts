import { IMessage } from "../../models/admin/message";

export interface IMessagesDto
{
     emailMessage: IMessage;
     errorString: string;
     cvRefIds: number[];
}