namespace core.Dtos
{
    public class LoggedInUserDto
    {
        public string LoggedInAppUserId { get; set; }
        public string LoggedIAppUsername { get; set; }
        public string LoggedInAppUserEmail { get; set; }
        public int LoggedInEmployeeId { get; set; }
        public bool HasAdminPrivilege { get; set; }
    }
}