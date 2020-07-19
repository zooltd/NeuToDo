using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ICampusStorageService
    {
        /// <summary>
        /// 返回Campus，如偏好存储中没有则弹出选择窗口
        /// </summary>
        /// <returns></returns>
        Task<Campus> GetOrSelectCampus();

        /// <summary>
        /// 返回偏好存储中的Campus，如没有则返回空Campus（非null）
        /// </summary>
        /// <returns></returns>
        Campus GetCampus();

        /// <summary>
        /// 保存学期信息到偏好存储
        /// </summary>
        /// <param name="campus"></param>
        void SaveCampus(Campus campus);
    }
}