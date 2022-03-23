using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoctorsOffice.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorsOffice.Controllers
{
  public class PatientsController : Controller
  {
    private readonly DoctorsOfficeContext _db;

    public PatientsController(DoctorsOfficeContext db)
    {
      _db = db;
    }
    public ActionResult Index()
    {
      return View(_db.Patients.ToList());
    }
    public ActionResult Create()
    {
      //Provides data needed for an HTML select list. First argument reps the data that will populate our list <option> elements (a list of categories). Second is the value of every <option> element, third is the displayed text/column of every <option> element. 
      ViewBag.DoctorId = new SelectList(_db.Doctors, "DoctorId", "Name");
      return View();
    }
    [HttpPost]
    public ActionResult Create(Patient patient, int DoctorId)
    {
      _db.Patients.Add(patient);
      _db.SaveChanges();
      //This conditional is incase no doctors have yet been made.
      if (DoctorId != 0)
      {
        _db.DoctorPatient.Add(new DoctorPatient() {DoctorId = DoctorId, PatientId = patient.PatientId });
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }
    public ActionResult Details(int id)
    {
      var thisPatient = _db.Patients
        .Include(patient => patient.JoinEntities)
        .ThenInclude(join => join.Doctor)
        .FirstOrDefault(patient => patient.PatientId == id);
      return View(thisPatient);
    }
    public ActionResult Edit (int id)
    {
      var thisPatient = _db.Patients.FirstOrDefault(patient => patient.PatientId == id);
      ViewBag.DoctorId = new SelectList(_db.Doctors, "DoctorId", "Name");
      return View (thisPatient);
    }
    [HttpPost]
    public ActionResult Edit(Patient patient, int DoctorId)
    {
      bool duplicate = _db.DoctorPatient.Any(join => join.DoctorId == DoctorId && join.PatientId == patient.PatientId);
      if (DoctorId != 0 && !duplicate)
      {
        _db.DoctorPatient.Add(new DoctorPatient() { DoctorId = DoctorId, PatientId = patient.PatientId });
      }
      _db.Entry(patient).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult Delete (int id)
    {
      var thisPatient = _db.Patients.FirstOrDefault(patient => patient.PatientId == id);
      return View(thisPatient);
    }
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisPatient = _db.Patients.FirstOrDefault(patient => patient.PatientId == id);
      _db.Patients.Remove(thisPatient);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult AddDoctor(int id)
    {
      var thisPatient = _db.Patients.FirstOrDefault(patient => patient.PatientId == id);
      ViewBag.PatientId = new SelectList(_db.Doctors, "DoctorId", "Name");
      return View(thisPatient);
    }
    [HttpPost]
    public ActionResult AddDoctor(Patient patient, int DoctorId)
    {
      if (DoctorId != 0)
      {
        _db.DoctorPatient.Add(new DoctorPatient() { DoctorId = DoctorId, PatientId = patient.PatientId });
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }
    //Deletes a Doctor relationship from a Patient without deleting the Patient itself.
    [HttpPost]
    public ActionResult DeleteDoctor(int joinId)
    {
      var joinEntry = _db.DoctorPatient.FirstOrDefault(entry => entry.DoctorPatientId == joinId);
      _db.DoctorPatient.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}