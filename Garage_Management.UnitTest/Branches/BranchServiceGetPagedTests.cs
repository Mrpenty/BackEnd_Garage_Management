using Garage_Management.Application.Interfaces.Repositories.Branches;
using Garage_Management.Application.Services.Branches;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Branches
{
    [TestClass]
    public class BranchServiceGetPagedTests
    {
        private Mock<IBranchRepository> _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IBranchRepository>();
            _repo.Setup(x => x.CountEmployeesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
            _repo.Setup(x => x.CountActiveJobCardsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        }

        /// <summary>
        /// UTCID01 - Normal: Admin thấy tất cả chi nhánh (scopedBranchId = null)
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_Admin_ReturnsAllBranches()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery();
            _repo.Setup(x => x.GetPagedAsync(query, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Branch>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData = new List<Branch>
                    {
                        new Branch { BranchId = 1, BranchCode = "HN-01", Name = "Hà Nội", Address = "123", IsActive = true },
                        new Branch { BranchId = 2, BranchCode = "SG-01", Name = "Sài Gòn", Address = "456", IsActive = true }
                    }
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(2, result.Total);
            _repo.Verify(x => x.GetPagedAsync(query, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID02 - Normal: Staff BranchId=1 chỉ thấy chi nhánh mình (scopedBranchId = 1)
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_Staff_ReturnsOnlyOwnBranch()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsStaff(branchId: 1));
            var query = new ParamQuery();
            _repo.Setup(x => x.GetPagedAsync(query, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Branch>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<Branch>
                    {
                        new Branch { BranchId = 1, BranchCode = "HN-01", Name = "Hà Nội", Address = "123", IsActive = true }
                    }
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.First().BranchId);
            _repo.Verify(x => x.GetPagedAsync(query, 1, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID03 - Normal: DB rỗng → total=0, pageData rỗng
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery();
            _repo.Setup(x => x.GetPagedAsync(query, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Branch>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 0,
                    PageData = new List<Branch>()
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }

        /// <summary>
        /// UTCID04 - Normal: Response có đính kèm EmployeeCount + ActiveJobCardCount cho từng branch
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_IncludesEmployeeAndJobCardCounts()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.CountEmployeesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(5);
            _repo.Setup(x => x.CountActiveJobCardsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(8);
            var query = new ParamQuery();
            _repo.Setup(x => x.GetPagedAsync(query, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Branch>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<Branch>
                    {
                        new Branch { BranchId = 1, BranchCode = "HN-01", Name = "Hà Nội", Address = "123", IsActive = true }
                    }
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(5, result.PageData.First().EmployeeCount);
            Assert.AreEqual(8, result.PageData.First().ActiveJobCardCount);
        }
    }
}
