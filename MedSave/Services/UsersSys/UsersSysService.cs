using MedSave.Context;
using MedSave.DTOs;
using MedSave.DTOs.UsersSys;
using MedSave.Model;
using MedSave.Repositories.UsersSys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedSave.Services.UsersSys;

public class UsersSysService : IUsersSysService
{
    private readonly MedSaveContext _context;
    private readonly IUsersSysRepository _usersSysRepository;
    private readonly IContactUserRepository _contactUserRepository;

    public UsersSysService(MedSaveContext context, IUsersSysRepository usersSysRepository, IContactUserRepository contactUserRepository)
    {
        _context = context;
        _usersSysRepository = usersSysRepository;
        _contactUserRepository = contactUserRepository;
    }
    
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) {}
    }
    
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) {}
    }

    public async Task<IEnumerable<UsersSysDTO>> GetAllAsync()
    {
        var users = await _usersSysRepository.GetAllAsync();

        return users.Select(user => new UsersSysDTO
        {
            ContactUserId = user.ContactUserId,
            Email = user.Email,
            NameUser = user.NameUser,
            PasswordUser = user.PasswordUser,
            ProfUserId = user.ProfUserId,
            RoleUserId = user.RoleUserId,
            UserId = user.UserId
        }).ToList();
    }

    public async Task<CreateUserRequest?> GetByIdAsync(long id)
    {
        var user = await _usersSysRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException($"User with Id {id} not found");
        }

        var contact = await _contactUserRepository.GetByIdAsync(user.ContactUserId);

        if (contact == null)
        {
            throw new NotFoundException($"Contact User with Id {user.ContactUserId} not found");
        }

        var userDTO = new UsersSysDTO
        {
            ContactUserId = user.ContactUserId,
            Email = user.Email,
            NameUser = user.NameUser,
            PasswordUser = user.PasswordUser,
            ProfUserId = user.ProfUserId,
            RoleUserId = user.RoleUserId,
            UserId = user.UserId
        };

        var contactDTO = new ContactUserDTO
        {
            ContactUserId = contact.ContactUserId,
            EmailUser = contact.EmailUser,
            PhoneNumberUser = contact.PhoneNumberUser
        };

        return new CreateUserRequest
        {
            ContactUserDto = contactDTO,
            UsersSysDto = userDTO
        };
    }

    public async Task<UsersSysDTO?> AddAsync(CreateUserRequest createUserRequest)
    {
        if (createUserRequest.UsersSysDto == null) throw new ArgumentNullException(nameof(createUserRequest), "UsersSysDto can't be null");

        if (createUserRequest.UsersSysDto.NameUser == null) throw new ArgumentNullException(nameof(createUserRequest.UsersSysDto), "NameUser can't be null");
        
        if (createUserRequest.UsersSysDto.Email == null) throw new ArgumentNullException(nameof(createUserRequest.UsersSysDto), "Email can't be null");
        
        if (createUserRequest.UsersSysDto.PasswordUser == null) throw new ArgumentNullException(nameof(createUserRequest.UsersSysDto), "PasswordUser can't be null");
        
        if (createUserRequest.UsersSysDto.RoleUserId == 0) throw new ArgumentNullException(nameof(createUserRequest.UsersSysDto), "RoleUserId can't be 0");
        
        if (createUserRequest.UsersSysDto.ProfUserId == 0) throw new ArgumentNullException(nameof(createUserRequest.UsersSysDto), "ProfUserId can't be null");

        
        if (createUserRequest.ContactUserDto == null) throw new ArgumentNullException(nameof(createUserRequest), "ContactUserDto can't be null");
        
        if (createUserRequest.ContactUserDto.PhoneNumberUser == 0) throw new ArgumentNullException(nameof(createUserRequest), "PhoneNumberUser can't be 0");

        
        var search = await _context.ContactUser.FirstOrDefaultAsync(c => c.EmailUser == createUserRequest.ContactUserDto.EmailUser);
        
        if (search != null) throw new ConflictException("Email Already registered");
        
        
        var search2 = await _context.ContactUser.FirstOrDefaultAsync(c => c.PhoneNumberUser == createUserRequest.ContactUserDto.PhoneNumberUser);
        
        if (search2 != null) throw new ConflictException("Phone Number Already registered");
        
        
        var contact = new ContactUser
        {
            EmailUser = createUserRequest.UsersSysDto.Email,
            PhoneNumberUser = createUserRequest.ContactUserDto.PhoneNumberUser
        };

        await _contactUserRepository.AddAsync(contact);

        var user = new Model.UsersSys
        {
            Email = createUserRequest.UsersSysDto.Email,
            NameUser = createUserRequest.UsersSysDto.NameUser,
            PasswordUser = BCrypt.Net.BCrypt.HashPassword(createUserRequest.UsersSysDto.PasswordUser),
            RoleUserId = createUserRequest.UsersSysDto.RoleUserId,
            ProfUserId = createUserRequest.UsersSysDto.ProfUserId,
            ContactUserId = contact.ContactUserId
        };

        await _usersSysRepository.AddAsync(user);

        return new UsersSysDTO
        {
            Email = user.Email,
            NameUser = user.NameUser,
            RoleUserId = user.RoleUserId,
            ProfUserId = user.ProfUserId,
            ContactUserId = user.ContactUserId
        };
    }

    public async Task UpdateAsync(long id, UsersSysDTO usersSysDto)
    {
        if (usersSysDto == null) throw new ArgumentNullException(nameof(usersSysDto), "Users Sys DTO can't be null");

        if (usersSysDto.NameUser == null) throw new ArgumentNullException(nameof(usersSysDto), "NameUser can't be null");
        
        if (usersSysDto.Email == null) throw new ArgumentNullException(nameof(usersSysDto), "Email can't be null");
        
        if (usersSysDto.PasswordUser == null) throw new ArgumentNullException(nameof(usersSysDto), "PasswordUser can't be null");
        
        if (usersSysDto.RoleUserId == 0) throw new ArgumentNullException(nameof(usersSysDto), "RoleUserId can't be 0");
        
        if (usersSysDto.ProfUserId == 0) throw new ArgumentNullException(nameof(usersSysDto), "ProfUserId can't be null");

        
        var search = await _usersSysRepository.GetByIdAsync(id);

        if (search == null) throw new NotFoundException($"User with id {id} not found");

        usersSysDto.UserId = id;

        search.NameUser = usersSysDto.NameUser;
        search.Email = usersSysDto.Email;
        search.PasswordUser = usersSysDto.PasswordUser;
        search.RoleUserId = usersSysDto.RoleUserId;
        search.ProfUserId = usersSysDto.ProfUserId;

        await _usersSysRepository.UpdateAsync(search);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _usersSysRepository.GetByIdAsync(id);

        if (user == null) throw new NotFoundException($"User with id {id} not found");

        var contact = await _contactUserRepository.GetByIdAsync(user.ContactUserId);

        await _usersSysRepository.DeleteAsync(id);

        if (contact != null) await _contactUserRepository.DeleteAsync(contact.ContactUserId);
    }

    public async Task<PagedResult<UsersSysDTO>> SearchAsync(string? nameUser,
        string? email,
        long? roleUserId,
        long? profUserId,
        int page,
        int pageSize,
        string sortBy,
        string sortDir)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var (items, total) = await _usersSysRepository.SearchAsync(nameUser, email, roleUserId, profUserId, page, pageSize, sortBy ?? "userId", sortDir ?? "asc");

        var dtoItems = items.Select(userSys => new UsersSysDTO
        {
            ContactUserId = userSys.ContactUserId,
            Email = userSys.Email,
            NameUser = userSys.NameUser,
            PasswordUser = userSys.PasswordUser,
            ProfUserId = userSys.ProfUserId,
            RoleUserId = userSys.RoleUserId,
            UserId = userSys.UserId
        }).ToList();

        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        
        return new PagedResult<UsersSysDTO>
        {
            Items = dtoItems,
            PageInfo = new PageInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages
            }
        };
    }
}