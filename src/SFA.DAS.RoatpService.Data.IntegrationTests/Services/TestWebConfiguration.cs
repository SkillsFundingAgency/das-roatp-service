﻿using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Services
{
    public class TestWebConfiguration : IWebConfiguration
    {
        public AuthSettings Authentication
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ApiAuthentication ApiAuthentication
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        //public AzureApiAuthentication AzureApiAuthentication
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public ClientApiAuthentication ClientApiAuthentication
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public NotificationsApiClientConfiguration NotificationsApiClientConfiguration
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public CertificateDetails CertificateDetails
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public SftpSettings Sftp
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public string AssessmentOrgsApiClientBaseUrl
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public string IfaApiClientBaseUrl
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public string IFATemplateStorageConnectionString
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        public string SqlConnectionString { get; set; }
       // public string SpecflowDBTestConnectionString { get; set; }

        public string SessionRedisConnectionString
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        //public AuthSettings StaffAuthentication
        //{
        //    get => throw new NotImplementedException();
        //    set => throw new NotImplementedException();
        //}

        //public ClientApiAuthentication ApplyApiAuthentication { get; set; }
        //public string RoatpApiClientBaseUrl { get; set; }
        // public ClientApiAuthentication RoatpApiAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
