using Demo.Database;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductController(IProductRepository repository)
        => _repository = repository;


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var product = await _repository.GetAsync(id, cancellationToken);
        if(product is null)
        {
            return NotFound(id);
        }

        return Ok(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity
        });
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        => Ok(await _repository.ListAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Add(
        ProductRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Quantity = request.Quantity
        };

        await _repository.AddAsync(product, cancellationToken);

        return Ok(new ProductResponse
        {
            Id = product.Id,
            Name = request.Name,
            Quantity = request.Quantity
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        ProductRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var product = await _repository.GetAsync(id, cancellationToken);
        if(product is null)
        {
            return NotFound(id);
        }

        product.Name = request.Name;
        product.Quantity = request.Quantity;

        await _repository.UpdateAsync(product, cancellationToken);

        return NoContent();
    }
}
