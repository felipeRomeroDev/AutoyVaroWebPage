using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoyVaro.Helpers.EnumHelper
{
    public static class EnumHelper
    {
        public static SelectList GetSelectedItemList<T>() where T : struct
        {
            T t = default(T);

            if (!t.GetType().IsEnum) { throw new ArgumentNullException("Por favor asegurese de que T es un Enum"); }

            var nameList = t.GetType().GetEnumNames();

            int counter = 1;
            Dictionary<int, String> myDictionary = new Dictionary<int,string>();
            if(nameList != null && nameList.Length > 0)
            {
                foreach(var name in nameList)
                {

                    T newEnum = (T)Enum.Parse(t.GetType(), name);
                    string descripcion = getDescripcionFromValue(newEnum as Enum);

                    if(!myDictionary.ContainsKey(counter))
                    {
                        myDictionary.Add(counter, descripcion);
                    }
                    counter++;
                }
                counter = 0;
            }

            return new SelectList(myDictionary,"key", "value");
        }

        public static string getDescripcionFromValue(Enum value)
        {
            DescriptionAttribute descriptionAttribute = 
            value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;

            return descriptionAttribute == null ? value.ToString() : descriptionAttribute.Description;

        }
    }
}