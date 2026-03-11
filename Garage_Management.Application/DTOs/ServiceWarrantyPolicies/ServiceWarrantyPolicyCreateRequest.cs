namespace Garage_Management.Application.DTOs.ServiceWarrantyPolicies
{
    public class ServiceWarrantyPolicyCreateRequest
    {
        public string PolicyName { get; set; } = string.Empty;
        public int? DurationMonths { get; set; }
        public int? MileageLimit { get; set; }
        public string? TermsAndConditions { get; set; }
    }
}
