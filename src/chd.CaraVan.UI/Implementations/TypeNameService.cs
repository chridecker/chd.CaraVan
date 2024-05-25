using chd.CaraVan.Contracts.Attributes;
using chd.CaraVan.Contracts.Dtos.Base;
using chd.CaraVan.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.UI.Implementations
{
    public class TypeNameService : ITypeNameService
    {
        public (string name, string unit) GetName(DataBase data)
        {
            var val = this.GetAttribute(data.Type);
            return (val.Name, val.Unit);
        }
        private DataNameAttribute GetAttribute(EDataType type)
        {
            var enumType = typeof(EDataType);
            var memberInfos = enumType.GetMember(type.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DataNameAttribute), false);
            return (DataNameAttribute)valueAttributes[0];
        }
    }
    public interface ITypeNameService
    {
        (string name, string unit) GetName(DataBase type);
    }
}
