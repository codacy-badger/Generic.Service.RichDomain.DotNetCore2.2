using System.Collections.Generic;

namespace Generic.Service.Models.BaseModel.BasePagination {
    public interface IBasePage<TReturn>
     where TReturn: class
     {
        IEnumerable<TReturn> Content { get; }

        int TotalElements { get; }

        string Sort { get; }

        string Order { get; }

        int Size { get; }

        int NumberPage { get; }
    }
}