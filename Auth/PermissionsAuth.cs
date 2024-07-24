using static OtlobLap.Data.Utility;

namespace OtlobLap.Auth
{
    public static class PermissionsAuth
    {

        public static List<string> GetPermissions(string module)
        {
            return new List<string> {
            $"Permissions.{module}.View",
            $"Permissions.{module}.Add",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Remove",
            };
        }

        public static List<string> GetPermissionsList()
        {
            var allPermession = new List<string>();
            foreach (var module in Enum.GetNames<Modules>())
                allPermession.AddRange(GetPermissions(module));

            return allPermession;
        }

    }

}