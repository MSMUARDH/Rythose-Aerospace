using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace APP.Areas.Main.Controllers
{
    [Area("Main")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PrintController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public ReportController(ILogger<PrintController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public IActionResult OrderReport()
        {
            return View();
        }

        public IActionResult AircraftReport()
        {
            return View();
        }

        public IActionResult PaymentReport()
        {
            return View();
        }

        public IActionResult AnalysisReport()
        {
            return View();
        }

        public async Task<IActionResult> GetAllOrders()
        {
            var objList = from ord in await _unitOfWork.OrderDetails.GetAllAsync()
                          join or in await _unitOfWork.Order.GetAllAsync() on ord.OrderId equals or.Id
                          join us in await _unitOfWork.ApplicationUser.GetAllAsync() on or.UserId equals us.Id
                          join ar in await _unitOfWork.aircraft.GetAllAsync() on ord.AircraftId equals ar.AircratfId
                          join shd in await _unitOfWork.shippingDetail.GetAllAsync() on or.ShippingID equals shd.ShippingID
                          join co in await _unitOfWork.city.GetAllAsync() on shd.CityId equals co.Id
                          select new
                          {
                              orderNo=or.Id,
                              orderDate=or.OrderDate.ToShortDateString(),
                              userName=us.Email,
                              aircraftName=ar.Name,
                              qty=ord.Qty,
                              grandTotal=or.GrandTotal,
                              shippingCountry=co.Name,
                              deliveryDate=or.DeliveryDate.HasValue ?or.DeliveryDate.Value.ToShortDateString():"" ,
                              orderStatus=or.OrderStatus

                          };


            var data = objList;
            return Json(new { data });

        }

        [HttpGet]
        public async Task<IActionResult> GetAllAircraft()
        {
            var objList = await _unitOfWork.aircraft.GetAllAsync();
            var aircraftlist = objList.Select(item => new
            {
                url = item.ImgUrl,
                code = item.Code,
                name = item.Name,
                category = item.Category.ToString(), // Convert enum to string
                model = item.Model,
                price = item.Price,
                qty = item.Qty,
            });
            return Json(new { data = aircraftlist });
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentDetails()
        {
            var payments = (from p in await _unitOfWork.payment.GetAllAsync()
                            join o in await _unitOfWork.Order.GetAllAsync() on p.OrdersID equals o.Id
                            join u in await _unitOfWork.ApplicationUser.GetAllAsync() on o.UserId equals u.Id
                            select new
                            {
                                payId = p.PaymentID,
                                payDate = p.PaymentOn.ToShortDateString(),
                                user = u.Email,
                                totalAmount=o.TotAmount,
                                shippingAmount=o.ShippingAmount,
                                grandTotal=o.GrandTotal
                            }).ToList();
            return Json(new { data = payments });
        }

        [HttpPost]
        public async Task<List<object>> GetOrderCount()
        {
            try
            {
                List<object> data = new List<object>();
                var orderList = (from o in await _unitOfWork.Order.GetAllAsync()
                                 join sh in await _unitOfWork.shippingDetail.GetAllAsync() on o.ShippingID equals sh.ShippingID
                                 join c in await _unitOfWork.city.GetAllAsync() on sh.CityId equals c.Id
                                 select new
                                 {
                                     cityid = c.Id,
                                     cityname = c.Name
                                 }).ToList();

                var chartData = orderList
                        .GroupBy(r => r.cityname)
                        .Select(g => new { cityname = g.Key, TotalReq = g.Count() })
                        .ToList();
                List<string> city = chartData.Select(x => x.cityname).ToList();
                data.Add(city);
                List<string> reqcount = chartData.Select(x => x.TotalReq.ToString()).ToList();
                data.Add(reqcount);

                return data;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
        }
    }
}
