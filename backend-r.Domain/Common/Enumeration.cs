namespace backend_r.Domain.Common
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }
        public int Id { get; private set; }

        protected Enumeration() {}
        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;
        public static IEnumerable<T> GetAll<T>() where T : Enumeration => typeof(T).GetFields(BindingFlags.Instance |
            BindingFlags.Public | BindingFlags.DeclaredOnly).Select(f => f.GetValue(null)).Cast<T>();
        
        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue) return false;
            var typesMatch = GetType().Equals(obj.GetType());
            var valuesMatch = Id.Equals(otherValue.Id);
            return typesMatch && valuesMatch;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsolutesDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absolutesDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absolutesDifference;
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingValue = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingValue;
        }

        public static T FromDispalyName<T>(string name) where T : Enumeration { return Parse<T, string>(name, "name", item =>
            item.Name == name); }
        
        public static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value} is not '{description}' in {typeof(T)}");

            return matchingItem;
        }

        public int CompareTo(object obj) => Id.CompareTo(((Enumeration)obj).Id);
    }
}