namespace Garage_Management.Application.DTOs.Inventories.SparePartBrands
{
    public class SparePartBrandResponse
    {
        public int SparePartBrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
