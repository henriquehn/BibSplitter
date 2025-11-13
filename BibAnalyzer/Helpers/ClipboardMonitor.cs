using BibAnalyzer.Eventos;
using System.Runtime.InteropServices;

namespace BibAnalyzer
{
    public sealed partial class ClipboardMonitor : IDisposable
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        private readonly MessageWindow window;
        private bool disposed;

        public event EventHandler<ClipboardChangedEventArgs>? ClipboardChanged;

        public ClipboardMonitor()
        {
            window = new MessageWindow(this);
            if (!AddClipboardFormatListener(window.Handle))
            {
                Console.Error.WriteLine("Falha ao registrar o listener da área de transferência.");
            }
        }

        private void OnClipboardUpdate()
        {
            /* Lê o texto da área de transferência em uma thread STA
             * O modelo STA (single-threaded apartment) permite usar threads para otimizar o processamento,
             * mas, ao mesmo tempo, garante que uma única thread acesse certos recursos, tornando desnecessário
             * o bloqueio da região crítica
             */

            string text = ReadClipboardTextSta();
            /* Dispara o evento informando que a área de transferência sofreu alteração */
            ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(text));
        }

        private static string ReadClipboardTextSta()
        {
            string result = null;
            Thread thread = new(() =>
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        result = Clipboard.GetText();
                    }
                }
                catch
                {
                    // A área de transferência não está acessível, podemos ignorar
                }
            });
            /* Define que a thread será executada no modelo STA (single-threaded apartment) */
            thread.SetApartmentState(ApartmentState.STA);
            /* Inicia a thread */
            thread.Start();
            /* Aguarda a conclusão da thread com um tempo limite */
            thread.Join(1000);
            return result;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            try
            {
                if (window?.Handle != IntPtr.Zero)
                {
                    RemoveClipboardFormatListener(window.Handle);
                }
            }
            catch { }

            window?.Dispose();
            /* Previne que o finalizador da classe seja chamado desnecessariamente*/
            GC.SuppressFinalize(this);
        }

        ~ClipboardMonitor() => Dispose();
    }
}
