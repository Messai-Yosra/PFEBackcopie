using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stage_api.configuration;
using stage_api.NewFolder;
using System.Reflection;

namespace stage_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnitityController : ControllerBase
    {
		private readonly dbContext _context;

		public EnitityController(dbContext context)
		{
			_context = context;
		}

		[HttpGet]
        [Route("getEntities")]
        public IActionResult GetModelInfo()
        {
            // Get all loaded assemblies in the current application domain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // List to store model information
            var modelInfoList = new List<ModelInfo>();

            // Namespace containing your models
            string modelNamespace = "stage_api.Models";

            // Iterate through each assembly
            foreach (var assembly in assemblies)
            {
                try
                {
                    // Get all types defined in the assembly
                    var types = assembly.GetTypes();

                    // Filter types that are in the model namespace and are not ModelInfo
                    var modelTypes = types.Where(t => t.Namespace == modelNamespace && t.IsClass && t != typeof(ModelInfo));

                    // Iterate through model types
                    foreach (var modelType in modelTypes)
                    {
                        // Create model information object
                        var modelInfo = new ModelInfo
                        {
                            Name = modelType.Name,
                            Attributes = new List<string>() // Initialize the list
                        };

                        // Get properties of the model type
                        foreach (var prop in modelType.GetProperties())
                        {
                            // Add property name to the attributes list
                            modelInfo.Attributes.Add(prop.Name);
                        }

                        // Add model information to the list
                        modelInfoList.Add(modelInfo);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle exceptions when trying to load types from the assembly
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        Console.WriteLine(loaderException.Message);
                    }
                }
            }

            return Ok(modelInfoList);
        }

		[HttpPost("CreateTable")]
		public async Task<IActionResult> CreateTable([FromBody] CreateTableRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Build SQL command dynamically using request parameters
				string sqlCommand = $"CREATE TABLE {request.TableName} (Id INT PRIMARY KEY";

				foreach (var attribute in request.Attributes)
				{
					sqlCommand += $", {attribute.Name} {attribute.DataType}";
				}

				sqlCommand += ")";

				// Execute the dynamically generated SQL command
				await _context.Database.ExecuteSqlRawAsync(sqlCommand);

				return Ok("Table created successfully.");
			}
			catch (Exception ex)
			{
				// Handle exceptions
				return StatusCode(500, $"Error creating table: {ex.Message}");
			}
		}
	}
}
