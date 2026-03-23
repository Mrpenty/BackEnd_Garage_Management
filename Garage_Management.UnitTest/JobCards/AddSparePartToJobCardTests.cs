using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Repositories.Appointments;
using Garage_Management.Application.Repositories.Inventories;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Application.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;
//using JobCardSparepartApp = Garage_Management.Application.Services.JobCards.JobCardSparepartService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;
using Microsoft.AspNetCore.Http;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
namespace Garage_Management.UnitTest.JobCard
{
    [TestClass]
    public class AddSparePartToJobCardTests
    {
        //private Mock<IJobCardRepository> _jobCardRepo;
        //private Mock<IInventoryRepository> _inventoryRepo;
        //private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo;

        //private JobCardServiceApp _service;
        ////private JobCardSparepartApp _sparepartservice;
        //private Mock<IHttpContextAccessor> _httpContextAccessor;
        //[TestInitialize]
        //public void Setup()
        //{
        //    _jobCardRepo = new Mock<IJobCardRepository>();
        //    _inventoryRepo = new Mock<IInventoryRepository>();
        //    _jobCardSparePartRepo = new Mock<IJobCardSparePartRepository>();
        //    _httpContextAccessor = new Mock<IHttpContextAccessor>();
        //    _service = new JobCardServiceApp(
        //        _jobCardRepo.Object, null,
        //        _inventoryRepo.Object, null,
        //        _jobCardSparePartRepo.Object, null, null,
        //          _httpContextAccessor.Object
        //    );
        //}

        //[TestMethod]
        //public async Task AddSparePartAsync_ReturnFalse_WhenJobCardNotFound()
        //{
        //    _jobCardRepo
        //        .Setup(x => x.GetByIdAsync(999))
        //        .ReturnsAsync((JobCardEntity)null);

        //    var dto = new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 };

        //    var result = await _sparepartservice.AddSparePartAsync(999, dto, CancellationToken.None);

        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public async Task AddSparePartAsync_ReturnFalse_WhenInventoryNotFound()
        //{
        //    _jobCardRepo
        //        .Setup(x => x.GetByIdAsync(1))
        //        .ReturnsAsync(new JobCardEntity { JobCardId = 1 });

        //    _inventoryRepo
        //        .Setup(x => x.GetByIdAsync(999))
        //        .ReturnsAsync((Inventory)null);

        //    var dto = new AddSparePartToJobCardDto { SparePartId = 999, Quantity = 1 };

        //    var result = await _sparepartservice.AddSparePartAsync(1, dto, CancellationToken.None);

        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public async Task AddSparePartAsync_ReturnFalse_WhenQuantityNonPositive()
        //{
        //    _jobCardRepo
        //        .Setup(x => x.GetByIdAsync(1))
        //        .ReturnsAsync(new JobCardEntity { JobCardId = 1 });

        //    _inventoryRepo
        //        .Setup(x => x.GetByIdAsync(5))
        //        .ReturnsAsync(new Inventory { SparePartId = 5, SellingPrice = 10m });

        //    var dto = new AddSparePartToJobCardDto { SparePartId = 5, Quantity = 0 };

        //    var result = await _sparepartservice.AddSparePartAsync(1, dto, CancellationToken.None);

        //    Assert.IsFalse(result);
        //}

        //[TestMethod]
        //public async Task AddSparePartAsync_ReturnTrue_WhenValid()
        //{
        //    var jobCard = new JobCardEntity { JobCardId = 7 };

        //    var inventory = new Inventory
        //    {
        //        SparePartId = 7,
        //        SellingPrice = 20m
        //    };

        //    _jobCardRepo
        //        .Setup(x => x.GetByIdAsync(7))
        //        .ReturnsAsync(jobCard);

        //    _inventoryRepo
        //        .Setup(x => x.GetByIdAsync(7))
        //        .ReturnsAsync(inventory);

        //    _jobCardSparePartRepo
        //        .Setup(x => x.AddAsync(It.IsAny<JobCardSparePart>(), It.IsAny<CancellationToken>()))
        //        .Returns(Task.CompletedTask);

        //    _jobCardSparePartRepo
        //        .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
        //        .Returns(Task.CompletedTask);

        //    var dto = new AddSparePartToJobCardDto
        //    {
        //        SparePartId = 7,
        //        Quantity = 2,
        //        IsUnderWarranty = true,
        //        Note = "ok"
        //    };

        //    var result = await _sparepartservice.AddSparePartAsync(7, dto, CancellationToken.None);

        //    Assert.IsTrue(result);

        //    _jobCardSparePartRepo.Verify(
        //        x => x.AddAsync(It.Is<JobCardSparePart>(j =>
        //            j.JobCardId == 7 &&
        //            j.SparePartId == 7 &&
        //            j.TotalAmount == 40m),
        //        It.IsAny<CancellationToken>()),
        //        Times.Once);
        //}
    }
}