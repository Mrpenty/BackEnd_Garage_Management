using Garage_Management.Application.Interfaces.Services.Auth;
using Moq;

namespace Garage_Management.UnitTest.Helper
{
    /// <summary>
    /// Mock tiện ích cho ICurrentUserService trong unit tests.
    /// </summary>
    public static class MockCurrentUser
    {
        /// <summary>
        /// Non-admin thuộc chi nhánh (default BranchId=1, EmployeeId=1, UserId=1).
        /// </summary>
        public static ICurrentUserService AsStaff(int branchId = 1, int employeeId = 1, int userId = 1)
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.IsAdmin()).Returns(false);
            mock.Setup(x => x.GetCurrentBranchId()).Returns(branchId);
            mock.Setup(x => x.GetCurrentEmployeeId()).Returns(employeeId);
            mock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            mock.Setup(x => x.GetCurrentCustomerId()).Returns((int?)null);
            mock.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            return mock.Object;
        }

        /// <summary>
        /// Admin (không bị scope theo branch).
        /// </summary>
        public static ICurrentUserService AsAdmin(int userId = 1)
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.IsAdmin()).Returns(true);
            mock.Setup(x => x.GetCurrentBranchId()).Returns((int?)null);
            mock.Setup(x => x.GetCurrentUserId()).Returns(userId);
            mock.Setup(x => x.GetCurrentEmployeeId()).Returns((int?)null);
            mock.Setup(x => x.GetCurrentCustomerId()).Returns((int?)null);
            mock.Setup(x => x.IsInRole(It.IsAny<string>())).Returns<string>(r => r == "Admin");
            return mock.Object;
        }
    }
}
