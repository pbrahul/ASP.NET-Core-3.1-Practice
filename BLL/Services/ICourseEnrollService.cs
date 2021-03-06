﻿using BLL.Request;
using DLL.Model;
using DLL.Repository;
using DLL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Utility.Exceptions;

namespace BLL.Services
{
    public interface ICourseEnrollService
    {
        Task<CourseStudent> AddCourseEnrollAsync(CourseEnrollInsertRequest request);
        Task<List<CourseEnrollmentReport>> GetAllStudentCourseEnrollReportAsync();

    }

    public class CourseEnrollService : ICourseEnrollService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseEnrollService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        

        public async Task<CourseStudent> AddCourseEnrollAsync(CourseEnrollInsertRequest request)
        {

            CourseStudent courseEnroll = new CourseStudent()
            {
                StudentId = request.StudentID,
                CourseId = request.CourseID
            };
            //var query = await _unitOfWork.courseEnrollRepository.QueryAll().Where(x=>x.StudentId == request.StudentID).Where(x => x.CourseId == request.CourseID).ToListAsync();
            //if (query != null)
            //{
            //    throw new MyAppException("Student Alreary Entrolled On This Course");
            //}

            await _unitOfWork.courseEnrollRepository.CreateAsync(courseEnroll);
            if (await _unitOfWork.ApplicationSaveChangesAsync())
            {
                return courseEnroll;

            }
            throw new MyAppException("Something Went Wrong");
            
        }

        public async Task<List<CourseEnrollmentReport>> GetAllStudentCourseEnrollReportAsync()
        {
            var allStudent = await _unitOfWork.studentRepository.QueryAll().Include(x => x.StudentCourses).ThenInclude(x => x.Course).ToListAsync();
            
   //         var allStudent = await _unitOfWork.courseEnrollRepository.QueryAll().Include(x => x.Student).Include(x => x.Course).GroupBy(p => p.StudentId)
   //.Select(g => g.OrderBy(p => p.Student.Name)).ToListAsync();

//            GroupBy(x => new { x.Birthdate.Year })
//.Where(x => x.Count() >= 2)
//.Select(x => new { x.Key, Count = x.Count() })
//.ToList();
            //var studentCourses =
            //    await _unitOfWork.StudentRepository.QueryAll().Include(x => x.CourseStudents).ThenInclude(x => x.Course)
            //        .ToListAsync();

            var result = new List<CourseEnrollmentReport>();
           
            foreach (var studentEnroll in allStudent)
            {
                //var studentID = studentEnroll.Student.StudentID;

                result.Add(new CourseEnrollmentReport()
                {
                    StudentName = studentEnroll.Name,
                    StudentRollNo = studentEnroll.RollNo,
                    StudentEmail = studentEnroll.Email,
                    //CourseCode = studentEnroll.Course.Code,
                    //CourseName=studentEnroll.Course.Name
                    CourseStusents=studentEnroll.StudentCourses

                });
                 
            }
            return result;
        }
     }
}
