using Practice.DAL.Models;
using Practice.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.BAL.Interface
{
    public interface IStudentRepo
    {
        public List<StudentList> GetStudentData();

        public List<Course> GetCourses();

        public void PostStudentData(StudentList StudentInfo);

        public StudentList GetStudentEditData(int studentId);

        public void PostEditStudentData(StudentList StudentInfo);

        public void DeleteData(int studentId);

    }
}
