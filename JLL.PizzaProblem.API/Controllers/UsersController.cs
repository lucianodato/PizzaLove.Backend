using AutoMapper;
using JLL.PizzaProblem.API.Dtos;
using JLL.PizzaProblem.API.Filters;
using JLL.PizzaProblem.API.Middleware;
using JLL.PizzaProblem.Domain;
using JLL.PizzaProblem.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JLL.PizzaProblem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IOptions<AppSettings> appSettings, 
            IUserService userService,
            IMapper mapper)
        {
            _appSettings = appSettings.Value;
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllAsync()
        {
            var users = await _userService.GetAllAsync();
            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        [Authorize]
        [HttpGet("{Id}", Name = "GetUser")]
        public async Task<ActionResult<List<UserDto>>> GetUserAsync(int Id)
        {
            var user = await _userService.GetByIdAsync(Id);
            
            if(user == null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet("GetTopTenUser")]
        public async Task<ActionResult<List<UserDto>>> GetTopTenUserAsync()
        {
            var users = await _userService.GetTopTenPizzaLoveAsync();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUserAsync(UserForCreationDto newUser)
        {
            var user = await _userService.AddNewUserAsync(_mapper.Map<User>(newUser));

            return CreatedAtRoute("GetUser",
                new { user.Id },
                _mapper.Map<UserDto>(user));
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponseDto>> AuthenticateAsync(AuthenticateRequestDto model)
        {
            var request = _mapper.Map<AuthenticateRequest>(model);
            var response = await _userService.AuthenticateAsync(request);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            response.Token = JwtTokenGenerator.GenerateJwtToken(response.Id, _appSettings.Secret);

            return Ok(_mapper.Map<AuthenticateResponseDto>(response));
        }

        [HttpPatch("{Id}")]
        [Authorize]
        public async Task<IActionResult> PatchAsync(int Id, [FromBody] JsonPatchDocument<UserForPatchDto> patchDoc)
        {
            var user = await _userService.GetByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            if(patchDoc != null)
            {
                // Update entity fields
                var tmp = _mapper.Map <UserForPatchDto>(user);
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
