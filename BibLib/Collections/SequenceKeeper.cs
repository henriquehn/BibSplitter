namespace BibLib.Collections
{
    public class SequenceKeeper
    {
        private Dictionary<string, int> store = new();
        private object storeLock = new();

        public int Next (string hash)
        {
            lock (storeLock)
            {
                if (store.TryGetValue(hash, out int current))
                {
                    current++;
                    store[hash] = current;
                    return current;
                }
                else
                {
                    store[hash] = 1;
                    return 1;
                }
            }
        }

        public void Reset()
        {
            lock (storeLock)
            {
                store.Clear();
            }
        }
    }
}
