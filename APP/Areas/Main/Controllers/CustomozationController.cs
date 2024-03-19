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
    public class CustomozationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AircraftController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public CustomozationController(ILogger<AircraftController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<IActionResult> Index(string id)
        {
            if (id != null)
            {
                TempData["airId"] = id;
                AircraftVM aircraftVM = new AircraftVM();
                aircraftVM.Aircrafts = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == Convert.ToInt32(id));
                if (aircraftVM == null)
                {
                    return NotFound();
                }
                return View(aircraftVM);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }         
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            AvailableCustomozation availableCustomozation = new AvailableCustomozation();
            if (id == null)
            {
                //this is for create
              
                return View(availableCustomozation);
            }
            //this is for edit
            availableCustomozation = await _unitOfWork.availableCustomozation.GetAsync(id.GetValueOrDefault());

            if (availableCustomozation == null)
            {
                return NotFound();
            }
            return View(availableCustomozation);

        }       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(AvailableCustomozation availableCustomozation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (availableCustomozation.Id == 0)
                    {
                        //specifcation.AircratfId = Convert.ToInt32(TempData["airId"]);
                        await _unitOfWork.availableCustomozation.AddAsync(availableCustomozation);
                    }
                    else
                    {
                        var obj = await _unitOfWork.availableCustomozation.GetFirstOrDefaultAsync(a => a.Id == availableCustomozation.Id);
                        _unitOfWork.availableCustomozation.Update(availableCustomozation);
                    }
                    _unitOfWork.SaveAsync();
                    //TempData["AlertMsg"] = "Record Created Sucessfully...!";
                    return RedirectToAction("Index", new { id = availableCustomozation.AircratfId});
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                    throw;
                }
                
            }
            return View(availableCustomozation);
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int airid = Convert.ToInt32(TempData.Peek("airId") ?? 0);
            var objList = from a in await _unitOfWork.availableCustomozation.GetAllAsync(includeProperties: "Aircraft")
                          where a.AircratfId == airid
                          select new
                          {
                              a.Id,
                              customzationType = a.CustomzationType,
                              customzationValue = a.CustomzationValue,
                              price=a.Price
                          };
            return Json(new { data = objList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.availableCustomozation.GetAsync(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.availableCustomozation.Remove(objFromDb);
            _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
