namespace Coursea.Dto.Course
{
    public class ListCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Code_course { get; set; } = string.Empty;
    }
}
