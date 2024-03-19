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
    public class TeamController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AircraftController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public TeamController(ILogger<AircraftController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
            Team team = new Team();
            if (id == null)
            {
                //this is for create
              
                return View(team);
            }
            //this is for edit
            team = await _unitOfWork.team.GetAsync(id.GetValueOrDefault());

            if (team == null)
            {
                return NotFound();
            }
            return View(team);

        }       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Team team)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (team.Id == 0)
                    {
                        await _unitOfWork.team.AddAsync(team);
                    }
                    else
                    {
                        var obj = await _unitOfWork.team.GetFirstOrDefaultAsync(a => a.Id == team.Id);
                        _unitOfWork.team.Update(team);
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
            return View(team);
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var objList = await _unitOfWork.team.GetAllAsync();                      
                          
            return Json(new { data = objList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.team.GetAsync(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.team.Remove(objFromDb);
            _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
