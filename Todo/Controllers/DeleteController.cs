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
    [ApiController]
    public class DeleteController : Controller
    {
        private DBAccess m_dba = new DBAccess();
        private readonly ILogger<DeleteController> _logger;

        public DeleteController(ILogger<DeleteController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// name: PostTaskItem
        /// proc: タスクを削除する
        /// </summary>
        /// <param name="json">削除するタスクのid</param>
        /// <returns>レスポンス結果</returns>
        [Route("api/[controller]")]
        [HttpDelete]
        public IActionResult PostTaskItem([FromBody] TaskItem json)
        {
            if (m_dba.Delete(json.id))
            {
                //レスポンス生成
                return Json(new ResponseJson(true, ""));
            }

            return Json(new ResponseJson(false, "DB接続エラー"));
        }

        /// <summary>
        /// name: TaskItem
        /// proc: 削除するタスクのデータ
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
        }
    }
}
