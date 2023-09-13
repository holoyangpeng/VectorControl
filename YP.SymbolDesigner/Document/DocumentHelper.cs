using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YP.SymbolDesigner.Document
{
    public class DocumentHelper
    {
        #region ..const fields
        const string ElectricDocumentType = "电气接线图演示文档";
        const string PSASPDocumentType = "PSASP电气接线图演示文档";
        const string MatlabDocumentType = "建模演示";
        const string NormalDocumentType = "SVG文档";
        const string BasicFlowchartDocumentType = "基本流程图演示文档";
        const string ConfigurationDocumentType = "组态演示文档";

        public static String[] DocumentTypes = { NormalDocumentType, PSASPDocumentType, ElectricDocumentType, MatlabDocumentType, BasicFlowchartDocumentType,ConfigurationDocumentType };
        #endregion

        #region ..GetDefaultSymbolFile
        /// <summary>
        /// 根据文档类型，取得默认的图元库
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public static string GetDefaultSymbolFile(string documentType)
        {
            if (documentType == null)
                return string.Empty;
            switch (documentType)
            {
                case ElectricDocumentType:
                    return "electric.symbol"; 
                case MatlabDocumentType:
                    return "matlab.symbol";
                case BasicFlowchartDocumentType:
                    return "basicflowchart.symbol";
                case PSASPDocumentType:
                    return "psasp.symbol";
                case ConfigurationDocumentType:
                    return "configuration.symbol";
            }
            return "basic.symbol";
        }
        #endregion

        #region ..GetFileExtension
        /// <summary>
        /// 取得文档类型对应的文件后缀
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public static string GetFileExtension(string documentType)
        {
            if (documentType == null)
                return ".svg";
            switch (documentType)
            {
                case ElectricDocumentType:
                    return ".eld";
                case MatlabDocumentType:
                    return ".mld";
                case BasicFlowchartDocumentType:
                    return ".bfl";
                case PSASPDocumentType:
                    return ".psasp";
                case ConfigurationDocumentType:
                    return ".cfd";
            }
            return ".svg";
        }
        #endregion

        #region ..GetFileFilter
        /// <summary>
        /// 根据文档类型，获取对应的Filter
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public static string GetFileFilter(string documentType)
        {
            if (documentType == null)
                return string.Empty;
            return string.Format("{0}(*{1})|*{1}", documentType, GetFileExtension(documentType));
        }
        #endregion

        #region ..CreateDocument
        /// <summary>
        /// 根据文件名，创建对应的DocumentControl
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DocumentControl CreateDocument(string documentType,string fileName)
        {
            DocumentControl dc = null;
            switch (documentType)
            {
                case ElectricDocumentType:
                    dc = fileName == null ? new ElectricDocumentControl(): new ElectricDocumentControl(fileName);
                    break;
                case MatlabDocumentType:
                    dc = fileName == null ? new MatlabDocumentControl():new MatlabDocumentControl(fileName);
                    break;
                case PSASPDocumentType:
                    dc = fileName == null ? new PSASPDocumentControl() : new PSASPDocumentControl(fileName);
                    break;
                case ConfigurationDocumentType:
                    dc = fileName == null ? new ConfigurationDocumentControl() : new ConfigurationDocumentControl(fileName);
                    break;
                default:
                    dc = fileName == null ?new DocumentControl():new DocumentControl(fileName);
                    break;
            }
       
            dc.DocumentType = documentType;
            return dc;
        }
        #endregion

        #region ..GetDocumentType
        public static string GetDocumentType(string extension)
        {
            var ext = extension;
            if (ext.StartsWith("."))
                ext = ext.Substring(1);

            switch (ext.ToLower())
            {
                case "eld":
                    return ElectricDocumentType;
                case "mld":
                    return MatlabDocumentType;
                case "bfl":
                    return BasicFlowchartDocumentType;
                case "psasp":
                    return PSASPDocumentType;
                case "cfd":
                    return ConfigurationDocumentType;
                default:
                    return NormalDocumentType;
            }
        }
        #endregion

        #region ..GetAllSupportFileFilter
        /// <summary>
        /// 获取所有支持文件类型对应的Filter
        /// </summary>
        /// <returns></returns>
        public static string GetAllSupportFileFilter()
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (string str in DocumentTypes)
            {
                strBuilder.Append(string.Format("|{0}",GetFileFilter(str)));
            }

            strBuilder.Append("|所有文件(*.*)|*.*");
            return strBuilder.ToString(1, strBuilder.Length - 1);
        }
        #endregion
    }
}
