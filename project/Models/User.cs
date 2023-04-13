using System.ComponentModel.DataAnnotations;

namespace project.Models;

public class User
{
    [Key]
    public Guid Guid { get; set; }
    [RegularExpression("[a-zA-Z\\d]")]
    public string Login { get; set; }
    [RegularExpression("[a-zA-Z\\d]")]
    public string Password { get; set; }
    [RegularExpression("[a-zA-Zа-яА-ЯёЁ]")]
    public string Name { get; set; }
    [RegularExpression("[0-2]")]
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool Admin { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string? RevokedBy { get; set; }
}