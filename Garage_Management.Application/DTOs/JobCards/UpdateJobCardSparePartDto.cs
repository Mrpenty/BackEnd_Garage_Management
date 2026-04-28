namespace Garage_Management.Application.DTOs.JobCards
{
    public class UpdateJobCardSparePartDto
    {
        public int? Quantity { get; set; }
        public bool? IsUnderWarranty { get; set; }
        public string? Note { get; set; }
    }
}
