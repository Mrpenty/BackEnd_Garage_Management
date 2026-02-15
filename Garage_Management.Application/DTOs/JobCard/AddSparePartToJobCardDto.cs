using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCard
{
    public class AddSparePartToJobCardDto
{
    public int SparePartId { get; set; }
    public int Quantity { get; set; }
    public bool IsUnderWarranty { get; set; }
    public string? Note { get; set; }
}


}
