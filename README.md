# SqlGen - An easy SQL generator using .NET

## What this is not
This is not another ORM.

## What is it then?
This is for generating SQL, simple and pure.

If you think that an ORM is not suitable for your project, that it takes too many resources, has too much overhead or otherwise just seems overkill, this might be the perfect solution.

Instead of having to write the command text of your queries by hand, use SqlGen to create it for you.

# LICENSE
MIT - See [LICENSE](https://github.com/mfas2410/SqlGenerator/blob/master/LICENSE)

# INFO
## Getting started with SqlGen
Get started by creating a POCO, e.g.:

```
public class Table1
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

and use the QueryBuilder:

```
string sql = new QueryBuilder(Dialect.TSql2017).From<Table1>().ToString();
```

This will generate the following SQL:

```
SELECT [Table1].[Id],[Table1].[Name] FROM [Table1]
```

## Examples
For more examples please explore the [SqlGen.UnitTest](https://github.com/mfas2410/SqlGenerator/tree/master/src/SqlGen.UnitTest) project.

## State
This is a very early development version with only very crude SQL query generating abilities.

You are more than welcome to contribute to this project.

## Contributing
1. Clone
2. Branch
3. Make changes
4. Push
5. Make a pull request

# REQUIREMENTS
You need at least Visual Studio 2017, .NET Core 2.0 and .NET Framework 4.7.2

# CREDITS
Copyright (c) 2018 Michael Fastrup
