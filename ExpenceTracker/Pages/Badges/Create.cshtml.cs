using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using ExpenceTracker.Resources;
using ExpenceTracker.Modules.Badges.Application.Commands;

namespace ExpenceTracker.Pages.Badges
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CreateModel(IMediator mediator, IStringLocalizer<SharedResource> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Color { get; set; } = "#4CAF50";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _mediator.Send(new CreateBadgeCommand(Name, Color));
                return RedirectToPage("Index");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
