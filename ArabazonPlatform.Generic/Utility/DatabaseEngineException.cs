using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Generic.Utility
{
    public class DatabaseEngineException
    {
        public class DBConncetionException : Exception
        {
            public DBConncetionException(string name, Exception inner) : base(name, inner)
            {
            }
        }

        public class DBException : Exception
        {
            public DBException(string name, Exception inner) : base(name, inner)
            {
            }
        }
    }
}
