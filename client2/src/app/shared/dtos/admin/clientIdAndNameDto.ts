export interface IClientIdAndNameDto
{
	customerId: number;
	customerName: string;
	knownAs: string;
}

export class ClientIdAndNameDto implements IClientIdAndNameDto
{
	customerId = 0;
	customerName = '';
	knownAs = '';
}