namespace WebApp.Models
{
    public class DepartmentView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ChiefName { get; set; }
        public string ParentName { get; set; }
        public string Tree { get; set; }
    }
}
