using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Domain.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IGenericService<Order> _orderService;
        private readonly IGenericService<OrderDetails> _orderDetailService;
        private readonly IGenericService<Product> _productService;
        private readonly IGenericService<User> _userService;

        public IGenericService<User> UserService { get; }

        //localhost:PortNo/api/Category/TumKategorileriGetir

        public OrderController(IGenericService<User> userService, IGenericService<Product> productService, IGenericService<Order> orderService, IGenericService<OrderDetails> orderDetailService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _productService=productService;
            _userService=userService;
        }

        [HttpGet]
        public IActionResult TumSiparisleriGetir()
        {
            return Ok(_orderService.GetAll(t0=>t0.OrderDetails, t1=>t1.User));
        }

        [HttpGet]
        public IActionResult AktifSiparisleriGetir()
        {
            return Ok(_orderService.GetActive());
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreSiparisGetir(int id)
        {
            return Ok(_orderService.GetByID(id, t0 => t0.OrderDetails, t1 => t1.User));
        }
        [HttpGet]
        public IActionResult BekleyenSiparisleriGetir()
        {
            return Ok(_orderService.GetDefault(x=>x.Status==Domain.Enums.Status.Pending));
        }

        [HttpGet]
        public IActionResult OnaylananSiparisleriGetir()
        {
            return Ok(_orderService.GetDefault(x => x.Status == Domain.Enums.Status.Confirmed));
        }
        [HttpGet]
        public IActionResult RedEdilenSiparisleriGetir()
        {
            return Ok(_orderService.GetDefault(x => x.Status == Domain.Enums.Status.Cancelled));
        }



        [HttpPost]
        public IActionResult SiparisEkle(int userID,[FromQuery] int[] productIDs, [FromQuery] short[] quantities)
        {
            Order newOrder = new Order();
            newOrder.UserID = userID;
            newOrder.Status= Domain.Enums.Status.Pending;
            newOrder.IsActive=true;

            _orderService.Add(newOrder);

            for (int i = 0; i < productIDs.Length; i++)
            {
                OrderDetails newDetail = new OrderDetails();
                newDetail.ProductID = productIDs[i];
                newDetail.OrderID=newOrder.ID;  
                newDetail.Quantity = quantities[i];
                newDetail.UnitPrice = _productService.GetByID(productIDs[i]).UnitPrice;
                newDetail.IsActive = true;

                _orderDetailService.Add(newDetail);
            }
            return CreatedAtAction("IdyeGoreSiparisGetir",new {id=newOrder.ID},newOrder);
        }

        [HttpGet("{id}")]
        public IActionResult SiparisOnayla(int id)
        {
            Order confirmedOrder=_orderService.GetByID(id);
            if (confirmedOrder != null)
            {
                return NotFound();
            }
            else
            {
                List<OrderDetails> details=_orderDetailService.GetDefault(x=>x.OrderID==confirmedOrder.ID).ToList();

                foreach (OrderDetails item in details)
                {
                    Product product=_productService.GetByID(item.ProductID);
                    product.Stock -= item.Quantity;
                    _productService.Update(product);
                    item.IsActive = false;
                    _orderDetailService.Update(item);
                }

                confirmedOrder.Status=Domain.Enums.Status.Confirmed;
                confirmedOrder.IsActive = false;
                _orderService.Update(confirmedOrder);
                return Ok(confirmedOrder);
            }


        }

        [HttpGet("{id}")]
        public IActionResult SiparisReddet(int id)
        {
            Order cancelledorder = _orderService.GetByID(id);
            if (cancelledorder != null)
            {
                return NotFound();
            }
            else
            {
                List<OrderDetails> details = _orderDetailService.GetDefault(x => x.OrderID == cancelledorder.ID).ToList();

                foreach (OrderDetails item in details)
                {
                    
                    item.IsActive = false;
                    _orderDetailService.Update(item);
                }

                cancelledorder.Status = Domain.Enums.Status.Cancelled;
                cancelledorder.IsActive = false;
                _orderService.Update(cancelledorder);
                return Ok(cancelledorder);
            }


        }
    }
}
