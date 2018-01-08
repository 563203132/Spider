using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public static class SqlHelper
    {
        private static string _resourcePath = @"F:\AbotSpider\Spider\DB\res.txt";
        private static string _connectString = "Server=127.0.0.1;Database=SpiderData;User Id=sa; Password=abc123_; MultipleActiveResultSets=true; max pool size=512;";
        private static string _insertSql = @"insert into CrawledItem
                                             (
                                               [Name],
                                               [Url],
                                               Detail,
                                               CreatedTime,
                                               PageNumber
                                             )
                                             values(@name, @url, @detail, @createdTime, @pageNumber)";

        private static string _InitResourceSql = @"insert into [Resource]([Name])Values(@name)";
        private static string _getUnloadResource = @"select top 1000 [Name] from [Resource]
                                                     where [name] not in (select distinct [Name] from CrawledItem )";
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
                        param.Add("@pageNumber", item.PageNumber);

                        parameters.Add(param);
                    }

                    sqlConnection.Execute(_insertSql, parameters, transaction);

                    transaction.Commit();
                }
            }
        }

        public static void InitResource()
        {
            var res = File.ReadAllLines(_resourcePath);

            using (var sqlConnection = new SqlConnection(_connectString))
            {
                sqlConnection.Open();

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    var parameters = new List<DynamicParameters>();
                    foreach (var r in res)
                    {
                        var param = new DynamicParameters();
                        param.Add("@name", r);

                        parameters.Add(param);
                    }

                    sqlConnection.Execute(_InitResourceSql, parameters, transaction);

                    transaction.Commit();
                }
            }
        }

        public static List<string> UnloadResources()
        {
            var res = new List<string>();

            using (var sqlConnection = new SqlConnection(_connectString))
            {
                sqlConnection.Open();

                res = sqlConnection.Query<string>(_getUnloadResource).ToList();
            }

            return res;
        }
    }
}
