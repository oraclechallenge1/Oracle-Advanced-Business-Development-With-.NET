namespace MedSave.DTOs.UsersSys;

public class CreateUserRequest
{
    public ContactUserDTO ContactUserDto { get; set; } = default!;
    public UsersSysDTO UsersSysDto { get; set; } = default!;
}