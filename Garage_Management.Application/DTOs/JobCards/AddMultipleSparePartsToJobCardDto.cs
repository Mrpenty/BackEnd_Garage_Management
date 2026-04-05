namespace Garage_Management.Application.DTOs.JobCards
{
    public class AddMultipleSparePartsToJobCardDto
    {
        public List<AddSparePartToJobCardDto> SpareParts { get; set; } = new();
    }
}
