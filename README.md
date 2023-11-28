# Benefit
Extensions to autofixture, add capability to:
  - fluent build instance: support complex object (navigation property)
  - GenLaw to define default behavior when generate object instance

# Example: 
- supposed we have the following classes :
```cs
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
```
## example of build using imperative way
```cs
        [TestMethod]
        public void TestBuildInstance_ShouldWork_WithBuildImperative()
        {
            const int MyCustomId = 1989;
            var fixture = new Fixture();

            //build object with imperative command
            var obj = fixture.BuildInstance<Model>()
                .With(x => x.Id, MyCustomId)
                    .BuildScalarProperty(x => x.ScalarProp)
                        .With(x => x.Name, "myCustomScalarPropertyName")
                    .Done()
                    .BuildCollectionProperty(x => x.CollectionProp)
                        .With(x => x.Name, "myCustomCollectionPropertyName")
                    .AddManyThenDone()
                .Create();

            //assert the generated object
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
```
## example of build using GenLaw default
```cs
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

            //build object using default in GenLaw
            var obj = fixture.BuildInstance<Model>(genLaw)
                .Create();

            //assert the generated object
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
```
