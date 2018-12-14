namespace Geone.JCXX.Meta
{
    public class JCXX_CaseTimeLimit : BaseEntity
    {
        public static string GetTbName()
        {
            return "JCXX_CaseTimeLimit";
        }

        ///// <summary>
        ///// 节点类型
        ///// </summary>
        public string NodeType
        {
            get;
            set;
        }

        ///// <summary>
        ///// 节点类型描述
        ///// </summary>
        public string NodeTypeDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 时限
        ///// </summary>
        public int? TimeLimit
        {
            get;
            set;
        }

        ///// <summary>
        ///// 时限描述
        ///// </summary>
        public string TimeLimitDesc
        {
            get;
            set;
        }

        ///// <summary>
        ///// 备注
        ///// </summary>
        public string Note
        {
            get;
            set;
        }
    }
}