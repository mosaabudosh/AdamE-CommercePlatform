using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Generic.Utility
{
    public class DapperParameters
    {
        private DynamicParameters dynamicParameters;

        public DynamicParameters DynamicParameters => dynamicParameters;

        public DapperParameters()
        {
            dynamicParameters = new DynamicParameters();
        }

        public void AddParameter(string name, object value, DbType? dbType = null, ParameterDirection? parameterDirection = null, int? size = null)
        {
            dynamicParameters.Add(name, value, dbType, parameterDirection, size);
        }

        public void AddDynamicParams(object parameter)
        {
            dynamicParameters.AddDynamicParams(parameter);
        }
    }
}
