using System.ComponentModel.DataAnnotations;

namespace core.Entities.MasterEntities
{
    public class ChecklistHRData: BaseEntity
    {
          public ChecklistHRData()
          {
          }

          public ChecklistHRData(int srno, string parameter)
          {
               SrNo = srno;
               Parameter = parameter;
          }

          [Range(1,1000)]
        public int SrNo {get; set;}
        public string Parameter { get; set; }
    }
}