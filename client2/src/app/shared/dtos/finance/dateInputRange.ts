export interface IDateInputRange
{
	fromDate: Date,
	uptoDate: Date
}

export class DateInputRange implements IDateInputRange
{
	fromDate = new Date();
	uptoDate= new Date();
}