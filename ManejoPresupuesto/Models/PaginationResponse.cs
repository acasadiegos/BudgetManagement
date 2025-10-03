namespace ManejoPresupuesto.Models
{
    public class PaginationResponse
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / RecordsPerPage);
        public string BaseURL { get; set; } = string.Empty;
    }

    public class PaginationResponse<T> : PaginationResponse
    {
        public IEnumerable<T> Elements { get; set; } = new List<T>();
    }
}
