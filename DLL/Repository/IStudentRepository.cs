﻿using DLL.DBContext;
using DLL.Model;
using DLL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Repository
{
    public interface IStudentRepository : IRepositoryBase<Student>
    {
      
    }

    public class StudentRepository : RepositoryBase<Student>, IStudentRepository
    {

        public StudentRepository(ApplicationDBContext context) : base(context)
        {

        }

    }

 }
