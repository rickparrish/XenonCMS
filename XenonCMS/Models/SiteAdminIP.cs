namespace XenonCMS.Models
{
    public partial class SiteAdminIP
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Address { get; set; }

        public virtual Site Site { get; set; }
    }
}
