using System.ComponentModel.DataAnnotations;

namespace PathWay_Solution.Dto
{
    public class CounterCreateDto
    {
        [Required]
        public int RouteId { get; set; }

        [Required, MaxLength(100)]
        public string CounterName { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Address { get; set; } = null!;

        [Required, MaxLength(20)]
        public string ContactNumber { get; set; } = null!;
    }

    public class CounterUpdateDto
    {
        [Required]
        public int CounterId { get; set; }
        [Required]
        public int RouteId { get; set; }

        [Required, MaxLength(100)]
        public string CounterName { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Address { get; set; } = null!;

        [Required, MaxLength(20)]
        public string ContactNumber { get; set; } = null!;
    }

}
