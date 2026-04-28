using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class GetJobCardTests
    {
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IServiceRepository> _serviceRepo = null!;
        private Mock<IInventoryRepository> _inventoryRepo = null!;
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo = null!;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo = null!;
        private Mock<IWorkBayRepository> _workBayRepo = null!;
        private Mock<IAppointmentRepository> _appointmentRepo = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
        private Mock<ProgressCalculator> _progressCalculator = null!;
        private JobCardServiceApp _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _jobCardRepo = new Mock<IJobCardRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _jobCardServiceRepo = new Mock<IJobCardServiceRepository>();
            _jobCardSparePartRepo = new Mock<IJobCardSparePartRepository>();
            _workBayRepo = new Mock<IWorkBayRepository>();
            _appointmentRepo = new Mock<IAppointmentRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _progressCalculator = new Mock<ProgressCalculator>();

            _service = new JobCardServiceApp(
                _jobCardRepo.Object,
                _serviceRepo.Object,
                _inventoryRepo.Object,
                _jobCardServiceRepo.Object,
                _jobCardSparePartRepo.Object,
                _workBayRepo.Object,
                _appointmentRepo.Object,
                _httpContextAccessor.Object,
                _progressCalculator.Object);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNull_WhenJobCardNotFound()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((JobCard?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.IsNull(result);
            _jobCardRepo.Verify(x => x.GetByIdAsync(999), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_MapsAllFields_AndFiltersDeletedServices()
        {
            var now = new DateTime(2026, 4, 26, 8, 30, 0);
            var endDate = now.AddHours(4);
            var assignedAt = now.AddMinutes(15);
            var startedAt = now.AddMinutes(30);
            var completedAt = now.AddHours(3);
            var sparePartCreatedAt = now.AddHours(1);

            var entity = new JobCard
            {
                JobCardId = 1,
                AppointmentId = 11,
                CustomerId = 2,
                Customer = new Customer
                {
                    CustomerId = 2,
                    FirstName = "An",
                    LastName = "Nguyen",
                    User = new User
                    {
                        PhoneNumber = "0909000111"
                    }
                },
                VehicleId = 3,
                Vehicle = new Vehicle
                {
                    VehicleId = 3,
                    LicensePlate = "59A-12345",
                    Year = 2022,
                    Brand = new VehicleBrand
                    {
                        BrandId = 5,
                        BrandName = "Honda"
                    },
                    Model = new VehicleModel
                    {
                        ModelId = 6,
                        ModelName = "City"
                    }
                },
                WorkBay = new WorkBay
                {
                    Id = 7,
                    Name = "Bay 7"
                },
                QueueOrder = 2.5m,
                StartDate = now,
                EndDate = endDate,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 60,
                CompletedSteps = "Inspect, Diagnose",
                ProgressNotes = "Waiting for spare part",
                Note = "Customer requests quick turnaround",
                SupervisorId = 8,
                Supervisor = new Employee
                {
                    EmployeeId = 8,
                    FirstName = "Binh",
                    LastName = "Tran"
                },
                CreatedBy = 99,
                Services = new List<JobCardServiceEntity>
                {
                    new JobCardServiceEntity
                    {
                        JobCardServiceId = 100,
                        JobCardId = 1,
                        ServiceId = 501,
                        Description = "Oil service",
                        Price = 450000m,
                        Status = ServiceStatus.InProgress,
                        Service = new Service
                        {
                            ServiceId = 501,
                            ServiceName = "Oil Change"
                        },
                        ServiceTasks = new List<JobCardServiceTask>
                        {
                            new JobCardServiceTask
                            {
                                JobCardServiceTaskId = 9002,
                                Status = ServiceStatus.Completed,
                                ServiceTask = new ServiceTask
                                {
                                    TaskName = "Drain old oil"
                                }
                            },
                            new JobCardServiceTask
                            {
                                JobCardServiceTaskId = 9001,
                                Status = ServiceStatus.InProgress,
                                ServiceTask = new ServiceTask
                                {
                                    TaskName = "Refill new oil"
                                }
                            }
                        }
                    },
                    new JobCardServiceEntity
                    {
                        JobCardServiceId = 101,
                        JobCardId = 1,
                        ServiceId = 502,
                        DeletedAt = now,
                        Service = new Service
                        {
                            ServiceId = 502,
                            ServiceName = "Should Be Filtered"
                        }
                    }
                },
                Mechanics = new List<JobCardMechanic>
                {
                    new JobCardMechanic
                    {
                        JobCardId = 1,
                        EmployeeId = 21,
                        Employee = new Employee
                        {
                            EmployeeId = 21,
                            FirstName = "Minh",
                            LastName = "Le"
                        },
                        AssignedAt = assignedAt,
                        StartedAt = startedAt,
                        CompletedAt = completedAt
                    }
                },
                SpareParts = new List<JobCardSparePart>
                {
                    new JobCardSparePart
                    {
                        JobCardId = 1,
                        SparePartId = 301,
                        Inventory = new Inventory
                        {
                            SparePartId = 301,
                            PartName = "Engine Oil"
                        },
                        Quantity = 2,
                        UnitPrice = 150000m,
                        TotalAmount = 300000m,
                        IsUnderWarranty = false,
                        Note = "Use premium oil",
                        CreatedAt = sparePartCreatedAt
                    }
                }
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.JobCardId);
            Assert.AreEqual(11, result.AppointmentId);
            Assert.AreEqual(2, result.CustomerId);
            Assert.AreEqual("Nguyen An", result.CustomerName);
            Assert.AreEqual("0909000111", result.CustomerPhone);
            Assert.AreEqual(3, result.VehicleId);
            Assert.AreEqual(7, result.WorkbayId);
            Assert.AreEqual(2.5m, result.QueueOrder);
            Assert.AreEqual(now, result.StartDate);
            Assert.AreEqual(endDate, result.EndDate);
            Assert.AreEqual(JobCardStatus.InProgress, result.Status);
            Assert.AreEqual(60, result.ProgressPercentage);
            Assert.AreEqual("Inspect, Diagnose", result.CompletedSteps);
            Assert.AreEqual("Waiting for spare part", result.ProgressNotes);
            Assert.AreEqual("Customer requests quick turnaround", result.Note);
            Assert.AreEqual(8, result.SupervisorId);
            Assert.AreEqual("Tran Binh", result.SupervisorName);
            Assert.AreEqual(99, result.CreatedByEmployeeId);

            Assert.AreEqual(1, result.Vehicles.Count);
            Assert.AreEqual("59A-12345", result.Vehicles[0].LicensePlate);
            Assert.AreEqual("Honda", result.Vehicles[0].Brand);
            Assert.AreEqual("City", result.Vehicles[0].Model);
            Assert.AreEqual(2022, result.Vehicles[0].Year);

            Assert.AreEqual(1, result.Services.Count);
            Assert.AreEqual(100, result.Services[0].JobCardServiceId);
            Assert.AreEqual("Oil Change", result.Services[0].ServiceName);
            Assert.AreEqual(2, result.Services[0].ServiceTasks.Count);
            Assert.AreEqual(9001, result.Services[0].ServiceTasks[0].JobCardServiceTaskId);
            Assert.AreEqual(9002, result.Services[0].ServiceTasks[1].JobCardServiceTaskId);

            Assert.IsNotNull(result.Mechanics);
            Assert.AreEqual(1, result.Mechanics.Count);
            Assert.AreEqual(21, result.Mechanics[0].MechanicId);
            Assert.AreEqual("Le Minh", result.Mechanics[0].MechanicName);
            Assert.AreEqual(assignedAt, result.Mechanics[0].AssignedAt);
            Assert.AreEqual(startedAt, result.Mechanics[0].StartedAt);
            Assert.AreEqual(completedAt, result.Mechanics[0].CompletedAt);

            Assert.AreEqual(1, result.SpareParts.Count);
            Assert.AreEqual(301, result.SpareParts[0].SparePartId);
            Assert.AreEqual("Engine Oil", result.SpareParts[0].SparePartName);
            Assert.AreEqual(2, result.SpareParts[0].Quantity);
            Assert.AreEqual(150000m, result.SpareParts[0].UnitPrice);
            Assert.AreEqual(300000m, result.SpareParts[0].TotalAmount);
            Assert.IsFalse(result.SpareParts[0].IsUnderWarranty);
            Assert.AreEqual("Use premium oil", result.SpareParts[0].Note);
            Assert.AreEqual(sparePartCreatedAt, result.SpareParts[0].CreatedAt);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenOptionalNavigationsMissing_ReturnsSafeDefaults()
        {
            var entity = new JobCard
            {
                JobCardId = 2,
                CustomerId = 10,
                VehicleId = 20,
                StartDate = new DateTime(2026, 4, 26, 9, 0, 0),
                Status = JobCardStatus.Created,
                Services = new List<JobCardService>(),
                Mechanics = new List<JobCardMechanic>(),
                SpareParts = new List<JobCardSparePart>()
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(2);

            Assert.IsNotNull(result);
            Assert.IsNull(result.CustomerName);
            Assert.IsNull(result.CustomerPhone);
            Assert.IsNull(result.SupervisorName);
            Assert.IsNull(result.WorkbayId);
            Assert.AreEqual(0, result.Vehicles.Count);
            Assert.AreEqual(0, result.Services.Count);
            Assert.IsNotNull(result.Mechanics);
            Assert.AreEqual(0, result.Mechanics.Count);
            Assert.AreEqual(0, result.SpareParts.Count);
        }

        
    }
}
