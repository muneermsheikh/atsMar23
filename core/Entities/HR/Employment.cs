using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using core.Entities.Orders;

namespace core.Entities.HR
{
    public class Employment: BaseEntity
    {
        public Employment()
        {
        }

          public Employment(int cVRefId, DateTime selectedOn, string salaryCurrency, int salary, 
                int contractPeriodInMonths, bool housingProvidedFree, int housingAllowance, bool foodProvidedFree, 
                int foodAllowance, bool transportProvidedFree, int transportAllowance, int otherAllowance, 
                int leavePerYearInDays, int leaveAirfareEntitlementAfterMonths, int charges )
          {
               CVRefId = cVRefId;
               SelectedOn = selectedOn;
               SalaryCurrency = salaryCurrency;
               Salary = salary;
               ContractPeriodInMonths = contractPeriodInMonths;
               HousingProvidedFree = housingProvidedFree;
               HousingAllowance = housingAllowance;
               FoodProvidedFree = foodProvidedFree;
               FoodAllowance = foodAllowance;
               TransportProvidedFree = transportProvidedFree;
               TransportAllowance = transportAllowance;
               OtherAllowance = otherAllowance;
               LeavePerYearInDays = leavePerYearInDays;
               LeaveAirfareEntitlementAfterMonths = leaveAirfareEntitlementAfterMonths;
          }

            public Employment(int cVRefId, int weeklyhours, DateTime selectedOn, string salaryCurrency, 
                  int salary, int contractPeriodInMonths, bool housingProvidedFree, int housingAllowance, 
                  bool foodProvidedFree, int foodAllowance, bool transportProvidedFree, int transportAllowance, 
                  int otherAllowance, int leavePerYearInDays, int leaveAirfareEntitlementAfterMonths, int charges,
                  int categoryid, int candidateid, int appno, string candidatename, string employername,
                  string categoryname, int orderitemid, int orderid, int orderno, string agentname, int customerid)
            {
                  OrderItemId = orderitemid;
                  OrderId = orderid;
                  OrderNo = orderno;
                  AgentName=agentname;
                  CVRefId = cVRefId;
                  WeeklyHours = weeklyhours;
                  SelectedOn = selectedOn;
                  SalaryCurrency = salaryCurrency;
                  Salary = salary;
                  ContractPeriodInMonths = contractPeriodInMonths;
                  HousingProvidedFree = housingProvidedFree;
                  HousingAllowance = housingAllowance;
                  FoodProvidedFree = foodProvidedFree;
                  FoodAllowance = foodAllowance;
                  TransportProvidedFree = transportProvidedFree;
                  TransportAllowance = transportAllowance;
                  OtherAllowance = otherAllowance;
                  LeavePerYearInDays = leavePerYearInDays;
                  LeaveAirfareEntitlementAfterMonths = leaveAirfareEntitlementAfterMonths;
                  Charges = charges;
                  CategoryId = categoryid;
                  CandidateId = candidateid;
                  ApplicationNo= appno;
                  CandidateName = candidatename;
                  CustomerName = employername;
                  CategoryName = categoryname;
                  CustomerId = customerid;
          }

          public Employment(int cVRefId, int weeklyhours, DateTime selectedOn, string salaryCurrency, int salary, 
                int contractPeriodInMonths, bool housingProvidedFree, int housingAllowance, bool foodProvidedFree, 
                int foodAllowance, bool transportProvidedFree, int transportAllowance, int otherAllowance, 
                int leavePerYearInDays, int leaveAirfareEntitlementAfterMonths, int charges,
                int categoryid, int candidateid, int appno, string candidatename,
                string companyname, int orderid, int orderitemid, int orderno)
          {
               CVRefId = cVRefId;
               WeeklyHours = weeklyhours;
               SelectedOn = selectedOn;
               SalaryCurrency = salaryCurrency;
               Salary = salary;
               ContractPeriodInMonths = contractPeriodInMonths;
               HousingProvidedFree = housingProvidedFree;
               HousingAllowance = housingAllowance;
               FoodProvidedFree = foodProvidedFree;
               FoodAllowance = foodAllowance;
               TransportProvidedFree = transportProvidedFree;
               TransportAllowance = transportAllowance;
               OtherAllowance = otherAllowance;
               LeavePerYearInDays = leavePerYearInDays;
               LeaveAirfareEntitlementAfterMonths = leaveAirfareEntitlementAfterMonths;
                  Charges = charges;
                  CustomerName = companyname;
                  OrderItemId = orderitemid;
                  OrderId = orderid;
                  OrderNo = orderno;
          }

          public int CVRefId { get; set; }
          public int SelectionDecisionId {get; set;}
          public DateTime SelectedOn { get; set; }
          public int Charges {get; set;}
          public string SalaryCurrency { get; set; }
          public int Salary { get; set; }
          [Range(1,36)]
          public int ContractPeriodInMonths { get; set; }=24;
          public int WeeklyHours {get; set;}=48;
          public bool HousingProvidedFree { get; set; }
          public int HousingAllowance { get; set; }
          public bool FoodProvidedFree { get; set; }
          public int FoodAllowance { get; set; }
          public bool TransportProvidedFree { get; set; }
          public int TransportAllowance { get; set; }
          public int OtherAllowance { get; set; }
          public int LeavePerYearInDays { get; set; }
          public int LeaveAirfareEntitlementAfterMonths {get; set;}
          public DateTime OfferAcceptedOn { get; set; }
          public string OfferAttachmentUrl { get; set; }
          public string OfferAcceptanceUrl { get; set; }
          //related - to delete once navigation properties can be activated
          public int CategoryId {get; set;}
          public string CategoryName { get; set; }
          public int OrderItemId { get; set; }
          public int OrderId {get; set;}
          public int OrderNo {get; set;}
          public int CustomerId {get; set;}
          public string CustomerName {get; set;}
          
          public int CandidateId { get; set; }
          public int ApplicationNo { get; set; }
          public string CandidateName { get; set; }
          public string AgentName { get; set; }     //agent name
          public bool Approved { get; set; }
          public int ApprovedByEmpId { get; set; }
          public DateTime ApprovedOn {get; set;}

          //[ForeignKey("SelectionDecisionId")]
          //public SelectionDecision SelectionDecision {get; set;}
      }
}