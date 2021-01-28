using System;
using System.Collections.Generic;
using System.Text;
using Comfy.App.Authorization;

namespace Comfy.App
{
    public enum Module
    {
        #region AppSetting

        UserInfo = 1,
        Login = 2,
        Home = 3,
        Default = 4,        
        AppUserFavorite = 9,
        AppUserFastCreate = 10,
        AppUserSettings = 11,

        [Command(Command.Search
            | Command.Export
            | Command.Delete
            | Command.Edit
            | Command.New)]
        AppRole = 6,

        [Command(Command.Search
            | Command.Delete
            | Command.Edit
            | Command.New)]
        AppNavigation = 7,

        [Command(Command.Search
            | Command.Export
            | Command.Edit
            | Command.Delete
            | Command.New)]
        AppUser = 8,

        [Command(Command.Search
            | Command.Export
            | Command.Delete
            | Command.Edit
            | Command.New)]
        AppNotice = 12,
        #endregion


    }
}
