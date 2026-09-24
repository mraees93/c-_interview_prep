# 📡 React API Consumption & Performance Interview Guide

### Q1: What happens if the user types 10 letters in 1 second inside our filter box? How do we optimize it?
**A:** Right now, our `useMemo` array processor re-evaluates the entire 500-record array matrix on *every single keystroke*. While modern machines can handle 500 entries smoothly, typing rapidly can still waste valuable CPU cycles. 
* **The Optimization:** In a production-grade application, I would introduce an **input debounce mechanism** (typically using a 300ms window via a timer or a library like Lodash). This stalls processing until the user pauses their typing, executing the filter matrix exactly once instead of 10 times.

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
