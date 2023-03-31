namespace core.Interfaces
{
     public interface IExcelService
    {
        Task<string> ReadAndSaveProspectiveXLToDb(string filePathName, int userid, string username);
        Task<string> ReadAndSaveApplicationsXLToDb(string filePathName, int userid, string username);
    }
}