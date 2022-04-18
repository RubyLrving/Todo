using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Common;
using Todo;
using System.Runtime.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [ApiController]
    public class SelectController : Controller
    {
        private DBAccess m_dba = new DBAccess();
        private readonly ILogger<SelectController> _logger;

        public SelectController(ILogger<SelectController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// name: Get
        /// proc: タスクの検索結果を返す
        /// </summary>
        /// <param name="param">
        /// Allは全てのタスク
        /// Doneは完了したタスク
        /// Todoは未完了のタスク
        /// </param>
        /// <returns>検索結果</returns>
        [Route("api/[controller]")]
        [HttpGet]
        public ResponseJson Get([FromQuery] QueryParameter param)
        {
            // Todoを検索
            List<TodoItem> list = m_dba.Select(param.Filter);
            //エラー情報を返す
            if (list == null)
            {
                return new ResponseJson(false, "DB接続エラー");
            }
            //検索結果を返す
            ResponseJson res = new ResponseJson(true, "");
            res.list = list.AsEnumerable<TodoItem>();
            return res;
        }

        /// <summary>
        /// name: QueryParameter
        /// proc: パラメータクエリ
        /// </summary>
        public class QueryParameter
        {
            /// <summary>
            /// 抽出条件を指定（SQLのWHEREに相当）
            /// </summary>
            public string Filter { get; set; }
        }

        /// <summary>
        /// name: ResponseJson
        /// proc: レスポンス結果
        /// </summary>
        [DataContract]
        public class ResponseJson
        {
            public ResponseJson(bool ok_, string errorMsg_)
            {
                ok = ok_;
                errorMsg = errorMsg_;
            }
            // 通信結果, true: 成功, false: 失敗
            [DataMember]
            public bool ok { get; set; }
            // エラーメッセージ
            [DataMember]
            public string errorMsg { get; set; }
            // 検索結果
            [DataMember]
            public IEnumerable<TodoItem> list { get; set; }
        }
    }
}