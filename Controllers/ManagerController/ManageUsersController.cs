using EDIBANK.Conf_Db_With_Entity;
using EDIBANK.Models.Users_EdiWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EDIBANK.Controllers.ManagerController;

// Instance of the ASP.NET Core Identity UserManager available in the controller
[Authorize(Roles = "Admin")]
public class ManageUsersController(AppDbContext context, UserManager<ApplicationUser> usrMgr, IPasswordHasher<ApplicationUser> passwordHash, RoleManager<IdentityRole> roleMgr) : Controller
{
    private readonly AppDbContext _context = context;

    private readonly UserManager<ApplicationUser> _userManager = usrMgr;

    private IPasswordHasher<ApplicationUser> _passwordHasher = passwordHash;

    private RoleManager<IdentityRole> _roleManager = roleMgr;

    //use the GetUserAsync method to retrieve an instance of ApplicationIdentityUser for the currently logged in user.
    //This means making the controller method asynchronous so that we can await the call to GetUserAsync 
    public async Task<IActionResult> Index()
    {
        ApplicationUser[] admin = (await _userManager
            .GetUsersInRoleAsync("Admin"))
            .ToArray();

        ApplicationUser[] everyone = await _userManager.Users
            .ToArrayAsync();

        ManageUsersViewModel model = new()
        {
            Admin = admin,
            Everyone = everyone
        };

        return View(model);
    }

    // Get all Identity Roles

    // Create User
    public async Task<IActionResult> CreateAsync()
    {
        string ediId = (await _context.EDISelectorAsync()).First().Value;
        // Get all roles
        var roles = await _roleManager.Roles.ToListAsync();

        // Convert roles to SelectListItem for the dropdown
        var roleSelectListItems = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();

        return View(new Users
        {
            Name = "",
            Email = null,
            Password = "",
            DescripUser = null,
            SelectedRole = "", 
            Roles = roleSelectListItems,
            EDIs = await _context.EDISelectorAsync(ediId),
            EDIId = ediId
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Users user)
    {
        if (!ModelState.IsValid)
        {
            user.EDIs = await _context.EDISelectorAsync(user.EDIId);
            return View(user);
        }

        ApplicationUser appUser = new()
        {
            UserName = user.Email,
            Email = user.Email,
            DescripUser = user.DescripUser,
            EDIId = user.EDIId,
            TwoFactorEnabled = true,
            EmailConfirmed = true,
            LockoutEnabled = true
        };
        IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

        if (result.Succeeded)
        {
            // Set the user role
            await _userManager.AddToRoleAsync(appUser, user.SelectedRole);

            //  var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            //  var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);
            //   EmailHelper emailHelper = new EmailHelper();
            //  bool emailResponse = emailHelper.SendEmail(user.Email, confirmationLink);

            //  if (emailResponse)
            // return RedirectToAction("Index");
            // else
            //   {
            // log email failed 
            // }
            return RedirectToAction("Index");
        }
        user.EDIs = await _context.EDISelectorAsync(user.EDIId);
        foreach (IdentityError error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
            ModelState.AddModelError(string.Empty, error.Code switch
            {
                "DuplicateEmail" => "El correo electrónico ya está en uso.",
                "PasswordTooShort" => "La contraseña debe tener al menos 8 caracteres.",
                _ => "Ocurrió un error al crear el usuario."
            });
        }
        return View(user);
    }

    //Update
    public async Task<IActionResult> Update(string id)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
            return View(user);
        else
            return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(string id, string email, string? password, string DescripUser)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            if (!string.IsNullOrEmpty(email))
                user.Email = email;

            // Verificar si el username es diferente del nuevo email
            if (user.UserName != email)
            {
                // Actualizar el username con el nuevo email
                user.UserName = email;
            }

            if (!string.IsNullOrEmpty(DescripUser))
                user.DescripUser = DescripUser;
            else
                ModelState.AddModelError("", "La Descripcion del usuario no puede estar vacío");

            if (!string.IsNullOrEmpty(password))
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
            IdentityResult resultadoPass = await _userManager.UpdateAsync(user);

            if (resultadoPass.Succeeded)
                return RedirectToAction("Index");

            if (!string.IsNullOrEmpty(email) && string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(DescripUser))
            {
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
        }
        else
            ModelState.AddModelError("", "Usuario no encontrado");
        return View(user);
    }

    private void Errors(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
            ModelState.AddModelError("", error.Description);
    }

    // Delete

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            IdentityResult result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                Errors(result);
        }
        else
            ModelState.AddModelError("", "Usuario no encontrado");
        return View("Index", _userManager.Users);
    }

    // UnBlockUser

    //  [HttpPost]
    public async Task<IActionResult> UnblockUser(string id)

    {
        // Encuentra al usuario por su ID
        ApplicationUser user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            // Manejar el caso en que el usuario no se encuentra
            return NotFound();
        }

        // Verificar si el usuario está bloqueado
        if (await _userManager.IsLockedOutAsync(user))
        {
            // Desbloquear al usuario
            //  await _userManager.SetLockoutEnabledAsync(user, false);
            await _userManager.ResetAccessFailedCountAsync(user);
            // Volver a habilitar el bloqueo para futuros intentos fallidos
            await _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));

            // Actualizar la última fecha de inicio de sesión para reiniciar el temporizador de bloqueo
            await _userManager.UpdateSecurityStampAsync(user);

            // Agregar un mensaje de éxito o redireccionar a una página de confirmación
            return RedirectToAction("Index", "ManageUsers", new { message = "Usuario desbloqueado correctamente" });
        }
        else
        {
            // Agregar un mensaje de error indicando que el usuario no estaba bloqueado
            return RedirectToAction("Index", "ManageUsers", new { message = "El usuario ya no estaba bloqueado" });
        }
    }
}