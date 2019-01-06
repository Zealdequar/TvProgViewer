﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace TVProgViewer.TVProgApp.Classes
{
    public class ExcelExport
    {
        private const string E_TEMPLATE = "Не удается распознать тип {0} для шаблона.";
        private const string MARKER_DS = "ds";
        private const string MARKER_PARAMETER = "param";
        private const int MAX_ROWS = 65000;

        private static readonly string[] MONTHS = new[]
                                                      {
                                                          "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль",
                                                          "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь", ""
                                                      };

        private static readonly string[] MONTHS_FROM = new[]
                                                           {
                                                               "Января", "Февраля", "Марта", "Апреля", "Мая", "Июня",
                                                               "Июля",
                                                               "Августа", "Сентября", "Октября", "Ноября", "Декабря", ""
                                                           };
        private static readonly Regex REGEXdaTE = new Regex(@"(?<YEAR>\d{4})-(?<MON>\d{2})-(?<DAY>\d{2})");
        private static readonly Regex REGEX_MARKER = 
            new Regex(@"\${(?<type>\w+);(?<name>[^\;\}]+)(\;(?<format>[^\}]+))?}");

        private readonly DataSet _dset;
        private readonly ExcelFile _Excel;
        private CultureInfo _Culture;
        private Dictionary<int, string> _rnames = new Dictionary<int, string>();
        private SqlParameterCollection _SqlParameters;
        private string _xlstemplate;


        /// <summary>
        /// Конструктор экспорта 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="xlstemplate"></param>
        public ExcelExport(DataSet ds, string xlstemplate)
        {
            _dset = ds;
            _xlstemplate = xlstemplate;
            _Excel = new XlsFile();
            _Excel.Open(xlstemplate);
            _rnames = new Dictionary<int, string>();
        }

        /// <summary>
        /// Обработчик события нахождения и замены шаблонного значения.
        /// </summary>
        /// <param name="match">Найденный шаблон.</param>
        /// <param name="rtype">Тип.</param>
        /// <returns>Результат замены</returns>
        private object ReplaceHandler (Match match, out Type rtype)
        {
            string type = match.Result("${type}");
            string name = match.Result("${name}");

            rtype = typeof (DBNull);
            object result = null;
            // Различаем два типа шаблона:
            // - XML-шаблон, значение которого задает XPath функцию;
            // - Параметр SQL-запроса, значение которого задает имя параметра.
            switch (type)
            {
                case MARKER_DS:
                    string[] vals = name.Split('/');
                    if (vals.Length == 3)
                    {
                        // значение начинается с кавычки - запрос
                        if (vals[1].StartsWith("\'"))
                        {
                            vals[1] = vals[1].Trim('\'');
                            int tid = Convert.ToInt32(vals[0]);
                            if (tid < _dset.Tables.Count)
                            {
                                if (_dset.Tables[tid].Select(vals[1]).Length > 0)
                                {
                                    if (_dset.Tables[tid].Columns.Contains(vals[2]))
                                    {
                                        result = _dset.Tables[tid].Select(vals[1])[0][vals[2]];
                                        rtype = result.GetType();
                                    }
                                }
                            }
                        }
                        if (vals[1].StartsWith("["))
                        {
                            vals[1] = vals[1].Trim('[', ']');
                            int tid = Convert.ToInt32(vals[0]);
                            int rid = Convert.ToInt32(vals[1]);
                            if (tid < _dset.Tables.Count)
                            {
                                if (_dset.Tables[tid].Columns.Contains(vals[2]))
                                {
                                    if (_dset.Tables[tid].Rows.Count > rid)
                                    {
                                        result = _dset.Tables[tid].Rows[rid][vals[2]];
                                        rtype = result.GetType();
                                    }
                                }
                                

                            }
                        }
                    }
                    break;
                case MARKER_PARAMETER:
                    DbParameter parameter = _SqlParameters[name];
                    if (parameter != null)
                    {
                        result = parameter.Value;
                        rtype = result.GetType();
                    }
                    break;
            }

            if (result != null)
            {
                if (result.ToString() == Boolean.TrueString) result = "Да";
                if (result.ToString() == Boolean.FalseString) result = "Нет";
            }
            else result = "-";

            // Форматировать значение, в случае если задано правило форматирования.
            string format = match.Result("${format}");
            if (!String.IsNullOrEmpty(format))
            {
                format = string.Concat("{0:", format, "}");
                return string.Format(_Culture, format, result);
            }
            return result;
        }

        /// <summary>
        /// Замена шаблонного значения.
        /// </summary>
        /// <param name="value">Заменяемое шаблонное значение.</param>
        /// <returns>Результат замены</returns>
        private object ReplaceValue(string value)
        {
            Type rtp = typeof (DBNull);
            object rep = ReplaceHandler(REGEX_MARKER.Match(value), out rtp);

            if (rep != null)
            {
                if (rtp == typeof(Int32)) return Convert.ToInt32(rep);
                if (rtp == typeof(Decimal)) return Convert.ToDecimal(rep);
                if (rtp == typeof(Double)) return Convert.ToDouble(rep);

                /*if (rtp == typeof (DateTime))
                {
                    var rval = (DateTime) rep;
                    return rval.ToShortDateString() + " " + rval.ToLongTimeString();
                }*/
            }
            return rep;
        }

        /// <summary>
        /// Формирование выходного HTTP-потока из книги Excel.
        /// </summary>
        /// <param name="zip">Сжимать?</param>
        /// <returns></returns>
        private MemoryStream RenderWorkbook(bool zip)
        {
            // Получить содержание отчета в виде бинарного файла.
            byte[] buffer = null;
            var stream = new MemoryStream();
            try
            {
                _Excel.Save(stream);
                buffer = stream.ToArray();
            }
            finally
            {
                stream.Close();
            }

            if(!zip)
            {
                return stream;
            }
            else
            {
                ICSharpCode.SharpZipLib.Zip.ZipOutputStream zw = new ZipOutputStream(stream);
                using (MemoryStream ms = new MemoryStream())
                {
                    int bytes = 0;
                    byte[] temp = new byte[4096];
                    while ((bytes = zw.Read(temp, 0, temp.Length)) != 0)
                    {
                        ms.Write(temp, 0, bytes);
                    }
                    return ms;
                }
            } 
        }

        /// <summary>
        /// Замена всех шаблонных значений в заданном диапазоне.
        /// </summary>
        /// <param name="range">Диапазон, в котором производится замена.</param>
        private void ReplaceRange (TXlsCellRange range)
        {
            for (int column = range.Left; column < range.Left + range.ColCount; column++)
            {
               for (int row = range.Top; row < range.Top + range.RowCount; row++)
               {
                   object value = _Excel.GetCellValue(row, column);
                   string strValue = null;
                   object result = value;

                   // Если ячейка пуста или содержит формулу, то игнорируем её.
                   if (value == null || value.GetType() == typeof(TFormula) ||
                       (strValue = value.ToString()).Length == 0) continue;

                   // Если нет шаблонных значений, то также игнорируем ячейку.
                   MatchCollection matches = REGEX_MARKER.Matches(strValue);
                   if (matches.Count == 0) continue;

                   // Возможны два варианта замены шаблонных значений в ячейке. В первом случае,
                   // когда шаблон занимает всю ячейку, мы размещаем там непосредственно объект.
                   // Т. е. в случае числового значения там будет размещено число. Во втором случае
                   // присутствует дополнительный текст или другие шаблоны, а также может быть задан
                   // формат отображения. Тогда мы рассматриваем ячейку как текстовую и осушествляем
                   // текстовую же замену в ячейке.

                   Match match = null;
                   string type = null, name = null, format = null;
                   if (matches.Count == 1 && (match = matches[0]).Length == strValue.Length && 
                       (format = match.Result("${format}")).Length == 0)
                   {
                       type = match.Result("${type}");
                       name = match.Result("${name}");

                       // Различаем два типа шаблона:
                       // - Датасет
                       // - Параметр SQL-запроса, значение которого задает имя параметра.
                       switch (type.ToLower())
                       {
                           case MARKER_DS:
                               result = ReplaceValue(strValue);
                               break;
                           case MARKER_PARAMETER:
                               DbParameter parameter = _SqlParameters[name];
                               result = parameter != null ? parameter.Value : null;
                               break;
                           default:
                               throw new Exception(string.Format(E_TEMPLATE, type));
                       }
                   }
                   else
                   {
                       result = ReplaceValue(strValue);
                   }

                   // Для нулевых числовых значений установить прочерк.
                   if (result.GetType() == typeof(Double) && (double)result == 0F) result = "-";

                   // Установить новое значение ячейки.
                   _Excel.SetCellValue(row, column, result);
               }
            }
        }


        /// <summary>
        /// Формирование книги Excel.
        /// Автоматическое расширение диапазонов для именованных диапазонов с именами
        /// AEXR_№таблицы_ИМЯ ДИАПАЗОНА - авторасширение рядов
        /// если таблица расширяется за пределы максимального  количества рядов, то
        /// лист на котором находится диапазон копируется и вставляется, а диапазон замены продолжается
        /// на новом листе со следующей позиции
        /// Процедура учитывает все "расширенные" диапазоны, чтобы избежать бесконечного цикла добавления 
        /// (страницы копируются с сохраненим всех диапазонов)
        /// AMRG_ИМЯ ДИАПАЗОНА - автообъединение рядов с одинаковыми значениями.
        /// </summary>
        /// <returns></returns>
        private bool Build()
        {
            var expranges = new List<string>();
            // Осуществить замену шаблонных значений по всем страницам и именованным диапазонам.
            for (int sheet = 1; sheet <= _Excel.SheetCount; sheet++)
            {
                _rnames = new Dictionary<int, string>();

                int range = 1;
                // запомнить текущий лист
                int cur_sheet = sheet;
                TXlsNamedRange r;
                _Excel.ActiveSheet = sheet;

                // Записать имена всех диапазонов:
                for (range = 1; range <= _Excel.NamedRangeCount; range++)
                {
                    _rnames.Add(range, _Excel.GetNamedRange(range).Name);
                }

                // Обработать диапазоны:
                foreach (var pr in _rnames)
                {
                    // добавить авторасширяемые диапазоны
                    if (pr.Value.StartsWith("AEXR_"))
                    {
                        if (ProcessAutoExpand(sheet, pr.Key, pr.Value) > 0)
                        {
                            // если были добавлены страницы
                            sheet = cur_sheet;
                            _Excel.ActiveSheet = sheet;
                        }
                    }
                    else
                    {
                        ReplaceRange(_Excel.GetNamedRange(pr.Key));
                    }
                }

                // объединить ячейки
                foreach (var pr in _rnames)
                {
                    // Объединить значения если значения совпадают
                    if (pr.Value.StartsWith("AMRG_"))
                    {
                        ProcessMerge(pr.Key, pr.Value);
                    }
                }
            }
            

            // Инициализировать начальное состояние книги для открытия.
            _Excel.ActiveSheet = 1;
            return true;
        }

        /// <summary>
        /// Объеднинение ячеек в столбец
        /// </summary>
        /// <param name="key">код диапазона</param>
        /// <param name="value">имя диапазона</param>
        private void ProcessMerge (int key, string value)
        {
            TXlsNamedRange r = _Excel.GetNamedRange(key);
            string[] arr = value.Split('_');
            int tid = Convert.ToInt32(arr[1]);
            int cnt = 0;
            if (_dset.Tables.Count > tid) cnt = _dset.Tables[tid].Rows.Count;

            for (int col = r.Left; col <= r.Right; col++)
            {
                int rtop = r.Top;
                int row = r.Top;
                // в цикле до 2-ух разрывов
                while (row < r.Top + cnt)
                {
                    string val = (_Excel.GetCellValue(row, col) != null ? _Excel.GetCellValue(row, col).ToString() : "");
                    // в цикле пробежать по следующим рядам до первого неравного значения
                    while ((_Excel.GetCellValue(row, col) != null ? _Excel.GetCellValue(row, col).ToString() : "") ==
                       val 
                       && row < r.Top + cnt )
                    {
                        row++;
                    }
                    _Excel.MergeCells(rtop, col, row - 1, col);

                    rtop = row;
                }
            }
        }

        /// <summary>
        /// Обработать авторасширение по вертикали
        /// </summary>
        /// <param name="sheet">номер листа</param>
        /// <param name="rid">номер диапазона</param>
        /// <param name="namefull">название дапазона</param>
        /// <returns>число добавленных страниц</returns>
        private int ProcessAutoExpand(int sheet, int rid, string namefull)
        {
            TXlsNamedRange r = _Excel.GetNamedRange(rid);
            string[] arr = namefull.Split('_');

            int tid = Convert.ToInt32(arr[1]);
            int cnt = 0;
            if (_dset.Tables.Count > tid) cnt = _dset.Tables[tid].Rows.Count;
            int rest = 0;
            int newpages = 0;

            if ((cnt) > MAX_ROWS)
            {
                newpages = (int) Math.Floor((double) (cnt/MAX_ROWS)) + 1;
                rest = newpages*MAX_ROWS;
            }

            if (newpages > 0)
            {
                // диапазон превысил 1

                #region multi_page

                // Добавить новые листы
                for (int sh = 1; sh< newpages; sh++)
                {
                    // количество строк превысило 65000 - создать новые листы:
                    _Excel.InsertAndCopySheets(1, _Excel.SheetCount + 1, 1);
                    _Excel.ActiveSheet = sh + 1;
                    _Excel.SheetName = _Excel.GetSheetName(sheet) + "(" + (sh + 1) + ")";
                }
                // Создать диапазоны:
                for (int sh = 1; sh <= newpages; sh++)
                {
                    _Excel.ActiveSheet = sh;
                    // включена разбивка

                    if(sh > 1)
                    {
                        // для всех листов после текущего необходимо изменить исходный диапазон
                        for (int nrc = r.Left; nrc <= r.Right; nrc++)
                        {
                            object cval = _Excel.GetCellValue(r.Top, nrc);
                            int xfn = _Excel.GetCellFormat(r.Top,nrc);

                            if (cval != null)
                            {
                                cval = cval.ToString().Replace("[0]", "[" + (((sh - 1)*MAX_ROWS)) + "]");
                                _Excel.SetCellValue(r.Top, nrc, cval, xfn);
                            }
                        }
                    }
                    int maxrows = r.Top + MAX_ROWS;
                    if (sh == newpages) maxrows = r.Top + MAX_ROWS - rest;
                    // Заменить все остальные данных
                    for (int nrr = r.Top + 1; nrr < maxrows; nrr++)
                    {
                        // Скопировать диапазон - строку
                        _Excel.InsertAndCopyRange(new TXlsCellRange(r.Top, r.Left, r.Top, r.Right),
                                                   nrr, r.Left, 1, TFlxInsertMode.ShiftRangeDown);
                        // Вставить нужные подстановочные выражения:
                        for (int nrc = r.Left; nrc <= r.Right; nrc ++)
                        {
                            object cval = _Excel.GetCellValue(r.Top, nrc);
                            int xfn = _Excel.GetCellFormat(r.Top, nrc);

                            if (cval != null)
                            {
                                cval = cval.ToString().Replace("[" + (((sh - 1)*MAX_ROWS)) + "]",
                                                               "[" + (nrr - r.Top + ((sh - 1)*MAX_ROWS)) + "]");
                                _Excel.SetCellValue(nrr, nrc, cval, xfn);
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                // Диапазон влез на 1 страницу:
                #region one_page

                _Excel.SetNamedRange(new TXlsNamedRange(r.Name, r.NameSheetIndex, r.SheetIndex, r.Top,
                                                        r.Left,
                                                        r.Top + cnt, r.Right, r.OptionFlags,
                                                        r.RangeFormula));
                // цикл расширения диапазона
                for (int nrr = r.Top + 1; nrr < r.Top + cnt; nrr++)
                {
                    //скопировать диапазон - строку
                    _Excel.InsertAndCopyRange(new TXlsCellRange(r.Top, r.Left, r.Top, r.Right), 
                        nrr, r.Left, 1, TFlxInsertMode.ShiftRowDown);
                    // Вставить нужные подстановочные выражения:
                    for (int nrc = r.Left; nrc <= r.Right; nrc++)
                    {
                        object cval = _Excel.GetCellValue(r.Top, nrc);
                        int xfn = _Excel.GetCellFormat(r.Top, nrc);

                        if (cval != null)
                        {
                            cval = cval.ToString().Replace("[0]", "[" + (nrr - r.Top) + "]");
                            _Excel.SetCellValue(nrr, nrc, cval);
                        }
                    }
                }

                #endregion
            }
            // Перейти на текущий лист:
            TXlsCellRange rng = _Excel.GetNamedRange(rid);
            rng.Bottom += cnt;
            ReplaceRange(rng);
            return newpages;
        }


        public MemoryStream BuildWorkBook()
        {
            var ms = new MemoryStream();
            if (_Excel != null)
                if (_dset != null)
                    if (Build())
                        ms = RenderWorkbook(false);
            return ms;
        }
    }
}
