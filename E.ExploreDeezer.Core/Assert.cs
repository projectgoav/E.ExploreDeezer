using System;
using System.Collections.Generic;
using System.Text;

namespace E.ExploreDeezer.Core
{
    public static class Assert
    {
        public static void That(bool condition, string message = "Assertion failed.")
            => Assert.That(condition, () => message);

        public static void ObjectOfType<T>(object obj)
            => Assert.That(obj is T, () => $"Expected {typeof(T)} but object is: {obj.GetType()}.");


        // Allows more complex messages to only be constructed when
        // the condition is false
        private static void That(bool condition, Func<string> messageFactory)
        {
            if (!condition)
                throw new Exception(messageFactory());
        }
    }
}
