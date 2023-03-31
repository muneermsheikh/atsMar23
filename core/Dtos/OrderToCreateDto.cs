using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using core.Entities.Orders;

namespace core.Dtos
{
     public class OrderToCreateDto
     {
          //public int LoggedInAppUserId { get; set; }
          [Required]
          public DateTime OrderDate { get; set; }
          public string OrderRef { get; set; }
          public DateTime OrderRefDate {get; set;}
          [Required]
          public int CustomerId { get; set; }
          public string CustomerName { get; set; }
          public string CityOfEmployment { get; set; }
          public DateTime CompleteBy { get; set; }
          public int? SalesmanId { get; set; }
          [Required]
          public int? ProjectManagerId {get; set;}
          public int? VisaProcessInchargeEmpId {get; set;}
          public int? TravelProcessInchargeId {get; set;}
          public string Remarks { get; set; }
          public ICollection<OrderItemDto> OrderItems { get; set; }
     }
}