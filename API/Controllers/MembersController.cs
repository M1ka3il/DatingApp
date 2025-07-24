using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class MembersController(AppDbContext context) : BaseAPIController
    {
        #region "User"
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await context.Users.ToListAsync();
            return members;
        }

        [HttpGet("{id}")]  //localhost:5001/api/members/jane-id
        public async Task<ActionResult<AppUser>> GetMemberByID(string id)
        {
            var member = await context.Users.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return member;
        }
        #endregion 
    }
}