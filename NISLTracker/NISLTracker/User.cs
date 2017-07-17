// ************************************************************************************
//
// 文件名(File Name):            User.cs
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
    public class User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// 安全戳
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// 身份
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// 实验室号
        /// </summary>
        public int Laboratory { get; set; }
    }
}
