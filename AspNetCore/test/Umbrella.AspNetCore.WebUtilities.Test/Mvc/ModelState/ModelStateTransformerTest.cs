using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelState;
using Umbrella.WebUtilities.ModelState;
using Xunit;

namespace Umbrella.AspNetCore.WebUtilities.Test.Mvc.ModelState
{
    public class ModelStateTransformerTest
    {
        public class TestModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public TestModel Child { get; set; }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Test_ModelErrors_Success(bool useCamelCase)
        {
            //Arrange
            var transformer = CreateModelStateTransformer(useCamelCase);

            ModelStateDictionary modelState = new ModelStateDictionary();
            modelState.AddModelError<TestModel>(x => x.FirstName, "Please enter your first name");
            modelState.AddModelError<TestModel>(x => x.LastName, "Please enter your last name");
            modelState.AddModelError<TestModel>(x => x.Child.FirstName, "Please enter your child's first name");
            modelState.AddModelError("", "This is a generic error");
            modelState.AddModelError("", "This is a second generic error");

            //Act
            var transformedModelState = transformer.Transform(modelState);

            //Assert
            Assert.Equal(4, transformedModelState.Entries.Count);

            var firstNameEntry = transformedModelState.Entries.Find(x => x.Key == (useCamelCase ? "firstName" : "FirstName"));
            Assert.NotNull(firstNameEntry);
            Assert.StrictEqual(1, firstNameEntry.Errors.Count);
            Assert.Equal("Please enter your first name", firstNameEntry.Errors[0]);

            var lastNameEntry = transformedModelState.Entries.Find(x => x.Key == (useCamelCase ? "lastName" : "LastName"));
            Assert.NotNull(lastNameEntry);
            Assert.StrictEqual(1, lastNameEntry.Errors.Count);
            Assert.Equal("Please enter your last name", lastNameEntry.Errors[0]);

            var childFirstNameEntry = transformedModelState.Entries.Find(x => x.Key == (useCamelCase ? "child.firstName" : "Child.FirstName"));
            Assert.NotNull(childFirstNameEntry);
            Assert.StrictEqual(1, childFirstNameEntry.Errors.Count);
            Assert.Equal("Please enter your child's first name", childFirstNameEntry.Errors[0]);

            var emptyKeyEntry = transformedModelState.Entries.Find(x => x.Key == "");
            Assert.NotNull(emptyKeyEntry);
            Assert.StrictEqual(2, emptyKeyEntry.Errors.Count);
            Assert.Equal("This is a generic error", emptyKeyEntry.Errors[0]);
            Assert.Equal("This is a second generic error", emptyKeyEntry.Errors[1]);
        }

        private IModelStateTransformer<DefaultTransformedModelState<DefaultTransformedModelStateEntry>> CreateModelStateTransformer(bool useCamelCase)
        {
            var options = new ModelStateTransformerOptions { UseCamelCaseCaseForKeys = useCamelCase };
            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            return new ModelStateTransformer<DefaultTransformedModelState<DefaultTransformedModelStateEntry>, DefaultTransformedModelStateEntry>(options, memoryCache);
        }
    }
}