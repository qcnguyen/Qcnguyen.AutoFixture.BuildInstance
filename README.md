# Qcnguyen.AutoFixture.BuildInstance
Extensions to autofixture, add capability to:
  - fluent build instance
  - GenLaw to define default behivor when generate object instance

Example: 
1. build imperative 
```cs
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
```
2. build using GenLaw
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
```
