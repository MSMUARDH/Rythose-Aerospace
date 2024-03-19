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
    public class SpecifcationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AircraftController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public SpecifcationController(ILogger<AircraftController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
            Specifcation specifcation = new Specifcation();
            if (id == null)
            {
                //this is for create
              
                return View(specifcation);
            }
            //this is for edit
            specifcation = await _unitOfWork.specifcation.GetAsync(id.GetValueOrDefault());

            if (specifcation == null)
            {
                return NotFound();
            }
            return View(specifcation);

        }       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Specifcation specifcation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (specifcation.Id == 0)
                    {
                        //specifcation.AircratfId = Convert.ToInt32(TempData["airId"]);
                        await _unitOfWork.specifcation.AddAsync(specifcation);
                    }
                    else
                    {
                        var obj = await _unitOfWork.specifcation.GetFirstOrDefaultAsync(a => a.Id == specifcation.Id);
                        _unitOfWork.specifcation.Update(specifcation);
                    }
                    _unitOfWork.SaveAsync();
                    //TempData["AlertMsg"] = "Record Created Sucessfully...!";
                    return RedirectToAction("Index", new { id =specifcation.AircratfId});
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                    throw;
                }
                
            }
            return View(specifcation);
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int airid = Convert.ToInt32(TempData.Peek("airId") ?? 0);
            var objList = from a in await _unitOfWork.specifcation.GetAllAsync(includeProperties: "Aircraft")
                          where a.AircratfId == airid
                          select new
                          {
                              a.Id,
                              title = a.Title,
                              description = a.Description, 
                          };
            return Json(new { data = objList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.specifcation.GetAsync(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.specifcation.Remove(objFromDb);
            _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
