using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using MySqlConnector;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using K4os.Compression.LZ4.Internal;
using System.Linq;

namespace FencingtrackerBot.References.SQL
{
    public class SQL
    {
        public static DbConnection Connection;
        public static ServerContext Context;

        // Establishes a safe and secure connection with the provided server
        public static void EstablishConnection(IConfigurationRoot Configuration)
        {
            MySqlConnectionStringBuilder Builder = new MySqlConnectionStringBuilder()
            { 
                Server = Configuration["sql:server"],
                Database = Configuration["sql:database"],
                UserID = Configuration["sql:user"],
                Password = Configuration["sql:password"],
                Port = uint.Parse(Configuration["sql:port"]),
                SslMode = MySqlSslMode.None
            };

            Connection = new MySqlConnection(Builder.ConnectionString);

            Console.WriteLine($"{DateTime.UtcNow.ToString("hh:mm:ss")} [Info] Connecting to server");
            
            Connection.Open();
            Context = new ServerContext();

            Console.WriteLine($"{DateTime.UtcNow.ToString("hh:mm:ss")} [Info] Connected");
        }

        public static void Add(Member Item)
        {
            Member Member = GetMember(Convert.ToUInt64(Item.UserId));

            if (Member != null)
                Context.Remove(Member);

            Context.Add(Item);
            Context.SaveChanges();
        }

        public static Member GetMember(ulong Id)
        {
            foreach (var Member in Context.Members)
            {
                if (Member.UserId == Convert.ToInt64(Id))
                    return Member;
            }

            return null;
        }

        public static int GetVerificationsCount()
        {
            int Return = 0;
            foreach (var Member in Context.Verification)
                Return++;
            return Return;
        }
    }
}
