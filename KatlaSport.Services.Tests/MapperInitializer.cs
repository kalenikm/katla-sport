using AutoMapper;
using KatlaSport.Services.HiveManagement;

namespace KatlaSport.Services.Tests
{
    public static class MapperInitializer
    {
        private static bool _isInitialized;

        public static void Initialize()
        {
            if (!_isInitialized)
            {
                Mapper.Reset();
                Mapper.Initialize(m => m.AddProfile(new HiveManagementMappingProfile()));
                _isInitialized = true;
            }
        }
    }
}
