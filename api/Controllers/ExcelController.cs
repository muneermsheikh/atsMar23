//using System.Web.Http;

//using Microsoft.AspNetCore.Http.HttpContext;
using System;
using System.IO;
using core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

     public class ExcelController : BaseApiController
    {
        private readonly IExcelService _excelServices;
        Microsoft.Office.Interop.Excel.Application _objexcel;
        public ExcelController(IExcelService excelServices, Microsoft.Office.Interop.Excel.Application objexcel)
        {
            _excelServices = excelServices;
            _objexcel = objexcel;
        }
        
      
    
    }


}