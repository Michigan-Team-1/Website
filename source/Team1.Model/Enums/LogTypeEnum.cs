using Team1.Model.Attributes;

namespace Team1.Model.Enums
{
    [TypeWriterIgnore]
    public enum LogTypeEnum : byte
    {
        Unknown = 0,
        LoginInvalidLogin = 1,
        LoginLockOut = 2,
        LoginFindUserName = 3,
        LoginFindEmail = 4,
        LoginSuccess = 5,
        User = 6,
        Address = 7,

        ReCaptcha = 100,
        VerifyOneTimePassword = 101,
        CreateAuthyUser = 102,

        /// <summary>
        /// Log type reserved for unhandled exceptions caught by the middleware
        /// </summary>
        UncaughtError = 200,
       
    }
}
