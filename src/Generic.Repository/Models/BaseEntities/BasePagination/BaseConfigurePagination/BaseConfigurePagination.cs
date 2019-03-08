namespace Generic.Repository.Models.BaseEntities.BasePagination
{
    public class BaseConfigurePagination
    {
        public int page { get; set; }
        public int size { get; set; }
        public string sort { get; set; }
        public string order { get; set; }
    }
}