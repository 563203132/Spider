using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public static class SqlHelper
    {
        private static string _connectString = "Server=CNE1QAOMNIDB01.e1ef.com;Database=SpiderData;User Id=sa; Password=sa; MultipleActiveResultSets=true; max pool size=512;";
        private static string _sql = @"insert into CrawledItem
                                       (
                                         [Name],
                                         [Url],
                                         Detail,
                                         CreatedTime
                                       )
                                       values(@name, @url, @detail, @createdTime)";

        public static void Store(IEnumerable<CrawledItem> items)
        {
            using (var sqlConnection = new SqlConnection(_connectString))
            {
                sqlConnection.Open();

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    var parameters = new List<DynamicParameters>();
                    foreach (var item in items)
                    {
                        var param = new DynamicParameters();
                        param.Add("@name", item.Name);
                        param.Add("@url", item.Url);
                        param.Add("@detail", item.Detail);
                        param.Add("@createdTime", DateTime.Now);

                        parameters.Add(param);
                    }

                    sqlConnection.Execute(_sql, parameters, transaction);

                    transaction.Commit();
                }
            }
        }
    }
}
