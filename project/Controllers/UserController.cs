using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using project.Data;
using project.Dto;
using project.Models;

namespace project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserDbRepository _userDbRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserController(IUserDbRepository userDbRepository, IMapper mapper, 
        IConfiguration configuration)
    {
        _userDbRepository = userDbRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userDbRepository.GetAll();
        return Ok(users);
    }
    
    [HttpGet("{login}")]
    public async Task<IActionResult> GetUser(string login)
    {
        if (!_userDbRepository.IsUserExist(login)) 
            return NotFound("Пользователь не найден");
        
        var user = _mapper.Map<UserDto>(await _userDbRepository.GetByLogin(login));

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(user);
    }

    [HttpPost("createUser")]
    public IActionResult Create(CreateUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (_userDbRepository.IsUserExist(userDto.Login))
        {
            return BadRequest($"Пользователь с логином {userDto.Login} уже существует");
        }

        var user = _mapper.Map<User>(userDto);
        user.CreatedOn = DateTime.Now;

        if (!_userDbRepository.Add(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Пользователь успешно создан!");
    }

    [HttpPost("createUserByAdmin")]
    public IActionResult CreateByAdmin(User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (_userDbRepository.IsUserExist(user.Login))
        {
            return BadRequest($"Пользователь с логином {user.Login} уже существует");
        }

        if (!_userDbRepository.Add(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Пользователь успешно создан!");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(string login, string password, UserCredentialsDto userCredentials)
    {
        if (!_userDbRepository.IsUserExist(login))
            return BadRequest("Пользователь не найден");

        var user = await _userDbRepository.GetByLogin(login);
        if (user.Password != password)
        {
            return BadRequest("Неверный пароль");
        }

        return Ok();
    }
    
    [HttpPost("edit")]
    public async Task<IActionResult> EditMainInfo(string login, string password, EditUserInfoDto editUserInfoDto)
    {
        if (!_userDbRepository.IsUserExist(login))
        {
            return BadRequest($"Пользователь с логином {login} не существует");
        }

        var user = await _userDbRepository.GetByLogin(login);
        if (password != user.Password)
        {
            return BadRequest("Неверный пароль!");
        }
        
        if (user.RevokedOn != null)
        {
            return BadRequest($"Пользователь с логином {user.Login} удалён");
        }
        
        user.Name = editUserInfoDto.Name;
        user.Gender = editUserInfoDto.Gender;
        user.Birthday = editUserInfoDto.Birthday;
    
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = user.Login;
    
        _userDbRepository.Update(user);
        return Ok(user);
    }
    
    // [HttpPost("editByAdmin")]
    // public async Task<IActionResult> EditByAdmin(string name, int gender, DateTime? birthday, string userToModify)
    // {
    //     if (!_userDbRepository.IsUserExist(userToModify))
    //     {
    //         return BadRequest($"Пользователь с логином {userToModify} не существует");
    //     }
    //     
    //     var user = await _userDbRepository.GetByLogin(userToModify);
    //     
    //     user.Name = name;
    //     user.Gender = gender;
    //     user.Birthday = birthday;
    //
    //     user.ModifiedOn = DateTime.Now;
    //     user.ModifiedBy = (await _userDbRepository.GetByGuid(authorizedUserGuid))?.Login;
    //
    //     _userDbRepository.Update(user);
    //     return Ok(user);
    // }
}