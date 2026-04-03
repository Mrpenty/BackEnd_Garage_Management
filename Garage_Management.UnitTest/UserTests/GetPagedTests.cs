using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Accounts;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.UserTests
{
    [TestClass]
    public class GetPagedTests
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<IEmployeeRepository> _mockEmployeeRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        }

    }
}
