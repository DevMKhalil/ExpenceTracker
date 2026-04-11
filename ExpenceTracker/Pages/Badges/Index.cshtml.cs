using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using ExpenceTracker.Resources;
using ExpenceTracker.Modules.Badges.Domain;
using ExpenceTracker.Modules.Badges.Application.Queries;
using ExpenceTracker.Modules.Badges.Application.Commands;

namespace ExpenceTracker.Pages.Badges
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(IMediator mediator, IStringLocalizer<SharedResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        public List<Badge> Badges { get; set; } = new List<Badge>();

        public async Task OnGetAsync()
        {
            Badges = await _mediator.Send(new GetAllBadgesQuery());
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteBadgeCommand(id));
            return RedirectToPage();
        }
    }
}
