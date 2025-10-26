namespace DentalStudioScheduler.Data.Models
{
    public class PageResult<T> : PagingBase
    {
        public List<T> Result { get; set; }
    }
}
