    using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Commons
{
    public class ExcelLib : IDisposable
    {
        protected static int propertyRow = 2;
        private bool disposedValue;
        ExcelWorksheet _worksheet;
        public IEnumerable<string> GetSheetNames(FileInfo file)
        {
            using (ExcelPackage package = new ExcelPackage(file))
            {
                return package.Workbook.Worksheets.Select(s => s.Name).ToList();
            }
        }
        public void DisableValidation(FileInfo file)
        {
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    DisableValidation(package);
                    package.Save();
                }
            }
            catch (System.Exception ex)
            {

            }
        }
        public void DisableValidation(ExcelPackage package)
        {
            try
            {

                foreach (var ws in package.Workbook.Worksheets)
                {
                    ws.DataValidations.InternalValidationEnabled = false;
                }

            }
            catch (System.Exception ex)
            {

            }
        }
        public IEnumerable<T> ReadExcelFileSingleSheet<T>(Stream fileStream, string sheetName = null)
        {
            try
            {
                bool isHasError = false;
                var lstProperties = typeof(T).GetProperties();
                List<string> lstPropertiesName = lstProperties.Select(s => s.Name).ToList();
                List<T> result = new List<T>();
                using (ExcelPackage package = new ExcelPackage(fileStream))
                {
                    List<ExcelRange> cellErrors = new List<ExcelRange>();
                    if (!string.IsNullOrEmpty(sheetName))
                        _worksheet = package.Workbook.Worksheets[sheetName];
                    else
                        _worksheet = package.Workbook.Worksheets.First();

                    if (_worksheet == null)
                        throw new Exception("File does not exists sheetName");

                    int totalRows = _worksheet.Dimension.End.Row;
                    int totalColumns = _worksheet.Dimension.End.Column;

                    if (totalRows < 3)
                        throw new Exception("File has no content");

                    Dictionary<int, string> dictType = new Dictionary<int, string>();

                    #region Read Type field
                    for (int icol = 1; icol <= totalColumns; icol++)
                    {
                        var propertyCell = _worksheet.Cells[propertyRow, icol];
                        if (propertyCell != null)
                        {
                            string propertyName = GetCellValue(propertyCell);
                            if (lstPropertiesName.Contains(propertyName))
                                dictType.Add(icol, propertyName);
                        }
                    }
                    #endregion

                    if (!dictType.Any())
                        throw new Exception("File has no field");

                    #region Read Data
                    int dataRow = propertyRow + 1;
                    for (int irow = dataRow; irow <= totalRows; irow++)
                    {
                        T obj = default(T);
                        obj = Activator.CreateInstance<T>();
                        bool isError = false;
                        foreach (var typename in dictType)
                        {
                            var dataCell = _worksheet.Cells[irow, typename.Key];
                            PropertyInfo curr = typeof(T).GetProperty(typename.Value);
                            if (curr == null)
                            {
                                continue;
                            }
                            if (!curr.CanWrite)
                                continue;

                            if (!setPropertyValue(obj, curr, dataCell))
                            {
                                isError = true;
                                cellErrors.Add(dataCell);
                                // break;
                            }
                        }
                        if (!isError)
                            result.Add(obj);
                        else isHasError = true;
                    }
                    #endregion

                    if (isHasError)
                    {
                        DisableValidation(package);
                        FormatTabError();
                        package.Save();
                    }

                }
                if (isHasError)
                    throw new Exception("File has errors");

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> ReadExcelFileSingleSheet<T>(FileInfo file, string sheetName = null)
        {
            try
            {
                bool isHasError = false;
                var lstProperties = typeof(T).GetProperties();
                List<string> lstPropertiesName = lstProperties.Select(s => s.Name).ToList();
                List<T> result = new List<T>();
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    List<ExcelRange> cellErrors = new List<ExcelRange>();
                    if (!string.IsNullOrEmpty(sheetName))
                        _worksheet = package.Workbook.Worksheets[sheetName];
                    else
                        _worksheet = package.Workbook.Worksheets.First();



                    int totalRows = _worksheet.Dimension.End.Row;
                    int totalColumns = _worksheet.Dimension.End.Column;

                    if (totalRows < 3)
                        throw new Exception("File has no content");

                    Dictionary<int, string> dictType = new Dictionary<int, string>();

                    #region Read Type field
                    for (int icol = 1; icol <= totalColumns; icol++)
                    {
                        var propertyCell = _worksheet.Cells[propertyRow, icol];
                        if (propertyCell != null)
                        {
                            string propertyName = GetCellValue(propertyCell);
                            if (lstPropertiesName.Contains(propertyName))
                                dictType.Add(icol, propertyName);
                        }
                    }
                    #endregion

                    if (!dictType.Any())
                        throw new Exception("File has no field");

                    #region Read Data
                    int dataRow = propertyRow + 1;
                    for (int irow = dataRow; irow <= totalRows; irow++)
                    {
                        T obj = default(T);
                        obj = Activator.CreateInstance<T>();
                        bool isError = false;
                        foreach (var typename in dictType)
                        {
                            var dataCell = _worksheet.Cells[irow, typename.Key];
                            PropertyInfo curr = typeof(T).GetProperty(typename.Value);
                            if (curr == null)
                            {
                                continue;
                            }
                            if (!curr.CanWrite)
                                continue;

                            if (!setPropertyValue(obj, curr, dataCell))
                            {
                                isError = true;
                                cellErrors.Add(dataCell);
                                // break;
                            }
                        }
                        if (!isError)
                            result.Add(obj);
                        else isHasError = true;
                    }
                    #endregion

                    if (isHasError)
                    {
                        DisableValidation(package);
                        FormatTabError();
                        package.Save();
                    }

                }
                if (isHasError)
                    throw new Exception("File has errors");

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ExportExcelFile<T>(FileInfo file, IEnumerable<T> data, string sheetName = null)
        {
            var lstProperties = typeof(T).GetProperties();
            List<string> lstPropertiesName = lstProperties.Select(s => s.Name).ToList();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                List<ExcelRange> cellErrors = new List<ExcelRange>();
                if (!string.IsNullOrEmpty(sheetName))
                    _worksheet = package.Workbook.Worksheets[sheetName];
                else
                    _worksheet = package.Workbook.Worksheets.First();

                int totalRows = _worksheet.Dimension.End.Row;
                int totalColumns = _worksheet.Dimension.End.Column;

                if (totalRows < 2)
                    throw new Exception("File has no wrong template");

                Dictionary<int, string> dictType = new Dictionary<int, string>();

                #region Read Type field
                for (int icol = 1; icol <= totalColumns; icol++)
                {
                    var propertyCell = _worksheet.Cells[propertyRow, icol];
                    if (propertyCell != null)
                    {
                        string propertyName = GetCellValue(propertyCell);
                        if (lstPropertiesName.Contains(propertyName))
                            dictType.Add(icol, propertyName);
                    }
                }
                #endregion

                if (!dictType.Any())
                    throw new Exception("File has no field");

                #region Add Data
                int curr_row = propertyRow + 1;
                foreach (var item in data)
                {
                    foreach (var col in dictType)
                    {
                        var dataCell = _worksheet.Cells[curr_row, col.Key];
                        var data_val = typeof(T).GetProperty(col.Value).GetValue(item, null);
                        if (data_val != null)
                            dataCell.Value = data_val.ToString();
                    }
                    curr_row++;
                }
                #endregion

                package.Save();
            }
            return true;
        }
        private bool setPropertyValue<T>(T obj, PropertyInfo itemProperty, ExcelRange cell)
        {
            try
            {
                string strVal = GetCellValue(cell);
                bool hasRequired = Attribute.IsDefined(itemProperty, typeof(RequiredAttribute));
                if (string.IsNullOrEmpty(strVal) && hasRequired)
                {
                    var required = (RequiredAttribute)Attribute.GetCustomAttribute(itemProperty, typeof(RequiredAttribute));
                    if (required.AllowEmptyStrings == false)
                    {
                        var headerCell = _worksheet.Cells[1, cell.Start.Column].Value.ToString().Trim();
                        CellErrorComment(cell, $"{headerCell} is Required");
                        return false;
                    }
                }

                System.TypeCode typeCode = Type.GetTypeCode(itemProperty.PropertyType);
                if (Nullable.GetUnderlyingType(itemProperty.PropertyType) != null)
                {
                    typeCode = Type.GetTypeCode(itemProperty.PropertyType.GetGenericArguments()[0]);
                }

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        setBooleanValue(obj, itemProperty, strVal);
                        break;
                    case TypeCode.String:
                        itemProperty.SetValue(obj, strVal);
                        break;
                    case TypeCode.Decimal:
                        setDecimalValue(obj, itemProperty, strVal);
                        break;
                    case TypeCode.Double:
                        setDoubleValue(obj, itemProperty, strVal);
                        break;
                    case TypeCode.Int32:
                        setInt32Value(obj, itemProperty, strVal);
                        break;
                    case TypeCode.Int64:
                        setInt64Value(obj, itemProperty, strVal);
                        break;
                    case TypeCode.DateTime:
                        setDateTimeValue(obj, itemProperty, strVal);
                        break;
                    case TypeCode.Object:

                        break;

                }
                ICollection<ValidationResult> validationResults = new List<ValidationResult>();
                bool isValid = CommonFuncMain.ValidateModelProperty(obj, itemProperty, out validationResults);
                if (!isValid)
                {
                    CellErrorComment(cell, string.Join(";", validationResults.Select(s => s.ErrorMessage)));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                CellErrorComment(cell, ex.Message);
                return false;
            }
        }

        private string GetCellValue(ExcelRange cell)
        {
            string result = string.Empty;
            if (cell != null && cell.Value != null)
                result = cell.Value.ToString().Trim();

            return result;
        }
        private void setBooleanValue<T>(T obj, PropertyInfo itemProperty, string parseString)
        {
            bool val = false;
            var isParsed = Boolean.TryParse(parseString, out val);

            if (isParsed)
                itemProperty.SetValue(obj, val);
            else
                itemProperty.SetValue(obj, !string.IsNullOrEmpty(parseString));

        }

        private void setDecimalValue<T>(T obj, PropertyInfo itemProperty, string parseString)
        {
            decimal val = 0;
            var isParsed = Decimal.TryParse(parseString, out val);

            if (itemProperty.PropertyType.IsGenericType && itemProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (isParsed)
                    itemProperty.SetValue(obj, val);
                else
                    itemProperty.SetValue(obj, null);
            }
            else
            {
                itemProperty.SetValue(obj, val);
            }
        }
        private void setDoubleValue<T>(T obj, PropertyInfo itemProperty, string parseString)
        {
            double val = 0;
            var isParsed = double.TryParse(parseString, out val);

            if (itemProperty.PropertyType.IsGenericType && itemProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (isParsed)
                    itemProperty.SetValue(obj, val);
                else
                    itemProperty.SetValue(obj, null);
            }
            else
            {
                itemProperty.SetValue(obj, val);
            }
        }
        private void setInt32Value<T>(T obj, PropertyInfo itemProperty, string parseString)
        {
            int val = 0;
            var isParsed = int.TryParse(parseString, out val);

            if (itemProperty.PropertyType.IsGenericType && itemProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (isParsed)
                    itemProperty.SetValue(obj, val);
                else
                    itemProperty.SetValue(obj, null);
            }
            else
            {
                itemProperty.SetValue(obj, val);
            }
        }
        private void setInt64Value<T>(T obj, PropertyInfo itemProperty, string parseString)
        {
            long val = 0;
            var isParsed = long.TryParse(parseString, out val);

            if (itemProperty.PropertyType.IsGenericType && itemProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (isParsed)
                    itemProperty.SetValue(obj, val);
                else
                    itemProperty.SetValue(obj, null);
            }
            else
            {
                itemProperty.SetValue(obj, val);
            }
        }
        private void setDateTimeValue<T>(T obj, PropertyInfo itemProperty, string parseString)
        {
            DateTime val = new DateTime();
            bool isParsed = false;
            if (parseString.Contains("/") || parseString.Contains("-"))
            {
                isParsed = DateTime.TryParse(parseString, out val);
                if (!isParsed)
                {
                    System.Globalization.CultureInfo parseCulture = System.Globalization.CultureInfo.CreateSpecificCulture("it-IT");
                    isParsed = DateTime.TryParse(parseString, parseCulture, System.Globalization.DateTimeStyles.None, out val);
                }
            }
            else
            {
                double d = 0;
                bool isParsedDouble = Double.TryParse(parseString, out d);
                if (isParsedDouble)
                {
                    val = DateTime.FromOADate(d);
                    isParsed = true;
                }
            }

            if (itemProperty.PropertyType.IsGenericType && itemProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (isParsed)
                    itemProperty.SetValue(obj, val);
                else
                    itemProperty.SetValue(obj, null);
            }
            else
            {
                itemProperty.SetValue(obj, val);
            }
        }
        private void CellErrorComment(ExcelRange cell, string comment)
        {
            if (cell.Comment != null)
                cell.Comment.Text += "\r\n" + comment;
            else
                cell.AddComment(comment, "system");

            FormatCellError(cell);
        }
        private void FormatCellError(ExcelRange cell)
        {
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffc7ce"));
            cell.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#be0006"));

        }
        private void FormatTabError()
        {
            _worksheet.TabColor = ColorTranslator.FromHtml("#be0006");

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ExcelLib()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
