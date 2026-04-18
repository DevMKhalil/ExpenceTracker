using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ExpenceTracker.Modules.Badges.Domain;

namespace ExpenceTracker.Modules.Badges.Application.Commands
{
    public record UpdateBadgeCommand(Guid Id, string Name, string Color) : IRequest<bool>;

    public class UpdateBadgeCommandHandler : IRequestHandler<UpdateBadgeCommand, bool>
    {
        private readonly IBadgeRepository _repository;

        public UpdateBadgeCommandHandler(IBadgeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateBadgeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name cannot be empty", nameof(request.Name));
            if (request.Name.Length > 100)
                throw new ArgumentException("Name cannot exceed 100 characters", nameof(request.Name));
            if (!Regex.IsMatch(request.Color, "^#[0-9A-Fa-f]{6}$"))
                throw new ArgumentException("Color must be a valid hex color code", nameof(request.Color));

            var badge = await _repository.GetByIdAsync(request.Id);
            if (badge == null)
                return false;

            if (await _repository.ExistsByNameAsync(request.Name, request.Id))
                throw new InvalidOperationException("A badge with this name already exists");

            badge.Name = request.Name.Trim();
            badge.Color = request.Color;

            await _repository.UpdateAsync(badge);
            return true;
        }
    }
}
