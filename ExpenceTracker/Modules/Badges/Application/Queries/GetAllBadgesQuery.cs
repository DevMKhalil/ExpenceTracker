using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Application.Queries
{
    public record GetAllBadgesQuery() : IRequest<List<Badge>>;

    public class GetAllBadgesQueryHandler : IRequestHandler<GetAllBadgesQuery, List<Badge>>
    {
        private readonly IBadgeRepository _repository;

        public GetAllBadgesQueryHandler(IBadgeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Badge>> Handle(GetAllBadgesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllActiveAsync();
        }
    }
}
