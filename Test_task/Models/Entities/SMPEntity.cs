using System.ComponentModel.DataAnnotations;

namespace Test_task.Models.Entities
{
    public class SMPEntity
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Название СМП обязательно")]
        [StringLength(150, MinimumLength = 5, ErrorMessage ="Название должно быть от 5 до 150 символов")]
        public string Name { get; set; } = string.Empty;

        public List<CheckEntity> Checks { get; set; } = [];
    }
}
