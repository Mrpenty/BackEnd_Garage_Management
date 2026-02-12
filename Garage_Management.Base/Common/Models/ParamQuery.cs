namespace Garage_Management.Base.Common.Models
{
    public class ParamQuery
    {
        public ParamQuery()
        {
            this.OrderBy = string.Empty;

            this.SortOrder = "ASC";

            this.Filter = string.Empty;

            this.Search = string.Empty;

            this.PageSize = 10;

            this.Page = 1;
        }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Filter { get; set; } = string.Empty;

        public string? Search { get; set; } = string.Empty;

        public string? OrderBy { get; set; } = string.Empty;

        public string? SortOrder { get; set; } = "ASC"; // ASC | DESC

    }
}
