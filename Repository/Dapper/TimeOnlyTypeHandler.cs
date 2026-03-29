using System;
using System.Data;
using Dapper;

namespace Repository.Dapper
{
    public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override TimeOnly Parse(object value)
        {
            return value switch
            {
                TimeSpan ts => TimeOnly.FromTimeSpan(ts),
                TimeOnly t => t,
                DateTime dt => TimeOnly.FromDateTime(dt),
                string s => TimeOnly.Parse(s),
                _ => throw new DataException($"Cannot convert {value.GetType()} to TimeOnly")
            };
        }

        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.DbType = DbType.Time;
            parameter.Value = value.ToTimeSpan();
        }
    }
}
