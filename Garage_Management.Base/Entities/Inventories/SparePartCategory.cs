using Garage_Management.Base.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Entities.Inventories
{
    public class SparePartCategory : AuditableEntity
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        public ICollection<Inventory> SparePart {  get; set; } = new List<Inventory>();

    }
}
