﻿using BLL.Request;
using DLL.Model;
using DLL.Repository;
using DLL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
//using Utility.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DLL.MongoReport;
using DLL.MongoReport.Models;
using Utility.Exceptions;

namespace BLL.Services
{
    public interface IStudentService
    {
       public Task<Student> AddStudentAsync(StudentInsertRequest request);
       public Task<Student> FindAStudentAsync(string code);
       public Task<List<Student>> GetAllStudentAsync();

       Task<List<StudentReport>> GetAllStudentDepartmentReportAsync();

        Task<Boolean> IsEmailExist(string email);
        Task<Boolean> IsRollNoExist(string rollNo);

        Task<Student> DeleteAStudentAsync(string rollNo);
        Task<Student> UpdateStudentAsync(string rollNo, StudentUpdateRequest aStudent);
        Task<Boolean> IsDepartmentExist(int deptId);
        Task<Boolean> IsStudentExist(int studentId);
    }
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MongoDBContext _context;

        public StudentService(IUnitOfWork unitOfWork, MongoDBContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Student> AddStudentAsync(StudentInsertRequest request)
        {
            Student student = new Student()
            {
                Name = request.Name,
                Email = request.Email,
                RollNo = request.RollNo,
                DeptID=request.DeptId

            };
             await _unitOfWork.studentRepository.CreateAsync(student);
            
            if (await _unitOfWork.ApplicationSaveChangesAsync())
            {
                //Mongodb department student report Insert

                DepartmentStudentReportMongo reportMongo=new DepartmentStudentReportMongo()
                {
                    DepartmentCode = student.Department.Code,
                    DepartmentName = student.Department.DeptName,
                    StudentName = student.Name,
                    StudentEmail = student.Email,
                    StudentRollNo = student.RollNo
                };
                await _context.DepartmentStudentReport.InsertOneAsync(reportMongo);

                return student;
            }
            throw new MyAppException("Student Data Not save");
        }
        public async Task<Student> FindAStudentAsync(string rollNo)
        {
            var student = await _unitOfWork.studentRepository.GetAAsynce(x => x.RollNo == rollNo);
            if (student == null)
            {
                throw new MyAppException("Student Not Found");
            }
            return student;
        }

        public async Task<List<Student>> GetAllStudentAsync()
        {
            return await _unitOfWork.studentRepository.GetListAsynce();
        }
       
        public async Task<Student> UpdateStudentAsync(string rollNo, StudentUpdateRequest aStudent)
        {
            var student = await _unitOfWork.studentRepository.GetAAsynce(x => x.RollNo == rollNo);
            if (student == null)
            {
                throw new MyAppException("Student Not Found");
            }

            if (!string.IsNullOrWhiteSpace(aStudent.RollNo))
            {
                var isCodeExistAnotherDepartment = await _unitOfWork.studentRepository.GetAAsynce(x => x.RollNo == aStudent.RollNo
                && x.StudentID != student.StudentID);
                if (isCodeExistAnotherDepartment == null)
                {
                    student.RollNo = aStudent.RollNo;
                }
                else
                {
                    throw new MyAppException("Code Already Exists On different Department");
                }
            }

            if (!string.IsNullOrWhiteSpace(aStudent.Email))
            {
                var isNameExistAnotherDepartment = await _unitOfWork.studentRepository.GetAAsynce(x => x.Email == aStudent.Email
                && x.StudentID != student.StudentID);
                if (isNameExistAnotherDepartment == null)
                {
                    student.Email = aStudent.Email;
                }
                else
                {
                    throw new MyAppException("Email Already Exists On different Student");
                }
            }
            _unitOfWork.studentRepository.UpdateAsyc(student);

            if (await _unitOfWork.ApplicationSaveChangesAsync())
            {
                return student;
            }

            throw new MyAppException("Something Went Wrong");

        }

        public async Task<Student> DeleteAStudentAsync(string rollNo)
        {
            var student = await _unitOfWork.studentRepository.GetAAsynce(x => x.RollNo == rollNo);
            if (student == null)
            {
                throw new MyAppException("Student Not Found");
            }
            _unitOfWork.studentRepository.DeleteAsync(student);

            if (await _unitOfWork.ApplicationSaveChangesAsync())
            {
                return student;
            }

            throw new MyAppException("Something Went Wrong");

        }
        public async Task<bool> IsEmailExist(string email)
        {
            var student = await _unitOfWork.studentRepository.GetAAsynce(x => x.Email == email);

            if (student != null)
            {
                return true;
            }
            return false;

        }
        public async Task<bool> IsRollNoExist(string rollNo)
        {
            var student = await _unitOfWork.studentRepository.GetAAsynce(x => x.RollNo == rollNo);

            if (student != null)
            {
                return true;
            }
            return false;

        }

        public async Task<bool> IsDepartmentExist(int deptId)
        {
            var department = await _unitOfWork.departmentRepository.GetAAsynce(x => x.DeptId == deptId);

            if (department != null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<StudentReport>> GetAllStudentDepartmentReportAsync()
        {
            var allStudent = await _unitOfWork.studentRepository.QueryAll().Include(x => x.Department).ToListAsync();
           
            var result = new List<StudentReport>();

            foreach (var student in allStudent)
            {
                result.Add(new StudentReport()
                {
                    StudentName = student.Name,
                    StudentRollNo = student.RollNo,
                    StudentEmail=student.Email,
                    DepartmentCode = student.Department.Code,
                    DepartmentName = student.Department.DeptName
                });
            }

            return result;
        }

        public async Task<bool> IsStudentExist(int studentId)
        {
            var student = await _unitOfWork.studentRepository.GetAAsynce(x => x.StudentID == studentId);

            if (student != null)
            {
                return false;
            }
            return true;
        }
    }
}

