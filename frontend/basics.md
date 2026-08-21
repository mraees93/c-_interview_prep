1. Is node.js single threaded?

Yes and it utilizes event-driven architecture and non-blocking I/O operations.

2. What kind of API functions are supported in node.js?

Ther are 2 api functions:

Synchronous: used for blocking code
Asynchronous: used for non-blocking code

3. What is the event loop?

A mechanism that handles many async tasks concurrently within a single thread.

4. Whats a promise?

A promise is an object that links **code that takes time (producing code)** AND **code that must wait for a result (consuming code)**.

5. How many states can a promise be in? 

A promise can be in 3 states; Pending, fulfilled, rejected

6. When is a promise settled?

When its either fulfilled or rejected.



7. Async programming explained.

Think non-blocking code

Thread pool: kitchen of chefs

1. current thread executes synchronously: chef alex starts preparing recipe
**JavaScript:** Begins with exactly one worker (the single main thread) handling the entire kitchen.

async method: complex recipe

2. await(set timer): add recipe in oven, set timer => await doesnt mean freeze, it means yield
**JavaScript**: Chef Alex hands the dish to an external smart appliance (Browser Web API or Node.js runtime) to track the time because he cannot track background tasks himself.

3. current thread becomes free: chef alex

4. control returns to the caller: waiter hands chef alex different async method or he picks one

5. runtime registers callback: timer on oven dings(callback telling kitchen that background task completed)
**JavaScript:** The smart appliance dings and places the finished dish onto a waiting line counter (the Callback/Task Queue).

remainder of method scheduled to run

6. different thread in thread pool: different chef hears the timer, pulls steak out of oven, plates it, and serves it (the continuation). 
**JavaScript:** The exact same chef (Chef Alex) must finish it. He cannot touch it instantly when it dings; he must completely finish his current synchronous task, empty his hands (clear the Call Stack), and then pull the dish from the waiting line counter.

The recipe completes perfectly


Async is not same as parallel:

Async programming is ONE chef prepping 3 dishes so they never stay idle.

c#: Parallel programming is TWO chefs chopping onions at the exact same time.
**Javascript:** Parallel programming is NON-EXISTENT for that single chef. To get parallel work, you must hire an entirely separate chef in a separate kitchen (using Web Workers or Worker Threads) who cannot share the same cutting board.

## 1. C# Parallel Programming
*   **The Mechanics:** Executes tasks simultaneously across multiple CPU cores via the CLR ThreadPool.
*   **Memory Profile:** Shared Memory Architecture. All threads access the identical managed heap space concurrently.
*   **The Risk:** High threat of race conditions and state corruption. Requires explicit locking strategies or concurrent collection wrappers.

## 2. JavaScript Parallel Programming (Node.js Worker Threads)
*   **The Mechanics:** Delegates CPU-heavy tasks to an isolated `Worker` execution context running a separate instance of the V8 runtime.
*   **Memory Profile:** Shared-Nothing Architecture. Workers possess completely isolated heaps and communication happens strictly via message-passing channels (`postMessage`).
*   **The Benefit:** Inherently safe from shared-memory cross-thread data mutation defects.


8. Synchronous programming explained: 

Think blocking code

ONE chef prepares 3 dishes, but refuses to start the next step until the current one is 100% complete. He puts the cake in the oven and stands there staring at the oven door for 45 minutes doing absolutely nothing else. The restaurant's waiters are frozen, customer orders pile up at the kitchen door, and the entire app becomes completely unresponsive until that cake is baked.