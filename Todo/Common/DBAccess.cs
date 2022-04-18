using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace Todo.Common
{
    /// <summary>
    /// name: DBAccess
    /// proc: DBアクセス時の処理を記載する
    /// </summary>
    public class DBAccess
    {
        private static IConfiguration Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBAccess()
        {
        }

        /// <summary>
        /// name: Select
        /// proc: Taskを検索する
        /// </summary>
        /// <param name="dbt">dbt　検索結果</param>
        /// <param name="filter">検索条件</param>
        /// <returns>DBアクセスの結果</returns>
        public List<TodoItem> Select(string filter)
        {
            //sqlクエリ
            string sqlquery = $@"
                SELECT *
                FROM Todo 
            ";
            if(filter == "Todo")
            {
                sqlquery += " Where done = 0;";
            }
            else if(filter == "Done")
            {
                sqlquery += " Where done = 1;";
            }

            // DB接続
            DataSet ds = NonQueryConnection(sqlquery);
            if (ds == null) return null;

            // 検索結果をlist化
            List<TodoItem> list = new List<TodoItem>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                TodoItem item =
                    new TodoItem
                    {
                        id = Convert.ToInt32(row["id"]),
                        txt = row["txt"].ToString(),
                        done = Convert.ToBoolean(row["done"]),
                        endDate = Convert.ToDateTime(row["endDate"]).ToString("yyyy-MM-dd"),
                        compDate = Convert.ToDateTime(row["compDate"]).ToString("yyyy-MM-dd"),
                    };
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// name: Delete
        /// proc: Taskを削除する
        /// </summary>
        /// <param name="id">削除するid</param>
        /// <returns>DBアクセスの結果</returns>
        public bool Delete(int id)
        {
            string sqlquery = $@"
                DELETE FROM Todo
                WHERE id = @id;
            ";

            //DB接続
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            // SqlParameterを生成
            sqlParameters.Add(new SqlParameter("@id", id));

            if (NonQueryConnection(sqlquery, sqlParameters))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// name: AddTask
        /// proc: Taskを追加する
        /// </summary>
        /// <param name="txt">追加するテキスト</param>
        /// <param name="date">期日</param>
        /// <returns>DBアクセスの結果</returns>
        public bool AddTask(string txt, string date)
        {
            string sqlquery = $@"
                INSERT INTO Todo
                (txt, endDate)
                VALUES (@txt, @endDate)
            ";

            // SqlParameterのリストを生成
            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@txt", txt));
            sqlParameters.Add(new SqlParameter("@endDate", Convert.ToDateTime(date)));

            if (NonQueryConnection(sqlquery, sqlParameters))
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// name: TxtEdit
        /// proc: テキストの編集 
        /// </summary>
        /// <param name="id">編集するタスクのid</param>
        /// <param name="txt">タスクの内容</param>
        /// <returns>DBのアクセス結果</returns>
        public bool TodoEdit(int id, string txt)
        {
            string sqlquery = $@"
                UPDATE Todo
                SET txt = @txt
                WHERE id = @id
            ";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            // SqlParameterを生成
            sqlParameters.Add(new SqlParameter("@id", id));
            sqlParameters.Add(new SqlParameter("@txt", txt));

            //DB接続
            if (NonQueryConnection(sqlquery, sqlParameters))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// name: UpDone
        /// proc: 完了フラグの更新
        /// </summary>
        /// <param name="id">完了するタスクのid</param>
        /// <returns>DBのアクセス結果</returns>
        public bool Done(int id)
        {
            string sqlquery = $@"
                UPDATE Todo
                SET done = 1, compDate = @compDate
                WHERE id = @id
            ";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            // SqlParameterを生成
            sqlParameters.Add(new SqlParameter("@id", id));
            sqlParameters.Add(new SqlParameter("@compDate", DateTime.Now));

            //DB接続
            if(NonQueryConnection(sqlquery, sqlParameters)){
                return true;
            }

            return false;
        }

        /// <summary>
        /// name: Todo
        /// proc: 未完了フラグの更新
        /// </summary>
        /// <param name="id">完了するタスクのid</param>
        /// <returns>DBのアクセス結果</returns>
        public bool Todo(int id)
        {
            string sqlquery = $@"
                UPDATE Todo
                SET done = 0, compDate = @compDate
                WHERE id = @id
            ";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            // SqlParameterを生成
            sqlParameters.Add(new SqlParameter("@id", id));
            sqlParameters.Add(new SqlParameter("@compDate", new DateTime(1900, 1, 1)));

            //DB接続
            if (NonQueryConnection(sqlquery, sqlParameters))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// name: NonQueryConnection
        /// proc: 非クエリの実行
        /// </summary>
        /// <param name="sqlquery">sqlのクエリ</param>
        /// <param name="sqlParameters">SqlParameterのリスト</param>
        /// <returns></returns>
        public bool NonQueryConnection(string sqlquery, List<SqlParameter> sqlParameters)
        {
            //DB接続先情報
            bool ok = true;
            using (SqlConnection connection = new SqlConnection(Config.GetConnectionString("WebApplication1Context")))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    using (SqlCommand command = new SqlCommand()
                    {
                        Connection = connection,
                        Transaction = transaction
                    })
                    {
                        try
                        {
                            command.CommandText = sqlquery;
                            foreach (SqlParameter param in sqlParameters)
                            {
                                command.Parameters.Add(param);
                            }
                            // SQLの実行
                            command.ExecuteNonQuery();
                            // コミット
                            transaction.Commit();
                        }
                        catch
                        {
                            // ロールバック
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    ok = false;
                    throw;
                }
                finally
                {
                    // データベースの接続終了
                    connection.Close();
                }
            }
            return ok;
        }

        /// <summary>
        /// name: NonQueryConnection
        /// proc: タスクの検索を行う
        /// </summary>
        /// <param name="string">sqlのクエリ</param>
        /// <returns>検索結果</returns>
        public DataSet NonQueryConnection(string sqlquery)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(Config.GetConnectionString("WebApplication1Context")))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    using (adapter.SelectCommand = new SqlCommand(sqlquery, connection))
                    {
                        adapter.Fill(ds);
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            return ds;
        }
    }
}
