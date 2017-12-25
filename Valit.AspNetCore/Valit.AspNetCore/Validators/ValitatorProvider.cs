using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Valit.AspNetCore.Controllers;

namespace Valit.AspNetCore.Validators
{
	public class ValitatorProvider : IModelValidatorProvider
	{
		public void CreateValidators(ModelValidatorProviderContext context)
		{
			if(context.ModelMetadata.MetadataKind == ModelMetadataKind.Type)
			{
				var item = new ValidatorItem
				{
					Validator = new Valitator(),
					IsReusable = true,
					
				};

				context.Results.Add(item);
			}
            var test = context;
			//throw new System.NotImplementedException();
		}
	}


	public class Valitator : IModelValidator
	{
		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
		{
			var result = ValitRules<Model>
				.Create()
				.Ensure(m => m.Value, _=>_.Required().WithMessage("1").Email().WithMessage("2"))
				.Ensure(m => m.Value2, _=>_.IsPositive())
				.For(context.Model as Model)
				.Validate();

			return result.Succeeded? Enumerable.Empty<ModelValidationResult>() : result.ErrorMessages.Select(em => new ModelValidationResult("TESTO", em));

		}
	}
}