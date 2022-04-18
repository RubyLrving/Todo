using System;
using System.Collections.Generic;
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
    public class EditController : Controller
    {
        private DBAccess m_dba = new DBAccess();
        private readonly ILogger<EditController> _logger;

        public EditController(ILogger<EditController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// name: PostUpTxt
        /// proc: タスクの編集内容を更新
        /// </summary>
        /// <param name="json">
        /// id 更新するタスクのid
        /// txt 編集内容
        /// </param>
        /// <returns>レスポンス結果</returns>
        [Route("api/[controller]")]
        [HttpPut]
        public IActionResult PostTodoEdit([FromBody] TaskItem json)
        {
            string error;
            string txt = HttpUtility.HtmlEncode(json.txt);

            if (!Validation.txtCheck(txt, 100, 1, out error))
            {
                return Json(new ResponseJson(false, error, ""));
            }

            if (m_dba.TodoEdit(json.id, json.txt))
            {
                //レスポンス生成
                return Json(new ResponseJson(true, "", txt));
            }

            //レスポンス生成
            return Json(new ResponseJson(false, "DB接続エラー", ""));
        }

        /// <summary>
        /// name: TaskItem
        /// proc: 追加するタスクのデータ
        /// </summary>
        public class TaskItem
        {
            public int id { get; set; }
            public string txt { get; set; }
        }

        /// <summary>
        /// name: ResponseJson
        /// proc: レスポンス結果
        /// </summary>
        [DataContract]
        public class ResponseJson
        {
            public ResponseJson(bool ok_, string error_, string txt_)
            {
                ok = ok_;
                error = error_;
                txt = txt_;
            }

            [DataMember]
            public bool ok { get; set; }
            [DataMember]
            public string txt { get; set; }
            [DataMember]
            public string error { get; set; }
        }
    }
}
