using Microsoft.Extensions.FileProviders;
using System;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Services
{
    public class TestHostingEnvironment : Microsoft.AspNetCore.Hosting.IHostingEnvironment
    {
        public string EnvironmentName { get => Microsoft.AspNetCore.Hosting.EnvironmentName.Development; set => throw new NotImplementedException(); }
        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
