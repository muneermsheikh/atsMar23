using System.Text.Json;
using core.Entities;
using core.Entities.AccountsNFinance;
using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Entities.Users;
using Microsoft.Extensions.Logging;

namespace infra.Data
{
     public class ATSContextSeed
    {
        public static async Task SeedAsync(ATSContext context)
        {
            
                //finance
                if (!context.COAs.Any()) {
                    var jsonDatam = File.ReadAllText("../infra/data/SeedData/AccountsCOASeedData.json");
                    var fileDatam = JsonSerializer.Deserialize<List<Coa>>(jsonDatam);
                    foreach(var item in fileDatam) {
                        context.COAs.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                if (!context.DeployStages.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/DeployStageSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<DeployStage>>(jsonData);
                    foreach(var item in fileData) {
                        context.DeployStages.Add(item);
                    }

                    await context.SaveChangesAsync();
                }
                
                if (!context.Deployments.Any()) {
                    var jsnData2 = File.ReadAllText("../infra/data/SeedData/DeploySeedData.json");
                    var fData2 = JsonSerializer.Deserialize<List<Deployment>>(jsnData2);
                    foreach(var item in fData2) {
                        context.Deployments.Add(item);
                    }

                    await context.SaveChangesAsync();
                }
                
                if (!context.Helps.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/HelpSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Help>>(jsonData);
                    foreach(var item in fileData) {
                        context.Helps.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                

                if(!context.FinanceVouchers.Any()) 
                {
                    var jsonD = File.ReadAllText("../infra/data/SeedData/FinanceVoucherSeedData.json");
                    var fileD = JsonSerializer.Deserialize<List<FinanceVoucher>>(jsonD);
                    foreach(var em in fileD) {
                        context.FinanceVouchers.Add(em);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.VoucherEntries.Any())  
                {
                    var jsonD = File.ReadAllText("../infra/data/SeedData/VoucherEntrySeedData.json");
                    var fileD = JsonSerializer.Deserialize<List<VoucherEntry>>(jsonD);
                    foreach(var em in fileD) {
                        context.VoucherEntries.Add(em);
                    }
                    await context.SaveChangesAsync();
                }


                if (!context.ContactResults.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ContactResultSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ContactResult>>(jsonData);
                    foreach(var item in fileData) {
                        context.ContactResults.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                if(!context.Categories.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/CategorySeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Category>>(jsonData);
                    foreach(var item in fileData) {
                        context.Categories.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Industries.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/IndustrySeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Industry>>(jsonData);
                    foreach(var item in fileData) {
                        context.Industries.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Qualifications.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/QualificationSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Qualification>>(jsonData);
                    foreach(var item in fileData) {
                        context.Qualifications.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                if(!context.ReviewItemStatuses.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ReviewItemStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ReviewItemStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.ReviewItemStatuses.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.ReviewStatuses.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ReviewStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ReviewStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.ReviewStatuses.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.SelectionStatuses.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/SelectionStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<SelectionStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.SelectionStatuses.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                /*
                if(!context.DeployStatus.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/DeployStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<DeployStage>>(jsonData);
                    foreach(var item in fileData) {
                        context.DeployStages.Add(item);
                    }
                }
                */
                
                if(!context.MessageComposeSources.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/MessageComposeSourceSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<MessageComposeSource>>(jsonData);
                    foreach(var item in fileData) {
                        context.MessageComposeSources.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.TaskTypes.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/TaskTypeSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<TaskType>>(jsonData);
                    foreach(var item in fileData) {
                        context.TaskTypes.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.MessageTypes.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/MessageTypeSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<MessageType>>(jsonData);
                    foreach(var item in fileData) {
                        context.MessageTypes.Add(item);
                    }
                }

                if(!context.ReviewItemDatas.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ReviewItemDataSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ReviewItemData>>(jsonData);
                    foreach(var item in fileData) {
                        context.ReviewItemDatas.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.ChecklistHRDatas.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/ChecklistHRDataSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<ChecklistHRData>>(jsonData);
                    foreach(var item in fileData) {
                        context.ChecklistHRDatas.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
            

                if(!context.AssessmentStandardQs.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/AssessmentStddQsSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<AssessmentStandardQ>>(jsonData);
                    foreach(var item in fileData) {
                        context.AssessmentStandardQs.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
            
                if(!context.SkillDatas.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/SkillSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<SkillData>>(jsonData);
                    foreach(var item in fileData) {
                        context.SkillDatas.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.InterviewAttendancesStatus.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/InterviewAttendanceStatusSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<InterviewAttendanceStatus>>(jsonData);
                    foreach(var item in fileData) {
                        context.InterviewAttendancesStatus.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Employees.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/EmployeeSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Employee>>(jsonData);
                    foreach(var item in fileData) {
                        context.Employees.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
        
                if(!context.Customers.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/CustomerSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<Customer>>(jsonData);
                    foreach(var item in fileData) {
                        context.Customers.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.CustomerOfficials.Any()) {
                    var jsonData = File.ReadAllText("../infra/data/SeedData/CustomerOfficialSeedData.json");
                    var fileData = JsonSerializer.Deserialize<List<CustomerOfficial>>(jsonData);
                    foreach(var item in fileData) {
                        context.CustomerOfficials.Add(item);
                    }
                }
                

                if(!context.Candidates.Any()) {
                    var jsonData1 = File.ReadAllText("../infra/data/SeedData/UserSeedData.json");
                    var fileData1 = JsonSerializer.Deserialize<List<Candidate>>(jsonData1);
                    foreach(var item in fileData1) {
                        context.Candidates.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                if(!context.ProspectiveCandidates.Any()) {
                    var js = File.ReadAllText("../infra/data/SeedData/ProspectiveCandidateSeedData.json");
                    var fData = JsonSerializer.Deserialize<List<ProspectiveCandidate>>(js);
                    foreach(var item in fData) {
                        context.ProspectiveCandidates.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                

                if(!context.Orders.Any()) 
                {
                    var jsnData = File.ReadAllText("../infra/data/SeedData/OrderSeedData.json");
                    var filData = JsonSerializer.Deserialize<List<Order>>(jsnData);
                    foreach(var item in filData) {
                        context.Orders.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if(!context.Remunerations.Any()) 
                {
                    var jsnData = File.ReadAllText("../infra/data/SeedData/RemunerationSeedData.json");
                    var filData = JsonSerializer.Deserialize<List<Remuneration>>(jsnData);
                    foreach(var item in filData) {
                        context.Remunerations.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                
                if(!context.JobDescriptions.Any()) 
                {
                    var jsnData = File.ReadAllText("../infra/data/SeedData/JobDescriptionSeedData.json");
                    var filData = JsonSerializer.Deserialize<List<JobDescription>>(jsnData);
                    foreach(var item in filData) {
                        context.JobDescriptions.Add(item);
                    }
                    await context.SaveChangesAsync();
                }


                if (context.SelectionDecisions.Any()) {
                    if(!context.SelectionDecisions.Any()) {
                        var jsonData = File.ReadAllText("../infra/data/SeedData/SelectionDecisionSeedData.json");
                        var fileData = JsonSerializer.Deserialize<List<SelectionDecision>>(jsonData);
                        foreach(var item in fileData) {
                            context.SelectionDecisions.Add(item);
                        }

                        if(!context.Employments.Any()) 
                        {
                            var jsonD = File.ReadAllText("../infra/data/SeedData/EmploymentSeedData.json");
                            var fileD = JsonSerializer.Deserialize<List<Employment>>(jsonData);
                            foreach(var em in fileD) {
                                context.Employments.Add(em);
                            }
                        }
                        await context.SaveChangesAsync();
                    }
                }

                //if(context.ChangeTracker.HasChanges()) await context.SaveChangesAsync();

           
        }
    }
}
