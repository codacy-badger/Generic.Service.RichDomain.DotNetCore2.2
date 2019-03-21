namespace Generic.Service.Models.BaseModel.BasePagination.BaseConfigurePage
{
    public class BaseConfigurePage : IBaseConfigurePage
    {
        public BaseConfigurePage() { }
        public int page { get; set; }
        public int size { get; set; }
        public string sort { get; set; }
        public string order { get; set; }
    }
}