using core.Entities.Users;

namespace core.Dtos
{
    public class CandidateMessageParamDto
    {
        public Candidate Candidate { get; set; }
        public bool DirectlySendMessage { get; set; }
    }
}