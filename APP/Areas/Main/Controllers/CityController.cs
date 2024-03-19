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
    public class CityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AircraftController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public CityController(ILogger<AircraftController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
            City city = new City();
            if (id == null)
            {
                //this is for create
              
                return View(city);
            }
            //this is for edit
            city = await _unitOfWork.city.GetAsync(id.GetValueOrDefault());

            if (city == null)
            {
                return NotFound();
            }
            return View(city);

        }       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(City city)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (city.Id == 0)
                    {
                        await _unitOfWork.city.AddAsync(city);
                    }
                    else
                    {
                        var obj = await _unitOfWork.city.GetFirstOrDefaultAsync(a => a.Id == city.Id);
                        _unitOfWork.city.Update(city);
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
            return View(city);
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetAllCity()
        {
            var objList = await _unitOfWork.city.GetAllAsync();                      
                          
            return Json(new { data = objList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.city.GetAsync(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.city.Remove(objFromDb);
            _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
