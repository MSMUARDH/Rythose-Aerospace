using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Models;
using Models.Enum;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace APP.Areas.Main.Controllers
{

    [Area("Main")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ShippingSetController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AircraftController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public ShippingSetController(ILogger<AircraftController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ShippingSetVM shippingSetVM = new ShippingSetVM()
            {
                ShippingSet = new ShippingSet(),
                CityList = (await _unitOfWork.city.GetAllAsync()).Select(i => new SelectListItem
                {
                    Text=i.Name,
                    Value= i.Id.ToString()
                })
            };
            if (id == null)
            {
                //this is for create
                return View(shippingSetVM);
            }

            //this is for edit
            shippingSetVM.ShippingSet = await _unitOfWork.shippingSet.GetAsync(id ?? 0);
           
            if (shippingSetVM.ShippingSet == null)
            {
                return NotFound();
            }
            return View(shippingSetVM);

        }       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ShippingSetVM shippingSetVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (shippingSetVM.ShippingSet.ShippingSetId == 0)
                    {
                        await _unitOfWork.shippingSet.AddAsync(shippingSetVM.ShippingSet);
                    }
                    else
                    {
                        var obj = await _unitOfWork.shippingSet.GetFirstOrDefaultAsync(a => a.ShippingSetId == shippingSetVM.ShippingSet.ShippingSetId);
                        _unitOfWork.shippingSet.Update(shippingSetVM.ShippingSet);
                    }
                    _unitOfWork.SaveAsync();
                    //TempData["AlertMsg"] = "Record Created Sucessfully...!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                    throw;
                }
                
            }
            return View(shippingSetVM);
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetShippingSet()
        {
            var objList = (from sh in await _unitOfWork.shippingSet.GetAllAsync()
                           join c in await _unitOfWork.city.GetAllAsync() on sh.CityId equals c.Id
                           select new
                           {
                               shippingid = sh.ShippingSetId,
                               cityName = c.Name,
                               amount = sh.Amount
                           }).ToList();
            return Json(new { data = objList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.shippingSet.GetAsync(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.shippingSet.Remove(objFromDb);
            _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
