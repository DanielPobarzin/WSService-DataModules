using AutoMapper;
using System.Reflection;

namespace Application.Mappings
{
	public class AssemblyMappingProfile : Profile
	{
		public AssemblyMappingProfile(Assembly assembly) =>
			ApplyMappingsFromAssembly(assembly);

		private void ApplyMappingsFromAssembly(Assembly assembly)
		{
			var types = assembly.GetExportedTypes()
				.Where(type => type.GetInterfaces()
				.Any(i => i.IsGenericType &&
				i.GetGenericTypeDefinition() == typeof(IMapWith<>)))
				.ToList();
			foreach (var type in types)
			{
				var instance = Activator.CreateInstance(type);
				var metodInfo = type.GetMethod("Mapping");
				metodInfo?.Invoke(instance, new object[] { this });
			}
		}
	}
}
