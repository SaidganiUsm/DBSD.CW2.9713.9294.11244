using DBSD.CW2._9713._9294._11244.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using DBSD.CW2._9713._9294._11244.Interfaces;
using X.PagedList;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json;
using System.Globalization;
using CsvHelper;
using Dapper;
using System.Linq;
using System.Text;

namespace DBSD.CW2._9713._9294._11244.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployeeRepository _repository;

        public EmployeeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        // GET: EmployeeController
        public ActionResult Index()
        {
            var empList = _repository.GetAllEmployees();

            return View(empList);
        }

        // GET: EmployeeController/Details/5
        public ActionResult Details(int id)
        {
            return View(_repository.GetById(id));
        }

        // GET: EmployeeController/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: EmployeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee emp)
        {
            try
            {
                int id = _repository.Insert(emp);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: EmployeeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_repository.GetById(id));
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Employee emp)
        {
            try
            {
                emp.EmployeeId = id;
                _repository.Update(emp);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: EmployeeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_repository.GetById(id));
        }

        // POST: EmployeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Employee emp)
        {
            try
            {
                _repository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Filter(EmployeeFilterModel filterModel)
        {
            int totalRows;
            var employees = _repository.Filter(
                filterModel.FirstName, filterModel.LastName, 
                filterModel.Age, filterModel.Address, filterModel.SectorId, 
                out totalRows, filterModel.Page, filterModel.PageSize,
                filterModel.SortColumn, filterModel.SortDesc
                );

            filterModel.Employees = employees;

            return View(filterModel);
        }

        public ActionResult Import()
        {
            return View(new List<Employee>());
        }

        [HttpPost]
        public ActionResult Import(IFormFile importFile)
        {
            IList<Employee> employees = null;
            if (importFile != null)
            {
                using var stream = importFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                employees = (List<Employee>)JsonSerializer.
                    Deserialize(reader.ReadToEnd(), typeof(List<Employee>));

                _repository.Insert(employees);
            }

            return View(employees);
        }

        public ActionResult ImportXml()
        {
            return View(new List<Employee>());
        }

        [HttpPost]
        public ActionResult ImportXml(IFormFile importFile)
        {
            using var stream = importFile.OpenReadStream();
            using var reader = new StreamReader(stream);

            var employees_ = _repository.ImportXml(reader.ReadToEnd());

            return View(employees_);
        }

        public ActionResult ImportCsv()
        {
            return View(new List<Employee>());
        }

        [HttpPost]
        public ActionResult ImportCsv(IFormFile importFile)
        {
            using var stream = importFile.OpenReadStream();
            using var reader = new StreamReader(stream);

            var employees_ = _repository.ImportCsv(reader.ReadToEnd());

            return View(employees_);

        }
        public ActionResult ExportAsXml()
        {
            var xml = _repository.ExportAsXml();

            if (string.IsNullOrWhiteSpace(xml))
                return NotFound();
            else
                return File(Encoding.UTF8.GetBytes(xml),
        "application/xml",
        $"Emp_{DateTime.Now}.xml");
        }

        public ActionResult ExportAsJson()
        {
            var json = _repository.ExportAsJson();

            if (string.IsNullOrWhiteSpace(json))
                return NotFound();
            else
                return File(Encoding.UTF8.GetBytes(json), "application/json", $"Export_{DateTime.Now}.json");

        }

    }
}
