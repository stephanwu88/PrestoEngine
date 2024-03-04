using System;
using System.Collections.Generic;

namespace Engine.Automation.Sparker
{
    public class sMaterial
    {
        private Dictionary<string, sMaterialRow> _BaseData = new Dictionary<string, sMaterialRow>();
        private string _InstrumentKey = "";
        private string _MaterialName = "";
        private string _MatKey = "";
        private string _ProgKey = "";

        public sMaterial(string materialName, string insKey, string matKey, string progKey)
        {
            this._MaterialName = materialName;
            this._InstrumentKey = insKey;
            this._MatKey = matKey;
            this._ProgKey = progKey;
            //DbConnection connection = sMain.Database.Connection;
            //DbCommand command = sMain.Database.Connection.CreateCommand();
            //DbCommand command1 = connection.CreateCommand();
            //command1.CommandText = " select * from material_elem  where main_id in(select index_id from material_main where material_name='" + materialName + "') ";
            //DbDataReader reader = command1.ExecuteReader();
            //while (reader.Read())
            //{
            //    sMaterialRow row = new sMaterialRow
            //    {
            //        ElemName = reader["elem_name"].ToString(),
            //        Method = reader["method"].ToString(),
            //        LimitMax = this.ToNumber(reader["limit_max"]),
            //        LimitMin = this.ToNumber(reader["limit_min"])
            //    };
            //    if (row.LimitMax <= 0f)
            //    {
            //        row.LimitMax = -1f;
            //    }
            //    if (row.LimitMin <= 0f)
            //    {
            //        row.LimitMin = -1f;
            //    }
            //    string[] textArray1 = new string[] { " select * from type_stand_sample_elem  where main_id = ", reader["t_sample_1_id"].ToString(), " and  instrument_key='", insKey, "' and  mat_key='", matKey, "' and  prog_key='", progKey, "' and elem_name ='", row.ElemName, "'" };
            //    command.CommandText = string.Concat(textArray1);
            //    DbDataReader reader2 = command.ExecuteReader();
            //    if (reader2.Read())
            //    {
            //        row.ActualValue1 = this.ToNumber(reader2["actual_value"]);
            //        row.SetValue1 = this.ToNumber(reader2["set_value"]);
            //        row.AdditiveValue = this.ToNumber(reader2["additive_value"]);
            //        row.MultiValue = this.ToNumber(reader2["multi_value"]);
            //    }
            //    reader2.Close();
            //    if ((reader["t_sample_2_id"] != null) && (reader["t_sample_2_id"].ToString() != ""))
            //    {
            //        string[] textArray2 = new string[] { " select * from type_stand_sample_elem  where main_id = ", reader["t_sample_2_id"].ToString(), " and  instrument_key='", insKey, "' and  mat_key='", matKey, "' and  prog_key='", progKey, "' and elem_name ='", row.ElemName, "'" };
            //        command.CommandText = string.Concat(textArray2);
            //        reader2 = command.ExecuteReader();
            //        if (reader2.Read())
            //        {
            //            row.IsDouble = true;
            //            row.ActualValue2 = this.ToNumber(reader2["actual_value"]);
            //            row.SetValue2 = this.ToNumber(reader2["set_value"]);
            //            sAnaTolerance.BaseValue(row.SetValue1, row.ActualValue1, row.SetValue2, row.ActualValue2, out row.AdditiveValue, out row.MultiValue);
            //        }
            //        reader2.Close();
            //    }
            //    this._BaseData.Add(row.ElemName, row);
            //}
            //reader.Close();
            //connection.Close();
        }

        public float Actual(string elemName, float actualValue)
        {
            sMaterialRow row;
            if (!this._BaseData.TryGetValue(elemName, out row))
            {
                return actualValue;
            }
            if (row.IsDouble)
            {
                actualValue = sAnaTolerance.ActualValue(actualValue, row.AdditiveValue, row.MultiValue);
            }
            else if (row.Method == "MULTI")
            {
                actualValue = sAnaTolerance.ActualValue(actualValue, row.MultiValue, row.Method);
            }
            else
            {
                actualValue = sAnaTolerance.ActualValue(actualValue, row.AdditiveValue, row.Method);
            }
            if ((row.LimitMax > 0f) && (actualValue > row.LimitMax))
            {
                actualValue = row.LimitMax;
            }
            else if ((row.LimitMin > 0f) && (actualValue < row.LimitMin))
            {
                actualValue = row.LimitMin;
            }
            return (float)Math.Round((double)actualValue, 5);
        }
    }

    public class sMaterialRow
    {
        public float ActualValue1;
        public float ActualValue2;
        public float AdditiveValue;
        public string ElemName = "";
        public bool IsDouble;
        public float LimitMax;
        public float LimitMin;
        public string Method = "";
        public float MultiValue;
        public float SetValue1;
        public float SetValue2;
        public string TSampleID1 = "";
        public string TSampleID2 = "";
    }

    public class sAnaTolerance
    {
        public static float ActualValue(float actualValue, float additiveValue, float multiValue)
        {
            return ((multiValue * actualValue) + additiveValue);
        }

        public static float ActualValue(float actualValue, float baseValue, string method)
        {
            if ((method != null) && (method == "MULTI"))
            {
                return (actualValue * baseValue);
            }
            return (actualValue + baseValue);
        }

        public static void BaseValue(float setValue, float actualValue, out float additiveValue, out float multiValue)
        {
            additiveValue = setValue - actualValue;
            multiValue = setValue / actualValue;
        }

        public static void BaseValue(float setValue1, float actualValue1, float setValue2, float actualValue2, out float additiveValue, out float multiValue)
        {
            multiValue = (setValue1 - setValue2) / (actualValue1 - actualValue2);
            additiveValue = setValue1 - (multiValue * actualValue1);
        }
    }
}

