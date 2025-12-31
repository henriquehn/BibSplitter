namespace BibLib.Collections
{
    public abstract class ElementHolder<T>
    {
        private readonly Dictionary<int, string> ElementsById = [];
        private readonly Dictionary<string, int> ElementsByNames = new(StringComparer.OrdinalIgnoreCase);

        public void Add(T element)
        {
            ElementsById[GetId(element)] = GetName(element);
            ElementsByNames[GetName(element)] = GetId(element);
        }

        public string this[int id]
        {
            get
            {
                return ElementsById[id];
            }
        }

        public int this[string name]
        {
            get
            {
                ElementsByNames.TryGetValue(name, out int id);
                if (id < 1)
                {
                    var newValue = CreateNew(name);
                    this.Add(newValue);
                    return GetId(newValue);
                }
                return id;
            }
        }

        protected abstract string GetName(T element);
        protected abstract int GetId(T element);
        protected abstract T CreateNew(string name);
    }
}
