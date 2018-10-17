using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Common;

namespace EF.MasterSlave
{
    public class MasterSlaveDbInterceptor : DbCommandInterceptor
    {
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            //Insert/Update(写操作)走主库
            foreach(var context in interceptionContext.DbContexts)
            {
                if(context.Database.CurrentTransaction==null)
                {
                    MasterSlaveManager.SwitchToMaster(command);
                }
            }

            base.NonQueryExecuting(command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            //Select(读操作)走从库
            var sqlText = command.CommandText;
            if (!sqlText.ToUpper().StartsWith("INSERT") || !sqlText.ToUpper().StartsWith("UPDATE"))
                MasterSlaveManager.SwitchToSlave(command);
            base.ScalarExecuting(command, interceptionContext);
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //Select(读操作)走从库
            var sqlText = command.CommandText;
            if (!sqlText.ToUpper().StartsWith("INSERT") || !sqlText.ToUpper().StartsWith("UPDATE"))
                MasterSlaveManager.SwitchToSlave(command);
            base.ReaderExecuting(command, interceptionContext);
        }
    }
}
