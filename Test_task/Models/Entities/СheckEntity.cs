using System.ComponentModel.DataAnnotations;

namespace Test_task.Models.Entities
{
    public class CheckEntity
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage ="Дата начала обязательна")]
        public DateOnly DateStart { get; set; }

        public DateOnly DateFinish { get; set; }

        [Required(ErrorMessage ="Введите кол-во дней")]
        [Range(1, 365, ErrorMessage ="Длительность плановой проверки должна быть от 1 до 365 дней")]
        public int PlannedDuration { get; set; } = 0;

        public Guid SMPId { get; set; }

        public SMPEntity? SMP { get; set; }

        [Required(ErrorMessage ="Название контролирующего органа обязательно")]
        public Guid SupervisoryId { get; set; }

        public SupervisoryEntity? Supervisory { get; set; }
    }
}
