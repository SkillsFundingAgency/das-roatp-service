using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;
using SFA.DAS.RoATPService.Domain.Models.ProvideFeedback;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;

namespace SFA.DAS.RoATPService.Application.Services
{
    public class FatDataExportService : IFatDataExportService
    {
        private readonly IFatDataExportRepository _repository;
        private readonly IProvideFeedbackService _service;

        public FatDataExportService (IFatDataExportRepository repository, IProvideFeedbackService service)
        {
            _repository = repository;
            _service = service;
        }
        public async Task<IEnumerable<FatDataExport>> GetData()
        {
            var data = _repository.GetFatDataExport();
            var serviceData = _service.GetProviderFeedBack();

            await Task.WhenAll(data, serviceData);
            
            var fatDataExports = data.Result.Select(c=>(FatDataExport)c).ToList();

            foreach (var fatDataExport in fatDataExports)
            {
                var filteredFeedBack = serviceData
                    .Result
                    .Where(c => c.Ukprn == fatDataExport.UkPrn)
                    .ToList();
                if (filteredFeedBack.Any())
                {
                    var fatProviderFeedbackData = new FatProviderFeedbackData
                    {
                        Total = filteredFeedBack.Count,
                        FeedbackRating = GroupFeedbackAndCount(filteredFeedBack),
                        ProviderAttributes = GroupProviderAttributesAndCountStrengthsAndWeaknesses(filteredFeedBack)
                    };
                    fatDataExport.Feedback = fatProviderFeedbackData;
                }
                else
                {
                    fatDataExport.Feedback = new FatProviderFeedbackData();    
                }
            }
            
            return fatDataExports;
        }

        private static List<KeyValuePair<string, int>> GroupFeedbackAndCount(IEnumerable<EmployerFeedbackSourceDto> filteredFeedBack)
        {
            return filteredFeedBack
                .GroupBy(c=>c.ProviderRating)
                .ToDictionary(c=>c.Key, val=>val.Count()).ToList();
        }

        private static List<ProviderAttribute> GroupProviderAttributesAndCountStrengthsAndWeaknesses(IEnumerable<EmployerFeedbackSourceDto> filteredFeedBack)
        {
            return filteredFeedBack
                .SelectMany(c => c.ProviderAttributes)
                .GroupBy(g => new {g.Name})
                .Select(s=>new ProviderAttribute(
                    s.Key.Name, 
                    s.Count(c=>c.Value == 1),
                    s.Count(c=>c.Value == -1)
                ))
                .ToList();
        }
    }
}