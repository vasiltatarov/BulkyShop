namespace BulkyWeb.Areas.Admin.Controllers;

using Stripe;
using Stripe.Checkout;

[Area(SD.Role_Admin)]
[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    [BindProperty]
    public OrderViewModel OrderViewModel { get; set; }

    public IActionResult Index() => View();

    public IActionResult Details(int orderId)
    {
        this.OrderViewModel = new()
        {
            OrderHeader = this.unitOfWork.OrderHeaderRepository.Get(x => x.Id == orderId, "User"),
            OrderDetails = this.unitOfWork.OrderDetailRepository.GetAll(x => x.OrderHeaderId == orderId, "Product")
        };

        return View(this.OrderViewModel);
    }

    [ActionName("Details")]
    [HttpPost]
    public IActionResult DetailsPayNow()
    {
        this.OrderViewModel.OrderHeader = this.unitOfWork.OrderHeaderRepository.Get(x => x.Id == this.OrderViewModel.OrderHeader.Id, "User");
        this.OrderViewModel.OrderDetails = this.unitOfWork.OrderDetailRepository.GetAll(x => x.OrderHeaderId == this.OrderViewModel.OrderHeader.Id, "Product");

        //stripe logic
        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{domain}Admin/Order/PaymentConfirmation?orderHeaderId={this.OrderViewModel.OrderHeader.Id}",
            CancelUrl = $"{domain}Admin/Order/Details?orderId={this.OrderViewModel.OrderHeader.Id}",
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment"
        };

        foreach (var orderDetails in this.OrderViewModel.OrderDetails)
        {
            var lineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(orderDetails.Price * 100),
                    Currency = "BGN",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = orderDetails.Product.Title
                    }
                },
                Quantity = orderDetails.Count
            };

            options.LineItems.Add(lineItem);
        }

        var service = new SessionService();
        Session session = service.Create(options);

        this.unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(this.OrderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
        this.unitOfWork.Save();

        Response.Headers.Add("Location", session.Url);

        return new StatusCodeResult(303);
    }

    public IActionResult PaymentConfirmation(int orderHeaderId)
    {
        var orderHeader = this.unitOfWork.OrderHeaderRepository.Get(x => x.Id == orderHeaderId);

        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                this.unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                this.unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                this.unitOfWork.Save();
            }
        }

        return View(orderHeaderId);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult UpdateOrderDetail()
    {
        var orderHeader = this.unitOfWork.OrderHeaderRepository.Get(x => x.Id == this.OrderViewModel.OrderHeader.Id);
        orderHeader.Name = this.OrderViewModel.OrderHeader.Name;
        orderHeader.PhoneNumber = this.OrderViewModel.OrderHeader.PhoneNumber;
        orderHeader.StreetAddress = this.OrderViewModel.OrderHeader.StreetAddress;
        orderHeader.City = this.OrderViewModel.OrderHeader.City;
        orderHeader.State = this.OrderViewModel.OrderHeader.State;
        orderHeader.PostalCode = this.OrderViewModel.OrderHeader.PostalCode;

        if (!string.IsNullOrEmpty(this.OrderViewModel.OrderHeader.Carrier))
        {
            orderHeader.Carrier = this.OrderViewModel.OrderHeader.Carrier;
        }

        if (!string.IsNullOrEmpty(this.OrderViewModel.OrderHeader.TrackingNumber))
        {
            orderHeader.TrackingNumber = this.OrderViewModel.OrderHeader.TrackingNumber;
        }

        this.unitOfWork.OrderHeaderRepository.Update(orderHeader);
        this.unitOfWork.Save();

        TempData["Success"] = string.Format(WebConstants.SuccessEditNotification, "Order Details");

        return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult StartProcessing()
    {
        this.unitOfWork.OrderHeaderRepository.UpdateStatus(this.OrderViewModel.OrderHeader.Id, SD.StatusInProcess);
        this.unitOfWork.Save();

        TempData["Success"] = WebConstants.OrderStartProcessing;

        return RedirectToAction(nameof(Details), new { orderId = this.OrderViewModel.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult ShipOrder()
    {
        var orderHeader = this.unitOfWork.OrderHeaderRepository.Get(x => x.Id == this.OrderViewModel.OrderHeader.Id);

        orderHeader.TrackingNumber = this.OrderViewModel.OrderHeader.TrackingNumber;
        orderHeader.Carrier = this.OrderViewModel.OrderHeader.Carrier;
        orderHeader.OrderStatus = SD.StatusShipped;
        orderHeader.ShippingDate = DateTime.Now;

        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        }

        this.unitOfWork.OrderHeaderRepository.Update(orderHeader);
        this.unitOfWork.Save();

        TempData["Success"] = WebConstants.OrderShipped;

        return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public IActionResult CancelOrder()
    {
        var orderHeader = this.unitOfWork.OrderHeaderRepository.Get(x => x.Id == this.OrderViewModel.OrderHeader.Id);

        if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            this.unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            TempData["Success"] = WebConstants.OrderRefundedAndCanceled;
        }
        else
        {
            this.unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            TempData["Success"] = WebConstants.OrderCanceled;
        }

        this.unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
    }

    #region Api Calls

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeaders;

        if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
        {
            orderHeaders = this.unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "User");
        }
        else
        {
            var userClaims = (ClaimsIdentity)User.Identity;
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

            orderHeaders = this.unitOfWork.OrderHeaderRepository.GetAll(x => x.UserId == userId, includeProperties: "User");
        }

        switch (status)
        {
            case "pending":
                orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                break;
            case "completed":
                orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                break;
            case "approved":
                orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                break;
            default:
                break;
        }

        return Json(new { data = orderHeaders });
    }

    #endregion
}
