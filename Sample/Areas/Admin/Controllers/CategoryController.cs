using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample.DataAccess.Data.Repository.IRepository;
using Sample.Models;
using Sample.Utility;

namespace Sample.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] //all the action within this class will only accessiable to this role
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //we need to get this unitofwork which was added in our dependency injection inside Startup.cs
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //We will using data tables using api calls and javascript
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            //we need a model for the view, in here we need category
            Category category = new Category();
            if (id == null)
            {
                //this is for create
                return View(category);
            }
            //this is for edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        #region API Calls

        //get all the categories and return in a Json format
        [HttpGet]//We will call this httpget inside the javascript which will comsume inside our index view
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion API Calls
    }
}