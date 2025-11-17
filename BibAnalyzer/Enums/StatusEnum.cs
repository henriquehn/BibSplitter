using BibLib.Attributes;
using System.ComponentModel;

namespace BibAnalyzer.Enums
{
    public enum StatusEnum
    {
        /// <summary>
        /// Imdica que o programa está ocioso
        /// </summary>
        [Description("Pronto")]
        [StatusColor("ControlText")]
        Done,
        /// <summary>
        /// Indica que o programa está buscando os arquivos BIB na pasta selecionada
        /// </summary>
        [Description("Procurando arquivos")]
        [StatusColor("SteelBlue")]
        SearchingFiles,
        /// <summary>
        /// Indica que o programa está analisando os arquivos encontrados
        /// </summary>
        [Description("Analisando arquivos")]
        [StatusColor("SteelBlue")]
        ParsingFiles,
        /// <summary>
        /// Indica que o programa está buscando as informações de paginação nos arquivos BIB encontrados
        /// </summary>
        [Description("Contando páginas")]
        [StatusColor("SteelBlue")]
        CountingPages,
        /// <summary>
        /// Indica que alguns dos registros mão possuem informação de paginaçlão e está consultando pelo DOI
        /// </summary>
        [Description("Procurando informações de paginação online")]
        [StatusColor("Green")]
        SearchingPageInfoOnline,
        /// <summary>
        /// Indica que alguns arquivos não possuem informação de paginação mesmo pbuscando pelo DOI e está buscando diretamente no PDF
        /// </summary>
        [Description("Procurando informações de paginação no PDF")]
        [StatusColor("Orange")]
        SerchingPageInfoOnPDF,
        /// <summary>
        /// Significa que houve algum erro durante o processamento
        /// </summary>
        [StatusColor("Red")]
        [Description("Erro")]
        Error
    }
}
