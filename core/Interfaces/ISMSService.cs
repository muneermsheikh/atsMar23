namespace core.Interfaces
{
     public interface ISMSService
    {
          string sendMessage(string phoneno, string templateId);
    }
}