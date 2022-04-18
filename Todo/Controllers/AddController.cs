using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Common;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [ApiController]
    public class AddController : Controller
    {
        private DBAccess m_dba = new DBAccess();
        private readonly ILogger<AddController> _logger;

        public AddController(ILogger<AddController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// name: PostTaskItem
        /// proc: タスクを追加する
        /// </summary>
        /// <param name="json">タスクの内容、期日のTaskItemデータ</param>
        /// <returns>レスポンス結果</returns>
        [Route("api/[controller]")]
        [HttpPost]
        public IActionResult PostTaskItem([FromBody] TaskItem json)
        {
            string txt = HttpUtility.HtmlEncode(json.txt);
            string error;
            // 入力チェック
            if (!Validation.txtCheck(txt, 100, 1, out error))
            {
                return Json(new ResponseJson(false, error));
            }
            if (!Validation.dateCheck(json.date, out error))
            {
                return Json(new ResponseJson(false, error));
            }
            //DB処理
            if(m_dba.AddTask(json.txt, json.date))
            {
                return Json(new ResponseJson(true, ""));
            }
            return Json(new ResponseJson(false, "DB接続エラー"));
        }

        /// <summary>
        /// name: TaskItem
        /// proc: 追加するタスクのデータ
        /// </summary>
        public class TaskItem
        {
            public string txt { get; set; }
            public string date { get; set; }
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
