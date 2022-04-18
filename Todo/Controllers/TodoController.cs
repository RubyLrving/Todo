using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Common;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    public class TodoController : Controller
    {
        private DBAccess m_dba = new DBAccess();
        private readonly ILogger<TodoController> _logger;

        public TodoController(ILogger<TodoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// name: PostTodo
        /// proc: 未完了フラグの更新
        /// </summary>
        /// <param name="json">タスクのid, 未完了フラグの状態</param>
        /// <returns>レスポンス結果</returns>
        [Route("api/[controller]")]
        [HttpPut]
        public IActionResult PostTodo([FromBody] TaskItem json)
        {
            if (m_dba.Todo(json.id))
            {
                //レスポンス生成
                return Json(new ResponseJson(true, "", new DateTime(1900, 1, 1).ToString("yyyy-MM-dd")));
            }

            return Json(new ResponseJson(false, "DB接続エラー", ""));
        }

        /// <summary>
        /// name: TaskItem
        /// proc: タスク
        /// </summary>
        public class TaskItem
        {
            public int id { get; set; }
        }

        /// <summary>
        /// name: ResponseJson
        /// proc: レスポンス結果
        /// </summary>
        [DataContract]
        public class ResponseJson
        {
            public ResponseJson(bool ok_, string errorMsg_, string date_)
            {
                ok = ok_;
                errorMsg = errorMsg_;
                date = date_;
            }
            // 通信結果, true: 成功, false: 失敗
            [DataMember]
            public bool ok { get; set; }
            // エラーメッセージ
            [DataMember]
            public string errorMsg { get; set; }
            [DataMember]
            public string date { get; set; }
        }
    }
}
