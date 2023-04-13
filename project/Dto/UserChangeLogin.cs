using System.ComponentModel.DataAnnotations;

namespace project.Dto;

public class UserChangeLogin
{
    [RegularExpression("[a-zA-Z\\d]+")]
    public string Value { get; set; }
}