using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using ExpenceTracker.Resources;
using ExpenceTracker.Modules.Badges.Application.Queries;
using ExpenceTracker.Modules.Badges.Application.Commands;

namespace ExpenceTracker.Pages.Badges
{
    public class EditModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public EditModel(IMediator mediator, IStringLocalizer<SharedResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [BindProperty]
        public Guid Id { get; set; }

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Color { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var badge = await _mediator.Send(new GetBadgeByIdQuery(id));
            if (badge == null)
            {
                return NotFound();
            }

            Id = badge.Id;
            Name = badge.Name;
            Color = badge.Color;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _mediator.Send(new UpdateBadgeCommand(Id, Name, Color));
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
