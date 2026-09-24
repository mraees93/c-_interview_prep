# 📡 Angular API Consumption & Performance Interview Guide

### Q1: What happens if the user types 10 letters in 1 second inside our filter box? How do we optimize it?
**A:** Right now, our `computed()` signal processor re-evaluates the entire 500-record array matrix on *every single keystroke*. While modern JavaScript engines process 500 records very quickly, rapid typing still forces unnecessary layout checks.
* **The Optimization:** In a production application, I would implement an **input debounce mechanism**. By taking our raw input `Event` and piping it into an RxJS stream using `debounceTime(300)`, we can convert that stream back into a signal using `toSignal()`. This forces the application to wait for a 300ms pause in typing before updating the state signal, executing the filter matrix exactly once instead of 10 times.

### Q2: How would we scale this architecture if the dataset grew from 500 records to 500,000 records?
**A:** Client-side processing (filtering and sorting directly in browser memory) is excellent for small-to-medium matrices up to a few thousand rows. Passing 500,000 deeply nested objects to the client will stall network bandwidth and freeze the browser's single main thread during array iteration.
* **The Architecture Pivot:** I would refactor the engine to use **Server-Side Pagination, Filtering, and Sorting**. Instead of pulling the entire database once at application startup, the component class would pass `page`, `pageSize`, and `searchQuery` directly to the `HttpClient` parameters. The backend database handles the heavy lifting, sending down only 20 records at a time.

### Q3: Why did we place our array filter/sort logic inside a `computed()` signal instead of a standard class method or an HTML template expression?
**A:** If you bind an HTML template expression straight to a standard class function (e.g., `<div>{{ filterData() }}</div>`), Angular is forced to re-run that entire function during *every single change detection cycle*—even if the user just clicked an unrelated checkbox or opened a menu modal. Caching our processing matrix inside **`computed()`** establishes a memoized dependency gate. It guarantees that the 500 rows are only re-filtered when their specific underlying signals change (`comments`, `searchTerm`, or `sortDirection`), protecting against unnecessary rendering performance costs.

### Q4: Why can we use an initialization `effect()` block inside the constructor to fetch data instead of traditional lifecycle hooks like `ngOnInit`?
**A:** In modern Angular, placing an asynchronous data fetch inside an **`effect()`** block within the class constructor replaces the need for traditional imperative lifecycles. Because `effect()` is natively reactive, Angular automatically registers its tracking context and schedules it to fire safely right after the initial rendering cycle completes. This provides a declarative setup that closely mirrors the execution profile of React’s `useEffect` hook with an empty dependency array.

### Q5: What is the architectural difference between Angular's `HttpClient` returning an RxJS Observable and us converting it to a Promise with `firstValueFrom`?
**A:** Angular's `HttpClient` natively returns an **RxJS Observable**, which acts as a lazy, multi-emission data stream built to handle continuous events over time (like web sockets or value logs). However, an HTTP network request is fundamentally a single-emission event—it fires once, returns data, and finishes. By wrapping the call in **`firstValueFrom`**, we cleanly convert that stream into a standard JavaScript **`Promise`**. This allows us to use readable `async / await` syntax inside our `effect()` block while ensuring that the underlying stream automatically unsubscribes to completely prevent memory leaks.

