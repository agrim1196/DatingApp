using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOS;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{  
    
    
        [Authorize]
        [Route("api/[controller]")]
        [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        
     


        [HttpGet]
        public async Task<IActionResult> getUser() 
        {
             var users = await _repo.GetUsers();
             var userslist = _mapper.Map<IEnumerable<UserForList>>(users);
             return Ok(userslist);       
 
        }

        [HttpGet("{id}")]
         public async Task<IActionResult> getUser(int id) 
        {
             var users = await _repo.GetUser(id);

             var userforDetail = _mapper.Map<UserForDetailsDTO>(users);
             return Ok(userforDetail);       
 
        }

       /*    [HttpPost]
         public IActionResult samplecall() 
        {

            return Ok();
        }
        */
        

    }
}