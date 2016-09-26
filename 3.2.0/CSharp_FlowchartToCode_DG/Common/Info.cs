using System;
using System.Text;

namespace CSharp_FlowchartToCode_DG
{
    public abstract class Info
    {
        //版权信息通用模块
        public static readonly string VersionNum = "3.2.0";         //版本号
        public static readonly string Author = "QIXIAO 七小(東哥)";        //作者

        //设置版权信息
        private static string _CopyRight;
        public static string CopyRight
        {
            get
            {
                #region 版权信息
                StringBuilder str = new StringBuilder();    //版本信息
                str.Append("\t" + "/// <summary>" + "\r\n");
                str.Append("\t" + "/// QIXIAO CodesBuilders 七小代码生成器" + "\r\n");
                str.Append("\t" + "/// CopyRight（版权信息）------" + "\r\n");
                str.Append("\t" + "/// Version （版本号）:" + VersionNum + "\r\n");
                str.Append("\t" + "/// Author （作者）:" + Author + "\r\n");
                str.Append("\t" + "/// History Version 2.1.0 Made：2016-05-07 Asian China Tianjin" + "\r\n");
                str.Append("\t" + "/// History Version 2.2.0 Made：2016-06-05 Asian China Tianjin" + "\r\n");
                str.Append("\t" + "/// History Version 3.1.0 Made：2016-06-08 Asian China Tianjin" + "\r\n");
                str.Append("\t" + "/// Codes Standard （代码标准）：東哥的SqlHelper_DG" + "\r\n");
                str.Append("\t" + "/// Codes Builder Time (代码生成时间):" + DateTime.Now.ToString() + "\r\n");
                str.Append("\t" + "/// </summary>" + "\r\n");
                #endregion
                return str.ToString(); ;
            }
        }

    }
}
