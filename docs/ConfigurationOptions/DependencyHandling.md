---
title: "Dependency Handling"
permalink: /dependency-handling/
parent: Configuration options
---
# Dependency Handling

## Natural dependency handling

Grate has two methods of handling script dependencies: the first is by ensuring files are named in a fashion that clearly defines the order of execution of each script.

For example, if have two tables, one called `fact.Orders` and one called `dim.Suppliers`, and your fact table has a referential constraint on the dimension table. Given that the letter `d` comes before `f`, this requirement is naturally enforced thus allowing the fact table to build successfully.

However if you have the scripts just named `Orders.sql` and `Suppliers.sql` then this dependecy will essentially be broken as the `Orders.sql` script will be executed before the `Suppliers.sql` script, resulting in a potential failure. To get past this you'd need to specifically name the scripts something like `001-Suppliers.sql` and `002-Orders.sql` which then can make it hard to find names of objects.

## Declarative dependency handling

This is where grate's built in declarative dependency handling can come into play. grate now supports the ability to parse dependent names inside scripts and to ensure they're handled in a [Topological order](https://en.wikipedia.org/wiki/Topological_sorting)

Using the previous example, you could add the following comment to the top of the `Orders.sql` script:

```sql
-- Dependencies: Suppliers.sql
```

To request grate honor these dependencies, you must also pass the following command line argument:

`--AnalyzeScriptsForDependencies`

That alone is enough to ensure that the `Suppliers.sql` script will be executed _before_ `Order.sql`.

The default [Regular Expression](https://en.wikipedia.org/wiki/Regular_expression) used to locate dependencies is:<br><br> `^(\-\-[ \t]*Dependencies[\t ]*:[\t ]*)(([\\*?#\w.]+\.sql[\t ]*)+)$`
<br>

You're not limited to using that format, however. Let's say, for example, you have a corporate banner format that looks like this

```sql
/**************************************************************************
* Script Name  : Orders.sql
* Author       : Jane Appleseed
* Date Created : 09/01/2022
***************************************************************************/
```

You could change it to look like

```sql
/**************************************************************************
* Script Name  : Orders.sql
* Author       : Jane Appleseed
* Date Created : 09/01/2022
* Dependencies : Suppliers.sql
***************************************************************************/
```

You would then need to pass a modifified [Regular Expression](https://en.wikipedia.org/wiki/Regular_expression) pattern to match this format:<br>

e.g.

`--dependencyregex "^(\*[ \t]*Dependencies[\t ]*:[\t ]*)(([\\*?#\w.]+\.sql[\t ]*)+)$"`

Or perhaps you like to keep it simpler:

`--dependencyregex "^(\*[ \t]*Uses[\t ]*:[\t ]*)(([\\*?#\w.]+\.sql[\t ]*)+)$"`

```sql
-- Uses: Suppliers.Sql
```

You're also not limited to a single dependency, for example this is also valid:

```sql
/**************************************************************************
* Script Name  : Orders.sql
* Author       : Jane Appleseed
* Date Created : 09/01/2022
* Dependencies : Suppliers.sql Customers.sql
***************************************************************************/
```

## Using subfolders

The dependency checking will also handle subfolders as well: For example in tjhe following example you have subfolders under the `up` folder with files like:

```
up/2022views/2022_Orders.sql
up/tables/Orders.sql
```

Where the `2022_Orders.sql` is dependent on the table defined in `Order.sql`.

This can be specified as:

```sql
/**************************************************************************
* Script Name  : 2022-Orders.sql
* Author       : Jane Appleseed
* Date Created : 09/01/2022
* Dependencies : tables:Order.sql
***************************************************************************/
```

Here the subfolders are seperated with a `\` character. You __must__ use this character, regardless of the platform. Don't worry though because grate will convert it to `/` on Linux and MacOs as required.

## Handling mutiple dependencies

Grate is also able to allow you to define how you seperate the dependencies. The default is to use the following [Regular Expression](https://en.wikipedia.org/wiki/Regular_expression): `[\s]+`. 

However perhaps you perfer to list out your dependencies in a comma seperated format. grate's got you covered here as well:

`--dependencysplitterregex ",*"`

Would cover the the following example (assuming the default setting for `--dependencyregex`)

```sql
/**************************************************************************
* Script Name  : Orders.sql
* Author       : Jane Appleseed
* Date Created : 09/01/2022
* Dependencies : Suppliers.sql,Customers.sql
***************************************************************************/
```

## Wildcard dependencies

Finally, grate is fully able to process basic wildcard symbols in your dependencies:

| Symbol | Meaning |
|:---:| --- |
| * | Any series of characters |
| ? | Any single character |
| # | Any numeric character (0-9) |

Given the following file listing:

```
Tables\dim.Customer.sql
Tables\dim.Product.sql
Tables\fact.Orders.sql
```

The dependency specification

```sql
-- Dependencies: Tables\dim.*.sql
```

would be equivilent to 

```sql
-- Dependencies: Tables\dim.Customer.sql Tables\dim.Products.sql
```

And

```sql
-- Dependencies: Tables\*.sql
```

would be equivilent to 


```sql
-- Dependencies: Tables\dim.Customer.sql Tables\dim.Products.sql Tables\fact.Orders.sql
```