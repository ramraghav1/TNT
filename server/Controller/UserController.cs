using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Business.Services;
using Bussiness.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]  // GET at api/user
        [Route("GetAll")] // Optional, explicitly maps to api/user
        public IActionResult GetAllUsers()
        {
            return Ok(_userService.GetAllUserInformation());
        }

        // POST: api/user
        [HttpPost]
        [Route("Add")]
        public IActionResult AddUser([FromBody] InsertUserInformation user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            return Ok(_userService.AddUser(user));
            
        }

        

    }
}
