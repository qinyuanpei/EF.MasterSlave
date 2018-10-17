using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure.Interception;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.MasterSlave
{
    class Program
    {
        static void Main(string[] args)
        {
            DbInterception.Add(new MasterSlaveDbInterceptor());
            using (var context = new DataContext())
            {
                context.Users.Add(new User()
                {
                    UserName = "小古",
                    UserRole = "Lover"
                });

                context.Users.First(e => e.UserName == "小古");
            }
        }
    }
}
