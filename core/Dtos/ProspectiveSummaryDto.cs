using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class ProspectiveSummaryDto
    {
        public string CategoryRef { get; set; }
        public string Status {get; set;}
        public string Source { get; set; }
        public DateTime Date { get; set; }

        public int Pending { get; set; }
        public int Concluded { get; set; }
        public int PhoneNoWrong {get; set;}
        public int PhoneNotReachable {get; set;}
        public int NotResponding {get; set;}
        public int NotInterested {get; set;}
        public int ScNotAcceptable {get; set;}
        public int AskedToReachHimLater {get; set;}
        public int PhoneUnanswered {get; set;}
        public int PpIssues {get; set;}
        public int LowSalary {get; set;}
        public int Others {get; set;}
        public int Total {get; set;}
    }
}