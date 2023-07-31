using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.Users;
using core.Interfaces;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Excel = Microsoft.Office.Interop.Excel;       //Microsoft Excel 14 object in references-> COM tab


namespace infra.Services
{
     public class ExcelService : IExcelService
	{
        private readonly ATSContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserService _userService;
		
        public const int colDate=1;
        public const int colSource=2;
        public const int colCategoryRef=4;



        public ExcelService(ATSContext context, UserManager<AppUser> userManager, IUserService userService)
		{
            _userService = userService;
            _userManager = userManager;
            _context = context;
		}

	
        public async Task<string> ReadAndSaveApplicationsXLToDb(string filePathName, int userid, string username) // [FromBody] FileToUpload theFile) 
        {
            //var prospectives = new List<ProspectiveCandidate>();
        
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@filePathName);
            Excel._Worksheet xlWorksheet = (Excel._Worksheet)xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            
            var rangeHeader = (Excel.Range)xlWorksheet.Columns["A:A"];
            var result = xlRange.Find("Name", LookAt: Excel.XlLookAt.xlWhole);
            
            var address = result.Address;//cell address, gets absolute address like $a$2

            var value = result.Value2;//cell value

            if(string.IsNullOrEmpty((string)value)) return "failed to locate required Column Caption 'Name' in the worksheet";
            
            int dataRow = Convert.ToInt32(address.Substring(3));
            
            if(dataRow > 10) {
                closeExcelApplication(xlApp, xlWorkbook);
                return "The column title 'Name' is too down in the sheet";
            }

            int constHeaderValueCol=7;

            var OrderItemId= ((Excel.Range)xlWorksheet.Cells[3, constHeaderValueCol]).Value2;
            int orderitemid = Convert.ToInt32(OrderItemId);
            
            if(orderitemid==0 || !await OrderItemIdIsValidForProspective(orderitemid)) {
                closeExcelApplication(xlApp, xlWorkbook);    
                return "Invalid Order Item Id";
            }

            var dated = ((Excel.Range)xlWorksheet.Cells[1, constHeaderValueCol]).Value;
            var Source= ((Excel.Range)xlWorksheet.Cells[2, constHeaderValueCol]).Value2;
            
            var CategoryRef= ((Excel.Range)xlWorksheet.Cells[4, constHeaderValueCol]).Value2;     //row, column

            string strError="";

            int totRecordsOk=0;
            
            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            
            for (int row = dataRow+1; row <= xlWorksheet.UsedRange.Rows.Count; row++)
            {
                
                var emailValue =(string)((Excel.Range)xlWorksheet.Cells[row, 12]).Value;        //excel column index begins from 1
                if(!string.IsNullOrEmpty(emailValue)) {
                    var existingAppUser = await _userManager.FindByEmailAsync((string)emailValue);
                    if(existingAppUser != null) continue;       //IdentityUser exists.  return error value
                }
                
                var aadharno = (string)((Excel.Range)xlWorksheet.Cells[6, 7]).Value;
                if(!string.IsNullOrEmpty(aadharno)) {
                    if(await _userService.CheckCandidateAadharNoExists(aadharno)) continue; //return error string
                }
                
                var ppno = (string)((Excel.Range)xlWorksheet.Cells[7, 8]).Value;
                if (!string.IsNullOrEmpty(ppno) && await _userService.CheckPPNumberExists(ppno) != "") continue;
                
                var newCandidate = new Candidate();
                newCandidate.Created=DateTime.Now;
                newCandidate.CandidateStatus=(int)EnumCandidateStatus.NotReferred;
                newCandidate.ApplicationNo=GetNewApplication();
                newCandidate.LastActive=DateTime.Now;

                var userphones = new List<UserPhone>();
                var userProfs = new List<UserProfession>();
                var userQs = new List<UserQualification>();

                for (int col = 1; col <= xlWorksheet.UsedRange.Columns.Count; col++)
                {
                        var val=(string) Convert.ToString(((Excel.Range)xlWorksheet.Cells[row, col]).Value2);

                        object _ColumnName;
                        
                        if(!string.IsNullOrEmpty(val)) {
                            _ColumnName= ((Excel.Range)xlWorksheet.Cells[3, col]).Value2; //itle is in row 9
                            switch(_ColumnName) {
                                case "Gender":
                                    newCandidate.Gender=val.Substring(0,1);
                                    break;
                                case "First Name":
                                    newCandidate.FirstName= TrimIfNecessary(val, 15);
                                    break;
                                case "Second Name":
                                    newCandidate.SecondName=TrimIfNecessary(val, 50);
                                    break;
                                case "Family Name":
                                    newCandidate.FamilyName=TrimIfNecessary(val, 50);
                                    break;
                                case "Known As":
                                    newCandidate.KnownAs=TrimIfNecessary(val, 10);
                                    break;
                                case "Referred By":
                                    newCandidate.ReferredBy=Convert.ToInt32(val);
                                    break;
                                case "Source":
                                    newCandidate.Source=TrimIfNecessary(val, 15);
                                    break;
                                case "Place Of Birth":
                                    newCandidate.PlaceOfBirth=TrimIfNecessary(val,15);
                                    break;
                                case "Mobile":
                                    userphones.Add(new UserPhone(val,true));
                                    break;
                                case "Mobile 2":
                                    userphones.Add(new UserPhone(val,false));
                                    break;
                                case "Mobile 3":
                                    userphones.Add(new UserPhone(val,false));
                                    break;
                                
                                case "Aadhar No":
                                    newCandidate.AadharNo=val;
                                    break;
                                case "Passport No":
                                    newCandidate.PpNo=val;
                                    break;
                                case "ECNR":
                                    newCandidate.Ecnr=val;
                                    break;
                                case "Address":
                                    newCandidate.Address=TrimIfNecessary(val,150);
                                    break;
                                case "City":
                                    newCandidate.City =val;
                                    break;
                                case "District":
                                    newCandidate.District=val;
                                    break;
                                case "PIN":
                                    newCandidate.Pin=val;
                                    break;
                                case "Nationality":
                                    newCandidate.Nationality=val;
                                    break;
                                case "Email":
                                    newCandidate.Email=emailValue;
                                    break;
                                case "CompanyId":
                                    newCandidate.CompanyId=Convert.ToInt32(val);
                                    break;
                                case "Agent":
                                    int ind = val.IndexOf("|");
                                    if(ind != -1)newCandidate.ReferredBy=Convert.ToInt32(val.Substring(ind+1,val.Length-ind));
                                    break;
                                case "Category":
                                    int i = val.IndexOf("|");
                                    if(i!=-1) userProfs.Add(new UserProfession(i,val.Substring(0,i-1),0,true));
                                    break;
                                
                                case "Notification Desired":
                                    newCandidate.NotificationDesired=Convert.ToBoolean(val);
                                    break;
                               
                                case "Qualification":
                                    int j = val.IndexOf("|");
                                    if(j!=-1) userQs.Add(new UserQualification(j,true));
                                    break;
                                default:
                                    break;
                        }
                    }
                }
                totRecordsOk++;
                if(userProfs.Count > 0) newCandidate.UserProfessions=userProfs;
                if(userphones.Count > 0) newCandidate.UserPhones=userphones;
                if(userQs.Count > 0) newCandidate.UserQualifications=userQs;
                //prospectives.Add(newProspect);
                _context.Candidates.Add(newCandidate);
            }
            
            if(totRecordsOk==0) {
                closeExcelApplication(xlApp, xlWorkbook);
                strError="None of the records found valid to save to Database";
                return strError;
            }
            bool succeeded=false;

            try {
                succeeded = await _context.SaveChangesAsync() == 0;
            } catch (Exception ex) {
                strError = "Failed to Copy Candidates to database" + ex.Message;
            }

            closeExcelApplication(xlApp, xlWorkbook);

            return strError;
        }

        private int GetNewApplication()
        {
            return 0;
        }
		public async Task<string> ReadAndSaveProspectiveXLToDb(string filePathName, int userid, string username)
		{
			
            //valid column names in the sheet
            //Gender, Resume Id, Resume Title, Name, Age, Mobile No, Alternate Contact No,
            //Alternate Number, Current Location, Address, Work Experience, Email Id,
            //Alternate Email Id, Date of Birth
            //ORDER ITEM ID IS IN ROW 3, COL 7

            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@filePathName);
            Excel._Worksheet xlWorksheet = (Excel._Worksheet)xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            
            var rangeHeader = (Excel.Range)xlWorksheet.Columns["A:A"];
            var result = xlRange.Find("Name", LookAt: Excel.XlLookAt.xlWhole);
            
            var address = result.Address;//cell address, gets absolute address like $a$2

            var value = result.Value2;//cell value

            if(string.IsNullOrEmpty((string)value)) return "failed to locate required Column Caption 'Name' in the worksheet";
            
            int dataRow = Convert.ToInt32(address.Substring(3));
            
            if(dataRow > 10) {
                closeExcelApplication(xlApp, xlWorkbook);
                return "The column title 'Name' is too down in the sheet";
            }

            int constHeaderValueCol=20;
            //the OrderItemId value is in row 3, column 20
            var strOrderItemId= ((Excel.Range)xlWorksheet.Cells[3, constHeaderValueCol]).Value2.ToString();
            if(string.IsNullOrEmpty( strOrderItemId)) {
                closeExcelApplication(xlApp, xlWorkbook);    
                return "Order Item Id not available in column 3 or row 3";
            }
            int orderitemid = Convert.ToInt32(strOrderItemId);
            
            if(orderitemid==0 || !await OrderItemIdIsValidForProspective(orderitemid)) {
                closeExcelApplication(xlApp, xlWorkbook);    
                return "Invalid Order Item Id";
            }
            
            var dated = ((Excel.Range)xlWorksheet.Cells[colDate, constHeaderValueCol]).Value;
            var Source= ((Excel.Range)xlWorksheet.Cells[colSource, constHeaderValueCol]).Value2;
            
            var CategoryRef= ((Excel.Range)xlWorksheet.Cells[colCategoryRef, constHeaderValueCol]).Value2;     //row, column

            string strError="";

            bool NotOk=false;
            int totRecordsOk=0;

            try{
                NotOk = Convert.ToDateTime(dated).Year<2000 
                        || string.IsNullOrEmpty((string)Source)
                        || orderitemid==0 
                        || string.IsNullOrEmpty((string) CategoryRef);
            } catch (Exception ex) {
                        closeExcelApplication(xlApp, xlWorkbook);
                        strError = "Header information in the Excel sheet not correct or not provided" + ex.Message;
            } 
            if(NotOk) strError="Header Informtion incorrect or not provided";
            if(!string.IsNullOrEmpty(strError)) {
                closeExcelApplication(xlApp, xlWorkbook);
                return strError;
            }

            /*string ColumnNamearray="Name,Gender,Mobile No.,Alternate Contact No.,Work experience,Current Location,Age,Email,Alternate Email Id.,Date of Birth,Resume Id";
            string DbFieldNameArray="FirstName,Gender,PhoneNo,AlternatePhoneNo,WorkExperience,City,Age,Email,AlternateEmail,DateOfBirth,Mobile,ResumeId";
            string[] columnNames = ColumnNamearray.Split(",");
            string[] dbFIeldNames = DbFieldNameArray.Split(",");
            */
            
            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            
            for (int i = dataRow+1; i <= xlWorksheet.UsedRange.Rows.Count; i++)
            {
                var newProspect = new ProspectiveCandidate();
                newProspect.CategoryRef = (string)CategoryRef;
                newProspect.OrderItemId= orderitemid;
                newProspect.Source = (string)Source;
                newProspect.Date= (DateTime?)dated;
                newProspect.StatusByUserId=userid;
                //newProspect.UserName=username;
                newProspect.Status="Pending";

                for (int j = 1; j <= xlWorksheet.UsedRange.Columns.Count; j++)
                {
                        var val= Convert.ToString(((Excel.Range)xlWorksheet.Cells[i, j]).Value2);

                        object _ColumnName;
                        
                        if(!string.IsNullOrEmpty((string)val)) {
                            _ColumnName= ((Excel.Range)xlWorksheet.Cells[9, j]).Value2; //itle is in row 9
                            switch(_ColumnName) {
                                case "Gender":
                                    newProspect.Gender=val.Substring(0,1);
                                    break;
                                case "Resume Id":
                                    newProspect.ResumeId= TrimIfNecessary(val, 15);
                                    break;
                                case "Resume Title":
                                    newProspect.ResumeTitle=TrimIfNecessary(val, 50);
                                    break;
                                case "Name":
                                    newProspect.CandidateName=TrimIfNecessary(val, 50);
                                    break;
                                case "Age":
                                    newProspect.Age=TrimIfNecessary(val, 10);
                                    break;
                                case "Mobile No.":
                                    newProspect.PhoneNo=TrimIfNecessary(val, 15);
                                    break;
                                case "Alternate Contact No.":
                                case "Alternate Number":
                                    newProspect.AlternatePhoneNo=TrimIfNecessary(val, 15);
                                    break;
                                case "Current Location":
                                    newProspect.CurrentLocation=val;
                                    newProspect.City=val;
                                    break;
                                case "Address":
                                    newProspect.Address=val;
                                    break;
                                case "Work Experience":
                                    newProspect.WorkExperience=val;
                                    break;
                                case "Email Id":
                                    newProspect.Email=val;
                                    break;
                                case "Alternate Email Id.":
                                    newProspect.AlternateEmail=val;
                                    break;
                                case "Date of Birth":
                                    var dob = Convert.ToDateTime(val);
                                    if(dob.Year < 1900) newProspect.DateOfBirth=dob;
                                    break;
                                default:
                                    break;
                            }
                        }
                        
                }
                if(string.IsNullOrEmpty(newProspect.PhoneNo)
                        || string.IsNullOrEmpty(newProspect.CategoryRef)
                        || string.IsNullOrEmpty(newProspect.Source)
                        || string.IsNullOrEmpty(newProspect.ResumeId)
                        || string.IsNullOrEmpty(newProspect.CandidateName)
                        || Convert.ToDateTime(newProspect.Date).Year < 2000
                        || string.IsNullOrEmpty(newProspect.City)
                        ) continue;
                totRecordsOk++;
                //prospectives.Add(newProspect);
                _context.ProspectiveCandidates.Add(newProspect);
            }
            
            if(totRecordsOk==0) {
                closeExcelApplication(xlApp, xlWorkbook);
                strError="None of the records found valid to save to Database";
                return strError;
            }
            bool succeeded=false;

            try {
                succeeded = await _context.SaveChangesAsync() == 0;
            } catch (Exception ex) {
                strError = "Failed to Copy Prospective Excel Sheet to database" + ex.Message;
            }

            closeExcelApplication(xlApp, xlWorkbook);

            return strError;
		}

        private async Task<bool> OrderItemIdIsValidForProspective(int orderitemid)
        {
            var qry = await(from i in _context.OrderItems where i.Id == orderitemid 
                select new{i.Status}).FirstOrDefaultAsync();
            if(qry==null) return false;
            if(qry.Status.ToLower() != "under process") return false;
            return true;
        }
        
        private string TrimIfNecessary(string str, int maxLength) {
            if(str.Length > maxLength) {
                return str.Substring(0, maxLength);
            } else {
                return str;
            }
        }
    
          private bool closeExcelApplication(Excel.Application xlApp, Excel.Workbook wbook) {
            wbook.Close(false);
            xlApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            return true;
        }
	}
}