using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Method used for datashaping
        /// Datashaping allows the consumer of the API to choose the resource fields (http://host/api/authors?fields=id,name)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source, string fields)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));

            // Create a list to hold the ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            // Create a list with PropertyInfo objects on TSource. Reflection is
            // expensive, so rather than doing it for each object in the list, we do 
            // it once and reuse the results. After all, part of the reflection is on the 
            // type of the object (TSource), not on the instance
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                // All public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                        .GetProperties(BindingFlags.IgnoreCase
                        | BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    // Use reflection to get the property on the source object
                    // we need to include public and instance, because specifying a binding 
                    // flag overwrites the already-existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on" +
                            $" {typeof(TSource)}");
                    }

                    // Add propertyInfo to list 
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // Now we have a list of propertyInfos of all the properties we want in our ExpandoObject
            // Now we need to get those from the source objects
            foreach (TSource sourceObject in source)
            {
                // Create an ExpandoObject that will hold the 
                // selected properties & values
                var dataShapedObject = new ExpandoObject();

                // Get the value of each property we have to return. For that,
                // we run through the list
                foreach (var propertyInfo in propertyInfoList)
                {
                    // GetValue returns the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    // Add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                // Add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            // Return the list
            return expandoObjectList;
        }
    }
}