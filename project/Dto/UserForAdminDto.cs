namespace project.Dto;

public class UserForAdminDto
{
    public string Login { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime? RevokedOn { get; set; }
}