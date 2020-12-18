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
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //we need to get this unitofwork which was added in our dependency injection inside Startup.cs
        public CompanyController(IUnitOfWork unitOfWork)
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
            //we need a model for the view, in here we need company
            Company company = new Company();
            if (id == null)
            {
                //this is for create
                return View(company);
            }
            //this is for edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        #region API Calls

        //get all the categories and return in a Json format
        [HttpGet]//We will call this httpget inside the javascript which will comsume inside our index view
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Company.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion API Calls
    }
}