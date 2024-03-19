using System.Text.RegularExpressions;
using BlogEF3.Data;
using BlogEF3.Extensions;
using BlogEF3.Models;
using BlogEF3.Services;
using BlogEF3.ViewModels;
using BlogEF3.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace BlogEF3.Controllers;

[ApiController]
public class AccountController : ControllerBase
{

    [HttpPost("v1/accounts")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] EmailService emailService,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<List<string>>(ModelState.GetErrors()));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-")
        };

        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(
                user.Name,
                user.Email,
                subject: "Bem-vindo ao blog!",
                body: $"Sua senha é <strong>{password}</strong>"
            );

            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email,
                password
            })); //pra não criar um viewmodel só pra esse retorno ai, foi usado o dynamic
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("Este E-mail já está cadastrado."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor."));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] BlogDataContext context,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<List<string>>(ModelState.GetErrors()));

        var user = await context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("O usuário ou senha inválidos."));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("O usuário ou senha inválidos."));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor."));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,")
            .Replace(model.Base64Image, "");

        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna do Servidor."));
        }

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
            return NotFound(new ResultViewModel<User>("Usuário não encontrado."));

        user.Image = $"http://localhost:0000/images/{fileName}";

        try
        {
            context.Update(user);
            await context.SaveChangesAsync();
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna do Servidor."));
        }

        return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!"));
    }
}