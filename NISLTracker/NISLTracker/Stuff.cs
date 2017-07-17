// ************************************************************************************
//
// 文件名(File Name):            Stuff.cs
//
// 数据表(Tables):               None
//
// 作者(Author):                 曹帅(Implementist)
//
// 创建日期(Create Date):        2017年07月15日
//
// 修改记录(Revision History):   
//
// ************************************************************************************

namespace NISLTracker
{
    public class Stuff
    {
        /// <summary>
        /// 物资Id号
        /// </summary>
        public int StuffId { get; set; }

        /// <summary>
        /// 物资名称
        /// </summary>
        public string StuffName { get; set; }

        /// <summary>
        /// 物资估值
        /// </summary>
        public int ValueOfAssessment { get; set; }

        /// <summary>
        /// 物资状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 物资拥有者
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 当前持有者
        /// </summary>
        public string CurrentHolder { get; set; }
    }
}
