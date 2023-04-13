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
    private readonly IMapper _mapper;

    public UserController(IUserDbRepository userDbRepository, IMapper mapper)
    {
        _userDbRepository = userDbRepository;
        _mapper = mapper;
    }

    [HttpPost("create")]
    public IActionResult Create(CreateUserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (_userDbRepository.IsUserExist(userDto.Login))
        {
            return BadRequest($"Пользователь с логином {userDto.Login} уже существует");
        }

        var user = _mapper.Map<User>(userDto);
        user.Guid = Guid.NewGuid();
        user.CreatedOn = DateTime.Now;
        user.CreatedBy = user.Login;
        user.Admin = false;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = user.Login;

        if (!_userDbRepository.Add(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Пользователь успешно создан!");
    }

    [HttpPost("admin/create")]
    public async Task<IActionResult> CreateByAdmin(string login, string password, CreateUserDto userDto, bool admin)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");

        if (_userDbRepository.IsUserExist(userDto.Login))
        {
            return BadRequest($"Пользователь с логином {userDto.Login} уже существует");
        }

        var user = _mapper.Map<User>(userDto);
        user.Guid = Guid.NewGuid();
        user.CreatedOn = DateTime.Now;
        user.CreatedBy = login;
        user.Admin = admin;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;

        if (!_userDbRepository.Add(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Пользователь успешно создан!");
    }
    
    [HttpPost("editMainInfo")]
    public async Task<IActionResult> EditMainInfo(string login, string password, EditUserInfoDto editUserInfoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (_userDbRepository.IsUserDeleted(login))
            return BadRequest($"Пользователь с логином {login} удалён");
        
        var user = loginResult.Value;

        user.Name = editUserInfoDto.Name;
        user.Gender = editUserInfoDto.Gender;
        user.Birthday = editUserInfoDto.Birthday;
    
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = user.Login;
    
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok("Данные успешно обновлены!");
    }
    
    [HttpPost("admin/{userLogin}/editMainInfo")]
    public async Task<IActionResult> EditMainInfoByAdmin(string login, string password, 
        EditUserInfoDto editUserInfoDto, string userLogin)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLogin))
        {
            return BadRequest($"Пользователь с логином {userLogin} не существует");
        }
        
        var user = await _userDbRepository.GetByLogin(userLogin);
        
        user.Name = editUserInfoDto.Name;
        user.Gender = editUserInfoDto.Gender;
        user.Birthday = editUserInfoDto.Birthday;
        
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
    
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok("Данные успешно обновлены!");
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword(string login, string password, UserChangePassword newPassword)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;

        if (_userDbRepository.IsUserDeleted(login))
            return BadRequest($"Пользователь с логином {login} удалён");

        var user = loginResult.Value;

        user.Password = newPassword.Value;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok("Пароль успешно обновлён!");
    }
    
    [HttpPost("admin/{userLogin}/changePassword")]
    public async Task<IActionResult> ChangePasswordByAdmin(string login, string password, 
        string userLogin, UserChangePassword newPassword)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLogin))
        {
            return BadRequest($"Пользователь с логином {userLogin} не существует");
        }
        
        var user = await _userDbRepository.GetByLogin(userLogin);

        user.Password = newPassword.Value;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok($"Пароль пользователя {userLogin} успешно обновлён!");
    }
    
    [HttpPost("changeLogin")]
    public async Task<IActionResult> ChangeLogin(string login, string password, UserChangeLogin newLogin)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;

        if (_userDbRepository.IsUserDeleted(login))
            return BadRequest($"Пользователь с логином {login} удалён");

        var user = loginResult.Value;

        user.Login = newLogin.Value;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok("Логин успешно изменён!");
    }
    
    [HttpPost("admin/{userLogin}/changeLogin")]
    public async Task<IActionResult> ChangeLoginByAdmin(string login, string password, 
        string userLogin, UserChangeLogin newLogin)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLogin))
        {
            return BadRequest($"Пользователь с логином {userLogin} не существует");
        }
        
        var user = await _userDbRepository.GetByLogin(userLogin);

        user.Login = newLogin.Value;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok($"Логин пользователя {userLogin} успешно изменён!");
    }

    [HttpGet("admin/getAllActiveUsers")]
    public async Task<IActionResult> GetAllActiveUsersByAdmin(string login, string password)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
                
        var activeUsers = (await _userDbRepository.GetAllActive())?.OrderBy(u=>u.CreatedOn).ToList(); 
        return Ok(activeUsers);
    }
    
    [HttpGet("admin/{userLogin}/info")]
    public async Task<IActionResult> GetUserByAdmin(string login, string password, string userLogin)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLogin)) 
            return NotFound($"Пользователь {userLogin} не существует");
        
        var user = _mapper.Map<UserForAdminDto>(await _userDbRepository.GetByLogin(userLogin));
        
        return Ok(user);
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetUser(string login, string password)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserExist(login)) 
            return NotFound($"Пользователь {login} не существует");
        
        if (_userDbRepository.IsUserDeleted(login))
            return BadRequest($"Пользователь с логином {login} удалён");
        
        var user = _mapper.Map<UserDto>(await _userDbRepository.GetByLogin(login));
        
        return Ok(user);
    }

    [HttpGet("admin/olderThan-{age}")]
    public async Task<IActionResult> GetOlderThan(string login, string password, int age)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        var users = _mapper.Map<IEnumerable<UserForAdminDto>>(await _userDbRepository.GetAllOlderThan(age));
        return Ok(users);
    }

    [HttpPost("admin/{userLogin}/softDelete")]
    public async Task<IActionResult> SoftDeleteByAdmin(string login, string password, 
        string userLogin)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLogin)) 
            return NotFound($"Пользователь {userLogin} не существует");

        var user = await _userDbRepository.GetByLogin(userLogin);

        user.RevokedOn = DateTime.Now;
        user.RevokedBy = login;
        
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok($"Пользователь {userLogin} успешно \"мягко\" удалён!");
    }
    
    [HttpPost("admin/{userLogin}/hardDelete")]
    public async Task<IActionResult> HardDeleteByAdmin(string login, string password, 
        string userLogin)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLogin)) 
            return NotFound($"Пользователь {userLogin} не существует");

        var user = await _userDbRepository.GetByLogin(userLogin);

        if (!_userDbRepository.Remove(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok($"Пользователь {userLogin} успешно \"жёстко\" удалён!");
    }

    [HttpPost("admin/{userLogin}/restore")]
    public async Task<IActionResult> RestoreByAdmin(string login, string password,
        string userLogin)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLogin)) 
            return NotFound($"Пользователь {userLogin} не существует");
        
        var user = await _userDbRepository.GetByLogin(userLogin);

        user.RevokedOn = null;
        user.RevokedBy = null;
        
        if (!_userDbRepository.Update(user))
        {
            ModelState.AddModelError("", "Ошибка при сохранении в БД");
            return StatusCode(500, ModelState);
        }
        return Ok($"Пользователь {userLogin} успешно восстановлен!");
    }

    private async Task<ActionResult<User>> Login(string login, string password)
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

        return user;
    }
}