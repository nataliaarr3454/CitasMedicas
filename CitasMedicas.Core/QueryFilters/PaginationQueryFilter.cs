namespace CitasMedicas.Core.QueryFilters
{
    public abstract class PaginationQueryFilter
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
