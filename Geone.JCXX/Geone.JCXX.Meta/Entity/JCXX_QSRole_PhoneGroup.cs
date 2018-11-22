using Geone.Utiliy.Database;
using System;

namespace Geone.JCXX.Meta
{
    ///<summary>
    ///
    ///</summary>
    public class JCXX_QSRole_PhoneGroup : IEntity
    {
        ///// <summary>
        ///// GUID
        ///// </summary>
        public string ID
        {
            get;
            set;
        }

        ///// <summary>
        ///// GUID
        ///// </summary>
        public string RoleID
        {
            get;
            set;
        }

        ///// <summary>
        ///// GUID
        ///// </summary>
        public string GroupID
        {
            get;
            set;
        }

        ///// <summary>
        ///// ����ʱ��
        ///// </summary>
        public DateTime? CREATED
        {
            get;
            set;
        }

        ///// <summary>
        ///// ������
        ///// </summary>
        public string CREATED_MAN
        {
            get;
            set;
        }

        ///// <summary>
        ///// ����ʱ��
        ///// </summary>
        public DateTime? UPDATED
        {
            get;
            set;
        }

        ///// <summary>
        ///// ������
        ///// </summary>
        public string UPDATED_MAN
        {
            get;
            set;
        }
    }
}