using System;

namespace Generic.Repository.Models.BaseModel.BasePagination.BaseConfigurePage
{
    public interface IBaseConfigurePage
    {
        int page { get; set; }
        int size { get; set; }
        string sort { get; set; }
        string order { get; set; }
    }
}
