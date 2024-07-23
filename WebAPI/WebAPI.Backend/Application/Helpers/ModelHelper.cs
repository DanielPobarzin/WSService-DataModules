using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Helpers
{
	public class ModelHelper : IModelHelper
	{
		public string ValidateModelFields<T>(string fields)
		{
			string retString = string.Empty;

			var bindingFlags = System.Reflection.BindingFlags.Instance |
								System.Reflection.BindingFlags.NonPublic |
								System.Reflection.BindingFlags.Public;
			var listFields = typeof(T).GetProperties(bindingFlags).Select(f => f.Name).ToList();
			string[] arrayFields = fields.Split(',');
			foreach (var field in arrayFields)
			{
				if (listFields.Contains(field.Trim(), StringComparer.OrdinalIgnoreCase))
					retString += field + ",";
			};
			return retString;
		}
		public string GetModelFields<T>()
		{
			string retString = string.Empty;

			var bindingFlags = System.Reflection.BindingFlags.Instance |
								System.Reflection.BindingFlags.NonPublic |
								System.Reflection.BindingFlags.Public;
			var listFields = typeof(T).GetProperties(bindingFlags).Select(f => f.Name).ToList();

			foreach (string field in listFields)
			{
				retString += field + ",";
			}

			return retString;
		}
	}
}
