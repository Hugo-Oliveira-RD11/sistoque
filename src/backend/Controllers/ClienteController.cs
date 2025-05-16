using backend.Models;
using backend.Services.Clientes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
#if !DEBUG
[Authorize]
#endif
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService clienteService) =>
        _clienteService = clienteService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> ObterTodos()
    {
        var clientes = await _clienteService.ObterTodosAsync();

        if (clientes == null)
        {
            return NotFound();
        }

        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> ObterPorId(Guid id)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id);

        if (cliente == null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<Cliente>> ObterPorEmail(string email)
    {
        var cliente = await _clienteService.ObterPorEmailAsync(email);

        if (cliente == null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }

    [HttpGet("cpf-cnpj/{cpfCnpj}")]
    public async Task<ActionResult<Cliente>> ObterPorCpfCnpj(string cpfCnpj)
    {
        var cliente = await _clienteService.ObterPorCpfCnpjAsync(cpfCnpj);

        if (cliente == null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Cliente>> Adicionar([FromBody] Cliente cliente)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _clienteService.AdicionarAsync(cliente);
            return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] Cliente cliente)
    {
        if (id != cliente.Id)
            return BadRequest("ID do cliente não corresponde ao ID na URL.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _clienteService.AtualizarAsync(cliente);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        try
        {
            await _clienteService.RemoverIdAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
            return BadRequest("Email e senha são obrigatórios.");

        try
        {
            var token = await _clienteService.LoginAsync(request.Email, request.Senha);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
            return BadRequest("Token é obrigatório.");

        try
        {
            await _clienteService.LogoutAsync(request.Token);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Senha { get; set; }
}

public class LogoutRequest
{
    public string Token { get; set; }
}
