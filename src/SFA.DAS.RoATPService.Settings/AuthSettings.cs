﻿
using Newtonsoft.Json;

namespace SFA.DAS.RoATPService.Settings
{
    public class AuthSettings : IAuthSettings
    {
        [JsonRequired] public string WtRealm { get; set; }

        [JsonRequired] public string MetadataAddress { get; set; }

        [JsonRequired] public string Role { get; set; }
    }
}
