using System;
using System.Linq;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveSectionManagementServiceTests
    {
        private static StoreHive[] _hives = new StoreHive[]
{
            new StoreHive() { Id = 1, Code = "hive1" },
            new StoreHive() { Id = 2, Code = "hive2" },
            new StoreHive() { Id = 3, Code = "hive3" },
            new StoreHive() { Id = 4, Code = "hive4" },
            new StoreHive() { Id = 5, Code = "hive5" }
};

        private static StoreHiveSection[] _sections = new StoreHiveSection[]
        {
            new StoreHiveSection() { Id = 1, Code = "sect1", StoreHiveId = 1 },
            new StoreHiveSection() { Id = 2, Code = "sect2", StoreHiveId = 1 },
            new StoreHiveSection() { Id = 3, Code = "sect3", StoreHiveId = 2 },
            new StoreHiveSection() { Id = 4, Code = "sect4", StoreHiveId = 2 },
            new StoreHiveSection() { Id = 5, Code = "sect5", StoreHiveId = 2 }
        };

        public HiveSectionManagementServiceTests()
        {
            MapperInitializer.Initialize();
        }

        [Fact]
        public void Ctor_ContextIsNull_ExceptionThrown()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new HiveSectionService(null, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public async Task GetHiveSectionsAsync_SetWithFiveElements_ReturnedFiveElementsList()
        {
            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);
            var sections = await service.GetHiveSectionsAsync();

            Assert.Equal(_sections.Length, sections.Count);
            Assert.Equal(_sections[0].Code, sections[0].Code);
            Assert.Equal(_sections[2].Code, sections[2].Code);
            Assert.Equal(_sections[4].Code, sections[4].Code);
        }

        [Fact]
        public async Task GetHiveSectionsAsync_SetWithFiveElements_HiveIdAsParametr_ReturnedElementsList()
        {
            var hiveId = 2;
            var expectedCount = _sections.Where(s => s.StoreHiveId == hiveId).ToList().Count;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);
            var sections = await service.GetHiveSectionsAsync(hiveId);

            Assert.Equal(expectedCount, sections.Count);
        }

        [Fact]
        public async Task GetHiveSectionAsync_SetWithFiveElements_IdAsParametr_ReturnedOneElement()
        {
            var hiveSectionId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);
            var hiveSection = await service.GetHiveSectionAsync(hiveSectionId);

            Assert.Equal(hiveSectionId, hiveSection.Id);
        }

        [Fact]
        public async Task GetHiveSectionAsync_SetWithFiveElements_NotExistedIdAsParametr_ThrowException()
        {
            var hiveSectionId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveSectionAsync(hiveSectionId));
        }

        [Fact]
        public async Task CreateHiveSectionAsync_CreateRequestAsParametr_ReturnedCreatedElement()
        {
            var createReq = new UpdateHiveSectionRequest()
            {
                Name = "CreateName",
                Code = "TEST1",
                HiveId = 2
            };

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(new StoreHiveSection[0]);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);
            var hiveSection = await service.CreateHiveSectionAsync(createReq);

            Assert.Equal(createReq.Name, hiveSection.Name);
            Assert.Equal(createReq.Code, hiveSection.Code);
            Assert.Equal(createReq.HiveId, hiveSection.HiveId);
        }

        [Fact]
        public async Task CreateHiveSectionAsync_CreateRequestAsParametr_ExistedCode_ThrowException()
        {
            var createReq = new UpdateHiveSectionRequest()
            {
                Name = "CreateName",
                Code = _sections[0].Code,
                HiveId = 2
            };

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateHiveSectionAsync(createReq));
        }

        [Fact]
        public async Task CreateHiveSectionAsync_CreateRequestAsParametr_NotExistedHiveId_ThrowException()
        {
            var createReq = new UpdateHiveSectionRequest()
            {
                Name = "CreateName",
                Code = "TEST7",
                HiveId = 10
            };

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.CreateHiveSectionAsync(createReq));
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_UpdateRequestAsParametr_ReturnedUpdatedElement()
        {
            var updateReq = new UpdateHiveSectionRequest()
            {
                Name = "UpdateName",
                Code = "TEST1",
                HiveId = 2
            };

            var hiveSectionId = 1;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);
            var hiveSection = await service.UpdateHiveSectionAsync(hiveSectionId, updateReq);

            Assert.Equal(updateReq.Name, hiveSection.Name);
            Assert.Equal(updateReq.Code, hiveSection.Code);
            Assert.Equal(updateReq.HiveId, hiveSection.HiveId);
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_UpdateRequestAsParametr_ExistedCode_ThrowException()
        {
            var updateReq = new UpdateHiveSectionRequest()
            {
                Name = "UpdateName",
                Code = _sections[0].Code,
                HiveId = 2
            };

            var hiveSectionId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateHiveSectionAsync(hiveSectionId, updateReq));
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_UpdateRequestAsParametr_NotExistedHiveId_ThrowException()
        {
            var updateReq = new UpdateHiveSectionRequest()
            {
                Name = "UpdateName",
                Code = "TEST2",
                HiveId = 2
            };

            var hiveId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateHiveSectionAsync(hiveId, updateReq));
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_IdAsParametr_StatusIsDeletedFalse_ThrownException()
        {
            var hiveSectionId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteHiveSectionAsync(hiveSectionId));
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_IdAsParametr_NotExistedHiveId_ThrownException()
        {
            var hiveSectionId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteHiveSectionAsync(hiveSectionId));
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_IdAsParametr_StatusIsDeletedTrue()
        {
            var list = new StoreHiveSection[1]
            {
                new StoreHiveSection { Id = 3, Code = "SECT3", IsDeleted = true }
            };

            var hiveSectionId = list[0].Id;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(list);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await service.DeleteHiveSectionAsync(hiveSectionId);
        }

        [Fact]
        public async Task SetStatusAsync_IdAndBooleanAsParameters()
        {
            var hiveSectionId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await service.SetStatusAsync(hiveSectionId, true);
            await service.SetStatusAsync(hiveSectionId, false);
        }

        [Fact]
        public async Task SetStatusAsync_IdAndBooleanAsParameters_NotExistedHiveId_ThrownException()
        {
            var hiveSectionId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Sections).ReturnsAsyncEntitySet(_sections);

            var service = new HiveSectionService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(hiveSectionId, true));
        }
    }
}
