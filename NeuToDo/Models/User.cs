namespace NeuToDo.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Campus Campus { get; set; }
    }

    public enum Campus
    {
        NanhuCampus,
        HunnanCampus,
        ShenheCampus
    }
}