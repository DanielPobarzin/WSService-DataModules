using Application.Interfaces;

namespace Application.Helpers
{
	public class ModelHelper : IModelHelper
	{
		/// <summary>
		/// Validates the specified fields against the properties of the given model type.
		/// </summary>
		/// <typeparam name="T">The type of the model to validate against.</typeparam>
		/// <param name="fields">A comma-separated string of field names to validate.</param>
		/// <returns>
		/// A comma-separated string of valid field names that exist in the model type.
		/// If no valid fields are found, an empty string is returned.
		/// </returns>
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

		/// <summary>
		/// Retrieves all property names of the specified model type as a comma-separated string.
		/// </summary>
		/// <typeparam name="T">The type of the model to retrieve fields from.</typeparam>
		/// <returns>
		/// A comma-separated string containing all property names of the model type.
		/// If there are no properties, an empty string is returned.
		/// </returns>
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
