using backend.Models;
using backend.Services.Produtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
#if !DEBUG
[Authorize]
#endif
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutoController(IProdutoService produtoService) =>
        _produtoService = produtoService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> ObterTodos()
    {
        var produtos = await _produtoService.ObterTodosAsync();

        if (produtos == null)
            return NotFound();

        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Produto>> ObterPorId(Guid id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);

        if (produto == null)
            return NotFound();

        return Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> Adicionar([FromBody] Produto produto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _produtoService.AdicionarProdutoAsync(produto);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] Produto produto)
    {
        if (id != produto.Id)
            return BadRequest("ID do produto não corresponde ao ID na URL.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _produtoService.AtualizarProdutoAsync(produto);
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
            await _produtoService.RemoverProdutoAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
