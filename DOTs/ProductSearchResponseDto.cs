namespace EasyShop.API.DTOs;

public class ProductSearchResponseDto
{
    public IEnumerable<ProductDto> Items { get; set; }
        = Enumerable.Empty<ProductDto>();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}