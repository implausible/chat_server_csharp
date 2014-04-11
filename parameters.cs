using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace csharp_server
{
    class parameters<X>
    {
        public X a0;

        public parameters()
        {
        }

        public parameters(X p0)
        {
            a0 = p0;
        }
    }
    class parameters<X, Y>
    {
        public X a0;
        public Y a1;

        public parameters() { }

        public parameters(X p0)
        {
            a0 = p0;
        }

        public parameters(X p0, Y p1)
        {
            a0 = p0;
            a1 = p1;
        }
    }
    class parameters<X, Y, Z>
    {
        public X a0;
        public Y a1;
        public Z a2;

        public parameters() { }

        public parameters(X p0)
        {
            a0 = p0;
        }

        public parameters(X p0, Y p1)
        {
            a0 = p0;
            a1 = p1;
        }

        public parameters(X p0, Y p1, Z p2)
        {
            a0 = p0;
            a1 = p1;
            a2 = p2;
        }
    }
}
