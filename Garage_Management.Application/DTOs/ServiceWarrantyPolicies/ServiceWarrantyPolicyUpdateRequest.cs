namespace Garage_Management.Application.DTOs.ServiceWarrantyPolicies
{
    public class ServiceWarrantyPolicyUpdateRequest
    {
        public string? PolicyName { get; set; }
        public int? DurationMonths { get; set; }
        public int? MileageLimit { get; set; }
        public string? TermsAndConditions { get; set; }
    }
}
