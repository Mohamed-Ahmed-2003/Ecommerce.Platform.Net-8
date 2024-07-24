namespace OtlobLap.ViewModels
{
    public class PermissionsVM
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List <RoleClaimsVM> RoleClaims { get; set; } = new List<RoleClaimsVM>();
      

    }
}
public class RoleClaimsVM
{
    public string Value { get; set; } = string.Empty;
    public bool Selected { get; set; }
}