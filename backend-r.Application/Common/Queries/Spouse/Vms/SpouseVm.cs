namespace backend_r.Application.Common.Queries.Spouse
{
    public class SpouseVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Member.Vms.Gender Gender { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime DOB { get; set; }
        public bool IsActive { get; set; }
    }
}