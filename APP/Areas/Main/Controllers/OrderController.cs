using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Utility;

namespace APP.Areas.Main.Controllers
{
    [Area("Main")]
    [Authorize(Roles = SD.RoleAdmin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        public OrderController(ILogger<OrderController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PayOrder()
        {
            return View();
        }
        public async Task<IActionResult> ViewOrder(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                TempData["orderId"] = id;
                var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(id),includeProperties: "User,ShippingDetail");
                
                return View(order);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateJob(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                TempData["orderId"] = id;
                var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(id), includeProperties: "User,ShippingDetail");

                return View(order);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> PayOrderRcpt(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(id),includeProperties:"User");
                
                return View(order);
            }
            return RedirectToAction("Index");
        }

        public async Task<JsonResult> AddTeamToJob(string fetch,DateTime selectedDate)
        {
            try
            {
                
                int teamid = fetch.Trim() != "" ? Convert.ToInt32(fetch.Trim()) : 0;
                int orderid = Convert.ToInt32(TempData.Peek("orderId") ?? 0);

                if (teamid > 0 && orderid > 0)
                {
                    var orderInfo = await _unitOfWork.Order.GetAsync(orderid);
                    var teamInfo = await _unitOfWork.team.GetAsync(teamid);
                    var objFromDb = await _unitOfWork.job.GetAllAsync(a => a.OrderId ==orderid);
                    var duduplicateRec = await _unitOfWork.job.GetAllAsync(a => a.OrderId == orderid && a.TeamId == teamid && a.CurStatus==SD.Active);
                    int emprecodecount = objFromDb.Count();
                    //check duduplicate Record
                    if (!duduplicateRec.Any())
                    {
                        
                                //Job create
                                Job job = new Job()
                                {
                                    OrderId=orderid,
                                    TeamId=teamid,
                                    CurStatus=SD.Active
                                };
                                await _unitOfWork.job.AddAsync(job);
                                _unitOfWork.SaveAsync();

                                //Update order tbl
                                orderInfo.OrderStatus = SD.JobAssigned;
                                orderInfo.DeliveryDate = selectedDate;
                                _unitOfWork.Order.Update(orderInfo);
                                _unitOfWork.SaveAsync();

                                //Update team tbl
                                teamInfo.CurStatus = SD.JobAssigned;
                                _unitOfWork.team.Update(teamInfo);
                                _unitOfWork.SaveAsync();

                        await SendJobAssignedEmail(orderInfo, teamInfo, job);

                        //Email to customer


                        return Json(new { success = true, message = "Job Added Successfully" });                          
                    }
                    return Json(new { success = false, message = "Team Already Added" });
                }
                else
                {
                    return Json(new { success = false, message = "Faild To Add Team" });
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
            return Json(new { success = false, message = "Faild To Add Caregiver" });
        }

        private async Task SendJobAssignedEmail(Orders order, Team team, Job job)
        {
            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == order.UserId);

            MailMessage message = new MailMessage();
            message.From = new MailAddress("hhealthcareplus@gmail.com");
            message.To.Add(user.Email);
            message.Subject = "Team Assigned to Your Order";
            string body = $"Dear {user.FirstName} {user.LastName},\n\n";
            body += $"We have assigned a team to your order (Order ID: {order.Id}).\n";
            body += $"The team assigned is: {team.Name}\n";
            body += $"Your Job Number: {job.Id}\n\n";
            body += "Thank you for choosing our service.";
            message.Body = body;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("hhealthcareplus@gmail.com", "blgatarjczxjdsrw");
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);
        }

        private async Task SendOrderStatusEmail(string orderNo,string userId,string orStatus)
        {
            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == userId);

            MailMessage message = new MailMessage();
            message.From = new MailAddress("hhealthcareplus@gmail.com");
            message.To.Add(user.Email);
            message.Subject = "Order Status Update";
            string body = $"Dear {user.FirstName} {user.LastName},\n\n";
            body += $"Your order with ID {orderNo} is now in the {orStatus} stage.";            
            message.Body = body;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("hhealthcareplus@gmail.com", "blgatarjczxjdsrw");
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayOrderRcpt(Orders orders)
        {
            var ord = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == orders.Id, includeProperties: "User");
            if (ModelState.IsValid)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        ord.TotPaid = ord.TotAmount;
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);
                        return RedirectToAction(nameof(PayOrder));
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                }
            }

            return View(ord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewOrder(Orders orders)
        {
            var ord = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == orders.Id, includeProperties: "User");
            if (ModelState.IsValid)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {                   
                        ord.OrderStatus = orders.OrderStatus;
                        _unitOfWork.SaveAsync();
                       await SendOrderStatusEmail(ord.Id.ToString(), ord.User.Id, ord.OrderStatus);

                        if (ord.OrderStatus==SD.Delivered)
                        {
                            var joninfo = await _unitOfWork.job.GetFirstOrDefaultAsync(x => x.OrderId == ord.Id);
                            joninfo.CurStatus = SD.JobClosed;
                            _unitOfWork.job.Update(joninfo);
                            _unitOfWork.SaveAsync();

                            var teaminfo = await _unitOfWork.team.GetFirstOrDefaultAsync(x => x.Id == joninfo.TeamId);
                            teaminfo.CurStatus = SD.Active;
                            _unitOfWork.team.Update(teaminfo);
                            _unitOfWork.SaveAsync();

                        }

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

            return View(ord);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveTeamToJob(int id)
        {
            int orderid = Convert.ToInt32(TempData.Peek("orderId") ?? 0);

            var objFromDb = await _unitOfWork.job.GetFirstOrDefaultAsync(a => a.TeamId == id && a.OrderId == orderid);
            var jobCount = await _unitOfWork.job.GetFirstOrDefaultAsync(x => x.OrderId == orderid);
            var teamInfo = await _unitOfWork.team.GetAsync(objFromDb.TeamId);
            var orderInfo = await _unitOfWork.Order.GetFirstOrDefaultAsync(x => x.Id == orderid);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            //Remove the team into the Job
            _unitOfWork.job.Remove(objFromDb);
            _unitOfWork.SaveAsync();

            //Update the team status as "Active"
            teamInfo.CurStatus = SD.Active;
            _unitOfWork.team.Update(teamInfo);
            _unitOfWork.SaveAsync();

            //Update the order status as "Active"
            orderInfo.OrderStatus = SD.Active;
            _unitOfWork.Order.Update(orderInfo);
            _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #region Data Loads
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var objList = from a in await _unitOfWork.Order.GetAllAsync(includeProperties: "User")
                          select new
                          {
                              user = a.User.UserName,
                              orderno=a.Id,
                              date = a.OrderDate.ToShortDateString(),
                              totAmt= a.TotAmount,
                              paidAmt = a.TotAmount,
                              orderStatus= a.OrderStatus,
                              curStatus= a.CurStatus,
                              a.Id
                          };
          

            var data = objList;
            return Json(new { data});
            
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrder()
        {
            var objList = from a in await _unitOfWork.Order.GetAllAsync(includeProperties: "User")
                          where a.CurStatus == SD.Active && a.TotAmount>a.TotPaid && a.OrderStatus==SD.Delivered
                          select new
                          {
                              user = a.User.UserName,
                              date = a.OrderDate,
                              totAmt = a.TotAmount,
                              paidAmt = a.TotAmount,
                              orderStatus = a.OrderStatus,
                              curStatus = a.CurStatus,
                              a.Id
                          };


            var data = objList;
            return Json(new { data });

        }
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailAll(string id)
        {
            var objList = from a in await _unitOfWork.OrderDetails.GetAllAsync(includeProperties: "Aircraft")
                          where a.OrderId==Convert.ToInt32(id)
                          select new
                          {
                              url=a.Aircraft.ImgUrl,
                              code = a.Aircraft.Code,
                              name = a.Aircraft.Name,
                              model = a.Aircraft.Model,
                              category = a.Aircraft.Category.ToString(),
                              qty = a.Qty,

                          };
            return Json(new { data = objList });
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveTeams()
        {
            var objList = await _unitOfWork.team.GetAllAsync(x=>x.CurStatus==SD.Active);

            return Json(new { data = objList });
        }

        [HttpGet]
        public async Task<IActionResult> GetJobAssignedTeams()
        {
            int orderid = Convert.ToInt32(TempData.Peek("orderId") ?? 0);
            List<int> teamList = new List<int>();
            teamList = (await _unitOfWork.job.GetAllAsync(a => a.OrderId == orderid)).Select(x => x.TeamId).ToList();
            var alf = await _unitOfWork.team.GetAllAsync(x => teamList.Contains(x.Id));
            return Json(new { data = alf });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderCustomDetail()
        {
            int orderid = Convert.ToInt32(TempData.Peek("orderId") ?? 0);
            var events = (from ord in await _unitOfWork.OrderDetails.GetAllAsync()
                          join or in await _unitOfWork.Order.GetAllAsync() on ord.OrderId equals or.Id
                          join orcu in await _unitOfWork.orderCustomozationDetails.GetAllAsync() on ord.Id equals orcu.OrderDetailsId
                          join avcust in await _unitOfWork.availableCustomozation.GetAllAsync() on orcu.AvailableCustomozationId equals avcust.Id
                          where or.Id== orderid
                          select new
                          {
                              customzationType= avcust.CustomzationType,
                              customzationValue= avcust.CustomzationValue

                          }).ToList();

            return Json(new { data = events });
        }

        #endregion
    }
}
