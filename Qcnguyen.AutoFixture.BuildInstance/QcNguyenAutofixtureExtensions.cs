using AutoFixture.Kernel;
using Fare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qcnguyen.AutoFixture.BuildInstance.GenLaw;

namespace AutoFixture
{
    public static class QcNguyenAutofixtureExtensions
    {
        private readonly static Random _random = new Random();

        public static InstanceComposer<T> BuildInstance<T>(this Fixture fixture, GenLaw law = null) where T : class
        {
            return new InstanceComposer<T>(fixture, law);
        }

        public static T GenerateExcept<T>(this Fixture fixture, T val) where T:Enum
        {
            return fixture.Create<Generator<T>>().First(x => !x.Equals(val));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxCharacters"></param>
        /// <returns></returns>
        public static string String(this Fixture fixture, int maxCharacters = 256)
        {
            var val = fixture.Create<string>();
            if(val.Length > maxCharacters)
                val = val.Substring(0, maxCharacters);
            return val;
        }

        public static string StringWithPattern(this Fixture fixture, string pattern)
        {
            var xeger = new Xeger(pattern);
            xeger.Generate();
            return xeger.Generate();
        }

        public static string StringEmail(this Fixture fixture)
        {
            return fixture.StringWithPattern(@"[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z]+");
        }

        public static string StringNotEmail(this Fixture fixture)
        {
            return fixture.RandomFromList(new string[] {
                fixture.StringWithPattern(@"\w+[^@]+"), //without @
                fixture.StringWithPattern(@"(.*@.*){2,}"), //more than 2 @
                fixture.StringWithPattern(@"\w+[^\.]+"), //without .
                fixture.StringWithPattern(@"[@\.]+"), //start with @ or .
                fixture.StringWithPattern(@".*@\..*"), //@ stay next to . each other
                fixture.StringWithPattern(@"\w+[^@\.]+"), //without both @ and .
                });
        }

        public static T RandomFromList<T>(this Fixture fixture, params T[] elements)
        {
            return fixture.RandomFromList(elements.AsEnumerable());
        }

        public static T RandomFromList<T>(this Fixture fixture, IEnumerable<T> elements)
        {
            var arr = elements.ToArray();
            var idx = _random.Next(arr.Length);
            return arr[idx];
        }

        public static T RandomFromEnum<T>(this Fixture fixture) where T : struct
        {
            return fixture.RandomFromList((T[])Enum.GetValues(typeof(T)));
        }

        public static int Int(this Fixture fixture, int min = 0, int max = int.MaxValue)
        {
            return _random.Next(min,max);
        }
        public static bool Bool(this Fixture fixture)
        {
            return fixture.Create<bool>();
        }

        public static DateTime DateTime(this Fixture fixture)
        {
            return fixture.Create<DateTime>();
        }

        public static string DateFormat(this Fixture fixture)
        {
            return fixture.RandomFromList("dd/MM/yyyy", "MM/dd/yyyy");
        }

        public static DateTime DateTimeGreaterThan(this Fixture fixture, DateTime date, TimeSpan leastTimeSpan)
        {
            var timespan = fixture.Create<TimeSpan>();
            if (timespan < leastTimeSpan) timespan = leastTimeSpan;
            return date.Add(timespan);
        }

        public static DateTime DateGreaterThan(this Fixture fixture, DateTime date)
        {
            return fixture.DateTimeGreaterThan(date, TimeSpan.FromDays(1));
        }

        public static DateTime DateGreaterThanOrEqual(this Fixture fixture, DateTime date, int equalityHappen1OutOf = 5)
        {
            if (_random.Next(equalityHappen1OutOf) == 0) return date;
            return fixture.DateTimeGreaterThan(date, TimeSpan.FromDays(1));
        }

        public static DateTime DateTimeLessThan(this Fixture fixture, DateTime date, TimeSpan leastTimeSpan)
        {
            var timespan = fixture.Create<TimeSpan>();
            if (timespan < leastTimeSpan) timespan = leastTimeSpan;
            return date.Subtract(timespan);
        }

        public static DateTime DateLessThan(this Fixture fixture, DateTime date)
        {
            return fixture.DateTimeLessThan(date, TimeSpan.FromDays(1));
        }

        public static DateTime DateLessThanOrEqual(this Fixture fixture, DateTime date, int equalityHappen1OutOf = 5)
        {
            if (_random.Next(equalityHappen1OutOf) == 0) return date;
            return fixture.DateTimeLessThan(date, TimeSpan.FromDays(1));
        }

        public static Guid Guid(this Fixture fixture)
        {
            return fixture.Create<Guid>();
        }
    }
}
