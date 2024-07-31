using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practice.BAL.Interface;
using Practice.DAL.DataContext;
using Practice.DAL.Models;
using Practice.DAL.ViewModels;

namespace Practice.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentRepo _studentRepo;

        public StudentController(ApplicationDbContext context,IStudentRepo studentRepo)
        {
            _context = context;
            _studentRepo = studentRepo;
        }

        public IActionResult Index()
        {
            StudentData studentData = new StudentData();
            studentData.studentList = _studentRepo.GetStudentData();
            return View(studentData);
        }

        public IActionResult CreateFormModal()
        {
            StudentData studentData = new StudentData();
            studentData.courses = _studentRepo.GetCourses();
            
            return PartialView("Student/_Create",studentData);
        }

        public IActionResult PostCreateForm(StudentList studentInfo)
        {
            _studentRepo.PostStudentData(studentInfo);

            return Ok();
        }

        public IActionResult EditFormModal(int studentId)
        {
            StudentData studentData = new StudentData();
            studentData.courses = _studentRepo.GetCourses();
            studentData.StudentInfo = _studentRepo.GetStudentEditData(studentId);
            //studentData.StudentInfo.StudentId = studentId;


            return PartialView("Student/_Edit", studentData);
        }

        public IActionResult PostEditForm(StudentList studentInfo)
        {
            _studentRepo.PostEditStudentData(studentInfo);

            return Ok();
        }

        public IActionResult DeleteRow(int studentId)
        {
            _studentRepo.DeleteData(studentId);

            return Ok();
        }

    }
}
