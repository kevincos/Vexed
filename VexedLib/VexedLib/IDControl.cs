
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VL
{
    public class IDControl
    {
        static int nextId=0;

        public static int GetID()
        {            
            nextId++;
            return nextId;
        }
    }
}
