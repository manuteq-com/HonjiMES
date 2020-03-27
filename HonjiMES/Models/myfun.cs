using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class myfun
    {
        /// <summary>
        /// 要回寫資料庫的資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Olddata">舊資料</param>
        /// <param name="Newdata">新資料</param>
        /// <returns>回傳訊息</returns>
        internal static string MappingData<T>(ref T Olddata, T Newdata)
        {
            var Olddata_Type = Olddata.GetType();
            var Newdata_Type = Olddata.GetType();
            foreach (var New_Props in Newdata_Type.GetProperties())
            {
                var New_Props_Name = New_Props.Name;
                var New_Props_Value = New_Props.GetValue(Newdata);
                foreach (var Old_Props in Olddata_Type.GetProperties())
                {
                    var Old_Props_Name = Old_Props.Name;
                    var Old_Props_Value = Old_Props.GetValue(Olddata);

                    if (New_Props_Name == Old_Props_Name)
                    {
                        if (New_Props_Value != Old_Props_Value)
                        {
                            if (New_Props_Value != null)
                            {
                                if (New_Props.PropertyType == typeof(DateTime))
                                {
                                    if (((DateTime)New_Props_Value).Year != 1)
                                    {
                                        Old_Props.SetValue(Olddata, New_Props_Value);
                                    }
                                }
                                else
                                {
                                    if (true)
                                    {
                                        Old_Props.SetValue(Olddata, New_Props_Value);
                                    }
                                }

                            }
                        }
                    }
                }


            }
            return "";
        }
    }
}
