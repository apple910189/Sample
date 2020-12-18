using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.DataAccess.Data.Repository.IRepository;
using Sample.Models;
using Sample.Models.ViewModels;

namespace Sample.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductID = productFromDb.Id
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]//an user must login to make purchase
        public IActionResult Details(ShoppingCart CartObjerct)
        {
            CartObjerct.Id = 0;
            if (ModelState.IsValid)
            {
                //then we will add to cart
                //we need to find out the id of the login user
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);//find base on the claim types
                CartObjerct.ApplicationUserId = claim.Value;//claim.value has the actual user id of the login user, so we can stored at cart
                //we want to get the shopping cart from database based on user id and product id
                //because a uniqe cart composed by userid and productid
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == CartObjerct.ApplicationUserId && u.ProductID == CartObjerct.ProductID,
                    includeProperties: "Product"
                    );
                if (cartFromDb == null)
                {
                    //this user do not have the product in cart, so we want to add that
                    _unitOfWork.ShoppingCart.Add(CartObjerct);
                }
                else
                {
                    //the product is in the cart, we need to increase the quantity
                    cartFromDb.Count += CartObjerct.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);//you can skip this update.
                }
                _unitOfWork.Save();//Save while Update. EntityFramework can track and know cartFromDb is from db
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //if is not valid, return to the view
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == CartObjerct.Id, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductID = productFromDb.Id
                };
                return View(cartObj);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}