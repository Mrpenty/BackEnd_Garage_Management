namespace Garage_Management.Base.Common.Models
{
    public class Pagination
    {
        public required object PageData { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }
    }
}
