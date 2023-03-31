using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace api.Extensions
{
	public static class DisplayEnumStringExtn
    {
        public static string GetEnumDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                   .First()
                   .GetCustomAttribute<DisplayAttribute>()
                   .Name;
        }
        
    }
}