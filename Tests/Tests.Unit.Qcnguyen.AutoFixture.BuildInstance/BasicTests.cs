using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using Qcnguyen.AutoFixture.BuildInstance.GenLaw;

namespace Tests.Unit.Qcnguyen.AutoFixture.BuildInstance
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void TestBuildInstance_ShouldWork_WithBuildImperative()
        {
            const int MyCustomId = 1989;
            var fixture = new Fixture();
            var obj = fixture.BuildInstance<Model>()
                .With(x => x.Id, MyCustomId)
                    .BuildScalarProperty(x => x.ScalarProp)
                        .With(x => x.Name, "myCustomScalarPropertyName")
                    .Done()
                    .BuildCollectionProperty(x => x.CollectionProp)
                        .With(x => x.Name, "myCustomCollectionPropertyName")
                    .AddManyThenDone()
                .Create();

            var expectedJson = @"
                    {
                      ""Id"": 1989,
                      ""ScalarProp"": {
                        ""Id"": 119,
                        ""Name"": ""myCustomScalarPropertyName""
                      },
                      ""CollectionProp"": [
                        {
                          ""Id"": 228,
                          ""Name"": ""myCustomCollectionPropertyName""
                        },
                        {
                          ""Id"": 94,
                          ""Name"": ""myCustomCollectionPropertyName""
                        },
                        {
                          ""Id"": 24,
                          ""Name"": ""myCustomCollectionPropertyName""
                        }
                      ]
                    }
";
            var expectedObj = JsonConvert.DeserializeObject<Model>(expectedJson);
            obj.Id.Should().Be(expectedObj.Id);
            obj.ScalarProp.Name.Should().Be(expectedObj.ScalarProp.Name);
            obj.CollectionProp.Select(x => x.Name).Should().BeEquivalentTo(expectedObj.CollectionProp.Select(x => x.Name));

        }

        [TestMethod]
        public void TestBuildInstance_ShouldWork_WithGenlaw()
        {
            const int MyCustomId = 1989;
            var genLaw = new GenLaw();
            genLaw.SetRuleSet(
                new GenRuleSet<Model>()
                .DefaultValue(x => x.Id, MyCustomId)
            );
            genLaw.SetRuleSet(
                new GenRuleSet<ScalarProp>()
                .DefaultValue(x => x.Name, "myCustomScalarPropertyName")
            );
            genLaw.SetRuleSet(
                new GenRuleSet<CollectionProp>()
                .DefaultValue(x => x.Name, "myCustomCollectionPropertyName")
            );

            var fixture = new Fixture();

            var obj = fixture.BuildInstance<Model>(genLaw)
                .Create();

            var expectedJson = @"
                    {
                      ""Id"": 1989,
                      ""ScalarProp"": {
                        ""Id"": 119,
                        ""Name"": ""myCustomScalarPropertyName""
                      },
                      ""CollectionProp"": [
                        {
                          ""Id"": 228,
                          ""Name"": ""myCustomCollectionPropertyName""
                        },
                        {
                          ""Id"": 94,
                          ""Name"": ""myCustomCollectionPropertyName""
                        },
                        {
                          ""Id"": 24,
                          ""Name"": ""myCustomCollectionPropertyName""
                        }
                      ]
                    }
";
            var expectedObj = JsonConvert.DeserializeObject<Model>(expectedJson);
            obj.Id.Should().Be(expectedObj.Id);
            obj.ScalarProp.Name.Should().Be(expectedObj.ScalarProp.Name);
            obj.CollectionProp.Select(x => x.Name).Should().BeEquivalentTo(expectedObj.CollectionProp.Select(x => x.Name));

        }

        public class Model
        {
            public int Id { get; set; }
            public ScalarProp ScalarProp { get; set; }
            public IEnumerable<CollectionProp> CollectionProp { get; set; }
        }
        public class ScalarProp
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class CollectionProp
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    
}