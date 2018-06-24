using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using KatlaSport.DataAccess;
using KatlaSport.Services.Tests.DbAsync;
using Moq;
using Moq.Language.Flow;

namespace KatlaSport.Services.Tests
{
    /// <summary>
    /// Contains extentions for Moq.
    /// </summary>
    public static class MockExtensions
    {
        public static IReturnsResult<TMock> ReturnsEntitySet<TMock, TResult>(this ISetup<TMock, IEntitySet<TResult>> setup, IList<TResult> items)
            where TMock : class
            where TResult : class
        {
            return setup.Returns(new FakeEntitySet<TResult>(items));
        }

        public static IReturnsResult<TMock> SetupEntitySet<TMock, TResult>(this Mock<TMock> mock, Expression<Func<TMock, IEntitySet<TResult>>> expression, IList<TResult> items)
            where TMock : Mock<TMock>
            where TResult : class
        {
            return mock.Setup(expression).Returns(new FakeEntitySet<TResult>(items));
        }

        public static IReturnsResult<TMock> ReturnsAsyncEntitySet<TMock, TResult>(this ISetup<TMock, IEntitySet<TResult>> setup, IList<TResult> items)
            where TMock : class
            where TResult : class
        {
            var data = items.AsQueryable();

            var mockSet = new Mock<IEntitySet<TResult>>();

            mockSet.As<IDbAsyncEnumerable<TResult>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<TResult>(data.GetEnumerator()));

            mockSet.As<IQueryable<TResult>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<TResult>(data.Provider));

            mockSet.As<IQueryable<TResult>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TResult>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TResult>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return setup.Returns(mockSet.Object);
        }
    }
}
