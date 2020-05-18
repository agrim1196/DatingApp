
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOS;
using DatingApp.API.Helpers;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace DatingApp.API.Controllers
{
        [Authorize]
        [Route("api/users/{userId}/photo")]
        [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryconfig;
        private Cloudinary _cloudinary;

       public PhotoController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryconfig) 
       {
            _cloudinaryconfig = cloudinaryconfig;
            _mapper = mapper;
            _repo = repo;

 
            Account acc= new Account(
              _cloudinaryconfig.Value.CloudName,
              _cloudinaryconfig.Value.APIKey,
              _cloudinaryconfig.Value.APISecretKey
              
            );

            _cloudinary = new Cloudinary(acc);
       }

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo =_mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto _photoForCreationDto)       
        {

             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
              return Unauthorized();

              var userFromRepo = await _repo.GetUser(userId);
              var file = _photoForCreationDto.File;
              var uploadResult = new ImageUploadResult();
              
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            _photoForCreationDto.Url = uploadResult.Uri.ToString();
            _photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(_photoForCreationDto);

            var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);

            if (!userFromRepo.Photos.Any(p => p.IsMain))
               photo.IsMain = true;
            

               userFromRepo.Photos.Add(photo);

            if (await _repo.SaveUsers())
            {
                return CreatedAtAction("GetPhoto", new {id = photo.Id }, photoToReturn);
            }

            return BadRequest("Photo not added!");

        }

         [HttpPost("{id}/SetMain")]
          public async Task<IActionResult> setMainPhoto(int userid,int id)
          {
             if(userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             {
                return Unauthorized();
             }

             var user = await _repo.GetUser(userid);

             if(!user.Photos.Any(p => p.Id == id))
               return Unauthorized();

             var photo = await _repo.GetPhoto(id);

             if(photo.IsMain)
               return BadRequest("Main photo is already set");

            var currentmainphoto = await _repo.GetMainPhotoForUser(userid);
            currentmainphoto.IsMain = false;

            photo.IsMain = true;

            if(await _repo.SaveUsers())
            {
                return NoContent();
            }

             return BadRequest("Could not set main photo");

          }


          [HttpPost("{id}/DeletePhoto")]
           public async Task<IActionResult> DeletePhoto(int userid,int id){           
            if(userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             {
                return Unauthorized();
             }

             var user = await _repo.GetUser(userid);

             if(!user.Photos.Any(p => p.Id == id))
               return Unauthorized();

             var photo = await _repo.GetPhoto(id);

             if(photo.IsMain)
               return BadRequest("Main photo is already set");

             if(photo.PublicId != null) 
              {
              var deleteParams= new DeletionParams(photo.PublicId);
              var result = _cloudinary.Destroy(deleteParams);
              if(result.Result == "ok")
                  _repo.Delete(photo);
              }
               if (await _repo.SaveUsers())
                return Ok();


              return BadRequest("There was a problem in deleting photo");      
          }
    }

    
}