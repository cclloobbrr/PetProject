using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test_task.Data.Repositories;
using Test_task.Models.Entities;

namespace Test_task.Pages
{
    public class UpdateModel : PageModel
    {
        private readonly ChecksRepository _repository;

        public UpdateModel(ChecksRepository repository)
        {
            _repository = repository;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CheckEntity Check { get; set; }

        [BindProperty]
        public SMPEntity SMP { get; set; }

        [BindProperty]
        public SupervisoryEntity Supervisory { get; set; }

        [BindProperty]
        public List<SupervisoryEntity> SupervisoryList { get; set; } = new();

        public async Task OnGetAsync()
        {
            Check = await _repository.GetById(Id);
            SupervisoryList = await _repository.GetSupervisoryList();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var _SMP = await _repository.GetSMP(SMP.Name);

            if(_SMP == null)
            {
                await _repository.AddSMP(Guid.NewGuid(), SMP.Name);
                _SMP = await _repository.GetSMP(SMP.Name);
            }

            var updCheck = Check;
            updCheck.Id = Id;
            DateOnly dateFinish = updCheck.DateStart.AddDays(updCheck.PlannedDuration);
            updCheck.DateFinish = dateFinish;
            updCheck.SMPId = _SMP.Id;
            await _repository.UpdateCheck(updCheck);

            return RedirectToPage("/Index");
        }
    }
}
