using System.Reflection;

namespace CourseLibrary.API.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            // Check if the requested fields exist on source
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                // Reflection to check if the property can be found on T. 
                var propertyInfo = typeof(T)
                    .GetProperty(propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // It can't be found, return false
                if (propertyInfo == null)
                {
                    return false;
                }
            }

            // All checks out, return true
            return true;
        }
    }
}