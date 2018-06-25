using System.Threading.Tasks;
using AutoMapper;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveManagementServiceTests
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
            new StoreHiveSection() { Id = 1, Code = "sect1" },
            new StoreHiveSection() { Id = 2, Code = "sect2" },
            new StoreHiveSection() { Id = 3, Code = "sect3" },
            new StoreHiveSection() { Id = 4, Code = "sect4" },
            new StoreHiveSection() { Id = 5, Code = "sect5" }
        };

        public HiveManagementServiceTests()
        {
            MapperInitializer.Initialize();
        }

        [Fact]
        public async Task GetHivesAsync_SetWithFiveElements_ReturnedFiveElementsList()
        {
            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);
            storeContext.Setup(c => c.Sections).ReturnsEntitySet(_sections);

            var service = new HiveService(storeContext.Object, userContext.Object);
            var hives = await service.GetHivesAsync();

            Assert.Equal(_hives.Length, hives.Count);
            Assert.Equal(_hives[0].Code, hives[0].Code);
            Assert.Equal(_hives[2].Code, hives[2].Code);
            Assert.Equal(_hives[4].Code, hives[4].Code);
        }

        [Fact]
        public async Task GetHiveAsync_SetWithFiveElements_IdAsParametr_ReturnedOneElement()
        {
            var hiveId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);
            var hive = await service.GetHiveAsync(hiveId);

            Assert.Equal(hiveId, hive.Id);
        }

        [Fact]
        public async Task GetHiveAsync_SetWithFiveElements_NotExistedIdAsParametr_ThrowException()
        {
            var hiveId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveAsync(hiveId));
        }

        [Fact]
        public async Task CreateHiveAsync_CreateRequestAsParametr_ReturnedCreatedElement()
        {
            var createReq = new UpdateHiveRequest()
            {
                Name = "CreateName",
                Code = "TEST1",
                Address = "string"
            };

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(new StoreHive[0]);

            var service = new HiveService(storeContext.Object, userContext.Object);
            var hive = await service.CreateHiveAsync(createReq);

            Assert.Equal(createReq.Name, hive.Name);
            Assert.Equal(createReq.Code, hive.Code);
            Assert.Equal(createReq.Address, hive.Address);
        }

        [Fact]
        public async Task CreateHiveAsync_CreateRequestAsParametr_ExistedCode_ThrowException()
        {
            var createReq = new UpdateHiveRequest()
            {
                Name = "CreateName",
                Code = _hives[0].Code,
                Address = "string"
            };

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.CreateHiveAsync(createReq));
        }

        [Fact]
        public async Task UpdateHiveAsync_UpdateRequestAsParametr_ReturnedUpdatedElement()
        {
            var updateReq = new UpdateHiveRequest()
            {
                Name = "UpdateName",
                Code = "TEST1",
                Address = "string"
            };

            var hiveId = 1;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);
            var hive = await service.UpdateHiveAsync(hiveId, updateReq);

            Assert.Equal(updateReq.Name, hive.Name);
            Assert.Equal(updateReq.Code, hive.Code);
            Assert.Equal(updateReq.Address, hive.Address);
        }

        [Fact]
        public async Task UpdateHiveAsync_UpdateRequestAsParametr_ExistedCode_ThrowException()
        {
            var updateReq = new UpdateHiveRequest()
            {
                Name = "UpdateName",
                Code = _hives[0].Code,
                Address = "string"
            };

            var hiveId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.UpdateHiveAsync(hiveId, updateReq));
        }

        [Fact]
        public async Task UpdateHiveAsync_UpdateRequestAsParametr_NotExistedHiveId_ThrowException()
        {
            var updateReq = new UpdateHiveRequest()
            {
                Name = "UpdateName",
                Code = "TEST2",
                Address = "string"
            };

            var hiveId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.UpdateHiveAsync(hiveId, updateReq));
        }

        [Fact]
        public async Task DeleteHiveAsync_IdAsParametr_StatusIsDeletedFalse_ThrownException()
        {
            var hiveId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(() => service.DeleteHiveAsync(hiveId));
        }

        [Fact]
        public async Task DeleteHiveAsync_IdAsParametr_NotExistedHiveId_ThrownException()
        {
            var hiveId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.DeleteHiveAsync(hiveId));
        }

        [Fact]
        public async Task DeleteHiveAsync_IdAsParametr_StatusIsDeletedTrue()
        {
            var list = new StoreHive[1]
            {
                new StoreHive { Id = 3, Code = "HIVE3", IsDeleted = true }
            };

            var hiveId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(list);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await service.DeleteHiveAsync(hiveId);
        }

        [Fact]
        public async Task SetStatusAsync_IdAndBooleanAsParameters()
        {
            var hiveId = 3;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await service.SetStatusAsync(hiveId, true);
            await service.SetStatusAsync(hiveId, false);
        }

        [Fact]
        public async Task SetStatusAsync_IdAndBooleanAsParameters_NotExistedHiveId_ThrownException()
        {
            var hiveId = 10;

            var storeContext = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            storeContext.Setup(c => c.Hives).ReturnsAsyncEntitySet(_hives);

            var service = new HiveService(storeContext.Object, userContext.Object);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.SetStatusAsync(hiveId, true));
        }
    }
}
