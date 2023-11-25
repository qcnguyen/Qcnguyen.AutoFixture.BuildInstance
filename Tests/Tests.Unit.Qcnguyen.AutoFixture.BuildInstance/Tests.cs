using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using Qcnguyen.AutoFixture.BuildInstance.GenLaw;

namespace Tests.Unit.Qcnguyen.AutoFixture.BuildInstance
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestBuildInstance_ShouldPreferImperativeOverGenLaw()
        {
            const int MyCustomId = 1989;

            var genLaw = new GenLaw();
            genLaw.SetRuleSet(
                new GenRuleSet<Model>()
                .DefaultValue(x => x.Id, MyCustomId)
            );
            genLaw.SetRuleSet(
                new GenRuleSet<ScalarProp>()
                .DefaultValue(x => x.Name, "myCustomScalarPropertyName_GenLaw")
            );
            genLaw.SetRuleSet(
                new GenRuleSet<CollectionProp>()
                .DefaultValue(x => x.Name, "myCustomCollectionPropertyName_GenLaw")
            );

            var fixture = new Fixture();
            var obj = fixture.BuildInstance<Model>(genLaw)
                .With(x => x.Id, MyCustomId)
                    .BuildScalarProperty(x => x.ScalarProp)
                        .With(x => x.Name, "myCustomScalarPropertyName-Imperative")
                    .Done()
                    .BuildCollectionProperty(x => x.CollectionProp)
                        .With(x => x.Name, "myCustomCollectionPropertyName-Imperative")
                    .AddManyThenDone()
                .Create();

            var expectedJson = @"
                    {
                      ""Id"": 1989,
                      ""ScalarProp"": {
                        ""Id"": 119,
                        ""Name"": ""myCustomScalarPropertyName-Imperative""
                      },
                      ""CollectionProp"": [
                        {
                          ""Id"": 228,
                          ""Name"": ""myCustomCollectionPropertyName-Imperative""
                        },
                        {
                          ""Id"": 94,
                          ""Name"": ""myCustomCollectionPropertyName-Imperative""
                        },
                        {
                          ""Id"": 24,
                          ""Name"": ""myCustomCollectionPropertyName-Imperative""
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
        public void TestBuildInstance_ShouldCorrectlyGenImperativeAndGenLaw()
        {
            const int MyCustomId = 1989;

            var genLaw = new GenLaw();
            genLaw.SetRuleSet(
                new GenRuleSet<ModelWithDup>()
                .DefaultValue(x => x.Id, MyCustomId)
            );
            genLaw.SetRuleSet(
                new GenRuleSet<ScalarProp>()
                .DefaultValue(x => x.Name, "myCustomScalarPropertyName_GenLaw")
            );
            genLaw.SetRuleSet(
                new GenRuleSet<CollectionProp>()
                .DefaultValue(x => x.Name, "myCustomCollectionPropertyName_GenLaw")
            );

            var fixture = new Fixture();
            var obj = fixture.BuildInstance<ModelWithDup>(genLaw)
                .With(x => x.Id, MyCustomId)
                    .BuildScalarProperty(x => x.ScalarProp)
                        .With(x => x.Name, "myCustomScalarPropertyName-Imperative")
                    .Done()
                    .BuildCollectionProperty(x => x.CollectionProp)
                        .With(x => x.Name, "myCustomCollectionPropertyName-Imperative")
                    .AddManyThenDone()
                .Create();

            var expectedJson = @"
                    {
                      ""Id"": 1989,
                      ""ScalarProp"": {
                        ""Id"": 164,
                        ""Name"": ""myCustomScalarPropertyName-Imperative""
                      },
                      ""ScalarPropDup"": {
                        ""Id"": 60,
                        ""Name"": ""myCustomScalarPropertyName_GenLaw""
                      },
                      ""CollectionProp"": [
                        {
                          ""Id"": 229,
                          ""Name"": ""myCustomCollectionPropertyName-Imperative""
                        },
                        {
                          ""Id"": 237,
                          ""Name"": ""myCustomCollectionPropertyName-Imperative""
                        },
                        {
                          ""Id"": 75,
                          ""Name"": ""myCustomCollectionPropertyName-Imperative""
                        }
                      ],
                      ""CollectionPropDup"": [
                        {
                          ""Id"": 103,
                          ""Name"": ""myCustomCollectionPropertyName_GenLaw""
                        },
                        {
                          ""Id"": 243,
                          ""Name"": ""myCustomCollectionPropertyName_GenLaw""
                        },
                        {
                          ""Id"": 110,
                          ""Name"": ""myCustomCollectionPropertyName_GenLaw""
                        }
                      ]
                    }
";
            var expectedObj = JsonConvert.DeserializeObject<ModelWithDup>(expectedJson);
            obj.Id.Should().Be(expectedObj.Id);
            obj.ScalarProp.Name.Should().Be(expectedObj.ScalarProp.Name);
            obj.ScalarPropDup.Name.Should().Be(expectedObj.ScalarPropDup.Name);
            obj.CollectionProp.Select(x => x.Name).Should().BeEquivalentTo(expectedObj.CollectionProp.Select(x => x.Name));
            obj.CollectionPropDup.Select(x => x.Name).Should().BeEquivalentTo(expectedObj.CollectionPropDup.Select(x => x.Name));

        }


        public class Model
        {
            public int Id { get; set; }
            public ScalarProp ScalarProp { get; set; }
            public IEnumerable<CollectionProp> CollectionProp { get; set; }
        }
        public class ModelWithDup
        {
            public int Id { get; set; }
            public ScalarProp ScalarProp { get; set; }
            public ScalarProp ScalarPropDup { get; set; }
            public IEnumerable<CollectionProp> CollectionProp { get; set; }
            public IEnumerable<CollectionProp> CollectionPropDup { get; set; }
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