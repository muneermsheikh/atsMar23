using core.Entities;
using core.Entities.HR;
using core.Entities.Orders;
using core.Entities.Users;
using core.Entities.EmailandSMS;
using core.Entities.Tasks;
using core.Entities.Process;
using core.Entities.Admin;
using core.Dtos;
using api.Extensions;
using AutoMapper;
using api.DTOs;

namespace api.Helpers
{
     public class MappingProfiles : Profile
     {
          public MappingProfiles()
          {
               CreateMap<ApplicationTask, TaskDashboardDto>();
               CreateMap<ChecklistHR, ChecklistHRDto>();
               CreateMap<ChecklistHRDto, ChecklistHR>();
               CreateMap<Customer, CustomerTypeNameKnownAsOfficialsToReturnDto>();
               CreateMap<CustomerOfficialToCreateDto, CustomerOfficial>()
                    .ForMember(d => d.CustomerId, o => o.MapFrom(s => s.CompanyId))
                    .ForMember(d => d.AppUserId, o => o.MapFrom(s => 0));
                    
               CreateMap<Customer, CustomerDto>();
               //CreateMap<CVRef, CVRefDto>();
               CreateMap<CVRef, CVRefAndDeployDto>();
                    //.ForMember(d => d.PictureUrl, o => o.MapFrom<ProductUrlResolver>());
               CreateMap<CVRef, CVRefPostedDto>();
               CreateMap<CVRef, SelectionsPendingDto>();

               CreateMap<Deploy, DeploymentObjDto>()
                    .ForMember(d => d.NextStageName, o => o.MapFrom(s => s.NextStageId.GetEnumDisplayName()))
                    .ForMember(d => d.StageName, o => o.MapFrom(s => s.StageId.GetEnumDisplayName()));
               CreateMap<EmailMessage, MessageDto>();
               CreateMap<Employee, EmployeeBriefDto>();
               CreateMap<Employment, EmploymentDto>();
               CreateMap<EmployeeToAddDto, EmployeeDto>();
                    
               CreateMap<OrderItem, OrderItemDto>();
               CreateMap<OrderItem, OrderItemBriefDto>();
                    //.ForMember(d => d.OrderNo, o => o.MapFrom(s => s.Order.OrderNo)),
                    //.ForMember(d => d.OrderDate, o => o.MapFrom(s => s.Order.OrderDate))
                    //.ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Order.Customer.CustomerName)),
                    //.Formember(d => d.CategoryRef, o => o.MapFrom(s => s.Order.OrderNo + s.SrNo + s.Category.Name));

               CreateMap<OrderItem, OrderItemToReturnDto>();
               CreateMap<Order, OrderToReturnDto>();
               CreateMap<Order,OrderBriefDto>();
                    /*
                    .ForMember(d => d.ReviewStatus, o => o.MapFrom(s => EnumReviewStatus.GetAttribute<s.ContractReviewId>()))
                    .ForMember(d => d.Status, o => o.MapFrom(s => EnumOrderStatus.GetAttribute<s.Status>()));
                    */
               CreateMap<ProspectiveCandidate, ProspectiveCandidateEditDto>()
                    .ForMember(d => d.Checked, o => o.MapFrom(s => false))
                    .ForMember(d => d.NewStatus, o => o.MapFrom(s => ""))
                    .ForMember(d => d.Remarks, o => o.MapFrom(s => ""));
               
               CreateMap<ProspectiveCandidate, ProspectiveSummaryDto>();
                    
               CreateMap<Remuneration, RemunerationDto>();
               CreateMap<RemunerationFullDto,Remuneration>();
               
               CreateMap<JDDto, JobDescription>();
               
               CreateMap<SelectionDecision, SelectionDecisionToReturnDto>();
               
               /*
               CreateMap<RegisterDto, Candidate>()
                    .ForMember(d => d.FirstName, o => o.MapFrom(s => s.Address.FirstName))
                    .ForMember(d => d.SecondName, o => o.MapFrom(s => s.Address.SecondName))
                    .ForMember(d => d.FamilyName, o => o.MapFrom(s => s.Address.FamilyName))
                    .ForMember(d => d.KnownAs, o => o.MapFrom(s => s.DisplayName))
                    .ForMember(d => d.DOB, o => o.MapFrom(s => s.Address.DOB))
                    ;
               */
               CreateMap<Candidate, CandidateBriefDto>();

               
               //CreateMap<Address, EntityAddress>();
               CreateMap<SelectionDecision, SelectionDecisionToRegisterDto>();

               CreateMap<ContractReview, ContractReviewDto>();
                    //.ForMember(d => d.ReviewStatus, o => o.MapFrom(s => s.ReviewStatus.Status));
               CreateMap<ContractReviewItem, ContractReviewItemDto>();

               CreateMap<CreateSelDecision, RejDecisionToAddDto>();
               CreateMap<CVRef, DeploymentPendingDto>()
                    .ForMember(d => d.CVRefId, o => o.MapFrom(s => s.Id));
                    //.ForMember(d => d.DeployStageId, o => o.MapFrom(s => EnumDeployStatus.GetAttribute<s.DeployStageId>()));
               CreateMap<Deploy, DeployAddedDto>();
               CreateMap<Deploy, DeployRefDto>();

               CreateMap<UserProfession, Prof>();

               CreateMap<UserHistory, UserHistoryDto>();
               CreateMap<UserHistoryItem, UserHistoryItemDto>();
               //itnerviews

          }
     }
}