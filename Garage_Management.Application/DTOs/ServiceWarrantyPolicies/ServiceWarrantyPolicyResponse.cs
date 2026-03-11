namespace Garage_Management.Application.DTOs.ServiceWarrantyPolicies
{
    public class ServiceWarrantyPolicyResponse
    {
        public int PolicyId { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public int? DurationMonths { get; set; }
        public int? MileageLimit { get; set; }
        public string? TermsAndConditions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
