using System;
using System.Threading.Tasks;

namespace AsyncEventHandler
{
    public delegate Task AsyncEventHandler<T>(object sender, T argument);

    public delegate Task AsyncEventHandler(object sender, EventArgs args);
}