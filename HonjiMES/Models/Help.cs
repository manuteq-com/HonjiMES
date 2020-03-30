using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class DBHelper
    {
        //用一個簡單的Reflection去把資料放入
        internal static T DataReaderMapping<T>(IDataReader reader) where T : new()
        {
            var datetype = typeof(T);
            T _person = new T();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                //取得 DB 欄位名稱
                string propName = reader.GetName(i);
                //取得 DB 值
                var dbvalue = reader.GetValue(i);

                foreach (var Props in typeof(T).GetProperties())
                {
                    if (Props.GetCustomAttribute<ColumnAttribute>()?.Name.ToLower() == propName.ToLower())
                    {
                        Props.SetValue(_person, dbvalue);
                    }
                }
            }
            return _person;
        }

        //寫一個簡單的方式來過濾DataReader的資料
        private static object ReturnValue(object data)
        {
            string typename = data.GetType().Name.ToLower();
            if (typename.Contains("string"))
            {
                return data.ToString();
            }
            if (typename.Contains("int"))
            {
                return int.Parse(data.ToString());
            }
            if (typename.Contains("datetime"))
            {
                DateTime dateTimeValue = new DateTime();
                DateTime.TryParse(data.ToString(), out dateTimeValue);
                return dateTimeValue;
            }
            if (typename.Contains("decimal"))
            {
                return decimal.Parse(data.ToString());
            }
            return null;
        }

    }
}
