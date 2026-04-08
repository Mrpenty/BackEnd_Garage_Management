namespace Garage_Management.Application.DTOs.JobCards
{
    public class ReorderJobCardQueueDto
    {
        public int JobCardId { get; set; }
        public int WorkBayId { get; set; }
        public int? PreviousJobCardId { get; set; }
        public int? NextJobCardId { get; set; }
    }
}
