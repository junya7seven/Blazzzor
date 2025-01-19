namespace BlazorTemplateAPI.Models
{
    public class PagginationModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int LatPage { get; set; }
    }
}
