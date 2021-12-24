namespace CourseLibrary.API.ResourceParameters
{
    public class AuthorsResourceParameters
    {
        // Always use a max page size for when the user tries for example 1000
        private const int maxPageSize = 20;

        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }

        // Important to give defaults, for when the user does not
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize 
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        // For authors sorting, name is an okay default
        // But, name is not on the entity, hence map it
        public string OrderBy { get; set; } = "Name";
        public string Fields { get; set; }
    }
}