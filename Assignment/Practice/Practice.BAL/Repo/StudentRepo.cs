using Microsoft.EntityFrameworkCore;
using Practice.BAL.Interface;
using Practice.DAL.DataContext;
using Practice.DAL.Models;
using Practice.DAL.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.BAL.Repo
{
    public class StudentRepo : IStudentRepo
    {
        public readonly ApplicationDbContext _db;

        public StudentRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<StudentList> GetStudentData()
        {     
            var rl = _db.Students.ToList();

            var list = rl.Select(r => new StudentList()
            {
                StudentId = r.Studentid,
                FirstName = r.Firstname,
                LastName = r.Lastname,
                Email = r.Email,
                Age = r.Age,
                Gender = r.Gender,
                Course = _db.Courses.FirstOrDefault(x => x.Courseid == r.Courseid).Name,
                Grade = r.Grade,
            }).ToList();

            return list;
        }

        public List<Course> GetCourses()
        {
            var courses = _db.Courses.ToList();
            return courses;
        }

        public void PostStudentData(StudentList StudentInfo)
        {
            Student? student = new Student()
            {
                Firstname = StudentInfo.FirstName,
                Lastname = StudentInfo.LastName,
                Courseid = StudentInfo.CourseId,
                Age = StudentInfo.Age,
                Email = StudentInfo.Email,
                Gender = StudentInfo.Gender,
                Grade = StudentInfo.Grade,
            };
            _db.Students.Add(student);
            _db.SaveChanges();
        }

        public StudentList GetStudentEditData(int studentId)
        {
            var sd = _db.Students.FirstOrDefault(i=>i.Studentid == studentId);

            StudentList Model = new StudentList()
            {
                StudentId = studentId,
                FirstName = sd.Firstname,
                LastName =sd.Lastname,
                CourseId = sd.Courseid,
                Age = sd.Age,
                Email = sd.Email,
                Gender =sd.Gender,
                Grade = sd.Grade,
            };
            return Model;
        }

        public void PostEditStudentData(StudentList StudentInfo)
        {
            var student = _db.Students.FirstOrDefault(x=>x.Studentid == StudentInfo.StudentId);
            if (student != null)
            {
                student.Firstname = StudentInfo.FirstName;
                student.Lastname = StudentInfo.LastName;
                student.Courseid = StudentInfo.CourseId;
                student.Age = StudentInfo.Age;
                student.Email = StudentInfo.Email;
                student.Gender = StudentInfo.Gender;
                student.Grade = StudentInfo.Grade;
                _db.SaveChanges();
            }
           
        }

        public void DeleteData(int studentId)
        {
            var row = _db.Students.FirstOrDefault(i => i.Studentid == studentId);   
            _db.Students.Remove(row);
            _db.SaveChanges(true);
        }
    }
}
