using API.Data;
using API.DTO;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    [Authorize]
    public class MembersController(AppDbContext context, IMapper mapper) : BaseAPIController
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
    }
}