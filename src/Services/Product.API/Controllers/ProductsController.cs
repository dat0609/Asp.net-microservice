using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interfaces;
using Shared.DTOs.Product;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    
    public ProductsController(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    #region CRUD
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _repository.GetProductsAsync();
        var result = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(result);
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetProduct([Required] long id)
    {
        var product = await _repository.GetProductAsync(id);
        if (product == null) return NotFound();
        
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }
    
    [HttpPost]
    // [Authorize]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        var productEntity = await _repository.GetProductByNoAsync(productDto.No);
        if (productEntity != null) return BadRequest($"Product No: {productDto.No} is existed.");
        
        var product = _mapper.Map<CatalogProduct>(productDto);
        await _repository.CreateProductAsync(product);
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }
    
    [HttpPut("{id:long}")]
    // [Authorize]
    public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
    {
        var product = await _repository.GetProductAsync(id);
        if (product == null) return NotFound();

        var updateProduct = _mapper.Map(productDto, product);
        await _repository.UpdateProductAsync(updateProduct);
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }
    
    [HttpDelete("{id:long}")]
    // [Authorize]
    public async Task<IActionResult> DeleteProduct([Required] long id)
    {
        var product = await _repository.GetProductAsync(id);
        if (product == null) return NotFound();

        await _repository.DeleteProductAsync(id);
        return NoContent();
    }
        
    #endregion

    #region Additional Resources

    [HttpGet("get-product-by-no/{productNo}")]
    public async Task<IActionResult> GetProductByNo([Required] string productNo)
    {
        var product = await _repository.GetProductByNoAsync(productNo);
        if (product == null) return NotFound();
        
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    #endregion
    
}