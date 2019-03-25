﻿namespace Geone.JCXX.WebService.Meta.QueryEntity
{
    public class Req_QSRole
    {
        /// <summary>
        /// 权属角色类型
        /// </summary>
        public string RoleType { get; set; }

        /// <summary>
        /// 权属角色类型
        /// </summary>
        public string NoContainsRoleType { get; set; }

        /// <summary>
        /// 权属角色编号（模糊查询）
        /// </summary>
        public string Like_RoleCode { get; set; }

        /// <summary>
        /// 权属角色名称（模糊查询）
        /// </summary>
        public string Like_RoleName { get; set; }

        /// <summary>
        /// 权属角色有效性 0无效/1有效
        /// </summary>
        public int? Enabled { get; set; }

        /// <summary>
        ///如果是2则表示要派遣到二级单位（如街道、执法等）
        /// </summary>
        public string OperatorLevel { get; set; }

        public string ParentCode { get; set; }
    }
}