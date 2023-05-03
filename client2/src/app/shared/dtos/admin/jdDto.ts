export interface IJDDto {
     id: number;
     orderItemId: number;
     orderId: number;
     orderNo: number;
     orderDate: Date;
     customerName: string;
     categoryName: string;
     jobDescInBrief: string;
     qualificationDesired: string;
     expDesiredMin: number;
     expDesiredMax: number;
     minAge: number;
     maxAge: number;
}
