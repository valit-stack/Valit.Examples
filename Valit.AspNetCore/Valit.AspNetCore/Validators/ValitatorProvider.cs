using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Valit.AspNetCore.Controllers;

namespace Valit.AspNetCore.Validators
{
	public class ValitatorProvider : IModelValidatorProvider
	{
		private readonly ValitatorFactory _factory;

		public ValitatorProvider(ValitatorFactory factory)
		{
			_factory = factory;
		}

		public void CreateValidators(ModelValidatorProviderContext context)
		{
			if(context.ModelMetadata.MetadataKind == ModelMetadataKind.Type)
			{
				var item = new ValidatorItem
				{
					Validator = new Valitator(_factory),
					IsReusable = true,
					
				};

				context.Results.Add(item);
			}
            var test = context;
			//throw new System.NotImplementedException();
		}
	}

	public class ModelValitator : IValitator<Model>
	{
		public IValitResult Validate(Model @object, IValitStrategy strategy = null)
			=> ValitRules<Model>
				.Create()
				.Ensure(m => m.Value, _=>_.Required().WithMessage("1").Email().WithMessage("2"))
				.Ensure(m => m.Value2, _=>_.IsPositive())
				.For(@object)
				.Validate();
	}

	public class Resolver 
	{
		private readonly IServiceProvider _provider;
		public Resolver(IServiceProvider provider)
		{
			_provider = provider;
		}

		public TImpl Resolve<TImpl>() where TImpl : class => _provider.GetService(typeof(TImpl)) as TImpl;
	}

	public class ValitatorFactory
	{
		private readonly Resolver _r;

		public ValitatorFactory(Resolver r)
		{
			_r = r;
		}

		public IValitator<TModel> GetValitator<TModel>() where TModel : class => _r.Resolve<IValitator<TModel>>();
	}

	public class Valitator : IModelValidator
	{

		private readonly ValitatorFactory _factory;

		public Valitator(ValitatorFactory factory)
		{
			_factory = factory;
		}

		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
		{
			var result = _factory.GetValitator<Model>().Validate(context.Model as Model);

			return result.Succeeded? Enumerable.Empty<ModelValidationResult>() : result.ErrorMessages.Select(em => new ModelValidationResult("TESTO", em));
		}
	}
}