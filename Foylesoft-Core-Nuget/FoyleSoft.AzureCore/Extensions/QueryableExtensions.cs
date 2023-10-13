using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FoyleSoft.AzureCore.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Include<T>(
            this IQueryable<T> source,
            Expression<Func<T, object>>[] navigationPropertyPaths)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (navigationPropertyPaths == null)
            {
                return source;
            }

            source = navigationPropertyPaths.Aggregate(
                source,
                (sourceIncludingNavigationProperties, navigationPropertyPath) =>
                    navigationPropertyPath == null ?
                        sourceIncludingNavigationProperties :
                        sourceIncludingNavigationProperties.Include(navigationPropertyPath));

            return source;
        }
    }
}