using System;
namespace Todo.Common
{
    /// <summary>
    /// name: Validation
    /// proc: 入力チェック
    /// </summary>
    public class Validation
    {
        public Validation()
        {
        }

        /// <summary>
        /// name: txtCheck
        /// proc: テキストチェック
        /// </summary>
        /// <param name="txt">タスクの内容</param>
        /// <param name="max_len">最大文字数</param>
        /// <param name="min_len">最小文字数</param>
        /// <param name="error">エラー内容</param>
        /// <returns>判定結果</returns>
        public static bool txtCheck(string txt, int max_len, int min_len, out string error)
        {
            bool ok = true;
            error = string.Empty;
            if (txt.Length > max_len)
            {
                ok = false;
                error = "文字数を" + max_len.ToString() + "以内にしてください。";
            }
            
            if (txt.Length < min_len)
            {
                ok = false;
                error = "文字数を" + min_len.ToString() + "以上にしてください。";
            }

            return ok;
        }

        /// <summary>
        /// name: dateCheck
        /// proc: 日付チェック
        /// </summary>
        /// <param name="txt">タスクの期日</param>
        /// <param name="error">エラー内容</param>
        /// <returns>判定結果</returns>
        public static bool dateCheck(string txt, out string error)
        {
            bool ok = true;
            error = string.Empty;
            if (txt.Length == 0)
            {
                ok = false;
                error = "日付を入力してください。";
            }

            return ok;
        }
    }
}
