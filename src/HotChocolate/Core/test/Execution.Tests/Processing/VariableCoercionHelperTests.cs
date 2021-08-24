using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Language;
using HotChocolate.StarWars;
using HotChocolate.StarWars.Models;
using HotChocolate.StarWars.Types;
using HotChocolate.Types;
using HotChocolate.Utilities;
using Snapshooter.Xunit;
using Xunit;

namespace HotChocolate.Execution.Processing
{
    public class VariableCoercionHelperTests
    {
        [Fact]
        public void VariableCoercionHelper_Schema_Is_Null()
        {
            // arrange
            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new(null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>();
            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();

            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                null!, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void VariableCoercionHelper_VariableDefinitions_Is_Null()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();
            var variableValues = new Dictionary<string, object>();
            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action()
                => helper.CoerceVariableValues(schema, null!, variableValues, coercedValues);

            // assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void VariableCoercionHelper_VariableValues_Is_Null()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, null!, coercedValues);

            // assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void VariableCoercionHelper_CoercedValues_Is_Null()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>();

            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, null!);

            // assert
            Assert.Throws<ArgumentNullException>(Action);
        }

        [Fact]
        public void Coerce_Nullable_String_Variable_With_Default_Where_Value_Is_Not_Provided()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>();
            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();

            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Empty(coercedValues);
        }

        [Fact]
        public void Coerce_Nullable_String_Variable_With_Default_Where_Value_Is_Provided()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", new StringValueNode("xyz")}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();

            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("String", Assert.IsType<StringType>(t.Value.Type).Name);
                    Assert.Equal("xyz", t.Value.Value);
                    Assert.Equal("xyz", Assert.IsType<StringValueNode>(t.Value.ValueLiteral).Value);
                });
        }

        [Fact]
        public void Coerce_Nullable_String_Variable_With_Default_Where_Plain_Value_Is_Provided()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", "xyz"}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();

            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("String", Assert.IsType<StringType>(t.Value.Type).Name);
                    Assert.Equal("xyz", t.Value.Value);
                    t.Value.ValueLiteral!.ToString().MatchSnapshot();
                });
        }

        [Fact]
        public void Coerce_Nullable_String_Variable_With_Default_Where_Null_Is_Provided()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", NullValueNode.Default}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("String", Assert.IsType<StringType>(t.Value.Type).Name);
                    Assert.Null(t.Value.Value);
                    Assert.IsType<NullValueNode>(t.Value.ValueLiteral);
                });
        }

        [Fact]
        public void Coerce_Nullable_String_Variable_With_Default_Where_Plain_Null_Is_Provided()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", null}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("String", Assert.IsType<StringType>(t.Value.Type).Name);
                    Assert.Null(t.Value.Value);
                    Assert.IsType<NullValueNode>(t.Value.ValueLiteral);
                });
        }

        [Fact]
        public void Coerce_Nullable_ReviewInput_Variable_With_Object_Literal()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("ReviewInput"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", new ObjectValueNode(new ObjectFieldNode("stars", 5))}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("ReviewInput", Assert.IsType<ReviewInputType>(t.Value.Type).Name);
                    Assert.Equal(5, Assert.IsType<Review>(t.Value.Value).Stars);
                    Assert.IsType<ObjectValueNode>(t.Value.ValueLiteral);
                });
        }

        [Fact]
        public void Coerce_Nullable_ReviewInput_Variable_With_Dictionary()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("ReviewInput"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", new Dictionary<string, object> { {"stars", 5} }}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("ReviewInput", Assert.IsType<ReviewInputType>(t.Value.Type).Name);
                    Assert.Equal(5, Assert.IsType<Review>(t.Value.Value).Stars);
                    t.Value.ValueLiteral!.ToString().MatchSnapshot();
                });
        }

        [Fact]
        public void Coerce_Nullable_ReviewInput_Variable_With_Review_Object()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("ReviewInput"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                { "abc", new Review { Stars = 5 } }
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("ReviewInput", Assert.IsType<ReviewInputType>(t.Value.Type).Name);
                    Assert.Equal(5, Assert.IsType<Review>(t.Value.Value).Stars);
                    t.Value.ValueLiteral!.ToString().MatchSnapshot();
                });
        }

        [Fact]
        public void Error_When_Value_Is_Null_On_Non_Null_Variable()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NonNullTypeNode(new NamedTypeNode("String")),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", NullValueNode.Default}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Throws<GraphQLException>(Action).Errors.MatchSnapshot();
        }

        [Fact]
        public void Error_When_PlainValue_Is_Null_On_Non_Null_Variable()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NonNullTypeNode(new NamedTypeNode("String")),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", null}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Throws<GraphQLException>(Action).Errors.MatchSnapshot();
        }

        [Fact]
        public void Error_When_Value_Type_Does_Not_Match_Variable_Type()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new VariableDefinitionNode(
                    null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {"abc", new IntValueNode(1)}
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Throws<SerializationException>(Action)
                .Errors.Select(t => t.RemoveException())
                .ToList()
                .MatchSnapshot();
        }

        [Fact]
        public void Error_When_PlainValue_Type_Does_Not_Match_Variable_Type()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new(null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                { "abc", 1 }
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Throws<SerializationException>(Action).Errors.MatchSnapshot();
        }

        [Fact]
        public void Variable_Type_Is_Not_An_Input_Type()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new(null,
                    new VariableNode("abc"),
                    new NamedTypeNode("Human"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                { "abc", 1 }
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Throws<GraphQLException>(Action).Errors.MatchSnapshot();
        }

        [Fact]
        public void Error_When_Input_Field_Has_Different_Properties_Than_Defined()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new(null,
                    new VariableNode("abc"),
                    new NamedTypeNode("ReviewInput"),
                    new StringValueNode("def"),
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                { "abc", new ObjectValueNode(new ObjectFieldNode("abc", "def")) }
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            void Action() => helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Throws<SerializationException>(Action)
                .Errors.Select(t => t.RemoveException())
                .ToList()
                .MatchSnapshot();
        }

        [Fact]
        public void CoerceVariableValues_Should_CoerceEnumList_AsEnumValues()
        {
            // arrange
            ISchema schema = SchemaBuilder.New()
                .AddDocumentFromString(
                    @"
                    type Query {
                        test(list: [FooInput]): String
                    }

                    input FooInput {
                        enum: TestEnum
                    }

                    enum TestEnum {
                        Foo
                        Bar
                    }")
                .Use(_ => _ => default)
                .Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new(null,
                    new VariableNode("abc"),
                    new ListTypeNode(new NamedTypeNode("FooInput")),
                    null,
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {
                    "abc",
                    new ListValueNode(
                        new ObjectValueNode(
                            new ObjectFieldNode("enum", "Foo")),
                        new ObjectValueNode(
                            new ObjectFieldNode("enum", "Bar")))
                }
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal(
                        "[ { enum: Foo }, { enum: Bar } ]",
                        t.Value.ValueLiteral!.ToString());
                });
        }

        [Fact]
        public void CoerceVariableValues_Should_CoerceEnumObject_AsEnumValues()
        {
            // arrange
            ISchema schema = SchemaBuilder.New()
                .AddDocumentFromString(
                    @"
                    type Query {
                        test(list: FooInput): String
                    }

                    input FooInput {
                        enum: TestEnum
                        enum2: TestEnum
                    }

                    enum TestEnum {
                        Foo
                        Bar
                    }")
                .Use(_ => _ => default)
                .Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new(null,
                    new VariableNode("abc"),
                    new NamedTypeNode("FooInput"),
                    null,
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>
            {
                {
                    "abc",
                    new ObjectValueNode(
                        new ObjectFieldNode("enum", "Foo"),
                        new ObjectFieldNode("enum2", "Bar"))
                }
            };

            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();
            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(
                schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Collection(coercedValues,
                t =>
                {
                    Assert.Equal("abc", t.Key);
                    Assert.Equal("{ enum: Foo, enum2: Bar }", t.Value.ValueLiteral!.ToString());
                });
        }

        [Fact]
        public void Variable_Is_Nullable_And_Not_Set()
        {
            // arrange
            ISchema schema = SchemaBuilder.New().AddStarWarsTypes().Create();

            var variableDefinitions = new List<VariableDefinitionNode>
            {
                new(null,
                    new VariableNode("abc"),
                    new NamedTypeNode("String"),
                    null,
                    Array.Empty<DirectiveNode>())
            };

            var variableValues = new Dictionary<string, object>();
            var coercedValues = new Dictionary<string, VariableValueOrLiteral>();

            var helper = new VariableCoercionHelper(new(), new(new DefaultTypeConverter()));

            // act
            helper.CoerceVariableValues(schema, variableDefinitions, variableValues, coercedValues);

            // assert
            Assert.Empty(coercedValues);
        }
    }
}
