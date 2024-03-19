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
    public class AircraftController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AircraftController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public AircraftController(ILogger<AircraftController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SetQty()
        {
            return View();
        }
        public IActionResult Create()
        {
            var enumData = from AircraftCat e in Enum.GetValues(typeof(AircraftCat))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");
            return View();
        }

        public async Task<IActionResult> AddQty(string id)
        {
            var book = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == Convert.ToInt32(id));
            if (book == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQty(Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var air = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == aircraft.AircratfId);


                        air.Qty = aircraft.Qty;
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);
                        return RedirectToAction(nameof(SetQty));
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                }
            }

            return View(aircraft);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        string webRootPath = _hostEnvironment.WebRootPath;
                        var files = HttpContext.Request.Form.Files;
                        if (files.Count > 0)
                        {
                            string fileName = Guid.NewGuid().ToString();
                            var uploads = Path.Combine(webRootPath, @"img\aircraft");
                            var extenstion = Path.GetExtension(files[0].FileName);

                            //if (books.ImgUrl != null)
                            //{
                            //    //this is an edit and we need to remove old image
                            //    var imagePath = Path.Combine(webRootPath, books.ImgUrl.TrimStart('\\'));
                            //    if (System.IO.File.Exists(imagePath))
                            //    {
                            //        System.IO.File.Delete(imagePath);
                            //    }
                            //}

                            using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                            {
                                files[0].CopyTo(filesStreams);
                            }
                            aircraft.ImgUrl = @"\img\aircraft\" + fileName + extenstion;
                        }
                        //var aircrafts = new Aircraft()
                        //{
                        //    ImgUrl = aircraft.ImgUrl,
                        //    Code = aircraft.Code,                           
                        //    Name = aircraft.Name,
                        //    Model = aircraft.Model,
                        //    Category = aircraft.Category,
                        //    Qty = aircraft.Qty,
                        //    Price = aircraft.Price,
                        //    Description= aircraft.Description,
                        //    CurStatus =SD.Active

                        //};

                        // await _unitOfWork.aircraft.AddAsync(aircraft);
                        // _unitOfWork.SaveAsync();
                        //_unitOfWork.CommitAsync(transaction);
                        await _unitOfWork.aircraft.AddAsync(aircraft);
                         _unitOfWork.SaveAsync();
                         _unitOfWork.CommitAsync(transaction);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                    
                }
            }

            return View(aircraft);
        }
    
        public async Task<IActionResult> EditAircraft(string id)
        {
            var aircraft = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == Convert.ToInt32(id));
            if (aircraft == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var enumData = from AircraftCat e in Enum.GetValues(typeof(AircraftCat))
                           select new
                           {
                               ID = (int)e,
                               Name = e.ToString()
                           };
            SelectList enumList = new SelectList(enumData, "ID", "Name", (int)aircraft.Category);
            ViewBag.EnumList = enumList;
            return View(aircraft);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAircraft(Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        string webRootPath = _hostEnvironment.WebRootPath;
                        var files = HttpContext.Request.Form.Files;
                        if (files.Count > 0)
                        {
                            string fileName = Guid.NewGuid().ToString();
                            var uploads = Path.Combine(webRootPath, @"img\aircraft");
                            var extenstion = Path.GetExtension(files[0].FileName);

                            if (aircraft.ImgUrl != null)
                            {
                                
                                var imagePath = Path.Combine(webRootPath, aircraft.ImgUrl.TrimStart('\\'));
                                if (System.IO.File.Exists(imagePath))
                                {
                                    System.IO.File.Delete(imagePath);
                                }
                            }
                            using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                            {
                                files[0].CopyTo(filesStreams);
                            }
                            aircraft.ImgUrl = @"\img\aircraft\" + fileName + extenstion;
                        }
                        var aircrafts = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == aircraft.AircratfId);

                        //aircraft.ImgUrl = aircrafts.ImgUrl;
                        //aircraft.Code = aircrafts.Code;
                        //aircraft.Model = aircrafts.Model;
                        //aircraft.Name = aircrafts.Name;
                        //aircraft.Price = aircrafts.Price;
                        //aircraft.Description = aircrafts.Description;
                        //aircraft.Category = aircraft.Category;
                        //aircraft.Qty = aircraft.Qty;
                        //aircraft.CurStatus = aircraft.CurStatus;
                        _unitOfWork.aircraft.Update(aircraft);
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                }
            }

            return View(aircraft);
        }

        public async Task<IActionResult> ViewAircraft(string id)
        {

            var aircraft = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == Convert.ToInt32(id));
            var images = await _unitOfWork.aircrafImages.GetAllAsync(x => x.AircratfId == Convert.ToInt32(id));
            var aircraftVM = new AircraftVM()
            {
                Aircrafts = aircraft,
                AircrafImages = images
            };
            if (aircraftVM == null)
            {
                return RedirectToAction(nameof(Index));
            }
           
            return View(aircraftVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewAircraft(Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        
                        var aircrafts = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == aircraft.AircratfId);


                        aircraft.CurStatus = aircraft.CurStatus==SD.Active?SD.DeActive:SD.Active;
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                }
            }

            return View(aircraft);
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var objList = await _unitOfWork.aircraft.GetAllAsync();
            var aircraftlist = objList.Select(item => new
            {
                code = item.Code,
                name = item.Name,
                category = item.Category.ToString(), // Convert enum to string
                model = item.Model,
                price = item.Price,
                qty = item.Qty,
                aircratfId = item.AircratfId
            });
            return Json(new { data = aircraftlist });
        }

        [HttpGet]
        public async Task<IActionResult> GetAircraftCustom(int airid)
        {          
            var objList = from a in await _unitOfWork.availableCustomozation.GetAllAsync(x=>x.AircratfId== airid, includeProperties: "Aircraft")
                          where a.AircratfId == airid
                          select new
                          {
                              a.Id,
                              customzationType = a.CustomzationType,
                              customzationValue = a.CustomzationValue,
                              price = a.Price
                          };
            return Json(new { data = objList });
        }

        [HttpGet]
        public async Task<IActionResult> GetAircraftSpe(int airid)
        {
           
            var objList = from a in await _unitOfWork.specifcation.GetAllAsync(x=>x.AircratfId==airid,includeProperties: "Aircraft")
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
            var objFromDb = await _unitOfWork.aircraft.GetAsync(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.aircraft.Remove(objFromDb);
            _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion

    }
}
