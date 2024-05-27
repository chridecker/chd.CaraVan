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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace chd.CaraVan.UI.Implementations
{
    public class TypeNameService : ITypeNameService
    {
        public (string Name, string Unit) GetName(DataBase data) => this.GetName(data.Type);

        public (string Name, string Unit) GetName(EDataType type)
        {
            var val = this.GetAttribute(type);
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
        (string Name, string Unit) GetName(DataBase data);
        (string Name, string Unit) GetName(EDataType type);
    }
}
