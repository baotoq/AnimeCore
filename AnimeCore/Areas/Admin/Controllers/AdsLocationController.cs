using System.Threading.Tasks;
using AnimeCore.Common;
using Entities.Domain;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace AnimeCore.Areas.Admin.Controllers
{
    public class AdsLocationController : AdminController
    {
        private readonly IAdsLocationRepository _adsLocationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdsLocationController(IUnitOfWork unitOfWork, IAdsLocationRepository adsLocationRepository)
        {
            _unitOfWork = unitOfWork;
            _adsLocationRepository = adsLocationRepository;
        }

        public IActionResult Index()
        {
            var model = _adsLocationRepository.GetAll();
            return View(model);
        }

        public IActionResult Add()
        {
            ViewData["Action"] = "Add";
            var model = new AdsLocation();
            return PartialView("_AddEditPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AdsLocation model)
        {
            ViewData["Action"] = "Add";
            if (ModelState.IsValid)
            {
                await _adsLocationRepository.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();
                return JsonStatus.Ok;
            }
            return PartialView("_AddEditPartial", model);
        }
    }
}