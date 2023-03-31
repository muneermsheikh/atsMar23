using core.Interfaces;

namespace api.Controllers
{

     public class QualificationController : BaseApiController
    {
        private readonly IQualificationService _qService;

        public QualificationController(IQualificationService qService)
        {
            this._qService = qService;
        }
    }
}