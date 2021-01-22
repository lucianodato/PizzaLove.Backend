﻿using Microsoft.AspNetCore.Mvc;
using JLL.PizzaProblem.API.Models;
using JLL.PizzaProblem.API.Services;
using JLL.PizzaProblem.API.Filters;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace JLL.PizzaProblem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{Id}", Name = "GetUser")]
        public ActionResult<List<User>> GetUser(int Id)
        {
            var user = _userService.GetById(Id);
            
            if(user == null)
            {
                return NotFound();
            }
            
            return Ok(user);
        }

        [HttpGet("GetTopTenUser")]
        public ActionResult<List<User>> GetTopTenUser()
        {
            var users = _userService.GetTopTenPizzaLove();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpPost]
        public ActionResult<User> PostUser(UserForCreation newUser)
        {
            var user = _userService.AddNewUser(_mapper.Map<User>(newUser));

            return CreatedAtRoute("GetUser",
                new { user.Id },
                user);
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPut("{Id}")]
        [Authorize]
        public ActionResult<User> Update(int Id, UserForCreation userToUpdate)
        {
            if (_userService.GetById(Id) == null)
            {
                var newUser = _userService.AddNewUser(_mapper.Map<User>(userToUpdate));

                return CreatedAtRoute("GetUser",
                    new { newUser.Id },
                    newUser);
            }

            var user = _mapper.Map<User>(userToUpdate);
            user.Id = Id;
            _userService.UpdateUser(user);

            return NoContent();
        }

        [HttpPatch]
        public IActionResult Patch(int Id, [FromBody] JsonPatchDocument<UserForPatch> patchDoc)
        {
            var user = _userService.GetById(Id);
            if (user == null)
            {
                return NotFound();
            }

            if(patchDoc != null)
            {
                // Update entity fields
                var tmp = _mapper.Map <UserForPatch>(user);
                patchDoc.ApplyTo(tmp);
                user = _mapper.Map<User>(tmp);

                _userService.UpdateUser(user);

                return NoContent();
            }

            return BadRequest();
        }
    }
}
