namespace BibLib.Collections
{
    /// <summary>
    /// Provê um armazém de estados seguro para threads.
    /// </summary>
    /// <remarks>
    /// O armazém de estados é seguro para threads, mas isso não garante que os objetos armazenados também são.
    /// </remarks>
    public class ThreadSafeStateMachine
    {
        private Dictionary<string, object> states = new();
        private object lockObj = new();

        /// <summary>
        /// Obtém o valor associado à chave especificada e o remove do estado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetOnce<T>(string key)
        {
            lock (states)
            {
                var response = default(T);
                if (states.ContainsKey(key))
                {
                    response = (T)states[key];
                    states.Remove(key);
                }
                return response;
            }
        }

        /// <summary>
        /// Obtém o valor associado à chave especificada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            lock (states)
            {
                if (states.ContainsKey(key))
                {
                    return (T)states[key];
                }
                return default(T);
            }
        }

        /// <summary>
        /// Atribui um valor à chave especificada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set<T>(string key, T value)
        {
            lock (states)
            {
                states[key] = value;
            }
        }

        /// <summary>
        /// Determina se o estado contém a chave especificada.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Has(string key)
        {
            lock (states)
            {
                return states.ContainsKey(key);
            }
        }

        /// <summary>
        /// Descarta o valor associado à chave especificada.
        /// </summary>
        /// <param name="key"></param>
        public void Discard(string key)
        {
            lock (states)
            {
                if (states.ContainsKey(key))
                {
                    states.Remove(key);
                }
            }
        }

        /// <summary>
        /// Descarta todos os estados armazenados.
        /// </summary>
        public void Clear()
        {
            lock (states)
            {
                states.Clear();
            }
        }
    }
}
