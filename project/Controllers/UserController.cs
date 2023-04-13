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

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userDbRepository.GetAll();
        return Ok(users);
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

    [HttpPost("createByAdmin")]
    public async Task<IActionResult> CreateByAdmin(string login, string password, User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");

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
    
        _userDbRepository.Update(user);
        return Ok(user);
    }
    
    [HttpPost("editMainInfoByAdmin")]
    public async Task<IActionResult> EditMainInfoByAdmin(string login, string password, 
        EditUserInfoDto editUserInfoDto, string userToModify)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userToModify))
        {
            return BadRequest($"Пользователь с логином {userToModify} не существует");
        }
        
        var user = await _userDbRepository.GetByLogin(userToModify);
        
        user.Name = editUserInfoDto.Name;
        user.Gender = editUserInfoDto.Gender;
        user.Birthday = editUserInfoDto.Birthday;
        
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
    
        _userDbRepository.Update(user);
        return Ok(user);
    }

    [HttpPost("changePassword")]
    public async Task<IActionResult> ChangePassword(string login, string password, string newPassword)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;

        if (_userDbRepository.IsUserDeleted(login))
            return BadRequest($"Пользователь с логином {login} удалён");

        var user = loginResult.Value;

        user.Password = newPassword;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        _userDbRepository.Update(user);
        return Ok(user);
    }
    
    [HttpPost("changePasswordByAdmin")]
    public async Task<IActionResult> ChangePasswordByAdmin(string login, string password, 
        string userToModify, string newPassword)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userToModify))
        {
            return BadRequest($"Пользователь с логином {userToModify} не существует");
        }
        
        var user = await _userDbRepository.GetByLogin(userToModify);

        user.Password = newPassword;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        _userDbRepository.Update(user);
        return Ok(user);
    }
    
    [HttpPost("changeLogin")]
    public async Task<IActionResult> ChangeLogin(string login, string password, string newLogin)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;

        if (_userDbRepository.IsUserDeleted(login))
            return BadRequest($"Пользователь с логином {login} удалён");

        var user = loginResult.Value;

        user.Login = newLogin;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        _userDbRepository.Update(user);
        return Ok(user);
    }
    
    [HttpPost("changeLoginByAdmin")]
    public async Task<IActionResult> ChangeLoginByAdmin(string login, string password, 
        string userToModify, string newLogin)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userToModify))
        {
            return BadRequest($"Пользователь с логином {userToModify} не существует");
        }
        
        var user = await _userDbRepository.GetByLogin(userToModify);

        user.Login = newLogin;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = login;
        _userDbRepository.Update(user);
        return Ok(user);
    }

    [HttpGet("getAllActiveByAdmin")]
    public async Task<IActionResult> GetAllActiveUsersByAdmin(string login, string password)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
                
        var activeUsers = (await _userDbRepository.GetAllActive())?.OrderBy(u=>u.CreatedOn).ToList(); 
        return Ok(activeUsers);
    }
    
    
    [HttpGet("{userLoginToGet}")]
    public async Task<IActionResult> GetUser(string login, string password, string userLoginToGet)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLoginToGet)) 
            return NotFound($"Пользователь {userLoginToGet} не существует");
        
        var user = _mapper.Map<UserForAdminDto>(await _userDbRepository.GetByLogin(userLoginToGet));
        
        return Ok(user);
    }

    [HttpGet("{login}")]
    public async Task<IActionResult> GetUser(string login, string password)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserExist(login)) 
            return NotFound($"Пользователь {login} не существует");
        
        if (_userDbRepository.IsUserDeleted(login))
            return BadRequest($"Пользователь с логином {login} удалён");
        
        var user = _mapper.Map<UserForAdminDto>(await _userDbRepository.GetByLogin(login));
        
        return Ok(user);
    }

    [HttpGet("olderThan-{age}")]
    public async Task<IActionResult> GetOlderThan(string login, string password, int age)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        var users = _mapper.Map<IEnumerable<UserForAdminDto>>(await _userDbRepository.GetAllOlderThan(age));
        return Ok(users);
    }

    [HttpPost("{userLoginToDelete}/softDelete")]
    public async Task<IActionResult> SoftDeleteByAdmin(string login, string password, 
        string userLoginToDelete)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLoginToDelete)) 
            return NotFound($"Пользователь {userLoginToDelete} не существует");

        var user = await _userDbRepository.GetByLogin(userLoginToDelete);

        user.RevokedOn = DateTime.Now;
        user.RevokedBy = login;
        
        _userDbRepository.Update(user);
        return Ok();
    }
    
    [HttpPost("{userLoginToDelete}/hardDelete")]
    public async Task<IActionResult> HardDeleteByAdmin(string login, string password, 
        string userLoginToDelete)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLoginToDelete)) 
            return NotFound($"Пользователь {userLoginToDelete} не существует");

        var user = await _userDbRepository.GetByLogin(userLoginToDelete);

        _userDbRepository.Remove(user);
        return Ok();
    }

    [HttpPost("{userLoginToRestore}/restore")]
    public async Task<IActionResult> RestoreByAdmin(string login, string password,
        string userLoginToRestore)
    {
        var loginResult = await Login(login, password);
        if (loginResult.Value == null) return loginResult.Result;
        
        if (!_userDbRepository.IsUserAdmin(login))
            return BadRequest($"Пользователь {login} не является администратором");
        
        if (!_userDbRepository.IsUserExist(userLoginToRestore)) 
            return NotFound($"Пользователь {userLoginToRestore} не существует");
        
        var user = await _userDbRepository.GetByLogin(userLoginToRestore);

        user.RevokedOn = null;
        user.RevokedBy = null;
        
        _userDbRepository.Update(user);
        return Ok();
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