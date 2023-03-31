using core.Entities.EmailandSMS;
using core.Params;

 namespace core.Specifications
{
    public class EmailMessagesForUserSpecs: BaseSpecification<EmailMessage>
    {
        public EmailMessagesForUserSpecs(string Container, string username, int pageSize, int pageIndex)
        : base (x =>
                (Container=="Inbox" || x.RecipientEmailAddress.ToLower()==username.ToLower()) &&
                (Container=="Sent" || x.SenderEmailAddress.ToLower()==username.ToLower() ) 
            )
          {
              switch(Container.ToLower()) {
                case "inbox":
                  AddOrderByDescending(x => x.DateReadOn);
                  break;
                case "sent":
                  AddOrderByDescending(x => x.MessageSentOn);
                  break;
                case "outbox":
                case "draft":
                  AddOrderByDescending(x => x.MessageComposedOn);
                  break;
                
                default: 
                  AddOrderBy(x => x.DateReadOn);
                  break;
              }
              
              ApplyPaging(pageSize * (pageIndex - 1), pageSize); 
          }
        
        public EmailMessagesForUserSpecs(EmailMessageSpecParams msgParams)
        : base (x =>
                (msgParams.Container=="Inbox"|| x.RecipientEmailAddress.ToLower()==msgParams.Username.ToLower()) &&
                (msgParams.Container=="Sent" || x.SenderEmailAddress.ToLower()==msgParams.Username.ToLower() ) 
            )
        {
                
              switch(msgParams.Container.ToLower()) {
                case "inbox":
                  AddOrderByDescending(x => x.DateReadOn);
                  break;
                case "sent":
                  AddOrderByDescending(x => x.MessageSentOn);
                  break;
                case "outbox":
                case "draft":
                  AddOrderByDescending(x => x.MessageComposedOn);
                  break;
                
                default: 
                  AddOrderBy(x => x.DateReadOn);
                  break;
              }
              
              ApplyPaging(msgParams.PageSize * (msgParams.PageIndex - 1), msgParams.PageSize);  
        }

    }
}
