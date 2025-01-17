namespace BlazorTemplateAPI.Models
{
    public class PagginatedModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int LastPage { get; set; }
    }
}
