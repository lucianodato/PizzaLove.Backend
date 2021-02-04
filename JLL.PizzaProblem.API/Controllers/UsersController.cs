using Microsoft.AspNetCore.Mvc;
using JLL.PizzaProblem.API.Models;
using JLL.PizzaProblem.API.Services;
using JLL.PizzaProblem.API.Filters;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

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
        public async Task<ActionResult<List<User>>> GetAllAsync()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{Id}", Name = "GetUser")]
        public async Task<ActionResult<List<User>>> GetUserAsync(int Id)
        {
            var user = await _userService.GetByIdAsync(Id);
            
            if(user == null)
            {
                return NotFound();
            }
            
            return Ok(user);
        }

        [HttpGet("GetTopTenUser")]
        public async Task<ActionResult<List<User>>> GetTopTenUserAsync()
        {
            var users = await _userService.GetTopTenPizzaLoveAsync();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUserAsync(UserForCreation newUser)
        {
            var user = await _userService.AddNewUserAsync(_mapper.Map<User>(newUser));

            return CreatedAtRoute("GetUser",
                new { user.Id },
                user);
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> AuthenticateAsync(AuthenticateRequest model)
        {
            var response = await _userService.AuthenticateAsync(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPatch("{Id}")]
        [Authorize]
        public async Task<IActionResult> PatchAsync(int Id, [FromBody] JsonPatchDocument<UserForPatch> patchDoc)
        {
            var user = await _userService.GetByIdAsync(Id);
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
                user.Id = Id;

                await _userService.UpdateUserAsync(user);

                return NoContent();
            }

            return BadRequest();
        }
    }
}
