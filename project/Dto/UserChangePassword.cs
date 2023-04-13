using System.ComponentModel.DataAnnotations;

namespace project.Dto;

public class UserChangePassword
{
    [RegularExpression("[a-zA-Z\\d]+")]
    public string Value { get; set; }
}