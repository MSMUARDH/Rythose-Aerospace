using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using APP.Session;
using Utility;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using Models.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APP.Areas.Public.Controllers
{
    [Area("Public")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;
        public readonly IWebHostEnvironment _hostEnvironment;
        private readonly ISession _session;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment,IHttpContextAccessor httpContext)
        {
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _session = httpContext.HttpContext.Session;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Aircraft> productList = await _unitOfWork.aircraft.GetAllAsync(a=>a.CurStatus==SD.Active);
           
            if (productList !=null)
            {
                return View(productList);
            }
           
            return View();

        }
		//public async Task<IActionResult> ShopPage()
		//{
		//    IEnumerable<Aircraft> productList = await _unitOfWork.aircraft.GetAllAsync(a => a.CurStatus == SD.Active);

		//    var enumData = from AircraftCat e in Enum.GetValues(typeof(AircraftCat))
		//                   select new
		//                   {
		//                       ID = (int)e,
		//                       Name = e.ToString()
		//                   };
		//    ViewBag.EnumList = new SelectList(enumData, "ID", "Name");

		//    if (productList != null)
		//    {
		//        return View(productList);
		//    }

		//    return View();

		//}

		//[Authorize(Roles = SD.RoleCustomer)]


		//public async Task<IActionResult> ShopPage(string catID)
		//{
		//	IEnumerable<Aircraft> productList;

		//	if (!string.IsNullOrEmpty(catID))
		//	{
		//		if (Enum.TryParse(catID, out AircraftCat selectedCategory))
		//		{
		//			productList = await _unitOfWork.aircraft.GetAllAsync(a => a.CurStatus == SD.Active && a.Category == selectedCategory);
		//		}
		//		else
		//		{
		//			productList = await _unitOfWork.aircraft.GetAllAsync(a => a.CurStatus == SD.Active);
		//		}
		//	}
		//	else
		//	{
		//		productList = await _unitOfWork.aircraft.GetAllAsync(a => a.CurStatus == SD.Active);
		//	}

		//	var enumData = from AircraftCat e in Enum.GetValues(typeof(AircraftCat))
		//				   select new
		//				   {
		//					   ID = (int)e,
		//					   Name = e.ToString()
		//				   };
		//	ViewBag.EnumList = new SelectList(enumData, "ID", "Name");

		//	if (productList != null)
		//	{
		//		return View(productList);
		//	}

		//	return View();
		//}

		public async Task<IActionResult> ShopPage(string catID, double? minPrice, double? maxPrice)
		{
			IEnumerable<Aircraft> productList;

			if (!string.IsNullOrEmpty(catID))
			{
				if (Enum.TryParse(catID, out AircraftCat selectedCategory))
				{
					productList = await _unitOfWork.aircraft.GetAllAsync(a => a.CurStatus == SD.Active && a.Category == selectedCategory);
				}
				else
				{
					productList = await _unitOfWork.aircraft.GetAllAsync(a => a.CurStatus == SD.Active);
				}
			}
			else
			{
				productList = await _unitOfWork.aircraft.GetAllAsync(a => a.CurStatus == SD.Active);
			}

			if (minPrice != null && maxPrice != null)
			{
				// Filter the productList based on price range
				productList = productList.Where(a => a.Price >= minPrice && a.Price <= maxPrice);
			}

			var enumData = from AircraftCat e in Enum.GetValues(typeof(AircraftCat))
						   select new
						   {
							   ID = (int)e,
							   Name = e.ToString()
						   };
			ViewBag.EnumList = new SelectList(enumData, "ID", "Name");

			if (productList != null)
			{
				return View(productList.ToList());
			}

			return View();
		}






		[Authorize(Roles = SD.RoleCustomer)]






		public async Task<IActionResult> AddToCart(string Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var aircrafts = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == Convert.ToInt32(Id));
            var availableCust = await _unitOfWork.availableCustomozation.GetAllAsync(x => x.AircratfId == Convert.ToInt32(Id));
            var spe = await _unitOfWork.specifcation.GetAllAsync(x => x.AircratfId == Convert.ToInt32(Id));
            var images = await _unitOfWork.aircrafImages.GetAllAsync(x => x.AircratfId == aircrafts.AircratfId);

            //var cmnt = await _unitOfWork.Comments.GetAllAsync(a => a.BookId == books.Id,includeProperties:"User");
            var crtVM = new CartVM()
            {
                Aircrafts = aircrafts,
                Availablecustomozation = availableCust,
                Specifcation = spe,
                AircrafImages =images
                //Comments=cmnt

            };
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var crt = await _unitOfWork.Cart.GetFirstOrDefaultAsync(a => a.UserId == claim.Value && a.CurStatus == SD.Active);
                    if (crt == null)
                    {
                        var cart = new Carts()
                        {
                            CurStatus = SD.Active,
                            UserId = claim.Value,
                        };
                        await _unitOfWork.Cart.AddAsync(cart);
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);
                        crtVM.Carts = cart;
                    }
                    else
                    {
                        crtVM.Carts = crt;
                    }

                }
                catch (Exception)
                {
                    _unitOfWork.RollbackAsync(transaction);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            return View(crtVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(CartVM cartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (cartVM.CartDetails.Qty>0)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var cartDetails = new CartDetails()
                        {
                            CartId = cartVM.Carts.Id,
                            AircraftId = cartVM.Aircrafts.AircratfId,
                            Qty = cartVM.CartDetails.Qty,
                            Price = cartVM.CartDetails.Price                                                      
                        };
                        await _unitOfWork.CartDetails.AddAsync(cartDetails);
                        _unitOfWork.SaveAsync();
                        var cartCutdetails = new List<CartCustomozationDetails>() {
                             new CartCustomozationDetails
                             {
                                 CartDetailsId = cartDetails.Id,
                                 AvailableCustomozationId=cartVM.CartDetails.ColorId
                             },
                             new CartCustomozationDetails
                             {
                                 CartDetailsId = cartDetails.Id,
                                 AvailableCustomozationId=cartVM.CartDetails.SeatId
                             }
                        };
                      

                        await _unitOfWork.CartCustomozationDetails.AddRangeAsync(cartCutdetails);
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);

                        await SetSession(claim);
                        return RedirectToAction("ViewCart");
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            else
            {
                return RedirectToAction("AddToCart", new { Id = cartVM.Aircrafts.AircratfId });
            }


        }
        

        [Authorize(Roles=SD.RoleCustomer)]
        public async Task<IActionResult> ViewCart()
        {
            var crtVM = new CartVM();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var carts = await _unitOfWork.Cart.GetFirstOrDefaultAsync(a => a.UserId == claim.Value && a.CurStatus == SD.Active);
            if (carts != null)
            {
                var cartDt = await _unitOfWork.CartDetails.GetAllAsync(a => a.CartId == carts.Id, includeProperties: "Aircraft");
                var cust = await _unitOfWork.CartCustomozationDetails.GetAllAsync(x => x.CartDetails.CartId == carts.Id, includeProperties: "AvailableCustomozation,CartDetails");
                if (cartDt != null)
                {
                    crtVM.CartDetailList = cartDt;
                    crtVM.Carts = carts;
                    crtVM.Availablecustomozation = cust.Select(s => s.AvailableCustomozation);
                    return View(crtVM);
                }
                
            }

           return View(crtVM);
            
          
        }

        //public async Task<IActionResult> AddCmt(string cmt, string bookId)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            var cmnt = new Comments()
        //            {
        //                BookId = Convert.ToInt32(bookId),
        //                UserId=claim.Value,
        //                Comment=cmt
        //            };
        //            await _unitOfWork.Comments.AddAsync(cmnt);
        //             _unitOfWork.SaveAsync();
        //             _unitOfWork.CommitAsync(transaction);
        //            return RedirectToAction("AddToCart", new { Id = bookId });
        //        }
        //        catch (Exception)
        //        {
        //            _unitOfWork.RollbackAsync(transaction);
        //            return StatusCode(StatusCodes.Status500InternalServerError);
        //        }
        //    }
        //}
        [Authorize(Roles = SD.RoleCustomer)]
        public async Task<IActionResult> UserDetails()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(a => a.Id == claim.Value);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDetails(ApplicationUser usr)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(a => a.Id == claim.Value);
            if (ModelState.IsValid)
            {
                
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        user.Address = usr.Address;
                        user.FirstName = usr.FirstName;
                        user.LastName = usr.LastName;
                        user.UpdatedBy = user.UserName;
                        user.UpdatedOnUTC = DateTime.UtcNow;
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            else
            {
                return View(usr);
            }


        }


        [Authorize(Roles=SD.RoleCustomer)]
        public async Task<IActionResult> Order()
        {
            var crtVM = new CartVM();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var carts = await _unitOfWork.Cart.GetFirstOrDefaultAsync(a => a.UserId == claim.Value && a.CurStatus == SD.Active);           
            if (carts != null)
            {
                var cartDt = await _unitOfWork.CartDetails.GetAllAsync(a => a.CartId == carts.Id, includeProperties: "Aircraft");
                var cust = await _unitOfWork.CartCustomozationDetails.GetAllAsync(x => x.CartDetails.CartId == carts.Id, includeProperties: "AvailableCustomozation,CartDetails");
                if (cartDt != null)
                {
                    crtVM.CartDetailList = cartDt;
                    crtVM.Carts = carts;
                    crtVM.Availablecustomozation = cust.Select(s => s.AvailableCustomozation);
                    crtVM.CityList = (await _unitOfWork.city.GetAllAsync()).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });

                    return View(crtVM);
                }
                
            }

           return View(crtVM);          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(CartVM cartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(a => a.Id == claim.Value);
            if (!string.IsNullOrEmpty(user.Address))
            {
                double totAmt = 0; 
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var shipping = new ShippingDetail()
                        {
                            FirstName = cartVM.ShippingDetail.FirstName,
                            LastName = cartVM.ShippingDetail.LastName,
                            Email = cartVM.ShippingDetail.Email,
                            Mobile = cartVM.ShippingDetail.Mobile,
                            Address = cartVM.ShippingDetail.Address,
                            CityId = cartVM.ShippingDetail.CityId, //this is for Country
                            Country = cartVM.ShippingDetail.Country,
                            PostCode = cartVM.ShippingDetail.PostCode,
                        };
                        await _unitOfWork.shippingDetail.AddAsync(shipping);
                        _unitOfWork.SaveAsync();
                        
                        int savedShippingId = shipping.ShippingID;

                        var cartDetails = await _unitOfWork.CartDetails.GetAllAsync(a => a.CartId == cartVM.Carts.Id, includeProperties: "Aircraft");
                       

                        var order = new Orders()
                        {
                            CurStatus = SD.Active,
                            OrderDate = DateTime.UtcNow,
                            OrderStatus = "PEN",
                            TotAmount = cartVM.Order.TotAmount,
                            UserId = claim.Value,
                            ShippingID= savedShippingId,
                            ShippingAmount=cartVM.Order.ShippingAmount,
                            GrandTotal=cartVM.Order.GrandTotal
                            
                        };
                        await _unitOfWork.Order.AddAsync(order);
                        _unitOfWork.SaveAsync();
                       
                        int savedOrderId = order.Id;

                        foreach (var item in cartDetails)
                        {
                            var cartCustDetails = await _unitOfWork.CartCustomozationDetails.GetAllAsync(a => a.CartDetailsId ==item.Id);
                            var orderDetails = new OrderDetails()
                            {
                                AircraftId = item.AircraftId,
                                OrderId = order.Id,
                                Qty = item.Qty,
                                Price =item.Price

                            };
                            totAmt = totAmt + item.Qty * item.Aircraft.Price;
                            await _unitOfWork.OrderDetails.AddAsync(orderDetails);

                            var aircraft = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == item.AircraftId);
                            aircraft.Qty = aircraft.Qty - item.Qty;

                            _unitOfWork.SaveAsync();
                            List<OrderCustomozationDetails> orderCustomozations = new List<OrderCustomozationDetails>();
                            foreach (var item2 in cartCustDetails)
                            {
                                var orderCust = new OrderCustomozationDetails()
                                {
                                    AvailableCustomozationId = item2.AvailableCustomozationId,
                                    OrderDetailsId = orderDetails.Id
                                };

                                orderCustomozations.Add(orderCust);
                            }
                            await _unitOfWork.orderCustomozationDetails.AddRangeAsync(orderCustomozations);

                            _unitOfWork.SaveAsync();
                        }

                       

                        //var odr = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == order.Id);
                       
                        //var cart = await _unitOfWork.Cart.GetFirstOrDefaultAsync(a => a.Id == cartVM.Carts.Id);
                        //cart.CurStatus = SD.DeActive;
                        _unitOfWork.SaveAsync();
                        _unitOfWork.CommitAsync(transaction);

                        await SetSession(claim);
                        ///_session.Remove<CartDetails>("Cart");


                        return RedirectToAction("RoadToPayment", new { orderId = savedOrderId, shippingId = savedShippingId });
                    }
                    catch (Exception ex)
                    {

                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            else
            {
                return RedirectToAction("UserDetails");
            }


        }

       
        [Authorize(Roles = SD.RoleCustomer)]
        public async Task<IActionResult> RoadToPayment(int orderId,int shippingId)
        {
           
            var crtVM = new CartVM();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var carts = await _unitOfWork.Cart.GetFirstOrDefaultAsync(a => a.UserId == claim.Value && a.CurStatus == SD.Active);
            var customer = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == claim.Value);
            if (carts != null)
            {
                var cartDt = await _unitOfWork.CartDetails.GetAllAsync(a => a.CartId == carts.Id, includeProperties: "Aircraft");
                var cust = await _unitOfWork.CartCustomozationDetails.GetAllAsync(x => x.CartDetails.CartId == carts.Id, includeProperties: "AvailableCustomozation,CartDetails");
                var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(x =>x.Id==orderId);
                var shipping = await _unitOfWork.shippingDetail.GetAllAsync(x => x.ShippingID == shippingId, includeProperties: "City");
                string HasKey= await getHas(order.Id);
                if (cartDt != null)
                {
                    crtVM.CartDetailList = cartDt;
                    crtVM.Carts = carts;
                    crtVM.Availablecustomozation = cust.Select(s => s.AvailableCustomozation);
                    crtVM.Order = order;
                    crtVM.ShippingDetailList = shipping;
                    crtVM.HasKey = HasKey;
                    crtVM.CustName = customer.FirstName +' '+ customer.LastName;
                    return View(crtVM);
                }

            }

            return View(crtVM);
        }


        [Authorize(Roles = SD.RoleCustomer)]
        public async Task<IActionResult> AfterPayment(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(a => a.Id == claim.Value);
           

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var payment = new Payment()
                        {
                           PaymentType= "Card Payment",
                           PaymentOn=DateTime.Now,
                           PaymentBy = claim.Value,
                           OrdersID= id,
                        };
                        await _unitOfWork.payment.AddAsync(payment);
                        _unitOfWork.SaveAsync();

                        int savedPaymentId = payment.PaymentID;

                        var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(x => x.Id == id);
                        

                        //order.PaymentID = savedPaymentId;
                        order.OrderStatus = SD.Active;
                        _unitOfWork.Order.Update(order);

                         _unitOfWork.SaveAsync();


                    var cart = await _unitOfWork.Cart.GetFirstOrDefaultAsync(x => x.UserId == user.Id);
                    var cartdetail = await _unitOfWork.CartDetails.GetAllAsync(x => x.CartId == cart.Id);
                    foreach (var item in cartdetail)
                    {
                        await _unitOfWork.CartDetails.Remove(Convert.ToInt32(item.Id));
                        _unitOfWork.SaveAsync();
                       
                    }
                    _unitOfWork.CommitAsync(transaction);

                    var cartDt = await _unitOfWork.OrderDetails.GetAllAsync(a => a.OrderId == id, includeProperties: "Aircraft");
                    //var prodetails = await _unitOfWork.CartDetails.GetAllAsync(z => z.AircraftId == cartDt.AircraftId, includeProperties: "Aircraft");

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("hhealthcareplus@gmail.com");
                    message.To.Add(user.Email);
                    message.Subject = "Invoice for Order #" + order.Id;

                    string htmlBody = $@"<html>
                      <body>
                          <p>Thank you for your purchase !!</p>
                          <p>Order ID: {order.Id}</p>
                          <p>Total Amount: {order.TotAmount}</p>
                          <p>Shipping Amount: {order.ShippingAmount}</p>
                          <p>Grand Total:Rs. {order.GrandTotal}</p>
                          <table border='1'>
                              <tr>
                                  <th>Aircraft</th>
                                  <th>Quantity</th>
                                  <th>Unit Price</th>
                              </tr>";

                    foreach (var item in cartDt)
                    {
                        // Add each aircraft detail as a row in the table
                        htmlBody += $@"<tr>
                        <td>{item.Aircraft.Name}</td>
                        <td>{item.Qty}</td>
                        <td>Rs.{item.Price}</td>
                    </tr>";
                    }

                    htmlBody += @"</table>
               </body>
               </html>";

                    message.Body = htmlBody;
                    message.IsBodyHtml = true; // Set this to true to indicate that the body contains HTML

                    // Configure the SMTP client
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new System.Net.NetworkCredential("hhealthcareplus@gmail.com", "blgatarjczxjdsrw");
                    smtpClient.EnableSsl = true; // Enable SSL if required

                    // Send the email
                    smtpClient.Send(message);

                    // Close the SMTP client
                    smtpClient.Dispose();

                    

                    await SetSession(claim);
                    _session.Remove<CartDetails>("Cart");
                    _session.Remove<CartCustomozationDetails>("Cartcustomization");


                    return RedirectToAction("ThankYouPage", new { id = id});
                    }
                    catch (Exception)
                    {
                        _unitOfWork.RollbackAsync(transaction);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }

        }

        [Authorize(Roles = SD.RoleCustomer)]
        public async Task<IActionResult> ThankYouPage(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var orderVM = new OrderVM();

            var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(id));
            var orderDetails = await _unitOfWork.OrderDetails.GetAllAsync(a => a.OrderId == Convert.ToInt32(id), includeProperties: "Aircraft");
            var customer = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == claim.Value);
            var ordercustom = (from ord in await _unitOfWork.OrderDetails.GetAllAsync()
                               join or in await _unitOfWork.Order.GetAllAsync() on ord.OrderId equals or.Id
                               join orcu in await _unitOfWork.orderCustomozationDetails.GetAllAsync() on ord.Id equals orcu.OrderDetailsId
                               join avcust in await _unitOfWork.availableCustomozation.GetAllAsync() on orcu.AvailableCustomozationId equals avcust.Id
                               where or.Id == Convert.ToInt32(id)
                               select new AvailableCustomozation
                               {
                                   CustomzationType = avcust.CustomzationType,
                                   CustomzationValue = avcust.CustomzationValue,
                                   Price = avcust.Price

                               }).ToList();
          
            orderVM.AvailableCustomozationList = ordercustom;
            orderVM.Orders = order;
            orderVM.OrderDetailList = orderDetails;
            orderVM.ShippingDetail = await _unitOfWork.shippingDetail.GetFirstOrDefaultAsync(x => x.ShippingID == order.ShippingID,includeProperties: "City");
            orderVM.User = customer;

            return View(orderVM);

        }


        [Authorize(Roles = SD.RoleCustomer)]
        public async Task<IActionResult> MyOrders()
        {
            var orderVM = new OrderVM();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var orders = await _unitOfWork.Order.GetAllAsync(a => a.UserId == claim.Value && a.CurStatus == SD.Active);
            if (orders != null)
            {
                orderVM.OrderList = orders;
                return View(orderVM);
            }
            return View();
        }
        public async Task<IActionResult> ViewOrder(string id)
        {
            TempData["orderId"] = id;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
          
                try
                {
                    var orderVM = new OrderVM();

                    var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(id));
                    var orderDetails = await _unitOfWork.OrderDetails.GetAllAsync(a => a.OrderId == Convert.ToInt32(id), includeProperties: "Aircraft");
                    var ordercustom = (from ord in await _unitOfWork.OrderDetails.GetAllAsync()
                                   join or in await _unitOfWork.Order.GetAllAsync() on ord.OrderId equals or.Id
                                   join orcu in await _unitOfWork.orderCustomozationDetails.GetAllAsync() on ord.Id equals orcu.OrderDetailsId
                                   join avcust in await _unitOfWork.availableCustomozation.GetAllAsync() on orcu.AvailableCustomozationId equals avcust.Id
                                   where or.Id == Convert.ToInt32(id)
                                   select new AvailableCustomozation
                                   {
                                       CustomzationType = avcust.CustomzationType,
                                       CustomzationValue = avcust.CustomzationValue,
                                       Price =avcust.Price

                                   }).ToList();
                orderVM.AvailableCustomozationList = ordercustom;
                orderVM.Orders = order;
                orderVM.OrderDetailList = orderDetails;
                    return View(orderVM);
                    
                }
                catch (Exception)
                {                  
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
        }
       
        public async Task<IActionResult> CancelOrder(string id)
        {

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var order = await _unitOfWork.Order.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(id));
                    order.CurStatus = SD.DeActive;
                    _unitOfWork.SaveAsync();
                    var orderDetails = await _unitOfWork.OrderDetails.GetAllAsync(a => a.OrderId == order.Id);
                    foreach (var item in orderDetails)
                    {
                        var books = await _unitOfWork.aircraft.GetFirstOrDefaultAsync(a => a.AircratfId == item.AircraftId);
                        books.Qty = books.Qty + item.Qty;
                        _unitOfWork.SaveAsync();
                    }
                    _unitOfWork.CommitAsync(transaction);
                    return RedirectToAction("MyOrders");
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackAsync(transaction);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

        }


        public async Task<IActionResult> plus(string Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var cartDt = await _unitOfWork.CartDetails.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(Id), includeProperties: "Aircraft");
                    cartDt.Qty = cartDt.Qty + 1;
                    _unitOfWork.SaveAsync();
                    _unitOfWork.CommitAsync(transaction);
                     await  SetSession(claim);
                    return RedirectToAction("ViewCart");
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackAsync(transaction);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                
            }
         
        }

        public async Task<IActionResult> minus(string Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var cartDt = await _unitOfWork.CartDetails.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(Id), includeProperties: "Aircraft");
                    cartDt.Qty = cartDt.Qty - 1;
                    _unitOfWork.SaveAsync();
                    _unitOfWork.CommitAsync(transaction);
                    await SetSession(claim);
                    return RedirectToAction("ViewCart");
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackAsync(transaction);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }
            
        }

        public async Task<IActionResult> remove(string Id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var cartDt = await _unitOfWork.CartDetails.GetFirstOrDefaultAsync(a => a.Id == Convert.ToInt32(Id), includeProperties: "Aircraft");
                    await _unitOfWork.CartDetails.Remove(Convert.ToInt32(Id));
                    _unitOfWork.SaveAsync();
                    _unitOfWork.CommitAsync(transaction);
                    await SetSession(claim);
                    return RedirectToAction("ViewCart");
                }
                catch (Exception)
                {
                    _unitOfWork.RollbackAsync(transaction);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }

        }
        private async Task SetSession(Claim claim)
        {
            var carts = await _unitOfWork.Cart.GetFirstOrDefaultAsync(a => a.UserId == claim.Value && a.CurStatus == SD.Active);
            if (carts != null)
            {

                var cartDt = await _unitOfWork.CartDetails.GetAllAsync(a => a.CartId == carts.Id, includeProperties: "Aircraft");
                var cust = await _unitOfWork.CartCustomozationDetails.GetAllAsync(x => x.CartDetails.CartId == carts.Id, includeProperties: "CartDetails");
                if (cartDt != null)
                {
                    _session.SetSessionsNew<IEnumerable<CartDetails>>("Cart", cartDt);
                    _session.SetSessionsNew<IEnumerable<CartCustomozationDetails>>("Cartcustomization", cust);

                }
            }
        }

        [HttpGet]
        public async Task<string> getHas(int? id)
        {
            Orders order = await _unitOfWork.Order.GetFirstOrDefaultAsync(x => x.Id == id);
            string merchantId = "1222958";
            string merchantSecret = "MzM1NjQwODcyODEzMzk5NTI3MzAyNzMzNzY5NTM1MTkxNzI3NjIzNA==";

            //Mzk5MTIyNTczMTE3MzQ4OTU4NjYzNzg5MjQ1NjkyMjE1OTA5ODE5Nw ==
            //    MTEyNjM2MDg4MTQyMzMyODk0ODIyMzY1MTM1MjgzMzM4MTI4MjQ3NA ==
            //    Mzk5MTIyNTczMTE3MzQ4OTU4NjYzNzg5MjQ1NjkyMjE1OTA5ODE5Nw ==

            string orderId = Convert.ToString(order.Id);
            double amount = order.GrandTotal;
            string hashedSecret = ComputeMD5(merchantSecret);
            string amountFormated = amount.ToString("####0.00");
            string currency = "LKR";
            string hash = ComputeMD5(merchantId + orderId + amountFormated + currency + hashedSecret.ToUpper());

            return hash;
            //return Json(new { data = hash.ToUpper(), success=true });
        }
        static string ComputeMD5(string s)
        {
            //  cryptographic :MD5 hash algorithm , //  MD5 work on binary data.

            StringBuilder sb = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }
            return sb.ToString();

            //128 - bit(16 - byte) hash value need to return : hexadecimal string
        }

        [HttpGet]
        public async Task<IActionResult> GetShippingAmount(int cityId)
        {           
            var shippingAmount = (await _unitOfWork.shippingSet.GetAllAsync(x => x.CityId == cityId)).Select(s=>s.Amount).FirstOrDefault();
           
            return Json(new { shippingAmount = shippingAmount });
        }



    }
}
