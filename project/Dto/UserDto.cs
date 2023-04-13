using System.ComponentModel.DataAnnotations;

namespace project.Dto;

public class UserDto
{
    [RegularExpression("[a-zA-Z\\d]+")]
    public string Login { get; set; }
    [RegularExpression("[a-zA-Zа-яА-ЯёЁ]+")]
    public string Name { get; set; }
    [RegularExpression("[0-2]")] 
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
}