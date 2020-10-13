using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RC4文件加解密
{
    public class threadVar
    {
        public int var = 0;

        public void setVar(int i)
        {
            this.var = i;
        }

        public int getVar()
        {
            return this.var;
        }
    }
}
