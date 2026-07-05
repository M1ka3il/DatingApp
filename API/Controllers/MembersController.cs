using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [Authorize]
    public class MembersController(
        AppDbContext context,
        IMapper mapper,
        IFileStorageService fileStorage) : BaseAPIController
    {
        #region "User"
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MemberDTO>>> GetMembers([FromQuery] UserParams userParams)
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userParams.Search))
            {
                var search = userParams.Search.ToLower();
                query = query.Where(u => u.UserName.ToLower().Contains(search));
            }

            var descending = userParams.Direction.ToLower() == "desc";
            query = userParams.OrderBy.ToLower() switch
            {
                "username" => descending
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName),
                _ => descending
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName),
            };

            var members = await PagedList<MemberDTO>.CreateAsync(
                query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider),
                userParams.PageNumber,
                userParams.PageSize);

            Response.AddPaginationHeader(members);

            return members;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]  //localhost:5001/api/members/jane-id
        public async Task<ActionResult<MemberDTO>> GetMemberByID(Guid id)
        {
            var member = await context.Users
                .Where(u => u.Id == id)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
            if (member == null)
            {
                return NotFound();
            }
            return member;
        }
        #endregion

        #region "Photos"
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await context.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.Id == User.GetUserId());
            if (user == null) return BadRequest("Cannot update user");

            var origin = $"{Request.Scheme}://{Request.Host}";
            var result = await fileStorage.SaveAsync(file, origin);

            var photo = new Photo
            {
                Url = result.Url,
                PublicId = result.PublicId,
                // First photo becomes the main one automatically.
                IsMain = user.Photos.Count == 0
            };

            user.Photos.Add(photo);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMemberByID), new { id = user.Id }, mapper.Map<PhotoDTO>(photo));
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await context.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.Id == User.GetUserId());
            if (user == null) return BadRequest("Cannot update user");

            var photo = user.Photos.SingleOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.SingleOrDefault(p => p.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await context.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.Id == User.GetUserId());
            if (user == null) return BadRequest("Cannot update user");

            var photo = user.Photos.SingleOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            await fileStorage.DeleteAsync(photo.Url, photo.PublicId);
            user.Photos.Remove(photo);

            await context.SaveChangesAsync();
            return Ok();
        }
        #endregion
    }
}