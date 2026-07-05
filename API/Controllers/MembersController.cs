using API.Data;
using API.DTO;
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
        public async Task<ActionResult<IReadOnlyList<MemberDTO>>> GetMembers()
        {
            var members = await context.Users
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(members);
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