namespace DentalStudioScheduler.Services.Base
{
    public static class PagingExtension
    {
        public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> data, int page, int size)
        {
            if (page > 0 && size > 0)
            {
                return data.Skip((page - 1) * size).Take(size);
            }

            return data;
        }
    }
}
