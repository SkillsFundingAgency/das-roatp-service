﻿using System;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Models
{
    public class OrganisationStatusModel:TestModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}