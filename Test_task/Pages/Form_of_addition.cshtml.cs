using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Test_task.Data.Repositories;
using Test_task.Models.Entities;

namespace Test_task.Pages
{
    public class Form_of_additionModel : PageModel
    {
        private readonly ChecksRepository _repository;
          
        public Form_of_additionModel(ChecksRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public CheckEntity NewCheck { get; set; } = new();

        [BindProperty]
        public SMPEntity SMP { get; set; } = new();

        [BindProperty]
        public Guid SupervisoryId { get; set; } = new();

        [BindProperty]
        public List<SupervisoryEntity> SupervisoryList { get; set; } = new();

        public async Task OnGetAsync()
        {
            SupervisoryList = await _repository.GetSupervisoryList();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {

           if(ModelState.IsValid)
            {
                var _SMP = await _repository.GetSMP(SMP.Name);
                if (_SMP == null)
                {
                    await _repository.AddSMP(Guid.NewGuid(), SMP.Name);
                    _SMP = await _repository.GetSMP(SMP.Name);
                }
                DateOnly dateFinish = NewCheck.DateStart.AddDays(NewCheck.PlannedDuration);
                await _repository.Add(
                    Guid.NewGuid(),
                    _SMP.Id, 
                    SupervisoryId, 
                    NewCheck.DateStart, 
                    dateFinish, 
                    NewCheck.PlannedDuration);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostImportFromExcelAsync(IFormFile file)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "ImportData", "checks.xlsx");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var (added, alreadyThere, errors) = await _repository.ImportChecksFromExcel(fileStream);

            Console.WriteLine($"Импорт завершен. Успешно: {added}\nУже существует: {alreadyThere}\nОшибок: {errors}");

            return RedirectToPage();
        }
    }
}
