using Microsoft.AspNetCore.Mvc;
using JLL.PizzaProblem.API.Models;
using JLL.PizzaProblem.API.Services;
using JLL.PizzaProblem.API.Filters;
using System.Collections.Generic;
using AutoMapper;

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
        public IActionResult IncreasePizzaLoveForUser(int Id)
        {
            if (_userService.GetById(Id) == null)
            {
                return BadRequest();
            }

            _userService.IncreasePizzaLoveForUser(Id);

            return NoContent();
        }
    }
}
