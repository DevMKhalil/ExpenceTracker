using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Application.Queries
{
    public record GetAllBadgesIncludingDeletedQuery() : IRequest<List<Badge>>;

    public class GetAllBadgesIncludingDeletedQueryHandler : IRequestHandler<GetAllBadgesIncludingDeletedQuery, List<Badge>>
    {
        private readonly IBadgeRepository _repository;

        public GetAllBadgesIncludingDeletedQueryHandler(IBadgeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Badge>> Handle(GetAllBadgesIncludingDeletedQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllIncludingDeletedAsync();
        }
    }
}
