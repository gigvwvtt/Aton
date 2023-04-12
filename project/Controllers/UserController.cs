using Microsoft.AspNetCore.Mvc;
using project.Data;
using project.Models;

namespace project.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserDbRepository _userDbRepository;
    
    public UserController(IUserDbRepository userDbRepository)
    {
        _userDbRepository = userDbRepository;
    }

    [HttpGet("allusers")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userDbRepository.GetAll();
        return Ok(users);
    }
    
    [HttpGet("{guid}")]
    [ProducesResponseType(200, Type = typeof(User))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser(Guid guid)
    {
        var user = await _userDbRepository.GetByGuid(guid);
        if (user == null) return NotFound("User not Found");
        
        return Ok(user);
    }

    [HttpPost("newuser")]
    public IActionResult Create(User user, string createdBy)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _userDbRepository.Add(user);
        return Ok();
    }
}