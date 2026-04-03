using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Format
{
    public class ProgressCalculator
    {
        /// <summary>
        /// Tính ProgressPercentage cho JobCard theo quy tắc:
        /// - Nếu Service đã Completed → tính 100% cho Service đó
        /// - Nếu Service chưa Completed và có Task → tính theo % Task hoàn thành
        /// - Nếu Service chưa Completed và không có Task → tính theo trạng thái Service
        /// </summary>
        public int CalculateJobCardProgress(JobCard jobCard)
        {
            if (jobCard?.Services == null || !jobCard.Services.Any())
                return 0;

            int totalItems = 0;
            int completedItems = 0;

            foreach (var service in jobCard.Services)
            {
                var tasks = service.ServiceTasks?.ToList() ?? new List<JobCardServiceTask>();

                if (service.Status == ServiceStatus.Completed)
                {
                    // Service đã hoàn thành → tính 100% cho Service này
                    totalItems += 1;
                    completedItems += 1;
                }
                else if (tasks.Any())
                {
                    // Service chưa hoàn thành nhưng có Task con → tính theo Task
                    totalItems += tasks.Count;
                    completedItems += tasks.Count(t => t.Status == ServiceStatus.Completed);
                }
                else
                {
                    // Service chưa hoàn thành và không có Task → tính theo trạng thái Service
                    totalItems += 1;
                    if (service.Status == ServiceStatus.Completed)
                        completedItems += 1;
                }
            }

            return totalItems == 0 ? 0 : (int)Math.Round((double)completedItems / totalItems * 100);
        }

        /// <summary>
        /// Tính chuỗi hiển thị khoảng thời gian dự kiến hoàn thành (ví dụ: "13:45 - 13:55").
        /// </summary>
        /// <param name="startTime">Thời điểm bắt đầu thực tế (có thể null nếu chưa bắt đầu)</param>
        /// <param name="remainingMinutes">Tổng số phút ước tính còn lại</param>
        /// <param name="bufferMinutes">Khoảng buffer ± (mặc định 5 phút)</param>
        /// <returns>Chuỗi dạng "HH:mm - HH:mm" hoặc thông báo trạng thái</returns>
        public string? CalculateEstimatedCompletionDisplay(DateTime? startTime, long remainingMinutes, int bufferMinutes = 5)
        {
            if (!startTime.HasValue)
            {
                return "Chưa bắt đầu";
            }

            if (remainingMinutes <= 0)
            {
                return "Đã hoàn thành";
            }

            var baseEndTime = startTime.Value.AddMinutes(remainingMinutes);

            // Tạo khoảng ± buffer
            var minEnd = baseEndTime.AddMinutes(-bufferMinutes);
            var maxEnd = baseEndTime.AddMinutes(bufferMinutes);
            string format;
            if (minEnd.Date == maxEnd.Date)
            {
                format = "HH:mm";
            }
            else
            {
                format = "dd/MM HH:mm";
            }
            string minFormatted = minEnd.ToString(format);
            string maxFormatted = maxEnd.ToString(format);
            return $"{minFormatted} - {maxFormatted}";
        }
    }
}
