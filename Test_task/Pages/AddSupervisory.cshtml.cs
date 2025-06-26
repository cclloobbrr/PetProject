using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test_task.Data.Repositories;
using Test_task.Models.Entities;

namespace Test_task.Pages
{
    public class AddSupervisoryModel : PageModel
    {
        private readonly ChecksRepository _repository;

        public AddSupervisoryModel(ChecksRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public SupervisoryEntity Supervisory { get; set; } = new();

        public async Task<IActionResult> OnPostAddSupervisoryAsync()
        {
            await _repository.AddSupervisory(Supervisory.Name);
            return RedirectToPage();
        }
    }
}
