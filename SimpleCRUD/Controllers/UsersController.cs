using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleCRUD.Data;
using SimpleCRUD.Dtos;
using SimpleCRUD.Models;
using static SimpleCRUD.ConnectionString;

namespace SimpleCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly ICrudRepository _repo;

        public UsersController(IOptions<ConnectionStrings> connectionStrings,
            ICrudRepository repo)
        {
            _connectionStrings = connectionStrings.Value;
            _repo = repo;
        }

        // test the connection
        [HttpGet, Route("test")]
        public IActionResult Test()
        {
            return Ok("test");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = new User
            {
                Name = userForRegisterDto.Name,
                Address = userForRegisterDto.Address
            };
            await _repo.Register(userToCreate);

            return StatusCode(201);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.Delete(id);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = _repo.GetUsers();

            return Ok(users);
        }
    }
}