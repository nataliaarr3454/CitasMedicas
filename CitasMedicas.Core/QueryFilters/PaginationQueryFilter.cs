namespace CitasMedicas.Core.QueryFilters
{
    public abstract class PaginationQueryFilter
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
