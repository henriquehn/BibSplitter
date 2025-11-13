namespace BibAnalyzer
{
    public sealed partial class ClipboardMonitor
    {
        /* Janela de mensagem escondida para receber WM_CLIPBOARDUPDATE */
        private sealed class MessageWindow : NativeWindow, IDisposable
        {
            private readonly ClipboardMonitor owner;
            private bool disposed;

            public MessageWindow(ClipboardMonitor owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
                var parameters = new CreateParams()
                {
                    Caption = "ClipboardMonitorWindow_" + Guid.NewGuid().ToString("N")
                    // Parent/Style defaults são suficientes para uma janela de mensagem não visível
                };
                this.CreateHandle(parameters);
            }

            /* Sobrescreve o método WndProc para capturar mensagens do Windows */
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_CLIPBOARDUPDATE)
                {
                    owner.OnClipboardUpdate();
                }
                base.WndProc(ref m);
            }

            public void Dispose()
            {
                if (disposed) return;
                disposed = true;
                try
                {
                    this.DestroyHandle();
                }
                catch { }
            }
        }
    }
}
