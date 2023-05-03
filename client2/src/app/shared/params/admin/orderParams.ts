export class orderParams {
     orderNo=0;
     orderDate = new Date();
     customerId: number = 0;
     customerNameLike: string='';
     city ='';
     categoryId = 0;
     
     sort = "orderno";
     pageNumber = 1;
     pageSize = 10;
     search: string='';
}