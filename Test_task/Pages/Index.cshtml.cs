using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test_task.Data.Repositories;
using Test_task.Models.Entities;

namespace Test_task.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ChecksRepository _repository;

        public IndexModel(ChecksRepository repository)
        {
            _repository = repository;
        }

        [BindProperty(SupportsGet = true)]
        public string CurrentSearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateOnly? CurrentSearchDate { get; set; }

        [BindProperty]
        public List<CheckEntity> Checks { get; set; } = new();

        [BindProperty]
        public SMPEntity SMP { get; set; } = new();

        public async Task OnGet()
        {
            Checks = await _repository.GetAll();
        }

        public async Task OnGetSearchAsync(string name, DateOnly date)
        {
            if(!string.IsNullOrEmpty(name) && date != default)
            {
                Checks = await _repository.Search(name, date);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                Checks = await _repository.Search(name);
            }
            else if (date != default)
            {
                Checks = await _repository.Search(null, date);
            }
            else
            {
                Checks = await _repository.GetAll();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _repository.Delete(id);
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostExportToExcelAsync()
        {
            var checks = await _repository.GetAll();


            var excelBytes = await _repository.ExportChecksToExcel(checks);
            return File(excelBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Проверки_экспорт.xlsx");
        }
    }
}
