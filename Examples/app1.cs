using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    class app1:IApp
    {

        public static void Main(string[] args)
        {
            IApp.main();
        }

        public void SetUp()
        {
            
            Size(1200, 1000);
            //Main m = new Main(this);
        }
    }
}
