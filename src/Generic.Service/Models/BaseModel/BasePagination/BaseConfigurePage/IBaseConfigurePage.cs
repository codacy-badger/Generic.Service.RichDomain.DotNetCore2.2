using System;

namespace Generic.Service.Models.BaseModel.BasePagination.BaseConfigurePage
{
    public interface IBaseConfigurePage
    {
        int page { get; set; }
        int size { get; set; }
        string sort { get; set; }
        string order { get; set; }
    }
}
