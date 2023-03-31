using core.Entities.HR;

namespace core.Dtos
{
    public class SelectionDecisionMessageParamDto
    {
        public SelectionDecision SelectionDecision { get; set; }
        public bool DirectlySendMessage { get; set; }=false;
    }
}