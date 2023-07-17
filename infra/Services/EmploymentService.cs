using core.Entities.HR;
using core.Interfaces;
using core.Params;
using core.Dtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace infra.Services
{
     public class EmploymentService : IEmploymentService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          public EmploymentService(IUnitOfWork unitOfWork, ATSContext context)
          {
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public async Task<Employment> AddEmployment(Employment employment)
          {
               _unitOfWork.Repository<Employment>().Add(employment);
               if (await _unitOfWork.Complete() > 0) {
                   var empParams = new EmploymentParams{CvRefId = employment.CVRefId};
                   
                   return employment;
               } else {
                   return null;
               }
          }

          public async Task<bool> DeleteEmployment(int employmentid)
          {
               var employment = await _context.Employments.FindAsync(employmentid);
               if(employment == null) return false;
               
               _unitOfWork.Repository<Employment>().Delete(employment);
               return await _unitOfWork.Complete() > 0;
          }

          public async Task<bool> EditEmployment(Employment employment)
          {
               // todo - verify object
                _unitOfWork.Repository<Employment>().Update(employment);

               return await _unitOfWork.Complete() > 0;
          }

          /*public async Task<Employment> GetEmployment(int CVRefId)
          {
               var emp = await _context.Employments.Where(x => x.CVRefId == CVRefId).FirstOrDefaultAsync();
               if (emp == null) {
                    var cvref = await _context.CVRefs.FindAsync(CVRefId);
                    if (cvref == null) return null;
                    emp = new Employment(CVRefId, DateTime.Now , "", 0, 24, false, 0, false, 0, false, 0,0,21,24,cvref.Charges);
                    _unitOfWork.Repository<Employment>().Add(emp);
                    if (await _unitOfWork.Complete() == 0) return null;
               }
               return emp;
          }
          */

          public async Task<Pagination<Employment>> GetEmployments(EmploymentParams empParams)
          {
               var qry = _context.Employments.AsQueryable();

               if(empParams.OrderId!=0) qry=qry.Where(x => x.OrderId==empParams.OrderId);
               if(empParams.OrderItemId!=0) qry=qry.Where(x => x.OrderItemId==empParams.OrderItemId);
               if(empParams.ApplicationNo > 1) qry=qry.Where(x => x.ApplicationNo==empParams.ApplicationNo);    
                    //with applicationNo parameter 0, the parameter object reeived in controller has all null values  
               if(empParams.CandidateId!=0) qry=qry.Where(x => x.CandidateId==empParams.CandidateId);
               //if(!string.IsNullOrEmpty(empParams.CandidateName)) qry=qry.Where(x => Regex.IsMatch(x.CandidateName, WildCardToRegular("*" + empParams.CandidateName + "*")));
               if(empParams.OrderNo !=0) qry = qry.Where(x => x.OrderNo == empParams.OrderNo);

               if(empParams.Approved != "null") 
                    qry = qry.Where(x => x.Approved==Convert.ToBoolean(empParams.Approved));

               if((empParams.SelectionDateFrom.Year > 2000) && (empParams.SelectionDateUpto.Year > 2000)) {
                    qry = qry.Where(x => x.SelectedOn >= empParams.SelectionDateUpto && x.SelectedOn <= empParams.SelectionDateFrom);
               } else if ((empParams.SelectionDateFrom.Year > 2000) && (empParams.SelectionDateUpto.Year < 2000)) {
                    qry = qry.Where(x => x.SelectedOn == empParams.SelectionDateFrom);
               }

               if(empParams.SelDecisionId !=0) qry = qry.Where(x => x.SelectionDecisionId == empParams.SelDecisionId);

               if(!string.IsNullOrEmpty(empParams.Sort)) {
                    switch (empParams.Sort) {
                         case "appno":
                              qry = qry.OrderBy(x => x.ApplicationNo);
                              break;
                         case "appnodesc":
                              qry = qry.OrderByDescending(x => x.ApplicationNo);
                              break;
                         case "orderno":
                              qry = qry.OrderBy(x => x.OrderNo);
                              break;
                         case "ordernodesc":
                              qry = qry.OrderByDescending(x => x.OrderNo);
                              break;
                         case "orderitem":
                              qry = qry.OrderBy(x => x.OrderItemId);
                              break;
                         case "orderitemdesc":
                              qry = qry.OrderByDescending(x => x.OrderItemId);
                              break;
                         case "candidatename":
                              qry = qry.OrderBy(x => x.CandidateName);
                              break;
                         case "selectedondesc":  
                              qry = qry.OrderByDescending(x => x.SelectedOn);
                              break;
                         default:
                              qry = qry.OrderBy(x => x.SelectedOn);
                              break;
                    }
               }
               var count = await qry.CountAsync();

               if(count == 0) return null;
               
               var data = await qry.Skip((empParams.PageIndex-1)*empParams.PageSize).Take(empParams.PageSize) .ToListAsync();

               return new Pagination<Employment>(empParams.PageIndex, empParams.PageSize, count, data);
                    
          }

          // If you want to implement "*" only
          private static String WildCardToRegular(String value) {
               return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$"; 
          }
          
          /*
          public async Task<Employment> GetEmploymentFromSelId(int Id)
          {
               var sel = await _context.SelectionDecisions.Where(x => x.Id == Id).Include(x => x.Employment).FirstOrDefaultAsync();
               var emp = sel.Employment;
               if (emp == null) {
                    var cvref = await _context.CVRefs.FindAsync(sel.CVRefId);
                    if (cvref == null) return null;
                    emp = new Employment(sel.CVRefId, DateTime.Now , "", 0, 24, false, 0, false, 0, false, 0,0,21,24,cvref.Charges);
                    _unitOfWork.Repository<Employment>().Add(emp);
                    if (await _unitOfWork.Complete() == 0) return null;
               }
               return emp;
          }

          public async Task<ICollection<EmploymentDto>> GetEmploymentDtoFromOrderNo (int orderNo)
          {
               var emps = await (from emp in _context.Employments where emp.OrderNo == orderNo
                    join cv in _context.Candidates on emp.CandidateId equals cv.Id
                    join item in _context.OrderItems on emp.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new EmploymentDto {
                         ApplicationNo = cv.ApplicationNo, 
                         CandidateName = cv.FullName, 
                         CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName,
                         Employment = emp
                         }
                    ).OrderBy(x => x.CategoryRef)
                    .ThenBy(x=> x.ApplicationNo)
                    .ToListAsync();
               return emps;
          }

          
          public async Task<ICollection<EmploymentDto>> GetEmploymentDtoBetwenDates (DateTime fromDate, DateTime uptoDate)
          {
               var emps = await (from emp in _context.Employments 
                         where (DateTime.Compare(emp.SelectedOn, fromDate) >= 0 
                              && DateTime.Compare(emp.SelectedOn, uptoDate) <= 0)
                    join cv in _context.Candidates on emp.CandidateId equals cv.Id
                    join item in _context.OrderItems on emp.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new EmploymentDto {
                         ApplicationNo = cv.ApplicationNo, 
                         CandidateName = cv.FullName, 
                         CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName,
                         Employment = emp
                         }
                    ).OrderBy(x => x.CategoryRef)
                    .ThenBy(x=> x.ApplicationNo)
                    .ToListAsync();
               return emps;
          }

          
          public async Task<ICollection<EmploymentDto>> GetEmploymentDtoFromCVRefId (int cvrefid)
          {
               var emps = await (from emp in _context.Employments where emp.CVRefId == cvrefid
                    join cv in _context.Candidates on emp.CandidateId equals cv.Id
                    join item in _context.OrderItems on emp.OrderItemId equals item.Id
                    join cat in _context.Categories on item.CategoryId equals cat.Id
                    join o in _context.Orders on item.OrderId equals o.Id
                    join c in _context.Customers on o.CustomerId equals c.Id
                    select new EmploymentDto {
                         ApplicationNo = cv.ApplicationNo, 
                         CandidateName = cv.FullName, 
                         CategoryRef = o.OrderNo + "-" + item.SrNo + "-" + cat.Name,
                         CompanyName = c.CustomerName,
                         Employment = emp
                         }
                    ).OrderBy(x => x.CategoryRef)
                    .ThenBy(x=> x.ApplicationNo)
                    .ToListAsync();
               return emps;
          }
          */


     }
}