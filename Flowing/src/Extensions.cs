using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    public static class Extensions
    {
        public static Type GetName(this IApp app)
        {
            return app.GetType();
        }
    }
}
