﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Request;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SimpleAPI.Controllers
{
    
    public class AccountController : APIConnectionController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            return Ok(await _accountService.loginUser(request));
        }

        [HttpGet("Test1")]
        public ActionResult Test1()
        {
            return Ok("enter test 1");
        }



        [HttpGet("test2")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Test2()
        {
            var tt = User;

            await _accountService.UserLoginInfo(tt);
            return Ok("enter test 2");
        }

        [HttpGet("test3")]
        [Authorize(Roles = "Teacher, Staff")]
        public ActionResult Test3()
        {
            var tt = User;

            _accountService.UserLoginInfo(tt);
            return Ok("enter test 3");
        }




    }
}