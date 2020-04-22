using System;
using System.Collections.Generic;
using System.Text;

namespace Deliver.Data.Common
{
    public interface ICommonProps : IAuditable
    {
        object GetId();
    }

    public interface ICommonProps<T> : ICommonProps
    {
        T Id { get; set; }
        new T GetId();
    }
}
