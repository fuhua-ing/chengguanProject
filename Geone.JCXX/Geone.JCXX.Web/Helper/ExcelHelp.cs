using Geone.Utiliy.Library;
using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace Geone.JCXX.Web
{
    public static class ExcelHelp
    {
        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="startRowIndex">开始读取的行数 0是第一行</param>
        /// <returns></returns>
        public static List<T> ExcelImport<T>(ISheet sheet, int startRowIndex, List<string> attrs)
        {
            var list = new List<T>();
            string jsonStr;
            //从起始行开始遍历
            for (int i = startRowIndex; i <= sheet.LastRowNum; i++)
            {
                jsonStr = "{";
                //从第一列开始遍历
                for (int j = 0; j < attrs.Count; j++)
                {
                    ICell cell = sheet.GetRow(i).GetCell(j);
                    if (cell != null)
                    {
                        var value = GetValueByType(cell);
                        jsonStr += "'" + attrs[j] + "':'" + (value == null ? "" : value.ToString()) + "',";
                    }
                }
                jsonStr = jsonStr.Substring(0, jsonStr.Length - 1) + "}";
                list.Add(JsonHelper.JsonDllDeserialize<T>(jsonStr));
            }
            return list;
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueByType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;

                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;

                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;

                case CellType.String: //STRING:
                    return cell.StringCellValue;

                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;

                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }
    }
}