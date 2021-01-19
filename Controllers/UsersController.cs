using Microsoft.AspNetCore.Mvc;
using JLL.PizzaProblem.Models;
using JLL.PizzaProblem.Services;
using JLL.PizzaProblem.Filters;
using System.Collections.Generic;
using AutoMapper;

namespace JLL.PizzaProblem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{Id}", Name = "GetUser")]
        public ActionResult<IEnumerable<User>> GetUser(int Id)
        {
            var user = _userService.GetById(Id);
            
            if(user == null)
            {
                return NotFound();
            }
            
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<User> Post(UserForCreation newUser)
        {
            var user = _userService.AddNewUser(_mapper.Map<User>(newUser));
            
            return CreatedAtRoute("GetUser",
                new { user.Id },
                user);
        }
    }
}
