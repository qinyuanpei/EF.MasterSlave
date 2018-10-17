using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;
using Newtonsoft.Json;
using System.IO;

namespace EF.MasterSlave
{
    public static class MasterSlaveManager
    {
        private static MasterSalveConfig _config => LoadConfig();

        /// <summary>
        /// 加载主从配置
        /// </summary>
        /// <param name="fileName">配置文件</param>
        /// <returns></returns>
        public static MasterSalveConfig LoadConfig(string fileName = "masterslave.config.json")
        {
            if (!File.Exists(fileName)) throw new Exception(string.Format("配置文件{0}不存在", fileName));
            return JsonConvert.DeserializeObject<MasterSalveConfig>(File.ReadAllText(fileName));
        }

        /// <summary>
        /// 切换到主库
        /// </summary>
        /// <param name="command">DbCommand</param>
        public static void SwitchToMaster(DbCommand command, string serverName = "")
        {
            DbServer masterServer = null;
            if (string.IsNullOrEmpty(serverName))
            {
                masterServer = _config.Masters.FirstOrDefault();
            }
            else
            {
                masterServer = _config.Slaves.FirstOrDefault(e => e.ServerName == serverName);
            }

            if (masterServer == null) throw new Exception("未配置主库服务器或者服务器名称不正确");

            //切换数据库连接
            ChangeDbConnection(command, masterServer);
        }

        /// <summary>
        /// 切换到从库
        /// </summary>
        /// <param name="command">DbCommand</param>
        public static void SwitchToSlave(DbCommand command, string serverName = "")
        {
            DbServer salveServer = null;
            if (string.IsNullOrEmpty(serverName))
            {
                salveServer = _config.Slaves.FirstOrDefault();
            }
            else
            {
                salveServer = _config.Slaves.FirstOrDefault(e => e.ServerName == serverName);
            }


            if (salveServer == null) throw new Exception("未配置从库服务器或者服务器名称不正确");

            //切换数据库连接
            ChangeDbConnection(command, salveServer);
        }

        /// <summary>
        /// 切换数据库连接
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dbServer"></param>
        private static void ChangeDbConnection(DbCommand command, DbServer dbServer)
        {
            var conn = command.Connection;
            if (conn.State == System.Data.ConnectionState.Open) conn.Close();
            conn.ConnectionString = dbServer.ConnectionString;
            conn.Open();
        }
    }

    [Serializable]
    public class MasterSalveConfig
    {
        /// <summary>
        /// 主库配置
        /// </summary>
        public List<DbServer> Masters { get; set; }

        /// <summary>
        /// 从库配置
        /// </summary>
        public List<DbServer> Slaves { get; set; }
    }

    [Serializable]
    public class DbServer
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
    }


}
