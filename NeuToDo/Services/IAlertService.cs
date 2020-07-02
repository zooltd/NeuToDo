using System.Threading.Tasks;

namespace NeuToDo.Services
{
    /// <summary>
    /// 警告服务。
    /// </summary>
    public interface IAlertService
    {
        /// <summary>
        /// 显示警告。
        /// </summary>
        /// <param name="title">标题。</param>
        /// <param name="content">内容</param>
        /// <param name="button">按钮文字。</param>
        void DisplayAlert(string title, string content, string button);

        /// <summary>
        /// 警告对话选择框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="accept"></param>
        /// <param name="cancel"></param>
        /// <param name="button"></param>
        Task<bool> DisplayAlert(string title, string content, string accept, string cancel);
    }
}