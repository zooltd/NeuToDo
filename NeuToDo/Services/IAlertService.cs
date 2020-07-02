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
    }
}