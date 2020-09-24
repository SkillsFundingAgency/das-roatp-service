using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Models.ProvideFeedback;

namespace SFA.DAS.RoATPService.Infrastructure.Interfaces
{
    public interface IProvideFeedbackService
    {
        Task<IEnumerable<EmployerFeedbackSourceDto>> GetProviderFeedBack();
    }
}