using System.ComponentModel.DataAnnotations;

namespace Test_task.Models.Entities
{
    public class SupervisoryEntity
    {
        [Required(ErrorMessage = "Название контролирующего органа обязательно")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Название контролирующего органа обязательно")]
        [StringLength(150, ErrorMessage = "Название не должно превышать 150 символов")]
        public string Name { get; set; } = string.Empty;

        public List<CheckEntity> Checks { get; set; } = [];
    }
}
