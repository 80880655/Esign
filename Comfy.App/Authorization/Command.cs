using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.App.Authorization
{
    /// <summary>
    /// 操作命令。
    /// </summary>
    [System.FlagsAttribute]
    public enum Command
    {
        /// <summary>
        /// 查詢。
        /// </summary>
        Search = 1,
        /// <summary>
        /// 編輯。
        /// </summary>
        Edit = 2,
        /// <summary>
        /// 新增。
        /// </summary>
        New = 4,
        /// <summary>
        /// 刪除。
        /// </summary>
        Delete = 8,
        /// <summary>
        /// 導出。
        /// </summary>
        Export = 16,
        /// <summary>
        /// 導入。
        /// </summary>
        Import = 32,
        /// <summary>
        /// 設計。
        /// </summary>
        Design = 64,

        Approve = 128,
    }
}
