# How It Works - Why **Norm** - And - Similarities With Dapper

Both libraries are implementation of set of various extensions over **`System.Data.Common.DbConnection`** system object to work efficiently with different databases.

They work quite differently. For example, query that returns **one million records** from database:

- Dapper:

```csharp
var results = connection.Query<TestClass>(query);
```

Resulting in `IEnumerable<TestClass>` in **`00:00:02.0330651`** seconds.

- Norm:

```csharp
var results = connection.Read(query).Select(t => new TestClass(t));
```

Resulting in `IEnumerable<TestClass>` in **`00:00:00.0003062`** seconds.

That is a fraction of time and reason is simple:

- *Dapper* triggers iteration and objects serialization immediately when called.

- *Norm* builds internal iterator over database results.

- This allows delaying any results iteration (and potential serialization) - so that expression tree can be built (using `System.Linq` and/or `System.Linq.Async` libraries or custom `IEnumerable` extensions) for our view models or service responses - **before any actual results iteration.**

This approach can save unnecessary database result iterations to improve system performances.

If we execute `ToList()` extension on results above - we will see similar execution times but this time - vice versa. Meaning this time results from Dapper will execute in fraction of time and results from NoOrm will execute approximately as Dapper the first time.

That is because `ToList()` triggers iteration automatically - if `List` structure hasn't been built yet. And since Dapper builds `List` internally each call by default, it will not be executed again. Contrary NoRom haven't run any iterations yet, and it will have to do it for the first time.

So, why it matters then?

This is typical application scenario with Dapper:

1. Runs iteration over database results to build internal list.

2. Application defines expressions and transformations that transforms the data in required output (such as typically view-models and service responses).

3. Application iterates again over transformed data to build required view or to serialize to response.

So this scenario requires at least two iteration over data.

With `Norm` approach it would look something like this:

1. Build iterator over tuples that will be returned from database

2. Build expression tree over enumerable iterator - sync or async - (`System.Linq`, `System.Linq.Async`, custom, etc)

3. Execute everything to create results (view-models and service responses)

This allows to keep current design with separation of concerns and to have only one, single iteration.

Also, when working asynchronously in first scenario - typically we have to wait until all results are retrieved from database and then start building the response or view asynchronously.

Norm utilizes new `IAsyncEnumerable` interface which doesn't need to wait until all records are fetched and retrieved from database to start building the results.

Instead, result item is processed as it appears, effectively doing the asynchronous streaming directly from database.
