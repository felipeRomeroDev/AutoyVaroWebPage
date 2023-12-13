using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoyVaro.Enums;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace AutoyVaro.Extenciones
{
    public static class SistemExtensions
    {
        public static string EstadoFisico(this int valor)
        {
            return valor == 0 ? "RE" : "ME";
        }

        public static IHtmlString DisplayEnumFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, int>> ex, Type enumType)
        {
            var value = (int)ModelMetadata.FromLambdaExpression(ex, html.ViewData).Model;
            string enumValue = Enum.GetName(enumType, value);
            return new HtmlString(html.Encode(enumValue));
        }

        public static List<SelectListItem> ToSelectList<T>(this List<T> Items, Func<T, string> getKey, Func<T, string> getValue, string selectedValue, string noSelection, bool search = false)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (search)
            {
                items.Add(new SelectListItem { Selected = true, Value = "-1", Text = string.Format("-- {0} --", noSelection) });
            }

            foreach (var item in Items)
            {
                items.Add(new SelectListItem
                {
                    Text = getKey(item),
                    Value = getValue(item),
                    Selected = selectedValue == getValue(item) ? true : false
                });
            }

            return items
                .OrderBy(l => l.Text)
                .ToList();
        }

        public static int GetWeekNumber(this DateTime date)
        {
            const int JAN = 1;
            const int DEC = 12;
            const int LASTDAYOFDEC = 31;
            const int FIRSTDAYOFJANUARY = 1;
            const int THURSDAY = 4;
            bool thursdayFlag = false;

            int dayOfTheYear = date.DayOfYear;
            int startWeekDayOfYear = (int)(new DateTime(date.Year, JAN, FIRSTDAYOFJANUARY)).DayOfWeek;
            int endWeekDayOfYear = (int)(new DateTime(date.Year, DEC, LASTDAYOFDEC)).DayOfWeek;

            if(startWeekDayOfYear == 0)            
                startWeekDayOfYear = 7;
            if (endWeekDayOfYear == 0)
                endWeekDayOfYear = 0;

            int daysInFirstWeek = 8 - (startWeekDayOfYear);

            if (startWeekDayOfYear == THURSDAY || endWeekDayOfYear == THURSDAY)
                thursdayFlag = true;

            int fullWeek = (int)Math.Ceiling((dayOfTheYear - (daysInFirstWeek)) / 7.0);
            int resultWeekNumber = fullWeek;

            if (daysInFirstWeek >= THURSDAY)
                resultWeekNumber = resultWeekNumber + 1;

            if (resultWeekNumber > 52 && !thursdayFlag)
                resultWeekNumber = 1;

            if (resultWeekNumber == 0)
                resultWeekNumber = GetWeekNumber(new DateTime(date.Year - 1, DEC, LASTDAYOFDEC));
            return resultWeekNumber;
        }

        public static DateTime GetFirstDateOfWeek(this DateTime date)
        {
            if (date == DateTime.MinValue)
                return date;

            int week = date.GetWeekNumber();
            while (week == date.GetWeekNumber())
                date = date.AddDays(-1);
            return date.AddDays(1);
        }

        public static DateTime GetLastDateOfWeek(this DateTime date)
        {
            if (date == DateTime.MaxValue)
                return date;

            int week = date.GetWeekNumber();
            while (week == date.GetWeekNumber())
                date = date.AddDays(1);
            return date.AddDays(-1);
        }

        public static int GetWeekNumberOfMonth(this DateTime date)
        {
            int CurrentMonth = date.Month;
            int countWeek = 0;           
            int week = date.GetWeekNumber();
            while (CurrentMonth == date.Month)
            {
                while (week == date.GetWeekNumber())
                {
                    date = date.AddDays(-1);
                }
                week = date.GetWeekNumber();
                countWeek++;
            }            
            return countWeek;
        }

        public static DateTime GetFechaMasDias(this DateTime date, int dias)
        {   
            return  date.AddDays(dias);
        }
    }
}