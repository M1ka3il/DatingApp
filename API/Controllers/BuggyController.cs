using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Endpoints used to verify the client/server error-handling pipeline.
public class BuggyController(AppDbContext context) : BaseAPIController
{
  [Authorize]
  [HttpGet("auth")]
  public IActionResult GetAuth()
  {
    return Ok("secret text");
  }

  [HttpGet("not-found")]
  public IActionResult GetNotFound()
  {
    var thing = context.Users.Find(Guid.NewGuid());
    if (thing == null) return NotFound();
    return Ok(thing);
  }

  [HttpGet("server-error")]
  public IActionResult GetServerError()
  {
    var thing = context.Users.Find(Guid.NewGuid()) ?? throw new Exception("A bad thing has happened");
    return Ok(thing);
  }

  [HttpGet("bad-request")]
  public IActionResult GetBadRequest()
  {
    return BadRequest("This was not a good request");
  }
}
