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
    public class AircrafImagesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AircraftController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public AircrafImagesController(ILogger<AircraftController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
            AircrafImages aircrafImages = new AircrafImages();
            if (id == null)
            {
                //this is for create
              
                return View(aircrafImages);
            }
            //this is for edit
            aircrafImages = await _unitOfWork.aircrafImages.GetAsync(id.GetValueOrDefault());

            if (aircrafImages == null)
            {
                return NotFound();
            }
            return View(aircrafImages);

        }       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(List<IFormFile> files,AircrafImages aircrafImages )
        {
                    if (ModelState.IsValid)
                    {
                        using (var transaction = await _unitOfWork.BeginTransactionAsync())
                        {
                            try
                            {
                                if(aircrafImages.Id ==0)
                                {

                            foreach (var item in files)
                            {
                                string webRootPath = _hostEnvironment.WebRootPath;
                                string fileName = Guid.NewGuid().ToString();
                                var uploads = Path.Combine(webRootPath, @"img\aircraft");
                                var extenstion = Path.GetExtension(item.FileName);

                                using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                                {
                                    item.CopyTo(filesStreams);
                                }

                                var newImage = new AircrafImages
                                {
                                    ImgUrl = @"\img\aircraft\" + fileName + extenstion,
                                    AircratfId = aircrafImages.AircratfId
                                };

                                await _unitOfWork.aircrafImages.AddAsync(newImage);
                            }

                        }
                        else
                                {
                                    string webRootPath = _hostEnvironment.WebRootPath;
                            var newfiles = HttpContext.Request.Form.Files;
                            if (newfiles.Count > 0)
                                    {                                       
                                        if (aircrafImages.ImgUrl != null)
                                        {

                                            var imagePath = Path.Combine(webRootPath, aircrafImages.ImgUrl.TrimStart('\\'));
                                            if (System.IO.File.Exists(imagePath))
                                            {
                                                System.IO.File.Delete(imagePath);
                                            }
                                        }
                                
                                    string fileName = Guid.NewGuid().ToString();
                                    var uploads = Path.Combine(webRootPath, @"img\aircraft");
                                    var extenstion = Path.GetExtension(files[0].FileName);

                                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                                    {
                                        newfiles[0].CopyTo(filesStreams);
                                    }
                                    aircrafImages.ImgUrl = @"\img\aircraft\" + fileName + extenstion;
                                    _unitOfWork.aircrafImages.Update(aircrafImages);
                                
                                       
                                    }
                                    var aircraftsimg = await _unitOfWork.aircrafImages.GetFirstOrDefaultAsync(a => a.Id == aircrafImages.Id);


                        }
                                _unitOfWork.SaveAsync();
                                 _unitOfWork.CommitAsync(transaction);
                        return RedirectToAction("Index", new { id = aircrafImages.AircratfId });
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                                throw;
                        //_unitOfWork.RollbackAsync(transaction);
                        //return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                        }
                    }
                    return View(aircrafImages);
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int airid = Convert.ToInt32(TempData.Peek("airId") ?? 0);
            var objList = from a in await _unitOfWork.aircrafImages.GetAllAsync(includeProperties: "Aircraft")
                          where a.AircratfId == airid
                          select new
                          {
                              a.Id,
                              a.ImgUrl 
                          };
            return Json(new { data = objList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.aircrafImages.GetAsync(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.aircrafImages.Remove(objFromDb);
            _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
