# 📡 React API Consumption & Performance Interview Guide

### Q1: What happens if the user types 10 letters in 1 second inside our filter box? How do we optimize it?
**A:** Right now, our `useMemo` array processor re-evaluates the entire 500-record array matrix on *every single keystroke*. While modern machines can handle 500 entries smoothly, typing rapidly can still waste valuable CPU cycles. 
* **The Optimization:** In a production-grade application, I would introduce an **input debounce mechanism** (typically using a 300ms window via a timer or a library like Lodash). This stalls processing until the user pauses their typing, executing the filter matrix exactly once instead of 10 times.

```tsx
export default function SearchInput() {
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [debouncedValue, setDebouncedValue] = useState<string>('');

  useEffect(() => {
    // 1. Set a timer to update the filter value after 300ms
    const timer = setTimeout(() => {
      setDebouncedValue(searchTerm);
    }, 300);

    // 2. Clear the timer instantly if the user types another letter before 300ms passes
    return () => clearTimeout(timer);
  }, [searchTerm]); // Re-runs on every keystroke

  // 3. Pass 'debouncedValue' to your filtering logic instead of 'searchTerm'
}
```

### Q2: How would we scale this architecture if the dataset grew from 500 records to 500,000 records?
**A:** Client-side processing (filtering and sorting directly in browser memory) is excellent for small-to-medium matrices up to a few thousand rows. Passing 500,000 deeply nested objects to the client will stall network bandwidth and freeze the browser thread entirely during array iteration.
* **The Architecture Pivot:** I would refactor the engine to use **Server-Side Pagination, Filtering, and Sorting**. Instead of pulling the entire database once, the component would track a `page`, `pageSize`, and `searchQuery` state, appending them directly to the API url parameters (e.g., `?page=1&search=nikita`). The backend database then does the heavy lifting, sending down only 20 records at a time.

### Q3: Why did we place our array filter/sort logic inside a `useMemo` hook instead of just writing it directly in the body of our React functional component?
**A:** If you write processing logic directly in the body of a functional component, that logic runs from scratch on **every single component re-render**. In React, *any* state change—even an unrelated UI loading toggle, modal pop-up, or parent update—forces a re-render. Caching our processing matrix inside **`useMemo`** acts as a structural gatekeeper. It ensures that the 500 rows are only re-filtered when their specific underlying dependencies change (`comments`, `searchTerm`, or `sortDirection`), protecting against unnecessary rendering performance costs.

### Q4: What is a "race condition" in API consumption, and how can a component handle it safely?
**A:** A race condition occurs when a user triggers multiple asynchronous network requests in rapid succession (e.g., clicking on Category A and then immediately clicking Category B). Because network latency shifts unpredictably, the response for Category A might resolve *after* Category B finishes loading. This overwrites your local state and leaves the UI rendering completely incorrect data.
* **The Fix:** Inside a **`useEffect`** hook, you can implement an internal boolean cleanup flag (often called an **Abort Controller** or an active-state latch). When the component re-fires or unmounts, you flip that flag to false, instructing your promise handler to ignore the outdated incoming data package entirely.

### Q5: Why is it risky to rely on raw `catch (err)` bindings without an explicit Type Guard in TypeScript?
**A:** In JavaScript runtime engines, a `throw` event is structurally unrestricted. A system function or backend engine can throw a standard `Error` instance, a raw text string, an integer status code, or even a null value. If your code unconditionally tries to read `err.message` without verification, and the runtime throws a raw string instead, your frontend code will experience a null-pointer crash. Using a **TypeScript Type Guard** (like `if (err instanceof Error)`) guarantees compilation safety by forcing you to isolate and verify the object schema before reading properties.

### Q6: Why is direct state mutation like sorting/filtering an array of objects directly BAD?
**A:** Direct mutation overrides data in place within its existing memory address. Because React relies on shallow reference comparison to detect state updates, returning the same memory pointer tricks the engine into thinking nothing changed, causing the UI to completely skip re-rendering. To keep state transitions predictable, always use operators like the spread syntax `[...]` to spin up a shallow copy in a fresh memory address before executing transforms.

### Q7: Why should the `useMemo` hook compute transformations instead of modifying the state directly?
**A:** The `useMemo` hook should always treat local component state as a read-only, single source of truth. Overwriting your raw state array during filtering or sorting actions causes you to lose your baseline dataset, forcing expensive, redundant network re-fetches when a user clears a search input. By using `useMemo` to create a shallow copy and execute transformations on the fly, you preserve the original data matrix intact in memory, ensure blistering fast client-side updates, and keep the application state highly predictable.

### Q8: Why should derived variables (like sorting an array) be placed *inside* `useMemo` rather than computed in the component body and passed into `useMemo` as a dependency?
**A:** In JavaScript, methods like `[...data].sort()` create a brand-new array reference in memory on *every single render*. If you compute a derived variable in the main component body and pass it into `useMemo`'s dependency array, `useMemo` sees a new memory pointer on every render and recalculates anyway—completely defeating the purpose of memoization. Combining both filtering and sorting inside a single `useMemo` that depends directly on raw state (`[data, searchTerm, currentSort]`) guarantees true caching.

### Q9: How does this immutable React `useMemo` filtering and sorting pattern compare to how we handle collections in a Java Spring Boot backend?
**A:** Both ecosystems follow the exact same **functional immutability paradigm** to ensure predictability and thread safety. In React, we avoid mutating state arrays in-place by creating a shallow copy (`[...data]`) and chaining `.filter()` and `.sort()` inside `useMemo`. In **Java 8+**, instead of mutating a `List<Movie>` directly with loops or `Collections.sort()`, we convert the list into a **Java Stream** (`movies.stream().filter(...).sorted(...).toList()`). Just like React's `useMemo`, Java Streams leave the original source collection completely untouched and produce a fresh, transformed output collection.

### Q10: What is the difference between `useMemo` and `useCallback`?
**A:** Both hooks optimize performance by caching between renders, but they cache different things:
* **`useMemo`** caches the **result (return value)** of an expensive calculation (like a filtered `processedMovies` array).
* **`useCallback`** caches the **function definition itself** so it retains the exact same memory reference across renders. You use `useCallback` when passing callback functions down as props to child components wrapped in `React.memo`, preventing the child from re-rendering needlessly.

### Q11: What is the difference between Controlled and Uncontrolled components in React?
**A:** 
* **Controlled Component:** Form input data is handled entirely by React state (`value={searchTerm}` + `onChange={handleSearch}`). React acts as the single source of truth, making instant validation and live search filtering trivial.
* **Uncontrolled Component:** Form data is handled directly by the DOM itself, accessed via a **`useRef`** hook only when needed (e.g., on form submit). Uncontrolled inputs trigger zero re-renders while typing, making them useful for massive forms where live keystroke tracking isn't needed.

### Q12: How would you reuse API fetching logic (`loading`, `error`, and `data` states) across multiple components without duplicating code?
**A:** By extracting it into a **Custom Hook** (e.g., `useFetch<T>(url: string)`). Custom hooks let you encapsulate stateful `useState` and `useEffect` logic into a clean, reusable TypeScript function. Any component can then consume the endpoint in a single line (`const { data, isLoading, error } = useFetch<TMDBMovie[]>(url)`), keeping UI components focused strictly on rendering.

### Q13: How do you prevent a runtime JavaScript error inside a child component from crashing the entire React application?
**A:** By wrapping UI sections in a React **Error Boundary**. Error Boundaries catch unhandled rendering errors anywhere in their child component tree, log the error, and display a graceful fallback UI (e.g., *"Something went wrong loading this widget"*) instead of leaving the user with a blank white screen. *(Note: Error Boundaries still require a class component implementing `componentDidCatch` or a lightweight library like `react-error-boundary`).*

Error boundaries catch Errors in rendering, Lifecycle methods, Constructors of class components.
Error boundaries *DONT* catch Event handlers (use try/catch inside them), Asynchronous code (e.g., setTimeout, Promises), Server-side rendering (SSR) errors


### Q14: What is `React.memo`, and when should you use it (or avoid it)?
**A:** `React.memo` is a Higher-Order Component that wraps a child component to prevent unnecessary re-renders. By default, when a parent component re-renders, React recursively re-renders all of its children—even if the child's props haven't changed. `React.memo` forces React to perform a **shallow comparison** of the child's props; if they are identical to the previous render, React skips rendering that child and reuses the last painted result.
* **When to use it:** On pure, presentational child components that render frequently with the exact same props (like an individual `MovieCard` inside a 500-item list).
* **When to avoid it:** On components whose props change on almost every render anyway, as the shallow comparison adds memory and CPU overhead for zero benefit. *(Note: Always pair `React.memo` with `useCallback` when passing function props, otherwise a new function reference breaks the shallow check).*

### Q15: What is a Higher-Order Component (HOC), and how has its usage changed in modern React?
**A:** A Higher-Order Component is an advanced design pattern where a **function takes a component as an argument and returns a brand-new, enhanced component** (e.g., `const ProtectedDashboard = withAuth(Dashboard)`). It was historically used to share cross-cutting logic like authentication checks, logging, or data fetching across multiple components.
* **Modern Shift:** In modern React, **Custom Hooks** (`useAuth()`, `useFetch()`) have largely replaced HOCs for sharing stateful logic because hooks avoid "wrapper hell" (deeply nested component trees in React DevTools) and prevent prop name collisions. However, HOCs are still actively used for UI wrappers like `React.memo` or route-level permission guards.

### Q16: What is Code Splitting (Lazy Loading) / Partial Rendering in React, and how does it improve performance?
**A:** By default, bundlers like Vite or Webpack compile your entire React application into one massive JavaScript file. If a user visits the login page, they are forced to download the code for heavy admin dashboards and analytics charts they haven't even opened yet.
* **The Solution:** **Code Splitting** breaks that single bundle into smaller chunks that load on demand. Using **`React.lazy()`** alongside **`<Suspense>`**, you can dynamically import heavy components only when the user actually navigates to them or clicks a tab, displaying a lightweight loading spinner in the meantime. This drastically reduces initial page load times.
