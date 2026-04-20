namespace Garage_Management.Application.DTOs.Branches
{
    public class BranchResponse
    {
        public int BranchId { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? ManagerEmployeeId { get; set; }
        public string? ManagerEmployeeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EmployeeCount { get; set; }
        public int ActiveJobCardCount { get; set; }
    }
}
