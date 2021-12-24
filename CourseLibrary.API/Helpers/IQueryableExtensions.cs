using CourseLibrary.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace CourseLibrary.API.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));

            if (mappingDictionary == null) 
                throw new ArgumentNullException(nameof(mappingDictionary));

            if (string.IsNullOrWhiteSpace(orderBy)) 
                return source;

            var orderByString = string.Empty;

            // The orderBy string is separated by ",", so split it
            var orderByAfterSplit = orderBy.Split(',');

            // Apply each orderby clause  
            foreach (var orderByClause in orderByAfterSplit)
            {
                // Trim, as it might containg leading or trailing spaces
                var trimmedOrderByClause = orderByClause.Trim();

                // If the sort option ends with " desc", we order desc, else asc
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // Remove " asc" or " desc" from the orderByClause, so we
                // get the property name to look for in the mapping dictionary
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1
                    ? trimmedOrderByClause
                    : trimmedOrderByClause.Remove(indexOfFirstSpace);

                // Find the matching property
                if (!mappingDictionary.ContainsKey(propertyName))
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");

                // Get the PropertyMappingValue
                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                    throw new ArgumentNullException(nameof(propertyMappingValue));

                // Revert sort order if necessary
                if (propertyMappingValue.Revert) orderDescending = !orderDescending;

                // Run through the property names
                // So the orderby clauses are applied in the correct order
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString)
                            ? string.Empty
                            : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");
                }
            }

            return source.OrderBy(orderByString);
        }
    }
}