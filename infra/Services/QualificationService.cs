using core.Entities.MasterEntities;
using core.Interfaces;
using core.Params;
using core.Dtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class QualificationService: IQualificationService
	{
            private readonly IUnitOfWork _unitOfWork;
            private readonly ATSContext _context;
		public QualificationService(IUnitOfWork unitOfWork, ATSContext context)
		{
                  //this._qService = qService;
                  _context = context;
                  _unitOfWork = unitOfWork;
		}

		public async Task<Qualification> AddQualification(string qualificationName)
		{
			var entity = new Qualification(qualificationName);
               _unitOfWork.Repository<Qualification>().Add(entity);
               if (await _unitOfWork.Complete() > 0) return entity;
               return null;
		}

		public async Task<bool> DeleteQualificationAsync(Qualification qualification)
		{
			_unitOfWork.Repository<Qualification>().Delete(qualification);
               return (await _unitOfWork.Complete() > 0);
		}

		public async Task<bool> EditQualificationAsync(Qualification qualification)
		{
			 _unitOfWork.Repository<Qualification>().Update(qualification);
               return (await _unitOfWork.Complete() > 0);
		}

		public async Task<ICollection<Qualification>> GetListAsync()
		{
			var lst = await _context.Qualifications.OrderBy(x => x.Name).ToListAsync();
            return lst;
		}

		public async Task<Pagination<Qualification>> GetQualificationPaginated(QualificationSpecParams specParams)
		{
			    var spec = new QualificationSpecs(specParams);
               var specCount = new QualificationForCountSpecs(specParams);
               var totalCount = await _unitOfWork.Repository<Qualification>().CountAsync(specCount);
               var lst = await _unitOfWork.Repository<Qualification>().ListAsync(spec);

               return new Pagination<Qualification>(specParams.PageIndex, specParams.PageSize, totalCount, lst);

		}

	}
}