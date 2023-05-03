export class userHistoryParams {
     id? = 0;
     userHistoryHeaderId?= 0;
     personType = '';
     personName = '';
     personId? = 0;
     customerOfficialId = 0;
     applicationNo? = 0;
     aadharNo = '';
     emailId = '';
     mobileNo = '';
     dateAdded: Date=new Date();
     categoryRef = '';
     status = '';
     concluded = null;        //default
     createNewIfNull=false;
     search = '';
     sort = '';
     pageNumber=1;
     pageSize=10;
}